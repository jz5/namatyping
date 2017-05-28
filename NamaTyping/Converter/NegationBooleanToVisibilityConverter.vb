Namespace Converter

    Public Class NegationBooleanToVisibilityConverter
        Implements IValueConverter

        Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
            Return If(CBool(value), Visibility.Collapsed, Visibility.Visible)
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
            Dim v = DirectCast(value, Visibility)
            Return If(v = Visibility.Collapsed, True, False)
        End Function
    End Class

End Namespace
