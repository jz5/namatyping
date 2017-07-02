Imports System.Text.RegularExpressions
Imports System.Runtime.Serialization

Namespace LiveStreaming

    Public Enum CommunityChannelRoom
        Arena
        StandingA
        StandingB
        StandingC
        StandingD
        StandingE
        StandingF
        StandingG
        StandingH
        StandingI
    End Enum

    <DataContract()>
    Public Class CommentServer
        ''' <summary>
        ''' <see cref="CommunityChannelRoom"/>に対応する<see cref="RoomLabel"/>。(<see cref="CommunityChannelRoom.Arena"/>を除く)
        ''' </summary>
        ''' <returns></returns>
        Private Shared ReadOnly Property RoomLabels As String()
            Get
                Return {
                    Nothing,
                    "立ち見A列",
                    "立ち見B列",
                    "立ち見C列",
                    "立ち見D列",
                    "立ち見E列",
                    "立ち見F列",
                    "立ち見G列",
                    "立ち見H列",
                    "立ち見I列"
                }
            End Get
        End Property

        Property LiveId As String
        <DataMember(Name:="addr")>
        Property Address As String
        <DataMember(Name:="port")>
        Property Port As Integer
        <DataMember(Name:="thread")>
        Property Thread As Long
        <DataMember(Name:="room_label")>
        Property RoomLabel As String

        Private _Room As CommunityChannelRoom? = Nothing
        ReadOnly Property Room As CommunityChannelRoom?
            Get
                If _Room Is Nothing Then
                    Dim index = Array.IndexOf(RoomLabels, RoomLabel)
                    If index <> -1 Then
                        _Room = DirectCast(index, CommunityChannelRoom)
                    ElseIf RoomLabel IsNot Nothing AndAlso RoomLabel.StartsWith("co") OrElse RoomLabel.StartsWith("ch") Then
                        _Room = CommunityChannelRoom.Arena
                    End If
                End If
                Return _Room
            End Get
        End Property

        Public ReadOnly Property IsValid As Boolean
            Get
                Return LiveId IsNot Nothing AndAlso Regex.IsMatch(LiveId, "^lv\d+$") AndAlso
                    Address IsNot Nothing AndAlso
                    Port > 0 AndAlso
                    Thread > 0 AndAlso
                    RoomLabel IsNot Nothing

            End Get
        End Property

        Public Sub ChangeRoom(dest As CommunityChannelRoom)
            If Me.Room Is Nothing Then
                Throw New InvalidOperationException
            End If

            Dim server = CommentServer.ChangeRoom(Me, dest)
            Me.Address = server.Address
            Me.Port = server.Port
            Me.Thread = server.Thread
        End Sub

        Public Shared Function ChangeRoom(currentServer As CommentServer, dest As CommunityChannelRoom) As CommentServer

            Dim delta = dest - currentServer.Room.Value

            ' Port
            Const minPort = 2805
            Const maxPort = 2854
            Dim destPort = currentServer.Port + (delta * 10)

            If destPort < minPort Then
                destPort = maxPort - (minPort - destPort)
            ElseIf destPort > maxPort Then
                destPort = minPort + (destPort - maxPort)
            End If

            ' Thread
            Dim destThread = currentServer.Thread + delta

            ' Host
            Const minHost = 101
            Const maxHost = 105
            Dim destAddress = currentServer.Address

            Dim m = Regex.Match(currentServer.Address, "^msg(?<host>\d+)\.live\.nicovideo\.jp$")
            If Not m.Success Then
                ' Invalid Address

            End If

            Dim hostNumber = Convert.ToInt32(m.Groups("host").Value) + delta
            If hostNumber < minHost Then
                hostNumber = maxHost
            ElseIf hostNumber > maxHost Then
                hostNumber = minHost
            End If

            destAddress = "msg" & hostNumber.ToString + ".live.nicovideo.jp"

            Dim server = New CommentServer With {
                .LiveId = currentServer.LiveId,
                .Address = destAddress,
                .Port = destPort,
                .Thread = destThread,
                ._Room = dest,
                .RoomLabel = RoomLabels(dest)}

            Return server

        End Function

    End Class

End Namespace
