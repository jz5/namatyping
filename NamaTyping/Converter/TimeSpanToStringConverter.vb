Imports System.Globalization

Namespace Converter

    Public Class TimeSpanToStringConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Dim ts = DirectCast(value, TimeSpan)
            'Return String.Format("{0:D2}:{1:D2}.{2:D1}", ts.Minutes, ts.Seconds, ts.Milliseconds \ 100)
            Return $"{ts.Minutes:D2}:{ts.Seconds:D2}"
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

End Namespace
