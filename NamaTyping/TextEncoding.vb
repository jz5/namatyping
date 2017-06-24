Imports System.Text
Imports Ude

''' <summary>
''' 文字符号化方式に関する処理を行います。
''' </summary>
Friend NotInheritable Class TextEncoding

    ''' <summary>
    ''' ファイルから文字列を取得します。
    ''' </summary>
    ''' <param name="file">存在するファイルのパス。</param>
    ''' <param name="encoding"><c>Nothing</c>であれば、UTF-8、Shift_JISを候補に符号化方式を検出し、設定します。</param>
    ''' <returns></returns>
    Friend Shared Function ReadAllText(ByVal file As String, Optional ByRef encoding As Encoding = Nothing) As String
        If encoding Is Nothing Then
            Dim bytes = My.Computer.FileSystem.ReadAllBytes(file)
            Dim detector = New CharsetDetector()
            detector.Feed(bytes, 0, bytes.Length)
            detector.DataEnd()
            encoding = If(detector.Charset Is Charsets.SHIFT_JIS, Encoding.GetEncoding("Shift_JIS"), Encoding.UTF8)
            Return encoding.GetString(bytes)
        Else
            Return My.Computer.FileSystem.ReadAllText(file, encoding)
        End If
    End Function

End Class
