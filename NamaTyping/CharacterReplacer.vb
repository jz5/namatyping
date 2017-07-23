Imports System.Xml
Imports System.Text.RegularExpressions
Imports System.IO

''' <summary>
''' ニコニコ生放送の運営NGワードに関する処理を行います。
''' </summary>
''' <remarks>
''' 参照:
''' <list>
'''     <item>
'''         <term>ニコニコ生放送:運営NGワード一覧とは (ニコニコナマホウソウウンエイエヌジーワードイチランとは) [単語記事] - ニコニコ大百科</term>
'''         <desciption>http://dic.nicovideo.jp/a/%E3%83%8B%E3%82%B3%E3%83%8B%E3%82%B3%E7%94%9F%E6%94%BE%E9%80%81%3A%E9%81%8B%E5%96%B6ng%E3%83%AF%E3%83%BC%E3%83%89%E4%B8%80%E8%A6%A7</desciption>
'''     </item>
'''     <item>
'''         <term>NiconamaCommentViewer : ニコ生コメントビューア</term>
'''         <desciption>http://www.posite-c.com/application/ncv/</desciption>
'''     </item>
'''     <item>
'''         <term>NGワード置換ファイル生成 | Greasy Fork</term>
'''         <desciption>https://greasyfork.org/scripts/11567</desciption>
'''     </item>
''' </list>
''' </remarks>
Friend NotInheritable Class CharacterReplacer
    ''' <summary>
    ''' 『NiconamaCommentViewer』の文字列置換の名前空間。
    ''' </summary>
    Private Const NCVSubstNamespace As String = "http://posite-c.jp/niconamacommentviewer/substitutionlist/"

    ''' <summary>
    ''' 『NGワード置換ファイル生成』の名前空間。
    ''' </summary>
    Private Const BlacklistNamespace As String = "https://greasyfork.org/scripts/11567"

    ''' <summary>
    ''' <see cref="System.Char.IsLetterOrDigit"/>が真を返す文字に一致する正規表現文字列。
    ''' </summary>
    Private Const LetterOrDigitPatternString As String = "[\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nd}]"

    Private Shared Property _BultinUpdated As DateTime
    ''' <summary>
    ''' 内蔵のNGワード置換ファイルの更新日時。
    ''' </summary>
    ''' <returns></returns>
    Friend ReadOnly Property BultinUpdated As DateTime
        Get
            If _BultinUpdated = Nothing Then
                _BultinUpdated = DateTime.Parse(BuiltinDocument.DocumentElement.GetAttribute("updated", BlacklistNamespace))
            End If
            Return _BultinUpdated
        End Get
    End Property

    Private Property _Updated As DateTime
    ''' <summary>
    ''' 外部から取得したNGワード置換ファイルの更新日時。
    ''' </summary>
    ''' <returns></returns>
    Friend Property Updated As DateTime
        Get
            Return _Updated
        End Get
        Private Set(ByVal value As DateTime)
            _Updated = value
        End Set
    End Property

    Private Shared Property _BuiltinDocument As XmlDocument
    ''' <summary>
    ''' 内蔵のNGワード置換ファイルを読み込んだXMLDocumentノード。
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property BuiltinDocument As XmlDocument
        Get
            If (_BuiltinDocument Is Nothing) Then
                _BuiltinDocument = New XmlDocument
                _BuiltinDocument.LoadXml(My.Resources.SubstList)
            End If
            Return _BuiltinDocument
        End Get
    End Property

    ''' <summary>
    ''' タイムタグ付きの歌詞に<see cref="MySettings.BlacklistCharactersSeparator"/>を挿入するための正規表現パターン。
    ''' </summary>
    Private Property PatternForReplacement As Regex

    ''' <summary>
    ''' タイムタグ無しの歌詞から強調表示箇所を抽出するための正規表現パターン。
    ''' </summary>
    Private Property HighlightPattern As Regex

    ''' <summary>
    ''' 1文字のNGワードに対する置換リスト。
    ''' </summary>
    ''' <example>
    ''' <code>
    ''' {{"姦", "姧"}, {"糞", "粪"}, {"ௌ": "ெ ௗ"}}
    ''' </code>
    ''' </example>
    Private Property Variants As Dictionary(Of String, String) = New Dictionary(Of String, String)

    ''' <summary>
    ''' 外部から取得したNGワード置換ファイルの保存先。
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property FilePath As String = Path.Combine(My.Settings.ParentPath, "SubstList.xml")

    Friend Sub New()
        If My.Settings.BlacklistCharactersSeparator.Length = 0 OrElse Regex.IsMatch(My.Settings.BlacklistCharactersSeparator, LetterOrDigitPatternString) Then
            Throw New Exception("My.Settings.BlacklistCharactersSeparator の値「" & My.Settings.BlacklistCharactersSeparator & "」は、記号ではありません。")
        End If

        Load()
    End Sub

    ''' <summary>
    ''' 運営NGワードを<see cref="MySettings.BlacklistCharactersSeparator"/>で分断します。
    ''' </summary>
    ''' <param name="lyrics">タイムタグ付きの歌詞。</param>
    ''' <returns></returns>
    Friend Function SplitWords(ByVal lyrics As String) As String
        Return PatternForReplacement.Replace(lyrics, "$0" & EscapeForReplacementPattern(My.Settings.BlacklistCharactersSeparator))
    End Function

    ''' <summary>
    ''' 1文字の運営NGワードを置換します。
    ''' </summary>
    ''' <param name="lyrics">タイムタグ付きの歌詞。</param>
    ''' <returns></returns>
    Friend Function ReplaceUnsplittableWords(ByVal lyrics As String) As String
        For Each pair As KeyValuePair(Of String, String) In Variants
            lyrics = lyrics.Replace(pair.Key, pair.Value)
        Next
        Return lyrics
    End Function

    ''' <summary>
    ''' 運営NGワードに一致する範囲のリストを返します。
    ''' </summary>
    ''' <param name="lyrics">タイムタグなしの歌詞。</param>
    ''' <returns>一致箇所のリスト (キーが開始位置、値が長さ)。範囲が重なっている場合があります。</returns>
    Friend Function Matches(ByVal lyrics As String) As Dictionary(Of Integer, Integer)
        Dim ranges = New Dictionary(Of Integer, Integer)
        For Each match As Match In HighlightPattern.Matches(lyrics)
            ranges.Add(match.Index, match.Length + match.Groups("last").Length)
        Next
        Return ranges
    End Function

    ''' <summary>
    ''' 外部 (<see cref="MySettings.SubstitutionListDownloadURL"/>) からNGワード置換ファイルをダウンロードして読み込みます。
    ''' </summary>
    Friend Sub Download()
        Dim doc = New XmlDocument
        Try
            doc.Load(My.Settings.SubstitutionListDownloadURL)

            Dim downloadedFileUpdated = DateTime.Parse(doc.DocumentElement.GetAttribute("updated", BlacklistNamespace))
            If downloadedFileUpdated > If(Updated <> Nothing, Updated, BultinUpdated) Then
                ' 外部から取得したファイルが新しければ
                LoadSubstitutionList(doc)
                doc.Save(FilePath)
                Updated = downloadedFileUpdated
                MessageBox.Show("NGワード置換ファイルの取得が完了しました。", My.Application.Info.Title, MessageBoxButton.OK, MessageBoxImage.Information)
            Else
                MessageBox.Show("NGワード置換ファイルは最新です。", My.Application.Info.Title, MessageBoxButton.OK, MessageBoxImage.Information)
            End If
        Catch ex As Exception
            MessageBox.Show(
                $"NGワード置換ファイルの取得中に例外が発生しました。{vbNewLine}{vbNewLine}例外情報:{vbNewLine}{ex}",
                My.Application.Info.Title,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            )
            Variants.Clear()
            Load()
        End Try
    End Sub

    ''' <summary>
    ''' 置換パターンにおける正規表現言語要素をエスケープします。
    ''' </summary>
    ''' <remarks>
    ''' 参照:
    ''' <list>
    '''     <item>
    '''         <term>“$” 文字の置換 | 正規表現での置換</term>
    '''         <desciption>https://msdn.microsoft.com/library/ewy2t5e0#DollarSign</desciption>
    '''     </item>
    ''' </list>
    ''' </remarks>
    ''' <param name="str"></param>
    ''' <returns></returns>
    Private Function EscapeForReplacementPattern(ByVal str As String) As String
        Return str.Replace("$", "$$")
    End Function

    ''' <summary>
    ''' NGワード置換ファイルを読み込みます。
    ''' </summary>
    Private Sub Load()
        If File.Exists(FilePath) Then
            Dim doc = New XmlDocument
            Try
                doc.Load(FilePath)

                Updated = DateTime.Parse(doc.DocumentElement.GetAttribute("updated", BlacklistNamespace))
                If Updated > BultinUpdated Then
                    ' 外部から取得したファイルが内蔵のものより新しければ
                    LoadSubstitutionList(doc)
                End If
            Catch ex As Exception
                PatternForReplacement = Nothing
                Updated = Nothing
                Variants.Clear()
                MessageBox.Show(
                    $"「{FilePath}」の読み込み中に例外が発生しました。内蔵ファイルから読み込んで続行します。{vbNewLine}{vbNewLine}例外情報:{vbNewLine}{ex}",
                    My.Application.Info.Title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                )
            End Try
        End If

        If PatternForReplacement Is Nothing Then
            LoadSubstitutionList(BuiltinDocument)
        End If
    End Sub

    ''' <summary>
    ''' 文書から必要なデータを取り出します。
    ''' </summary>
    ''' <param name="doc">『NGワード置換ファイル生成』から出力された文書。</param>
    Private Sub LoadSubstitutionList(ByVal doc As XmlDocument)
        Dim resolver = New XmlNamespaceManager(doc.NameTable)
        resolver.AddNamespace("subst", NCVSubstNamespace)
        resolver.AddNamespace("blacklist", BlacklistNamespace)

        Dim patternTemplate = doc.SelectSingleNode("//subst:subst_client[@blacklist:type='split']/subst:old", resolver).InnerText
        PatternForReplacement = New Regex(patternTemplate.Replace("[\s　]*", "(?:\[\d{2}:\d{2}:\d{2}\]|" & LetterOrDigitPatternString.Replace("[", "[^") & ")*"))
        HighlightPattern = New Regex(
            Regex.Replace(patternTemplate, "\(\?=([^()]*(?:(?:(?<open>\()[^()]*)+(?:(?<close-open>\))[^()]*)+)*)\)", "(?=(?<last>$1))").Replace("[\s　]*", LetterOrDigitPatternString.Replace("[", "[^") & "*")
        )

        For Each Subst As XmlElement In doc.SelectNodes("//subst:subst_client[@blacklist:type='unsplittable']", resolver)
            Variants(Subst.GetElementsByTagName("old", NCVSubstNamespace)(0).InnerText) = Subst.GetElementsByTagName("new", NCVSubstNamespace)(0).InnerText
        Next
    End Sub

End Class

