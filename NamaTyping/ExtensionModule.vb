Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions

Module ExtensionModule

    '<Extension()> _
    'Public Function WithoutSymbols(ByVal text As String) As String

    '    Dim builder = New System.Text.StringBuilder
    '    For Each c In text

    '        'If Not deleteChars.Contains(c) Then
    '        '    builder.Append(c)
    '        'End If
    '        If Char.IsLetterOrDigit(c) Then
    '            builder.Append(c)
    '        ElseIf Char.IsWhiteSpace(c) Then
    '            builder.Append(" ")
    '        End If

    '    Next
    '    Return builder.ToString

    'End Function


    '<Extension()> _
    'Public Function NormalizeAlphanumeric(ByVal text As String) As String

    '    Const oldChars As String = "ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ０１２３４５６７８９"
    '    Const newChars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

    '    Dim builder = New System.Text.StringBuilder
    '    For Each c In text
    '        Dim index = oldChars.IndexOf(c)
    '        If index >= 0 Then
    '            builder.Append(newChars(index))
    '        Else
    '            builder.Append(c)
    '        End If
    '    Next
    '    Return builder.ToString.ToUpper

    'End Function

    Private ReadOnly Alphanumerics As New Dictionary(Of Char, Char)
    Private ReadOnly KanaDictionary As New Dictionary(Of Char, Char)

    Sub New()
        Const wideChars = "ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ０１２３４５６７８９"
        Const harfChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

        For i = 0 To wideChars.Length - 1
            Alphanumerics.Add(wideChars(i), harfChars(i))
        Next

        Const katakana = "ヲァィゥェォャュョッアイウエオナニヌネノマミムメモヤユヨラリルレロワンカキクケコサシスセソタチツテトハヒフヘホガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポヴヰヱ"
        Const hiragana = "をぁぃぅぇぉゃゅょっあいうえおなにぬねのまみむめもやゆよらりるれろわんかきくけこさしすせそたちつてとはひふへほがぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽゔゐゑ"

        For i = 0 To katakana.Length - 1
            KanaDictionary.Add(katakana(i), hiragana(i))
        Next
    End Sub


    <Extension>
    Public Function ToLyricsWords(text As String, removeSymbols As Boolean) As String

        Dim builder = New StringBuilder
        For Each c In Regex.Replace(text, ViewModel.MainViewModel.RemoveSymbols, "")

            If Char.IsLetterOrDigit(c) Then

                If Alphanumerics.ContainsKey(c) Then
                    builder.Append(Alphanumerics(c))
                Else
                    builder.Append(Char.ToUpperInvariant(c))
                End If

            ElseIf Char.IsWhiteSpace(c) Then
                builder.Append(" ")

            Else
                If Not removeSymbols Then
                    builder.Append(" "c)
                End If
            End If

        Next
        Return builder.ToString

    End Function

    <Extension>
    Public Function ToHiragana(text As String, Optional ByVal replacementTable As IDictionary(Of String, String) = Nothing) As String

        If replacementTable IsNot Nothing Then
            For Each word In replacementTable
                text = text.Replace(word.Key, word.Value)
            Next
        End If

        Dim newText = New StringBuilder
        For Each c In text
            If KanaDictionary.ContainsKey(c) Then
                newText.Append(KanaDictionary(c))
            Else
                newText.Append(c)
            End If
        Next
        Return newText.ToString

    End Function

    <Extension>
    Public Function ToTimeSpan(timeTag As String) As TimeSpan

        Dim m = Regex.Match(timeTag, "\[(?<min>\d{2}):(?<sec>\d{2}):(?<csec>\d{2})\]")
        If Not m.Success Then
            Return TimeSpan.Zero
        End If

        Dim min = Convert.ToInt32(m.Groups("min").Value)
        Dim sec = Convert.ToInt32(m.Groups("sec").Value)
        Dim csec = Convert.ToInt32(m.Groups("csec").Value)
        Return New TimeSpan(0, 0, min, sec, csec * 10)

    End Function


End Module
