Imports Pronama.NamaTyping.Model

Partial Public Class ConsoleWindow

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Close()
    End Sub

    ''' <summary>
    ''' ユーザー設定ファイル (user.config) が保存されているフォルダを開き、ファイルを選択します。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OpenSettingsFolder(sender As Object, e As RoutedEventArgs)
        Process.Start("explorer", "/select,""" & My.Settings.FilePath & """")
    End Sub

End Class
