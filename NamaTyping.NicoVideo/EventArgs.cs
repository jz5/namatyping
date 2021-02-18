using System;
using NamaTyping.NicoVideo.Comments;
using Message = NamaTyping.NicoVideo.Messages.Message;

namespace NamaTyping.NicoVideo
{
    public class MessageEventArgs : EventArgs
    {
        public Message Message { get; set; }

        public MessageEventArgs(Message message)
        {
            Message = message;
        }
    }

    public class CommentReceivedEventArgs : EventArgs
    {
        public LiveCommentMessage Comment { get; }

        public CommentReceivedEventArgs(LiveCommentMessage comment)
        {
            Comment = comment;
        }
    }
}
