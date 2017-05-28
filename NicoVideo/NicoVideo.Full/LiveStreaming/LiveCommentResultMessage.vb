Imports System.Xml.Linq

Namespace LiveStreaming

    ' <chat_result no="39" status="0" thread="1">

    Public Class LiveCommentResultMessage
        Inherits Message

#Region "Properties"

        Private _No As Integer
        Public ReadOnly Property No As Integer
            Get
                Return _No
            End Get
        End Property

        Private _Status As Integer
        Public ReadOnly Property Status As Integer
            Get
                Return _Status
            End Get
        End Property

        Private _Success As Boolean
        Public ReadOnly Property Success() As Boolean
            Get
                Return _Success
            End Get
        End Property

#End Region

        Friend Sub New(ByVal xmlText As String)
            MyBase.New(xmlText)
            Parse(XElement.Parse(xmlText))
        End Sub

        Private Sub Parse(ByVal xml As XElement)

            _No = Integer.Parse(xml.@no)
            _Success = xml.@status = "0"
            _Status = Integer.Parse(xml.@status)

            ' MEMO status="4": PostKeyが無効
        End Sub

    End Class

End Namespace
