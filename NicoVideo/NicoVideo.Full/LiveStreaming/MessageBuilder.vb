Namespace LiveStreaming

    Friend NotInheritable Class MessageBuilder

        Public Shared Function BuildMessage(ByVal value As String) As Message

            If value.StartsWith("<chat ") Then
                Return New LiveCommentMessage(value)
            ElseIf value.StartsWith("<chat_result") Then
                Return New LiveCommentResultMessage(value)
            ElseIf value.StartsWith("<thread") Then
                Return New LiveThreadMessage(value)
            Else
                Return Nothing
            End If

        End Function

        Private Sub New()
        End Sub

    End Class

End Namespace