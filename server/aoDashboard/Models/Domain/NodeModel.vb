
Imports System.Net
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
        Public Shared Function getNodeHtml(cp As CPBaseClass, nodeConfig As Models.ConfigNodeModel) As String
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
                Dim IconHtml As String = ""
                If (Not String.IsNullOrEmpty(nodeConfig.addonGUID)) AndAlso (nodeConfig.addonGUID <> "0") Then
                    '
                    ' -- render addon (addonGuid legacy is id)
                    If IsNumeric(nodeConfig.addonGUID) And Len(nodeConfig.addonGUID) < 10 Then
                        addon = DbBaseModel.create(Of AddonModel)(cp, CInt(nodeConfig.addonGUID))
                    Else
                        addon = DbBaseModel.create(Of AddonModel)(cp, nodeConfig.addonGUID)
                    End If
                    If (addon Is Nothing) Then
                        '
                        ' -- this node is not valid, exit
                        Return String.Empty
                    End If
                    title = addon.name
                    If (nodeConfig.state = ConfigNodeState.closed) Then
                        '
                        ' Addon is just an icon
                        ItemHTMLClass = "dashNode iconNode"
                        IsIcon = True
                        IconFileName = addon.iconFilename
                        IconWidth = cp.Utils.EncodeInteger(addon.iconWidth)
                        IconHeight = cp.Utils.EncodeInteger(addon.iconHeight)
                        IconSprites = cp.Utils.EncodeInteger(addon.iconSprites)
                        IconHtml = addon.iconHtml
                        'If (String.IsNullOrWhiteSpace(IconHtml)) Then
                        '    '
                        '    ' -- lots of server hits
                        '    addon.iconHtml = getHtmlIcon(cp, addon.ccguid, "getAddonIcon")
                        '    addon.save(cp)
                        'End If
                        ShortcutHref = If(String.IsNullOrWhiteSpace(nodeConfig.link), "?addonid=" & addon.id, nodeConfig.link)
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
                    If (String.IsNullOrWhiteSpace(IconHtml) And String.IsNullOrEmpty(IconFileName)) Then
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
                        content = DbBaseModel.create(Of ContentModel)(cp, nodeConfig.contentGUID)
                    Else
                        content = DbBaseModel.createByUniqueName(Of ContentModel)(cp, nodeConfig.contentName)
                    End If
                    If (content Is Nothing) Then
                        '
                        ' -- this node is not valid
                        Return String.Empty
                    End If
                    IconHtml = content.iconHtml
                    'If (String.IsNullOrWhiteSpace(IconHtml)) Then
                    '    '
                    '    ' -- lots of server hits
                    '    content.iconHtml = getHtmlIcon(cp, content.ccguid, "getContentIcon")
                    '    content.save(cp)
                    'End If
                    IconFileName = content.iconLink
                    IconWidth = content.iconWidth
                    IconHeight = content.iconHeight
                    IconSprites = content.iconSprites
                    ShortcutHref = If(String.IsNullOrWhiteSpace(nodeConfig.link), "?cid=" & content.id, nodeConfig.link)
                    DroppableHoverClass = ""
                    content.id = 0
                    AddonOptions = ""
                    nodeConfig.state = ConfigNodeState.closed
                    title = content.name
                    ToolBar = "" _
                        & "<a href=""#"" onClick=""dashDeleteNode('" & nodeConfig.key & "','" & nodeConfig.key & "');return false;""><i title=""close"" class=""fas fa-window-close"" style=""color:#222""></i></a>" _
                        & ""
                    If (String.IsNullOrWhiteSpace(IconHtml) And String.IsNullOrEmpty(IconFileName)) Then
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
                    nodeConfig.state = ConfigNodeState.closed
                    title = nodeConfig.title
                    ToolBar = "" _
                        & "<a href=""#"" onClick=""dashDeleteNode('" & nodeConfig.key & "','" & nodeConfig.key & "');return false;""><i title=""close"" class=""fas fa-window-close"" style=""color:#222""></i></a>" _
                        & ""
                End If
                '
                ' -- Build inner content of icon or addon
                Dim DoDadContent As String = ""
                If (nodeConfig.state = ConfigNodeState.closed) Then
                    '
                    ' Icon Shortcut (Content or Addon)
                    If (String.IsNullOrWhiteSpace(IconHtml)) Then
                        '
                        ' -- image based icon
                        If IconSprites < 2 Then
                            '
                            ' Flat image
                            IconHtml = "<img title=""" & title & """ border=""0"" src=""" & IconFileName & """>"
                        Else
                            '
                            ' Sprites
                            IconHtml = Controllers.DashboardIconController.getDashboardIconHtml("", IconWidth, IconHeight, IconSprites, True, "", IconFileName, cp.Site.FilePath, title, title, "", 0, IconHtml)
                        End If
                    End If
                    DoDadContent = "" _
                        & vbCrLf & vbTab & vbTab & "<a class=""shortcut"" href=""" & ShortcutHref & """>" & IconHtml & "<div>" & title & "</div></a>" _
                        & ""
                ElseIf (addon IsNot Nothing) Then
                    '
                    ' -- execute the addon
                    DoDadContent = cp.Addon.Execute(addon.ccguid)
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
        ''
        ''=====================================================================================
        '''' <summary>
        '''' Fetch the addon's html icon and swallow errors from http fetch
        '''' </summary>
        '''' <param name="cp"></param>
        '''' <param name="sourceGuid"></param>
        '''' <returns></returns>
        'Public Shared Function getHtmlIcon(cp As CPBaseClass, sourceGuid As String, supportSiteRemoteMethod As String) As String
        '    Try
        '        '
        '        ' -- if there is a support site error, this throws an exception that we need to swallow
        '        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12
        '        Dim http As New WebClient()
        '        Return http.DownloadString("https://support.contensive.com/" & supportSiteRemoteMethod & "?guid=" & cp.Utils.EncodeUrl(sourceGuid))
        '    Catch ex As Exception
        '        cp.Site.ErrorReport(ex, "exception fetching an addon iconHtml from https://support.contensive.com/" & supportSiteRemoteMethod & "?guid=" & cp.Utils.EncodeUrl(sourceGuid))
        '        Return ""
        '    End Try
        'End Function
    End Class
End Namespace
