Imports System.Text.RegularExpressions
Imports <xmlns="http://www.w3.org/2005/Atom">
Imports <xmlns:ntype="http://namaalert.jp/namatyping">

Namespace Model

    Public Class Lyrics


        Private _ReplacementWords As New Dictionary(Of String, String)
        Public ReadOnly Property ReplacementWords() As IDictionary(Of String, String)
            Get
                Return _ReplacementWords
            End Get
        End Property

        Private _Lines As New List(Of LyricLine)
        Public ReadOnly Property Lines() As IList(Of LyricLine)
            Get
                Return _Lines
            End Get
        End Property

        Private _NgWords As New Dictionary(Of String, String)
        Public ReadOnly Property NgWords As IDictionary(Of String, String)
            Get
                Return _NgWords
            End Get
        End Property

        Private Shared _CharacterReplacer As CharacterReplacer
        Friend Shared Property CharacterReplacer As CharacterReplacer
            Get
                If _CharacterReplacer Is Nothing Then
                    _CharacterReplacer = New CharacterReplacer()
                End If
                Return _CharacterReplacer
            End Get
            Set(value As CharacterReplacer)
                _CharacterReplacer = value
            End Set
        End Property

        Property SoundFileName As String
        Property ImageFileName As String
        Property VideoFileName As String
        Property ReplacementWordsFileName As String
        Property NgWordsFileName As String
        Property LyricsFileName As String
        Property Encoding As System.Text.Encoding

        Property Title As String
        Property Offset As Double = 0
        Property Speed As Double = 1.0

        Property WipeEnabled As Boolean

        ''' <summary>
        ''' ファイルを読み込みます。
        ''' </summary>
        ''' <param name="file">存在するXMLファイル、またはlrcファイルのパス。</param>
        ''' <param name="errorMessage">読み込みに失敗した場合のエラーメッセージ。</param>
        ''' <returns>読み込みに成功していれば<c>True</c>を返します。</returns>
        Public Function TryLoad(ByVal file As String, Optional ByRef errorMessage As String = Nothing) As Boolean

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
            If Not System.IO.File.Exists(ReplacementWordsFileName) Then
                Exit Sub
            End If
            If Not System.IO.File.Exists(LyricsFileName) Then
                Exit Sub
            End If

            LoadReplacementWords(ReplacementWordsFileName, Encoding)
            LoadLyrics(LyricsFileName, Encoding)
        End Sub

        Private ReadOnly Property Exists As Boolean
            Get
                Return ReplacementWordsFileName IsNot Nothing AndAlso LyricsFileName IsNot Nothing AndAlso
                    (VideoFileName IsNot Nothing OrElse SoundFileName IsNot Nothing)
            End Get
        End Property

        Private Function TryLoadFromXml(ByVal file As String, Optional ByRef errorMessage As String = Nothing) As Boolean

            Dim doc As XDocument
            Try
                doc = XDocument.Load(file)
            Catch ex As System.Xml.XmlException
                errorMessage = String.Format("「{0}」の読み込みに失敗しました: {1}", My.Computer.FileSystem.GetName(file), ex.Message)
                Return False
            End Try
            Dim path = System.IO.Path.GetDirectoryName(file)

            For Each entry In doc...<entry>
                Title = entry.<title>.Value

                Encoding = System.Text.Encoding.GetEncoding("shift_jis")
                If entry.<ntype:encoding>.Value IsNot Nothing Then
                    Try
                        Encoding = System.Text.Encoding.GetEncoding(entry.<ntype:encoding>.Value)
                    Catch ex As ArgumentException
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
                    If link.@href.IndexOfAny(System.IO.Path.GetInvalidPathChars()) <> -1 Then
                        errorMessage = String.Format("href 属性値「{0}」には、パスに使えない文字が含まれています。", link.@href)
                        Return False
                    End If
                    Dim f = System.IO.Path.Combine(path, link.@href)
                    If Not System.IO.File.Exists(f) Then
                        Continue For
                    End If

                    If link.@rel = "alternate" Then
                        LyricsFileName = f

                    ElseIf link.@rel = "enclosure" Then

                        Select Case link.@type
                            Case "text/plain"
                                If f.ToUpper.EndsWith(".REPL.TXT") OrElse f.ToUpper.EndsWith(".REP.TXT") Then
                                    ReplacementWordsFileName = f

                                ElseIf f.ToUpper.EndsWith(".NG.TXT") Then
                                    NgWordsFileName = f
                                End If

                            Case "video/x-ms-wmv"
                                VideoFileName = f

                            Case "image/png", "iamge/png", "image/jpeg", "image/bmp"
                                ImageFileName = f

                            Case "audio/mpeg"
                                SoundFileName = f

                        End Select
                    End If
                Next

                If Exists Then
                    LoadReplacementWords(ReplacementWordsFileName, Encoding)
                    LoadLyrics(LyricsFileName, Encoding)
                    Return True
                End If

                ' MEMO: not supported multi entries
                Exit For
            Next

            Return False
        End Function

        ' for old format
        Private Function TryLoadFromLyrics(ByVal file As String, Optional ByRef errorMessage As String = Nothing) As Boolean
            Dim path = System.IO.Path.GetDirectoryName(file)
            Dim fileNameWithoutExtenstion = System.IO.Path.GetFileNameWithoutExtension(file)

            Title = fileNameWithoutExtenstion

            Dim replFile = System.IO.Path.Combine(path, fileNameWithoutExtenstion & ".rep.txt")
            If System.IO.File.Exists(replFile) Then
                ReplacementWordsFileName = replFile
            End If

            replFile = System.IO.Path.Combine(path, fileNameWithoutExtenstion & ".repl.txt")
            If System.IO.File.Exists(replFile) Then
                ReplacementWordsFileName = replFile
            End If


            Dim lrcFile = System.IO.Path.Combine(path, fileNameWithoutExtenstion & ".lrc")
            If System.IO.File.Exists(lrcFile) Then
                LyricsFileName = lrcFile
            End If

            Dim wmvFile = System.IO.Path.Combine(path, fileNameWithoutExtenstion & ".wmv")
            If System.IO.File.Exists(wmvFile) Then
                VideoFileName = wmvFile
            Else
                Dim mp3File = System.IO.Path.Combine(path, fileNameWithoutExtenstion & ".mp3")
                If System.IO.File.Exists(mp3File) Then
                    SoundFileName = mp3File
                End If

                Dim wmaFile = System.IO.Path.Combine(path, fileNameWithoutExtenstion & ".wma")
                If System.IO.File.Exists(wmaFile) Then
                    SoundFileName = wmaFile
                End If

                Dim supportImageExtensions = New String() {".jpg", ".jpeg", ".png", ".bmp"}
                For Each ext In supportImageExtensions
                    Dim imageFile = System.IO.Path.Combine(path, fileNameWithoutExtenstion & ext)
                    If System.IO.File.Exists(imageFile) Then
                        ImageFileName = imageFile
                        Exit For
                    End If
                Next
            End If

            Encoding = System.Text.Encoding.GetEncoding("Shift_JIS")

            If Exists Then
                LoadReplacementWords(ReplacementWordsFileName, Encoding)
                LoadLyrics(LyricsFileName, Encoding)
                Return True
            End If

            Return False
        End Function



        Private Sub LoadReplacementWords(ByVal file As String, ByVal encoding As System.Text.Encoding)
            ReplacementWords.Clear()

            Dim lines = My.Computer.FileSystem.ReadAllText( _
                file, encoding).Split(New String() {vbCrLf}, StringSplitOptions.RemoveEmptyEntries)

            Dim sortedLines = From l In lines Order By l.Length Descending

            For Each l In sortedLines
                If Not l.Contains(",") Then
                    Continue For
                End If

                Dim values = l.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
                If values.Count < 2 Then
                    ' 無効
                    Continue For
                End If

                If Not ReplacementWords.ContainsKey(values(0)) Then
                    ReplacementWords.Add(values(0).Trim, values(1).Trim)
                Else
                    ' MEMO: 風クスの同じ単語がある場合
                End If
            Next

        End Sub

        Private Sub LoadLyrics(ByVal file As String, ByVal encoding As System.Text.Encoding)
            Lines.Clear()

            Dim rawLines = New List(Of String)
            Dim t = My.Computer.FileSystem.ReadAllText(file, encoding)
            If My.Settings.BlacklistCharactersHighlight Then
                t = CharacterReplacer.SplitWords(t)
            End If
            For Each l In t.Split(New String() {vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
                If Not l.StartsWith("[") Then
                    Continue For
                End If
                rawLines.Add(l)
            Next

            ' 1行に複数タイムタグがあるか
            For Each l In rawLines
                Dim m = Regex.Matches(l, "\[\d{2}:\d{2}:\d{2}\]")
                If m.Count > 1 Then
                    WipeEnabled = True
                    Exit For
                End If
            Next


            For i = 0 To rawLines.Count - 2
                If Not rawLines(i).EndsWith("]") Then
                    rawLines(i) &= rawLines(i + 1).Substring(0, "[xx:xx:xx]".Length)
                End If
            Next

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

            ' 強調範囲の設定
            If My.Settings.BlacklistCharactersHighlight Then
                Dim allText = ""
                Dim startOffsets = New List(Of Integer)
                For i = 0 To Lines.Count - 1
                    startOffsets.Add(allText.Length)
                    allText &= GetWipeText(i)
                Next

                Dim ranges = CharacterReplacer.Matches(allText)
                While ranges.Count > 0
                    Dim range = ranges.First()
                    For i = 0 To Lines.Count - 1
                        Dim start = startOffsets(i)
                        Dim lineLength = GetWipeText(i).Length
                        If start <= range.Key AndAlso range.Key < start + lineLength Then
                            Lines(i).HighlightRanges.Add(range.Key - start, range.Value)
                            ranges.Remove(range.Key)

                            Dim rangeEnd = range.Key + range.Value
                            Dim nextLineStart = start + lineLength
                            If rangeEnd > nextLineStart Then
                                ranges.Add(nextLineStart, rangeEnd - nextLineStart)
                            End If
                            Exit For
                        End If
                    Next
                End While
            End If

        End Sub

        ''' <summary>
        ''' 実際に表示されるテキストを取得します。
        ''' </summary>
        ''' <seealso cref="WipeTextBlock.Wipe"/>
        ''' <param name="i">0から始まる行番号。</param>
        ''' <returns><see cref="WipeEnabled"/>が設定されていれば、和字間隔 (U+3000) をスペース (U+0020) に置き換えた文字列。</returns>
        Private Function GetWipeText(ByVal i As Integer) As String
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
