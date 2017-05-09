Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses

Namespace Contensive.Addons.aoDashbaord
    '
    ' Sample Vb2005 addon
    '
    Public Class OpenNodeClass
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
        Private Main As Object
        Private CSV As Object
        '
        '
        '
        Public Function Execute(CSVObject As Object, MainObject As Object, optionString As String, FilterInput As String) As String
            On Error GoTo ErrorTrap
            '
            Dim DefaultWrapperGUID As String
            Dim Content As String
    Set Main = MainObject
    Set CSV = CSVObject
    
    Dim Config As String
            Dim objXML As New MSXML2.DOMDocument60
            Dim Node As IXMLDOMElement
            Dim common As New commonClass
            Dim NodePtr As Long
            '
            Dim IconZIndex As Long
            Dim RequiredJS As String
            Dim Stream As String
            'Dim objFSO As New kmaFileSystem3.FileSystemClass
            Dim ParentNode As IXMLDOMElement
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
            Dim DefaultConfigfilename As String
            Dim UserConfigFilename As String
            Dim ItemID As String
            Dim CS As String
            '
            Dim objFSO As Object
    Set objFSO = CreateObject("kmaFileSystem3.FileSystemClass")
    '
    Set objXML = common.LoadConfig(Main)
    If objXML.hasChildNodes Then
                '
                ' Find defaultwrapper
                '
                For Each Node In objXML.documentElement.childNodes
                    If LCase(Node.baseName) = "defaultwrapper" Then
                        DefaultWrapperGUID = Node.getAttribute("guid")
                        CS = Main.OpenCSContent("wrappers", "ccguid=" & KmaEncodeSQLText(DefaultWrapperGUID))
                        If Main.IsCSOK(CS) Then
                            WrapperID = Main.GetCSInteger(CS, "id")
                        End If
                        Call Main.CloseCS(CS)
                        Exit For
                    End If
                Next

                NodePtr = Main.GetStreamInteger("ptr")
        Set Node = objXML.documentElement.childNodes(NodePtr)
        If Not (Node Is Nothing) Then
                    If Node.nodeName = "node" Then
                        Call Node.setAttribute("state", "open")
                        Config = objXML.xml
                        Call common.SaveConfig(Main, objXML)
                        AddonGuid = common.GetXMLAttribute(Node, "addonGUID")
                        ContentGuid = common.GetXMLAttribute(Node, "contentGUID")
                        ContentName = common.GetXMLAttribute(Node, "contentName")
                        SettingGUID = common.GetXMLAttribute(Node, "settingGUID")
                        Title = common.GetXMLAttribute(Node, "title")
                        PosX = kmaEncodeInteger(common.GetXMLAttribute(Node, "x"))
                        PosY = kmaEncodeInteger(common.GetXMLAttribute(Node, "y"))
                        State = common.GetXMLAttribute(Node, "state")
                        SizeX = kmaEncodeInteger(common.GetXMLAttribute(Node, "sizex"))
                        SizeY = kmaEncodeInteger(common.GetXMLAttribute(Node, "sizey"))
                        Options = common.GetXMLAttribute(Node, "optionstring")
                        Content = common.GetDodad(Main, AddonGuid, ContentGuid, ContentName, Title, PosX, PosY, State, SizeX, SizeY, Options, WrapperID, NodePtr, RequiredJS, IconZIndex)
                        Execute = "" _
                    & Content _
                    & vbCrLf & vbTab & "<script type=""text/javascript"">" _
                    & kmaIndent(RequiredJS) _
                    & vbCrLf & vbTab & "</script>"
                    End If
                End If
            End If

            Exit Function
ErrorTrap:
            Call HandleError("DashDragStopClass", "Init", Err.Number, Err.Source, Err.Description, True, False)
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
