Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Interfaces
    Public Class OpenNodeClass
        Inherits AddonBaseClass
        '
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                Dim DefaultWrapperGUID As String
                Dim Content As String
                'Dim Config As String
                Dim objXML As New XmlDocument
                Dim Node As XmlNode
                'Dim common As New genericController
                Dim NodePtr As Integer
                Dim IconZIndex As Integer
                Dim RequiredJS As String
                Dim Stream As String
                Dim ParentNode As XmlNode
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
                Dim wrapperDict As Dictionary(Of String, Models.wrapperModel) = Models.wrapperModel.getGuidDictionary(CP)
                '
                objXML = Controllers.genericController.LoadConfig(CP)
                If objXML.HasChildNodes Then
                    '
                    ' Find defaultwrapper
                    '

                    For Each Node In objXML.DocumentElement.ChildNodes
                        If LCase(Node.Name) = "defaultwrapper" Then
                            Dim attr As XmlAttribute = Node.Attributes("guid")
                            If (attr IsNot Nothing) Then
                                DefaultWrapperGUID = attr.Value
                                Dim wrapper As Contensive.Addons.aoDashboard.Models.wrapperModel = wrapperDict(DefaultWrapperGUID)
                                If (wrapper IsNot Nothing) Then
                                    WrapperID = wrapper.id
                                End If
                            End If
                        End If
                    Next

                    NodePtr = CP.Doc.GetInteger("ptr")
                    Node = objXML.DocumentElement.ChildNodes(NodePtr)
                    If Not (Node Is Nothing) Then
                        If Node.Name = "node" Then
                            Node.Attributes.Append(Controllers.genericController.createAttribute(objXML, "state", "open"))
                            'Config = objXML.xml
                            Call Controllers.genericController.SaveConfig(CP, objXML)
                            AddonGuid = Controllers.genericController.GetXMLAttribute(Node, "addonGUID")
                            ContentGuid = Controllers.genericController.GetXMLAttribute(Node, "contentGUID")
                            ContentName = Controllers.genericController.GetXMLAttribute(Node, "contentName")
                            SettingGUID = Controllers.genericController.GetXMLAttribute(Node, "settingGUID")
                            Title = Controllers.genericController.GetXMLAttribute(Node, "title")
                            PosX = CP.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(Node, "x"))
                            PosY = CP.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(Node, "y"))
                            State = Controllers.genericController.GetXMLAttribute(Node, "state")
                            SizeX = CP.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(Node, "sizex"))
                            SizeY = CP.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(Node, "sizey"))
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
