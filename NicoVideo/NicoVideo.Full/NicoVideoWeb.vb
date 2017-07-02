Imports System.Net
#If WINDOWSPHONE Then
Imports Microsoft.Phone.Reactive
#Else
Imports System.Threading.Tasks
#End If
Imports System.Text.RegularExpressions
Imports Pronama.NicoVideo.LiveStreaming

Public Class NicoVideoWeb

    'Public Shared Function GetLiveProgram(liveId As String) As Task(Of LiveProgram)
    '    Const uriFormat As String = "http://live.nicovideo.jp/embed/{0}"
    '    Dim uri = New Uri(String.Format(uriFormat, liveId))

    '    Dim req = HttpWebRequest.Create(uri)
    '    Dim webTask = Task.Factory.FromAsync(Of WebResponse)(AddressOf req.BeginGetResponse, AddressOf req.EndGetResponse, Nothing) _
    '                  .ContinueWith(Of LiveProgram)(
    '                      Function(t As Task(Of WebResponse))
    '                          Dim response = DirectCast(t.Result, HttpWebResponse)
    '                          Dim body As String
    '                          Using stream = response.GetResponseStream
    '                              Using reader = New System.IO.StreamReader(stream, System.Text.Encoding.UTF8)
    '                                  body = reader.ReadToEnd
    '                              End Using
    '                          End Using

    '                          Dim p = New LiveProgram With {.Id = liveId, .Community = New Community}

    '                          Dim m1 = Regex.Match(body, "<title>(?<title>.*?)</title>")
    '                          If m1.Success Then
    '                              p.Title = m1.Groups("title").Value
    '                              If p.Title.StartsWith("ニコニコ生放送‐") Then
    '                                  p.Title = p.Title.Substring("ニコニコ生放送‐".Length)
    '                              End If
    '                          End If

    '                          Dim m2 = Regex.Match(body, "<span class=""community"" title=""(?<name>.*?)"">.*?</span>")
    '                          If m2.Success Then
    '                              p.Community.Name = m2.Groups("name").Value
    '                          End If

    '                          Dim m3 = Regex.Match(body, "<img src=""(?<img>http://icon\.nimg\.jp/community/(?<id>.*?)\.jpg\?\d+)"" class=""banner"">")
    '                          If m3.Success Then
    '                              p.Community.Id = m3.Groups("id").Value
    '                              p.Community.IconUri = New Uri(m3.Groups("img").Value)
    '                          End If

    '                          Return p
    '                      End Function)
    '    Return webTask
    'End Function

    'Public Shared Function GetCommunityLevel(id As String) As Task(Of Integer)
    '    Const uriFormat As String = "http://ext.nicovideo.jp/thumb_community/{0}"

    '    Dim uri = New Uri(String.Format(uriFormat, id))
    '    Dim req = HttpWebRequest.Create(uri)

    '    Dim webTask = Task.Factory.FromAsync(Of WebResponse)(
    '        AddressOf req.BeginGetResponse,
    '        AddressOf req.EndGetResponse,
    '        Nothing) _
    '    .ContinueWith(Of Integer)(
    '        Function(t As Task(Of WebResponse))
    '            Dim response = DirectCast(t.Result, HttpWebResponse)
    '            Dim body As String
    '            Using stream = response.GetResponseStream
    '                Using reader = New System.IO.StreamReader(stream, System.Text.Encoding.UTF8)
    '                    body = reader.ReadToEnd
    '                End Using
    '            End Using

    '            Dim level As Integer
    '            Dim m2 = Regex.Match(body, "レベル：<strong>(?<lv>\d+)</strong>")
    '            If m2.Success Then
    '                level = Convert.ToInt32(m2.Groups("lv").Value)
    '            End If

    '            Return level
    '        End Function)

    '    Return webTask
    'End Function

    Private Const LiveProgramUriFormat As String = "http://live.nicovideo.jp/watch/{0}"

#If WINDOWSPHONE Then

    Public Shared Function GetLiveProgramAsync(id As String) As IObservable(Of LiveProgram)
        Dim client = New WebClient With {
            .Encoding = System.Text.Encoding.UTF8}
        Dim o = Observable.FromEvent(Of DownloadStringCompletedEventArgs)(client, "DownloadStringCompleted"). _
            Select(Function(p)
                       Return CreateLiveProgram(p.EventArgs.Result)
                   End Function)

        client.DownloadStringAsync(New Uri(String.Format(LiveProgramUriFormat, id)))
        Return o
    End Function

#Else
    Public Shared Function GetLiveProgramAsync(id As String) As Task(Of LiveProgram)
        Dim tcs = New TaskCompletionSource(Of LiveProgram)

        Dim client = New WebClient With {
            .Encoding = System.Text.Encoding.UTF8}

        AddHandler client.DownloadStringCompleted,
            Sub(sender As Object, e As DownloadStringCompletedEventArgs)
                If e.Cancelled Then
                    tcs.TrySetCanceled()
                ElseIf e.Error IsNot Nothing Then
                    tcs.TrySetException(e.Error)
                Else
                    tcs.TrySetResult(CreateLiveProgram(e.Result))
                End If
            End Sub
        client.DownloadStringAsync(New Uri(String.Format(LiveProgramUriFormat, id)))

        Return tcs.Task
    End Function
#End If

    Private Shared Function CreateLiveProgram(livePageHtml As String) As LiveProgram

        Dim p = New LiveProgram

        ' id, icon uri
        Dim id As String
        Dim m0 = Regex.Match(livePageHtml, "<img src=""(?<img>https?://icon\.nimg\.jp/(community/\d+|channel/s)/(?<id>.+?)\.jpg\?\d+)")
        If m0.Success Then
            id = m0.Groups("id").Value

            Select Case id.Substring(0, 2)
                Case "ch"
                    p.ChannelCommunity = New Channel
                Case Else ' "co" or Unknown
                    ' MEMO: 不正な ID の場合も Community として処理を続行する
                    p.ChannelCommunity = New Community
            End Select

            p.ChannelCommunity.Id = id
            p.ChannelCommunity.IconUri = New Uri(m0.Groups("img").Value)
        Else
            ' MEMO: 不正な ID の場合も Community として処理を続行する
            p.ChannelCommunity = New Community
        End If

        ' title
        Dim m1 = Regex.Match(livePageHtml, "<meta property=""og:title"" content=""(?<title>[^""]+)"">")
        If m1.Success Then
            p.Title = WebUtility.HtmlDecode(m1.Groups("title").Value)
        End If

        ' live id
        Dim m2 = Regex.Match(livePageHtml, "<meta property=""og:url"" content=""https?://live\.nicovideo\.jp/watch/(?<id>lv\d+)")
        If m2.Success Then
            p.Id = m2.Groups("id").Value
        End If

        ' community name
        Dim pattern2 As String
        If p.ChannelCommunity.Type = ChannelCommunityType.Channel Then
            'ch
            pattern2 = "class=""ch_name"" title=""(?<name>[^""]+)"">"
        Else
            pattern2 = "<span itemprop=""name"">(?<name>[^<]+)</span>"
        End If

        Dim m4 = Regex.Match(livePageHtml, pattern2, RegexOptions.Singleline)
        If m4.Success Then
            p.ChannelCommunity.Name = WebUtility.HtmlDecode(m4.Groups("name").Value)
        End If

        ' level
        If p.ChannelCommunity.Type = ChannelCommunityType.Community Then
            Dim m5 = Regex.Match(livePageHtml, "<p class=""community-info-score"">\s*[^：]+：<strong[^>]*>\d+</strong>\s*/\s*[^：]+：<strong[^>]*>(?<lv>\d+)</strong>\s*</p>")
            If m5.Success Then
                DirectCast(p.ChannelCommunity, Community).Level = Convert.ToInt32(m5.Groups("lv").Value)
            End If
        End If

        Return p

    End Function

End Class
