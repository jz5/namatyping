Imports Pronama.NicoVideo

Partial Public Class MainPage
    Inherits UserControl

    Public Sub New()
        InitializeComponent()

        Dim session = AuthClient.GetUserSession(".jp", "")
        session.ContinueWith(
            Sub(t)
                System.Diagnostics.Debug.WriteLine(t.Result)

            End Sub)
    End Sub

End Class