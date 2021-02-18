Imports NamaTyping.Auth


Public Class AuthWindow

    Private State As String

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)

        If String.IsNullOrWhiteSpace(AuthTextBox.Text) Then
            Return
        End If


        Dim result As Result
        Try
            result = Client.DecodeResult(AuthTextBox.Text.Trim(), State)

        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Exit Sub
        End Try

        StoreToken(result)
        Close()

    End Sub

    Private Sub OpenAuthPage(sender As Object, e As RoutedEventArgs)
        State = Guid.NewGuid().ToString()
        Client.OpenAuthPage(State)
        AuthTextBox.IsEnabled = True
    End Sub

    ''' <summary>
    ''' トークンを保存
    ''' </summary>
    Public Sub StoreToken(result As Result)

        Try
            
            My.Settings.AccessToken = result.AccessToken
            My.Settings.RefreshToken = result.RefreshToken
            My.Settings.UserId = result.UserId

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Public Shared Async Function TryRefreshTokenAsync(refreshToken As String) As Threading.Tasks.Task(Of Boolean)

        Try
            Dim result = Await Client.RefreshTokenAsync(refreshToken)

            My.Settings.AccessToken = result.AccessToken
            My.Settings.RefreshToken = result.RefreshToken
            Return True

        Catch ex As Exception

            Return False
        End Try

    End Function
    

    Private Async Sub ButtonBase_OnClick(sender As Object, e As RoutedEventArgs)
        Dim ok = Await TryRefreshTokenAsync(My.Settings.RefreshToken)

    End Sub
End Class
