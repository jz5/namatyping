Partial Public Class LyricWindow

    Public Property Lyrics As Model.Lyrics

    Private Sub LyricWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Reload()
        Me.Title = Lyrics.Title
    End Sub

    Private Sub Button_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Reload()
    End Sub

    Private Sub Reload()
        If Lyrics Is Nothing Then
            Exit Sub
        End If

        Lyrics.Reload()

        Dim sb = New System.Text.StringBuilder
        For Each l In Lyrics.Lines
            For Each y In l.Yomi
                sb.Append(y & vbCrLf)
            Next
        Next

        LyricTextBlock.Text = sb.ToString
    End Sub

End Class
