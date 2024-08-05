namespace NamaTyping.NicoVideo.Messages;

/// <summary>
/// ニコ動 API メッセージ
/// </summary>
public class Message
{
    public string Type { get; set; }
    public virtual Data Data { get; set; }
}