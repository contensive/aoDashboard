Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses
Imports System.Xml

Namespace Interfaces
    Public Class dashboardClass
        Inherits AddonBaseClass
        Public Overrides Function Execute(ByVal cp As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                returnHtml = legacyAddon(cp)
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return returnHtml
        End Function
        '
        Private GlobalJS As String
        '
        Private Function legacyAddon(cp As CPBaseClass) As String
            Dim result As String = ""
            Try
                '
                Dim hint As String
                Dim CS As Integer
                Dim IconZIndex As Integer
                Dim RequiredJS As String
                Dim Dashboard As String
                Dim objXML As New Xml.XmlDocument
                'Dim objXML As New XmlDocument
                Dim Node As XmlNode
                Dim ParentNode As XmlNode
                Dim Config As String
                Dim NodeCount As Integer
                Dim Counter As Integer
                Dim AddonGuid As String
                Dim ContentGuid As String = ""
                Dim ContentName As String = ""
                Dim SettingGUID As String
                Dim Title As String
                Dim PosX As Integer
                Dim PosY As Integer
                Dim State As String
                Dim SizeX As Integer
                Dim SizeY As Integer
                Dim Options As String
                Dim AttrCount As Integer
                Dim WrapperID As Integer
                Dim DefaultWrapperGUID As String
                Dim DefaultConfigfilename As String
                Dim UserConfigFilename As String
                Dim ItemID As String
                Dim NodePtr As Integer
                'Dim common As New genericController
                Dim ConfigChanged As Boolean
                '
                WrapperID = 0
                If objXML.HasChildNodes Then
                    '
                    ' review values, remove deleted nodes and get settings
                    '
                    ConfigChanged = False
                    For Each Node In objXML.DocumentElement.ChildNodes
                        Dim element As Xml.XmlElement = Nothing
                        If Node Is GetType(Xml.XmlElement) Then
                            element = DirectCast(Node, Xml.XmlElement)
                            Select Case LCase(element.Name)
                                Case "defaultwrapper"
                                    DefaultWrapperGUID = element.GetAttribute("guid")
                                    Dim wrapper As Models.wrapperModel = Models.wrapperModel.create(cp, DefaultWrapperGUID)
                                    If (wrapper IsNot Nothing) Then
                                        WrapperID = wrapper.id
                                    End If
                                Case "node"
                                    AddonGuid = cp.Utils.EncodeText(element.GetAttribute("addonGUID"))
                                    If AddonGuid = "" Then
                                        ContentGuid = cp.Utils.EncodeText(element.GetAttribute("contentGUID"))
                                        If ContentGuid = "" Then
                                            ContentName = cp.Utils.EncodeText(element.GetAttribute("contentName"))
                                        End If
                                    End If
                                    SizeY = cp.Utils.EncodeInteger(element.GetAttribute("sizey"))
                                    If ((AddonGuid = "") And (ContentGuid = "") And (ContentName = "")) Or cp.Utils.EncodeBoolean(element.GetAttribute("deleted")) Then
                                        '
                                        ' delete any nodes marked as delete
                                        ' this is actually a problem if the user has mulitple windows open,
                                        ' and he deletes an entry from one window, refreshes, then moves
                                        ' or deletes anything from the other window
                                        ' - need to get rid of the 'ptr' scheme and go to an ID
                                        '
                                        ParentNode = element.ParentNode
                                        Call ParentNode.RemoveChild(element)
                                        ConfigChanged = True
                                    Else
                                        If cp.Utils.EncodeInteger(element.GetAttribute("x")) < 0 Then
                                            Call element.SetAttribute("x", "0")
                                            ConfigChanged = True
                                        End If
                                        If cp.Utils.EncodeInteger(element.GetAttribute("y")) < 0 Then
                                            Call element.SetAttribute("y", "0")
                                            ConfigChanged = True
                                        End If
                                    End If
                            End Select
                        End If
                    Next
                    hint = "400"
                    If ConfigChanged Then
                        Call Controllers.genericController.SaveConfig(cp, objXML)
                    End If
                    '
                    ' draw the nodes that are left
                    '
                    NodePtr = 0
                    For Each Node In objXML.DocumentElement.ChildNodes
                        Dim element As Xml.XmlElement = Nothing
                        If Node Is GetType(Xml.XmlElement) Then
                            element = DirectCast(Node, Xml.XmlElement)
                            Select Case LCase(element.Name)
                                Case "node"
                                    RequiredJS = ""
                                    AddonGuid = Controllers.genericController.GetXMLAttribute(element, "addonGUID")
                                    ContentGuid = Controllers.genericController.GetXMLAttribute(element, "contentGUID")
                                    ContentName = Controllers.genericController.GetXMLAttribute(element, "contentName")
                                    Title = Controllers.genericController.GetXMLAttribute(element, "title")
                                    PosX = cp.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(element, "x"))
                                    PosY = cp.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(element, "y"))
                                    State = Controllers.genericController.GetXMLAttribute(element, "state")
                                    SizeX = cp.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(element, "sizex"))
                                    SizeY = cp.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(element, "sizey"))
                                    Options = Controllers.genericController.GetXMLAttribute(element, "optionstring")
                                    Dashboard = Dashboard & Controllers.genericController.GetDodad(cp, AddonGuid, ContentGuid, ContentName, Title, PosX, PosY, State, SizeX, SizeY, Options, WrapperID, NodePtr, RequiredJS, IconZIndex)
                                    GlobalJS = GlobalJS & RequiredJS
                                    IconZIndex = IconZIndex + 1
                            End Select
                        End If
                        NodePtr = NodePtr + 1
                    Next
                End If
                '
                ' Add JQuery UI dropable to desktop
                '
                GlobalJS = GlobalJS _
                    & "var iconZIndexTop=" & IconZIndex & ";" _
                    & "var nodeCnt=" & NodePtr & ";" _
                    & "$(""#desktop"").droppable({" _
                        & "tolerance: 'fit'" _
                    & "});"
                '
                ' Assemble final page
                '
                result = "" _
                    & "<div id=""dashBoardWrapper"" class=""dashBoardWrapper"">" _
                    & ("&nbsp;" & Dashboard) _
                    & "</div>" _
                    & "<script type=""text/javascript"">" _
                    & (GlobalJS) _
                    & "</script>"
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function

    End Class
End Namespace
