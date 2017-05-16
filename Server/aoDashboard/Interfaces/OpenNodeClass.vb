Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Contensive.Addons.aoDashboard
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
            Dim returnHtml As String = ""
            Try
                Dim DefaultWrapperGUID As String
                Dim Content As String
                Dim Config As String
                Dim objXML As New XmlDocument
                Dim Node As xmlNode
                'Dim common As New genericController
                Dim NodePtr As Integer
                Dim IconZIndex As Integer
                Dim RequiredJS As String
                Dim Stream As String
                Dim ParentNode As xmlNode
                Dim NodeCount As Integer
                Dim Counter As Integer
                Dim AddonGuid As String
                Dim ContentGuid As String
                Dim ContentName As String
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
                Dim DefaultConfigfilename As String
                Dim UserConfigFilename As String
                Dim ItemID As String
                Dim CS As String
                ''Dim objFSO As Object
                Dim wrapperDict As Dictionary(Of String, Models.wrappersModel) = Models.wrappersModel.getObjectGuidDictionary(CP)
                '
                objXML = Controllers.genericController.LoadConfig(CP)
                If objXML.hasChildNodes Then
                    '
                    ' Find defaultwrapper
                    '

                    For Each Node In objXML.documentElement.ChildNodes
                        If LCase(Node.Name) = "defaultwrapper" Then
                            DefaultWrapperGUID = Node.GetAttribute("guid")
                            Dim wrapper As Models.wrappersModel = wrapperDict(DefaultWrapperGUID)
                            If (wrapper IsNot Nothing) Then
                                WrapperID = wrapper.id
                            End If
                        End If
                    Next

                    NodePtr = CP.Doc.GetInteger("ptr")
                    Node = objXML.DocumentElement.ChildNodes(NodePtr)
                    If Not (Node Is Nothing) Then
                        If Node.Name = "node" Then
                            Call Node.SetAttribute("state", "open")
                            Config = objXML.xml
                            Call Controllers.genericController.SaveConfig(CP, objXML)
                            AddonGuid = Controllers.genericController.GetXMLAttribute(Node, "addonGUID")
                            ContentGuid = Controllers.genericController.GetXMLAttribute(Node, "contentGUID")
                            ContentName = Controllers.genericController.GetXMLAttribute(Node, "contentName")
                            SettingGUID = Controllers.genericController.GetXMLAttribute(Node, "settingGUID")
                            Title = Controllers.genericController.GetXMLAttribute(Node, "title")
                            PosX = cp.utils.encodeInteger(Controllers.genericController.GetXMLAttribute(Node, "x"))
                            PosY = cp.utils.encodeInteger(Controllers.genericController.GetXMLAttribute(Node, "y"))
                            State = Controllers.genericController.GetXMLAttribute(Node, "state")
                            SizeX = cp.utils.encodeInteger(Controllers.genericController.GetXMLAttribute(Node, "sizex"))
                            SizeY = cp.utils.encodeInteger(Controllers.genericController.GetXMLAttribute(Node, "sizey"))
                            Options = Controllers.genericController.GetXMLAttribute(Node, "optionstring")
                            Content = Controllers.genericController.GetDodad(CP, AddonGuid, ContentGuid, ContentName, Title, PosX, PosY, State, SizeX, SizeY, Options, WrapperID, NodePtr, RequiredJS, IconZIndex)
                            returnHtml = "" _
                    & Content _
                    & vbCrLf & vbTab & "<script type=""text/javascript"">" _
                    & (RequiredJS) _
                    & vbCrLf & vbTab & "</script>"
                        End If
                    End If
                End If
            Catch ex As Exception
                CP.Site.ErrorReport(ex)
            End Try
            Return returnHtml
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
