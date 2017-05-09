Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses
Imports System.Xml

Namespace Contensive.Addons.aoDashbaord
    '
    ' Sample Vb2005 addon
    '
    Public Class dashboardClass
        Inherits AddonBaseClass
        '
        ' - update references to your installed version of cpBase
        ' - Verify project root name space is empty
        ' - Change the namespace to the collection name
        ' - Change this class name to the addon name
        ' - Create a Contensive Addon record with the namespace apCollectionName.ad
        ' - add reference to CPBase.DLL, typically installed in c:\program files\kma\contensive\
        '
        '=====================================================================================
        ' addon api
        '=====================================================================================
        '
        Public Overrides Function Execute(ByVal cp As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                returnHtml = legacyAddon(cp)
            Catch ex As Exception
                errorReport(cp, ex, "execute")
            End Try
            Return returnHtml
        End Function
        '
        '=====================================================================================
        ' legacy code
        '=====================================================================================
        '
        'Private Main As Object
        'Private CSV As Object
        Private GlobalJS As String
        '
        Private Function legacyAddon(cp As CPBaseClass) As String
            Dim result As String = ""
            Try
                '
                Dim hint As String
                Dim CS As Long
                Dim IconZIndex As Long
                Dim RequiredJS As String
                Dim Dashboard As String
                Dim objXML As New Xml.XmlDocument
                'Dim objXML As New MSXML2.DOMDocument60
                Dim Node As IXMLDOMElement
                Dim ParentNode As IXMLDOMElement
                Dim Config As String
                Dim NodeCount As Long
                Dim Counter As Long
                Dim AddonGuid As String
                Dim ContentGuid As String
                Dim ContentName As String
                Dim SettingGUID As String
                Dim Title As String
                Dim PosX As Long
                Dim PosY As Long
                Dim State As String
                Dim SizeX As Long
                Dim SizeY As Long
                Dim Options As String
                Dim AttrCount As Long
                Dim WrapperID As Long
                Dim DefaultWrapperGUID As String
                Dim DefaultConfigfilename As String
                Dim UserConfigFilename As String
                Dim ItemID As String
                Dim NodePtr As Long
                Dim common As New commonClass
                Dim ConfigChanged As Boolean
                '
                WrapperID = 0
                If objXML.hasChildNodes Then
                    '
                    ' review values, remove deleted nodes and get settings
                    '
                    ConfigChanged = False
                    For Each Node In objXML.documentElement.childNodes
                        Select Case LCase(Node.baseName)
                            Case "defaultwrapper"
                                DefaultWrapperGUID = Node.getAttribute("guid")
                                CS = Main.OpenCSContent("wrappers", "ccguid=" & KmaEncodeSQLText(DefaultWrapperGUID))
                                If Main.IsCSOK(CS) Then
                                    WrapperID = Main.GetCSInteger(CS, "id")
                                End If
                                Call Main.CloseCS(CS)
                            Case "node"
                                AddonGuid = kmaEncodeText(Node.getAttribute("addonGUID"))
                                If AddonGuid = "" Then
                                    ContentGuid = kmaEncodeText(Node.getAttribute("contentGUID"))
                                    If ContentGuid = "" Then
                                        ContentName = kmaEncodeText(Node.getAttribute("contentName"))
                                    End If
                                End If
                                SizeY = kmaEncodeInteger(Node.getAttribute("sizey"))
                                If ((AddonGuid = "") And (ContentGuid = "") And (ContentName = "")) Or kmaEncodeBoolean(Node.getAttribute("deleted")) Then
                            '
                            ' delete any nodes marked as delete
                            ' this is actually a problem if the user has mulitple windows open,
                            ' and he deletes an entry from one window, refreshes, then moves
                            ' or deletes anything from the other window
                            ' - need to get rid of the 'ptr' scheme and go to an ID
                            '
                            Set ParentNode = Node.ParentNode
                            Call ParentNode.removeChild(Node)
                                    ConfigChanged = True
                                Else
                                    If kmaEncodeInteger(Node.getAttribute("x")) < 0 Then
                                        Call Node.setAttribute("x", 0)
                                        ConfigChanged = True
                                    End If
                                    If kmaEncodeInteger(Node.getAttribute("y")) < 0 Then
                                        Call Node.setAttribute("y", 0)
                                        ConfigChanged = True
                                    End If
                                End If
                        End Select
                    Next
                    hint = "400"
                    If ConfigChanged Then
                        Call common.SaveConfig(Main, objXML)
                    End If
                    '
                    ' draw the nodes that are left
                    '
                    NodePtr = 0
                    For Each Node In objXML.documentElement.childNodes
                        Select Case LCase(Node.baseName)
                            Case "node"
                                RequiredJS = ""
                                AddonGuid = common.GetXMLAttribute(Node, "addonGUID")
                                ContentGuid = common.GetXMLAttribute(Node, "contentGUID")
                                ContentName = common.GetXMLAttribute(Node, "contentName")
                                Title = common.GetXMLAttribute(Node, "title")
                                PosX = kmaEncodeInteger(common.GetXMLAttribute(Node, "x"))
                                PosY = kmaEncodeInteger(common.GetXMLAttribute(Node, "y"))
                                State = common.GetXMLAttribute(Node, "state")
                                SizeX = kmaEncodeInteger(common.GetXMLAttribute(Node, "sizex"))
                                SizeY = kmaEncodeInteger(common.GetXMLAttribute(Node, "sizey"))
                                Options = common.GetXMLAttribute(Node, "optionstring")
                                Dashboard = Dashboard & common.GetDodad(Main, AddonGuid, ContentGuid, ContentName, Title, PosX, PosY, State, SizeX, SizeY, Options, WrapperID, NodePtr, RequiredJS, IconZIndex)
                                GlobalJS = GlobalJS & RequiredJS
                                IconZIndex = IconZIndex + 1
                        End Select
                        NodePtr = NodePtr + 1
                    Next
                End If
                '
                ' Add JQuery UI dropable to desktop
                '
                GlobalJS = GlobalJS _
        & cr & "var iconZIndexTop=" & IconZIndex & ";" _
        & cr & "var nodeCnt=" & NodePtr & ";" _
        & cr & "$(""#desktop"").droppable({" _
            & "tolerance: 'fit'" _
        & "});"
                '
                ' Assemble final page
                '
                GetContent = "" _
        & cr & "<div id=""dashBoardWrapper"" class=""dashBoardWrapper"">" _
        & kmaIndent("&nbsp;" & Dashboard) _
        & cr & "</div>" _
        & cr & "<script type=""text/javascript"">" _
        & kmaIndent(GlobalJS) _
        & cr & "</script>"
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function


        '
        '=====================================================================================
        ' common report for this class
        '=====================================================================================
        '
        Private Sub errorReport(ByVal cp As CPBaseClass, ByVal ex As Exception, ByVal method As String)
            Try
                cp.Site.ErrorReport(ex, "Unexpected error in sampleClass." & method)
            Catch exLost As Exception
                '
                ' stop anything thrown from cp errorReport
                '
            End Try
        End Sub
    End Class
End Namespace
