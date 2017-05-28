Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Net
Imports System.Xml
Imports System.Globalization

#If WINDOWSPHONE Then
Imports Microsoft.Phone.Reactive
#Else
Imports System.Threading.Tasks
#End If
Imports Pronama.NicoVideo

Namespace LiveStreaming

    Public Class LiveAlertClient

        Private Const ThreadFormat As String = "<thread thread=""{0}"" version=""20061206"" res_from=""{1}"" />" & vbNullChar
        Private Const AlertInfoUrlFormat As String = "http://live.nicovideo.jp/api/getalertinfo"

#Region "Events"
        Public Event ConnectedChanged As EventHandler(Of EventArgs)
        Protected Sub OnConnectedChanged(ByVal e As EventArgs)
            RaiseEvent ConnectedChanged(Me, e)
        End Sub

        Public Event ConnectCompleted As EventHandler(Of AsyncCompletedEventArgs)
        Protected Sub OnConnectCompleted(ByVal e As AsyncCompletedEventArgs)
            RaiseEvent ConnectCompleted(Me, e)
        End Sub

        Public Event LiveProgramPreviewStart As EventHandler(Of LiveProgramAlertedEventArgs)
        Protected Sub OnLiveProgramAlert(ByVal e As LiveProgramAlertedEventArgs)
            RaiseEvent LiveProgramPreviewStart(Me, e)
        End Sub

#End Region

        Private SocketAsyncEventArgs As SocketAsyncEventArgs
        Private Socket As Socket
        Private AlertInfo As AlertInfo

        Private _Connected As Boolean
        Public ReadOnly Property Connected As Boolean
            Get
                Return Socket IsNot Nothing AndAlso Socket.Connected
            End Get
        End Property


#If WINDOWSPHONE Then
        'Public Shared Function GetAlertInfoAsync(liveId As String) As IObservable(Of AlertInfo)
        '    Dim uri = New Uri(AlertInfoUrlFormat)

        '    Dim req = HttpWebRequest.Create(uri)
        '    Dim o = Observable.FromAsyncPattern(Of WebResponse)(AddressOf req.BeginGetResponse, AddressOf req.EndGetResponse) _
        '            .Invoke _
        '            .Select(Function(res)
        '                        Dim body As String
        '                        Using stream = res.GetResponseStream
        '                            Using reader = New System.IO.StreamReader(stream, System.Text.Encoding.UTF8)
        '                                body = reader.ReadToEnd
        '                            End Using
        '                        End Using
        '                        Return CreateAlertInfo(body)
        '                    End Function)

        '    Return o
        'End Function
#Else
        Public Shared Function GetAlertInfoAsync() As Task(Of AlertInfo)
            Dim uri = New Uri(AlertInfoUrlFormat)

            Dim req = HttpWebRequest.Create(uri)
            Dim webTask = Task.Factory.FromAsync(Of WebResponse)(AddressOf req.BeginGetResponse, AddressOf req.EndGetResponse, Nothing) _
                          .ContinueWith(Of AlertInfo)(
                              Function(t As Task(Of WebResponse))
                                  Dim response = DirectCast(t.Result, HttpWebResponse)
                                  Dim body As String
                                  Using stream = response.GetResponseStream
                                      Using reader = New System.IO.StreamReader(stream, System.Text.Encoding.UTF8)
                                          body = reader.ReadToEnd
                                      End Using
                                  End Using

                                  Return CreateAlertInfo(body)
                              End Function)

            Return webTask
        End Function

#End If

        Private Shared Function CreateAlertInfo(ByVal xmlDocumentText As String) As AlertInfo

            Dim doc As XDocument = Nothing
            Try
                doc = XDocument.Parse(xmlDocumentText)
            Catch xmlEx As XmlException
                Throw New NicoVideoException(Nothing, xmlEx)
            End Try

            If doc.<getalertstatus>.@status.ToUpperInvariant <> "OK" Then
                Dim ex = New NicoVideoException(doc.<getalertstatus>.@status,
                                                doc.<getalertstatus>.<error>.<code>.Value,
                                                doc.<getalertstatus>.<error>.<description>.Value,
                                                Nothing)
                Throw ex
            End If

            Dim ai = New AlertInfo
            Try
                Dim cs = New CommentServer

                cs.Address = doc.<getalertstatus>.<ms>.<addr>.Value
                cs.Port = Integer.Parse(doc.<getalertstatus>.<ms>.<port>.Value)
                cs.Thread = Long.Parse(doc.<getalertstatus>.<ms>.<thread>.Value)
                ai.CommentServer = cs

                For Each cid In doc.<getalertstatus>...<community_id>
                    ai.ChannelAndCommunityIds.Add(cid.Value)
                Next

            Catch ex As Exception
                Dim nicoVideoEx = New NicoVideoException(doc.<getalertstatus>.@status,
                                                         Nothing,
                                                         Nothing,
                                                         ex)
                Throw nicoVideoEx
            End Try
            Return ai

        End Function


        Public Sub ConnectAsync(alertInfo As AlertInfo)
            Me.AlertInfo = alertInfo

            Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            SocketAsyncEventArgs = New SocketAsyncEventArgs

#If SILVERLIGHT Then
            args.SocketClientAccessPolicyProtocol = SocketClientAccessPolicyProtocol.Tcp
            args.RemoteEndPoint = New DnsEndPoint(server.Address, server.Port)
#ElseIf WINDOWSPHONE Then
            args.RemoteEndPoint = New DnsEndPoint(server.Address, server.Port)
#Else
            Dim ip = Dns.GetHostEntry(alertInfo.CommentServer.Address).AddressList.FirstOrDefault
            SocketAsyncEventArgs.RemoteEndPoint = New IPEndPoint(ip, alertInfo.CommentServer.Port)
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


        Private Sub Socket_ConnectCompleted(ByVal sender As Object, ByVal e As SocketAsyncEventArgs)
            If e.SocketError <> SocketError.Success Then
                OnConnectCompleted(New AsyncCompletedEventArgs(New SocketException(e.SocketError), False, Nothing))
                Exit Sub
            End If

            Dim exception As Exception = Nothing
            Try
                Dim initialReadCount = Convert.ToInt32(e.UserToken)
                Dim tag = String.Format(CultureInfo.InvariantCulture, ThreadFormat, Me.AlertInfo.CommentServer.Thread, 0) & vbNullChar

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

        Private Sub Socket_SendCompleted(ByVal sender As Object, ByVal e As SocketAsyncEventArgs)
            Dim buffer(4096) As Byte
            Dim args = New SocketAsyncEventArgs
            args.SetBuffer(buffer, 0, buffer.Length)
            AddHandler args.Completed, New EventHandler(Of SocketAsyncEventArgs)(AddressOf Socket_ReceiveCompleted)

            ReceiveData(args)
        End Sub

        Private ContextBuffer As New List(Of Byte)

        Private Sub Socket_ReceiveCompleted(ByVal sender As Object, ByVal e As SocketAsyncEventArgs)
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

        Private Sub ReceiveData(ByVal e As SocketAsyncEventArgs)
            If Not Socket.Connected Then
                OnConnectedChanged(EventArgs.Empty)
                Exit Sub
            End If

            If Not Socket.ReceiveAsync(e) Then
                ' 同期時
                Socket_ReceiveCompleted(Socket, e)
            End If
        End Sub

        Private Sub ParseReceivedText(ByVal text As String)
            If Not text.StartsWith("<chat ") Then
                Exit Sub
            End If

            Dim p = New LiveProgram
            Dim exception As Exception = Nothing

            Try
                Dim doc = XDocument.Parse(text)

                Dim values = doc.<chat>.Value.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
                If values.Length < 3 Then
                    Exit Sub
                End If

                ' live id
                p.Id = "lv" & values(0)

                ' station id
                If values(1) = "official" Then
                    p.IsOfficial = True
                Else
                    If values(1).StartsWith("ch") Then
                        p.ChannelCommunity = New Channel() With {.Id = values(1)}
                    ElseIf values(1).StartsWith("co") Then
                        p.ChannelCommunity = New Community() With {.Id = values(1)}
                    End If
                End If

                ' caster id
                p.CasterId = Convert.ToInt64(values(2))

            Catch ex As Exception
                exception = ex
            End Try

            ' Raise event
            OnLiveProgramAlert(New LiveProgramAlertedEventArgs(p, exception))

        End Sub
    End Class

End Namespace
