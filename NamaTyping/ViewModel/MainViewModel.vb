Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports Pronama.NamaTyping.Model
Imports Microsoft.Win32
Imports Pronama.NicoVideo.LiveStreaming
Imports Pronama.NicoVideo.LiveStreaming.CommunityChannelRoom
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Imports System.Runtime.InteropServices

Namespace ViewModel

    Public Class MainViewModel
        Inherits ViewModelBase


        Private _Dispatcher As Dispatcher
        Public Property Dispatcher() As Dispatcher
            Get
                Return _Dispatcher
            End Get
            Set(ByVal value As Dispatcher)
                _Dispatcher = value
            End Set
        End Property

        ''' <summary>
        ''' 運営NGワードの強調色。
        ''' </summary>
        Private ReadOnly BlacklistCharactersHighlightColor As Brush = Brushes.Red

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


        Private _RecentLyrics As New ObservableCollection(Of WipeTextBlock)
        Public ReadOnly Property RecentLyrics() As ObservableCollection(Of WipeTextBlock)
            Get
                Return _RecentLyrics
            End Get
        End Property

        Private _Messages As New ObservableCollection(Of String)
        Public ReadOnly Property Messages() As ObservableCollection(Of String)
            Get
                Return _Messages
            End Get
        End Property

        Private _LiveProgramIdText As String = ""
        Public Property LiveProgramIdText() As String
            Get
                Return _LiveProgramIdText
            End Get
            Set(ByVal value As String)
                _LiveProgramIdText = value
                OnPropertyChanged("LiveProgramIdText")
            End Set
        End Property

        Protected ReadOnly Property LiveProgramId() As String
            Get
                Dim match = System.Text.RegularExpressions.Regex.Match(LiveProgramIdText, "(?<id>lv\d+)")
                If match.Success Then
                    Return match.Groups("id").Value
                Else
                    Return ""
                End If
            End Get
        End Property

        Private _IsExampleMode As Boolean
        Public Property IsExampleMode() As Boolean
            Get
                Return _IsExampleMode
            End Get
            Set(ByVal value As Boolean)
                _IsExampleMode = value
                OnPropertyChanged("IsExampleMode")
            End Set
        End Property

        Private _ShowNameEntryMessages As Boolean = My.Settings.ShowNameEntryMessages
        Public Property ShowNameEntryMessages() As Boolean
            Get
                Return _ShowNameEntryMessages
            End Get
            Set(ByVal value As Boolean)
                _ShowNameEntryMessages = value
                OnPropertyChanged("ShowNameEntryMessages")
            End Set
        End Property

        Private _ShowPointMessages As Boolean = My.Settings.ShowPointMessages
        Public Property ShowPointMessages() As Boolean
            Get
                Return _ShowPointMessages
            End Get
            Set(ByVal value As Boolean)
                _ShowPointMessages = value
                OnPropertyChanged("ShowPointMessages")
            End Set
        End Property

        Private _ShowFilteredMessages As Boolean = My.Settings.ShowFilteredMessages
        Public Property ShowFilteredMessages() As Boolean
            Get
                Return _ShowFilteredMessages
            End Get
            Set(ByVal value As Boolean)
                _ShowFilteredMessages = value
                OnPropertyChanged("ShowFilteredMessages")
            End Set
        End Property

        Private _ShowOtherMessages As Boolean
        Public Property ShowOtherMessages() As Boolean
            Get
                Return _ShowOtherMessages
            End Get
            Set(ByVal value As Boolean)
                _ShowOtherMessages = value
                OnPropertyChanged("ShowOtherMessages")
            End Set
        End Property

        Private _DisplayCommentPattern As String = My.Settings.DisplayCommentPattern
        Public Property DisplayCommentPattern As String
            Get
                Return _DisplayCommentPattern
            End Get
            Set(ByVal value As String)
                _DisplayCommentPattern = value
                OnPropertyChanged("DisplayCommentPattern")
            End Set
        End Property

        Private _BackgroundImage As Uri
        Public Property BackgroundImage() As Uri
            Get
                If Lyrics IsNot Nothing AndAlso Lyrics.ImageFileName <> "" Then
                    _BackgroundImage = New Uri(Lyrics.ImageFileName)
                    Return _BackgroundImage
                Else
                    Return _BackgroundImage
                End If
            End Get
            Set(ByVal value As Uri)
                _BackgroundImage = value
                OnPropertyChanged("BackgroundImage")
            End Set
        End Property


        Private _StatusMessage As String
        Public Property StatusMessage() As String
            Get
                Return _StatusMessage
            End Get
            Set(ByVal value As String)
                If value = _StatusMessage Then
                    _StatusMessage = ""
                    OnPropertyChanged("StatusMessage")
                End If
                _StatusMessage = value
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

        Private _ReverseRank As Boolean = True
        Public Property ReverseRank() As Boolean
            Get
                Return _ReverseRank
            End Get
            Set(ByVal value As Boolean)
                _ReverseRank = value
                OnPropertyChanged("ReverseRank")
            End Set
        End Property

        Private _HighlightUsers As String = My.Settings.HighlightUsers
        Public Property HighlightUsers() As String
            Get
                Return _HighlightUsers
            End Get
            Set(ByVal value As String)
                _HighlightUsers = value
                OnPropertyChanged("HighlightUsers")
            End Set
        End Property

        Private _Connected As Boolean = False
        Public Property Connected As Boolean
            Get
                Return _Connected
            End Get
            Set(ByVal value As Boolean)
                _Connected = value
                OnPropertyChanged("Connected")
            End Set
        End Property

        Private _Playing As Boolean
        Public Property Playing As Boolean
            Get
                Return _Playing
            End Get
            Set(ByVal value As Boolean)
                _Playing = value
                OnPropertyChanged("Playing")
            End Set
        End Property

        Private _LastLyricShown As Boolean
        Public Property LastLyricShown As Boolean
            Get
                Return _LastLyricShown
            End Get
            Set(ByVal value As Boolean)
                _LastLyricShown = value
                OnPropertyChanged("LastLyricShown")
            End Set
        End Property

        Private Const TitleFotter As String = " - ニコ生タイピング"
        Private _WindowTitle As String = "ニコ生タイピング"
        Public Property WindowTitle As String
            Get
                Return _WindowTitle
            End Get
            Set(ByVal value As String)
                _WindowTitle = value & TitleFotter
                OnPropertyChanged("WindowTitle")
            End Set
        End Property

        Private _MessageFontSize As Double = My.Settings.MessageFontSize
        Public Property MessageFontSize As Double
            Get
                Return _MessageFontSize
            End Get
            Set(ByVal value As Double)
                _MessageFontSize = value
                OnPropertyChanged("MessageFontSize")
            End Set
        End Property

        Private _LyricFontSize As Double = My.Settings.LyricFontSize
        Public Property LyricFontSize As Double
            Get
                Return _LyricFontSize
            End Get
            Set(ByVal value As Double)
                _LyricFontSize = value
                OnPropertyChanged("LyricFontSize")
            End Set
        End Property

        Private _RankingFontSize As Double = My.Settings.RankingFontSize
        Public Property RankingFontSize As Double
            Get
                Return _RankingFontSize
            End Get
            Set(ByVal value As Double)
                _RankingFontSize = value
                OnPropertyChanged("RankingFontSize")
            End Set
        End Property

        Private _LyricTitle As String = ""
        Public Property LyricTitle As String
            Get
                Return _LyricTitle
            End Get
            Set(ByVal value As String)
                _LyricTitle = value
                OnPropertyChanged("LyricTitle")
            End Set
        End Property

        Private _BottomGridOpacity As Double = My.Settings.BottomGridOpacity
        Public Property BottomGridOpacity As Double
            Get
                Return _BottomGridOpacity
            End Get
            Set(ByVal value As Double)
                _BottomGridOpacity = value
                OnPropertyChanged("BottomGridOpacity")
            End Set
        End Property

        Private _RecentLyricLineCount As Integer = My.Settings.RecentLyricLineCount
        Public Property RecentLyricLineCount As Integer
            Get
                Return _RecentLyricLineCount
            End Get
            Set(ByVal value As Integer)
                _RecentLyricLineCount = value
                OnPropertyChanged("RecentLyricLineCount")
            End Set
        End Property

#End Region

#Region "Evnets"

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


        Private Sub AddMessage(ByVal no As Integer, ByVal message As String, ByVal kind As MessageKind)

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

        Private Member As New System.Collections.Generic.Dictionary(Of String, User)

        Public Sub InjectComment(ByVal comment As LiveCommentMessage)
            LiveProgramClient_CommentReceived(Me, New CommentReceivedEventArgs(comment))
        End Sub

        Private Sub LiveProgramClient_CommentReceived(ByVal sender As Object, ByVal e As CommentReceivedEventArgs)

            If Not Dispatcher.CheckAccess Then
                Dispatcher.Invoke(New Action(Of Object, CommentReceivedEventArgs)(AddressOf LiveProgramClient_CommentReceived), New Object() {sender, e})
                Exit Sub
            End If

            If (e.Comment.Source <> ChatSource.Broadcaster OrElse
                    TypeOf sender Is LiveProgramClient AndAlso LiveProgramClientAndRoomLabels.Count > 1 AndAlso
                        Not LiveProgramClientAndRoomLabels.Find(Function(clientAndLabel) clientAndLabel.client Is sender).label.StartsWith("立ち見")) AndAlso
                e.Comment.Source <> ChatSource.General AndAlso
                e.Comment.Source <> ChatSource.Premium Then
                Exit Sub
            End If

            Dim commentAdded As Boolean = False

            ' 
            If ShowFilteredMessages AndAlso
               (DisplayCommentPattern.Trim = "" OrElse e.Comment.Text.StartsWith(DisplayCommentPattern.Trim)) Then
                AddMessage(e.Comment.No, e.Comment.Text, MessageKind.Filtered)
                commentAdded = True
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

        Protected Function NameEntryProcedure(ByVal comment As LiveCommentMessage) As Boolean

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

        Private Sub TypingProcedure(ByVal comment As LiveCommentMessage)

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
                    For i = user.LyricsIndex To LyricsIndex - 1

                        Dim start As Integer = 0
                        If user.LyricsIndex = i Then
                            start = user.LyricsSubIndex
                        End If

                        For j = start To Lyrics.Lines(i).Words.Count - 1
                            If t.StartsWith(Lyrics.Lines(i).Words(j)) Then
                                Dim point As Integer

                                ' Point
                                point = Lyrics.Lines(i).Yomi(j).Length * 3

                                user.RawScore += point

                                ' ---
                                Dim skipped = True
                                If i = user.LyricsIndex Then
                                    If j = user.LyricsSubIndex Then
                                        skipped = False
                                    End If
                                ElseIf i = user.LyricsIndex + 1 Then
                                    If j = 0 AndAlso user.LyricsSubIndex = Lyrics.Lines(i - 1).Words.Count Then
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

                                AddMessage(comment.No, "Great! " & user.Name & " " & Lyrics.Lines(i).Words(j), MessageKind.System)
                                user.ScoringResults.Add(New ScoringResult With {
                                                        .LineIndex = i,
                                                        .WordIndex = j,
                                                        .Text = Lyrics.Lines(i).Words(j),
                                                        .Rating = Rating.Great,
                                                        .CommentNo = comment.No})

                                If t.Length > Lyrics.Lines(i).Words(j).Length Then
                                    t = t.Substring(Lyrics.Lines(i).Words(j).Length)
                                Else
                                    Exit Do
                                End If

                            Else
                                Dim yomi = t.ToHiragana(Lyrics.ReplacementWords)
                                If yomi.StartsWith(Lyrics.Lines(i).Yomi(j)) Then

                                    ' Point
                                    Dim point As Integer
                                    point = Lyrics.Lines(i).Yomi(j).Length * 2

                                    user.RawScore += point


                                    ' ---
                                    Dim skipped = True
                                    If i = user.LyricsIndex Then
                                        If j = user.LyricsSubIndex Then
                                            skipped = False
                                        End If
                                    ElseIf i = user.LyricsIndex + 1 Then
                                        If j = 0 AndAlso user.LyricsSubIndex = Lyrics.Lines(i - 1).Words.Count Then
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

                                    AddMessage(comment.No, "Good! " & user.Name & " " & Lyrics.Lines(i).Yomi(j), MessageKind.System)
                                    user.ScoringResults.Add(New ScoringResult With {
                                                            .LineIndex = i,
                                                            .WordIndex = j,
                                                            .Text = Lyrics.Lines(i).Yomi(j),
                                                            .Rating = Rating.Good,
                                                            .CommentNo = comment.No})


                                    If yomi.Length > Lyrics.Lines(i).Yomi(j).Length Then
                                        t = yomi.Substring(Lyrics.Lines(i).Yomi(j).Length)
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


        Private Lyrics As Lyrics

        ' Status
        Private IsLoaded As Boolean


        Private LyricsIndex As Integer
        Private StartDateTime As DateTime
        Private WithEvents LyricTimer As New DispatcherTimer
        Private WithEvents PlayerTimer As New DispatcherTimer

        Private Sub LyricTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles LyricTimer.Tick

            If Player.HasAudio AndAlso Player.Position.TotalMilliseconds >= Lyrics.Lines(LyricsIndex).TimePosition * 1000 + Lyrics.Offset * 1000 Then

                Do
                    If RecentLyrics.Count >= RecentLyricLineCount Then
                        RecentLyrics.RemoveAt(0)
                    Else
                        Exit Do
                    End If
                Loop

                ' TODO
                Dim wtb = New WipeTextBlock
                wtb.FontWeight = FontWeights.Bold
                wtb.Foreground = New SolidColorBrush(Colors.White)

                Dim binding = New Binding("LyricFontSize") With {.Source = Me}
                wtb.SetBinding(WipeTextBlock.FontSizeProperty, binding)
                If Lyrics.WipeEnabled Then
                    wtb.TextWithTimeTag = Lyrics.Lines(LyricsIndex).TextWithTimeTag
                Else
                    wtb.WipeEnabled = False
                    wtb.Text = Lyrics.Lines(LyricsIndex).Text
                End If

                ' 強調範囲の設定
                For Each range As KeyValuePair(Of Integer, Integer) In Lyrics.Lines(LyricsIndex).HighlightRanges
                    wtb.WipeTextBlock.TextEffects.Add(New TextEffect(Nothing, BlacklistCharactersHighlightColor, Nothing, range.Key, range.Value))
                Next

                RecentLyrics.Add(wtb)

                '' お手本モード
                'If IsExampleMode Then
                '    Dim sb = New System.Text.StringBuilder
                '    If LiveProgramClient IsNot Nothing AndAlso LiveProgramClient.Connected Then
                '        LiveProgramClient.BroadcastComment(Lyrics.Lines(LyricsIndex).Text)
                '    End If
                'End If

                LyricsIndex += 1
                If LyricsIndex >= Lyrics.Lines.Count Then
                    LyricTimer.Stop()
                    LastLyricShown = True
                End If

            End If


        End Sub

#Region "Player"

        'Private Player As New Windows.Media.MediaPlayer

        Private WithEvents _Player As MediaElement
        Public Property Player() As MediaElement
            Get
                Return _Player
            End Get
            Set(ByVal value As MediaElement)
                _Player = value
                _Player.LoadedBehavior = MediaState.Manual
                _Player.UnloadedBehavior = MediaState.Manual
            End Set
        End Property

        Public ReadOnly Property PlayerPosition() As TimeSpan
            Get
                Return Player.Position
            End Get
        End Property

        Public ReadOnly Property MediaLength() As TimeSpan
            Get
                If Player.HasAudio AndAlso Player.NaturalDuration.HasTimeSpan Then
                    Return Player.NaturalDuration.TimeSpan
                Else
                    Return TimeSpan.FromSeconds(0)
                End If
            End Get
        End Property


        Public Property Volume() As Double
            Get
                Return Player.Volume
            End Get
            Set(ByVal value As Double)
                Player.Volume = value
                OnPropertyChanged("Volume")
            End Set
        End Property

        Public Property SpeedRatio() As Double
            Get
                Return Player.SpeedRatio
            End Get
            Set(ByVal value As Double)
                Player.SpeedRatio = value
                OnPropertyChanged("SpeedRatio")
            End Set
        End Property

#End Region

#Region "Play"
        Private _PlayCommand As ICommand
        Public ReadOnly Property PlayCommand() As ICommand
            Get
                If _PlayCommand Is Nothing Then
                    _PlayCommand = New RelayCommand(New Action(Of Object)(AddressOf Play), New Predicate(Of Object)(AddressOf CanPlay))
                End If
                Return _PlayCommand
            End Get
        End Property

        Private Sub Play(ByVal obj As Object)

            LyricsIndex = 0
            StartDateTime = Now

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

        Private Function CanPlay(ByVal obj As Object) As Boolean
            Return IsLoaded AndAlso Not Playing
        End Function
#End Region

#Region "Stop"
        Private _stopCommand As ICommand
        Public ReadOnly Property StopCommand() As ICommand
            Get
                If _stopCommand Is Nothing Then
                    _stopCommand = New RelayCommand(New Action(Of Object)(AddressOf Me.Stop), New Predicate(Of Object)(AddressOf CanStop))
                End If
                Return _stopCommand
            End Get
        End Property

        Private Sub [Stop](ByVal obj As Object)
            Playing = False
            OnStopped()

            RecentLyrics.Clear()


            LyricTimer.Stop()
            LastLyricShown = False
            ShowRanking()

        End Sub

        Private Function CanStop(ByVal obj As Object) As Boolean
            Return Playing
        End Function
#End Region

#Region "Load"
        Private _LoadCommand As ICommand
        Public ReadOnly Property LoadCommand() As ICommand
            Get
                If _LoadCommand Is Nothing Then
                    _LoadCommand = New RelayCommand(New Action(Of Object)(AddressOf Me.Load), New Predicate(Of Object)(AddressOf Me.CanLoad))
                End If
                Return _LoadCommand
            End Get
        End Property

        Private Sub Load(ByVal obj As Object)

            Dim dialog = New OpenFileDialog
            dialog.Filter = "*.xml,*.lrc|*.xml;*.lrc|*.xml|*.xml|*.lrc|*.lrc"

            'Dim xmlFiles = System.IO.Directory.GetFiles("", "*.xml")
            'If xmlFiles.Count > 0 Then
            '    dialog.FilterIndex = 1
            'End If


            If dialog.ShowDialog Then


                Dim l = New Lyrics

                Dim errorMessage As String = Nothing
                If Not l.TryLoad(dialog.FileName, errorMessage) Then
                    StatusMessage = errorMessage
                    Exit Sub
                End If

                Lyrics = l

                LyricTitle = Lyrics.Title
                WindowTitle = Lyrics.Title

            Else
                Exit Sub
            End If

            OnPropertyChanged("BackgroundImage")
            Player.Close()

            If Lyrics.VideoFileName <> "" Then
                BackgroundImage = Nothing
                Player.Source = New Uri(Lyrics.VideoFileName)
            ElseIf Lyrics.SoundFileName <> "" Then
                Player.Source = New Uri(Lyrics.SoundFileName)

            End If

            IsLoaded = True

            ' 総時間表示、およびメディア形式検証のため Pause で Media を開く
            Player.Pause()
            Player.Position = TimeSpan.FromSeconds(0)
            OnPropertyChanged("MediaLength")
            OnPropertyChanged("PlayerPosition")

        End Sub

        Private Function CanLoad(ByVal obj As Object) As Boolean
            Return Not Playing
        End Function
#End Region

#Region "Connect"
        Private Connecting As Boolean

        Private ReadOnly LiveProgramClientAndRoomLabels As List(Of (client As LiveProgramClient, label As String)) = New List(Of (client As LiveProgramClient, label As String))

        Private _ConnectCommand As ICommand
        Public ReadOnly Property ConnectCommand() As ICommand
            Get
                If _ConnectCommand Is Nothing Then
                    _ConnectCommand = New RelayCommand(New Action(Of Object)(AddressOf Me.Connect), New Predicate(Of Object)(AddressOf CanConnect))
                End If
                Return _ConnectCommand
            End Get
        End Property

        Private Async Sub Connect(ByVal obj As Object)
            Disconnect()

            If Not Me.LiveProgramId.StartsWith("lv") Then
                Exit Sub
            End If

            Connecting = True

            Dim commentServers = If(ConnectAllCommentServers, GetAllCommentServersAsync(), LiveProgramClient.GetCommentServersAsync(Me.LiveProgramId))

            Try
                For Each server In Await If(ConnectAllCommentServers, GetAllCommentServersAsync(), LiveProgramClient.GetCommentServersAsync(Me.LiveProgramId))
                    Dim client = New LiveProgramClient()
                    AddHandler client.CommentReceived, AddressOf LiveProgramClient_CommentReceived
                    AddHandler client.ConnectCompleted, AddressOf LiveProgramClient_ConnectCompleted
                    AddHandler client.ConnectedChanged, AddressOf LiveProgramClient_ConnectionStatusChanged
                    client.ConnectAsync(server)
                    Me.LiveProgramClientAndRoomLabels.Add((client, server.RoomLabel))
                Next
            Catch ex As Exception When ex.Message = "closed"
                Me.StatusMessage = $"「{LiveProgramId}」は現在配信中ではありません。"
            Catch ex As Exception When ex.Message = "require_community_member"
                Me.StatusMessage = "ニコ生タイピングは、フォロワー限定 (コミュ限) の配信には接続できません。"
            End Try

            Connecting = False
        End Sub

        ''' <summary> 
        ''' すべてのコメントサーバーを取得します (ニコニコミュニティの配信のみ)。 
        ''' </summary>
        ''' <returns></returns> 
        Private Async Function GetAllCommentServersAsync() As Task(Of IList(Of CommentServer))
            Dim webTask = LiveProgramClient.GetCommentServersAsync(LiveProgramId)
            Dim liveProgramTask = NicoVideo.NicoVideoWeb.GetLiveProgramAsync(LiveProgramId)

            Await Task.WhenAll(webTask, liveProgramTask)

            Dim commentServers = webTask.Result
            Dim program = liveProgramTask.Result
            Return If(commentServers.Count = 1 AndAlso Not program.IsOfficial AndAlso TypeOf program.ChannelCommunity Is NicoVideo.Community,
                GetAllCommentServers(program, commentServers(0)),
                webTask.Result)
        End Function

        ''' <summary> 
        ''' 指定したライブ配信のすべてのコメントサーバーを取得します。 
        ''' </summary> 
        ''' <param name="program">ニコニコミュニティの配信。</param> 
        ''' <param name="basicServer">指定した配信のいずれかのコメントサーバー。</param> 
        ''' <returns></returns> 
        Private Function GetAllCommentServers(ByVal program As LiveProgram, ByVal basicServer As CommentServer) As IList(Of CommentServer)
            Dim servers = New List(Of CommentServer)

            For Each room In GetCommunityChannelRooms(DirectCast(program.ChannelCommunity, NicoVideo.Community).Level)
                servers.Add(If(basicServer.Room = room, basicServer, CommentServer.ChangeRoom(basicServer, room)))
            Next

            Return servers
        End Function

        ''' <summary>
        ''' コミュニティレベルをもとに、作られる<see cref="CommunityChannelRoom"/>を取得します。 
        ''' </summary>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Private Function GetCommunityChannelRooms(ByVal level As Integer) As CommunityChannelRoom()
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

        Private Function CanConnect(ByVal obj As Object) As Boolean
            If LiveProgramId = "" Then
                Return False
            End If

            If LiveProgramClientAndRoomLabels.Count = 0 Then
                If Connecting Then
                    Return False
                Else
                    Return True
                End If
            End If

            Return False

        End Function

#End Region

#Region "Disconnect"

        Private _DisconnectCommand As ICommand
        Public ReadOnly Property DisconnectCommand() As ICommand
            Get
                If _DisconnectCommand Is Nothing Then
                    _DisconnectCommand = New RelayCommand(New Action(Of Object)(AddressOf Me.Disconnect), New Predicate(Of Object)(AddressOf CanDisconnect))
                End If
                Return _DisconnectCommand
            End Get
        End Property

        Private Sub Disconnect(ByVal obj As Object)

            If LiveProgramClientAndRoomLabels.Count > 0 Then
                For Each clientAndLabel In LiveProgramClientAndRoomLabels
                    Dim client = clientAndLabel.client
                    RemoveHandler client.CommentReceived, AddressOf LiveProgramClient_CommentReceived
                    RemoveHandler client.ConnectCompleted, AddressOf LiveProgramClient_ConnectCompleted
                    RemoveHandler client.ConnectedChanged, AddressOf LiveProgramClient_ConnectionStatusChanged
                    client.Close()
                Next
                LiveProgramClientAndRoomLabels.Clear()

                Me.StatusMessage = "切断しました"

                Me.Connected = False
            End If

        End Sub

        Private Function CanDisconnect(ByVal obj As Object) As Boolean
            Return LiveProgramClientAndRoomLabels.Count > 0
        End Function

        Public Sub Disconnect()
            Disconnect(Nothing)
        End Sub

#End Region

#Region "GoHome"
        Private _GoHomeCommand As ICommand
        Public ReadOnly Property GoHomeCommand() As ICommand
            Get
                If _GoHomeCommand Is Nothing Then
                    _GoHomeCommand = New RelayCommand(New Action(Of Object)(AddressOf Me.GoHome), New Predicate(Of Object)(AddressOf Me.CanGoHome))
                End If
                Return _GoHomeCommand
            End Get
        End Property

        Private Sub GoHome(ByVal obj As Object)

        End Sub

        Private Function CanGoHome(ByVal obj As Object) As Boolean
            Return Not Playing
        End Function

#End Region

        Private _RankedUsers As New ObservableCollection(Of User)
        Public ReadOnly Property RankedUsers() As ObservableCollection(Of User)
            Get
                Return _RankedUsers
            End Get
        End Property




        Private WithEvents RankingTimer As New DispatcherTimer
        Private RankedUsersQueue As New Queue(Of User)

        Public Sub ShowRanking()
            Dim sb = New System.Text.StringBuilder

            RankedUsers.Clear()
            RankedUsersQueue.Clear()

            Dim maxScore As Integer = 0
            For Each l In Lyrics.Lines
                For Each ll In l.Yomi
                    maxScore += ll.Length * 3
                Next
            Next

            Dim ids = _HighlightUsers.Split(New Char() {","c}, StringSplitOptions.None).Select(Function(id) id.Trim()).ToList
            For Each m In Member.Keys
                Member(m).Highlighted = ids.Contains(m)
            Next

            For Each m In Member.Values
                m.NormalizeScore(maxScore)
                'm.NormalizedScore = Convert.ToInt32(m.Score / maxScore * 1000.0)
            Next

            Dim orderedMember = From m In Member.Values
                                Where m.RecentCommentDateTime >= StartDateTime
                                Order By m.NormalizedScore Descending, m.TotalScore Descending, m.PreviousRank Ascending


            Dim rank As Integer = 1
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

                    RankedUsersQueue.Enqueue(m)

                    prevRank = m.NormalizedScore
                    index += 1

                End Using
            Next

            ' ランクを逆順にする
            If ReverseRank Then
                Dim list = New List(Of User)
                For Each i In RankedUsersQueue
                    list.Insert(0, i)
                Next
                RankedUsersQueue.Clear()
                For Each i In list
                    RankedUsersQueue.Enqueue(i)
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

        Private Sub RankingTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles RankingTimer.Tick

            If RankedUsersQueue.Count > 0 Then
                RankedUsers.Insert(0, RankedUsersQueue.Dequeue)
                OnRankingAdded()
            Else
                RankingTimer.Stop()
            End If

        End Sub

        Private Sub LiveProgramClient_ConnectCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
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

        Private Sub LiveProgramClient_ConnectionStatusChanged(ByVal sender As Object, ByVal e As EventArgs)
            Dim client = DirectCast(sender, LiveProgramClient)

            If client.Connected Then
                Dim prefix = "接続しました: "
                Dim label = LiveProgramClientAndRoomLabels.Find(Function(clientAndLabel) clientAndLabel.client Is client).label
                If Me.StatusMessage IsNot Nothing AndAlso Me.StatusMessage.StartsWith(prefix) Then
                    Me.StatusMessage &= ", " & label
                Else
                    Me.StatusMessage = prefix & label
                End If

                Me.Connected = True
            Else
                Disconnect()
            End If
        End Sub



#Region "Show Lyric Command"
        Private _ShowLyricCommand As ICommand
        Public ReadOnly Property ShowLyricCommand() As ICommand
            Get
                If _ShowLyricCommand Is Nothing Then
                    _ShowLyricCommand = New RelayCommand(New Action(Of Object)(AddressOf ShowLyric), New Predicate(Of Object)(AddressOf CanShowLyric))
                End If
                Return _ShowLyricCommand
            End Get
        End Property

        Private Sub ShowLyric(ByVal obj As Object)

            Dim window = New LyricWindow
            window.Lyrics = Lyrics
            window.ShowDialog()

        End Sub

        Private Function CanShowLyric(ByVal obj As Object) As Boolean
            Return IsLoaded
        End Function

#End Region

        Private Sub _Player_MediaOpened(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles _Player.MediaOpened
            OnPropertyChanged("MediaLength")
        End Sub

        Private Sub _Player_MediaFailed(ByVal sender As Object, ByVal e As System.Windows.ExceptionRoutedEventArgs) Handles _Player.MediaFailed
            If TypeOf e.ErrorException Is COMException Then
                Dim fileName = IO.Path.GetFileName(If(Lyrics.SoundFileName, Lyrics.VideoFileName))
                Dim type = If(Lyrics.SoundFileName IsNot Nothing, "音声", "動画")
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

        Private Sub _Player_MediaEnded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles _Player.MediaEnded
            PlayerTimer.Stop()
        End Sub

        Private Sub PlayerTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles PlayerTimer.Tick
            OnPropertyChanged("PlayerPosition")
        End Sub


#Region "ClearMessages"

        Private _ClearMessagesCommand As ICommand
        Public ReadOnly Property ClearMessagesCommand As ICommand
            Get
                If _ClearMessagesCommand Is Nothing Then
                    _ClearMessagesCommand = New RelayCommand(New Action(Of Object)(AddressOf ClearMessages),
                                                             New Predicate(Of Object)(AddressOf CanClearMessages))
                End If
                Return _ClearMessagesCommand
            End Get
        End Property

        Private Sub ClearMessages(ByVal obj As Object)
            Messages.Clear()
        End Sub

        Private Function CanClearMessages(ByVal obj As Object) As Boolean
            Return Messages.Count > 0
        End Function

#End Region

#Region "Font size change"

        Private _ChangeMessageFontSizeCommand As ICommand
        Public ReadOnly Property ChangeMessageFontSizeCommand() As ICommand
            Get
                If _ChangeMessageFontSizeCommand Is Nothing Then
                    _ChangeMessageFontSizeCommand = New RelayCommand(New Action(Of Object)(AddressOf ChangeMessageFontSize),
                                                              New Predicate(Of Object)(AddressOf CanChangeMessageFontSize))
                End If
                Return _ChangeMessageFontSizeCommand
            End Get
        End Property

        Private Sub ChangeMessageFontSize(ByVal obj As Object)
            Dim d = Convert.ToInt32(obj)
            MessageFontSize += d
        End Sub

        Private Function CanChangeMessageFontSize(ByVal obj As Object) As Boolean
            Dim d = Convert.ToInt32(obj)
            Return MessageFontSize + d > 10 AndAlso MessageFontSize + d <= 34
        End Function


        Private _ChangeLyricFontSizeCommand As ICommand
        Public ReadOnly Property ChangeLyricFontSizeCommand() As ICommand
            Get
                If _ChangeLyricFontSizeCommand Is Nothing Then
                    _ChangeLyricFontSizeCommand = New RelayCommand(New Action(Of Object)(AddressOf ChangeLyricFontSize),
                                                              New Predicate(Of Object)(AddressOf CanChangeLyricFontSize))
                End If
                Return _ChangeLyricFontSizeCommand
            End Get
        End Property

        Private Sub ChangeLyricFontSize(ByVal obj As Object)
            Dim d = Convert.ToInt32(obj)
            LyricFontSize += d
        End Sub

        Private Function CanChangeLyricFontSize(ByVal obj As Object) As Boolean
            Dim d = Convert.ToInt32(obj)
            Return LyricFontSize + d > 10 AndAlso LyricFontSize + d <= 34
        End Function


        Private _ChangeRankingFontSizeCommand As ICommand
        Public ReadOnly Property ChangeRankingFontSizeCommand() As ICommand
            Get
                If _ChangeRankingFontSizeCommand Is Nothing Then
                    _ChangeRankingFontSizeCommand = New RelayCommand(New Action(Of Object)(AddressOf ChangeRankingFontSize),
                                                              New Predicate(Of Object)(AddressOf CanChangeRankingFontSize))
                End If
                Return _ChangeRankingFontSizeCommand
            End Get
        End Property

        Private Sub ChangeRankingFontSize(ByVal obj As Object)
            Dim d = Convert.ToInt32(obj)
            RankingFontSize += d
        End Sub

        Private Function CanChangeRankingFontSize(ByVal obj As Object) As Boolean
            Dim d = Convert.ToInt32(obj)
            Return RankingFontSize + d > 10 AndAlso RankingFontSize + d <= 34
        End Function
#End Region


        Private _ShowSettingsCommand As ICommand
        Public ReadOnly Property ShowSettingsCommand() As ICommand
            Get
                If _ShowSettingsCommand Is Nothing Then
                    _ShowSettingsCommand = New RelayCommand(
                        New Action(Of Object)(Sub()
                                                  OnShowSettings()
                                              End Sub))
                End If
                Return _ShowSettingsCommand
            End Get
        End Property

        Private _ShowResultsCommand As ICommand
        Public ReadOnly Property ShowResultsCommand() As ICommand
            Get
                If _ShowResultsCommand Is Nothing Then
                    _ShowResultsCommand = New RelayCommand(
                        New Action(Of Object)(Sub()
                                                  OnShowResults()
                                              End Sub))
                End If
                Return _ShowResultsCommand
            End Get
        End Property

        Private _WindowSizePattern As Integer = My.Settings.WindowSizePattern
        Public Property WindowSizePattern As Integer
            Get
                Return _WindowSizePattern
            End Get
            Set(ByVal value As Integer)
                _WindowSizePattern = value
            End Set
        End Property

        Public Property BlacklistCharactersHighlight As Boolean
            Get
                Return My.Settings.BlacklistCharactersHighlight
            End Get
            Set(ByVal value As Boolean)
                My.Settings.BlacklistCharactersHighlight = value
            End Set
        End Property

        Public Property SplitBlacklistCharacters As Boolean
            Get
                Return My.Settings.SplitBlacklistCharacters
            End Get
            Set(ByVal value As Boolean)
                My.Settings.SplitBlacklistCharacters = value
            End Set
        End Property

        Public ReadOnly Property BlacklistCharactersSeparator As String = My.Settings.BlacklistCharactersSeparator

        Public Property ConnectAllCommentServers As Boolean = My.Settings.ConnectAllCommentServers

        Public Property ShowTimeOnLyricsGrid As Boolean
            Get
                Return My.Settings.ShowTimeOnLyricsGrid
            End Get
            Set(ByVal value As Boolean)
                My.Settings.ShowTimeOnLyricsGrid = value
                OnPropertyChanged("ShowTimeOnLyricsGrid")
            End Set
        End Property

    End Class
End Namespace
