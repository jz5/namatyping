Imports System.Xml.Linq

Namespace LiveStreaming

    ' <chat_result no="39" status="0" thread="1">

    Public Class LiveCommentResultMessage
        Inherits Message

#Region "Properties"

        Public Property No As Integer

        Public Property Status As Integer

        Public Property Success As Boolean

#End Region

        Friend Sub New(xmlText As String)
            MyBase.New(xmlText)
            Parse(XElement.Parse(xmlText))
        End Sub

        Private Sub Parse(xml As XElement)

            No = Integer.Parse(xml.@no)
            Success = xml.@status = "0"
            Status = Integer.Parse(xml.@status)

            ' MEMO status="4": PostKeyが無効
        End Sub
    End Class

End Namespace
