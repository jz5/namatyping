Imports System.Net
Imports System.Threading.Tasks

Public Class AuthClient

    Public Shared Function GetUserSession(ByVal email As String, ByVal password As String) As Task(Of Cookie)
        Const path As String = "https://secure.nicovideo.jp/secure/login?site=niconico"
        Dim req = DirectCast(HttpWebRequest.Create(New Uri(path)), HttpWebRequest)
        Dim param = String.Format("mail={0}&password={1}", Uri.EscapeDataString(email), Uri.EscapeDataString(password))
        Dim buf = System.Text.Encoding.UTF8.GetBytes(param)

        req.Method = "POST"
        req.ContentType = "application/x-www-form-urlencoded"
        req.ContentLength = buf.Length
        req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.30; .NET CLR 3.0.04506.648)"
        'req.Referer = "https://secure.nicovideo.jp/secure/login_form"
        req.CookieContainer = New CookieContainer

        Dim webTask = Task.Factory.FromAsync(Of System.IO.Stream)(AddressOf req.BeginGetRequestStream, AddressOf req.EndGetRequestStream, Nothing) _
                 .ContinueWith(
                     Sub(t As Task(Of System.IO.Stream))
                         Dim stream = t.Result

                         stream.Write(buf, 0, buf.Length)
                         stream.Close()

                     End Sub) _
                 .ContinueWith(Of Cookie)(
                     Function()
                         Dim t1 = Task.Factory.FromAsync(Of WebResponse)(AddressOf req.BeginGetResponse, AddressOf req.EndGetResponse, Nothing) _
                                       .ContinueWith(Of Cookie)(
                                           Function(t2 As Task(Of WebResponse))
                                               Dim response = DirectCast(t2.Result, HttpWebResponse)
                                               Dim body As String
                                               Using stream = response.GetResponseStream
                                                   Using reader = New System.IO.StreamReader(stream, System.Text.Encoding.UTF8)
                                                       body = reader.ReadToEnd
                                                   End Using
                                               End Using

                                               Dim cookies = req.CookieContainer.GetCookies(New Uri(path))
                                               Return cookies("user_session") ' MEMO: 無い場合は Nothing
                                           End Function)
                         t1.Wait()
                         Return t1.Result
                     End Function)
        Return webTask

    End Function

End Class
