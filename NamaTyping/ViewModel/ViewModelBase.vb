Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel
Imports System.Diagnostics

Namespace ViewModel
    ''' <summary>
    ''' Base class for all ViewModel classes in the application.
    ''' It provides support for property change notifications 
    ''' and has a DisplayName property.  This class is abstract.
    ''' </summary>
    Public MustInherit Class ViewModelBase
        Implements INotifyPropertyChanged, IDisposable

#Region "Constructor"

        Protected Sub New()
        End Sub

#End Region ' Constructor

#Region "DisplayName"

        Public Overridable Property DisplayName As String

#End Region ' DisplayName

#Region "Debugging Aides"

        ''' <summary>
        ''' Warns the developer if this object does not have
        ''' a public property with the specified name. This 
        ''' method does not exist in a Release build.
        ''' </summary>
        <Conditional("DEBUG"), DebuggerStepThrough> _
        Public Sub VerifyPropertyName(propertyName As String)
            ' Verify that the property name matches a real,  
            ' public, instance property on this object.
            If TypeDescriptor.GetProperties(Me)(propertyName) Is Nothing Then
                Dim msg As String = "Invalid property name: " & propertyName

                If Me.ThrowOnInvalidPropertyName Then
                    Throw New Exception(msg)
                Else
                    Debug.Fail(msg)
                End If
            End If
        End Sub

        Protected Overridable Property ThrowOnInvalidPropertyName As Boolean

#End Region ' Debugging Aides

#Region "INotifyPropertyChanged Members"

        ''' <summary>
        ''' Raised when a property on this object has a new value.
        ''' </summary>
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        ''' <summary>
        ''' Raises this object's PropertyChanged event.
        ''' </summary>
        ''' <param name="propertyName">The property that has a new value.</param>
        Protected Overridable Sub OnPropertyChanged(propertyName As String)
            Me.VerifyPropertyName(propertyName)

            Dim handler As PropertyChangedEventHandler = Me.PropertyChangedEvent
            If handler IsNot Nothing Then
                Dim e = New PropertyChangedEventArgs(propertyName)
                handler(Me, e)
            End If
        End Sub

#End Region ' INotifyPropertyChanged Members

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' 重複する呼び出しを検出するには

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me._disposedValue Then
                If disposing Then
                    ' TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                End If

                ' TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下の Finalize() をオーバーライドします。
                ' TODO: 大きなフィールドを null に設定します。
            End If
            Me._disposedValue = True
        End Sub

        ' TODO: 上の Dispose(ByVal disposing As Boolean) にアンマネージ リソースを解放するコードがある場合にのみ、Finalize() をオーバーライドします。
        'Protected Overrides Sub Finalize()
        '    ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace