namespace NamaTyping.NicoVideo.OAuth
{
    /// <summary>
    /// ニコ動 OAuth API レスポンス
    /// </summary>
    public class Response
    {
        public Meta Meta { get; set; }
        public virtual Data Data { get; set; }
    }


    public class WebSocketEndpoint : Response
    {
        public new WebSocketEndpointData Data { get; set; }
    }

}
