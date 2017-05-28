Public Enum ChannelCommunityType
    Unknown
    Channel
    Community
End Enum

Public MustInherit Class ChannelCommunity

    Property Id As String
    Property Name As String
    Property IconUri As Uri

    ReadOnly Property Type As ChannelCommunityType
        Get
            If Id Is Nothing Then
                Return ChannelCommunityType.Unknown
            ElseIf Id.StartsWith("ch") Then
                Return ChannelCommunityType.Channel
            ElseIf Id.StartsWith("co") Then
                Return ChannelCommunityType.Community
            Else
                Return ChannelCommunityType.Unknown
            End If
        End Get
    End Property

End Class
