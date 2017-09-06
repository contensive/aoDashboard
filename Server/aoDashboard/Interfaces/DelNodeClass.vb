
Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Interfaces
    Public Class DelNodeClass
        Inherits AddonBaseClass
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                '
                Dim objXML As XmlDocument = Controllers.genericController.LoadConfig(CP)
                If objXML.HasChildNodes Then
                    Dim NodePtr As Integer = CP.Doc.GetInteger("ptr")
                    Dim Node As XmlNode = objXML.DocumentElement.ChildNodes(NodePtr)
                    If (Node IsNot Nothing) Then
                        If Node.Name.ToLower() = "node" Then
                            Node.Attributes.Append(Controllers.genericController.createAttribute(objXML, "deleted", "yes"))
                            Call Controllers.genericController.SaveConfig(CP, objXML)
                        End If
                    End If
                End If
            Catch ex As Exception
                CP.Site.ErrorReport(ex)
            End Try
            Return returnHtml
        End Function
    End Class
End Namespace
