Imports System.Configuration

Partial Public Class ConsoleWindow

    Private Sub Button_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
    End Sub

    ''' <summary>
    ''' ユーザー設定ファイル (user.config) が保存されているフォルダを開き、ファイルを選択します。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OpenSettingsFolder(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Process.Start("explorer", "/select,""" & ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming).FilePath & """")
    End Sub
End Class
