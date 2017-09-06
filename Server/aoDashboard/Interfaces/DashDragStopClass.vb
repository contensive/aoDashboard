Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Interfaces
    Public Class DashDragStopClass
        Inherits AddonBaseClass
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                '
                Dim objXML As XmlDocument = Controllers.genericController.LoadConfig(CP)
                Dim WrapperID As Integer = 0
                If objXML.HasChildNodes Then
                    Dim NodePtr As Integer = CP.Doc.GetInteger("ptr")
                    Dim Node As XmlNode = objXML.DocumentElement.ChildNodes(NodePtr)
                    If (Node IsNot Nothing) Then
                        If Node.Name.ToLower() = "node" Then
                            Dim xValue As String = CP.Doc.GetText("x")
                            xValue = Replace(xValue, "px", "")
                            If IsNumeric(xValue) Then
                                Node.Attributes.Append(Controllers.genericController.createAttribute(objXML, "x", xValue))
                            End If
                            Dim yValue As String = CP.Doc.GetText("y")
                            yValue = Replace(xValue, "px", "")
                            If IsNumeric(yValue) Then
                                Node.Attributes.Append(Controllers.genericController.createAttribute(objXML, "y", yValue))
                            End If
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
