Imports Pronama.NamaTyping.ViewModel

Public Class ScreenControl



    Private Sub ToolBar_Loaded(sender As Object, e As RoutedEventArgs)
        'Dim toolBar = DirectCast(sender, ToolBar)
        'Dim grid = toolBar.Template.FindName("OverflowGrid", toolBar)
        'If grid IsNot Nothing Then
        '    DirectCast(grid, FrameworkElement).Visibility = Windows.Visibility.Collapsed
        'End If
    End Sub

    Private Sub ToolBar_IsVisibleChanged(sender As Object, e As DependencyPropertyChangedEventArgs)
        Dim toolBar = DirectCast(sender, ToolBar)
        Dim grid = toolBar.Template.FindName("OverflowGrid", toolBar)
        If grid IsNot Nothing Then
            DirectCast(grid, FrameworkElement).Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub LyricLineCountComboBox_Loaded(sender As Object, e As RoutedEventArgs) Handles LyricLineCountComboBox.Loaded
        If DataContext Is Nothing Then
            Exit Sub
        End If

        LyricLineCountComboBox.Items.Clear()
        For i = 1 To 10
            LyricLineCountComboBox.Items.Add(i)
        Next

        Dim viewModel = DirectCast(DataContext, MainViewModel)
        LyricLineCountComboBox.SelectedIndex = viewModel.RecentLyricLineCount - 1

    End Sub

    Private Sub LyricLineCountComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles LyricLineCountComboBox.SelectionChanged
        If DataContext Is Nothing Then
            Exit Sub
        End If

        If LyricLineCountComboBox.SelectedIndex < 0 Then
            Exit Sub
        End If

        Dim viewModel = DirectCast(DataContext, MainViewModel)
        viewModel.RecentLyricLineCount = LyricLineCountComboBox.SelectedIndex + 1

    End Sub

End Class
