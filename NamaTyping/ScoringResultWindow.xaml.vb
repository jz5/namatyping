Imports Pronama.NamaTyping.ViewModel
Imports System.Collections.ObjectModel

Public Class ScoringResultWindow

    Private Sub RankListBox_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles RankListBox.SelectionChanged
        If RankListBox.SelectedItem Is Nothing Then
            ResultListBox.ItemsSource = Nothing
            Exit Sub
        End If

        Dim user = DirectCast(RankListBox.SelectedItem, User)
        ResultListBox.ItemsSource = user.ScoringResults

    End Sub

    Private Sub Button_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)



        Dim sb = New System.Text.StringBuilder
        For Each d In DirectCast(Me.DataContext, ObservableCollection(Of User))
            sb.Append(d.Rank.ToString & vbTab & d.Score.ToString & vbTab & d.Name & vbTab & d.Id)
            sb.Append(vbCrLf)
        Next

        Try
            Clipboard.SetText(sb.ToString)
        Catch ex As Runtime.InteropServices.ExternalException
            MessageBox.Show(
                "クリップボードへのコピーに失敗しました。別のアプリケーションがクリップボードを使用中です。",
                My.Application.Info.Title,
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            )
        End Try

    End Sub
End Class
