Public Class User

    Private _name As String = ""
    Public Property Name As String
        Get
            If _name = "" Then
                Return "名無し"
            End If
            Return _name
        End Get
        Set
            _name = Value
        End Set
    End Property

    ''' <summary>
    ''' ユーザー毎に固有の設定。
    ''' </summary>
    Public Property SettingFlags As String

    ''' <summary>
    ''' 設定名と<see cref="SettingFlags">に含まれるフラグの組。
    ''' </summary>
    Friend Shared ReadOnly Property SettingFlagList As New Dictionary(Of String, Char) From {
    }

    Public Property Id As String
    Public Property Highlighted As Boolean
    Public Property Premium As Integer
    Public Property RawScore As Integer

    Public Property NormalizedScore As Integer

    Public Sub NormalizeScore(totalScore As Integer)
        NormalizedScore = Convert.ToInt32(RawScore / totalScore * 1000.0)
    End Sub

    Public Property Rank As Integer? = Nothing
    Public Property PreviousRank As Integer? = Nothing

    Public ReadOnly Property Score As Integer
        Get
            Return NormalizedScore
        End Get
    End Property

    Private _totalScore As Integer
    Public ReadOnly Property TotalScore As Integer
        Get
            Return _totalScore + NormalizedScore
        End Get
    End Property

    'Private _CommentNumber As New List(Of Integer)
    'Public ReadOnly Property CommentNumber() As List(Of Integer)
    '    Get
    '        Return _CommentNumber
    '    End Get
    'End Property

    Public Property LyricsIndex As Integer
    Public Property LyricsSubIndex As Integer
    Public Property RecentCommentDateTime As DateTime

    Public Sub Clear()

    End Sub



    Public Sub Reset()
        If Rank IsNot Nothing Then
            PreviousRank = Rank
        Else
            PreviousRank = Nothing
        End If
        Rank = Nothing

        _totalScore += NormalizedScore

        RawScore = 0
        LyricsIndex = 0
        LyricsSubIndex = 0
        _ScoringResults.Clear()
    End Sub

    Private ReadOnly _scoringResults As New List(Of ScoringResult)
    Public ReadOnly Property ScoringResults As IList(Of ScoringResult)
        Get
            Return _ScoringResults
        End Get
    End Property



End Class
