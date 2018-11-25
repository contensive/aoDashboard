
Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Models
    Public Class NodeModel

        '
        '=====================================================================================
        '
        '   Dodad - the entire contructed object that is moved around on the dashboard
        '
        '       From inside out:
        '           DodadContent - The inner-most part of the Dodad, the actual content that is on the desktop
        '           DesignWrapper - the visible border for all Dodads
        '           ControlWrapper = the absolute positioned wrapper that contains all drag/drop/resize/toolbar features
        '
        Public Shared Function getNodeHtml(cp As CPBaseClass, nodeConfig As Models.configModel.ConfigNodeModel) As String
            Dim AddonOptions As String = ""
            For Each nvp In nodeConfig.addonArgList
                AddonOptions &= "&" & nvp.name & "=" & nvp.value
            Next
            AddonOptions = If(String.IsNullOrEmpty(AddonOptions), "", AddonOptions.Substring(1))
            Return getNodeHtml(cp, nodeConfig.addonGUID, nodeConfig.contentGUID, nodeConfig.contentName, nodeConfig.title, nodeConfig.x, nodeConfig.y, nodeConfig.state, nodeConfig.sizex, nodeConfig.sizey, AddonOptions, nodeConfig.wrapperId, nodeConfig.key, nodeConfig.z)
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
        Public Shared Function getNodeHtml(cp As CPBaseClass, AddonIdOrGuid As String, ContentGuid As String, ContentName As String, Title As String, PosX As Integer, PosY As Integer, State As String, SizeX As Models.configModel.ConfigNodeState, SizeY As Integer, AddonOptions As String, WrapperID As Integer, NodeKey As String, IconZIndex As Integer) As String
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
                Dim IconImg As String
                Dim DesignResizerHTMLId As String
                Dim ToolBarHTMLId As String
                Dim content As Models.contentModel = Nothing
                Dim addon As Models.addonModel = Nothing
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
                If (AddonIdOrGuid <> "") And (AddonIdOrGuid <> "0") Then
                    If IsNumeric(AddonIdOrGuid) And Len(AddonIdOrGuid) < 10 Then
                        addon = Models.addonModel.create(cp, CInt(AddonIdOrGuid))
                    Else
                        addon = Models.addonModel.create(cp, AddonIdOrGuid)
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
                            IconFileName = addon.IconFilename
                            IconWidth = addon.IconWidth
                            IconHeight = addon.IconHeight
                            IconSprites = addon.IconSprites
                            ShortcutHref = "?addonid=" & addon.id
                            'DoDadContent = GetDodadContent(cp, Addon.ID, AddonGuid, AddonOptions, State, IconSprites, Title, IconFileName, IconWidth, IconHeight, cp.ServerFilePath, WrapperID, 0, 0, ShortcutHref)
                            ToolBar = "" _
                                & "<a alt=""Minimize"" title=""Minimize"" href=""#"" onClick=""Return False;"" Class=""opacity50""><img border=0 src=""/cclib/images/opendown1313.gif"" width=""13"" height=""13""></a>" _
                                & "<a alt=""Run In window"" title=""Run In Window"" href=""#"" onClick=""dashOpenNode('" & NodeKey & "','" & NodeKey & "');return false;""><img border=0 src=""/cclib/images/box1313.gif"" width=""13"" height=""13""></a>" _
                                & "<a alt=""Run full size"" title=""Run full size""  href=""?addonguid=" & addon.ccguid & """><img border=0 src=""/cclib/images/openup1313.gif"" width=""13"" height=""13""></a>" _
                                & "<a alt=""Remove from dashboard"" title=""Remove from dashboard"" href=""#"" onClick=""dashDeleteNode('" & NodeKey & "','" & NodeKey & "');return false;""><img border=0 src=""/cclib/images/closex1313.gif"" width=""13"" height=""13""></a>" _
                                & ""
                        Else
                            '
                            ' open Addon running in a wrapper
                            '
                            ItemHTMLClass = "dashNode windowNode"
                            IsIcon = False
                            WrapperWidth = SizeX
                            WrapperHeight = SizeY
                            ShortcutHref = ""
                            'DoDadContent = GetDodadContent(cp, Addon.ID, AddonGuid, AddonOptions, State, IconSprites, Title, IconFileName, IconWidth, IconHeight, cp.ServerFilePath, WrapperID, 0, 0, "")
                            ToolBar = "" _
                                & "<div style=""float:left"">" & Title & "</div>" _
                                & "<a alt=""Minimize"" title=""Minimize"" href=""#"" onClick=""closeNode('" & NodeKey & "','" & NodeKey & "');return false;""><img border=0 src=""/cclib/images/opendown1313.gif"" width=""13"" height=""13""></a>" _
                                & "<a alt=""Run in window"" title=""Run in Window"" href=""#"" class=""opacity50"" onClick=""return false;""><img border=0 src=""/cclib/images/box1313.gif"" width=""13"" height=""13""></a>" _
                                & "<a alt=""Run full size"" title=""Run full size""  href=""?addonguid=" & addon.ccguid & """><img border=0 src=""/cclib/images/openup1313.gif"" width=""13"" height=""13""></a>" _
                                & "<a alt=""Remove from dashboard"" title=""Remove from dashboard"" href=""#"" onClick=""dashDeleteNode('" & NodeKey & "','" & NodeKey & "');return false;""><img border=0 src=""/cclib/images/closex1313.gif"" width=""13"" height=""13""></a>" _
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
                        content = Contensive.Addons.Dashboard.Models.contentModel.create(cp, ContentGuid)
                    Else
                        content = Models.contentModel.createByName(cp, ContentName)
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
                    End If
                    ToolBar = "" _
                        & "<a href=""#"" onClick=""dashDeleteNode('" & NodeKey & "','" & NodeKey & "');return false;""><img border=0 src=""/cclib/images/closex1313.gif"" width=""13"" height=""13""></a>" _
                        & ""
                End If
                '
                ' Build inner content of icon or addon
                Dim DoDadContent As String = ""
                If LCase(State) = "closed" Then
                    '
                    ' Icon Shortcut (Content or Addon)
                    '
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
                        IconImg = "<img title=""" & Title & """ border=""0"" src=""" & IconFileName & """>"
                    Else
                        '
                        ' Sprites
                        '
                        IconImg = GetAddonIconImg("", IconWidth, IconHeight, IconSprites, True, "", IconFileName, cp.Site.FilePath, Title, Title, "", 0)
                    End If
                    DoDadContent = "" _
                        & vbCrLf & vbTab & vbTab & "<a class=""shortcut"" href=""" & ShortcutHref & """>" & IconImg & "<br>" & Title & "</a>" _
                        & ""
                ElseIf (addon IsNot Nothing) Then
                    '
                    ' -- execute the addon
                    DoDadContent = cp.Utils.ExecuteAddon(addon.ccguid)
                End If
                '
                'DoDadContent = GetDodadContent(cp, Addon.ID, AddonGuid, AddonOptions, State, IconSprites, Title, IconFileName, IconWidth, IconHeight, cp.ServerFilePath, WrapperID, 0, 0, ShortcutHref)
                DodadWrappedContent = DoDadContent
                '
                ' Add Design-Resizer (an inner wrapper to use for resizing the design wrapper with the control wrapper)
                '
                If Not IsIcon Then
                    '
                    ' non-icon, add resize
                    '
                    DodadWrappedContent = "" _
                        & "<div" _
                            & " id=""" & DesignResizerHTMLId & """" _
                            & " class=""designResizer""" _
                            & " style=""height: " & WrapperHeight & "px;""" _
                            & " >" _
                            & (DodadWrappedContent) _
                            & "</div>"
                    'If WrapperID <> 0 Then
                    '    '
                    '    ' Add Design-Wrapper (first an inner wrapper to use for resizing the design wrapper with the control wrapper)
                    '    '
                    '    DodadWrappedContent = cp   WrapContent(DodadWrappedContent, WrapperID)
                    'End If
                End If
                '
                ' Build Toolbar wrapper
                '
                'ToolBar =  ToolBar
                ToolBar = "" _
                    & "<div" _
                    & " class=""toolBarInner""" _
                    & " >" _
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
