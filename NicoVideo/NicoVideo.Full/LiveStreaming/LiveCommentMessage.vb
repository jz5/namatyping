Imports System.Collections.ObjectModel
Imports System.Net
Imports System.Xml.Linq

Namespace LiveStreaming

    ' <chat anonymity="1" date="1265287015" mail="184" no="1" premium="1" thread="1014106601" user_id="4m70Bm36foTJdSZkbqaK0O0fYSM" vpos="3443">わこつ</chat>

    Public Class LiveCommentMessage
        Inherits Message

#Region "Properties"

        Public Property UserId As String

        'Private _UserId As String
        'Public ReadOnly Property UserId() As String
        '    Get
        '        Return _UserId
        '    End Get
        'End Property

        Public Property Text As String

        'Private _Text As String
        'Public ReadOnly Property Text() As String
        '    Get
        '        Return _Text
        '    End Get
        'End Property

        Public Property No As Integer

        'Private _No As Integer
        'Public ReadOnly Property No() As Integer
        '    Get
        '        Return _No
        '    End Get
        'End Property

        Public Property Thread As Long

        'Private _Thread As Long
        'Public ReadOnly Property Thread() As Long
        '    Get
        '        Return _Thread
        '    End Get
        'End Property

        Public Property Source As ChatSource

        'Private _Source As ChatSource
        'Public ReadOnly Property Source() As ChatSource
        '    Get
        '        Return _Source
        '    End Get
        'End Property

        Public Property DateTime As DateTime
        'Private _DateTime As DateTime
        'Public ReadOnly Property DateTime() As DateTime
        '    Get
        '        Return _DateTime
        '    End Get
        'End Property

        Public Property UnixTime As Long
        'Private _UnixTime As Long
        'Public ReadOnly Property UnixTime As Long
        '    Get
        '        Return _UnixTime
        '    End Get
        'End Property

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
        'Private _Mail As String
        'Public ReadOnly Property Mail As String
        '    Get
        '        Return _Mail
        '    End Get
        'End Property

        Public Property Anonymous As Boolean
        'Private _Anonymous As Boolean
        'Public ReadOnly Property Anonymous As Boolean
        '    Get
        '        Return _Anonymous
        '    End Get
        'End Property

        Public Property VPos As Integer
        'Private _VPos As Integer
        'Public ReadOnly Property VPos As Integer
        '    Get
        '        Return _VPos
        '    End Get
        'End Property

        Property Score As Integer
#End Region

        Public Sub New()

        End Sub

        Friend Sub New(ByVal xmlText As String)
            MyBase.New(xmlText)
            Parse(XElement.Parse(xmlText))
        End Sub

        Private Sub Parse(ByVal xml As XElement)
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
