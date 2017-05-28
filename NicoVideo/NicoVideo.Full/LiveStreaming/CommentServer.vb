Imports System.Text.RegularExpressions
Imports System.Runtime.Serialization

Namespace LiveStreaming

    Public Enum CommunityChannelRoom
        Arena
        StandingA
        StandingB
        StandingC
    End Enum

    <DataContract()>
    Public Class CommentServer

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
                    Select Case RoomLabel
                        Case "立ち見A列"
                            _Room = CommunityChannelRoom.StandingA
                        Case "立ち見B列"
                            _Room = CommunityChannelRoom.StandingB
                        Case "立ち見C列"
                            _Room = CommunityChannelRoom.StandingC
                        Case Else
                            If RoomLabel IsNot Nothing AndAlso RoomLabel.StartsWith("co") OrElse RoomLabel.StartsWith("ch") Then
                                _Room = CommunityChannelRoom.Arena
                            End If
                    End Select
                End If
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

            Const minPort = 2805
            Const maxPort = 2814
            Dim destPort = currentServer.Port

            Select Case currentServer.Room
                Case CommunityChannelRoom.Arena
                    Select Case dest
                        Case CommunityChannelRoom.StandingA
                            destPort += 1
                        Case CommunityChannelRoom.StandingB
                            destPort += 2
                        Case CommunityChannelRoom.StandingC
                            destPort += 3
                    End Select

                Case CommunityChannelRoom.StandingA
                    Select Case dest
                        Case CommunityChannelRoom.Arena
                            destPort -= 1
                        Case CommunityChannelRoom.StandingB
                            destPort += 1
                        Case CommunityChannelRoom.StandingC
                            destPort += 2
                    End Select

                Case CommunityChannelRoom.StandingB
                    Select Case dest
                        Case CommunityChannelRoom.Arena
                            destPort -= 2
                        Case CommunityChannelRoom.StandingA
                            destPort -= 1
                        Case CommunityChannelRoom.StandingC
                            destPort += 1
                    End Select

                Case CommunityChannelRoom.StandingC
                    Select Case dest
                        Case CommunityChannelRoom.Arena
                            destPort -= 3
                        Case CommunityChannelRoom.StandingA
                            destPort -= 2
                        Case CommunityChannelRoom.StandingC
                            destPort -= 1
                    End Select

            End Select

            Dim hostNumberDelta = 0

            If destPort < minPort Then
                hostNumberDelta -= 1
                destPort = (maxPort + 1) - (minPort - destPort)
            ElseIf destPort > maxPort Then
                hostNumberDelta += 1
                destPort = (minPort - 1) + (destPort - maxPort)
            End If

            ' Thread
            Dim destThread = currentServer.Thread
            Select Case currentServer.Room
                Case CommunityChannelRoom.Arena
                    Select Case dest
                        Case CommunityChannelRoom.StandingA
                            destThread += 1
                        Case CommunityChannelRoom.StandingB
                            destThread += 2
                        Case CommunityChannelRoom.StandingC
                            destThread += 3
                    End Select

                Case CommunityChannelRoom.StandingA
                    Select Case dest
                        Case CommunityChannelRoom.Arena
                            destThread -= 1
                        Case CommunityChannelRoom.StandingB
                            destThread += 1
                        Case CommunityChannelRoom.StandingC
                            destThread += 2
                    End Select

                Case CommunityChannelRoom.StandingB
                    Select Case dest
                        Case CommunityChannelRoom.Arena
                            destThread -= 2
                        Case CommunityChannelRoom.StandingA
                            destThread -= 1
                        Case CommunityChannelRoom.StandingC
                            destThread += 1
                    End Select

                Case CommunityChannelRoom.StandingC
                    Select Case dest
                        Case CommunityChannelRoom.Arena
                            destThread -= 3
                        Case CommunityChannelRoom.StandingA
                            destThread -= 2
                        Case CommunityChannelRoom.StandingB
                            destThread -= 1
                    End Select

            End Select

            ' Host
            Const minHost = 101
            Const maxHost = 104
            Dim destAddress = currentServer.Address

            Dim m = Regex.Match(currentServer.Address, "^msg(?<host>\d+)\.live\.nicovideo\.jp$")
            If Not m.Success Then
                ' Invalid Address

            End If

            Dim hostNumber = Convert.ToInt32(m.Groups("host").Value) + hostNumberDelta
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
                .Thread = destThread}

            Return server

        End Function

    End Class

End Namespace
