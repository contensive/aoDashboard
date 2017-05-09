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
    Public Class DelNodeClass
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
    Set Main = MainObject
    Set CSV = CSVObject
    
    Dim Stream As String
            Dim Config As String
            Dim NodeCount As Long
            Dim Counter As Long
            Dim AddonGuid As String
            Dim ContentGuid As String
            Dim ContentName As String
            Dim SettingGUID As String
            Dim Title As String
            Dim PosX As String
            Dim PosY As String
            Dim State As String
            Dim SizeX As String
            Dim SizeY As String
            Dim Options As String
            Dim AttrCount As Long
            Dim WrapperID As Long
            Dim DefaultConfigfilename As String
            Dim UserConfigFilename As String
            Dim ItemID As String
            Dim Copy As String
            Dim NodeAttribute As IXMLDOMAttribute
            'Dim ParentNode As IXMLDOMNode
            Dim objXML As New MSXML2.DOMDocument60
            'Dim objFSO As New kmaFileSystem3.FileSystemClass
            Dim Node As IXMLDOMElement
            Dim common As New commonClass
            Dim objFSO As Object
    'Dim objFSO As New kmaFileSystem3.FileSystemClass
    Set objFSO = CreateObject("kmaFileSystem3.FileSystemClass")
    '
    Dim NodePtr As Long
    '
    Set objXML = common.LoadConfig(Main)
    'DefaultConfigfilename = Main.PhysicalFilePath & "upload\dashboard\dashconfig.xml"
    'UserConfigFilename = Main.PhysicalFilePath & "upload\dashboard\dashconfig." & Main.memberID & ".xml"
    'Config = objFSO.ReadFile(UserConfigFilename)
    'If Config = "" Then
    '    Config = objFSO.ReadFile(DefaultConfigfilename)
    '    Call objFSO.SaveFile(UserConfigFilename, Config)
    'End If
    'objXML.loadXML (Config)
    WrapperID = 0
            If objXML.hasChildNodes Then
                NodePtr = Main.GetStreamInteger("ptr")
        Set Node = objXML.documentElement.childNodes(NodePtr)
        If Not (Node Is Nothing) Then
                    If Node.nodeName = "node" Then
                        Call Node.setAttribute("deleted", "yes")
                        'Set ParentNode = Node.ParentNode
                        'Call ParentNode.removeChild(Node)
                        Call common.SaveConfig(Main, objXML)
                        'Config = objXML.xml
                        'Call objFSO.SaveFile(UserConfigFilename, Config)
                    End If
                End If
            End If

            Exit Function
ErrorTrap:
            Call HandleError("DashDragStopClass", "Init", Err.Number, Err.Source, Err.Description, True, False)
        End Function
        ''
        ''
        ''
        'Private Function GetXMLAttribute(Node As IXMLDOMNode, Name As String) As String
        '    On Error GoTo ErrorTrap
        '
        '    Dim NodeAttribute As IXMLDOMAttribute
        '    Dim ResultNode As IXMLDOMNode
        '    Dim UcaseName As String
        '    Dim Found As Boolean
        '
        '    Found = False
        '    If Not (Node.Attributes Is Nothing) Then
        '    Set ResultNode = Node.Attributes.getNamedItem(Name)
        '    If (ResultNode Is Nothing) Then
        '        UcaseName = UCase(Name)
        '        For Each NodeAttribute In Node.Attributes
        '            If UCase(NodeAttribute.nodeName) = UcaseName Then
        '                GetXMLAttribute = NodeAttribute.nodeValue
        '                Found = True
        '                Exit For
        '                End If
        '            Next
        '        If Not Found Then
        '            GetXMLAttribute = ""
        '        End If
        '    Else
        '        GetXMLAttribute = ResultNode.nodeValue
        '        Found = True
        '    End If
        '    End If
        '    Exit Function
        '
        'ErrorTrap:
        '    Call HandleError("DashDragStopClass", "GetXMLAttribute", Err.Number, Err.Source, Err.Description, True, True)
        '    Resume Next
        '    End Function
        '
        '
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
