Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Controllers
    Public Class genericController
        '
        '=====================================================================================
        ' legacy code
        '=====================================================================================
        '
        '   Dodad - the entire contructed object that is moved around on the dashboard
        '
        '       From inside out:
        '           DodadContent - The inner-most part of the Dodad, the actual content that is on the desktop
        '           DesignWrapper - the visible border for all Dodads
        '           ControlWrapper = the absolute positioned wrapper that contains all drag/drop/resize/toolbar features
        '
        '
        Public Shared Function GetDodad(cp As CPBaseClass, AddonIdOrGuid As String, ContentGuid As String, ContentName As String, Title As String, PosX As Integer, PosY As Integer, State As String, SizeX As Integer, SizeY As Integer, AddonOptions As String, WrapperID As Integer, NodePtr As Integer, Return_RequiredJS As String, IconZIndex As Integer) As String
            Dim result As String = ""
            Try
                Dim DodadWrappedContent As String
                Dim HandleJS As String
                Dim ResizeableJS As String
                Dim WrapperWidth As Integer
                Dim WrapperHeight As Integer
                Dim DoDadContent As String
                Dim IconWidth As Integer
                Dim IconHeight As Integer
                Dim IconSprites As Integer
                Dim IsIcon As Boolean
                Dim Dodad As String
                ' Dim CS As Integer
                Dim ContentID As Integer
                Dim IconImg As String
                Dim ItemHtmlID As String
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
                ItemHtmlID = "dashnode" & NodePtr
                ToolBarHTMLId = "toolBar" & ItemHtmlID
                DesignResizerHTMLId = "designResizer" & ItemHtmlID
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
                            ItemHTMLClass = "iconNode"
                            IsIcon = True
                            IconFileName = addon.IconFilename
                            IconWidth = addon.IconWidth
                            IconHeight = addon.IconHeight
                            IconSprites = addon.IconSprites
                            ShortcutHref = "?addonid=" & addon.id
                            'DoDadContent = GetDodadContent(cp, Addon.ID, AddonGuid, AddonOptions, State, IconSprites, Title, IconFileName, IconWidth, IconHeight, cp.ServerFilePath, WrapperID, 0, 0, ShortcutHref)
                            ToolBar = "" _
                                & "<a alt=""Minimize"" title=""Minimize"" href=""#"" onClick=""Return False;"" Class=""opacity50""><img border=0 src=""/cclib/images/opendown1313.gif"" width=""13"" height=""13""></a>" _
                                & "<a alt=""Run In window"" title=""Run In Window"" href=""#"" onClick=""dashOpenNode('" & NodePtr & "','" & ItemHtmlID & "');return false;""><img border=0 src=""/cclib/images/box1313.gif"" width=""13"" height=""13""></a>" _
                                & "<a alt=""Run full size"" title=""Run full size""  href=""?addonguid=" & addon.ccguid & """><img border=0 src=""/cclib/images/openup1313.gif"" width=""13"" height=""13""></a>" _
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
                            'DoDadContent = GetDodadContent(cp, Addon.ID, AddonGuid, AddonOptions, State, IconSprites, Title, IconFileName, IconWidth, IconHeight, cp.ServerFilePath, WrapperID, 0, 0, "")
                            ToolBar = "" _
                    & "<div style=""float:left"">" & Title & "</div>" _
                    & "<a alt=""Minimize"" title=""Minimize"" href=""#"" onClick=""closeNode('" & NodePtr & "','" & ItemHtmlID & "');return false;""><img border=0 src=""/cclib/images/opendown1313.gif"" width=""13"" height=""13""></a>" _
                    & "<a alt=""Run in window"" title=""Run in Window"" href=""#"" class=""opacity50"" onClick=""return false;""><img border=0 src=""/cclib/images/box1313.gif"" width=""13"" height=""13""></a>" _
                    & "<a alt=""Run full size"" title=""Run full size""  href=""?addonguid=" & addon.ccguid & """><img border=0 src=""/cclib/images/openup1313.gif"" width=""13"" height=""13""></a>" _
                    & "<a alt=""Remove from dashboard"" title=""Remove from dashboard"" href=""#"" onClick=""dashDeleteNode('" & NodePtr & "','" & ItemHtmlID & "');return false;""><img border=0 src=""/cclib/images/closex1313.gif"" width=""13"" height=""13""></a>" _
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
                        content = Contensive.Addons.aoDashboard.Models.contentModel.create(cp, ContentGuid)
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
                        & "<a href=""#"" onClick=""dashDeleteNode('" & NodePtr & "','" & ItemHtmlID & "');return false;""><img border=0 src=""/cclib/images/closex1313.gif"" width=""13"" height=""13""></a>" _
                        & ""
                End If
                '
                ' Build inner content of icon or addon
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
                Else
                    '
                    ' Addon in a wrapper
                    '
                    '
                    ' from Csvr
                    '
                    ' Const ContextAdmin = 2
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
                    & "<div id=""" & ItemHtmlID & """" _
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
                result = Dodad
            Catch ex As ArgumentException
                cp.Site.ErrorReport(ex)
            Finally

            End Try
            Return result
        End Function


        Public Shared Function GetGridWrapper(cp As CPBaseClass, Title As String, Content As String, Width As Integer, Height As Integer) As String
            Dim result As String = ""
            Try

                Dim Stream As String = ""
                Dim SpacerHeight As Integer
                Dim InsideWidth As Integer
                Dim InsideHeight As Integer

                SpacerHeight = cp.Utils.EncodeInteger(Height) - 38
                InsideWidth = cp.Utils.EncodeInteger(Width)
                InsideHeight = cp.Utils.EncodeInteger(Height) - 70

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

                result = Stream
            Catch ex As ArgumentException
                cp.Site.ErrorReport(ex)
            End Try
        End Function
        '
        '
        '
        Public Shared Function LoadConfig(cp As CPBaseClass) As XmlDocument
            Dim result As New System.Xml.XmlDocument
            Try
                Dim UserConfigFilename As String = "upload\dashboard\dashconfig." & cp.User.Id & ".xml"
                Dim Config As String = cp.File.ReadVirtual(UserConfigFilename)
                If Config = "" Then
                    Dim DefaultConfigfilename As String
                    DefaultConfigfilename = "upload\dashboard\dashconfig.xml"
                    DefaultConfigfilename = cp.Site.GetText("Dashboard Default Config Content Filename", DefaultConfigfilename)
                    DefaultConfigfilename = cp.Site.PhysicalFilePath & DefaultConfigfilename
                    Config = cp.File.Read(DefaultConfigfilename)
                    Call cp.File.Save(UserConfigFilename, Config)
                End If
                result.LoadXml(Config)
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
        '
        '
        '
        Public Shared Sub SaveConfig(cp As CPBaseClass, objXML As XmlDocument)
            '
            Dim UserConfigFilename As String
            'Dim objFSO As Object
            'Dim objFSO As New kmaFileSystem3.FileSystemClass
            'objFSO = CreateObject("kmaFileSystem3.FileSystemClass")
            '
            UserConfigFilename = cp.Site.PhysicalFilePath & "upload\dashboard\dashconfig." & cp.User.Id & ".xml"
            Call cp.File.Save(UserConfigFilename, objXML.OuterXml)
            '
        End Sub

        '
        Public Shared Function encodeSqlTextLike(cp As CPBaseClass, source As String) As String
            Dim sqlText As String = cp.Db.EncodeSQLText(source)
            If sqlText.Length <= 2 Then
                Return String.Empty
            Else
                Return "'%" & sqlText.Substring(1, source.Length - 2) & "%'"
            End If
        End Function
        '
        '====================================================================================================
        ''' <summary>
        ''' return a normalized guid in registry format
        ''' </summary>
        ''' <param name="CP"></param>
        ''' <param name="registryFormat"></param>
        ''' <returns></returns>
        Public Shared Function getGUID(ByVal CP As BaseClasses.CPBaseClass, Optional ByRef registryFormat As Boolean = False) As String
            Dim result As String = ""
            Try
                Dim g As Guid = Guid.NewGuid
                If g <> Guid.Empty Then
                    result = g.ToString
                    '
                    If result <> "" Then
                        result = If(registryFormat, result, "{" & result & "}")
                    End If
                End If
            Catch ex As Exception
                CP.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
        '
        '
        '
        Public Shared Function GetAddonIconImg(AdminURL As String, IconWidth As Long, IconHeight As Long, IconSprites As Long, IconIsInline As Boolean, IconImgID As String, IconFilename As String, serverFilePath As String, IconAlt As String, IconTitle As String, ACInstanceID As String, IconSpriteColumn As Long) As String
            '
            Dim ImgStyle As String
            Dim IconHeightNumeric As Long
            '
            If IconAlt = "" Then
                IconAlt = "Add-on"
            End If
            If IconTitle = "" Then
                IconTitle = "Rendered as Add-on"
            End If
            If IconFilename = "" Then
                '
                ' No icon given, use the default
                '
                If IconIsInline Then
                    IconFilename = "/ccLib/images/IconAddonInlineDefault.png"
                    IconWidth = 62
                    IconHeight = 17
                    IconSprites = 0
                Else
                    IconFilename = "/ccLib/images/IconAddonBlockDefault.png"
                    IconWidth = 57
                    IconHeight = 59
                    IconSprites = 4
                End If
            ElseIf InStr(1, IconFilename, "://") <> 0 Then
                '
                ' icon is an Absolute URL - leave it
                '
            ElseIf Left(IconFilename, 1) = "/" Then
                '
                ' icon is Root Relative, leave it
                '
            Else
                '
                ' icon is a virtual file, add the serverfilepath
                '
                IconFilename = serverFilePath & IconFilename
            End If
            'IconFilename = kmaEncodeJavascript(IconFilename)
            If (IconWidth = 0) Or (IconHeight = 0) Then
                IconSprites = 0
            End If

            If IconSprites = 0 Then
                '
                ' just the icon
                '
                GetAddonIconImg = "<img" _
                    & " border=0" _
                    & " id=""" & IconImgID & """" _
                    & " onDblClick=""window.parent.OpenAddonPropertyWindow(this,'" & AdminURL & "');""" _
                    & " alt=""" & IconAlt & """" _
                    & " title=""" & IconTitle & """" _
                    & " src=""" & IconFilename & """"
                'GetAddonIconImg = "<img" _
                '    & " id=""AC,AGGREGATEFUNCTION,0," & FieldName & "," & ArgumentList & """" _
                '    & " onDblClick=""window.parent.OpenAddonPropertyWindow(this);""" _
                '    & " alt=""" & IconAlt & """" _
                '    & " title=""" & IconTitle & """" _
                '    & " src=""" & IconFilename & """"
                If IconWidth <> 0 Then
                    GetAddonIconImg = GetAddonIconImg & " width=""" & IconWidth & "px"""
                End If
                If IconHeight <> 0 Then
                    GetAddonIconImg = GetAddonIconImg & " height=""" & IconHeight & "px"""
                End If
                If IconIsInline Then
                    GetAddonIconImg = GetAddonIconImg & " style=""vertical-align:middle;display:inline;"" "
                Else
                    GetAddonIconImg = GetAddonIconImg & " style=""display:block"" "
                End If
                If ACInstanceID <> "" Then
                    GetAddonIconImg = GetAddonIconImg & " ACInstanceID=""" & ACInstanceID & """"
                End If
                GetAddonIconImg = GetAddonIconImg & ">"
            Else
                '
                ' Sprite Icon
                '
                GetAddonIconImg = GetIconSprite(IconImgID, IconSpriteColumn, IconFilename, IconWidth, IconHeight, IconAlt, IconTitle, "window.parent.OpenAddonPropertyWindow(this,'" & AdminURL & "');", IconIsInline, ACInstanceID)
                '        GetAddonIconImg = "<img" _
                '            & " border=0" _
                '            & " id=""" & IconImgID & """" _
                '            & " onMouseOver=""this.style.backgroundPosition='" & (-1 * IconSpriteColumn * IconWidth) & "px -" & (2 * IconHeight) & "px'""" _
                '            & " onMouseOut=""this.style.backgroundPosition='" & (-1 * IconSpriteColumn * IconWidth) & "px 0px'""" _
                '            & " onDblClick=""window.parent.OpenAddonPropertyWindow(this,'" & AdminURL & "');""" _
                '            & " alt=""" & IconAlt & """" _
                '            & " title=""" & IconTitle & """" _
                '            & " src=""/ccLib/images/spacer.gif"""
                '        ImgStyle = "background:url(" & IconFilename & ") " & (-1 * IconSpriteColumn * IconWidth) & "px 0px no-repeat;"
                '        ImgStyle = ImgStyle & "width:" & IconWidth & "px;"
                '        ImgStyle = ImgStyle & "height:" & IconHeight & "px;"
                '        If IconIsInline Then
                '            'GetAddonIconImg = GetAddonIconImg & " align=""middle"""
                '            ImgStyle = ImgStyle & "vertical-align:middle;display:inline;"
                '        Else
                '            ImgStyle = ImgStyle & "display:block;"
                '        End If
                '
                '
                '        'Return_IconStyleMenuEntries = Return_IconStyleMenuEntries & vbCrLf & ",["".icon" & AddonID & """,false,"".icon" & AddonID & """,""background:url(" & IconFilename & ") 0px 0px no-repeat;"
                '        'GetAddonIconImg = "<img" _
                '        '    & " id=""AC,AGGREGATEFUNCTION,0," & FieldName & "," & ArgumentList & """" _
                '        '    & " onMouseOver=""this.style.backgroundPosition=\'0px -" & (2 * IconHeight) & "px\'""" _
                '        '    & " onMouseOut=""this.style.backgroundPosition=\'0px 0px\'""" _
                '        '    & " onDblClick=""window.parent.OpenAddonPropertyWindow(this);""" _
                '        '    & " alt=""" & IconAlt & """" _
                '        '    & " title=""" & IconTitle & """" _
                '        '    & " src=""/ccLib/images/spacer.gif"""
                '        If ACInstanceID <> "" Then
                '            GetAddonIconImg = GetAddonIconImg & " ACInstanceID=""" & ACInstanceID & """"
                '        End If
                '        GetAddonIconImg = GetAddonIconImg & " style=""" & ImgStyle & """>"
                '        'Return_IconStyleMenuEntries = Return_IconStyleMenuEntries & """]"
            End If
        End Function
        Public Shared Function GetIconSprite(TagID As String, SpriteColumn As Long, IconSrc As String, IconWidth As Long, IconHeight As Long, IconAlt As String, IconTitle As String, onDblClick As String, IconIsInline As Boolean, ACInstanceID As String) As String
            '
            Dim ImgStyle As String
            '
            GetIconSprite = "<img" _
            & " border=0" _
            & " id=""" & TagID & """" _
            & " onMouseOver=""this.style.backgroundPosition='" & (-1 * SpriteColumn * IconWidth) & "px -" & (2 * IconHeight) & "px';""" _
            & " onMouseOut=""this.style.backgroundPosition='" & (-1 * SpriteColumn * IconWidth) & "px 0px'""" _
            & " onDblClick=""" & onDblClick & """" _
            & " alt=""" & IconAlt & """" _
            & " title=""" & IconTitle & """" _
            & " src=""/ccLib/images/spacer.gif"""
            ImgStyle = "background:url(" & IconSrc & ") " & (-1 * SpriteColumn * IconWidth) & "px 0px no-repeat;"
            ImgStyle = ImgStyle & "width:" & IconWidth & "px;"
            ImgStyle = ImgStyle & "height:" & IconHeight & "px;"
            If IconIsInline Then
                ImgStyle = ImgStyle & "vertical-align:middle;display:inline;"
            Else
                ImgStyle = ImgStyle & "display:block;"
            End If
            If ACInstanceID <> "" Then
                GetIconSprite = GetIconSprite & " ACInstanceID=""" & ACInstanceID & """"
            End If
            GetIconSprite = GetIconSprite & " style=""" & ImgStyle & """>"
        End Function
        '
        ' -- getAttribute
        Friend Shared Function getAttribute(cp As CPBaseClass, node As XmlNode, attrName As String) As String
            Dim result As String = ""
            Try
                Dim attr As XmlAttribute = node.Attributes(attrName)
                If (attr IsNot Nothing) Then
                    result = attr.Value
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
        '
        ' -- build an attribute
        Friend Shared Function createAttribute(doc As XmlDocument, attrName As String, attrValue As String) As XmlAttribute
            Dim returnAttr As XmlAttribute = doc.CreateAttribute(attrName)
            returnAttr.Value = attrValue
            Return returnAttr
        End Function
    End Class

End Namespace
