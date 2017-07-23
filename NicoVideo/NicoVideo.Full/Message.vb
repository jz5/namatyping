Public MustInherit Class Message
    Public ReadOnly Property RawValue As String

    Public Sub New()
    End Sub

    Protected Sub New(rawValue As String)
        Me.RawValue = rawValue
    End Sub
End Class
