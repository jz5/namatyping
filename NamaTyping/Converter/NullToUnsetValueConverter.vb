Imports System.Globalization

Namespace Converter

    ''' <summary>
    ''' <c>Nothing</c>を<see cref="DependencyProperty.UnsetValue"/>へ変換します。
    ''' </summary>
    ''' <seealso cref="ImageSourceConverter"/>
    Public Class NullToUnsetValueConverter
        Implements IValueConverter

        Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Return If(value, DependencyProperty.UnsetValue)
        End Function

        Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Return Binding.DoNothing
        End Function

    End Class

End Namespace
