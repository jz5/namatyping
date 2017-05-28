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

    End Class

End Namespace
