Imports Pronama.NamaTyping.Model

Partial Public Class ConsoleWindow

    ''' <summary>
    ''' 日時を表示する際の書式。
    ''' </summary>
    Private Const DateTimeFormat = "yyyy-MM-dd HH:mm"

    Private Sub ConsoleWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        ShowSubstitutionListUpdated()
    End Sub

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

    ''' <summary>
    ''' NGワード置換ファイルを更新します。
    ''' </summary>
    Private Sub DownloadSubstitutionList(sender As Object, e As RoutedEventArgs)
        Lyrics.CharacterReplacer.Download()
        ShowSubstitutionListUpdated()
    End Sub


    ''' <summary>
    ''' NGワード置換ファイルの更新日時を表示します。
    ''' </summary>
    Private Sub ShowSubstitutionListUpdated()
        Dim replacer = Lyrics.CharacterReplacer
        BuiltinUpdated.Text = replacer.BultinUpdated.ToString(DateTimeFormat)
        If replacer.Updated <> Nothing Then
            Updated.Text = replacer.Updated.ToString(DateTimeFormat)
            Dim oldStyle = CType(BuiltinUpdated.FindResource("old"), Style)
            If replacer.Updated > replacer.BultinUpdated Then
                BuiltinUpdated.Style = oldStyle
                Updated.Style = Nothing
            Else
                BuiltinUpdated.Style = Nothing
                Updated.Style = oldStyle
            End If
        End If
    End Sub

End Class
