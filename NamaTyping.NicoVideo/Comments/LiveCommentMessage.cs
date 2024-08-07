using Dwango.Nicolive.Chat.Data;
using Dwango.Nicolive.Chat.Service.Edge;
using System;

namespace NamaTyping.NicoVideo.Comments;

// {"chat":{"thread":"xxx","no":173,"vpos":131432,
// "date":1605704227,"date_usec":590855,"mail":"184",
// "user_id":"xxx","anonymity":1,"content":"xxx"}}

public class LiveCommentMessage
{
    public int No { get; set; }
    public long VPos { get; set; }
    public string UserId { get; set; }
    public int? Premium { get; set; }
    public string Content { get; set; }
    public ChatSource Source { get; set; }
    public string Text => Content; // 互換性保持
    public DateTime DateTime { get; set; }

    public static LiveCommentMessage Create(ChunkedMessage chunkedMessage)
    {
        switch (chunkedMessage.PayloadCase)
        {
            case ChunkedMessage.PayloadOneofCase.Message
                when chunkedMessage.Message.Chat != null:
                {
                    var chat = chunkedMessage.Message.Chat;
                    return new LiveCommentMessage
                    {
                        No = chat.No,
                        VPos = chat.Vpos,
                        UserId = chat.HasRawUserId ? chat.RawUserId.ToString() : chat.HashedUserId,
                        Premium = chat.AccountStatus == Chat.Types.AccountStatus.Premium ? 1 : null,
                        Content = chat.Content,
                        DateTime = chunkedMessage.Meta.At.ToDateTimeOffset().DateTime,
                        Source = ChatSource.General
                    };
                }
            case ChunkedMessage.PayloadOneofCase.State
                when !string.IsNullOrWhiteSpace(chunkedMessage.State.Marquee?.Display?.OperatorComment?.Content):
                // 運営コメント
                return new LiveCommentMessage
                {
                    No = 0,
                    VPos = 0,
                    UserId = "",
                    Premium = null,
                    Content = chunkedMessage.State.Marquee?.Display?.OperatorComment?.Content,
                    DateTime = chunkedMessage.Meta.At.ToDateTimeOffset().DateTime,
                    Source = ChatSource.Broadcaster
                };
            case ChunkedMessage.PayloadOneofCase.State
                when chunkedMessage.State.ProgramStatus?.State == ProgramStatus.Types.State.Ended:
                // 状態付きメッセージの放送状態
                return new LiveCommentMessage
                {
                    No = 0,
                    VPos = 0,
                    UserId = "",
                    Premium = null,
                    Content = "/disconnect",
                    DateTime = chunkedMessage.Meta.At.ToDateTimeOffset().DateTime,
                    Source = ChatSource.Operator
                };
            case ChunkedMessage.PayloadOneofCase.State:
                break;
            default:
                return null;
        }

        return null;
    }
}