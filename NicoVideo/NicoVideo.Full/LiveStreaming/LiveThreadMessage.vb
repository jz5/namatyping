Imports System.Xml.Linq

Namespace LiveStreaming

    ' <thread last_res="38" resultcode="0" revision="1" server_time="1265287761" thread="1014106601" ticket="0xb3b8860"/>

    Public Class LiveThreadMessage
        Inherits Message

#Region "Properties"

        Property LastNo As Integer
        Property ResultCode As Integer
        Property Revision As Integer
        Property ServerUnixTime As Long
        Property Thread As Long
        Property Ticket As String

#End Region

        Public Sub New()
        End Sub

        Friend Sub New(ByVal xmlText As String)
            MyBase.New(xmlText)
            Parse(XElement.Parse(xmlText))
        End Sub

        Private Sub Parse(ByVal xml As XElement)

            LastNo = If(xml.@last_res IsNot Nothing, Integer.Parse(xml.@last_res), 0)
            ResultCode = Integer.Parse(xml.@resultcode)
            Revision = Integer.Parse(xml.@revision)
            ServerUnixTime = Long.Parse(xml.@server_time)
            Thread = Long.Parse(xml.@thread)
            Ticket = xml.@ticket

        End Sub

    End Class

End Namespace