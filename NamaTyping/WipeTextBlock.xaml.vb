Imports System.Globalization
Imports System.Text
Imports System.Windows.Media.Animation
Imports System.Windows.Threading
Imports System.Text.RegularExpressions


Public Class WipeTextBlock


    Private WithEvents MyStoryboard As Storyboard
    Private _wipeDurations As List(Of TimeSpan)

    ''' <summary>
    ''' ワイプ表示を一旦停止する文字インデックスと停止時間。
    ''' </summary>
    Private _wipePauseDurations As Dictionary(Of Integer, TimeSpan) = New Dictionary(Of Integer, TimeSpan)

    Private _wipEnabled As Boolean = True
    Public Property WipeEnabled As Boolean
        Get
            Return _wipEnabled
        End Get
        Set
            _wipEnabled = Value
            If Value = True Then
                StartGradientStop.Color = Colors.Orange
            Else
                ' MEMO: ワイプ未対応歌詞のときオレンジ色が表示される問題用
                StartGradientStop.Color = Colors.White
            End If
        End Set
    End Property

    Public Property Text As String
        Get
            Return Convert.ToString(GetValue(TextProperty))
        End Get
        Set
            SetValue(TextProperty, Value)
            WipeTextBlock.Text = Value
        End Set
    End Property

    Public Shared ReadOnly TextProperty As DependencyProperty =
                               DependencyProperty.Register("Text",
                               GetType(String), GetType(WipeTextBlock),
                               New FrameworkPropertyMetadata(Nothing))

    Public Property TextWithTimeTag As String
        Get
            Return Convert.ToString(GetValue(TextWithTimeTagProperty))
        End Get
        Set
            SetValue(TextWithTimeTagProperty, Value)
            Text = Regex.Replace(Value, "\[\d{2}:\d{2}:\d{2}\]", "")
            Wipe(Value, TimeSpan.FromMilliseconds(0))
        End Set
    End Property

    Public Shared ReadOnly TextWithTimeTagProperty As DependencyProperty =
                               DependencyProperty.Register("TextWithTimeTag",
                               GetType(String), GetType(WipeTextBlock),
                               New FrameworkPropertyMetadata(Nothing))


    Public Sub Wipe(taggedText As String, offset As TimeSpan)

        Parse(taggedText.Replace("　", "  "))

        '#If DEBUG Then
        '        For i = 0 To Me.Text.Length - 1
        '            Console.WriteLine(Me.Text(i) & " " & WipeDurations(i).TotalMilliseconds.ToString)
        '        Next
        '#End If

        WipedTextEffect.PositionCount = 0
        WipeAnimationTextEffect.PositionStart = 0


        Dim timer = New DispatcherTimer With {
            .Interval = offset
        }
        AddHandler timer.Tick, Sub()
                                   timer.Stop()
                                   Wipe(0)
                               End Sub
        timer.Start()

    End Sub


    Private Sub Wipe(postion As Integer)
        'Console.WriteLine(postion)

        MyStoryboard = New Storyboard

        ' TODO 例外処理
        Dim d1 = New DoubleAnimation With {
            .From = 0,
            .To = 1,
            .Duration = New Duration(_wipeDurations(postion))
        }

        Dim d2 = d1.Clone

        MyStoryboard.Children.Add(d1)
        MyStoryboard.Children.Add(d2)

        'Storyboard.SetTarget(d1, StartGradientStop)
        Storyboard.SetTargetProperty(d1, New PropertyPath(GradientStop.OffsetProperty))
        Storyboard.SetTargetName(d1, "StartGradientStop")



        'Storyboard.SetTarget(d2, EndGradientStop)
        Storyboard.SetTargetProperty(d2, New PropertyPath(GradientStop.OffsetProperty))
        Storyboard.SetTargetName(d2, "EndGradientStop")



        MyStoryboard.Begin(Me)

    End Sub

    Private Async Sub MyStoryboard_Completed(sender As Object, e As EventArgs) Handles MyStoryboard.Completed

        Do
            WipedTextEffect.PositionCount += 1

            If WipeAnimationTextEffect.PositionStart < Text.Length - 1 Then
                If _wipePauseDurations.ContainsKey(WipeAnimationTextEffect.PositionStart + 1) Then
                    Await Threading.Tasks.Task.Delay(_wipePauseDurations(WipeAnimationTextEffect.PositionStart + 1))
                End If

                WipeAnimationTextEffect.PositionStart += 1

                If _wipeDurations(WipeAnimationTextEffect.PositionStart).TotalMilliseconds > 0 Then
                    Wipe(WipeAnimationTextEffect.PositionStart)
                    Exit Do
                End If
            Else
                Exit Do
            End If
        Loop

    End Sub

    Private Sub Parse(taggedText As String)
        Dim durations = New List(Of TimeSpan)

        Dim sb As New StringBuilder
        Dim matches = Regex.Matches(taggedText, "(?<tag>\[(?<min>\d{2}):(?<sec>\d{2}):(?<csec>\d{2})\])(?<lyric>((?!\[\d{2}:\d{2}:\d{2}\]).)*)")

        Dim previousLyric As String = Nothing
        Dim previousTimeSpan As TimeSpan

        For i = 0 To matches.Count - 1
            Dim m = matches(i)

            Dim min = Convert.ToInt32(m.Groups("min").Value)
            Dim sec = Convert.ToInt32(m.Groups("sec").Value)
            Dim csec = Convert.ToInt32(m.Groups("csec").Value)
            Dim lyric = m.Groups("lyric").Value

            Dim ts = New TimeSpan(0, 0, min, sec, csec * 10)

            If i > 0 Then

                Dim duration = ts.Subtract(previousTimeSpan)
                If previousLyric = "" Then
                    ' 連続したタイムタグなら
                    _wipePauseDurations.Add(durations.Count, duration)
                Else
                    durations.AddRange(GetDurations(duration, GetTextWidths(previousLyric)))
                End If

            End If

            previousTimeSpan = ts
            previousLyric = lyric
            sb.Append(lyric)
        Next


        Text = sb.ToString
        _wipeDurations = durations
    End Sub

    Private Function GetTextWidths(text As String) As List(Of Double)

        Dim widths = New List(Of Double)
        For i = 0 To text.Length - 1

            If Char.IsWhiteSpace(text(i)) Then
                widths.Add(0)
                Continue For
            End If

            Dim ft = New FormattedText(text(i), CultureInfo.CurrentCulture, FlowDirection,
                                       New Typeface(FontFamily, Me.FontStyle, Me.FontWeight, Me.FontStretch), Me.FontSize, New SolidColorBrush(Colors.Black))
            widths.Add(ft.Width)
        Next

        Return widths
    End Function

    Private Function GetDurations(totalDuration As TimeSpan, charWidths As List(Of Double)) As List(Of TimeSpan)

        Dim sum = charWidths.Sum
        Dim durations = New List(Of TimeSpan)

        If sum = 0 Then
            For i = 0 To charWidths.Count - 1
                durations.Add(TimeSpan.FromMilliseconds(totalDuration.TotalMilliseconds / charWidths.Count))
            Next
        Else
            For i = 0 To charWidths.Count - 1
                durations.Add(TimeSpan.FromMilliseconds(charWidths(i) / sum * totalDuration.TotalMilliseconds))
            Next
        End If

        Return durations

    End Function

End Class
