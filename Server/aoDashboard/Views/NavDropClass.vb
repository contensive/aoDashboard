Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.Addons.Dashboard.Controllers
Imports Contensive.BaseClasses

Namespace Views
    Public Class NavDropClass
        Inherits AddonBaseClass
        ''' <summary>
        ''' remote method called when new node is added by dragging an admin navigator
        ''' </summary>
        ''' <param name="CP"></param>
        ''' <returns></returns>
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim result As String = ""
            Try
                Dim request As New Models.RequestModel(CP)
                Dim config As Models.ConfigModel = Models.ConfigModel.create(CP, CP.User.Id)
                Dim wrapper As Models.WrapperModel = Models.WrapperModel.create(CP, config.defaultWrapper.guid)
                Select Case request.id.ToLower().Substring(0, 1)
                    Case "a"
                        '
                        ' -- An Addon was dragged onto the desktop
                        Dim requestAddonId As Integer = CP.Utils.EncodeInteger(request.id.Substring(1))
                        If (requestAddonId > 0) Then
                            Dim addon As Models.AddonModel = Models.AddonModel.create(CP, requestAddonId)
                            If (addon IsNot Nothing) Then
                                Dim AddonName As String = addon.name
                                Dim IconFileName As String = addon.IconFilename
                                Dim IconWidth As Integer
                                Dim IconHeight As Integer
                                Dim IconSprites As Integer
                                If (String.IsNullOrWhiteSpace(IconFileName)) Then
                                    '
                                    ' -- Default Icon
                                    IconFileName = "/dashboard/addon.png"
                                    IconWidth = 57
                                    IconHeight = 59
                                    IconSprites = 4
                                Else
                                    '
                                    ' -- Custom Icon
                                    IconWidth = addon.IconWidth
                                    IconHeight = addon.IconHeight
                                    IconSprites = addon.IconSprites
                                End If
                                If String.IsNullOrEmpty(addon.ccguid) Then
                                    addon.ccguid = CP.Utils.CreateGuid
                                End If
                                Dim ShortcutHref As String = "?addonid=" & addon.id
                                '
                                ' -- Add this add-on to the config and return the Icon
                                Dim nodeKey As String = "addon" & addon.id & "-" & Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "")
                                Dim newNode As New Models.ConfigModel.ConfigNodeModel() With {
                                    .addonArgList = New List(Of Models.ConfigModel.NameValueModel),
                                    .addonGUID = addon.ccguid,
                                    .contentGUID = "",
                                    .contentName = "",
                                    .link = "",
                                    .key = nodeKey,
                                    .sizex = 200,
                                    .sizey = 300,
                                    .state = Models.ConfigModel.ConfigNodeState.closed,
                                    .title = addon.name,
                                    .wrapperId = If(wrapper Is Nothing, 0, wrapper.id),
                                    .x = request.x,
                                    .y = request.y,
                                    .z = 9999
                                }
                                config.nodeList.Add(nodeKey, newNode)
                                config.save(CP)
                                result = Models.NodeModel.getNodeHtml(CP, newNode)
                            End If
                        End If
                    Case "c"
                        '
                        ' -- A content link was dragged onto the desktop
                        Dim contentId As Integer = CP.Utils.EncodeInteger(request.id.Substring(1))
                        If contentId > 0 Then
                            Dim content As Models.ContentModel = Models.ContentModel.create(CP, contentId)
                            If (content IsNot Nothing) Then
                                If (String.IsNullOrEmpty(content.IconLink)) Then
                                    '
                                    ' -- Default Icon
                                    content.IconLink = "/cclib/images/icons/content.png"
                                    content.IconWidth = 57
                                    content.IconHeight = 59
                                    content.IconSprites = 4
                                End If
                                Dim ShortcutHref As String = "?cid=" & content.id
                                '
                                ' Add this add-on to the config and return the Icon
                                Dim nodeKey As String = "content" & content.id & "-" & Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "")
                                Dim newNode As New Models.ConfigModel.ConfigNodeModel() With {
                                    .addonArgList = New List(Of Models.ConfigModel.NameValueModel),
                                    .addonGUID = "",
                                    .contentGUID = content.ccguid,
                                    .contentName = content.name,
                                    .link = "",
                                    .key = nodeKey,
                                    .sizex = 77,
                                    .sizey = 77,
                                    .state = Models.ConfigModel.ConfigNodeState.closed,
                                    .title = content.name,
                                    .wrapperId = If(wrapper Is Nothing, 0, wrapper.id),
                                    .x = request.x,
                                    .y = request.y,
                                    .z = 9999
                                }
                                config.nodeList.Add(nodeKey, newNode)
                                config.save(CP)
                                result = Models.NodeModel.getNodeHtml(CP, newNode)
                            End If
                        End If
                    Case "n"
                        '
                        ' -- A navigator entry not realted to content or addon
                        Dim navigatorId As Integer = CP.Utils.EncodeInteger(request.id.Substring(1))
                        If navigatorId > 0 Then
                            Dim navigatorEntry As Models.NavigatorEntryModel = Models.NavigatorEntryModel.create(CP, navigatorId)
                            If (navigatorEntry IsNot Nothing) Then
                                '
                                ' Add this add-on to the config and return the Icon
                                Dim nodeKey As String = "nav" & navigatorEntry.id & "-" & Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "")
                                Dim newNode As New Models.ConfigModel.ConfigNodeModel() With {
                                    .addonArgList = New List(Of Models.ConfigModel.NameValueModel),
                                    .addonGUID = "",
                                    .contentGUID = "",
                                    .contentName = "",
                                    .link = navigatorEntry.linkpage,
                                    .key = nodeKey,
                                    .sizex = 77,
                                    .sizey = 77,
                                    .state = Models.ConfigModel.ConfigNodeState.closed,
                                    .title = navigatorEntry.name,
                                    .wrapperId = If(wrapper Is Nothing, 0, wrapper.id),
                                    .x = request.x,
                                    .y = request.y,
                                    .z = 9999
                                }
                                config.nodeList.Add(nodeKey, newNode)
                                config.save(CP)
                                result = Models.NodeModel.getNodeHtml(CP, newNode)
                            End If
                        End If
                    Case Else
                        result = "<!-- no object found for this id -->"
                End Select
            Catch ex As Exception
                CP.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
    End Class
End Namespace
