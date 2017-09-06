Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Interfaces
    Public Class ResizeClass
        Inherits AddonBaseClass
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                Dim config As XmlDocument = Controllers.genericController.LoadConfig(CP)
                Dim WrapperID As Integer = 0
                If config.HasChildNodes Then
                    Dim NodePtr As Integer = CP.Doc.GetInteger("ptr")
                    Dim Node As XmlNode = config.DocumentElement.ChildNodes(NodePtr)
                    If (Node IsNot Nothing) Then
                        If Node.Name = "node" Then
                            Dim xValue As String = CP.Doc.GetText("x")
                            xValue = Replace(xValue, "px", "")
                            If IsNumeric(xValue) Then
                                Dim nodeAttr As XmlAttribute = Node.Attributes("sizex")
                                If (nodeAttr IsNot Nothing) Then
                                    nodeAttr.Value = xValue
                                End If
                            End If
                            Dim yValue As String = CP.Doc.GetText("y")
                            yValue = Replace(yValue, "px", "")
                            If IsNumeric(yValue) Then
                                Dim nodeAttr As XmlAttribute = Node.Attributes("sizey")
                                If (nodeAttr IsNot Nothing) Then
                                    nodeAttr.Value = yValue
                                End If
                            End If
                            Call Controllers.genericController.SaveConfig(CP, config)
                        End If
                    End If
                End If
            Catch ex As Exception
                CP.Site.ErrorReport(ex)
            End Try
            Return returnHtml
        End Function
        '        '
        '        '
        '        '
        '        Private Function GetXMLAttribute(Node As XmlNode, Name As String) As String
        '            On Error GoTo ErrorTrap

        '            Dim NodeAttribute As XmlAttribute
        '            Dim ResultNode As XmlNode
        '            Dim UcaseName As String
        '            Dim Found As Boolean

        '            Found = False
        '            If Not (Node.Attributes Is Nothing) Then
        '                ResultNode = Node.Attributes.GetNamedItem(Name)
        '                If (ResultNode Is Nothing) Then
        '                    UcaseName = UCase(Name)
        '                    For Each NodeAttribute In Node.Attributes
        '                        If UCase(NodeAttribute.Name) = UcaseName Then
        '                            GetXMLAttribute = NodeAttribute.nodeValue
        '                            Found = True
        '                            Exit For
        '                        End If
        '                    Next
        '                    If Not Found Then
        '                        GetXMLAttribute = ""
        '                    End If
        '                Else
        '                    GetXMLAttribute = ResultNode.nodeValue
        '                    Found = True
        '                End If
        '            End If
        '            Exit Function

        'ErrorTrap:
        '            Call HandleError("DashDragStopClass", "GetXMLAttribute", Err.Number, Err.Source, Err.Description, True, True)
        '            Resume Next
        '        End Function



        ''
        ''=====================================================================================
        '' common report for this class
        ''=====================================================================================
        ''
        'Private Sub errorReport(ByVal cp As CPBaseClass, ByVal ex As Exception, ByVal method As String)
        '    Try
        '        cp.Site.ErrorReport(ex, "Unexpected error in sampleClass." & method)
        '    Catch exLost As Exception
        '        '
        '        ' stop anything thrown from cp errorReport
        '        '
        '    End Try
        'End Sub
    End Class
End Namespace
