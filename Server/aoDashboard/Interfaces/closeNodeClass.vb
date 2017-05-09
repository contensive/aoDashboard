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
    Public Class closeNodeClass
        Inherits AddonBaseClass
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
        Public Overrides Function Execute(cp As CPBaseClass) As Object
            'Public Overrides Function Execute(CSVObject As Object, MainObject As Object, optionString As String, FilterInput As String) As String
            On Error GoTo ErrorTrap
            '
            Set Main = MainObject
            Set CSV = CSVObject
            Dim IconZIndex As Long
            Dim Config As String
            Dim objXML As New MSXML2.DOMDocument60
            Dim Node As IXMLDOMElement
            Dim common As New CommonClass
            Dim NodePtr As Long
            Dim RequiredJS As String
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
            Dim NodeAttribute As IXMLDOMAttribute
            Dim Copy As String
            '
            NodePtr = Main.GetStreamInteger("ptr")
    '
    ' open the config and mark the node in the config as closed
    '
    Set objXML = common.LoadConfig(Main)
    If objXML.hasChildNodes Then
        Set Node = objXML.documentElement.childNodes(NodePtr)
        If Not (Node Is Nothing) Then
                    If Node.nodeName = "node" Then
                        Call Node.setAttribute("state", "closed")
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
                        Execute = common.GetDodad(Main, AddonGuid, ContentGuid, ContentName, Title, PosX, PosY, State, SizeX, SizeY, Options, WrapperID, NodePtr, RequiredJS, IconZIndex)
                        '
                        ' Add in javascript
                        '
                        If RequiredJS <> "" Then
                            Execute = Execute _
                        & CR & "<script type=""text/javascript"">" _
                        & KmaIndent(RequiredJS) _
                        & CR & "</script>"
                        End If
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
