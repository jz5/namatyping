using System;
using Newtonsoft.Json;

namespace NamaTyping.NicoVideo.Comments
{
    public class LiveThreadMessage : Message
    {
        private class ThreadEnvelope
        {
            [JsonProperty("chat")]
            public LiveThreadMessage Thread { get; set; }
        }

        [JsonProperty("resultcode")]
        public int ResultCode { get; set; }

        [JsonProperty("thread")]
        public string Thread { get; set; }

        [JsonProperty("server_time")]
        public long ServerTime { get; set; }

        [JsonProperty("last_res")]
        public long LastRes { get; set; }

        [JsonProperty("ticket")]
        public string Ticket { get; set; }

        [JsonProperty("revision")]
        public int Revision { get; set; }

        private DateTime? _dateTime;
        public DateTime DateTime
        {
            get
            {
                if (_dateTime != null)
                {
                    return _dateTime.Value;
                }

                _dateTime = new DateTime(1970, 1, 1, 0, 0, 0,
                    DateTimeKind.Utc).AddSeconds(ServerTime);

                return _dateTime.Value;
            }
        }

        public static LiveThreadMessage Create(string json)
        {
            return JsonConvert.DeserializeObject<ThreadEnvelope>(json)?.Thread;
        }
    }
}
