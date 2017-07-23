Imports System.Text
Imports Pronama.NamaTyping.Model

Partial Public Class LyricWindow

    Public Property Lyrics As Lyrics

    Private Sub LyricWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Reload()
        Title = Lyrics.Title
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Reload()
    End Sub

    Private Sub Reload()
        If Lyrics Is Nothing Then
            Exit Sub
        End If

        Lyrics.Reload()

        Dim sb = New StringBuilder
        For Each l In Lyrics.Lines
            For Each y In l.Yomi
                sb.Append(y & vbCrLf)
            Next
        Next

        LyricTextBlock.Text = sb.ToString
    End Sub

End Class
