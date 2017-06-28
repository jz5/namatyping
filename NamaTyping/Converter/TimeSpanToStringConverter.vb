Imports System.Windows.Data
Imports System.Windows

Namespace Converter

    Public Class TimeSpanToStringConverter
        Implements IValueConverter

        Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
            Dim ts = DirectCast(value, TimeSpan)
            'Return String.Format("{0:D2}:{1:D2}.{2:D1}", ts.Minutes, ts.Seconds, ts.Milliseconds \ 100)
            Return $"{ts.Minutes:D2}:{ts.Seconds:D2}"
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

End Namespace
