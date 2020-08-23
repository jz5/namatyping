Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Imports <xmlns="http://www.w3.org/2005/Atom">
Imports <xmlns:ntype="http://namaalert.jp/namatyping">
Imports Pronama.NamaTyping.TextEncoding

Namespace Model

    Public Class Lyrics

        ''' <summary>
        ''' 動画ファイルの拡張子。音声ファイルでも使う拡張子のうち、音声ファイルである場合が多い拡張子は含まない。
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property VideoFileExtensions As String() = {
            ".3g2", ".3gp", ".3gp2", ".asf", ".avi", ".flv", ".ivf", ".m4v", ".mov", ".mp4", ".mp4v", ".mpeg", ".mpg", ".ogv", ".ogx", ".webm", ".wm", ".wmv"
        }

        ''' <summary>
        ''' 音声ファイルの拡張子。動画ファイルでも使う拡張子のうち、動画ファイルである場合が多い拡張子は含まない。
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property AudioFileExtensions As String() = {
            ".aac", ".aif", ".aifc", ".aiff", ".au", ".m4a", ".mid", ".midi", ".mp2", ".mp3", ".mpa", ".oga", ".ogg", ".rmi", ".snd", ".spx", ".wav", ".wma"
        }

        ''' <summary>
        ''' 画像ファイルの拡張子。
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property ImageFileExtensions As String() = {
            ".jpg", ".jpeg", ".png", ".bmp"
        }

        Private ReadOnly _replacementWords As New Dictionary(Of String, String)
        Public ReadOnly Property ReplacementWords As IDictionary(Of String, String)
            Get
                Return _replacementWords
            End Get
        End Property

        Private ReadOnly _lines As New List(Of LyricLine)
        Public ReadOnly Property Lines As IList(Of LyricLine)
            Get
                Return _lines
            End Get
        End Property

        Private ReadOnly _ngWords As New Dictionary(Of String, String)
        Public ReadOnly Property NgWords As IDictionary(Of String, String)
            Get
                Return _ngWords
            End Get
        End Property

        Property SoundFileName As String
        Property ImageFileName As String
        Property VideoFileName As String
        Property ReplacementWordsFileName As String
        Property LyricsFileName As String
        Property Encoding As Encoding
        Property ReplacementWordsFileEncoding As Encoding

        Property Title As String
        Property Offset As Double = 0
        Property Speed As Double = 1.0

        Property WipeEnabled As Boolean

        ''' <summary>
        ''' ファイルを読み込みます。
        ''' </summary>
        ''' <param name="file">存在するXMLファイル、またはlrcファイルのパス。</param>
        ''' <param name="errorMessage">読み込みに失敗した場合のエラーメッセージ、または成功したものの問題があった場合の警告メッセージ。</param>
        ''' <returns>読み込みに成功していれば<c>True</c>を返します。</returns>
        Public Function TryLoad(file As String, Optional ByRef errorMessage As String = Nothing) As Boolean

            Dim success = False
            If file.ToUpper.EndsWith(".XML") Then
                success = TryLoadFromXml(file, errorMessage)
            ElseIf file.ToUpper.EndsWith(".LRC") Then
                success = TryLoadFromLyrics(file, errorMessage)
            Else

            End If

            If Not success AndAlso errorMessage Is Nothing Then
                If ReplacementWordsFileName Is Nothing Then
                    errorMessage = "置換ファイル(.rep.txt, .repl.txt)がありません"
                ElseIf LyricsFileName Is Nothing Then
                    errorMessage = "歌詞ファイル(.lrc)がありません"
                ElseIf VideoFileName Is Nothing OrElse SoundFileName Is Nothing Then
                    errorMessage = "動画またはサウンドファイルがありません"
                End If
            End If

            Return success
        End Function

        Public Sub Reload()
            If Not File.Exists(ReplacementWordsFileName) Then
                Exit Sub
            End If
            If Not File.Exists(LyricsFileName) Then
                Exit Sub
            End If

            Dim previousReplacementWords = New Dictionary(Of String, String)(ReplacementWords)
            LoadReplacementWords(ReplacementWordsFileName, ReplacementWordsFileEncoding)
            If Not TryLoadLyrics(LyricsFileName, Encoding) Then
                ReplacementWords.Clear()
                For Each values In previousReplacementWords
                    ReplacementWords.Add(values)
                Next
                MessageBox.Show(
                    $"「{Path.GetFileName(LyricsFileName)}」にはタイムタグで始まる行がありません",
                    My.Application.Info.Title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                )
            End If
        End Sub

        Private ReadOnly Property Exists As Boolean
            Get
                Return ReplacementWordsFileName IsNot Nothing AndAlso LyricsFileName IsNot Nothing AndAlso
                    (VideoFileName IsNot Nothing OrElse SoundFileName IsNot Nothing)
            End Get
        End Property

        Private Function TryLoadFromXml(file As String, Optional ByRef errorMessage As String = Nothing) As Boolean

            Dim doc As XDocument
            Try
                doc = XDocument.Load(file)
            Catch ex As XmlException
                errorMessage = $"「{IO.Path.GetFileName(file)}」の読み込みに失敗しました: {ex.Message}"
                Return False
            End Try
            Dim path = IO.Path.GetDirectoryName(file)

            For Each entry In doc...<entry>
                Title = entry.<title>.Value

                If entry.<ntype:encoding>.Value IsNot Nothing Then
                    Try
                        Encoding = Encoding.GetEncoding(entry.<ntype:encoding>.Value)
                        ReplacementWordsFileEncoding = Encoding
                    Catch ex As ArgumentException When ex.ParamName = "name"
                        ' ignore
                    End Try
                End If

                If Double.TryParse(entry.<ntype:offset>.Value, Offset) Then
                    ' Do nothing
                End If

                If Double.TryParse(entry.<ntype:speed>.Value, Speed) Then
                    If Speed < 0 Then
                        Speed = 0
                    ElseIf Speed > 5 Then
                        Speed = 5
                    End If
                End If


                For Each link In entry...<link>
                    ' TODO: http:// 処理

                    If link.@href Is Nothing Then
                        errorMessage = "次の <link> タグに href 属性が存在しません: " & New XElement(link.Name.LocalName, link.Elements(), link.Attributes()).ToString()
                        Return False
                    End If
                    If link.@href.IndexOfAny(IO.Path.GetInvalidPathChars()) <> -1 Then
                        errorMessage = $"href 属性値「{link.@href}」には、パスに使えない文字が含まれています。"
                        Return False
                    End If
                    Dim f = IO.Path.Combine(path, link.@href)
                    If Not IO.File.Exists(f) Then
                        Continue For
                    End If

                    If link.@rel = "alternate" Then
                        LyricsFileName = f

                    ElseIf link.@rel = "enclosure" AndAlso link.@type IsNot Nothing Then

                        Select Case link.@type
                            Case "text/plain"
                                If f.ToUpper.EndsWith(".REPL.TXT") OrElse f.ToUpper.EndsWith(".REP.TXT") Then
                                    ReplacementWordsFileName = f
                                End If

                            Case "image/png", "iamge/png", "image/jpeg", "image/bmp"
                                ImageFileName = f

                            Case Else
                                If link.@type.StartsWith("video/") Then
                                    VideoFileName = f
                                ElseIf link.@type.StartsWith("audio/") Then
                                    SoundFileName = f
                                End If

                        End Select
                    End If
                Next

                If Exists Then
                    LoadReplacementWords(ReplacementWordsFileName, ReplacementWordsFileEncoding)
                    If Not TryLoadLyrics(LyricsFileName, Encoding, errorMessage) Then
                        errorMessage = $"「{IO.Path.GetFileName(LyricsFileName)}」にはタイムタグで始まる行がありません"
                        Return False
                    End If
                    Return True
                End If

                ' MEMO: not supported multi entries
                Exit For
            Next

            Return False
        End Function

        ' for old format
        Private Function TryLoadFromLyrics(file As String, Optional ByRef errorMessage As String = Nothing) As Boolean
            Dim path = IO.Path.GetDirectoryName(file)
            Dim fileNameWithoutExtenstion = IO.Path.GetFileNameWithoutExtension(file)

            Title = fileNameWithoutExtenstion

            Dim replFile = IO.Path.Combine(path, fileNameWithoutExtenstion & ".rep.txt")
            If IO.File.Exists(replFile) Then
                ReplacementWordsFileName = replFile
            End If

            replFile = IO.Path.Combine(path, fileNameWithoutExtenstion & ".repl.txt")
            If IO.File.Exists(replFile) Then
                ReplacementWordsFileName = replFile
            End If


            Dim lrcFile = IO.Path.Combine(path, fileNameWithoutExtenstion & ".lrc")
            If IO.File.Exists(lrcFile) Then
                LyricsFileName = lrcFile
            End If

            For Each fileInfo In New DirectoryInfo(path).GetFiles(fileNameWithoutExtenstion & ".*")
                Dim extension = fileInfo.Name.Substring(fileNameWithoutExtenstion.Length).ToLower()
                If VideoFileExtensions.Contains(extension) Then
                    VideoFileName = fileInfo.FullName
                ElseIf AudioFileExtensions.Contains(extension) Then
                    SoundFileName = fileInfo.FullName
                ElseIf ImageFileExtensions.Contains(extension) Then
                    ImageFileName = fileInfo.FullName
                End If
            Next

            If Exists Then
                LoadReplacementWords(ReplacementWordsFileName, ReplacementWordsFileEncoding)
                If Not TryLoadLyrics(LyricsFileName, Encoding, errorMessage) Then
                    errorMessage = $"「{IO.Path.GetFileName(LyricsFileName)}」にはタイムタグで始まる行がありません"
                    Return False
                End If
                Return True
            End If

            Return False
        End Function



        Private Sub LoadReplacementWords(file As String, ByRef encoding As Encoding)
            ReplacementWords.Clear()

            Dim words = New Dictionary(Of String, String)

            For Each l In ReadLinesWithoutBlankLines(ReadAllText(file, encoding))
                If Not l.Contains(",") Then
                    Continue For
                End If

                Dim values = l.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
                If values.Count < 2 Then
                    ' 無効
                    Continue For
                End If

                values(0) = values(0).Trim().ToLyricsWords(False)
                If values(0) = "" OrElse values(0).Contains(" ") Then
                    ' 検索文字列が空白文字のみで構成されている、または記号類が含まれていれば
                    ' 無効
                    Continue For
                End If

                If Not words.ContainsKey(values(0)) Then
                    values(1) = values(1).ToLyricsWords(True)
                    If values(1) <> "" Then
                        words.Add(values(0), values(1))
                    End If
                Else
                    ' MEMO: 風クスの同じ単語がある場合
                End If
            Next

            For Each values In From l In words Order By l.Key.Length Descending
                ReplacementWords.Add(values)
            Next

        End Sub

        Private Function TryLoadLyrics(file As String, ByRef encoding As Encoding, Optional ByRef errorMessage As String = "") As Boolean
            Dim rawLines = New List(Of String)

            For Each l In ReadLinesWithoutBlankLines(ReadAllText(file, encoding))
                Dim m = Regex.Match(l, "^\[\d{2}:\d{2}:\d{2}\](?<karaoke>.*\[\d{2}:\d{2}:\d{2}\])?")
                If m.Success Then
                    rawLines.Add(l)

                    ' 1行に複数タイムタグがあるか
                    If Not WipeEnabled AndAlso m.Groups.Item("karaoke").Success Then
                        WipeEnabled = True
                    End If
                End If
            Next

            If rawLines.Count = 0 Then
                Return False
            End If

            rawLines = TrimInvalidTimeTags(rawLines, errorMessage)

            rawLines = ComplementLineEndTimeTag(rawLines)

            Lines.Clear()

            For i = 0 To rawLines.Count - 1

                Dim m = Regex.Match(rawLines(i), "\[(?<min>\d{2}):(?<sec>\d{2}):(?<csec>\d{2})\]")

                Dim min = Convert.ToInt32(m.Groups("min").Value)
                Dim sec = Convert.ToInt32(m.Groups("sec").Value)
                Dim csec = Convert.ToInt32(m.Groups("csec").Value)
                Dim ts = New TimeSpan(0, 0, min, sec, csec * 10)

                Dim l = New LyricLine With {
                  .TextWithTimeTag = rawLines(i),
                  .TimePosition = ts.TotalSeconds}

                Dim text = Regex.Replace(rawLines(i), "\[\d{2}:\d{2}:\d{2}\]", "")
                For Each t In text.Split(New String() {" ", "　", vbTab}, StringSplitOptions.RemoveEmptyEntries)
                    For Each word In t.ToLyricsWords(False).Split(New String() {" "c}, StringSplitOptions.RemoveEmptyEntries)
                        l.Words.Add(word)
                        l.Yomi.Add(word.ToHiragana(ReplacementWords))
                    Next
                Next

                l.Text = text
                Lines.Add(l)
            Next

            Return True

        End Function

        ''' <summary>
        ''' 不正なタイムタグを取り除きます。
        ''' </summary>
        ''' <param name="lyrics"></param>
        ''' <param name="warningMessage">不正なタイムタグがあった場合の警告メッセージ。</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 同じ行内で、一つ前のタイムタグより値が小さいタイムタグを取り除きます。
        ''' </remarks>
        Private Function TrimInvalidTimeTags(lyrics As IEnumerable(Of String), ByRef warningMessage As String) As List(Of String)
            Dim lyricsWithValidTimeTagOnly = New List(Of String)
            Dim warningMessages = New List(Of String)

            For Each words In lyrics
                Dim matches = Regex.Matches(words, "\[(?<min>\d{2}):(?<sec>\d{2}):(?<csec>\d{2})\]")
                Dim invalidTimeTagOffsets = New List(Of Integer)
                Dim previousTimeSpan As TimeSpan

                For i = 0 To matches.Count - 1
                    Dim m = matches(i)

                    Dim ts = New TimeSpan(0, 0, Convert.ToInt32(m.Groups("min").Value), Convert.ToInt32(m.Groups("sec").Value), Convert.ToInt32(m.Groups("csec").Value) * 10)

                    If i > 0 AndAlso ts < previousTimeSpan Then
                        ' 不正なタイムタグであれば
                        invalidTimeTagOffsets.Insert(0, m.Index)
                        warningMessages.Add($"{m.Value} は、一つ前の {matches(i - 1).Value} よりも小さい値のタイムタグです。")
                    Else
                        previousTimeSpan = ts
                    End If
                Next

                Dim wordsWithValidTimeTagOnly = words
                For Each o In invalidTimeTagOffsets
                    wordsWithValidTimeTagOnly = wordsWithValidTimeTagOnly.Remove(o, "[xx:xx:xx]".Length)
                Next
                lyricsWithValidTimeTagOnly.Add(wordsWithValidTimeTagOnly)
            Next

            If warningMessages.Count > 0 Then
                warningMessage = String.Join(vbNewLine, warningMessages)
            End If

            Return lyricsWithValidTimeTagOnly
        End Function

        ''' <summary>
        ''' 行末にタイムタグを補完します。
        ''' </summary>
        ''' <param name="lyrics"></param>
        ''' <returns></returns>
        Private Function ComplementLineEndTimeTag(lyrics As IEnumerable(Of String)) As List(Of String)
            Dim lyricsComplementedTimeTag = New List(Of String)

            For i = 0 To lyrics.Count - 1
                Dim validTimeTag = ""

                Dim missingEndTimeTag = Regex.Match(lyrics(i), "(?<last>\[\d{2}:\d{2}:\d{2}\])(?:(?!\[\d{2}:\d{2}:\d{2}\]).)+$")
                If missingEndTimeTag.Success Then
                    ' 行末にタイムタグがなければ
                    If i < lyrics.Count - 1 Then
                        ' 最終行でなければ
                        Dim tag = lyrics(i + 1).Substring(0, "[xx:xx:xx]".Length)
                        If tag.ToTimeSpan() >= missingEndTimeTag.Groups("last").Value.ToTimeSpan() Then
                            ' 次行の行頭のタイムタグの値が、直近のタイムタグの値以上であれば
                            validTimeTag = tag
                        End If
                    End If

                    If validTimeTag = "" Then
                        validTimeTag = missingEndTimeTag.Groups("last").Value
                    End If
                End If

                lyricsComplementedTimeTag.Add(lyrics(i) & validTimeTag)
            Next

            Return lyricsComplementedTimeTag
        End Function

        ''' <summary>
        ''' 実際に表示されるテキストを取得します。
        ''' </summary>
        ''' <seealso cref="WipeTextBlock.Wipe"/>
        ''' <param name="i">0から始まる行番号。</param>
        ''' <returns><see cref="WipeEnabled"/>が設定されていれば、和字間隔 (U+3000) をスペース (U+0020) に置き換えた文字列。</returns>
        Private Function GetWipeText(i As Integer) As String
            Dim text = Lines(i).Text
            If WipeEnabled Then
                text = text.Replace("　", "  ")
            End If
            Return text
        End Function

        'Private Sub ReadLyrics(ByVal fileNameWithoutExtension As String)

        '    Lines.Clear()



        '    Using fs = New System.IO.FileStream(fileNameWithoutExtension & ".lrc", IO.FileMode.Open, IO.FileAccess.Read), _
        '          sr = New System.IO.StreamReader(fs, System.Text.Encoding.GetEncoding("Shift_JIS"))

        '        Do While sr.Peek <> -1

        '            Dim line = sr.ReadLine()
        '            Dim match = System.Text.RegularExpressions.Regex.Match(line, "^\[(?<mm>\d+):(?<ss>\d+):(?<ff>\d+)\](?<words>.*)$")

        '            If Not match.Success Then
        '                Continue Do
        '            End If
        '            'TODO Tag付のデータ（次の行のタグも考慮）を作成
        '            Dim mm = CInt(match.Groups("mm").Value)
        '            Dim ss = CInt(match.Groups("ss").Value)
        '            Dim ff = CInt(match.Groups("ff").Value)
        '            Dim words = match.Groups("words").Value.Trim

        '            Dim l = New LyricLine With { _
        '                .Text = words, _
        '                .TimePosition = mm * 60 + ss + ff * 0.01}

        '            For Each w In words.Split(New String() {"　", " ", vbTab}, StringSplitOptions.RemoveEmptyEntries)

        '                w = w.WithoutSymbols.NormalizeAlphanumeric

        '                l.Words.Add(w)
        '                l.Yomi.Add(LyricsHelper.ToHiragana(w, ReplacementWords))
        '            Next

        '            Lines.Add(l)
        '        Loop

        '    End Using
        'End Sub




    End Class

End Namespace
