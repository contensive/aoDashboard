Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses
Imports System.Xml

Namespace Contensive.Addons.aoDashboard
    '
    Public Class closeNodeClass
        Inherits AddonBaseClass
        '
        Public Overrides Function Execute(cp As CPBaseClass) As Object
            Dim result As String = ""
            Try
                Dim IconZIndex As Integer
                Dim Config As String
                Dim objXML As New XmlDocument
                Dim Node As XmlNode
                Dim NodePtr As Integer
                Dim RequiredJS As String
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
                Dim WrapperID As Integer
                '
                NodePtr = cp.Doc.GetInteger("ptr")
                '
                ' open the config and mark the node in the config as closed
                '
                objXML = Controllers.genericController.LoadConfig(cp)
                If objXML.HasChildNodes Then
                    Node = objXML.DocumentElement.ChildNodes(NodePtr)
                    If Not (Node Is Nothing) Then
                        If Node.Name = "node" Then
                            Node.Attributes("state").Value = "closed"
                            Config = objXML.OuterXml
                            Call Controllers.genericController.SaveConfig(cp, objXML)
                            AddonGuid = Controllers.genericController.GetXMLAttribute(Node, "addonGUID")
                            ContentGuid = Controllers.genericController.GetXMLAttribute(Node, "contentGUID")
                            ContentName = Controllers.genericController.GetXMLAttribute(Node, "contentName")
                            SettingGUID = Controllers.genericController.GetXMLAttribute(Node, "settingGUID")
                            Title = Controllers.genericController.GetXMLAttribute(Node, "title")
                            PosX = cp.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(Node, "x"))
                            PosY = cp.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(Node, "y"))
                            State = Controllers.genericController.GetXMLAttribute(Node, "state")
                            SizeX = cp.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(Node, "sizex"))
                            SizeY = cp.Utils.EncodeInteger(Controllers.genericController.GetXMLAttribute(Node, "sizey"))
                            Options = Controllers.genericController.GetXMLAttribute(Node, "optionstring")
                            result = Controllers.genericController.GetDodad(cp, AddonGuid, ContentGuid, ContentName, Title, PosX, PosY, State, SizeX, SizeY, Options, WrapperID, NodePtr, RequiredJS, IconZIndex)
                            '
                            ' Add in javascript
                            '
                            If RequiredJS <> "" Then
                                result = result _
                                    & "<script type=""text/javascript"">" _
                                    & (RequiredJS) _
                                    & "</script>"
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
    End Class
End Namespace
