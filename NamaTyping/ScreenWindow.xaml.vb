﻿'Imports Hal.CookieGetterSharp
Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms.Integration
Imports Microsoft.Win32
Imports Pronama.NicoVideo.LiveStreaming
Imports Pronama.NamaTyping.TextEncoding
Imports Pronama.NamaTyping.ViewModel

Partial Public Class ScreenWindow
    Public ReadOnly Property ScreenControl As ScreenControl = New ScreenControl


    Private ReadOnly _elementHost As New ElementHost

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        _elementHost.Child = ScreenControl
        ScreenWindowsFormsHost.Child = _elementHost

        'Dim getters = CookieGetter.CreateInstances(True)
        'For Each cg In getters
        '    CookieGetterComboBox.Items.Add(cg)
        'Next
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


    Private _commentNo As Integer = - 1

    Private Sub MessageTextBox_KeyDown(sender As Object, e As KeyEventArgs)
        If e.Key <> Key.Enter Then
            Exit Sub
        End If

        Dim comment = New LiveCommentMessage With {.UserId = UserComboBox.SelectedIndex.ToString,
                .Text = MessageTextBox.Text}
        comment.No = _commentNo
        _commentNo -= 1

        Select Case UserComboBox.SelectedIndex
            Case 0
                comment.Source = ChatSource.Broadcaster
            Case Else
                comment.Source = ChatSource.General
        End Select
        DirectCast(DataContext, MainViewModel).InjectComment(comment)

        MessageTextBox.Clear()
    End Sub


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

        Dim w As Integer
        Dim h As Integer
        Dim s As Stretch

        Select Case SizeComboBox.SelectedIndex
            Case 0
                w = 640
                h = 360
                s = Stretch.Uniform

                ScreenControl.MyImage.Height = 360
                ScreenControl.MyMediaElement.Height = 360
            Case 1
                w = 640
                h = 360
                s = Stretch.UniformToFill

                ScreenControl.MyImage.Height = 480
                ScreenControl.MyMediaElement.Height = 480
            Case 2
                w = 640
                h = 480
                s = Stretch.Uniform

                ScreenControl.MyImage.Height = 480
                ScreenControl.MyMediaElement.Height = 480
            Case Else
                w = 640
                h = 480
                s = Stretch.UniformToFill

                ScreenControl.MyImage.Height = 480
                ScreenControl.MyMediaElement.Height = 480
        End Select

        MainContentRowDefinition.Height = New GridLength(h)
        If ScreenWindowsFormsHost IsNot Nothing Then
            ScreenWindowsFormsHost.Width = w
            ScreenWindowsFormsHost.Height = h
        End If
        ScreenControl.Width = w
        ScreenControl.Height = h

        ScreenControl.MyImage.Width = w
        ScreenControl.MyImage.Stretch = s

        ScreenControl.MyMediaElement.Width = w
        ScreenControl.MyMediaElement.Stretch = s


        'Uniform
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
