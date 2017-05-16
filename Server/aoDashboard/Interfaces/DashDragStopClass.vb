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
    Public Class DashDragStopClass
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
            '
            '

            Dim Stream As String
            Dim objXML As New XmlDocument
            'Dim objFSO As New kmaFileSystem3.FileSystemClass

            Dim Node As xmlNode

            Dim Config As String
            Dim NodeCount As Integer
            Dim Counter As Integer

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
            Dim AttrCount As Integer
            Dim WrapperID As Integer
            Dim DefaultConfigfilename As String
            Dim UserConfigFilename As String
            Dim ItemID As String
            Dim NodeAttribute As xmlattribute
            Dim Copy As String
            'Dim common As New genericController
            '
            Dim NodePtr As Integer
            '
            ' store path to config file in a site property so defaults can be customized (like Realestate Sites, etc)
            '
            'Dim objFSO As Object
            objFSO = CreateObject("kmaFileSystem3.FileSystemClass")
            '
            objXML = Controllers.genericController.LoadConfig(cp)
            'DefaultConfigfilename = "upload\dashboard\dashconfig.xml"
            'DefaultConfigfilename = cp.site.gettext("Dashboard Default Config Content Filename", DefaultConfigfilename)
            'DefaultConfigfilename = cp.Site.physicalFilePath & DefaultConfigfilename
            'UserConfigFilename = cp.Site.physicalFilePath & "upload\dashboard\dashconfig." & cp.User.id & ".xml"
            'Config = cp.File.Read(UserConfigFilename)
            'If Config = "" Then
            '    Config = cp.File.Read(DefaultConfigfilename)
            '    Call cp.File.Save(UserConfigFilename, Config)
            'End If
            'objXML.loadXML (Config)
            WrapperID = 0
            If objXML.hasChildNodes Then
                NodePtr = cp.DOC.GETINTEGER("ptr")
                Node = objXML.documentElement.childNodes(NodePtr)
                If Not (Node Is Nothing) Then
                    If Node.name = "node" Then
                        Copy = cp.GetStreamText("x")
                        Copy = Replace(Copy, "px", "")
                        If IsNumeric(Copy) Then
                            Call Node.setAttribute("x", Copy)
                        End If
                        Copy = cp.GetStreamText("y")
                        Copy = Replace(Copy, "px", "")
                        If IsNumeric(Copy) Then
                            Call Node.setAttribute("y", Copy)
                        End If
                        Call Controllers.genericController.SaveConfig(cp, objXML)
                        'Config = objXML.xml
                        'Call cp.File.Save(UserConfigFilename, Config)
                    End If
                End If
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
