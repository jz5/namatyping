namespace NamaTyping.NicoVideo.Comments
{
    public class MessageBuilder
    {
        public static Message BuildMessage(string json)
        {
            if (json.StartsWith(@"{""chat"""))
            {
                return LiveCommentMessage.Create(json);
            }
            if (json.StartsWith(@"{""thread"""))
            {
                return LiveThreadMessage.Create(json);
            }
            

            return null;
        }
    }
}
