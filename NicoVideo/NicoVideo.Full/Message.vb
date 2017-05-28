Public MustInherit Class Message

    Private _RawValue As String
    Public ReadOnly Property RawValue() As String
        Get
            Return _RawValue
        End Get
    End Property

    Public Sub New()
    End Sub

    Protected Sub New(ByVal rawValue As String)
        _RawValue = rawValue
    End Sub

End Class
