Imports System.ComponentModel
Imports System.Globalization
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.Serialization.Json
Imports System.Runtime.Serialization

#If WINDOWSPHONE Then
Imports Microsoft.Phone.Reactive
#Else
Imports System.Threading.Tasks
#End If

Namespace LiveStreaming
    Public Class LiveProgramClient

        Private Const CloudAppUriFormat As String = "http://pronama-api.azurewebsites.net/NicoLiveCommentServer/{0}"

        <DataContract()>
        Private Class ErrorResult
            <DataMember(Name:="code")>
            Property Code As String
        End Class

#If WINDOWSPHONE Then
        Public Shared Function GetCommentServersAsync(liveId As String) As IObservable(Of IList(Of CommentServer))
            Dim uri = New Uri(String.Format(CloudAppUriFormat, liveId))

            Dim req = HttpWebRequest.Create(uri)
            Dim o = Observable.FromAsyncPattern(Of WebResponse)(AddressOf req.BeginGetResponse, AddressOf req.EndGetResponse) _
                    .Invoke _
                    .Select(Function(res)
                                Dim body As String
                                Using stream = res.GetResponseStream
                                    Using reader = New System.IO.StreamReader(stream, System.Text.Encoding.UTF8)
                                        body = reader.ReadToEnd
                                    End Using
                                End Using
                                Return CreateCommentServers(body)
                            End Function)

            Return o
        End Function
#Else
        Public Shared Function GetCommentServerAsync(liveId As String) As Task(Of CommentServer)
            Dim uri = New Uri(String.Format(CloudAppUriFormat, liveId))

            Dim req = HttpWebRequest.Create(uri)
            Dim webTask = Task.Factory.FromAsync(Of WebResponse)(AddressOf req.BeginGetResponse, AddressOf req.EndGetResponse, Nothing) _
                          .ContinueWith(Of CommentServer)(
                              Function(t As Task(Of WebResponse))
                                  Dim response = DirectCast(t.Result, HttpWebResponse)
                                  Dim body As String
                                  Using stream = response.GetResponseStream
                                      Using reader = New System.IO.StreamReader(stream, System.Text.Encoding.UTF8)
                                          body = reader.ReadToEnd
                                      End Using
                                  End Using

                                  Return CreateCommentServer(body)
                              End Function)

            Return webTask
        End Function

#End If

        Private Shared Function CreateCommentServer(json As String) As CommentServer

            Dim serializer As DataContractJsonSerializer
            Dim buf() As Byte

            serializer = New DataContractJsonSerializer(GetType(IList(Of CommentServer)))
            buf = System.Text.Encoding.UTF8.GetBytes(json)

            Dim servers = DirectCast(serializer.ReadObject(New System.IO.MemoryStream(buf)), IList(Of CommentServer))
            If servers.Count > 0 Then
                Return servers(0)
            End If

            serializer = New DataContractJsonSerializer(GetType(ErrorResult))
            buf = System.Text.Encoding.UTF8.GetBytes(json)
            Dim result = DirectCast(serializer.ReadObject(New System.IO.MemoryStream(buf)), ErrorResult)
            Throw New Exception(result.Code)

        End Function

        'Private Shared Function CreateCommentServer(json As String) As CommentServer
        '    Dim s = New CommentServer

        '    Dim m1 = Regex.Match(json, "{""addr"":""(?<addr>.+)"",""port"":(?<port>\d+),""thread"":(?<thread>\d+),""room_label"":""(?<room>.+)""}")
        '    If m1.Success Then
        '        s.Address = m1.Groups("addr").Value
        '        s.Port = Convert.ToInt32(m1.Groups("port").Value)
        '        s.Thread = Convert.ToInt64(m1.Groups("thread").Value)
        '        s.RoomLabel = m1.Groups("room").Value
        '    Else
        '        Dim m2 = Regex.Match(json, "{""code"":""(?<code>.+)""}")
        '        If m2.Success Then
        '            Throw New Exception(m2.Groups("code").Value)
        '        End If
        '    End If

        '    Return s
        'End Function

#Region "Events"
        Public Event ConnectedChanged As EventHandler(Of EventArgs)
        Protected Sub OnConnectedChanged(e As EventArgs)
            RaiseEvent ConnectedChanged(Me, e)
        End Sub

        Public Event ConnectCompleted As EventHandler(Of AsyncCompletedEventArgs)
        Protected Sub OnConnectCompleted(e As AsyncCompletedEventArgs)
            RaiseEvent ConnectCompleted(Me, e)
        End Sub

        Public Event CommentReceived As EventHandler(Of CommentReceivedEventArgs)
        Protected Sub OnCommentReceived(e As CommentReceivedEventArgs)
            RaiseEvent CommentReceived(Me, e)
        End Sub

#End Region

        Private _Connected As Boolean
        Public ReadOnly Property Connected As Boolean
            Get
                Return Socket IsNot Nothing AndAlso Socket.Connected
            End Get
        End Property


        Private SocketAsyncEventArgs As SocketAsyncEventArgs
        Private Socket As Socket
        Private Const ThreadFormat As String = "<thread thread=""{0}"" version=""20061206"" res_from=""{1}"" />"

        Private CommentServer As CommentServer

        Public Sub ConnectAsync(server As CommentServer)
            Me.CommentServer = server

            Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            SocketAsyncEventArgs = New SocketAsyncEventArgs

#If SILVERLIGHT Then
            SocketAsyncEventArgs.SocketClientAccessPolicyProtocol = SocketClientAccessPolicyProtocol.Tcp
            SocketAsyncEventArgs.RemoteEndPoint = New DnsEndPoint(server.Address, server.Port)
#ElseIf WINDOWSPHONE Then
            SocketAsyncEventArgs.RemoteEndPoint = New DnsEndPoint(server.Address, server.Port)
#Else
            Dim ip = Dns.GetHostEntry(server.Address).AddressList.FirstOrDefault
            SocketAsyncEventArgs.RemoteEndPoint = New IPEndPoint(ip, server.Port)
            SocketAsyncEventArgs.AcceptSocket = Nothing
#End If
            AddHandler SocketAsyncEventArgs.Completed, AddressOf Socket_ConnectCompleted
            Socket.ConnectAsync(SocketAsyncEventArgs)

        End Sub

        Public Sub Close()
            If Socket IsNot Nothing AndAlso Socket.Connected Then
                Me.Socket.Close()
            End If
        End Sub

        Private Sub Socket_ConnectCompleted(sender As Object, e As SocketAsyncEventArgs)
            If e.SocketError <> SocketError.Success Then
                OnConnectCompleted(New AsyncCompletedEventArgs(New SocketException(e.SocketError), False, Nothing))
                Exit Sub
            End If

            Dim exception As Exception = Nothing
            Try
                Dim initialReadCount = Convert.ToInt32(e.UserToken)
                Dim tag = String.Format(CultureInfo.InvariantCulture, ThreadFormat, Me.CommentServer.Thread, 0) & vbNullChar

                Dim writeBuffer = System.Text.Encoding.UTF8.GetBytes(tag)
                Dim args = New SocketAsyncEventArgs
                args.SetBuffer(writeBuffer, 0, writeBuffer.Length)
                AddHandler args.Completed, AddressOf Socket_SendCompleted

                Socket.SendAsync(args)

            Catch ex As Exception
                exception = ex
            End Try

            If exception Is Nothing Then
                _Connected = True
                OnConnectedChanged(EventArgs.Empty)
            End If

            OnConnectCompleted(New AsyncCompletedEventArgs(exception, False, Nothing))
        End Sub

        Private Sub Socket_SendCompleted(sender As Object, e As SocketAsyncEventArgs)
            Dim buffer(4096) As Byte
            Dim args = New SocketAsyncEventArgs
            args.SetBuffer(buffer, 0, buffer.Length)
            AddHandler args.Completed, New EventHandler(Of SocketAsyncEventArgs)(AddressOf Socket_ReceiveCompleted)

            ReceiveData(args)
        End Sub

        Private ContextBuffer As New List(Of Byte)

        Private Sub Socket_ReceiveCompleted(sender As Object, e As SocketAsyncEventArgs)
            If e.BytesTransferred > 0 Then


                For i = 0 To e.BytesTransferred - 1

                    If e.Buffer(i) <> 0 Then
                        ContextBuffer.Add(e.Buffer(i))
                        Continue For
                    End If

                    Dim buffer = ContextBuffer.ToArray
                    Dim text = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length)
                    ContextBuffer.Clear()
                    ParseReceivedText(text)

                Next

            End If
            ReceiveData(e)
        End Sub

        Private Sub ReceiveData(e As SocketAsyncEventArgs)
            If Not Socket.Connected Then
                OnConnectedChanged(EventArgs.Empty)
                Exit Sub
            End If

            If Not Socket.ReceiveAsync(e) Then
                ' 同期時
                Socket_ReceiveCompleted(Socket, e)
            End If
        End Sub

        Private Sub ParseReceivedText(text As String)

            Dim message As Message = Nothing
            Try
                message = MessageBuilder.BuildMessage(text)
            Catch ex As Exception
                Throw
                Exit Sub
            End Try

            If TypeOf message Is LiveCommentMessage Then
                Dim c = DirectCast(message, LiveCommentMessage)

                OnCommentReceived(New CommentReceivedEventArgs(c))
                'If c.Text = "/disconnect" AndAlso c.Source = ChatSource.Operator Then
                '    Exit Sub
                'End If

            ElseIf TypeOf message Is LiveCommentResultMessage Then

            ElseIf TypeOf message Is LiveThreadMessage Then

            End If

        End Sub

    End Class

End Namespace