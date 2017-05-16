Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses

Namespace Contensive.Addons.aoDashboard
    '
    ' Sample Vb2005 addon
    '
    Public Class NavDropClass
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
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String
            Try
                returnHtml = "Visual Studio 2005 Contensive Addon - OK response"
            Catch ex As Exception
                errorReport(CP, ex, "execute")
                returnHtml = "Visual Studio 2005 Contensive Addon - Error response"
            End Try
            Return returnHtml
        End Function
        '
        '=====================================================================================
        ' legacy code
        '=====================================================================================
        '
        Private cp as cpbaseclass
        Private CSV As Object
        '
        '
        '
        Public Function Execute(CSVObject As Object, cpObject As Object, optionString As String, FilterInput As String) As String
            On Error GoTo ErrorTrap
            '
            Dim IconZIndex As Integer
            Dim ItemHtmlID As String
            Dim GuidGenerator As New GuidGenerator
            'Dim Name As String
            Dim ShortcutHref As String
            Dim e As xmlElement
            Dim RequiredJS As String
            'Dim common As New genericController
            Dim UserConfigFilename As String
            Dim CS As Integer
            Dim AddonID As Integer
            '
            '

            Dim Stream As String
            Dim objXML As New XmlDocument
            'Dim objFSO As New kmaFileSystem3.FileSystemClass

            Dim Node As xmlNode

            Dim Config As String
            Dim NodeCount As Integer
            Dim Counter As Integer

            Dim AddonName As String
            Dim AddonGuid As String
            Dim ContentGuid As String
            Dim ContentName As String
            Dim SettingGUID As String
            Dim Title As String
            Dim SrcX As Integer
            Dim SrcY As Integer
            Dim State As String
            Dim SizeX As Integer
            Dim SizeY As Integer
            Dim Options As String
            Dim AttrCount As Integer
            Dim WrapperID As Integer
            Dim DefaultConfigfilename As String
            Dim ItemID As String
            Dim NodeAttribute As xmlattribute
            Dim Copy As String
            Dim NodePtr As Integer
            Dim SrcID As String
            Dim IconFileName As String
            Dim IconWidth As Integer
            Dim IconHeight As Integer
            Dim IconSprites As Integer
            '
            SrcID = cp.GetStreamText("id")
            SrcX = cp.DOC.GETINTEGER("x")
            SrcY = cp.DOC.GETINTEGER("y")
            '
            'Dim objFSO As Object
            objFSO = CreateObject("kmaFileSystem3.FileSystemClass")
            '
            Select Case LCase(Left(SrcID, 1))
                Case "a"
                    '
                    ' An Addon was dragged onto the desktop
                    '
                    AddonID = cp.utils.encodeInteger(Mid(SrcID, 2))
                    If AddonID = 0 Then
                        '
                        ' Bad ID - ignore call and send 'kill' back
                        '
                    Else
                        '
                        '
                        '
                        CS = cp.OpenCSContentRecord("Add-ons", AddonID)
                        If Not cp.IsCSOK(CS) Then
                            '
                            ' Bad ID - ignore call and send 'kill' back
                            '
                        Else
                            AddonName = cp.GetCSText(CS, "name")
                            IconFileName = cp.GetCSText(CS, "IconFilename")
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
                                IconWidth = cp.GetCSInteger(CS, "IconWidth")
                                IconHeight = cp.GetCSInteger(CS, "IconHeight")
                                IconSprites = cp.GetCSInteger(CS, "IconSprites")
                            End If
                            AddonGuid = cp.GetCSText(CS, "ccguid")
                            If AddonGuid = "" Then
                                AddonGuid = GuidGenerator.CreateGUID("")
                                Call cp.SetCS(CS, "ccguid", AddonGuid)
                            End If
                            ShortcutHref = cp.ServerPage & "?addonid=" & AddonID
                            '
                            ' Add this add-on to the config and return the Icon
                            '
                            objXML = Controllers.genericController.LoadConfig(cp)
                            If objXML.hasChildNodes Then
                                '
                                ' Find defaultwrapper
                                '
                                Dim DefaultWrapperGUID As String
                                For Each Node In objXML.documentElement.childNodes
                                    If LCase(Node.name) = "defaultwrapper" Then
                                        DefaultWrapperGUID = Node.getAttribute("guid")
                                        CS = cp.OpenCSContent("wrappers", "ccguid=" & KmaEncodeSQLText(DefaultWrapperGUID))
                                        If cp.IsCSOK(CS) Then
                                            WrapperID = cp.GetCSInteger(CS, "id")
                                        End If
                                        Call cp.CloseCS(CS)
                                        Exit For
                                    End If
                                Next
                                '
                                ' Create the new dodad node
                                '
                                e = objXML.documentElement
                                NodePtr = e.childNodes.length
                                IconZIndex = NodePtr
                                ItemHtmlID = "dashnode" & NodePtr
                                Node = objXML.createElement("node")
                                Call Node.setAttribute("addonGUID", AddonGuid)
                                Call Node.setAttribute("title", AddonName)
                                Call Node.setAttribute("x", Int(SrcX))
                                Call Node.setAttribute("y", Int(SrcY))
                                Call Node.setAttribute("state", "closed")
                                Call Node.setAttribute("sizex", 200)
                                Call Node.setAttribute("sizey", 300)
                                Call Node.setAttribute("optionstring", "")
                                Call e.appendChild(Node)
                                Call Controllers.genericController.SaveConfig(cp, objXML)
                                ContentGuid = ""
                                ContentName = ""
                                Execute = Controllers.genericController.GetDodad(cp, AddonGuid, ContentGuid, ContentName, AddonName, SrcX, SrcY, "closed", 77, 77, "", WrapperID, NodePtr, RequiredJS, IconZIndex)
                            End If
                        End If
                        Call cp.CloseCS(CS)
                    End If
                Case "c"
                    '
                    ' A content link was dragged onto the desktop
                    '
                    Dim ContentID As Integer
                    ContentID = cp.utils.encodeInteger(Mid(SrcID, 2))
                    If ContentID = 0 Then
                        '
                        ' Bad ID - ignore call and send 'kill' back
                        '
                    Else
                        '
                        '
                        '
                        CS = cp.OpenCSContentRecord("Content", ContentID)
                        If Not cp.IsCSOK(CS) Then
                            '
                            ' Bad ID - ignore call and send 'kill' back
                            '
                        Else
                            'IconFileName = cp.GetCSText(CS, "IconFilename")
                            If IconFileName = "" Then
                                '
                                ' Default Icon
                                '
                                IconFileName = "/cclib/images/icons/content.png"
                                IconWidth = 57
                                IconHeight = 59
                                IconSprites = 4
                            Else
                                '
                                ' Custom Icon
                                '
                                IconWidth = cp.GetCSInteger(CS, "IconWidth")
                                IconHeight = cp.GetCSInteger(CS, "IconHeight")
                                IconSprites = cp.GetCSInteger(CS, "IconSprites")
                            End If
                            ContentGuid = cp.GetCSText(CS, "ccguid")
                            SizeX = 77
                            SizeY = 77
                            ShortcutHref = cp.ServerPage & "?cid=" & ContentID
                            ContentName = cp.GetCSText(CS, "name")
                            '
                            ' Add this add-on to the config and return the Icon
                            '
                            objXML = Controllers.genericController.LoadConfig(cp)
                            If objXML.hasChildNodes Then
                                e = objXML.documentElement
                                NodePtr = e.childNodes.length
                                IconZIndex = NodePtr
                                ItemHtmlID = "dashnode" & NodePtr
                                Node = objXML.createElement("node")
                                Call Node.setAttribute("contentName", ContentName)
                                Call Node.setAttribute("contentGUID", ContentGuid)
                                Call Node.setAttribute("title", ContentName)
                                Call Node.setAttribute("x", SrcX)
                                Call Node.setAttribute("y", SrcY)
                                Call Node.setAttribute("state", "closed")
                                Call Node.setAttribute("sizex", SizeX)
                                Call Node.setAttribute("sizey", SizeY)
                                Call Node.setAttribute("optionstring", "")
                                Call e.appendChild(Node)
                                Call Controllers.genericController.SaveConfig(cp, objXML)
                                Execute = Controllers.genericController.GetDodad(cp, 0, "", ContentName, ContentName, SrcX, SrcY, "closed", SizeX, SizeY, "", 0, NodePtr, RequiredJS, IconZIndex)
                                'Execute = Controllers.genericController.GetDodadContent(cp, 0, "", "", "closed", IconSprites, Name, IconFileName, IconWidth, IconHeight, cp.ServerFilePath, WrapperID, SizeX, SizeY, ShortcutHref)
                            End If
                        End If
                        Call cp.CloseCS(CS)
                    End If
                Case Else
                    Execute = "<!-- no object found for this id -->"
            End Select
            '
            ' Add in javascript
            '
            If RequiredJS <> "" Then
                Execute = Execute _
            & "<script type=""text/javascript"">" _
            & (RequiredJS) _
            & "</script>"
            End If
            Exit Function
ErrorTrap:
            Call HandleError("DashDragStopClass", "Init", Err.Number, Err.Source, Err.Description, True, False)
        End Function
        '
        '
        '
        Private Function GetXMLAttribute(Node As xmlnode, Name As String) As String
            On Error GoTo ErrorTrap

            Dim NodeAttribute As xmlattribute
            Dim ResultNode As xmlnode
            Dim UcaseName As String
            Dim Found As Boolean

            Found = False
            If Not (Node.Attributes Is Nothing) Then
                ResultNode = Node.Attributes.getNamedItem(Name)
                If (ResultNode Is Nothing) Then
                    UcaseName = UCase(Name)
                    For Each NodeAttribute In Node.Attributes
                        If UCase(NodeAttribute.name) = UcaseName Then
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
            Call HandleError("DashDragStopClass", "GetXMLAttribute", Err.Number, Err.Source, Err.Description, True, True)
            Resume Next
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
