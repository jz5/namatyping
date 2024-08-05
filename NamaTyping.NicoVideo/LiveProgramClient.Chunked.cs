using Dwango.Nicolive.Chat.Service.Edge;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using NamaTyping.NicoVideo.Comments;
using NamaTyping.NicoVideo.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NamaTyping.NicoVideo;

public partial class LiveProgramClient
{

    public MessageServerMessage MessageServerMessage { get; private set; }

    public bool Connected { get; private set; }


    /// <summary>
    /// メッセージID（重複受信チェック用）
    /// </summary>
    private readonly HashSet<string> _messageIds = new();




    public void Disconnect()
    {

    }

    public void ConnectMessageServer()
    {
        _messageIds.Clear();

        var retriever = new Retriever();
        //var entryParser = new MessageParser<ChunkedEntry>(() => new ChunkedEntry());
        //var messageParser = new MessageParser<ChunkedMessage>(() => new ChunkedMessage());

        var at = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var uri = MessageServerMessage.Data.ViewUri;

        //_ = Task.Run(async () =>
        //{
        //    try
        //    {
        //        await FetchForwardPlaylistMessagesAsync(retriever, entryParser, messageParser, uri, at);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //});


        /*
        // Create WebSocket
        _messageServerSocket = new ClientWebSocket();
        _messageServerCancellationTokenSource = new CancellationTokenSource();

        // Connect to message server
        var uri = new Uri(MessageServerMessage.Data.MessageServer.Uri);
        await _messageServerSocket.ConnectAsync(uri, _messageServerCancellationTokenSource.Token);
        MessageServerConnectionStateChanged?.Invoke(this, EventArgs.Empty);

        // Send 
        var thread = $@"{{""thread"":{{""thread"":""{MessageServerMessage.Data.ThreadId}"",""version"":""20061206"",""user_id"":""{_userId}"",""res_from"":{resFrom},""with_global"":1,""scores"":1,""nicoru"":0,""threadkey"":""{MessageServerMessage.Data.YourPostKey}""}}}}";

        var firstSegment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(thread));
        await _messageServerSocket.SendAsync(firstSegment, WebSocketMessageType.Text, true, _messageServerCancellationTokenSource.Token);


#pragma warning disable 4014
        Task.Run(async () =>
#pragma warning restore 4014
        {
            try
            {

                while (true)
                {
                    var buffer = new byte[4096];
                    var segment = new ArraySegment<byte>(buffer);

                    // Receive
                    var result = await _messageServerSocket.ReceiveAsync(segment, _messageServerCancellationTokenSource.Token);

                    // Break loop when closed
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _messageServerSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", _messageServerCancellationTokenSource.Token);
                        MessageServerConnectionStateChanged?.Invoke(this, EventArgs.Empty);
                        break;
                    }


                    // Read to end of message
                    var count = result.Count;
                    while (!result.EndOfMessage)
                    {
                        segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        result = await _messageServerSocket.ReceiveAsync(segment, _messageServerCancellationTokenSource.Token);

                        count += result.Count;
                    }

                    // Create message
                    var json = Encoding.UTF8.GetString(buffer, 0, count);

                    var message = MessageBuilder.BuildMessage(json);
                    if (message is LiveCommentMessage liveCommentMessage)
                    {
                        if (_maxCommentNo >= liveCommentMessage.No) // 再接続時、重複して同じコメントを受信した場合無視
                            continue;

                        _maxCommentNo = liveCommentMessage.No;
                        CommentReceived?.Invoke(this, new CommentReceivedEventArgs(liveCommentMessage));
                    }

                }
            }
            catch (WebSocketException socketException)
            {
                //Console.WriteLine(socketException);
                MessageServerConnectionStateChanged?.Invoke(this, EventArgs.Empty);
            }

        });
        */


    }

    private async Task FetchForwardPlaylistMessagesAsync(
        Retriever retriever,
        MessageParser<ChunkedEntry> entryParser,
        MessageParser<ChunkedMessage> messageParser,
        string uri,
        long from
    )
    {
        Connected = true;
        MessageServerConnectionStateChanged?.Invoke(this, EventArgs.Empty);

        var initialPhase = true;
        var next = from;
        while (true)
        {
            try
            {
                await foreach (var entry in retriever.RetrieveAsync($"{uri}?at={next}", entryParser))
                {
                    Console.WriteLine($"ChunkedEntry: {entry}"); // メッセージの内容を表示

                    if (entry.EntryCase == ChunkedEntry.EntryOneofCase.Backward && initialPhase)
                    {

                    }
                    else if (entry.EntryCase == ChunkedEntry.EntryOneofCase.Previous && initialPhase)
                    {
                        await PullMessage(retriever, messageParser, entry.Previous.Uri);
                    }
                    else if (entry.EntryCase == ChunkedEntry.EntryOneofCase.Segment)
                    {
                        await SleepUntil(entry.Segment.From, 1000);
                        _ = PullMessage(retriever, messageParser, entry.Segment.Uri);
                    }
                    else if (entry.EntryCase == ChunkedEntry.EntryOneofCase.Next)
                    {
                        Console.WriteLine($"Next: {entry.Next.At}");
                        next = entry.Next.At;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error: {e.Message}");
                break;
            }
            initialPhase = false;
        }


        Connected = false;
        MessageServerConnectionStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task SleepUntil(Timestamp timestamp, int prefetch = 0)
    {
        var until = timestamp.Seconds * 1000 + timestamp.Nanos / 1000_000;
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (until <= now)
            return;

        await Task.Delay((int)(until - now - prefetch));
    }


    private async Task PullMessage(
        Retriever retriever,
        MessageParser<ChunkedMessage> parser,
        string uri)
    {
        try
        {
            await foreach (var message in retriever.RetrieveAsync(uri, parser))
            {

                if (message.PayloadCase == ChunkedMessage.PayloadOneofCase.Message)
                {
                    var at = message.Meta.At.ToDateTimeOffset();
                    Console.WriteLine($"{at}: {message.Message.Chat.Vpos}: {message.Message.Chat.Content}");

                    if (_messageIds.Contains(message.Meta.Id))
                        continue;

                    _messageIds.Add(message.Meta.Id);

                    // コメント通知
                    var comment = LiveCommentMessage.Create(message);
                    if (comment != null)
                    {
                        CommentReceived?.Invoke(this, new CommentReceivedEventArgs(comment));
                    }


                }
                else
                {
                    Console.WriteLine($"{message.PayloadCase}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unexpected error: {e.Message}");
        }
    }

}