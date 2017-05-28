Public Enum Rating

    None
    Good
    Great
    Skip

End Enum

Public Class ScoringResult

    Property Text As String
    Property LineIndex As Integer
    Property WordIndex As Integer
    Property Rating As Rating
    Property CommentNo As Integer

End Class
