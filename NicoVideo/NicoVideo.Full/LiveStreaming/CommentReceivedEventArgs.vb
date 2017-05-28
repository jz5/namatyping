Namespace LiveStreaming

    Public Class CommentReceivedEventArgs
        Inherits EventArgs

        'Private _Error As Exception
        'Public ReadOnly Property [Error] As Exception
        'Get
        'Return _Error
        'End Get
        'End Property

        Private _CommentMessage As LiveCommentMessage
        Public ReadOnly Property Comment As LiveCommentMessage
            Get
                Return _CommentMessage
            End Get
        End Property

        Public Sub New(ByVal comment As LiveCommentMessage)
            _CommentMessage = comment
        End Sub

        'Public Sub New(ByVal message As LiveChatMessage, ByVal [error] As Exception)
        '_CommentMessage = message
        '_Error = [error]
        'End Sub

    End Class

End Namespace
