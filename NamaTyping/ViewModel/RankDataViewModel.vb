

Public Enum RankTransition
    None
    Up
    Down
    Same
End Enum

Namespace ViewModel

    Public Class RankDataViewModel
        Inherits ViewModelBase

        Private _rank As Integer
        Public Property Rank As Integer
            Get
                Return _Rank
            End Get
            Set
                _Rank = value
                OnPropertyChanged("Rank")
            End Set
        End Property

        Private _score As Integer
        Public Property Score As Integer
            Get
                Return _Score
            End Get
            Set
                _Score = value
                OnPropertyChanged("Score")
            End Set
        End Property

        Private _user As User
        Public Property User As User
            Get
                Return _User
            End Get
            Set
                _User = value
                OnPropertyChanged("User")
            End Set
        End Property


    End Class


End Namespace
