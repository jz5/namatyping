Namespace Model

    Public Class LyricLine

        Property Text As String
        Property TextWithTimeTag As String
        Property TimePosition As Double

        Private _Words As IList(Of String) = New List(Of String)
        Public ReadOnly Property Words() As IList(Of String)
            Get
                Return _Words
            End Get
        End Property

        Private _Yomi As IList(Of String) = New List(Of String)
        Public ReadOnly Property Yomi() As IList(Of String)
            Get
                Return _Yomi
            End Get
        End Property

        ''' <summary>
        ''' 強調表示を行う範囲のリスト (キーが開始位置、値が長さ)。
        ''' </summary>
        ''' <example>
        ''' <code>
        ''' {{0, 3}, {10, 2}}
        ''' </code>
        ''' </example>
        Public Property HighlightRanges As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)

    End Class

End Namespace
