Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.Addons.aoDashboard.Controllers
Imports Contensive.BaseClasses

Namespace Interfaces
    Public Class NavDropClass
        Inherits AddonBaseClass
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                Dim SrcID As String = CP.Doc.GetText("id")
                Dim SrcX As Integer = CP.Doc.GetInteger("x")
                Dim SrcY As Integer = CP.Doc.GetInteger("y")
                Dim RequiredJS As String = ""
                '
                Select Case LCase(Left(SrcID, 1))
                    Case "a"
                        '
                        ' -- An Addon was dragged onto the desktop
                        Dim AddonID As Integer = CP.Utils.EncodeInteger(Mid(SrcID, 2))
                        If AddonID = 0 Then
                            '
                            ' Bad ID - ignore call and send 'kill' back
                            '
                        Else
                            '
                            '
                            '
                            Dim addon As Models.addonModel = Models.addonModel.create(CP, AddonID)
                            If (addon IsNot Nothing) Then
                                '
                                ' -- Bad ID - ignore call and send 'kill' back
                            Else
                                Dim AddonName As String = addon.name
                                Dim IconFileName As String = addon.IconFilename
                                Dim IconWidth As Integer
                                Dim IconHeight As Integer
                                Dim IconSprites As Integer
                                If IconFileName = "" Then
                                    '
                                    ' Default Icon
                                    '
                                    IconFileName = "/cclib/images/icons/addon.png"
                                    IconWidth = 57
                                    IconHeight = 59
                                    IconSprites = 4
                                Else
                                    '
                                    ' Custom Icon
                                    '
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
                                Dim config As XmlDocument = Controllers.genericController.LoadConfig(CP)
                                If config.HasChildNodes Then
                                    '
                                    ' -- get default wrapper id
                                    Dim DefaultWrapperGUID As String
                                    Dim wrapperId As Integer = 0
                                    For Each childNode As XmlNode In config.DocumentElement.ChildNodes
                                        If (childNode.Name.ToLower() = "defaultwrapper") Then
                                            Dim attrGuid As XmlAttribute = childNode.Attributes("guid")
                                            If (attrGuid IsNot Nothing) Then
                                                DefaultWrapperGUID = attrGuid.Value
                                                Dim wrapper As Models.wrapperModel = Models.wrapperModel.create(CP, DefaultWrapperGUID)
                                                If (wrapper IsNot Nothing) Then
                                                    wrapperId = wrapper.id
                                                End If
                                                Exit For
                                            End If
                                        End If
                                    Next
                                    '
                                    ' Create the new dodad node
                                    '
                                    Dim configRootNode As XmlNode = config.DocumentElement
                                    Dim NodePtr As Integer = configRootNode.ChildNodes.Count
                                    Dim IconZIndex As Integer = NodePtr
                                    Dim ItemHtmlID As String = "dashnode" & NodePtr
                                    Dim Node As XmlNode = config.CreateElement("node")
                                    Dim attrAddonGuid As XmlAttribute = config.CreateAttribute("addonGUID")
                                    attrAddonGuid.Value = addon.ccguid
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "addonGUID", addon.ccguid))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "title", addon.name))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "x", SrcX.ToString()))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "y", SrcY.ToString()))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "state", "closed"))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "sizex", "200"))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "sizey", "300"))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "optionstring", ""))
                                    Call configRootNode.AppendChild(Node)
                                    Call Controllers.genericController.SaveConfig(CP, config)
                                    Dim contentGuid As String = ""
                                    Dim contentName As String = ""
                                    returnHtml = Controllers.genericController.GetDodad(CP, addon.ccguid, contentGuid, contentName, AddonName, SrcX, SrcY, "closed", 77, 77, "", wrapperId, NodePtr, RequiredJS, IconZIndex)
                                End If
                            End If
                        End If
                    Case "c"
                        '
                        ' -- A content link was dragged onto the desktop
                        Dim ContentID As Integer = CP.Utils.EncodeInteger(Mid(SrcID, 2))
                        If ContentID = 0 Then
                            '
                            ' Bad ID - ignore call and send 'kill' back
                            '
                        Else
                            Dim content As Models.contentModel = Models.contentModel.create(CP, ContentID)
                            If (content IsNot Nothing) Then
                                If (String.IsNullOrEmpty(content.IconLink)) Then
                                    '
                                    ' -- Default Icon
                                    content.IconLink = "/cclib/images/icons/content.png"
                                    content.IconWidth = 57
                                    content.IconHeight = 59
                                    content.IconSprites = 4
                                End If
                                Dim SizeX As Integer = 77
                                Dim SizeY As Integer = 77
                                Dim ShortcutHref As String = "?cid=" & ContentID
                                '
                                ' Add this add-on to the config and return the Icon
                                Dim config As XmlDocument = Controllers.genericController.LoadConfig(CP)
                                If config.HasChildNodes Then
                                    Dim e As XmlNode = config.DocumentElement
                                    Dim NodePtr As Integer = e.ChildNodes.Count
                                    Dim IconZIndex As Integer = NodePtr
                                    Dim ItemHtmlID As String = "dashnode" & NodePtr
                                    Dim Node As XmlNode = config.CreateElement("node")
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "contentName", content.name))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "contentGUID", content.ccguid))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "title", content.name))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "x", SrcX.ToString()))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "y", SrcY.ToString()))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "state", "closed"))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "sizex", SizeX.ToString()))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "sizey", SizeY.ToString()))
                                    Call Node.Attributes.Append(genericController.createAttribute(config, "optionstring", ""))
                                    Call e.AppendChild(Node)
                                    Call Controllers.genericController.SaveConfig(CP, config)
                                    returnHtml = Controllers.genericController.GetDodad(CP, "0", "", content.name, content.name, SrcX, SrcY, "closed", SizeX, SizeY, "", 0, NodePtr, RequiredJS, IconZIndex)
                                    'returnHtml = Controllers.genericController.GetDodadContent(cp, 0, "", "", "closed", IconSprites, Name, IconFileName, IconWidth, IconHeight, cp.ServerFilePath, WrapperID, SizeX, SizeY, ShortcutHref)
                                End If
                            End If
                        End If
                    Case Else
                        returnHtml = "<!-- no object found for this id -->"
                End Select
                '
                ' Add in javascript
                '
                If RequiredJS <> "" Then
                    returnHtml = returnHtml _
                        & "<script type=""text/javascript"">" _
                        & (RequiredJS) _
                        & "</script>"
                End If
            Catch ex As Exception
                CP.Site.ErrorReport(ex)
            End Try
            Return returnHtml
        End Function
    End Class
End Namespace