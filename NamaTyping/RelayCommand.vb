Imports System
Imports System.Diagnostics
Imports System.Windows.Input

''' <summary>
''' A command whose sole purpose is to 
''' relay its functionality to other
''' objects by invoking delegates. The
''' default return value for the CanExecute
''' method is 'true'.
''' </summary>
Public Class RelayCommand
    Implements ICommand
#Region "Fields"

    Private ReadOnly _execute As Action(Of Object)
    Private ReadOnly _canExecute As Predicate(Of Object)

#End Region ' Fields

#Region "Constructors"

    ''' <summary>
    ''' Creates a new command that can always execute.
    ''' </summary>
    ''' <param name="execute">The execution logic.</param>
    Public Sub New(execute As Action(Of Object))
        Me.New(execute, Nothing)
    End Sub

    ''' <summary>
    ''' Creates a new command.
    ''' </summary>
    ''' <param name="execute">The execution logic.</param>
    ''' <param name="canExecute">The execution status logic.</param>
    Public Sub New(execute As Action(Of Object), canExecute As Predicate(Of Object))
        If execute Is Nothing Then
            Throw New ArgumentNullException("execute")
        End If

        _execute = execute
        _canExecute = canExecute
    End Sub

#End Region ' Constructors

#Region "ICommand Members"

    <DebuggerStepThrough>
    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return (_canExecute Is Nothing) OrElse _canExecute(parameter)
    End Function

    Public Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
        AddHandler(value As EventHandler)
            AddHandler CommandManager.RequerySuggested, value
        End AddHandler
        RemoveHandler(value As EventHandler)
            RemoveHandler CommandManager.RequerySuggested, value
        End RemoveHandler
        RaiseEvent(sender As Object, e As EventArgs)
        End RaiseEvent
    End Event

    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _execute(parameter)
    End Sub

#End Region
End Class
