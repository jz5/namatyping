﻿'------------------------------------------------------------------------------
' <auto-generated>
'     このコードはツールによって生成されました。
'     ランタイム バージョン:4.0.30319.42000
'
'     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
'     コードが再生成されるときに損失したりします。
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On



<Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
 Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.1.0.0"),  _
 Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
Partial Friend NotInheritable Class MySettings
    Inherits Global.System.Configuration.ApplicationSettingsBase
    
    Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
    
#Region "My.Settings 自動保存機能"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(sender As Global.System.Object, e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
    
    Public Shared ReadOnly Property [Default]() As MySettings
        Get
            
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
            Return defaultInstance
        End Get
    End Property
    
    '''<summary>
    '''設定が保存されたときのアセンブリバージョン。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("設定が保存されたときのアセンブリバージョン。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute(""),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property Version() As String
        Get
            Return CType(Me("Version"),String)
        End Get
        Set
            Me("Version") = value
        End Set
    End Property
    
    '''<summary>
    '''最前面に表示。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("最前面に表示。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("False"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property Topmost() As Boolean
        Get
            Return CType(Me("Topmost"),Boolean)
        End Get
        Set
            Me("Topmost") = value
        End Set
    End Property
    
    '''<summary>
    '''ユーザー名の表示。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("ユーザー名の表示。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("False"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property ShowNameEntryMessages() As Boolean
        Get
            Return CType(Me("ShowNameEntryMessages"),Boolean)
        End Get
        Set
            Me("ShowNameEntryMessages") = value
        End Set
    End Property
    
    '''<summary>
    '''採点メッセージの表示。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("採点メッセージの表示。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("False"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property ShowPointMessages() As Boolean
        Get
            Return CType(Me("ShowPointMessages"),Boolean)
        End Get
        Set
            Me("ShowPointMessages") = value
        End Set
    End Property
    
    '''<summary>
    '''コメントの表示。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("コメントの表示。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property ShowFilteredMessages() As Boolean
        Get
            Return CType(Me("ShowFilteredMessages"),Boolean)
        End Get
        Set
            Me("ShowFilteredMessages") = value
        End Set
    End Property
    
    '''<summary>
    '''表示するコメントの接頭辞。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("表示するコメントの接頭辞。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("■"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property DisplayCommentPattern() As String
        Get
            Return CType(Me("DisplayCommentPattern"),String)
        End Get
        Set
            Me("DisplayCommentPattern") = value
        End Set
    End Property
    
    '''<summary>
    '''色付きユーザーID (コンマ区切り)。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("色付きユーザーID (コンマ区切り)。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute(""),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property HighlightUsers() As String
        Get
            Return CType(Me("HighlightUsers"),String)
        End Get
        Set
            Me("HighlightUsers") = value
        End Set
    End Property
    
    '''<summary>
    '''ログのフォントサイズ (10〜34)。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("ログのフォントサイズ (10〜34)。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("16"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property MessageFontSize() As Double
        Get
            Return CType(Me("MessageFontSize"),Double)
        End Get
        Set
            Me("MessageFontSize") = value
        End Set
    End Property
    
    '''<summary>
    '''歌詞のフォントサイズ (10〜34)。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("歌詞のフォントサイズ (10〜34)。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("20"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property LyricFontSize() As Double
        Get
            Return CType(Me("LyricFontSize"),Double)
        End Get
        Set
            Me("LyricFontSize") = value
        End Set
    End Property
    
    '''<summary>
    '''ランキングのフォントサイズ (10〜34)。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("ランキングのフォントサイズ (10〜34)。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("20"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property RankingFontSize() As Double
        Get
            Return CType(Me("RankingFontSize"),Double)
        End Get
        Set
            Me("RankingFontSize") = value
        End Set
    End Property
    
    '''<summary>
    '''歌詞・ランキング背景の不透明度。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("歌詞・ランキング背景の不透明度。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0.6"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property BottomGridOpacity() As Double
        Get
            Return CType(Me("BottomGridOpacity"),Double)
        End Get
        Set
            Me("BottomGridOpacity") = value
        End Set
    End Property
    
    '''<summary>
    '''表示する歌詞の行数 (1〜10)。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("表示する歌詞の行数 (1〜10)。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("3"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property RecentLyricLineCount() As Integer
        Get
            Return CType(Me("RecentLyricLineCount"),Integer)
        End Get
        Set
            Me("RecentLyricLineCount") = value
        End Set
    End Property
    
    '''<summary>
    '''音量。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("音量。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("0.5"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property Volume() As Double
        Get
            Return CType(Me("Volume"),Double)
        End Get
        Set
            Me("Volume") = value
        End Set
    End Property
    
    '''<summary>
    '''ウィンドウサイズなどのパターン (0〜3)。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("ウィンドウサイズなどのパターン (0〜3)。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("2"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property WindowSizePattern() As Integer
        Get
            Return CType(Me("WindowSizePattern"),Integer)
        End Get
        Set
            Me("WindowSizePattern") = value
        End Set
    End Property
    
    '''<summary>
    '''運営NGワードについて、歌詞の一致部分を強調する。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("運営NGワードについて、歌詞の一致部分を強調する。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property BlacklistCharactersHighlight() As Boolean
        Get
            Return CType(Me("BlacklistCharactersHighlight"),Boolean)
        End Get
        Set
            Me("BlacklistCharactersHighlight") = value
        End Set
    End Property
    
    '''<summary>
    '''運営NGワードについて、歌詞の一致部分に記号を挿入する。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("運営NGワードについて、歌詞の一致部分に記号を挿入する。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("True"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property SplitBlacklistCharacters() As Boolean
        Get
            Return CType(Me("SplitBlacklistCharacters"),Boolean)
        End Get
        Set
            Me("SplitBlacklistCharacters") = value
        End Set
    End Property
    
    '''<summary>
    '''すべての部屋からコメントを取得する。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("すべての部屋からコメントを取得する。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("False"),  _
     Global.System.Configuration.SettingsManageabilityAttribute(Global.System.Configuration.SettingsManageability.Roaming)>  _
    Public Property ConnectAllCommentServers() As Boolean
        Get
            Return CType(Me("ConnectAllCommentServers"),Boolean)
        End Get
        Set
            Me("ConnectAllCommentServers") = value
        End Set
    End Property
    
    '''<summary>
    '''ウィンドウの横位置のキャッシュ。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("ウィンドウの横位置のキャッシュ。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("NaN")>  _
    Public Property WindowLeft() As Double
        Get
            Return CType(Me("WindowLeft"),Double)
        End Get
        Set
            Me("WindowLeft") = value
        End Set
    End Property
    
    '''<summary>
    '''ウィンドウの縦位置のキャッシュ。
    '''</summary>
    <Global.System.Configuration.UserScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("ウィンドウの縦位置のキャッシュ。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("NaN")>  _
    Public Property WindowTop() As Double
        Get
            Return CType(Me("WindowTop"),Double)
        End Get
        Set
            Me("WindowTop") = value
        End Set
    End Property
    
    '''<summary>
    '''運営NGワードを分断する文字。
    '''</summary>
    <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("運営NGワードを分断する文字。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("/")>  _
    Public ReadOnly Property BlacklistCharactersSeparator() As String
        Get
            Return CType(Me("BlacklistCharactersSeparator"),String)
        End Get
    End Property
    
    '''<summary>
    '''NGワード置換ファイルを手動でダウンロードするURL。
    '''</summary>
    <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
     Global.System.Configuration.SettingsDescriptionAttribute("NGワード置換ファイルを手動でダウンロードするURL。"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Configuration.DefaultSettingValueAttribute("https://id.pokemori.jp/niconico-live-ncv")>  _
    Public ReadOnly Property SubstitutionListDownloadURL() As String
        Get
            Return CType(Me("SubstitutionListDownloadURL"),String)
        End Get
    End Property
End Class

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.Pronama.NamaTyping.MySettings
            Get
                Return Global.Pronama.NamaTyping.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
