'Imports Hal.CookieGetterSharp
Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms.Integration
Imports Microsoft.Win32
Imports Pronama.NicoVideo.LiveStreaming
Imports Pronama.NamaTyping.TextEncoding
Imports Pronama.NamaTyping.ViewModel
Imports System.ComponentModel

Partial Public Class ScreenWindow

    ''' <summary>
    ''' 何%より小さい音量で、音量調整バーのツールチップに小数点以下を表示するか。
    ''' </summary>
    Private Const SmallVolumePercentageThreshold = 10

    ''' <summary>
    ''' 音量調整バーのツールチップに百分率で表示する小数点以下の桁数。
    ''' </summary>
    Private Const SmallVolumePercentagePrecision = 1

    Public ReadOnly Property ScreenControl As ScreenControl = New ScreenControl


    Private ReadOnly _elementHost As New ElementHost

    Public Sub New(context As MainViewModel)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        _elementHost.Child = ScreenControl
        ScreenWindowsFormsHost.Child = _elementHost

        'Dim getters = CookieGetter.CreateInstances(True)
        'For Each cg In getters
        '    CookieGetterComboBox.Items.Add(cg)
        'Next

        DataContext = context
        SizeComboBox.Text = $"{My.Settings.WindowWidth}×{My.Settings.WindowHeight}"
        ChangeWindowSize(SizeComboBox.Text)
        AddHandler ViewModel.PropertyChanged, AddressOf ViewModel_PropertyChanged
    End Sub

    Protected ReadOnly Property ViewModel As MainViewModel
        Get
            If DataContext Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(DataContext, MainViewModel)
        End Get
    End Property


    Public ReadOnly Property Player As MediaElement
        Get
            Return ScreenControl.MyMediaElement
        End Get
    End Property


    Public Sub ScrollRanking()
        For i = 0 To VisualTreeHelper.GetChildrenCount(ScreenControl.RankingItemsControl) - 1
            Dim child = VisualTreeHelper.GetChild(ScreenControl.RankingItemsControl, i)
            If TypeOf child Is ScrollViewer Then
                DirectCast(child, ScrollViewer).ScrollToTop()
                Exit Sub
            End If
        Next
    End Sub

    Public Sub ScrollMessages()

        For i = 0 To VisualTreeHelper.GetChildrenCount(ScreenControl.MessageItemsControl) - 1
            Dim child = VisualTreeHelper.GetChild(ScreenControl.MessageItemsControl, i)
            If TypeOf child Is ScrollViewer Then
                DirectCast(child, ScrollViewer).ScrollToBottom()
                Exit Sub
            End If
        Next
    End Sub

    Private Sub VolumeSlider_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles VolumeSlider.ValueChanged
        Dim slider = DirectCast(sender, Slider)
        Dim precision = If(e.NewValue < SmallVolumePercentageThreshold, SmallVolumePercentagePrecision, 0)
        slider.AutoToolTipPrecision = precision
        slider.ToolTip = Math.Round(e.NewValue, precision)
    End Sub

    Private Sub LiveIdTextBox_GotFocus(sender As Object, e As RoutedEventArgs) Handles LiveIdTextBox.GotFocus
        LiveIdTextBox.Background = New SolidColorBrush(Colors.White)
    End Sub

    Private Sub LiveIdTextBox_LostFocus(sender As Object, e As RoutedEventArgs) Handles LiveIdTextBox.LostFocus
        If LiveIdTextBox.Text.Length = 0 Then
            LiveIdTextBox.Background = New SolidColorBrush(Colors.Transparent)
        End If
    End Sub


    Private Sub SpeedButtonClick(sender As Object, e As RoutedEventArgs)
        If ViewModel Is Nothing Then
            Exit Sub
        End If

        If ViewModel.SpeedRatio = 1 Then
            ViewModel.SpeedRatio = 0
        Else
            ViewModel.SpeedRatio = 1
        End If
    End Sub


    Private _commentNo As Integer = -1

    Private Sub MessageTextBox_KeyDown(sender As Object, e As KeyEventArgs)
        If e.Key <> Key.Enter Then
            Exit Sub
        End If

        Dim textBox = DirectCast(sender, TextBox)

        Dim comment = New LiveCommentMessage With {.UserId = UserComboBox.SelectedIndex.ToString,
                .Text = textBox.Text}
        comment.No = _commentNo
        _commentNo -= 1

        Select Case UserComboBox.SelectedIndex
            Case 0
                comment.Source = ChatSource.Broadcaster
            Case Else
                comment.Source = ChatSource.General
        End Select
        DirectCast(DataContext, MainViewModel).InjectComment(comment)

        textBox.Clear()
    End Sub

    Private SinglePlayMessageTextBoxWatermark As String = "（コメント受信をエミュレートします。コメントサーバーへの送信は行いません。）"

    Private Sub SinglePlayMessageTextBox_Loaded(sender As Object, e As RoutedEventArgs) Handles SinglePlayMessageTextBox.Loaded
        SinglePlayMessageTextBox.Foreground = New SolidColorBrush(Colors.Gray)
        SinglePlayMessageTextBox.Text = SinglePlayMessageTextBoxWatermark
    End Sub

    Private Sub SinglePlayMessageTextBox_GotFocus(sender As Object, e As RoutedEventArgs) Handles SinglePlayMessageTextBox.GotFocus

        SinglePlayMessageTextBox.Foreground = New SolidColorBrush(Colors.Black)

        If SinglePlayMessageTextBox.Text = SinglePlayMessageTextBoxWatermark Then
            SinglePlayMessageTextBox.Text = ""
        End If
    End Sub

    'Private Sub SinglePlayMessageTextBox_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles SinglePlayMessageTextBox.LostFocus

    '    If SinglePlayMessageTextBox.Text.Length = 0 Then
    '        SinglePlayMessageTextBox.Foreground = New SolidColorBrush(Colors.Gray)
    '        SinglePlayMessageTextBox.Text = SinglePlayMessageTextBoxWatermark

    '    End If

    'End Sub

    Private MessageTextBoxWatermark As String = "（動作確認用。コメント送信は行いません。）"

    Private Sub MessageTextBox_Loaded(sender As Object, e As RoutedEventArgs) Handles MessageTextBox.Loaded
        MessageTextBox.Foreground = New SolidColorBrush(Colors.Gray)
        MessageTextBox.Text = MessageTextBoxWatermark
    End Sub

    Private Sub MessageTextBox_GotFocus(sender As Object, e As RoutedEventArgs) Handles MessageTextBox.GotFocus

        MessageTextBox.Foreground = New SolidColorBrush(Colors.Black)

        If MessageTextBox.Text = MessageTextBoxWatermark Then
            MessageTextBox.Text = ""
        End If
    End Sub

    'Private Sub MessageTextBox_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles MessageTextBox.LostFocus

    '    If MessageTextBox.Text.Length = 0 Then
    '        MessageTextBox.Foreground = New SolidColorBrush(Colors.Gray)
    '        MessageTextBox.Text = MessageTextBoxWatermark

    '    End If

    'End Sub

    Private Sub CommentFromFileButton(sender As Object, e As RoutedEventArgs)
        Dim path = Assembly.GetEntryAssembly().Location
        Dim dir = IO.Path.GetDirectoryName(path)
        Dim file = IO.Path.Combine(dir, "NamaTyping_TestComments.txt")

        If Not IO.File.Exists(file) Then
            Exit Sub
        End If

        Dim lines = ReadLinesWithoutBlankLines(ReadAllText(file))
        For Each l In lines
            Dim words = l.Split(","c)
            If words.Length < 2 Then
                Continue For
            End If

            Dim comment = New LiveCommentMessage With {.UserId = words(0), .Text = words(1), .Source = If(words(0) = "0", ChatSource.Broadcaster, ChatSource.General)}
            comment.No = _commentNo
            _commentNo -= 1

            DirectCast(DataContext, MainViewModel).InjectComment(comment)
        Next
    End Sub


    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        'RaiseEvent SizeComboBoxSelectionChanged(sender, e)

        If SizeComboBox.SelectedIndex >= 0 Then
            ChangeWindowSize(CType(CType(SizeComboBox.SelectedItem, ComboBoxItem).Content, TextBlock).Text)
        End If
    End Sub

    Private Sub ComboBox_KeyUp(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Return Then
            ChangeWindowSize(SizeComboBox.Text)
        End If
    End Sub

    Private Sub ComboBox_LostFocus(sender As Object, e As RoutedEventArgs)
        ChangeWindowSize(SizeComboBox.Text)
    End Sub

    Private Sub ViewModel_PropertyChanged(sender As Object, e As PropertyChangedEventArgs)
        If e.PropertyName = "SeparateMedia" Then
            ChangeWindowSize(SizeComboBox.Text)
        End If
    End Sub

    ''' <summary>
    ''' メディア表示領域のサイズを変更します。
    ''' </summary>
    ''' <param name="size"></param>
    Private Sub ChangeWindowSize(size As String)
        Dim w As Integer
        Dim h As Integer

        Dim m = Text.RegularExpressions.Regex.Match(
            size.Normalize(Text.NormalizationForm.FormKC),
            "(?<width>[0-9.]+)[^0-9.]+(?<height>[0-9.]+)"
        )

        If m.Success Then
            If Integer.TryParse(m.Groups("width").Value, w) Then
                Integer.TryParse(m.Groups("height").Value, h)
            End If
        End If

        If h = Nothing Then
            w = CType(My.Settings.Properties.Item("WindowWidth").DefaultValue, Integer)
            h = CType(My.Settings.Properties.Item("WindowHeight").DefaultValue, Integer)
        Else
            If w < MySettings.MinWindowWidth Then
                w = MySettings.MinWindowWidth
            ElseIf w > MySettings.MaxWindowWidth Then
                w = MySettings.MaxWindowWidth
            End If
            If h < MySettings.MinWindowHeight Then
                h = MySettings.MinWindowHeight
            End If
            If h > MySettings.MaxWindowHeight Then
                h = MySettings.MaxWindowHeight
            End If
        End If

        SizeComboBox.Text = $"{w}×{h}"

        MainContentRowDefinition.Height = New GridLength(h)
        If ScreenWindowsFormsHost IsNot Nothing Then
            ScreenWindowsFormsHost.Width = w * If(ViewModel.SeparateMedia, 2, 1)
            ScreenWindowsFormsHost.Height = h
        End If
        With ScreenControl
            .Width = w * If(ViewModel.SeparateMedia, 2, 1)
            .Height = h

            .Media.Width = w
            .Media.Height = h
            .MyImage.Width = w
            .MyImage.Height = h
            .MyMediaElement.Width = w
            .MyMediaElement.Height = h

            .ChromaKey.Width = w
            .ChromaKey.Height = h
            .Grid.Width = MySettings.ReferenceWindowWidth
            .Grid.Height = h / w * MySettings.ReferenceWindowWidth



            .TimeOnLyricsGrid.Background = New SolidColorBrush(If(ViewModel.SeparateMedia, Color.FromRgb(&HAD, &HAD, &HAD), Color.FromArgb(&H77, &HFF, &HFF, &HFF)))
        End With
    End Sub

    Private Sub StretchComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim s = DirectCast([Enum].Parse(
            GetType(Stretch),
            DirectCast(DirectCast(StretchComboBox.SelectedItem, ComboBoxItem).Tag, String)
        ), Stretch)
        ScreenControl.MyImage.Stretch = s
        ScreenControl.MyMediaElement.Stretch = s
    End Sub

    ''' <summary>
    ''' 「ファイルを開く」ダイアログを表示し、選択されたファイルからニコ生タイピング用置換ファイル (*.repl.txt) を生成します。
    ''' </summary>
    Private Sub GenerateReplacementWordsFile()
        Dim dialog = New OpenFileDialog() With {
                .Filter = "歌詞ファイル (*.lrc;*.txt)|*.lrc;*.txt|すべてのファイル (*.*)|*.*"
                }

        If dialog.ShowDialog() Then
            Dim outputFilePath As String = Nothing
            Dim errorMessage As String = Nothing
            ViewModel.StatusMessage = If(ReplacementWordsGenerator.TryGenerate(dialog.FileName, outputFilePath, errorMessage), $"ファイル名「{Path.GetFileName(outputFilePath)}」で保存しました。", errorMessage)
        End If
    End Sub
End Class
