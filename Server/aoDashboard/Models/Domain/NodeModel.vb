
Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses
Imports Contensive.Models.Db

Namespace Models
    Public Class NodeModel
        '
        '=====================================================================================
        ''' <summary>
        ''' the entire contructed object that is moved around on the dashboard.
        ''' From inside out:
        ''' DodadContent - The inner-most part of the Dodad, the actual content that is on the desktop
        ''' DesignWrapper - the visible border for all Dodads
        ''' ControlWrapper = the absolute positioned wrapper that contains all drag/drop/resize/toolbar features
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="nodeConfig"></param>
        ''' <returns></returns>
        Public Shared Function getNodeHtml(cp As CPBaseClass, nodeConfig As Models.ConfigModel.ConfigNodeModel) As String
            Dim result As String = ""
            Try
                Dim AddonOptions As String = ""
                For Each nvp In nodeConfig.addonArgList
                    AddonOptions &= "&" & nvp.name & "=" & nvp.value
                Next
                AddonOptions = If(String.IsNullOrEmpty(AddonOptions), "", AddonOptions.Substring(1))
                Dim WrapperWidth As Integer = 77
                Dim WrapperHeight As Integer = 77
                Dim IconWidth As Integer = 57
                Dim IconHeight As Integer = 59
                Dim IconSprites As Integer = 1
                Dim IsIcon As Boolean = True
                Dim IconFileName As String = ""
                Dim ShortcutHref As String = ""
                Dim ItemHTMLClass As String = ""
                Dim DroppableHoverClass As String = ""
                Dim ToolBar As String = ""
                Dim title As String = ""
                Dim addon As AddonModel = Nothing
                Dim IconHtml As String
                If (nodeConfig.addonGUID <> "") And (nodeConfig.addonGUID <> "0") Then
                    '
                    ' -- render addon
                    If IsNumeric(nodeConfig.addonGUID) And Len(nodeConfig.addonGUID) < 10 Then
                        addon = AddonModel.create(Of AddonModel)(cp, CInt(nodeConfig.addonGUID))
                    Else
                        addon = AddonModel.create(Of AddonModel)(cp, nodeConfig.addonGUID)
                    End If
                    If (addon Is Nothing) Then
                        '
                        ' -- this node is not valid, exit
                        Return String.Empty
                    End If
                    title = addon.name
                    If (nodeConfig.state = ConfigModel.ConfigNodeState.closed) Then
                        '
                        ' Addon is just an icon
                        ItemHTMLClass = "dashNode iconNode"
                        IsIcon = True
                        IconFileName = addon.iconFilename
                        IconWidth = cp.Utils.EncodeInteger(addon.iconWidth)
                        IconHeight = cp.Utils.EncodeInteger(addon.iconHeight)
                        IconSprites = cp.Utils.EncodeInteger(addon.iconSprites)
                        ShortcutHref = If(String.IsNullOrWhiteSpace(nodeConfig.link), "?addonid=" & addon.id, nodeConfig.link)
                        'ToolBar += "<a alt=""Run In window"" title=""Run In Window"" href=""#"" onClick=""dashOpenNode('" & nodeConfig.key & "','" & nodeConfig.key & "');return false;""><i title=""restore"" class=""fas fa-window-restore"" style=""color:#222""></i></a>"
                        ToolBar += "<a alt=""Remove from dashboard"" title=""Remove from dashboard"" href=""#"" onClick=""dashDeleteNode('" & nodeConfig.key & "','" & nodeConfig.key & "');return false;""><i title=""close"" class=""fas fa-window-close"" style=""color:#222""></i></a>"
                    Else
                        '
                        ' open Addon running in a wrapper
                        ItemHTMLClass = "dashNode windowNode"
                        IsIcon = False
                        WrapperWidth = nodeConfig.sizex
                        WrapperHeight = nodeConfig.sizey
                        ShortcutHref = If(String.IsNullOrWhiteSpace(nodeConfig.link), "", nodeConfig.link)
                        ToolBar += "<div style=""float:left"">" & title & "</div>"
                        ToolBar += "<a alt=""Minimize"" title=""Minimize"" href=""#"" onClick=""closeNode('" & nodeConfig.key & "','" & nodeConfig.key & "');return false;""><i title=""restore"" class=""fas fa-window-minimize"" style=""color:#222""></i></a>"
                        ToolBar += "<a alt=""Remove from dashboard"" title=""Remove from dashboard"" href=""#"" onClick=""dashDeleteNode('" & nodeConfig.key & "','" & nodeConfig.key & "');return false;""><i title=""close"" class=""fas fa-window-close"" style=""color:#222""></i></a>"
                    End If
                    DroppableHoverClass = "droppableHover"
                    If (String.IsNullOrEmpty(IconFileName)) Then
                        IconFileName = "/dashboard/addon.png"
                        IconWidth = 57
                        IconHeight = 59
                        IconSprites = 4
                    End If
                ElseIf (nodeConfig.contentGUID <> "") Or (nodeConfig.contentName <> "") Then
                    '
                    ' -- Node is content
                    IsIcon = True
                    ItemHTMLClass = "dashNode iconNode"
                    Dim content As ContentModel = Nothing
                    If (nodeConfig.contentGUID <> "") Then
                        content = ContentModel.create(Of ContentModel)(cp, nodeConfig.contentGUID)
                    Else
                        content = ContentModel.createByUniqueName(Of ContentModel)(cp, nodeConfig.contentName)
                    End If
                    If (content Is Nothing) Then
                        '
                        ' -- this node is not valid
                        Return String.Empty
                    End If
                    IconFileName = content.IconLink
                    IconWidth = content.IconWidth
                    IconHeight = content.IconHeight
                    IconSprites = content.IconSprites
                    ShortcutHref = If(String.IsNullOrWhiteSpace(nodeConfig.link), "?cid=" & content.id, nodeConfig.link)
                    DroppableHoverClass = ""
                    content.id = 0
                    AddonOptions = ""
                    nodeConfig.state = ConfigModel.ConfigNodeState.closed
                    title = content.name
                    ToolBar = "" _
                        & "<a href=""#"" onClick=""dashDeleteNode('" & nodeConfig.key & "','" & nodeConfig.key & "');return false;""><i title=""close"" class=""fas fa-window-close"" style=""color:#222""></i></a>" _
                        & ""
                    If (String.IsNullOrEmpty(IconFileName)) Then
                        IconFileName = "/dashboard/content.png"
                        IconWidth = 57
                        IconHeight = 59
                        IconSprites = 4
                    End If
                Else
                    '
                    ' -- simple link icon
                    IsIcon = True
                    ItemHTMLClass = "dashNode iconNode"
                    IconFileName = "/dashboard/content.png"
                    IconWidth = 57
                    IconHeight = 59
                    IconSprites = 4
                    ShortcutHref = nodeConfig.link
                    DroppableHoverClass = ""
                    nodeConfig.state = ConfigModel.ConfigNodeState.closed
                    title = nodeConfig.title
                    ToolBar = "" _
                        & "<a href=""#"" onClick=""dashDeleteNode('" & nodeConfig.key & "','" & nodeConfig.key & "');return false;""><i title=""close"" class=""fas fa-window-close"" style=""color:#222""></i></a>" _
                        & ""
                End If
                '
                ' -- Build inner content of icon or addon
                Dim DoDadContent As String = ""
                If (nodeConfig.state = ConfigModel.ConfigNodeState.closed) Then
                    '
                    ' Icon Shortcut (Content or Addon)
                    If IconSprites < 2 Then
                        '
                        ' Flat image
                        IconHtml = "<img title=""" & title & """ border=""0"" src=""" & IconFileName & """>"
                    Else
                        '
                        ' Sprites
                        IconHtml = Controllers.DashboardIconController.getDashboardIconHtml("", IconWidth, IconHeight, IconSprites, True, "", IconFileName, cp.Site.FilePath, title, title, "", 0, "")
                    End If
                    DoDadContent = "" _
                        & vbCrLf & vbTab & vbTab & "<a class=""shortcut"" href=""" & ShortcutHref & """>" & IconHtml & "<br>" & title & "</a>" _
                        & ""
                ElseIf (addon IsNot Nothing) Then
                    '
                    ' -- execute the addon
                    DoDadContent = cp.Utils.ExecuteAddon(addon.ccguid)
                End If
                '
                ' -- add wrapper (deprecated)
                Dim DodadWrappedContent As String = DoDadContent
                Dim DesignResizerHTMLId As String = "designResizer" & nodeConfig.key
                '
                ' -- add Design-Resizer (an inner wrapper to use for resizing the design wrapper with the control wrapper)
                If Not IsIcon Then
                    '
                    ' non-icon, add resize
                    DodadWrappedContent = "" _
                        & "<div" _
                            & " id=""" & DesignResizerHTMLId & """" _
                            & " class=""designResizer""" _
                            & " style=""height: " & WrapperHeight & "px;""" _
                            & " >" _
                            & (DodadWrappedContent) _
                            & "</div>"
                End If
                '
                ' Build Toolbar wrapper
                ToolBar = "" _
                    & "<div class=""toolBarInner""><div class=""toolBarGrip""><i class=""fas fa-grip-horizontal"" style=""color:#222;""></i></div>" _
                    & ToolBar _
                    & "</div>" _
                    & ""
                Dim ToolBarHTMLId As String = "toolBar" & nodeConfig.key
                ToolBar = "" _
                    & "<div" _
                    & " class=""toolBar""" _
                    & " onmouseout=""outTools(this)""" _
                    & " onmouseover=""overTools(this)""" _
                    & " id=""" & ToolBarHTMLId & """" _
                    & " >" _
                    & (ToolBar) _
                    & "</div>" _
                    & ""
                '
                ' Assemble common elements from all item types
                Dim Dodad As String = "" _
                    & "<div id=""" & nodeConfig.key & """" _
                    & " class=""" & ItemHTMLClass & """" _
                    & " onmouseout=""outBody('" & ToolBarHTMLId & "')""" _
                    & " onmouseover=""overBody('" & ToolBarHTMLId & "')""" _
                    & " style=""top:" & nodeConfig.y & "px; left:" & nodeConfig.x & "px; width: " & WrapperWidth & "px;""" _
                    & " >" _
                    & (ToolBar) _
                    & (DodadWrappedContent) _
                    & "</div>" _
                    & ""
                '
                ' ----- Control Wrapper - Make this new item resizeable and draggable
                Dim ResizeableJS As String = ""
                Dim HandleJS As String = ""
                If (Not IsIcon) Then
                    '
                    ' non-icon
                    ResizeableJS = ".resizable({" _
                        & "alsoResize: '#" & DesignResizerHTMLId & "'" _
                        & ",stop: function(event, ui) {" _
                        & "/* alert('resize stop') */" _
                        & "var e=document.getElementById('" & nodeConfig.key & "');" _
                        & "var r=document.getElementById('" & DesignResizerHTMLId & "');" _
                        & "cj.ajax.addon('dashboardresize','ptr=" & nodeConfig.key & "&x='+e.style.width+'&y='+r.style.height);" _
                        & "dashResize();" _
                        & "}" _
                        & "})"
                    HandleJS = ",handle: '#" & ToolBarHTMLId & "'"
                End If
                result = Dodad
            Catch ex As ArgumentException
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
        ''' <summary>
        ''' create the html for the node from individual node values (transitional signature)
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="AddonIdOrGuid"></param>
        ''' <param name="ContentGuid"></param>
        ''' <param name="ContentName"></param>
        ''' <param name="Title"></param>
        ''' <param name="PosX"></param>
        ''' <param name="PosY"></param>
        ''' <param name="State"></param>
        ''' <param name="SizeX"></param>
        ''' <param name="SizeY"></param>
        ''' <param name="AddonOptions"></param>
        ''' <param name="WrapperID"></param>
        ''' <param name="NodeKey"></param>
        ''' <param name="IconZIndex"></param>
        ''' <returns></returns>
        Public Shared Function getNodeHtml(cp As CPBaseClass, AddonIdOrGuid As String, ContentGuid As String, ContentName As String, Title As String, PosX As Integer, PosY As Integer, State As String, SizeX As Models.ConfigModel.ConfigNodeState, SizeY As Integer, AddonOptions As String, WrapperID As Integer, NodeKey As String, IconZIndex As Integer) As String
            Dim result As String = ""

            Try
                Dim DodadWrappedContent As String
                Dim HandleJS As String
                Dim ResizeableJS As String
                Dim WrapperWidth As Integer
                Dim WrapperHeight As Integer
                Dim IconWidth As Integer
                Dim IconHeight As Integer
                Dim IconSprites As Integer
                Dim IsIcon As Boolean
                Dim Dodad As String
                Dim ContentID As Integer
                Dim DesignResizerHTMLId As String
                Dim ToolBarHTMLId As String
                Dim content As ContentModel = Nothing
                Dim addon As AddonModel = Nothing
                Dim IconFileName As String = ""
                Dim ShortcutHref As String = ""
                Dim ItemHTMLClass As String = ""
                Dim DroppableHoverClass As String = ""
                Dim ToolBar As String = ""
                '
                ToolBarHTMLId = "toolBar" & NodeKey
                DesignResizerHTMLId = "designResizer" & NodeKey
                WrapperWidth = 77
                WrapperHeight = 77
                '
                Dim iconHtml As String = ""
                If (AddonIdOrGuid <> "") And (AddonIdOrGuid <> "0") Then
                    If IsNumeric(AddonIdOrGuid) And Len(AddonIdOrGuid) < 10 Then
                        addon = AddonModel.create(Of AddonModel)(cp, CInt(AddonIdOrGuid))
                    Else
                        addon = AddonModel.create(Of AddonModel)(cp, AddonIdOrGuid)
                    End If
                    '
                    ' Node is an Add-on
                    '
                    If (addon IsNot Nothing) Then
                        If LCase(State) = "closed" Then
                            '
                            ' Addon is just an icon
                            '
                            ItemHTMLClass = "dashNode iconNode"
                            IsIcon = True
                            IconFileName = addon.iconFilename
                            IconWidth = cp.Utils.EncodeInteger(addon.iconWidth)
                            IconHeight = cp.Utils.EncodeInteger(addon.iconHeight)
                            IconSprites = cp.Utils.EncodeInteger(addon.iconSprites)
                            ShortcutHref = "?addonid=" & addon.id
                            ToolBar = "" _
                                & "<a alt=""Remove from dashboard"" title=""Remove from dashboard"" href=""#"" onClick=""dashDeleteNode('" & NodeKey & "','" & NodeKey & "');return false;""><i title=""close"" class=""fas fa-window-close"" style=""color:#222""></i></a>" _
                                & ""
                            iconHtml = addon.iconHtml
                        Else
                            '
                            ' open Addon running in a wrapper
                            '
                            ItemHTMLClass = "dashNode windowNode"
                            IsIcon = False
                            WrapperWidth = SizeX
                            WrapperHeight = SizeY
                            ShortcutHref = ""
                            ToolBar = "" _
                                & "<div style=""float:left"">&nbsp;" & Title & "</div>" _
                                & "<a alt=""Remove from dashboard"" title=""Remove from dashboard"" href=""#"" onClick=""dashDeleteNode('" & NodeKey & "','" & NodeKey & "');return false;""><i title=""close"" class=""fas fa-window-close"" style=""color:#222""></i></a>" _
                                & ""
                        End If
                    End If
                    DroppableHoverClass = "droppableHover"
                ElseIf (ContentGuid <> "") Or (ContentName <> "") Then
                    '
                    ' Node is content
                    '
                    IsIcon = True
                    ItemHTMLClass = "iconNode"
                    If (ContentGuid <> "") Then
                        content = ContentModel.create(Of ContentModel)(cp, ContentGuid)
                    Else
                        content = ContentModel.createByUniqueName(Of ContentModel)(cp, ContentName)
                    End If
                    If (content IsNot Nothing) Then
                        ContentID = content.id
                        ContentName = content.name
                        IconFileName = content.IconLink
                        IconWidth = content.IconWidth
                        IconHeight = content.IconHeight
                        IconSprites = content.IconSprites
                        ShortcutHref = "?cid=" & ContentID
                        DroppableHoverClass = ""
                        content.id = 0
                        ContentGuid = ""
                        AddonOptions = ""
                        State = "closed"
                        Title = ContentName
                        iconHtml = content.iconHtml
                    End If
                    ToolBar = "" _
                        & "<a href=""#"" onClick=""dashDeleteNode('" & NodeKey & "','" & NodeKey & "');return false;""><i title=""close"" class=""fas fa-window-close"" style=""color:#222""></i></a>" _
                        & ""
                End If
                '
                ' Build inner content of icon or addon
                Dim DoDadContent As String = ""
                If LCase(State) = "closed" Then
                    '
                    ' Icon Shortcut (Content or Addon)
                    '
                    If (String.IsNullOrWhiteSpace(iconHtml)) Then
                        '
                        ' -- create icon html from filename, height, sprites
                        If IconFileName = "" Then
                            '
                            ' Default Icon
                            '
                            If (addon IsNot Nothing) Then
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
                            iconHtml = "<img title=""" & Title & """ border=""0"" src=""" & IconFileName & """>"
                        Else
                            '
                            ' Sprites
                            '
                            iconHtml = Controllers.DashboardIconController.getDashboardIconHtml("", IconWidth, IconHeight, IconSprites, True, "", IconFileName, cp.Site.FilePath, Title, Title, "", 0, iconHtml)
                        End If
                    End If
                    DoDadContent = vbCrLf & vbTab & vbTab & "<a class=""shortcut"" href=""" & ShortcutHref & """>" & iconHtml & "<br>" & Title & "</a>"
                ElseIf (addon IsNot Nothing) Then
                    '
                    ' -- execute the addon
                    DoDadContent = cp.Utils.ExecuteAddon(addon.ccguid)
                End If
                '
                DodadWrappedContent = DoDadContent
                '
                ' Add Design-Resizer (an inner wrapper to use for resizing the design wrapper with the control wrapper)
                If Not IsIcon Then
                    '
                    ' non-icon, add resize
                    DodadWrappedContent = "" _
                        & "<div" _
                            & " id=""" & DesignResizerHTMLId & """" _
                            & " class=""designResizer""" _
                            & " style=""height: " & WrapperHeight & "px;""" _
                            & " >" _
                            & (DodadWrappedContent) _
                            & "</div>"
                End If
                '
                ' Build Toolbar wrapper
                ToolBar = "" _
                    & "<div class=""toolBarInner"">" _
                    & (ToolBar) _
                    & "</div>" _
                    & ""
                ToolBar = "" _
                    & "<div" _
                    & " class=""toolBar""" _
                    & " onmouseout=""outTools(this)""" _
                    & " onmouseover=""overTools(this)""" _
                    & " id=""" & ToolBarHTMLId & """" _
                    & " >" _
                    & (ToolBar) _
                    & "</div>" _
                    & ""
                '
                ' Assemble common elements from all item types
                '
                Dodad = "" _
                    & "<div id=""" & NodeKey & """" _
                    & " class=""" & ItemHTMLClass & """" _
                    & " onmouseout=""outBody('" & ToolBarHTMLId & "')""" _
                    & " onmouseover=""overBody('" & ToolBarHTMLId & "')""" _
                    & " style=""top:" & PosY & "px; left:" & PosX & "px; width: " & WrapperWidth & "px;""" _
                    & " >" _
                    & (ToolBar) _
                    & (DodadWrappedContent) _
                    & "</div>" _
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
                        & "var e=document.getElementById('" & NodeKey & "');" _
                        & "var r=document.getElementById('" & DesignResizerHTMLId & "');" _
                        & "cj.ajax.addon('dashboardresize','ptr=" & NodeKey & "&x='+e.style.width+'&y='+r.style.height);" _
                        & "dashResize();" _
                        & "}" _
                        & "})"
                    HandleJS = ",handle: '#" & ToolBarHTMLId & "'"
                End If
                'Return_RequiredJS &= "" _
                '    & CR & "//" _
                '    & CR & "// -- add droppable to desktop" _
                '    & CR & "jQuery(function(){" _
                '    & CR2 & "jQuery('#" & ItemHtmlID & "').draggable({" _
                '    & CR2 & "stop: function(event, ui){" _
                '    & CR2 & "var e=document.getElementById('" & ItemHtmlID & "');" _
                '    & CR2 & "cj.ajax.addonCallback('dashboarddragstop','ptr=" & NodePtr & "&x='+e.style.left+'&y='+e.style.top,dashResize);" _
                '    & CR2 & "}" _
                '    & CR2 & ",start: function(event, ui){" _
                '    & CR2 & "var e=document.getElementById('" & ItemHtmlID & "');" _
                '    & CR2 & "e.style.zIndex=iconZIndexTop++;" _
                '    & CR2 & "jQuery('#" & ItemHtmlID & "').draggable('option', 'zIndex', iconZIndexTop );" _
                '    & CR2 & "}" _
                '    & CR2 & ",revert: 'invalid'" _
                '    & CR2 & ",zIndex: " & IconZIndex & "" _
                '    & CR2 & ",hoverClass: '" & DroppableHoverClass & "'" _
                '    & CR2 & ",opacity: 0.50" _
                '    & HandleJS _
                '    & CR2 & ",cursor: 'move'" _
                '    & CR2 & "})" & ResizeableJS & ";" _
                '    & CR & "});"
                ''
                '' make this item droppable greedy
                ''
                'Return_RequiredJS = Return_RequiredJS _
                '    & CR _
                '    & "jQuery(function(){" _
                '        & "jQuery('#" & ItemHtmlID & "').droppable({" _
                '            & "hoverClass: '" & DroppableHoverClass & "'," _
                '            & "greedy: true" _
                '        & "});" _
                '    & "});"
                ''
                result = Dodad
            Catch ex As ArgumentException
                cp.Site.ErrorReport(ex)
            Finally

            End Try

            Return result
        End Function


    End Class
End Namespace
