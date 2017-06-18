Partial Public Class ConsoleWindow

    ''' <summary>
    ''' 日時を表示する際の書式。
    ''' </summary>
    Private Const DateTimeFormat = "yyyy-MM-dd HH:mm"

    Private Sub ConsoleWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        ShowSubstitutionListUpdated()
    End Sub

    Private Sub Button_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
    End Sub

    ''' <summary>
    ''' ユーザー設定ファイル (user.config) が保存されているフォルダを開き、ファイルを選択します。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OpenSettingsFolder(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Process.Start("explorer", "/select,""" & My.Settings.FilePath & """")
    End Sub

    ''' <summary>
    ''' NGワード置換ファイルを更新します。
    ''' </summary>
    Private Sub DownloadSubstitutionList(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Model.Lyrics.CharacterReplacer.Download()
        ShowSubstitutionListUpdated()
    End Sub


    ''' <summary>
    ''' NGワード置換ファイルの更新日時を表示します。
    ''' </summary>
    Private Sub ShowSubstitutionListUpdated()
        Dim replacer = Model.Lyrics.CharacterReplacer
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
