'このクラスでは設定クラスでの特定のイベントを処理することができます:
' SettingChanging イベントは、設定値が変更される前に発生します。
' PropertyChanged イベントは、設定値が変更された後に発生します。
' SettingsLoaded イベントは、設定値が読み込まれた後に発生します。
' SettingsSaving イベントは、設定値が保存される前に発生します。
Imports System.Configuration
Imports System.IO

Partial Friend NotInheritable Class MySettings
    ''' <summary>
    ''' フォントサイズなどの基準となるメディア表示領域の幅。
    ''' </summary>
    Friend Const ReferenceWindowWidth = 640

    ''' <summary>
    ''' フォントサイズの最小値。
    ''' </summary>
    Public Const MinFontSize As Double = 10

    ''' <summary>
    ''' フォントサイズの最大値。
    ''' </summary>
    Public Const MaxFontSize As Double = 34

    ''' <summary>
    ''' 表示する歌詞の最大行数。
    ''' </summary>
    Public Const MaxRecentLyricLineCount As Integer = 10

    ''' <summary>
    ''' メディア表示領域の幅の最小値。
    ''' </summary>
    Friend Const MinWindowWidth As Integer = ReferenceWindowWidth

    ''' <summary>
    ''' メディア表示領域の幅の最大値。
    ''' </summary>
    Friend Const MaxWindowWidth As Integer = 3840

    ''' <summary>
    ''' メディア表示領域の高さの最小値。
    ''' </summary>
    Friend Const MinWindowHeight As Integer = 360

    ''' <summary>
    ''' メディア表示領域の高さの最大値。
    ''' </summary>
    Friend Const MaxWindowHeight As Integer = 2160

    ''' <summary>
    ''' メディアの拡大方法のパターン。
    ''' </summary>
    Private ReadOnly MediaStretches As Stretch() = {Stretch.Uniform, Stretch.UniformToFill}

    ''' <summary>
    ''' ユーザー設定ファイル (user.config) のパス。
    ''' </summary>
    ''' <returns></returns>
    Friend ReadOnly Property FilePath As String = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming).FilePath

    ''' <summary>
    ''' ユーザー設定ファイル (user.config) が保存されているフォルダのパス。
    ''' </summary>
    ''' <returns></returns>
    Friend ReadOnly Property ParentPath As String = Path.GetDirectoryName(FilePath)

    ''' <summary>
    ''' <see cref="Upgrade"/>を呼び出し済みなら真。
    ''' </summary>
    Private _alreadyUpgraded As Boolean = False

    Protected Overrides Sub OnSettingsLoaded(sender As Object, e As SettingsLoadedEventArgs)
        MyBase.OnSettingsLoaded(sender, e)

        Upgrade()
        CorrectRange()
    End Sub

    ''' <summary>
    ''' 旧バージョンのユーザー設定を引き継ぎます。
    ''' </summary>
    Public Overrides Sub Upgrade()
        If Not _alreadyUpgraded Then
            _alreadyUpgraded = True

            If Version = "" Then
                MyBase.Upgrade()

                Dim oldVersion As Version = Nothing
                If (System.Version.TryParse(Version, oldVersion)) Then
                    CopyOldVersionFiles(oldVersion.ToString())

                    If oldVersion.CompareTo(New Version(2, 5, 1, 0)) < 0 Then
                        Select Case WindowSizePattern
                            Case 0
                                WindowWidth = 640
                                WindowHeight = 360
                                MediaStretch = Stretch.Uniform
                            Case 1
                                WindowWidth = 640
                                WindowHeight = 360
                                MediaStretch = Stretch.UniformToFill
                            Case 2
                                WindowWidth = 640
                                WindowHeight = 480
                                MediaStretch = Stretch.Uniform
                            Case 3
                                WindowWidth = 640
                                WindowHeight = 480
                                MediaStretch = Stretch.UniformToFill
                        End Select
                    End If
                End If

                Version = My.Application.Info.Version.ToString()
                Save()
            End If
        End If
    End Sub

    ''' <summary>
    ''' 旧バージョンの設定ファイル保存フォルダに存在する user.config 以外のファイルを、現バージョンのフォルダにコピーします。
    ''' </summary>
    ''' <param name="oldVersion">旧バージョン番号。</param>
    Private Sub CopyOldVersionFiles(oldVersion As String)
        Dim oldParentPath = Path.Combine(Path.GetDirectoryName(ParentPath), oldVersion)
        If oldParentPath <> ParentPath Then
            Dim exclusionFileName = Path.GetFileName(FilePath)
            Dim oldDirectory = New DirectoryInfo(oldParentPath)
            For Each entry As FileSystemInfo In oldDirectory.EnumerateFileSystemInfos()
                If entry.Name <> exclusionFileName Then
                    Dim newEntryPath = Path.Combine(ParentPath, Path.GetFileName(entry.Name))
                    If (entry.Attributes And FileAttributes.Directory) = FileAttributes.Directory Then
                        My.Computer.FileSystem.CopyDirectory(entry.FullName, newEntryPath)
                    Else
                        File.Copy(entry.FullName, newEntryPath)
                    End If
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' 値の範囲を矯正します。
    ''' </summary>
    Private Sub CorrectRange()
        If MessageFontSize < MinFontSize Then
            MessageFontSize = MinFontSize
        End If
        If MessageFontSize > MaxFontSize Then
            MessageFontSize = MaxFontSize
        End If
        If LyricFontSize < MinFontSize Then
            LyricFontSize = MinFontSize
        End If
        If LyricFontSize > MaxFontSize Then
            LyricFontSize = MaxFontSize
        End If
        If RankingFontSize < MinFontSize Then
            RankingFontSize = MinFontSize
        End If
        If RankingFontSize > MaxFontSize Then
            RankingFontSize = MaxFontSize
        End If
        If BottomGridOpacity < 0 Then
            BottomGridOpacity = 0
        End If
        If BottomGridOpacity > 1 Then
            BottomGridOpacity = 1
        End If
        If RecentLyricLineCount < 1 Then
            RecentLyricLineCount = 1
        End If
        If RecentLyricLineCount > MaxRecentLyricLineCount Then
            RecentLyricLineCount = MaxRecentLyricLineCount
        End If
        If WindowWidth < MinWindowWidth Then
            WindowWidth = MinWindowWidth
        End If
        If WindowWidth > MaxWindowWidth Then
            WindowWidth = MaxWindowWidth
        End If
        If WindowHeight < MinWindowHeight Then
                WindowHeight = MinWindowHeight
            End If
        If WindowHeight > MaxWindowHeight Then
            WindowHeight = MaxWindowHeight
        End If
        If Not MediaStretches.Contains(DirectCast(MediaStretch, Stretch)) Then
            MediaStretch = DirectCast(Properties.Item("MediaStretch").DefaultValue, Integer)
        End If
    End Sub
End Class
