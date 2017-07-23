Imports System.Windows.Media.Animation
Imports System.Windows.Threading
Imports System.Text.RegularExpressions


Public Class WipeTextBlock


    Private WithEvents MyStoryboard As Storyboard
    Private WipeDurations As List(Of TimeSpan)

    Private _WipEnabled As Boolean = True
    Public Property WipeEnabled As Boolean
        Get
            Return _WipEnabled
        End Get
        Set(ByVal value As Boolean)
            _WipEnabled = value
            If value = True Then
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
        Set(ByVal value As String)
            SetValue(TextProperty, value)
            WipeTextBlock.Text = value
        End Set
    End Property

    Public Shared ReadOnly TextProperty As DependencyProperty = _
                               DependencyProperty.Register("Text", _
                               GetType(String), GetType(WipeTextBlock), _
                               New FrameworkPropertyMetadata(Nothing))

    Public Property TextWithTimeTag As String
        Get
            Return Convert.ToString(GetValue(TextWithTimeTagProperty))
        End Get
        Set(ByVal value As String)
            SetValue(TextWithTimeTagProperty, value)
            Me.Text = Regex.Replace(value, "\[\d{2}:\d{2}:\d{2}\]", "")
            Wipe(value, TimeSpan.FromMilliseconds(0))
        End Set
    End Property

    Public Shared ReadOnly TextWithTimeTagProperty As DependencyProperty = _
                               DependencyProperty.Register("TextWithTimeTag", _
                               GetType(String), GetType(WipeTextBlock), _
                               New FrameworkPropertyMetadata(Nothing))


    Public Sub Wipe(ByVal taggedText As String, ByVal offset As TimeSpan)

        Parse(taggedText.Replace("　", "  "))

        '#If DEBUG Then
        '        For i = 0 To Me.Text.Length - 1
        '            Console.WriteLine(Me.Text(i) & " " & WipeDurations(i).TotalMilliseconds.ToString)
        '        Next
        '#End If

        WipedTextEffect.PositionCount = 0
        WipeAnimationTextEffect.PositionStart = 0


        Dim timer = New DispatcherTimer
        timer.Interval = offset
        AddHandler timer.Tick, Sub()
                                   timer.Stop()
                                   Wipe(0)
                               End Sub
        timer.Start()

    End Sub


    Private Sub Wipe(ByVal postion As Integer)
        'Console.WriteLine(postion)

        MyStoryboard = New Storyboard

        Dim d1 = New DoubleAnimation
        d1.From = 0
        d1.To = 1

        ' TODO 例外処理
        d1.Duration = New Duration(WipeDurations(postion))

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

    Private Sub MyStoryboard_Completed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyStoryboard.Completed

        Do
            WipedTextEffect.PositionCount += 1

            If WipeAnimationTextEffect.PositionStart < Me.Text.Length - 1 Then
                WipeAnimationTextEffect.PositionStart += 1

                If WipeDurations(WipeAnimationTextEffect.PositionStart).TotalMilliseconds > 0 Then
                    Wipe(WipeAnimationTextEffect.PositionStart)
                    Exit Do
                End If
            Else
                Exit Do
            End If
        Loop

    End Sub

    Private Sub Parse(ByVal taggedText As String)
        Dim durations = New List(Of TimeSpan)

        Dim sb As New System.Text.StringBuilder
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

                If ts.Subtract(previousTimeSpan) > TimeSpan.FromSeconds(0) Then
                    durations.AddRange(GetDurations(ts.Subtract(previousTimeSpan), GetTextWidths(previousLyric)))
                Else
                    ' TODO invalid tag
                End If

            End If

            previousTimeSpan = ts
            previousLyric = lyric
            sb.Append(lyric)
        Next


        Me.Text = sb.ToString
        WipeDurations = durations
    End Sub

    Private Function GetTextWidths(ByVal text As String) As List(Of Double)

        Dim widths = New List(Of Double)
        For i = 0 To text.Length - 1

            If Char.IsWhiteSpace(text(i)) Then
                widths.Add(0)
                Continue For
            End If

            Dim ft = New FormattedText(text(i), Globalization.CultureInfo.CurrentCulture,
                                       Me.FlowDirection,
                                       New Typeface(Me.FontFamily, Me.FontStyle, Me.FontWeight, Me.FontStretch), Me.FontSize, New SolidColorBrush(Colors.Black))
            widths.Add(ft.Width)
        Next

        Return widths
    End Function

    Private Function GetDurations(ByVal totalDuration As TimeSpan, ByVal charWidths As List(Of Double)) As List(Of TimeSpan)

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
