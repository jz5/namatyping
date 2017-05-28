Imports System.Collections.ObjectModel

Public Enum RankTransition
    None
    Up
    Down
    Same
End Enum

Namespace ViewModel

    Public Class RankDataViewModel
        Inherits ViewModelBase

        Private _Rank As Integer
        Public Property Rank() As Integer
            Get
                Return _Rank
            End Get
            Set(ByVal value As Integer)
                _Rank = value
                OnPropertyChanged("Rank")
            End Set
        End Property

        Private _Score As Integer
        Public Property Score() As Integer
            Get
                Return _Score
            End Get
            Set(ByVal value As Integer)
                _Score = value
                OnPropertyChanged("Score")
            End Set
        End Property

        Private _User As User
        Public Property User As User
            Get
                Return _User
            End Get
            Set(ByVal value As User)
                _User = value
                OnPropertyChanged("User")
            End Set
        End Property


    End Class


End Namespace
