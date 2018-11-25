Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Views
    Public Class ResizeClass
        Inherits AddonBaseClass
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                Dim nodeKey As String = CP.Doc.GetText("node")
                If (Not String.IsNullOrWhiteSpace(nodeKey)) Then
                    Dim config2 As Models.configModel = Models.configModel.create(CP, CP.User.Id)
                    If (config2 IsNot Nothing) Then
                        If (config2.nodeList.ContainsKey(nodeKey)) Then
                            config2.nodeList(nodeKey).sizex = CP.Doc.GetInteger("x")
                            config2.nodeList(nodeKey).sizey = CP.Doc.GetInteger("y")
                            config2.save(CP)
                        End If
                    End If
                End If

                'Dim config As XmlDocument = Controllers.genericController.LoadConfig(CP)
                'Dim WrapperID As Integer = 0
                'If config.HasChildNodes Then
                '    Dim nodeKey As String = CP.Doc.GetText("node")
                '    Dim Node As XmlNode = config.DocumentElement.ChildNodes(nodeKey)
                '    If (Node IsNot Nothing) Then
                '        If Node.Name = "node" Then
                '            Dim xValue As String = CP.Doc.GetText("x")
                '            xValue = Replace(xValue, "px", "")
                '            If IsNumeric(xValue) Then
                '                Dim nodeAttr As XmlAttribute = Node.Attributes("sizex")
                '                If (nodeAttr IsNot Nothing) Then
                '                    nodeAttr.Value = xValue
                '                End If
                '            End If
                '            Dim yValue As String = CP.Doc.GetText("y")
                '            yValue = Replace(yValue, "px", "")
                '            If IsNumeric(yValue) Then
                '                Dim nodeAttr As XmlAttribute = Node.Attributes("sizey")
                '                If (nodeAttr IsNot Nothing) Then
                '                    nodeAttr.Value = yValue
                '                End If
                '            End If
                '            Call Controllers.genericController.SaveConfig(CP, config)
                '        End If
                '    End If
                'End If
            Catch ex As Exception
                CP.Site.ErrorReport(ex)
            End Try
            Return returnHtml
        End Function
    End Class
End Namespace
