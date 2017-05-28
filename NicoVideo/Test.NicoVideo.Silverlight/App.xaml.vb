 Partial Public Class App
    Inherits Application

    public Sub New()
        InitializeComponent()
    End Sub
    
    Private Sub Application_Startup(ByVal o As Object, ByVal e As StartupEventArgs) Handles Me.Startup
        Me.RootVisual = New MainPage()
    End Sub
    
    Private Sub Application_Exit(ByVal o As Object, ByVal e As EventArgs) Handles Me.Exit

    End Sub
    
    Private Sub Application_UnhandledException(ByVal sender As object, ByVal e As ApplicationUnhandledExceptionEventArgs) Handles Me.UnhandledException

        ' アプリケーションがデバッガーの外側で実行されている場合、ブラウザーの
        ' 例外メカニズムによって例外が報告されます。これにより、IE ではステータス バーに
        ' 黄色の通知アイコンが表示され、Firefox にはスクリプト エラーが表示されます。
        If Not System.Diagnostics.Debugger.IsAttached Then

            ' メモ : これにより、アプリケーションは例外がスローされた後も実行され続け、例外は
            ' ハンドルされません。 
            ' 実稼動アプリケーションでは、このエラー処理は、Web サイトにエラーを報告し、
            ' アプリケーションを停止させるものに置換される必要があります。
            e.Handled = True
            Deployment.Current.Dispatcher.BeginInvoke(New Action(Of ApplicationUnhandledExceptionEventArgs)(AddressOf ReportErrorToDOM), e)
        End If
    End Sub

   Private Sub ReportErrorToDOM(ByVal e As ApplicationUnhandledExceptionEventArgs)

        Try
            Dim errorMsg As String = e.ExceptionObject.Message + e.ExceptionObject.StackTrace
            errorMsg = errorMsg.Replace(""""c, "'"c).Replace(ChrW(13) & ChrW(10), "\n")

            System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(""Unhandled Error in Silverlight Application " + errorMsg + """);")
        Catch
        
        End Try
    End Sub
    
End Class
