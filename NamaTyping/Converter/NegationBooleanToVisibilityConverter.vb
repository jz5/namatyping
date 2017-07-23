Imports System.Globalization

Namespace Converter

    Public Class NegationBooleanToVisibilityConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Return If(CBool(value), Visibility.Collapsed, Visibility.Visible)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Dim v = DirectCast(value, Visibility)
            Return v = Visibility.Collapsed
        End Function
    End Class

End Namespace
