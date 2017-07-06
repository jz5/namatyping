Imports System.Xml
Imports System.ServiceModel.Syndication

Class Application

    ' Startup、Exit、DispatcherUnhandledException などのアプリケーション レベルのイベントは、
    ' このファイルで処理できます。

    Private WithEvents ViewModel As ViewModel.MainViewModel
    Private WithEvents ScreenWindow As ScreenWindow
    Private WithEvents ConsoleWindow As ConsoleWindow
    Private WithEvents ScoringResultWindow As ScoringResultWindow

    Protected Overrides Sub OnStartup(ByVal e As StartupEventArgs)
        MyBase.OnStartup(e)

        If Not System.Diagnostics.Debugger.IsAttached Then
            ' 「デバッグなしで開始」していれば
            AddHandler DispatcherUnhandledException, AddressOf Application_DispatcherUnhandledException
            AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException
        End If

        ViewModel = New ViewModel.MainViewModel

        ScreenWindow = New ScreenWindow
        ScreenWindow.DataContext = ViewModel
        ScreenWindow.ScreenControl.DataContext = ViewModel
        ScreenWindow.Topmost = My.Settings.Topmost
        If Not Double.IsNaN(My.Settings.WindowLeft) Then
            ScreenWindow.Left = My.Settings.WindowLeft
        End If
        If Not Double.IsNaN(My.Settings.WindowTop) Then
            ScreenWindow.Top = My.Settings.WindowTop
        End If

        ViewModel.Player = ScreenWindow.Player
        ViewModel.Volume = My.Settings.Volume

        ViewModel.Dispatcher = ScreenWindow.Dispatcher
        ScreenWindow.Show()


#If DEBUG Then
        For i = 0 To 30
            ViewModel.Messages.Add(i.ToString)
            ViewModel.RankedUsers.Add(New User With {.Rank = i, .RawScore = i, .Name = i.ToString})
        Next

#End If

        '' http://feeds.feedburner.com/bingimages

        'Dim path = "E:\ニコ生タイピング\複数テスト supercell\初音ミク　が　オリジナル曲を歌ってくれたよ「メルト」\メルト.xml"
        'Dim reader As XmlReader = XmlReader.Create(path)
        'Dim feed = SyndicationFeed.Load(reader)

        'For Each i In feed.Items
        '    Dim found = False
        '    For Each l In i.Links
        '        If l.MediaType = "image/jpeg" OrElse l.MediaType = "image/png" OrElse l.MediaType = "image/bmp" Then
        '            ViewModel.BackgroundImage = l.Uri
        '            found = True
        '            Exit For
        '        ElseIf l.MediaType = "video/x-ms-wmv" Then
        '            ViewModel.BackgroundImage = Nothing
        '            ViewModel.Player.Source = l.Uri

        '            If Not l.Uri.IsAbsoluteUri Then
        '                ViewModel.Player.Source = New Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), l.Uri.ToString))
        '            End If


        '            ViewModel.Player.Play()
        '            found = True
        '            Exit For
        '        End If
        '    Next
        '    If found Then
        '        Exit For
        '    End If
        'Next


    End Sub

    Private Sub ConsoleWindow_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles ConsoleWindow.Closed
        ConsoleWindow = Nothing
    End Sub

    Private Sub ScreenWindow_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles ScreenWindow.Closed
        SaveSettings()

        ScreenWindow = Nothing
        If ConsoleWindow IsNot Nothing Then
            ConsoleWindow.Close()
        End If
        If ScoringResultWindow IsNot Nothing Then
            ScoringResultWindow.Close()
        End If
        ViewModel.Disconnect()
    End Sub

    Private Sub ViewModel_MessageAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles ViewModel.MessageAdded
        If ScreenWindow Is Nothing Then
            Exit Sub
        End If
        ScreenWindow.ScrollMessages()
    End Sub


    Private Sub ViewModel_RankingAdded(ByVal sender As Object, ByVal e As System.EventArgs) Handles ViewModel.RankingAdded
        If ScreenWindow Is Nothing Then
            Exit Sub
        End If
        ScreenWindow.ScrollRanking()
    End Sub

    Private Sub ViewModel_ShowResults(ByVal sender As Object, ByVal e As System.EventArgs) Handles ViewModel.ShowResults

        If ScoringResultWindow Is Nothing Then
            ScoringResultWindow = New ScoringResultWindow
            ScoringResultWindow.DataContext = ViewModel.RankedUsers
            ScoringResultWindow.Show()
        Else
            ScoringResultWindow.Activate()
        End If

    End Sub

    Private Sub ViewModel_ShowSettings(ByVal sender As Object, ByVal e As System.EventArgs) Handles ViewModel.ShowSettings

        If ConsoleWindow Is Nothing Then
            ConsoleWindow = New ConsoleWindow
            ConsoleWindow.DataContext = ViewModel
            ConsoleWindow.Show()

        Else
            ConsoleWindow.Activate()
        End If

    End Sub

    Private Sub ScoringResultWindow_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles ScoringResultWindow.Closed
        ScoringResultWindow = Nothing
    End Sub

    Private MessagesGridHeight As Double

    Private Sub ViewModel_Played(ByVal sender As Object, ByVal e As System.EventArgs) Handles ViewModel.Played
    End Sub

    Private Sub ViewModel_Stopped(ByVal sender As Object, ByVal e As System.EventArgs) Handles ViewModel.Stopped
    End Sub

    ''' <summary>
    ''' ユーザー設定を保存します。
    ''' </summary>
    Protected Sub SaveSettings()
        My.Settings.Topmost = ScreenWindow.Topmost
        My.Settings.ShowNameEntryMessages = ViewModel.ShowNameEntryMessages
        My.Settings.ShowPointMessages = ViewModel.ShowPointMessages
        My.Settings.ShowFilteredMessages = ViewModel.ShowFilteredMessages
        My.Settings.DisplayCommentPattern = ViewModel.DisplayCommentPattern
        My.Settings.HighlightUsers = ViewModel.HighlightUsers
        My.Settings.MessageFontSize = ViewModel.MessageFontSize
        My.Settings.LyricFontSize = ViewModel.LyricFontSize
        My.Settings.RankingFontSize = ViewModel.RankingFontSize
        My.Settings.BottomGridOpacity = ViewModel.BottomGridOpacity
        My.Settings.RecentLyricLineCount = ViewModel.RecentLyricLineCount
        My.Settings.Volume = ViewModel.Volume
        My.Settings.WindowSizePattern = ViewModel.WindowSizePattern
        My.Settings.ConnectAllCommentServers = ViewModel.ConnectAllCommentServers

        My.Settings.WindowLeft = ScreenWindow.Left
        My.Settings.WindowTop = ScreenWindow.Top

        My.Settings.Save()
    End Sub

    ''' <summary>
    ''' WPF UIスレッドにおける未処理例外のイベントハンドラ。
    ''' </summary>
    Private Sub Application_DispatcherUnhandledException(sender As Object, e As System.Windows.Threading.DispatcherUnhandledExceptionEventArgs)
        e.Handled = True
        ShowExceptionAndShutdown(e.Exception)
    End Sub

    ''' <summary>
    ''' WPF UIスレッド以外における未処理例外のイベントハンドラ。
    ''' </summary>
    Private Sub CurrentDomain_UnhandledException(sender As Object, e As UnhandledExceptionEventArgs)
        ShowExceptionAndShutdown(CType(e.ExceptionObject, Exception))
    End Sub

    ''' <summary>
    ''' 異常終了を表すコード。
    ''' </summary>
    Private Const FailureExitCode = 1

    ''' <summary>
    ''' 例外情報を表示し、アプリケーションを終了します。
    ''' </summary>
    Private Sub ShowExceptionAndShutdown(ex As Exception)
        MessageBox.Show(
            $"問題が発生したため、アプリケーションを終了します。{vbNewLine}{vbNewLine}例外情報 (Ctrl+Cでコピー可能):{vbNewLine}{ex}",
            My.Application.Info.Title,
            MessageBoxButton.OK,
            MessageBoxImage.Error
        )
        Shutdown(FailureExitCode)
    End Sub

End Class
