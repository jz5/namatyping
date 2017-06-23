Public Class ScreenControl



    Private Sub ToolBar_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'Dim toolBar = DirectCast(sender, ToolBar)
        'Dim grid = toolBar.Template.FindName("OverflowGrid", toolBar)
        'If grid IsNot Nothing Then
        '    DirectCast(grid, FrameworkElement).Visibility = Windows.Visibility.Collapsed
        'End If
    End Sub

    Private Sub ToolBar_IsVisibleChanged(ByVal sender As System.Object, ByVal e As System.Windows.DependencyPropertyChangedEventArgs)
        Dim toolBar = DirectCast(sender, ToolBar)
        Dim grid = toolBar.Template.FindName("OverflowGrid", toolBar)
        If grid IsNot Nothing Then
            DirectCast(grid, FrameworkElement).Visibility = System.Windows.Visibility.Collapsed
        End If
    End Sub

    Private Sub LyricLineCountComboBox_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles LyricLineCountComboBox.Loaded
        If Me.DataContext Is Nothing Then
            Exit Sub
        End If

        LyricLineCountComboBox.Items.Clear()
        For i = 1 To 10
            LyricLineCountComboBox.Items.Add(i)
        Next

        Dim viewModel = DirectCast(Me.DataContext, ViewModel.MainViewModel)
        LyricLineCountComboBox.SelectedIndex = viewModel.RecentLyricLineCount - 1

    End Sub

    Private Sub LyricLineCountComboBox_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles LyricLineCountComboBox.SelectionChanged
        If Me.DataContext Is Nothing Then
            Exit Sub
        End If

        If LyricLineCountComboBox.SelectedIndex < 0 Then
            Exit Sub
        End If

        Dim viewModel = DirectCast(Me.DataContext, ViewModel.MainViewModel)
        viewModel.RecentLyricLineCount = LyricLineCountComboBox.SelectedIndex + 1

    End Sub

End Class
