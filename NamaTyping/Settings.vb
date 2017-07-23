'このクラスでは設定クラスでの特定のイベントを処理することができます:
' SettingChanging イベントは、設定値が変更される前に発生します。
' PropertyChanged イベントは、設定値が変更された後に発生します。
' SettingsLoaded イベントは、設定値が読み込まれた後に発生します。
' SettingsSaving イベントは、設定値が保存される前に発生します。
Imports System.Configuration
Imports System.IO

Partial Friend NotInheritable Class MySettings
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
    ''' ウィンドウサイズなどのパターン数。
    ''' </summary>
    Protected Const WindowSizePatternCount As Integer = 4

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
    Private AlreadyUpgraded As Boolean = False

    Protected Overrides Sub OnSettingsLoaded(sender As Object, e As SettingsLoadedEventArgs)
        MyBase.OnSettingsLoaded(sender, e)

        Upgrade()
        CorrectRange()
    End Sub

    ''' <summary>
    ''' 旧バージョンのユーザー設定を引き継ぎます。
    ''' </summary>
    Public Overrides Sub Upgrade()
        If Not AlreadyUpgraded Then
            AlreadyUpgraded = True

            If Version = "" Then
                MyBase.Upgrade()

                Dim OldVersion As Version = Nothing
                If (System.Version.TryParse(Version, OldVersion)) Then
                    CopyOldVersionFiles(OldVersion.ToString())

                    'If OldVersion.Major < 3 Then
                    '    マイグレーション処理
                    'End If
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
    Private Sub CopyOldVersionFiles(ByVal oldVersion As String)
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
    Protected Sub CorrectRange()
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
        If WindowSizePattern < 0 OrElse WindowSizePatternCount <= WindowSizePattern Then
            WindowSizePattern = CType(Properties.Item("WindowSizePattern").DefaultValue, Integer)
        End If
    End Sub
End Class
