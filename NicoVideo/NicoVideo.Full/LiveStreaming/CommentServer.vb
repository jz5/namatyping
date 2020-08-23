Imports System.Text.RegularExpressions
Imports System.Runtime.Serialization

Namespace LiveStreaming

    <DataContract()>
    Public Class CommentServer

        Property LiveId As String
        <DataMember(Name:="addr")>
        Property Address As String
        <DataMember(Name:="port")>
        Property Port As Integer
        <DataMember(Name:="thread")>
        Property Thread As Long

        Public ReadOnly Property IsValid As Boolean
            Get
                Return LiveId IsNot Nothing AndAlso Regex.IsMatch(LiveId, "^lv\d+$") AndAlso
                    Address IsNot Nothing AndAlso
                    Port > 0 AndAlso
                    Thread > 0

            End Get
        End Property

    End Class

End Namespace
