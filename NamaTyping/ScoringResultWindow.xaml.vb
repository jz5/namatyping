Imports Pronama.NamaTyping.ViewModel
Imports System.Collections.ObjectModel
Imports System.Runtime.InteropServices
Imports System.Text

Public Class ScoringResultWindow

    Private Sub RankListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles RankListBox.SelectionChanged
        If RankListBox.SelectedItem Is Nothing Then
            ResultListBox.ItemsSource = Nothing
            Exit Sub
        End If

        Dim user = DirectCast(RankListBox.SelectedItem, User)
        ResultListBox.ItemsSource = user.ScoringResults

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)



        Dim sb = New StringBuilder
        For Each d In DirectCast(DataContext, ObservableCollection(Of User))
            sb.Append(d.Rank.ToString & vbTab & d.Score.ToString & vbTab & d.Name & vbTab & d.Id)
            sb.Append(vbCrLf)
        Next

        Try
            Clipboard.SetText(sb.ToString)
        Catch ex As ExternalException
            MessageBox.Show(
                "クリップボードへのコピーに失敗しました。別のアプリケーションがクリップボードを使用中です。",
                My.Application.Info.Title,
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            )
        End Try

    End Sub
End Class
