Namespace LiveStreaming

    Public Class LiveProgramAlertedEventArgs
        Inherits EventArgs

        Private _LiveProgram As LiveProgram
        Public ReadOnly Property LiveProgram As LiveProgram
            Get
                Return _LiveProgram
            End Get
        End Property

        Private _Error As Exception
        Public ReadOnly Property [Error] As Exception
            Get
                Return _Error
            End Get
        End Property

        Public Sub New(ByVal program As LiveProgram)
            Me.New(program, Nothing)
        End Sub

        Public Sub New(ByVal program As LiveProgram, ByVal [error] As Exception)
            _LiveProgram = program
            _Error = [error]
        End Sub

    End Class

End Namespace
