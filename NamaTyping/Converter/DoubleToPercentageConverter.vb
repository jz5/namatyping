Imports System.Globalization

Namespace Converter

    ''' <summary>
    ''' 数値を100倍します。
    ''' </summary>
    Public Class DoubleToPercentageConverter
        Implements IValueConverter

        Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Return DirectCast(value, Double) * 100
        End Function

        Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return DirectCast(value, Double) / 100
        End Function

    End Class

End Namespace
