Imports System.Net
Imports System.Net.Sockets
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Xml

Namespace LiveStreaming

    Public Class AlertInfo

        Protected Const AlertInfoUrlFormat As String = "http://live.nicovideo.jp/api/getalertinfo"

        Private _UserId As String
        Public ReadOnly Property UserId As String
            Get
                Return _UserId
            End Get
        End Property

        Private _UserHash As String
        Public ReadOnly Property UserHash As String
            Get
                Return _UserHash
            End Get
        End Property

        Property CommentServer As CommentServer

        'Public ReadOnly Property CommentServer As CommentServer
        '    Get
        '        Return _CommentServer
        '    End Get
        'End Property

        Private _ChannelAndCommunityIds As New List(Of String)
        Public ReadOnly Property ChannelAndCommunityIds As IList(Of String)
            Get
                Return _ChannelAndCommunityIds
            End Get
        End Property

        'Private Ticket As String

        'Public Sub New()
        '    MyBase.New()

        '    ' 匿名で情報取得用
        '    Dim content = GetContent(New Uri("http://live.nicovideo.jp/api/getalertinfo"))
        '    Fill(content)
        'End Sub

        'Public Sub New(ByVal email As String, ByVal password As String)
        '    MyBase.New()

        '    Throw New NotSupportedException
        '    'Using auth = New Authentication
        '    '    Ticket = auth.GetLiveAlertTicket(email, password)
        '    'End Using

        '    'Dim content = GetContent(New Uri("http://live.nicovideo.jp/api/getalertstatus?ticket=" & Ticket))
        '    'Fill(content)

        'End Sub

        ''Public Shared Function Load() As AlertInfo
        ''    Return New AlertInfo
        ''End Function

        ''Public Shared Function Load(ByVal email As String, ByVal password As String) As AlertInfo
        ''    Return New AlertInfo(email, password)
        ''End Function

        'Private Sub Fill(ByVal xmlDocumentText As String)

        '    Dim doc As XDocument = Nothing
        '    Try
        '        doc = XDocument.Parse(xmlDocumentText)
        '    Catch xmlEx As XmlException
        '        Throw New NicoVideoException(Nothing, xmlEx)
        '    End Try

        '    If doc.<getalertstatus>.@status.ToUpperInvariant <> "OK" Then
        '        Dim ex = New NicoVideoException(doc.<getalertstatus>.@status,
        '                                        doc.<getalertstatus>.<error>.<code>.Value,
        '                                        doc.<getalertstatus>.<error>.<description>.Value,
        '                                        Nothing)
        '        Throw ex
        '    End If

        '    Try
        '        _CommentServer = New CommentServer

        '        _CommentServer.Address = doc.<getalertstatus>.<ms>.<addr>.Value
        '        _CommentServer.Port = Integer.Parse(doc.<getalertstatus>.<ms>.<port>.Value)
        '        _CommentServer.Thread = Long.Parse(doc.<getalertstatus>.<ms>.<thread>.Value)


        '        For Each cid In doc.<getalertstatus>...<community_id>
        '            _ChannelAndCommunityIds.Add(cid.Value)
        '        Next

        '    Catch ex As Exception
        '        Dim nicoVideoEx = New NicoVideoException(doc.<getalertstatus>.@status,
        '                                                 Nothing,
        '                                                 Nothing,
        '                                                 ex)
        '        Throw nicoVideoEx
        '    End Try

        'End Sub

    End Class

End Namespace

