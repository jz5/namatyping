using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace NamaTyping.NicoVideo.Comments
{
    // {"chat":{"thread":"xxx","no":173,"vpos":131432,
    // "date":1605704227,"date_usec":590855,"mail":"184",
    // "user_id":"xxx","anonymity":1,"content":"xxx"}}

    public class LiveCommentMessage : Message
    {
        private class ChatEnvelope
        {
            [JsonProperty("chat")]
            public LiveCommentMessage Chat { get; set; }
        }

        [JsonProperty("thread")]
        public string Thread { get; set; }

        [JsonProperty("no")]
        public int No { get; set; }

        [JsonProperty("vpos")]
        public long VPos { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("date_usec")]
        public long DateUsec { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("mail")]
        public string Mail { get; set; }

        [JsonProperty("anonymity")]
        public int Anonymity { get; set; }

        [JsonProperty("premium")]
        public int? Premium { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("deleted")]
        public int Deleted { get; set; }


        public ChatSource Source { get; set; }
        public string Text => Content; // 互換性保持

        private DateTime? _dateTime;
        public DateTime DateTime
        {
            get
            {
                if (_dateTime != null)
                {
                    return _dateTime.Value;
                }

                var dateTime = new DateTime(1970, 1, 1, 0, 0, 0,
                    DateTimeKind.Utc).AddSeconds(Date);
                _dateTime = dateTime.AddMilliseconds(DateUsec / 1000.0);

                return _dateTime.Value;
            }
        }

        public static LiveCommentMessage Create(string json)
        {
            var message =  JsonConvert.DeserializeObject<ChatEnvelope>(json)?.Chat;

            message.Source = Enum.TryParse<ChatSource>($"{message.Premium}", out var chatSource) ? chatSource : ChatSource.General;

            return message;
        }
    }
}
