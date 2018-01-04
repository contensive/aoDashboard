
Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses
Imports System.Xml
Imports Contensive.Addons.aoDashboard.Controllers

Namespace Interfaces
    Public Class dashboardClass
        Inherits AddonBaseClass
        '
        ' -- 
        Public GlobalJS As String = ""
        '
        Public Overrides Function Execute(ByVal cp As CPBaseClass) As Object
            Dim result As String = ""
            Dim hint As String = "1"
            Try
                Dim IconZIndex As Integer
                Dim RequiredJS As String
                Dim node As XmlNode
                Dim ParentNode As XmlNode
                Dim AddonGuid As String
                Dim ContentGuid As String = ""
                Dim ContentName As String = ""
                Dim Title As String
                Dim PosX As Integer
                Dim PosY As Integer
                Dim State As String
                Dim SizeX As Integer
                Dim SizeY As Integer
                Dim Options As String
                Dim WrapperID As Integer
                Dim DefaultWrapperGUID As String
                Dim NodePtr As Integer
                Dim ConfigChanged As Boolean
                '
                WrapperID = 0
                Dim Dashboard As String = ""
                Dim config As Xml.XmlDocument = Controllers.genericController.LoadConfig(cp)
                If config.HasChildNodes Then
                    '
                    ' review values, remove deleted nodes and get settings
                    '
                    hint &= ",2"
                    ConfigChanged = False
                    For Each node In config.DocumentElement.ChildNodes
                        Select Case LCase(node.Name)
                            Case "defaultwrapper"
                                hint &= ",3"
                                DefaultWrapperGUID = genericController.getAttribute(cp, node, "guid")
                                Dim wrapper As Models.wrapperModel = Models.wrapperModel.create(cp, DefaultWrapperGUID)
                                If (wrapper IsNot Nothing) Then
                                    WrapperID = wrapper.id
                                End If
                            Case "node"
                                hint &= ",4a"
                                AddonGuid = genericController.getAttribute(cp, node, "addonGUID")
                                If AddonGuid = "" Then
                                    ContentGuid = genericController.getAttribute(cp, node, "contentGUID")
                                    If ContentGuid = "" Then
                                        ContentName = genericController.getAttribute(cp, node, "contentName")
                                    End If
                                End If
                                SizeY = cp.Utils.EncodeInteger(genericController.getAttribute(cp, node, "sizey"))
                                If ((AddonGuid = "") And (ContentGuid = "") And (ContentName = "")) Or cp.Utils.EncodeBoolean(genericController.getAttribute(cp, node, "deleted")) Then
                                    hint &= ",4b"
                                    '
                                    ' delete any nodes marked as delete
                                    ' this is actually a problem if the user has mulitple windows open,
                                    ' and he deletes an entry from one window, refreshes, then moves
                                    ' or deletes anything from the other window
                                    ' - need to get rid of the 'ptr' scheme and go to an ID
                                    '
                                    ParentNode = node.ParentNode
                                    Call ParentNode.RemoveChild(node)
                                    ConfigChanged = True
                                Else
                                    hint &= ",4c"
                                    If cp.Utils.EncodeInteger(genericController.getAttribute(cp, node, "x")) < 0 Then
                                        hint &= ",4d"
                                        node.Attributes("x").Value = "0"
                                        ConfigChanged = True
                                    End If
                                    If cp.Utils.EncodeInteger(genericController.getAttribute(cp, node, "y")) < 0 Then
                                        hint &= ",4e"
                                        node.Attributes("y").Value = "0"
                                        ConfigChanged = True
                                    End If
                                End If
                        End Select
                    Next
                    If ConfigChanged Then
                        hint &= ",5"
                        Call Controllers.genericController.SaveConfig(cp, config)
                    End If
                    '
                    ' draw the nodes that are left
                    '
                    NodePtr = 0
                    For Each node In config.DocumentElement.ChildNodes
                        Select Case LCase(node.Name)
                            Case "node"
                                hint &= ",6"
                                RequiredJS = ""
                                AddonGuid = Controllers.genericController.getAttribute(cp, node, "addonGUID")
                                ContentGuid = Controllers.genericController.getAttribute(cp, node, "contentGUID")
                                ContentName = Controllers.genericController.getAttribute(cp, node, "contentName")
                                Title = Controllers.genericController.getAttribute(cp, node, "title")
                                PosX = cp.Utils.EncodeInteger(Controllers.genericController.getAttribute(cp, node, "x"))
                                PosY = cp.Utils.EncodeInteger(Controllers.genericController.getAttribute(cp, node, "y"))
                                State = Controllers.genericController.getAttribute(cp, node, "state")
                                SizeX = cp.Utils.EncodeInteger(Controllers.genericController.getAttribute(cp, node, "sizex"))
                                SizeY = cp.Utils.EncodeInteger(Controllers.genericController.getAttribute(cp, node, "sizey"))
                                Options = Controllers.genericController.getAttribute(cp, node, "optionstring")
                                Dashboard &= Controllers.genericController.GetDodad(cp, AddonGuid, ContentGuid, ContentName, Title, PosX, PosY, State, SizeX, SizeY, Options, WrapperID, NodePtr, RequiredJS, IconZIndex)
                                GlobalJS = GlobalJS & RequiredJS
                                IconZIndex = IconZIndex + 1
                        End Select
                        NodePtr = NodePtr + 1
                    Next
                End If
                hint &= ",7"
                '
                ' Add JQuery UI dropable to desktop
                '
                GlobalJS &= "" _
                    & CR & "//" _
                    & CR & "// -- add droppable to desktop" _
                    & CR & "var iconZIndexTop=" & IconZIndex & ";" _
                    & CR & "var nodeCnt=" & NodePtr & ";jQuery(""#desktop"").droppable({tolerance: 'fit'});"
                '
                ' Assemble final page
                '
                hint &= ",8"
                cp.Doc.AddBodyEnd("<script type=""text/javascript"">" & GlobalJS & "</script>")
                result = "" _
                    & "<div id=""dashBoardWrapper"" class=""dashBoardWrapper"">" _
                    & Dashboard _
                    & "</div>" _
                    & ""
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "dashboardClass, hint [" & hint & "]")
            End Try
            Return result
        End Function
    End Class
End Namespace
