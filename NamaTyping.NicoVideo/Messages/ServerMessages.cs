using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NamaTyping.NicoVideo.Messages
{
   

    public class RoomMessage : Message
    {
        public new RoomData Data { get; set; }
    }
}
