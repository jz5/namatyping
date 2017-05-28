Imports System.Runtime.Serialization
Imports System.Security.Permissions

<Serializable()>
Public Class NicoVideoException
    Inherits Exception
    Implements ISerializable

    Private _Status As String
    Public ReadOnly Property Status As String
        Get
            Return _Status
        End Get
    End Property

    Private _ErrorCode As String
    Public ReadOnly Property ErrorCode As String
        Get
            Return _ErrorCode
        End Get
    End Property

    Private _ErrorDescription As String
    Public ReadOnly Property ErrorDescription As String
        Get
            Return _ErrorDescription
        End Get
    End Property

    ' TODO UnixTime

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception)
        MyBase.New(message, innerException)
    End Sub

    Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        MyBase.New(info, context)
    End Sub

    Public Sub New(ByVal status As String, ByVal errorCode As String, ByVal errorDescription As String,
                   ByVal innerException As Exception)
        Me.New(status, errorCode, errorDescription, Nothing, innerException)
    End Sub

    Public Sub New(ByVal status As String, ByVal errorCode As String, ByVal errorDescription As String,
                   ByVal message As String, ByVal innerException As Exception)
        MyBase.New(message, innerException)
        _Status = status
        _ErrorCode = errorCode
        _ErrorDescription = errorDescription
    End Sub

    <SecurityPermission(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.SerializationFormatter)>
    Public Overrides Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
        MyBase.GetObjectData(info, context)
        info.AddValue("Status", Status)
        info.AddValue("ErrorCode", ErrorCode)
        info.AddValue("ErrorDescription", ErrorDescription)
    End Sub


End Class
