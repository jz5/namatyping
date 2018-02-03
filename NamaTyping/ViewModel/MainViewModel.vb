Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Threading
Imports Pronama.NamaTyping.Model
Imports Microsoft.Win32
Imports Pronama.NicoVideo.LiveStreaming
Imports Pronama.NicoVideo.LiveStreaming.CommunityChannelRoom
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Imports System.Runtime.InteropServices
Imports System.Text
Imports Pronama.NicoVideo

Namespace ViewModel

    Public Class MainViewModel
        Inherits ViewModelBase

        Public Property Dispatcher As Dispatcher

        ''' <summary>
        ''' 運営NGワードの強調色。
        ''' </summary>
        Private ReadOnly _blacklistCharactersHighlightColor As Brush = Brushes.Red

        ''' <summary>
        ''' <see cref="MediaElement"/>で Media 読み込み時、対応していないメディア形式だった場合に
        ''' 発生する<see cref="MediaElement.MediaFailed"/>における<see cref="COMException.ErrorCode"/>の値。
        ''' </summary>
        ''' <remarks>
        ''' 正式な定数名は不明。
        ''' </remarks>
        Private Const MilaverrLoadfailed = &HC00D11B1

        ''' <summary>
        ''' <see cref="MediaElement.SpeedRatio"/>設定時、速度変更に対応していないメディア形式だった場合に
        ''' 発生する<see cref="MediaElement.MediaFailed"/>における<see cref="COMException.ErrorCode"/>の値。
        ''' </summary>
        Private Const MilaverrUnexpectedwmpfailure = &H8898050C

#Region "Properties"


        Public ReadOnly Property RecentLyrics As ObservableCollection(Of WipeTextBlock) = New ObservableCollection(Of WipeTextBlock)

        Public ReadOnly Property Messages As ObservableCollection(Of String) = New ObservableCollection(Of String)

        Private _liveProgramIdText As String = ""
        Public Property LiveProgramIdText As String
            Get
                Return _liveProgramIdText
            End Get
            Set
                _liveProgramIdText = Value
                OnPropertyChanged("LiveProgramIdText")
            End Set
        End Property

        Protected ReadOnly Property LiveProgramId As String
            Get
                Dim match = Regex.Match(LiveProgramIdText, "(?<id>lv\d+)")
                If match.Success Then
                    Return match.Groups("id").Value
                Else
                    Return ""
                End If
            End Get
        End Property

        Private _isExampleMode As Boolean
        Public Property IsExampleMode As Boolean
            Get
                Return _isExampleMode
            End Get
            Set
                _isExampleMode = Value
                OnPropertyChanged("IsExampleMode")
            End Set
        End Property

        Private _showNameEntryMessages As Boolean = My.Settings.ShowNameEntryMessages
        Public Property ShowNameEntryMessages As Boolean
            Get
                Return _showNameEntryMessages
            End Get
            Set
                _showNameEntryMessages = Value
                OnPropertyChanged("ShowNameEntryMessages")
            End Set
        End Property

        Private _showPointMessages As Boolean = My.Settings.ShowPointMessages
        Public Property ShowPointMessages As Boolean
            Get
                Return _showPointMessages
            End Get
            Set
                _showPointMessages = Value
                OnPropertyChanged("ShowPointMessages")
            End Set
        End Property

        Private _showFilteredMessages As Boolean = My.Settings.ShowFilteredMessages
        Public Property ShowFilteredMessages As Boolean
            Get
                Return _showFilteredMessages
            End Get
            Set
                _showFilteredMessages = Value
                OnPropertyChanged("ShowFilteredMessages")
            End Set
        End Property

        Private _showOtherMessages As Boolean
        Public Property ShowOtherMessages As Boolean
            Get
                Return _showOtherMessages
            End Get
            Set
                _showOtherMessages = Value
                OnPropertyChanged("ShowOtherMessages")
            End Set
        End Property

        Private _displayCommentPattern As String = My.Settings.DisplayCommentPattern
        Public Property DisplayCommentPattern As String
            Get
                Return _displayCommentPattern
            End Get
            Set
                _displayCommentPattern = Value
                OnPropertyChanged("DisplayCommentPattern")
            End Set
        End Property

        Private _backgroundImage As Uri
        Public Property BackgroundImage As Uri
            Get
                If _lyrics IsNot Nothing AndAlso _lyrics.ImageFileName <> "" Then
                    _backgroundImage = New Uri(_lyrics.ImageFileName)
                    Return _backgroundImage
                Else
                    Return _backgroundImage
                End If
            End Get
            Set
                _backgroundImage = Value
                OnPropertyChanged("BackgroundImage")
            End Set
        End Property


        Private _statusMessage As String
        Public Property StatusMessage As String
            Get
                Return _statusMessage
            End Get
            Set
                If Value = _statusMessage Then
                    _statusMessage = ""
                    OnPropertyChanged("StatusMessage")
                End If
                _statusMessage = Value
                OnPropertyChanged("StatusMessage")
            End Set
        End Property

        'Private _Presecond As Double = 0.5
        'Public Property PreSecond() As Double
        '    Get
        '        Return _Presecond
        '    End Get
        '    Set(ByVal value As Double)
        '        _Presecond = value
        '        OnPropertyChanged("PreSecond")
        '    End Set
        'End Property

        Private _reverseRank As Boolean = True
        Public Property ReverseRank As Boolean
            Get
                Return _reverseRank
            End Get
            Set
                _reverseRank = Value
                OnPropertyChanged("ReverseRank")
            End Set
        End Property

        Private _highlightUsers As String = My.Settings.HighlightUsers
        Public Property HighlightUsers As String
            Get
                Return _highlightUsers
            End Get
            Set
                _highlightUsers = Value
                OnPropertyChanged("HighlightUsers")
            End Set
        End Property

        Private _connected As Boolean = False
        Public Property Connected As Boolean
            Get
                Return _connected
            End Get
            Set
                _connected = Value
                OnPropertyChanged("Connected")
            End Set
        End Property

        Private _playing As Boolean
        Public Property Playing As Boolean
            Get
                Return _playing
            End Get
            Set
                _playing = Value
                OnPropertyChanged("Playing")
            End Set
        End Property

        Private _lastLyricShown As Boolean
        Public Property LastLyricShown As Boolean
            Get
                Return _lastLyricShown
            End Get
            Set
                _lastLyricShown = Value
                OnPropertyChanged("LastLyricShown")
            End Set
        End Property

        Private Const TitleFotter As String = " - ニコ生タイピング"
        Private _windowTitle As String = "ニコ生タイピング"
        Public Property WindowTitle As String
            Get
                Return _windowTitle
            End Get
            Set
                _windowTitle = Value & TitleFotter
                OnPropertyChanged("WindowTitle")
            End Set
        End Property

        Private _messageFontSize As Double = My.Settings.MessageFontSize
        Public Property MessageFontSize As Double
            Get
                Return _messageFontSize
            End Get
            Set
                _messageFontSize = Value
                OnPropertyChanged("MessageFontSize")
            End Set
        End Property

        Private _lyricFontSize As Double = My.Settings.LyricFontSize
        Public Property LyricFontSize As Double
            Get
                Return _lyricFontSize
            End Get
            Set
                _lyricFontSize = Value
                OnPropertyChanged("LyricFontSize")
            End Set
        End Property

        Private _rankingFontSize As Double = My.Settings.RankingFontSize
        Public Property RankingFontSize As Double
            Get
                Return _rankingFontSize
            End Get
            Set
                _rankingFontSize = Value
                OnPropertyChanged("RankingFontSize")
            End Set
        End Property

        Private _lyricTitle As String = ""
        Public Property LyricTitle As String
            Get
                Return _lyricTitle
            End Get
            Set
                _lyricTitle = Value
                OnPropertyChanged("LyricTitle")
            End Set
        End Property

        Private _bottomGridOpacity As Double = My.Settings.BottomGridOpacity
        Public Property BottomGridOpacity As Double
            Get
                Return _bottomGridOpacity
            End Get
            Set
                _bottomGridOpacity = Value
                OnPropertyChanged("BottomGridOpacity")
            End Set
        End Property

        Private _recentLyricLineCount As Integer = My.Settings.RecentLyricLineCount
        Public Property RecentLyricLineCount As Integer
            Get
                Return _recentLyricLineCount
            End Get
            Set
                _recentLyricLineCount = Value
                OnPropertyChanged("RecentLyricLineCount")
            End Set
        End Property

#End Region

#Region "Events"

        Public Event ShowSettings As EventHandler(Of EventArgs)
        Protected Sub OnShowSettings()
            RaiseEvent ShowSettings(Me, EventArgs.Empty)
        End Sub

        Public Event ShowResults As EventHandler(Of EventArgs)
        Protected Sub OnShowResults()
            RaiseEvent ShowResults(Me, EventArgs.Empty)
        End Sub

        Public Event RankingAdded As EventHandler(Of EventArgs)
        Protected Sub OnRankingAdded()
            RaiseEvent RankingAdded(Me, EventArgs.Empty)
        End Sub

        Public Event MessageAdded As EventHandler(Of EventArgs)
        Protected Sub OnMessageAdded()
            RaiseEvent MessageAdded(Me, EventArgs.Empty)
        End Sub

        Public Event Played As EventHandler(Of EventArgs)
        Protected Sub OnPlayed()
            RaiseEvent Played(Me, EventArgs.Empty)
        End Sub

        Public Event Stopped As EventHandler(Of EventArgs)
        Protected Sub OnStopped()
            RaiseEvent Stopped(Me, EventArgs.Empty)
        End Sub


#End Region


        Private Sub AddMessage(no As Integer, message As String, kind As MessageKind)

            If (kind = MessageKind.System AndAlso ShowPointMessages) OrElse
                (kind = MessageKind.Filtered AndAlso ShowFilteredMessages) OrElse
                (kind = MessageKind.Other AndAlso ShowOtherMessages) OrElse
                (kind = MessageKind.NameEntry AndAlso ShowNameEntryMessages) Then

                'If _Message.Count > 5 Then
                '    _Message.RemoveAt(0)
                'End If
                Messages.Add($"{no}: {message}")
                OnMessageAdded()
            End If

        End Sub

        Private Member As New Dictionary(Of String, User)

        Public Sub InjectComment(comment As LiveCommentMessage)
            LiveProgramClient_CommentReceived(Me, New CommentReceivedEventArgs(comment))
        End Sub

        Private Sub LiveProgramClient_CommentReceived(sender As Object, e As CommentReceivedEventArgs)

            If Not Dispatcher.CheckAccess Then
                Dispatcher.Invoke(New Action(Of Object, CommentReceivedEventArgs)(AddressOf LiveProgramClient_CommentReceived), New Object() {sender, e})
                Exit Sub
            End If

            If (e.Comment.Source <> ChatSource.Broadcaster OrElse
                    TypeOf sender Is LiveProgramClient AndAlso _liveProgramClientAndRoomLabels.Count > 1 AndAlso
                        Not _liveProgramClientAndRoomLabels.Find(Function(clientAndLabel) clientAndLabel.client Is sender).label.StartsWith("立ち見")) AndAlso
                e.Comment.Source <> ChatSource.General AndAlso
                e.Comment.Source <> ChatSource.Premium Then
                Exit Sub
            End If

            ' 
            If ShowFilteredMessages AndAlso
               (DisplayCommentPattern.Trim = "" OrElse e.Comment.Text.StartsWith(DisplayCommentPattern.Trim)) Then
                AddMessage(e.Comment.No, e.Comment.Text, MessageKind.Filtered)
            End If

            If (e.Comment.Source = ChatSource.Broadcaster OrElse e.Comment.Source = ChatSource.Operator) AndAlso e.Comment.Text.StartsWith("/disconnect") Then
                Exit Sub
            End If

            ' コメント番号追加
            If Member.ContainsKey(e.Comment.UserId) Then
                Member(e.Comment.UserId).RecentCommentDateTime = Now
            End If

            ' 名前
            NameEntryProcedure(e.Comment)

            If Playing Then
                TypingProcedure(e.Comment)
            End If

            ' TODO Pointメッセージ表示時は非表示
            If ShowOtherMessages Then
                AddMessage(e.Comment.No, e.Comment.Text, MessageKind.Other)
            End If

        End Sub

        Protected Function NameEntryProcedure(comment As LiveCommentMessage) As Boolean

            If Not Regex.IsMatch(comment.Text, "^(@|＠).+$") Then
                Return False
            End If

            Dim name = comment.Text.Substring(1).Replace(vbCr, " ").Replace(vbLf, " ").Replace(vbTab, " ").Replace("　", " ").Trim



            'If comment.ContainsNGWords Then
            '    name = "名無し（NGコメ）"
            'Else
            If name.Length > 16 Then
                name = name.Substring(0, 16)
            End If
            'End If

            If Not Member.ContainsKey(comment.UserId) Then
                ' ユーザー追加 MEMO ユーザー追加場所は2か所
                Dim user = New User With {.Name = name, .Premium = comment.Source, .Id = comment.UserId}
                Member.Add(comment.UserId, user)

                AddMessage(comment.No, "名前設定: " & user.Name, MessageKind.NameEntry)
            Else
                ' 名前変更
                AddMessage(comment.No, "名前変更: " & Member(comment.UserId).Name & " → " & name, MessageKind.NameEntry)
                Member(comment.UserId).Name = name
            End If

            Return True
        End Function

        Private Sub TypingProcedure(comment As LiveCommentMessage)

            If Not Member.ContainsKey(comment.UserId) Then
                ' ユーザー追加 MEMO ユーザー追加場所は2か所
                Dim newUser = New User With {.Premium = comment.Source, .Id = comment.UserId}
                newUser.RecentCommentDateTime = Now
                Member.Add(comment.UserId, newUser)
            End If

            Dim user = Member(comment.UserId)
            Dim texts = comment.Text.ToLyricsWords(True).Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)

            ' TODO nest解消

            For Each t In texts
                Do
                    For i = user.LyricsIndex To _lyricsIndex - 1

                        Dim start = 0
                        If user.LyricsIndex = i Then
                            start = user.LyricsSubIndex
                        End If

                        For j = start To _lyrics.Lines(i).Words.Count - 1
                            If t.StartsWith(_lyrics.Lines(i).Words(j)) Then
                                Dim point As Integer

                                ' Point
                                point = _lyrics.Lines(i).Yomi(j).Length * 3

                                user.RawScore += point

                                ' ---
                                Dim skipped = True
                                If i = user.LyricsIndex Then
                                    If j = user.LyricsSubIndex Then
                                        skipped = False
                                    End If
                                ElseIf i = user.LyricsIndex + 1 Then
                                    If j = 0 AndAlso user.LyricsSubIndex = _lyrics.Lines(i - 1).Words.Count Then
                                        skipped = False
                                    End If
                                End If
                                If skipped Then
                                    user.ScoringResults.Add(New ScoringResult With {
                                                            .Rating = Rating.Skip})
                                End If
                                ' ---

                                user.LyricsIndex = i
                                user.LyricsSubIndex = j + 1

                                AddMessage(comment.No, "Great! " & user.Name & " " & _lyrics.Lines(i).Words(j), MessageKind.System)
                                user.ScoringResults.Add(New ScoringResult With {
                                                        .LineIndex = i,
                                                        .WordIndex = j,
                                                        .Text = _lyrics.Lines(i).Words(j),
                                                        .Rating = Rating.Great,
                                                        .CommentNo = comment.No})

                                If t.Length > _lyrics.Lines(i).Words(j).Length Then
                                    t = t.Substring(_lyrics.Lines(i).Words(j).Length)
                                Else
                                    Exit Do
                                End If

                            Else
                                Dim yomi = t.ToHiragana(_lyrics.ReplacementWords)
                                If yomi.StartsWith(_lyrics.Lines(i).Yomi(j)) Then

                                    ' Point
                                    Dim point As Integer
                                    point = _lyrics.Lines(i).Yomi(j).Length * 2

                                    user.RawScore += point


                                    ' ---
                                    Dim skipped = True
                                    If i = user.LyricsIndex Then
                                        If j = user.LyricsSubIndex Then
                                            skipped = False
                                        End If
                                    ElseIf i = user.LyricsIndex + 1 Then
                                        If j = 0 AndAlso user.LyricsSubIndex = _lyrics.Lines(i - 1).Words.Count Then
                                            skipped = False
                                        End If
                                    End If
                                    If skipped Then
                                        user.ScoringResults.Add(New ScoringResult With {
                                                                .Rating = Rating.Skip})
                                    End If
                                    ' ---

                                    user.LyricsIndex = i
                                    user.LyricsSubIndex = j + 1

                                    AddMessage(comment.No, "Good! " & user.Name & " " & _lyrics.Lines(i).Yomi(j), MessageKind.System)
                                    user.ScoringResults.Add(New ScoringResult With {
                                                            .LineIndex = i,
                                                            .WordIndex = j,
                                                            .Text = _lyrics.Lines(i).Yomi(j),
                                                            .Rating = Rating.Good,
                                                            .CommentNo = comment.No})


                                    If yomi.Length > _lyrics.Lines(i).Yomi(j).Length Then
                                        t = yomi.Substring(_lyrics.Lines(i).Yomi(j).Length)
                                    Else
                                        Exit Do
                                    End If
                                End If
                            End If

                        Next
                    Next

                    user.ScoringResults.Add(New ScoringResult With {
                           .Text = t,
                           .Rating = Rating.None,
                           .CommentNo = comment.No})

                    Exit Do
                Loop


            Next

            'ShowScoringResults(user)
        End Sub


        Private _lyrics As Lyrics

        ' Status
        Private _isLoaded As Boolean


        Private _lyricsIndex As Integer
        Private _startDateTime As DateTime
        Private WithEvents LyricTimer As New DispatcherTimer
        Private WithEvents PlayerTimer As New DispatcherTimer

        Private Sub LyricTimer_Tick(sender As Object, e As EventArgs) Handles LyricTimer.Tick

            If Player.HasAudio AndAlso Player.Position.TotalMilliseconds >= _lyrics.Lines(_lyricsIndex).TimePosition * 1000 + _lyrics.Offset * 1000 Then

                Do
                    If RecentLyrics.Count >= RecentLyricLineCount Then
                        RecentLyrics.RemoveAt(0)
                    Else
                        Exit Do
                    End If
                Loop

                ' TODO
                Dim wtb = New WipeTextBlock With {
                    .FontWeight = FontWeights.Bold,
                    .Foreground = New SolidColorBrush(Colors.White)
                }

                Dim binding = New Binding("LyricFontSize") With {.Source = Me}
                wtb.SetBinding(WipeTextBlock.FontSizeProperty, binding)
                If _lyrics.WipeEnabled AndAlso _lyrics.Lines(_lyricsIndex).Text <> "" Then
                    wtb.TextWithTimeTag = _lyrics.Lines(_lyricsIndex).TextWithTimeTag
                Else
                    wtb.WipeEnabled = False
                    wtb.Text = _lyrics.Lines(_lyricsIndex).Text
                End If

                ' 強調範囲の設定
                For Each range As KeyValuePair(Of Integer, Integer) In _lyrics.Lines(_lyricsIndex).HighlightRanges
                    wtb.WipeTextBlock.TextEffects.Add(New TextEffect(Nothing, _blacklistCharactersHighlightColor, Nothing, range.Key, range.Value))
                Next

                RecentLyrics.Add(wtb)

                '' お手本モード
                'If IsExampleMode Then
                '    Dim sb = New System.Text.StringBuilder
                '    If LiveProgramClient IsNot Nothing AndAlso LiveProgramClient.Connected Then
                '        LiveProgramClient.BroadcastComment(Lyrics.Lines(LyricsIndex).Text)
                '    End If
                'End If

                _lyricsIndex += 1
                If _lyricsIndex >= _lyrics.Lines.Count Then
                    LyricTimer.Stop()
                    LastLyricShown = True
                End If

            End If


        End Sub

#Region "Player"

        'Private Player As New Windows.Media.MediaPlayer

        Private WithEvents _player As MediaElement
        Public Property Player As MediaElement
            Get
                Return _player
            End Get
            Set
                _player = Value
                _player.LoadedBehavior = MediaState.Manual
                _player.UnloadedBehavior = MediaState.Manual
            End Set
        End Property

        Public ReadOnly Property PlayerPosition As TimeSpan
            Get
                Return Player.Position
            End Get
        End Property

        Public ReadOnly Property MediaLength As TimeSpan
            Get
                If Player.HasAudio AndAlso Player.NaturalDuration.HasTimeSpan Then
                    Return Player.NaturalDuration.TimeSpan
                Else
                    Return TimeSpan.FromSeconds(0)
                End If
            End Get
        End Property


        Public Property Volume As Double
            Get
                Return Player.Volume
            End Get
            Set
                Player.Volume = Value
                OnPropertyChanged("Volume")
            End Set
        End Property

        Public Property SpeedRatio As Double
            Get
                Return Player.SpeedRatio
            End Get
            Set
                Player.SpeedRatio = Value
                OnPropertyChanged("SpeedRatio")
            End Set
        End Property

#End Region

#Region "Play"
        Private _playCommand As ICommand
        Public ReadOnly Property PlayCommand As ICommand
            Get
                If _playCommand Is Nothing Then
                    _playCommand = New RelayCommand(New Action(Of Object)(AddressOf Play), New Predicate(Of Object)(AddressOf CanPlay))
                End If
                Return _playCommand
            End Get
        End Property

        Private Sub Play(obj As Object)

            _lyricsIndex = 0
            _startDateTime = Now

            TruncateOldMessages(My.Settings.RecentMessagesCount)
            RankedUsers.Clear()


            Player.Position = New TimeSpan(0)
            Player.Play()
            Playing = True
            OnPlayed()

            LyricTimer.Interval = TimeSpan.FromMilliseconds(100)
            LyricTimer.Start()

            PlayerTimer.Interval = TimeSpan.FromMilliseconds(100)
            PlayerTimer.Start()

            ' ユーザー点数初期化
            For Each m In Member.Values
                m.Reset()
            Next


        End Sub

        Private Function CanPlay(obj As Object) As Boolean
            Return _isLoaded AndAlso Not Playing
        End Function
#End Region

#Region "Stop"
        Private _stopCommand As ICommand
        Public ReadOnly Property StopCommand As ICommand
            Get
                If _stopCommand Is Nothing Then
                    _stopCommand = New RelayCommand(New Action(Of Object)(AddressOf [Stop]), New Predicate(Of Object)(AddressOf CanStop))
                End If
                Return _stopCommand
            End Get
        End Property

        Private Sub [Stop](obj As Object)
            Playing = False
            OnStopped()

            RecentLyrics.Clear()


            LyricTimer.Stop()
            LastLyricShown = False
            ShowRanking()

        End Sub

        Private Function CanStop(obj As Object) As Boolean
            Return Playing
        End Function
#End Region

#Region "Load"
        Private _loadCommand As ICommand
        Public ReadOnly Property LoadCommand As ICommand
            Get
                If _loadCommand Is Nothing Then
                    _loadCommand = New RelayCommand(New Action(Of Object)(AddressOf Load), New Predicate(Of Object)(AddressOf CanLoad))
                End If
                Return _loadCommand
            End Get
        End Property

        Private Sub Load(obj As Object)
            Dim dialog = New OpenFileDialog With {
                .Filter = "*.xml,*.lrc|*.xml;*.lrc|*.xml|*.xml|*.lrc|*.lrc"
            }

            'Dim xmlFiles = System.IO.Directory.GetFiles("", "*.xml")
            'If xmlFiles.Count > 0 Then
            '    dialog.FilterIndex = 1
            'End If


            If dialog.ShowDialog Then


                Dim l = New Lyrics

                Dim errorMessage As String = Nothing
                Dim result = l.TryLoad(dialog.FileName, errorMessage)
                If errorMessage IsNot Nothing Then
                    StatusMessage = errorMessage
                End If
                If Not result Then
                    Exit Sub
                End If

                _lyrics = l

                LyricTitle = _lyrics.Title
                WindowTitle = _lyrics.Title

            Else
                Exit Sub
            End If

            OnPropertyChanged("BackgroundImage")
            Player.Close()

            If _lyrics.VideoFileName <> "" Then
                BackgroundImage = Nothing
                Player.Source = New Uri(_lyrics.VideoFileName)
            ElseIf _lyrics.SoundFileName <> "" Then
                Player.Source = New Uri(_lyrics.SoundFileName)

            End If

            _isLoaded = True

            ' 総時間表示、およびメディア形式検証のため Pause で Media を開く
            Player.Pause()
            Player.Position = TimeSpan.FromSeconds(0)
            OnPropertyChanged("MediaLength")
            OnPropertyChanged("PlayerPosition")

        End Sub

        Private Function CanLoad(obj As Object) As Boolean
            Return Not Playing
        End Function
#End Region

#Region "Connect"
        Private _connecting As Boolean

        Private ReadOnly _liveProgramClientAndRoomLabels As New List(Of (client As LiveProgramClient, label As String))

        Private _connectCommand As ICommand
        Public ReadOnly Property ConnectCommand As ICommand
            Get
                If _connectCommand Is Nothing Then
                    _connectCommand = New RelayCommand(New Action(Of Object)(AddressOf Connect), New Predicate(Of Object)(AddressOf CanConnect))
                End If
                Return _connectCommand
            End Get
        End Property

        Private Async Sub Connect(obj As Object)
            Disconnect()

            If Not LiveProgramId.StartsWith("lv") Then
                Exit Sub
            End If

            _connecting = True

            Dim commentServers = If(ConnectAllCommentServers, GetAllCommentServersAsync(), LiveProgramClient.GetCommentServersAsync(LiveProgramId))

            Try
                For Each server In Await If(ConnectAllCommentServers, GetAllCommentServersAsync(), LiveProgramClient.GetCommentServersAsync(LiveProgramId))
                    Dim client = New LiveProgramClient()
                    AddHandler client.CommentReceived, AddressOf LiveProgramClient_CommentReceived
                    AddHandler client.ConnectCompleted, AddressOf LiveProgramClient_ConnectCompleted
                    AddHandler client.ConnectedChanged, AddressOf LiveProgramClient_ConnectionStatusChanged
                    client.ConnectAsync(server)
                    _liveProgramClientAndRoomLabels.Add((client, server.RoomLabel))
                Next
            Catch ex As Exception When ex.Message = "closed"
                StatusMessage = $"「{LiveProgramId}」は現在配信中ではありません。"
            Catch ex As Exception When ex.Message = "require_community_member"
                StatusMessage = "ニコ生タイピングは、フォロワー限定 (コミュ限) の配信には接続できません。"
            End Try

            _connecting = False
        End Sub

        ''' <summary> 
        ''' すべてのコメントサーバーを取得します (ニコニコミュニティの配信のみ)。 
        ''' </summary>
        ''' <returns></returns> 
        Private Async Function GetAllCommentServersAsync() As Task(Of IList(Of CommentServer))
            Dim webTask = LiveProgramClient.GetCommentServersAsync(LiveProgramId)
            Dim liveProgramTask = NicoVideoWeb.GetLiveProgramAsync(LiveProgramId)

            Await Task.WhenAll(webTask, liveProgramTask)

            Dim commentServers = webTask.Result
            Dim program = liveProgramTask.Result
            Return If(commentServers.Count = 1 AndAlso Not program.IsOfficial AndAlso TypeOf program.ChannelCommunity Is Community,
                GetAllCommentServers(program, commentServers(0)),
                webTask.Result)
        End Function

        ''' <summary> 
        ''' 指定したライブ配信のすべてのコメントサーバーを取得します。 
        ''' </summary> 
        ''' <param name="program">ニコニコミュニティの配信。</param> 
        ''' <param name="basicServer">指定した配信のいずれかのコメントサーバー。</param> 
        ''' <returns></returns> 
        Private Function GetAllCommentServers(program As LiveProgram, basicServer As CommentServer) As IList(Of CommentServer)
            Dim servers = New List(Of CommentServer)

            For Each room In GetCommunityChannelRooms(DirectCast(program.ChannelCommunity, Community).Level)
                servers.Add(If(basicServer.Room = room, basicServer, CommentServer.ChangeRoom(basicServer, room)))
            Next

            Return servers
        End Function

        ''' <summary>
        ''' コミュニティレベルをもとに、作られる<see cref="CommunityChannelRoom"/>を取得します。 
        ''' </summary>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Private Function GetCommunityChannelRooms(level As Integer) As CommunityChannelRoom()
            Select Case level
                Case Is < 50
                    Return {Arena, StandingA}
                Case Is < 70
                    Return {Arena, StandingA, StandingB}
                Case Is < 105
                    Return {Arena, StandingA, StandingB, StandingC}
                Case Is < 150
                    Return {Arena, StandingA, StandingB, StandingC, StandingD}
                Case Is < 190
                    Return {Arena, StandingA, StandingB, StandingC, StandingD, StandingE}
                Case Is < 230
                    Return {Arena, StandingA, StandingB, StandingC, StandingD, StandingE, StandingF}
                Case Is < 256
                    Return {Arena, StandingA, StandingB, StandingC, StandingD, StandingE, StandingF, StandingG}
                Case Else
                    Return {Arena, StandingA, StandingB, StandingC, StandingD, StandingE, StandingF, StandingG, StandingH, StandingI}
            End Select
        End Function

        Private Function CanConnect(obj As Object) As Boolean
            If LiveProgramId = "" Then
                Return False
            End If

            If _liveProgramClientAndRoomLabels.Count = 0 Then
                If _connecting Then
                    Return False
                Else
                    Return True
                End If
            End If

            Return False

        End Function

#End Region

#Region "Disconnect"

        Private _disconnectCommand As ICommand
        Public ReadOnly Property DisconnectCommand As ICommand
            Get
                If _disconnectCommand Is Nothing Then
                    _disconnectCommand = New RelayCommand(New Action(Of Object)(AddressOf Disconnect), New Predicate(Of Object)(AddressOf CanDisconnect))
                End If
                Return _disconnectCommand
            End Get
        End Property

        Private Sub Disconnect(obj As Object)

            If _liveProgramClientAndRoomLabels.Count > 0 Then
                For Each clientAndLabel In _liveProgramClientAndRoomLabels
                    Dim client = clientAndLabel.client
                    RemoveHandler client.CommentReceived, AddressOf LiveProgramClient_CommentReceived
                    RemoveHandler client.ConnectCompleted, AddressOf LiveProgramClient_ConnectCompleted
                    RemoveHandler client.ConnectedChanged, AddressOf LiveProgramClient_ConnectionStatusChanged
                    client.Close()
                Next
                _liveProgramClientAndRoomLabels.Clear()

                StatusMessage = "切断しました"

                Connected = False
            End If

        End Sub

        Private Function CanDisconnect(obj As Object) As Boolean
            Return _liveProgramClientAndRoomLabels.Count > 0
        End Function

        Public Sub Disconnect()
            Disconnect(Nothing)
        End Sub

#End Region

#Region "GoHome"
        Private _goHomeCommand As ICommand
        Public ReadOnly Property GoHomeCommand As ICommand
            Get
                If _goHomeCommand Is Nothing Then
                    _goHomeCommand = New RelayCommand(New Action(Of Object)(AddressOf GoHome), New Predicate(Of Object)(AddressOf CanGoHome))
                End If
                Return _goHomeCommand
            End Get
        End Property

        Private Sub GoHome(obj As Object)

        End Sub

        Private Function CanGoHome(obj As Object) As Boolean
            Return Not Playing
        End Function

#End Region

        Public ReadOnly Property RankedUsers As ObservableCollection(Of User) = New ObservableCollection(Of User)


        Private WithEvents RankingTimer As New DispatcherTimer
        Private ReadOnly _rankedUsersQueue As New Queue(Of User)

        Public Sub ShowRanking()

            RankedUsers.Clear()
            _rankedUsersQueue.Clear()

            Dim maxScore = 0
            For Each l In _lyrics.Lines
                For Each ll In l.Yomi
                    maxScore += ll.Length * 3
                Next
            Next

            Dim ids = _highlightUsers.Split(New Char() {","c}, StringSplitOptions.None).Select(Function(id) id.Trim()).ToList
            For Each m In Member.Keys
                Member(m).Highlighted = ids.Contains(m)
            Next

            For Each m In Member.Values
                m.NormalizeScore(maxScore)
                'm.NormalizedScore = Convert.ToInt32(m.Score / maxScore * 1000.0)
            Next

            Dim orderedMember = From m In Member.Values
                                Where m.RecentCommentDateTime >= _startDateTime
                                Order By m.NormalizedScore Descending, m.TotalScore Descending, m.PreviousRank Ascending


            Dim rank = 1
            Dim index As Integer
            Dim prevRank As Integer

            'Dim data As New RankDataViewModel

            For Each m In orderedMember

                ' お手本モード、一度でもお手本モードにしたら採点に反映しないようにする
                If IsExampleMode AndAlso m.Premium = 3 Then
                    Continue For
                End If

                Using data = New RankDataViewModel

                    If index > 0 AndAlso prevRank > m.NormalizedScore Then
                        rank = index + 1
                    End If

                    m.Rank = rank
                    'data.Rank = rank
                    'data.Score = m.NormalizedScore
                    'data.User = m

                    _rankedUsersQueue.Enqueue(m)

                    prevRank = m.NormalizedScore
                    index += 1

                End Using
            Next

            ' ランクを逆順にする
            If ReverseRank Then
                Dim list = New List(Of User)
                For Each i In _rankedUsersQueue
                    list.Insert(0, i)
                Next
                _rankedUsersQueue.Clear()
                For Each i In list
                    _rankedUsersQueue.Enqueue(i)
                Next
            End If

            If Player.HasAudio Then
                Dim p = Player.NaturalDuration.TimeSpan.TotalSeconds - (orderedMember.Count + 1)
                If p < 0 Then
                    p = 0
                End If
                Player.Stop()
                Console.WriteLine("p = " & p.ToString)
                Player.Position = New TimeSpan(0, 0, CInt(p))
                Player.Play()
            End If

            RankingTimer.Interval = New TimeSpan(0, 0, 1)
            RankingTimer.Start()

            '#If DEBUG Then
            '            For i = 0 To 3
            '                RankDataQueue.Enqueue(New RankDataViewModel With {.Rank = 999, .Score = 0, .User = New User With {.Name = "hoge"}})
            '            Next
            '#End If

        End Sub

        Private Sub RankingTimer_Tick(sender As Object, e As EventArgs) Handles RankingTimer.Tick

            If _rankedUsersQueue.Count > 0 Then
                RankedUsers.Insert(0, _rankedUsersQueue.Dequeue)
                OnRankingAdded()
            Else
                RankingTimer.Stop()
            End If

        End Sub

        Private Sub LiveProgramClient_ConnectCompleted(sender As Object, e As AsyncCompletedEventArgs)
            If e.Error Is Nothing Then
                Exit Sub
            End If

            Throw e.Error
            'If TypeOf e.Error Is NicoVideo.NicoVideoException Then
            '    Dim nex = DirectCast(e.Error, NicoVideo.NicoVideoException)
            '    If nex.ErrorDescription <> "" Then
            '        Me.StatusMessage = String.Format("接続に失敗しました（Code={0}, Desc={1}）", nex.ErrorCode, nex.ErrorDescription)
            '    Else
            '        Me.StatusMessage = String.Format("接続に失敗しました（Code={0}）", nex.ErrorCode)
            '    End If
            'Else
            '    Me.StatusMessage = String.Format("接続に失敗しました（{0}）", e.Error.Message)
            'End If

        End Sub

        Private Sub LiveProgramClient_ConnectionStatusChanged(sender As Object, e As EventArgs)
            Dim client = DirectCast(sender, LiveProgramClient)

            If client.Connected Then
                Dim prefix = "接続しました: "
                Dim label = _liveProgramClientAndRoomLabels.Find(Function(clientAndLabel) clientAndLabel.client Is client).label
                If StatusMessage IsNot Nothing AndAlso StatusMessage.StartsWith(prefix) Then
                    StatusMessage &= ", " & label
                Else
                    StatusMessage = prefix & label
                End If

                Connected = True
            Else
                Disconnect()
            End If
        End Sub



#Region "Show Lyric Command"
        Private _showLyricCommand As ICommand
        Public ReadOnly Property ShowLyricCommand As ICommand
            Get
                If _showLyricCommand Is Nothing Then
                    _showLyricCommand = New RelayCommand(New Action(Of Object)(AddressOf ShowLyric), New Predicate(Of Object)(AddressOf CanShowLyric))
                End If
                Return _showLyricCommand
            End Get
        End Property

        Private Sub ShowLyric(obj As Object)
            Dim window = New LyricWindow With {
                .Lyrics = _lyrics
            }
            window.ShowDialog()

        End Sub

        Private Function CanShowLyric(obj As Object) As Boolean
            Return _isLoaded
        End Function

#End Region

        Private Sub _Player_MediaOpened(sender As Object, e As RoutedEventArgs) Handles _player.MediaOpened
            OnPropertyChanged("MediaLength")
        End Sub

        Private Sub _Player_MediaFailed(sender As Object, e As ExceptionRoutedEventArgs) Handles _player.MediaFailed
            If TypeOf e.ErrorException Is COMException Then
                Dim fileName = Path.GetFileName(If(_lyrics.SoundFileName, _lyrics.VideoFileName))
                Dim type = If(_lyrics.SoundFileName IsNot Nothing, "音声", "動画")
                Select Case e.ErrorException.HResult
                    Case MilaverrLoadfailed
                        StatusMessage = $"「{fileName}」はWindows Media Playerで再生できない{type}ファイルです。"
                        Exit Sub
                    Case MilaverrUnexpectedwmpfailure
                        StatusMessage = $"「{fileName}」はWindows Media Playerで早送りできない{type}ファイルです。"
                        Exit Sub
                End Select
                Throw e.ErrorException
            End If
        End Sub

        Private Sub _Player_MediaEnded(sender As Object, e As RoutedEventArgs) Handles _player.MediaEnded
            PlayerTimer.Stop()
        End Sub

        Private Sub PlayerTimer_Tick(sender As Object, e As EventArgs) Handles PlayerTimer.Tick
            OnPropertyChanged("PlayerPosition")
        End Sub


#Region "ClearMessages"

        Private _clearMessagesCommand As ICommand
        Public ReadOnly Property ClearMessagesCommand As ICommand
            Get
                If _clearMessagesCommand Is Nothing Then
                    _clearMessagesCommand = New RelayCommand(New Action(Of Object)(AddressOf ClearMessages),
                                                             New Predicate(Of Object)(AddressOf CanClearMessages))
                End If
                Return _clearMessagesCommand
            End Get
        End Property

        Private Sub ClearMessages(obj As Object)
            Messages.Clear()
        End Sub

        Private Function CanClearMessages(obj As Object) As Boolean
            Return Messages.Count > 0
        End Function

#End Region

#Region "Font size change"

        Private _changeMessageFontSizeCommand As ICommand
        Public ReadOnly Property ChangeMessageFontSizeCommand As ICommand
            Get
                If _changeMessageFontSizeCommand Is Nothing Then
                    _changeMessageFontSizeCommand = New RelayCommand(New Action(Of Object)(AddressOf ChangeMessageFontSize),
                                                              New Predicate(Of Object)(AddressOf CanChangeMessageFontSize))
                End If
                Return _changeMessageFontSizeCommand
            End Get
        End Property

        Private Sub ChangeMessageFontSize(obj As Object)
            Dim d = Convert.ToInt32(obj)
            MessageFontSize += d
        End Sub

        Private Function CanChangeMessageFontSize(obj As Object) As Boolean
            Dim d = Convert.ToInt32(obj)
            Return MessageFontSize + d > 10 AndAlso MessageFontSize + d <= 34
        End Function


        Private _changeLyricFontSizeCommand As ICommand
        Public ReadOnly Property ChangeLyricFontSizeCommand As ICommand
            Get
                If _changeLyricFontSizeCommand Is Nothing Then
                    _changeLyricFontSizeCommand = New RelayCommand(New Action(Of Object)(AddressOf ChangeLyricFontSize),
                                                              New Predicate(Of Object)(AddressOf CanChangeLyricFontSize))
                End If
                Return _changeLyricFontSizeCommand
            End Get
        End Property

        Private Sub ChangeLyricFontSize(obj As Object)
            Dim d = Convert.ToInt32(obj)
            LyricFontSize += d
        End Sub

        Private Function CanChangeLyricFontSize(obj As Object) As Boolean
            Dim d = Convert.ToInt32(obj)
            Return LyricFontSize + d > 10 AndAlso LyricFontSize + d <= 34
        End Function


        Private _changeRankingFontSizeCommand As ICommand
        Public ReadOnly Property ChangeRankingFontSizeCommand As ICommand
            Get
                If _changeRankingFontSizeCommand Is Nothing Then
                    _changeRankingFontSizeCommand = New RelayCommand(New Action(Of Object)(AddressOf ChangeRankingFontSize),
                                                              New Predicate(Of Object)(AddressOf CanChangeRankingFontSize))
                End If
                Return _changeRankingFontSizeCommand
            End Get
        End Property

        Private Sub ChangeRankingFontSize(obj As Object)
            Dim d = Convert.ToInt32(obj)
            RankingFontSize += d
        End Sub

        Private Function CanChangeRankingFontSize(obj As Object) As Boolean
            Dim d = Convert.ToInt32(obj)
            Return RankingFontSize + d > 10 AndAlso RankingFontSize + d <= 34
        End Function
#End Region


        Private _showSettingsCommand As ICommand
        Public ReadOnly Property ShowSettingsCommand As ICommand
            Get
                If _showSettingsCommand Is Nothing Then
                    _showSettingsCommand = New RelayCommand(
                        New Action(Of Object)(Sub()
                                                  OnShowSettings()
                                              End Sub))
                End If
                Return _showSettingsCommand
            End Get
        End Property

        Private _showResultsCommand As ICommand
        Public ReadOnly Property ShowResultsCommand As ICommand
            Get
                If _showResultsCommand Is Nothing Then
                    _showResultsCommand = New RelayCommand(
                        New Action(Of Object)(Sub()
                                                  OnShowResults()
                                              End Sub))
                End If
                Return _showResultsCommand
            End Get
        End Property

        Public ReadOnly Property MediaStretch As Integer = My.Settings.MediaStretch

        Public Property BlacklistCharactersHighlight As Boolean
            Get
                Return My.Settings.BlacklistCharactersHighlight
            End Get
            Set
                My.Settings.BlacklistCharactersHighlight = Value
            End Set
        End Property

        Public Property SplitBlacklistCharacters As Boolean
            Get
                Return My.Settings.SplitBlacklistCharacters
            End Get
            Set
                My.Settings.SplitBlacklistCharacters = Value
            End Set
        End Property

        Public ReadOnly Property BlacklistCharactersSeparator As String = My.Settings.BlacklistCharactersSeparator

        Public Property ConnectAllCommentServers As Boolean = My.Settings.ConnectAllCommentServers

        Public Property ShowTimeOnLyricsGrid As Boolean
            Get
                Return My.Settings.ShowTimeOnLyricsGrid
            End Get
            Set
                My.Settings.ShowTimeOnLyricsGrid = Value
                OnPropertyChanged("ShowTimeOnLyricsGrid")
            End Set
        End Property

        ''' <summary>
        ''' 表示しているログを、指定件数だけ残して切り詰めます。
        ''' </summary>
        ''' <param name="count"></param>
        Private Sub TruncateOldMessages(count As Integer)
            For i = 1 To Messages.Count - count
                Messages.RemoveAt(0)
            Next
        End Sub

    End Class
End Namespace
