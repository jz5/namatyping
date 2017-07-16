Imports System.Text.RegularExpressions
Imports System.IO
Imports NMeCab

''' <summary>
''' ニコ生タイピング用置換ファイル (.rep.txt, .repl.txt) の生成を行います。
''' </summary>
Friend NotInheritable Class ReplacementWordsGenerator

    ''' <summary>
    ''' <see cref="TryConvertUniqueFileName"/>における連番の上限。
    ''' </summary>
    Private Const MaxDuplicateFileName = 100

    ''' <summary>
    ''' NAIST Japanese Dictionaryにおける、<see cref="MeCabNode.Feature"/>中の読みのインデックス。
    ''' </summary>
    Private Const YomiIndex = 7

    ''' <summary>
    ''' NAIST Japanese Dictionaryにおいて、<see cref="MeCabNode.CharType"/>に一致する値。
    ''' </summary>
    Private Enum CharacterCategory
        [Default]
        Space
        Kanji
        Symbol
        Numeric
        Alpha
        Hiragana
        Katakana
        Kanjinumeric
        Greek
        Cyrillic
    End Enum

    Private Shared ReadOnly Property Tagger As MeCabTagger = MeCabTagger.Create()

    ''' <summary>
    ''' 指定されたファイルに含まれる文書をもとに、ニコ生タイピング用置換ファイル (*.repl.txt) を生成します。
    ''' </summary>
    ''' <param name="inputPath">歌詞ファイルのパス。タイムタグなどが含まれていてもいなくても構いません。</param>
    ''' <param name="outputPath">保存先のパス。<c>Nothing</c>の場合、<c>inputPath</c>の拡張子を変更したファイル名で保存され、そのパスが入ります。</param>
    ''' <param name="errorMessage">保存先のフォルダがファイルを保存できる状態でなかった場合のエラーメッセージ。</param>
    ''' <returns>保存に成功した場合に<c>True</c>を返します。</returns>
    Friend Shared Function TryGenerate(ByVal inputPath As String, Optional ByRef outputPath As String = Nothing, Optional ByRef errorMessage As String = Nothing) As Boolean
        Dim outputDirectoryPath = Path.GetDirectoryName(If(outputPath, inputPath))
        If (New DirectoryInfo(outputDirectoryPath).Attributes And FileAttributes.ReadOnly) > 0 Then
            errorMessage = "フォルダは読み取り専用です。"
            Return False
        End If

        If outputPath Is Nothing AndAlso
                Not TryConvertUniqueFileName(outputDirectoryPath, Path.GetFileNameWithoutExtension(inputPath), ".repl.txt", outputPath) Then
            errorMessage = $"ファイル「{outputPath}」がすでに存在します。"
            Return False
        End If

        File.WriteAllText(outputPath, Generate(StripTags(TextEncoding.ReadAllText(inputPath))))
        Return True
    End Function

    ''' <summary>
    ''' 指定したディレクトリ内でファイル・フォルダ名が重複していれば、Windows Explorerにおける重複ファイル名の命名規則に従い、連番を末尾に付加した重複しないファイル名を生成します。
    ''' </summary>
    ''' <param name="directoryPath"></param>
    ''' <param name="fileNameWithoutExtension"></param>
    ''' <param name="extension"><c>.</c>で始まる拡張子。</param>
    ''' <param name="uniquePath">重複しないファイルパス。生成に失敗した場合は、最後に存在確認を行ったファイルパスが入ります。</param>
    ''' <returns></returns>
    Private Shared Function TryConvertUniqueFileName(
        ByVal directoryPath As String,
        ByVal fileNameWithoutExtension As String,
        ByVal extension As String,
        ByRef uniquePath As String
    ) As Boolean
        uniquePath = Path.Combine(directoryPath, fileNameWithoutExtension & extension)
        If Not File.Exists(uniquePath) AndAlso Not Directory.Exists(uniquePath) Then
            Return True
        Else
            Dim m = Regex.Match("^(.*)\(\d+\)$", fileNameWithoutExtension)
            Dim fileNameWithoutNumber = If(m.Success, m.Groups(1).Value, fileNameWithoutExtension & " ")

            Dim filenames = From f In Directory.GetFileSystemEntries(directoryPath, $"{fileNameWithoutNumber}(*){extension}")
                            Select Path.GetFileName(f)

            For i = 2 To MaxDuplicateFileName
                Dim fileName = $"{fileNameWithoutNumber}({i}){extension}"
                uniquePath = Path.Combine(directoryPath, fileName)
                If Not filenames.Contains(fileName) Then
                    Return True
                End If
            Next
        End If

        Return False
    End Function

    ''' <summary>
    ''' 歌詞ファイル (*.lrc) に於けるタイムタグと@タグを取り除きます。
    ''' </summary>
    ''' <param name="input"></param>
    ''' <returns></returns>
    Private Shared Function StripTags(ByVal input As String) As String
        Return Regex.Replace(input, "\[\d{2}:\d{2}:\d{2}\]|^@.*", "", RegexOptions.Multiline)
    End Function

    Private Shared Function Generate(ByVal lrycs As String) As String
        Dim replacementWords = New HashSet(Of String)

        Dim node = Tagger.ParseToNode(lrycs)
        While node IsNot Nothing
            If node.CharType = CharacterCategory.Kanji OrElse node.CharType = CharacterCategory.Kanjinumeric Then
                replacementWords.Add(node.Surface & "," & node.Feature.Split(","c)(YomiIndex).ToHiragana() & vbCrLf)
            End If
            node = node.Next
        End While

        Return String.Join("", replacementWords)
    End Function

End Class
