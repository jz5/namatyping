namespace NamaTyping.NicoVideo.Messages
{
    public class MessageServerData : Data
    {
        /// <summary>
        /// メッセージサーバの接続先 URI
        /// </summary>
        public string ViewUri { get; set; }

        /// <summary>
        /// vpos を計算する基準（vpos = 0）となる ISO8601 形式の時刻
        /// </summary>
        public string VposBaseTime { get; set; }

        /// <summary>
        /// 匿名コメント投稿時に用いられる自身のユーザ ID（ログインユーザのみ取得可能）自身が投稿したコメントかどうかを判別する上で使用可能です。
        /// </summary>
        public string HashedUserId { get; set; }

    }
}
