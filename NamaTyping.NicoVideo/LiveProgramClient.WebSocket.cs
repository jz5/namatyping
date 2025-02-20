using NamaTyping.NicoVideo.Messages;
using NamaTyping.NicoVideo.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Message = NamaTyping.NicoVideo.Messages.Message;

namespace NamaTyping.NicoVideo;

public partial class LiveProgramClient
{

    public WebSocketState ServerSocketState => _serverSocket?.State ?? WebSocketState.None;

    private string _webSocketUrl;

    private ClientWebSocket _serverSocket;
    private CancellationTokenSource _serverCancellationTokenSource;

    /// <summary>
    /// OAuth API で WebSocket URL を取得する
    /// </summary>
    /// <returns></returns>
    public async Task<WebSocketEndpoint> GetWebSocketEndpointAsync()
    {

        var url = $"https://api.live2.nicovideo.jp/api/v1/wsendpoint?nicoliveProgramId={_liveId}&userId={_userId}";

        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("ContentType", "application/json");
        request.Headers.Add("Authorization", "Bearer " + _accessToken);
        var response = await client.SendAsync(request);

        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(json))
        {
            // MEMO: Unauthorized 
            return null;
        }

        var webSocketEndpoint = JsonConvert.DeserializeObject<WebSocketEndpoint>(json);
        if (webSocketEndpoint?.Meta.Status == 200)
        {
            _webSocketUrl = webSocketEndpoint.Data.Url;
        }

        return webSocketEndpoint;
    }

    /// <summary>
    /// StartWatching
    /// </summary>
    /// <returns></returns>
    public async Task StartWatchingAsync()
    {
        // User-Agent を設定できるようにする
        // https://stackoverflow.com/questions/22635663/setting-user-agent-http-header-in-clientwebsocket
        // Use reflection to remove IsRequestRestricted from headerInfo hash table
        var a = typeof(HttpWebRequest).Assembly;
        foreach (var f in a.GetType("System.Net.HeaderInfoTable").GetFields(BindingFlags.NonPublic | BindingFlags.Static))
        {
            if (f.Name != "HeaderHashTable")
                continue;

            if (!(f.GetValue(null) is Hashtable hashTable))
                continue;

            foreach (string sKey in hashTable.Keys)
            {
                var headerInfo = hashTable[sKey];
                //Console.WriteLine(String.Format("{0}: {1}", sKey, hashTable[sKey]));
                foreach (var g in a.GetType("System.Net.HeaderInfo")
                             .GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (g.Name != "IsRequestRestricted")
                        continue;

                    var b = (bool)g.GetValue(headerInfo);
                    if (b)
                    {
                        g.SetValue(headerInfo, false);
                        //Console.WriteLine(sKey + "." + g.Name + " changed to false");
                    }
                }
            }
        }

        // Create WebSocket
        _serverSocket = new ClientWebSocket();
        _serverCancellationTokenSource = new CancellationTokenSource();

        // Set User-Agent（ニコ動 API 使用には必須）
        _serverSocket.Options.SetRequestHeader("User-Agent", "NamaTyping");

        // Connect
        var uri = new Uri(_webSocketUrl);
        await _serverSocket.ConnectAsync(uri, _serverCancellationTokenSource.Token);
        ServerConnectionStateChanged?.Invoke(this, EventArgs.Empty);

        // MEMO: ニコ生タイピングでは視聴しないため、視聴用の設定は固定
        var startWatchingData = @"{""type"":""startWatching"",""data"":{""stream"":{""quality"":""abr"",""latency"":""low""}}}";

        var firstSegment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(startWatchingData));
        await _serverSocket.SendAsync(firstSegment, WebSocketMessageType.Text, true, _serverCancellationTokenSource.Token);



#pragma warning disable 4014
        Task.Run(async () =>
#pragma warning restore 4014
        {
            try
            {
                while (true)
                {
                    var buffer = new byte[4096 * 10];

                    // Receive
                    var segment = new ArraySegment<byte>(buffer);
                    var result = await _serverSocket.ReceiveAsync(segment, CancellationToken.None);

                    // Break loop when closed
                    // MEMO: ニコ生タイピングではこの接続を維持する必要はない。応答しなければ切断される
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _serverSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);
                        ServerConnectionStateChanged?.Invoke(this, EventArgs.Empty);
                        break;
                    }

                    // Read to end of message
                    var count = result.Count;
                    while (!result.EndOfMessage)
                    {
                        segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        result = await _serverSocket.ReceiveAsync(segment, CancellationToken.None);

                        count += result.Count;
                    }

                    // Get message
                    // MEMO: ニコ生タイピングでは、メッセージ（コメント）サーバーの情報さえ取得できればよい
                    var json = Encoding.UTF8.GetString(buffer, 0, count);
                    var message = JsonConvert.DeserializeObject<Message>(json);

                    if (message?.Type == "messageServer")
                    {
                        MessageServerMessage = JsonConvert.DeserializeObject<MessageServerMessage>(json);
                    }
                    MessageReceived?.Invoke(this, new MessageEventArgs(message));
                }
            }
            catch (WebSocketException)
            {
                //Console.WriteLine(socketException);
                ServerConnectionStateChanged?.Invoke(this, EventArgs.Empty);
            }
        });
    }

}