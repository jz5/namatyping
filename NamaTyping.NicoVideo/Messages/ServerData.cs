using System;
using System.Collections.Generic;
using System.Text;

namespace NamaTyping.NicoVideo.Messages
{
    public class RoomData : Data
    {
        /// <summary>
        /// メッセージサーバー（コメントサーバー）
        /// </summary>
        public MessageServer MessageServer { get; set; }

        /// <summary>
        /// 部屋名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// メッセージサーバーのスレッドID
        /// </summary>
        public string ThreadId { get; set; }

        /// <summary>
        /// 先頭の部屋 (アリーナ) かどうか
        /// </summary>
        public bool IsFirst { get; set; }

        ///// <summary>
        ///// (互換性確保のためのダミー文字列)
        ///// </summary>
        //public string WaybackKey { get; set; }

        /// <summary>
        /// メッセージサーバーから受信するコメント（chatメッセージ）にyourpostフラグを付けるためのキー。threadメッセージのthreadkeyパラメータに設定する
        /// </summary>
        public string YourPostKey { get; set; }
    }

    public class MessageServer
    {
        public string Uri { get; set; }

        /// <summary>
        /// メッセージサーバの種類 (現在常に niwavided)
        /// </summary>
        public string Type { get; set; }
    }
}
