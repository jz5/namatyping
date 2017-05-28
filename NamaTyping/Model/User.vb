Public Class User

    Private _Name As String = ""
    Public Property Name() As String
        Get
            If _Name = "" Then
                Return "名無し"
            End If
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Public Property Id As String
    Public Property Highlighted() As Boolean
    Public Property Premium() As Integer
    Public Property RawScore() As Integer

    Private _NormalizedScore As Integer
    Public ReadOnly Property NormalizedScore() As Integer
        Get
            Return _NormalizedScore
        End Get
    End Property

    Public Sub NormalizeScore(ByVal totalScore As Integer)
        _NormalizedScore = Convert.ToInt32(RawScore / totalScore * 1000.0)
    End Sub

    Public Property Rank As Integer? = Nothing
    Public Property PreviousRank As Integer? = Nothing

    Public ReadOnly Property Score As Integer
        Get
            Return _NormalizedScore
        End Get
    End Property

    Private _TotalScore As Integer
    Public ReadOnly Property TotalScore As Integer
        Get
            Return _TotalScore + NormalizedScore
        End Get
    End Property

    'Private _CommentNumber As New List(Of Integer)
    'Public ReadOnly Property CommentNumber() As List(Of Integer)
    '    Get
    '        Return _CommentNumber
    '    End Get
    'End Property

    Public Property LyricsIndex() As Integer
    Public Property LyricsSubIndex() As Integer
    Public Property RecentCommentDateTime() As DateTime

    Public Sub Clear()

    End Sub



    Public Sub Reset()
        If Rank IsNot Nothing Then
            PreviousRank = Rank
        Else
            PreviousRank = Nothing
        End If
        Rank = Nothing

        _TotalScore += NormalizedScore

        RawScore = 0
        LyricsIndex = 0
        LyricsSubIndex = 0
        _ScoringResults.Clear()
    End Sub

    Private _ScoringResults As New List(Of ScoringResult)
    Public ReadOnly Property ScoringResults As IList(Of ScoringResult)
        Get
            Return _ScoringResults
        End Get
    End Property



End Class
