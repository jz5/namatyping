Imports System.Net

Namespace LiveStreaming

    ' <chat anonymity="1" date="1265287015" mail="184" no="1" premium="1" thread="1014106601" user_id="4m70Bm36foTJdSZkbqaK0O0fYSM" vpos="3443">わこつ</chat>

    Public Class LiveCommentMessage
        Inherits Message

#Region "Properties"

        Public Property UserId As String

        Public Property Text As String

        Public Property No As Integer

        Public Property Thread As Long

        Public Property Source As ChatSource

        Public Property DateTime As DateTime

        Public Property UnixTime As Long


        'Public Property ContainsNGWords As Boolean
        'Public ReadOnly Property ContainsNGWords() As Boolean
        '    Get
        '        Return Builder.ContainsNGWords
        '    End Get
        'End Property

        'Public Property IsByNGUser As Boolean
        'Public ReadOnly Property IsByNGUser() As Boolean
        '    Get
        '        Return Builder.IsByNGUser
        '    End Get
        'End Property

        'Public Property ContainsNGCommands As Boolean
        'Public ReadOnly Property ContainsNGCommands() As Boolean
        '    Get
        '        Return Builder.ContainsNGCommands
        '    End Get
        'End Property

        'Public Property HighlightedText As String
        'Public ReadOnly Property HighlightedText As String
        '    Get
        '        Return Builder.HighlightedText
        '    End Get
        'End Property

        'Public ReadOnly Property IsNG As Boolean
        '    Get
        '        Return ContainsNGWords OrElse IsByNGUser OrElse ContainsNGCommands
        '    End Get
        'End Property

        'Private _NGWords As New List(Of String)
        'Public ReadOnly Property NGWords As IList(Of String)
        '    Get
        '        Return _NGWords
        '    End Get
        'End Property

        Public Property Mail As String

        Public Property Anonymous As Boolean

        Public Property VPos As Integer


        Property Score As Integer
#End Region

        Public Sub New()

        End Sub

        Friend Sub New(xmlText As String)
            MyBase.New(xmlText)
            Parse(XElement.Parse(xmlText))
        End Sub

        Private Sub Parse(xml As XElement)
#If SILVERLIGHT Then
            Text = System.Windows.Browser.HttpUtility.HtmlDecode(xml.Value)
#ElseIf WINDOWSPHONE Then
            Text = System.Net.HttpUtility.HtmlDecode(xml.Value)
#Else
            Text = WebUtility.HtmlDecode(xml.Value)
#End If
            Integer.TryParse(xml.@no, No)
            Source = If(xml.@premium IsNot Nothing, DirectCast([Enum].Parse(GetType(ChatSource), xml.@premium, False), ChatSource), ChatSource.General)
            Thread = Long.Parse(xml.@thread)
            UserId = xml.@user_id
            UnixTime = Convert.ToInt64(xml.@date)
            DateTime = New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(_UnixTime).ToLocalTime
            Mail = If(xml.@mail, "")
            Anonymous = xml.@anonymity = "1"
            VPos = If(xml.@vpos IsNot Nothing, Integer.Parse(xml.@vpos), Nothing)

            Integer.TryParse(xml.@score, Score)

            ' MEMO yourpost = "1"
        End Sub


    End Class
End Namespace
