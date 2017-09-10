VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CommonClass"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False

Option Explicit
'
'   Dodad - the entire contructed object that is moved around on the dashboard
'
'       From inside out:
'           DodadContent - The inner-most part of the Dodad, the actual content that is on the desktop
'           DesignWrapper - the visible border for all Dodads
'           ControlWrapper = the absolute positioned wrapper that contains all drag/drop/resize/toolbar features
'
'
''
''
''
'Public Function GetDodadContent(Main As MainClass, AddonID As Long, AddonNameOrGuid As String, AddonOptions As String, State As String, IconSprites As Long, Title As String, IconFileName As String, IconWidth As Long, IconHeight As Long, ServerFilePath As String, WrapperID As Long, SizeX As Long, SizeY As Long, ShortcutHref As String) As String
'    On Error GoTo ErrorTrap
'    '
'    Dim Content As String
'    Dim IconImg As String
'    Dim InnerStream As String
'    Dim ToolBar As String
'    '
'    If LCase(State) = "closed" Then
'        '
'        ' Icon Shortcut (Content or Addon)
'        '
'        If IconFileName = "" Then
'            '
'            ' Default Icon
'            '
'            If (AddonID <> 0) Or (AddonNameOrGuid <> "") Then
'                IconFileName = "/cclib/images/icons/addon.png"
'            Else
'                IconFileName = "/cclib/images/icons/content.png"
'            End If
'            IconWidth = 57
'            IconHeight = 59
'            IconSprites = 4
'        End If
'        If IconSprites < 2 Then
'            '
'            ' Flat image
'            '
'            IconImg = "<img title=""" & Title & """ border=""0"" src=""" & IconFileName & """>"
'        Else
'            '
'            ' Sprites
'            '
'            IconImg = GetAddonIconImg("", IconWidth, IconHeight, IconSprites, True, "", IconFileName, Main.ServerFilePath, Title, Title, "", 0)
'        End If
'        Content = "" _
'            & vbCrLf & vbTab & vbTab & "<a class=""shortcut"" href=""" & ShortcutHref & """>" & IconImg & "<br>" & Title & "</a>" _
'            & ""
'    Else
'        '
'        ' Addon in a wrapper
'        '
'        Content = Main.ExecuteAddon(AddonID, AddonNameOrGuid, AddonOptions, ContextAdmin, "", 0, "", "", 0)
'    End If
'    '
'    GetDodadContent = Content
'    '
'    Exit Function
'ErrorTrap:
'    Call HandleError("DashboardClass", "GetDodadContent", Err.Number, Err.Source, Err.Description, True, False)
'End Function
'
'
'
Public Function GetDodad(Main As Object, AddonIdOrGuid As String, ContentGuid As String, ContentName As String, Title As String, PosX As Long, PosY As Long, State As String, SizeX As Long, SizeY As Long, AddonOptions As String, WrapperID As Long, NodePtr As Long, Return_RequiredJS As String, IconZIndex As Long) As String
    On Error GoTo ErrorTrap
    '
    Dim DodadWrappedContent As String
    Dim HandleJS As String
    Dim ToolBar As String
    Dim ResizeableJS As String
    Dim WrapperWidth As Long
    Dim WrapperHeight As Long
    Dim DoDadContent As String
    Dim ShortcutHref As String
    Dim IconWidth As Long
    Dim IconHeight As Long
    Dim IconSprites As Long
    Dim IsIcon As Boolean
    
    Dim IconFileName As String
    Dim Dodad As String
    Dim SpaceArray() As String
    Dim NavNodeID As Long
    Dim CS As Long
    Dim Criteria As String
    Dim Pointer As Long
    Dim ContentID As Long
    Dim SettingID As Long
    Dim js As String
    Dim IconImg As String
    Dim AddonID As Long
    Dim AddonGuid As String
    Dim GuidGenerator As New GuidGenerator
    Dim DroppableHoverClass As String
    '
    Dim ItemHTMLClass As String
    '
    Dim ItemHtmlID As String
    Dim DesignResizerHTMLId As String
    Dim ToolBarHTMLId As String
    '
    ItemHtmlID = "dashnode" & NodePtr
    ToolBarHTMLId = "toolBar" & ItemHtmlID
    DesignResizerHTMLId = "designResizer" & ItemHtmlID
    WrapperWidth = 77
    WrapperHeight = 77
    If (AddonIdOrGuid <> "") And (AddonIdOrGuid <> "0") Then
        If IsNumeric(AddonIdOrGuid) And Len(AddonIdOrGuid) < 10 Then
            AddonID = Int(AddonIdOrGuid)
            AddonGuid = ""
        Else
            AddonID = 0
            AddonGuid = AddonIdOrGuid
        End If
        '
        ' Node is an Add-on
        '
        If AddonID <> 0 Then
            CS = Main.OpenCSContentRecord("Add-Ons", AddonID)
        ElseIf Main.SiteProperty_BuildVersion >= "3.4.060" Then
            CS = Main.OpenCSContent("Add-Ons", "ccGUID=" & Main.EncodeSQLText(AddonIdOrGuid))
        Else
            CS = Main.OpenCSContent("Add-Ons", "aoGUID=" & Main.EncodeSQLText(AddonIdOrGuid))
        End If
        If Main.CSOK(CS) Then
            AddonID = Main.GetCSText(CS, "id")
            AddonGuid = Main.GetCSText(CS, "ccguid")
            If AddonGuid = "" Then
                AddonGuid = GuidGenerator.CreateGuid("")
                Call Main.SetCS(CS, "ccguid", AddonGuid)
            End If
            If LCase(State) = "closed" Then
                '
                ' Addon is just an icon
                '
                ItemHTMLClass = "iconNode"
                IsIcon = True
                IconFileName = Main.GetCSText(CS, "IconFilename")
                IconWidth = Main.GetCSInteger(CS, "IconWidth")
                IconHeight = Main.GetCSInteger(CS, "IconHeight")
                IconSprites = Main.GetCSInteger(CS, "IconSprites")
                ShortcutHref = Main.ServerPage & "?addonid=" & AddonID
                'DoDadContent = GetDodadContent(Main, AddonID, AddonGuid, AddonOptions, State, IconSprites, Title, IconFileName, IconWidth, IconHeight, Main.ServerFilePath, WrapperID, 0, 0, ShortcutHref)
                ToolBar = "" _
                    & "<a alt=""Minimize"" title=""Minimize"" href=""#"" onClick=""return false;"" class=""opacity50""><img border=0 src=""/cclib/images/opendown1313.gif"" width=""13"" height=""13""></a>" _
                    & "<a alt=""Run in window"" title=""Run in Window"" href=""#"" onClick=""dashOpenNode('" & NodePtr & "','" & ItemHtmlID & "');return false;""><img border=0 src=""/cclib/images/box1313.gif"" width=""13"" height=""13""></a>" _
                    & "<a alt=""Run full size"" title=""Run full size""  href=""?addonguid=" & AddonGuid & """><img border=0 src=""/cclib/images/openup1313.gif"" width=""13"" height=""13""></a>" _
                    & "<a alt=""Remove from dashboard"" title=""Remove from dashboard"" href=""#"" onClick=""dashDeleteNode('" & NodePtr & "','" & ItemHtmlID & "');return false;""><img border=0 src=""/cclib/images/closex1313.gif"" width=""13"" height=""13""></a>" _
                    & ""
            Else
                '
                ' open Addon running in a wrapper
                '
                ItemHTMLClass = "windowNode"
                IsIcon = False
                WrapperWidth = SizeX
                WrapperHeight = SizeY
                ShortcutHref = ""
                'DoDadContent = GetDodadContent(Main, AddonID, AddonGuid, AddonOptions, State, IconSprites, Title, IconFileName, IconWidth, IconHeight, Main.ServerFilePath, WrapperID, 0, 0, "")
                ToolBar = "" _
                    & "<div style=""float:left"">" & Title & "</div>" _
                    & "<a alt=""Minimize"" title=""Minimize"" href=""#"" onClick=""closeNode('" & NodePtr & "','" & ItemHtmlID & "');return false;""><img border=0 src=""/cclib/images/opendown1313.gif"" width=""13"" height=""13""></a>" _
                    & "<a alt=""Run in window"" title=""Run in Window"" href=""#"" class=""opacity50"" onClick=""return false;""><img border=0 src=""/cclib/images/box1313.gif"" width=""13"" height=""13""></a>" _
                    & "<a alt=""Run full size"" title=""Run full size""  href=""?addonguid=" & AddonGuid & """><img border=0 src=""/cclib/images/openup1313.gif"" width=""13"" height=""13""></a>" _
                    & "<a alt=""Remove from dashboard"" title=""Remove from dashboard"" href=""#"" onClick=""dashDeleteNode('" & NodePtr & "','" & ItemHtmlID & "');return false;""><img border=0 src=""/cclib/images/closex1313.gif"" width=""13"" height=""13""></a>" _
                    & ""
            End If
        End If
        Call Main.CloseCS(CS)
        'DoDadContent = GetDodadContent(Main, AddonID, AddonGuid, AddonOptions, State, IconSprites, Title, IconFileName, IconWidth, IconHeight, Main.ServerFilePath, WrapperID, 0, 0, ShortcutHref)
        DroppableHoverClass = "droppableHover"
    ElseIf (ContentGuid <> "") Or (ContentName <> "") Then
        '
        ' Node is content
        '
        IsIcon = True
        ItemHTMLClass = "iconNode"
        If (Main.SiteProperty_BuildVersion >= "3.4.060") And (ContentGuid <> "") Then
            CS = Main.OpenCSContent("Content", "ccGUID=" & Main.EncodeSQLText(ContentGuid))
        Else
            CS = Main.OpenCSContent("Content", "Name=" & Main.EncodeSQLText(ContentName))
        End If
        If Main.CSOK(CS) Then
            ContentID = Main.GetCSInteger(CS, "id")
            ContentName = Main.GetCSText(CS, "name")
            IconFileName = Main.GetCSText(CS, "IconLink")
            IconWidth = Main.GetCSInteger(CS, "IconWidth")
            IconHeight = Main.GetCSInteger(CS, "IconHeight")
            IconSprites = Main.GetCSInteger(CS, "IconSprites")
            ShortcutHref = Main.ServerPage & "?cid=" & ContentID
            DroppableHoverClass = ""
            AddonID = 0
            AddonGuid = ""
            AddonOptions = ""
            State = "closed"
            Title = ContentName
        End If
        Call Main.CloseCS(CS)
        'DoDadContent = GetDodadContent(Main, AddonID, AddonGuid, AddonOptions, State, IconSprites, Title, IconFileName, IconWidth, IconHeight, Main.ServerFilePath, WrapperID, 0, 0, ShortcutHref)
        ToolBar = "" _
            & "<a href=""#"" onClick=""dashDeleteNode('" & NodePtr & "','" & ItemHtmlID & "');return false;""><img border=0 src=""/cclib/images/closex1313.gif"" width=""13"" height=""13""></a>" _
            & ""
'        ToolBar = "" _
'            & "<a href=""#"" onClick=""dashOpenNode('" & NodePtr & "','" & ItemHtmlID & "');return false;""><img border=0 src=""/cclib/images/openup1313.gif"" width=""13"" height=""13""></a>" _
'            & "<a href=""#"" onClick=""dashDeleteNode('" & NodePtr & "','" & ItemHtmlID & "');return false;""><img border=0 src=""/cclib/images/closex1313.gif"" width=""13"" height=""13""></a>" _
'            & ""
    End If
    '
    ' Build inner content of icon or addon
    '
    If LCase(State) = "closed" Then
        '
        ' Icon Shortcut (Content or Addon)
        '
        If IconFileName = "" Then
            '
            ' Default Icon
            '
            If (AddonID <> 0) Or (AddonGuid <> "") Then
                IconFileName = "/cclib/images/icons/addon.png"
            Else
                IconFileName = "/cclib/images/icons/content.png"
            End If
            IconWidth = 57
            IconHeight = 59
            IconSprites = 4
        End If
        If IconSprites < 2 Then
            '
            ' Flat image
            '
            IconImg = "<img title=""" & Title & """ border=""0"" src=""" & IconFileName & """>"
        Else
            '
            ' Sprites
            '
            IconImg = GetAddonIconImg("", IconWidth, IconHeight, IconSprites, True, "", IconFileName, Main.ServerFilePath, Title, Title, "", 0)
        End If
        DoDadContent = "" _
            & vbCrLf & vbTab & vbTab & "<a class=""shortcut"" href=""" & ShortcutHref & """>" & IconImg & "<br>" & Title & "</a>" _
            & ""
    Else
        '
        ' Addon in a wrapper
        '
'
' from Csvr
'
Const ContextAdmin = 2
        DoDadContent = Main.ExecuteAddon(AddonID, AddonGuid, AddonOptions, ContextAdmin, "", 0, "", "", 0)
    End If
    '
    'DoDadContent = GetDodadContent(Main, AddonID, AddonGuid, AddonOptions, State, IconSprites, Title, IconFileName, IconWidth, IconHeight, Main.ServerFilePath, WrapperID, 0, 0, ShortcutHref)
    DodadWrappedContent = DoDadContent
    '
    ' Add Design-Resizer (an inner wrapper to use for resizing the design wrapper with the control wrapper)
    '
    If Not IsIcon Then
        '
        ' non-icon, add resize
        '
        DodadWrappedContent = "" _
            & CR & "<div" _
                & " id=""" & DesignResizerHTMLId & """" _
                & " class=""designResizer""" _
                & " style=""height: " & WrapperHeight & ";""" _
                & " >" _
            & KmaIndent(DodadWrappedContent) _
            & CR & "</div>"
        If WrapperID <> 0 Then
            '
            ' Add Design-Wrapper (first an inner wrapper to use for resizing the design wrapper with the control wrapper)
            '
            DodadWrappedContent = Main.WrapContent(DodadWrappedContent, WrapperID)
        End If
    End If
'    '
'    ' Add Design-Wrapper (first an inner wrapper to use for resizing the design wrapper with the control wrapper)
'    '
'    If WrapperID <> 0 And Not IsIcon Then
'        DodadWrappedContent = Main.WrapContent(DodadWrappedContent, WrapperID)
'    Else
'        DodadWrappedContent = DoDadContent
'    End If
    '
    ' Build Toolbar wrapper
    '
    ToolBar = CR & ToolBar
    ToolBar = "" _
        & CR & "<div" _
            & " class=""toolBarInner""" _
            & " >" _
        & KmaIndent(ToolBar) _
        & CR & "</div>" _
        & ""
    ToolBar = "" _
        & CR & "<div" _
            & " class=""toolBar""" _
            & " onmouseout=""outTools(this)""" _
            & " onmouseover=""overTools(this)""" _
            & " id=""" & ToolBarHTMLId & """" _
            & " >" _
        & KmaIndent(ToolBar) _
        & CR & "</div>" _
        & ""
    '
    ' Assemble common elements from all item types
    '
    Dodad = "" _
        & CR & "<div id=""" & ItemHtmlID & """" _
            & " class=""" & ItemHTMLClass & """" _
            & " onmouseout=""outBody('" & ToolBarHTMLId & "')""" _
            & " onmouseover=""overBody('" & ToolBarHTMLId & "')""" _
            & " style=""top:" & PosY & "px; left:" & PosX & "px; width: " & WrapperWidth & "px;""" _
            & " >" _
        & KmaIndent(ToolBar) _
        & KmaIndent(DodadWrappedContent) _
        & CR & "</div>" _
        & ""
    '
    ' ----- Control Wrapper - Make this new item resizeable and draggable
    '
    ResizeableJS = ""
    HandleJS = ""
    If IsIcon Then
        '
        ' icon
        '
    Else
        '
        ' non-icon
        '
        ResizeableJS = ".resizable({" _
                & "alsoResize: '#" & DesignResizerHTMLId & "'" _
                & ",stop: function(event, ui) {" _
                        & "/* alert('resize stop') */" _
                        & "var e=document.getElementById('" & ItemHtmlID & "');" _
                        & "var r=document.getElementById('" & DesignResizerHTMLId & "');" _
                        & "cj.ajax.addon('dashboardresize','ptr=" & NodePtr & "&x='+e.style.width+'&y='+r.style.height);" _
                        & "dashResize();" _
                    & "}" _
                & "})"
        HandleJS = ",handle: '#" & ToolBarHTMLId & "'"
    End If
    Return_RequiredJS = Return_RequiredJS _
        & CR _
        & "/* alert('draggable and resizable being added');*/" _
        & "$(function(){" _
            & "$('#" & ItemHtmlID & "').draggable({" _
                & "stop: function(event, ui){" _
                    & "var e=document.getElementById('" & ItemHtmlID & "');" _
                    & "cj.ajax.addonCallback('dashboarddragstop','ptr=" & NodePtr & "&x='+e.style.left+'&y='+e.style.top,dashResize);" _
                & "}" _
                & ",start: function(event, ui){" _
                    & "var e=document.getElementById('" & ItemHtmlID & "');" _
                    & "e.style.zIndex=iconZIndexTop++;" _
                    & "$('#" & ItemHtmlID & "').draggable('option', 'zIndex', iconZIndexTop );" _
                & "}" _
                & ",revert: 'invalid'" _
                & ",zIndex: " & IconZIndex & "" _
                & ",hoverClass: '" & DroppableHoverClass & "'" _
                & ",opacity: 0.50" _
                & HandleJS _
                & ",cursor: 'move'" _
            & "})" & ResizeableJS & ";" _
        & "});"
    '
    ' make this item droppable greedy
    '
    Return_RequiredJS = Return_RequiredJS _
        & CR _
        & "$(function(){" _
            & "$('#" & ItemHtmlID & "').droppable({" _
                & "hoverClass: '" & DroppableHoverClass & "'," _
                & "greedy: true" _
            & "});" _
        & "});"
    '
    GetDodad = Dodad
    '
    Exit Function
ErrorTrap:
    Call HandleError("DashboardClass", "GetDodad", Err.Number, Err.Source, Err.Description, True, False)
End Function

Public Function GetGridWrapper(Title As String, Content As String, Width As Long, Height As Long) As String
    On Error GoTo ErrorTrap

    Dim Stream As String
    Dim SpacerHeight As Long
    Dim InsideWidth As Long
    Dim InsideHeight As Long
    
    SpacerHeight = kmaEncodeInteger(Height) - 38
    InsideWidth = kmaEncodeInteger(Width)
    InsideHeight = kmaEncodeInteger(Height) - 70
    
    Stream = Stream & "<table cellpadding=""0"" cellspacing=""0"" border=""0"">"
    
    Stream = Stream & "<tr>"
    Stream = Stream & "<td><img src=""/upload/dashboard/container_01.png""></td>"
    Stream = Stream & "<td width=""" & InsideWidth & """><img src=""/upload/dashboard/container_02.png"" height=""13"" width=""100%""></td>"
    Stream = Stream & "<td><img src=""/upload/dashboard/container_03.png""></td>"
    Stream = Stream & "</tr>"
    
    Stream = Stream & "<tr>"
    Stream = Stream & "<td style=""BACKGROUND: url(/upload/dashboard/container_04.png) repeat-y left top"">"
    Stream = Stream & "<img src=""/ccLib/images/spacer.gif"" height=""" & SpacerHeight & """ width=""12"">"
    Stream = Stream & "</td>"
    
    Stream = Stream & "<td valign=""top"" style=""BACKGROUND: url(/upload/dashboard/container_05.png) left top"">"
    
    Stream = Stream & "<div class=""dodadTitle"">" & Title & "</div>"
    Stream = Stream & "<div style=""overflow: auto; width: 100%; height: " & InsideHeight & ";"">"
    
    Stream = Stream & Content
    
    Stream = Stream & "</div>"
    
    Stream = Stream & "</td>"
    
    Stream = Stream & "<td style=""BACKGROUND: url(/upload/dashboard/container_06.png) repeat-y left top"">"
    Stream = Stream & "<img src=""/ccLib/images/spacer.gif"" height=""" & SpacerHeight & """ width=""18"">"
    Stream = Stream & "</td>"
    Stream = Stream & "</tr>"
    
    Stream = Stream & "<tr>"
    Stream = Stream & "<td><img src=""/upload/dashboard/container_07.png""></td>"
    Stream = Stream & "<td width=""" & InsideWidth & """><img src=""/upload/dashboard/container_08.png"" height=""22"" width=""100%""></td>"
    Stream = Stream & "<td><img src=""/upload/dashboard/container_09.png""></td>"
    Stream = Stream & "</tr>"
    
    Stream = Stream & "</table>"
    
    GetGridWrapper = Stream

    Exit Function
ErrorTrap:
    Call HandleError("DashboardClass", "GetGridWrapper", Err.Number, Err.Source, Err.Description, True, False)
    End Function
'
'
'
Public Function GetXMLAttribute(Node As IXMLDOMNode, Name As String) As String
    On Error GoTo ErrorTrap
    
    Dim NodeAttribute As IXMLDOMAttribute
    Dim ResultNode As IXMLDOMNode
    Dim UcaseName As String
    Dim Found As Boolean
    
    Found = False
    If Not (Node.Attributes Is Nothing) Then
    Set ResultNode = Node.Attributes.getNamedItem(Name)
    If (ResultNode Is Nothing) Then
        UcaseName = UCase(Name)
        For Each NodeAttribute In Node.Attributes
            If UCase(NodeAttribute.nodeName) = UcaseName Then
                GetXMLAttribute = NodeAttribute.nodeValue
                Found = True
                Exit For
                End If
            Next
        If Not Found Then
            GetXMLAttribute = ""
        End If
    Else
        GetXMLAttribute = ResultNode.nodeValue
        Found = True
    End If
    End If
    Exit Function

ErrorTrap:
    Call HandleError("DashboardClass", "GetXMLAttribute", Err.Number, Err.Source, Err.Description, True, True)
    Resume Next
    End Function
'
'
'
Public Function LoadConfig(Main As Object) As MSXML2.DOMDocument60
    '
    Dim Config As String
    Dim DefaultConfigfilename As String
    Dim UserConfigFilename As String
    Dim objFSO As New kmaFileSystem3.FileSystemClass
    '
    ' store path to config file in a site property so defaults can be customized (like Realestate Sites, etc)
    '
    DefaultConfigfilename = "upload\dashboard\dashconfig.xml"
    DefaultConfigfilename = Main.GetSiteProperty("Dashboard Default Config Content Filename", DefaultConfigfilename)
    DefaultConfigfilename = Main.PhysicalFilePath & DefaultConfigfilename
    'DefaultConfigfilename = Main.PhysicalFilePath & "upload\dashboard\dashconfig.xml"
    UserConfigFilename = Main.PhysicalFilePath & "upload\dashboard\dashconfig." & Main.memberID & ".xml"
    Config = objFSO.ReadFile(UserConfigFilename)
    If Config = "" Then
        Config = objFSO.ReadFile(DefaultConfigfilename)
        Call objFSO.SaveFile(UserConfigFilename, Config)
    End If
    Set LoadConfig = New DOMDocument60
    LoadConfig.loadXML (Config)
    '
End Function
'
'
'
Public Sub SaveConfig(Main As Object, objXML As DOMDocument60)
    '
    Dim UserConfigFilename As String
    Dim objFSO As New kmaFileSystem3.FileSystemClass
    '
    UserConfigFilename = Main.PhysicalFilePath & "upload\dashboard\dashconfig." & Main.memberID & ".xml"
    Call objFSO.SaveFile(UserConfigFilename, objXML.xml)
    '
End Sub

