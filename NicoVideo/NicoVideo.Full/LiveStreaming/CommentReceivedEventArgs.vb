Namespace LiveStreaming

    Public Class CommentReceivedEventArgs
        Inherits EventArgs

        'Private _Error As Exception
        'Public ReadOnly Property [Error] As Exception
        'Get
        'Return _Error
        'End Get
        'End Property

        Public ReadOnly Property Comment As LiveCommentMessage

        Public Sub New(comment As LiveCommentMessage)
            Me.Comment = comment
        End Sub

        'Public Sub New(ByVal message As LiveChatMessage, ByVal [error] As Exception)
        '_CommentMessage = message
        '_Error = [error]
        'End Sub
    End Class

End Namespace
