Imports Pronama.NicoVideo
Imports Pronama.NicoVideo.LiveStreaming
Imports System.Threading.Tasks
Imports System.Net

Module Module1

    Private client As New LiveProgramClient
    Private WithEvents alertClient As New LiveAlertClient

    Private Const StreamInfoUrlFormat As String = "http://live.nicovideo.jp/api/getstreaminfo/{0}"
    Private Session As String = "user_session_806024_1207976940769993467"

    Sub Main()
        Dim session = AuthClient.GetUserSession("jz5@katamari.jp", "0830nmoti")
        session.ContinueWith(Sub(t)
                                 Console.WriteLine(t.Result)
                             End Sub)

        'LiveAlertClient.GetAlertInfoAsync() _
        '    .ContinueWith(
        '        Sub(t)
        '            If t.IsFaulted Then
        '                Exit Sub
        '            End If

        '            alertClient.ConnectAsync(t.Result)

        '        End Sub)

        LiveProgramClient.GetCommentServersAsync("lv103146231") _
            .ContinueWith(
                Sub(t)
                    If t.IsFaulted Then
                        Exit Sub
                    End If

                    client.ConnectAsync(t.Result.First())
                End Sub)

        AddHandler client.CommentReceived,
            Sub(s As Object, e As CommentReceivedEventArgs)
                Console.WriteLine(e.Comment.Text)
                Console.WriteLine(e.Comment.Score)
            End Sub

        'NicoVideoWeb.GetLiveProgramAsync("lv68084536").ContinueWith(
        '    Sub(r)
        '        Console.WriteLine(r.Result.Id)
        '        Console.WriteLine(r.Result.Title)
        '        Console.WriteLine(r.Result.ChannelCommunity.Id)
        '        Console.WriteLine(r.Result.ChannelCommunity.Name)
        '        Console.WriteLine(r.Result.ChannelCommunity.IconUri)
        '        If r.Result.ChannelCommunity.Type = ChannelCommunityType.Community Then
        '            Console.WriteLine(DirectCast(r.Result.ChannelCommunity, Community).Level)
        '        End If

        '        Dim client = New LiveProgramClient
        '        Dim c = LiveProgramClient.GetCommentServersAsync(r.Result.Id) _
        '                .ContinueWith(
        '                    Sub(t)
        '                        If t.Exception IsNot Nothing Then

        '                            Exit Sub
        '                        End If
        '                    End Sub)
        '    End Sub)

        Console.ReadLine()

        '   Foo(New Uri("http://yahoo.co.jp")).ContinueWith(
        'Sub(t)
        '        Console.WriteLine(t.Result)
        '    End Sub)

        '   GetPlayerStatusAsync("", "").ContinueWith(
        '       Sub(t)
        '               x(t.Result)
        '           End Sub)
    End Sub

    Private Sub x(x As XDocument)

    End Sub

    Private Sub alertClient_LiveProgramPreviewStart(sender As Object, e As Pronama.NicoVideo.LiveStreaming.LiveProgramAlertedEventArgs) Handles alertClient.LiveProgramPreviewStart
        Dim p = e.LiveProgram

        If p.IsOfficial Then

            GetPlayerStatusAsync(p.Id, Session).ContinueWith(
                Sub(t)
                        Process.Start("http://live.nicovideo.jp/watch/" & p.Id)
                        Console.WriteLine(t.Result)
                    End Sub)
        End If

        Console.WriteLine("{0},{1},{2}", p.Id, If(p.IsOfficial, "official", p.ChannelCommunity.Id), p.CasterId)


    End Sub


    Public Function GetPlayerStatusAsync(liveId As String, session As String) As Task(Of XDocument)
        Const PlayerStatusUrlFormat As String = "http://live.nicovideo.jp/api/getplayerstatus?v={0}"

        Dim uri = New Uri(String.Format(PlayerStatusUrlFormat, liveId))

        Dim req = DirectCast(HttpWebRequest.Create(uri), HttpWebRequest)
        req.CookieContainer = New CookieContainer
        req.CookieContainer.Add(New Uri("http://live.nicovideo.jp"), New Cookie("user_session", session))

        Dim webTask = Task.Factory.FromAsync(Of WebResponse)(AddressOf req.BeginGetResponse, AddressOf req.EndGetResponse, Nothing) _
                      .ContinueWith(Of XDocument)(
                          Function(t As Task(Of WebResponse))
                                  Dim response = DirectCast(t.Result, HttpWebResponse)
                                  Dim body As String
                                  Using stream = response.GetResponseStream
                                      Using reader = New System.IO.StreamReader(stream, System.Text.Encoding.UTF8)
                                          body = reader.ReadToEnd
                                      End Using
                                  End Using

                                  Return XDocument.Parse(body)
                              End Function)

        Return webTask
    End Function


    Private Function Foo(uri As Uri) As Task(Of String)
        Dim req = DirectCast(HttpWebRequest.Create(uri), HttpWebRequest)
        Dim webTask = Task.Factory.FromAsync(Of WebResponse)(AddressOf req.BeginGetResponse, AddressOf req.EndGetResponse, Nothing) _
                      .ContinueWith(Of String)(
                          Function(t As Task(Of WebResponse))
                                  Dim response = DirectCast(t.Result, HttpWebResponse)
                                  Dim body As String
                                  Using stream = response.GetResponseStream
                                      Using reader = New System.IO.StreamReader(stream, System.Text.Encoding.UTF8)
                                          body = reader.ReadToEnd
                                      End Using
                                  End Using

                                  Return body
                              End Function)

        Return webTask
    End Function

End Module
