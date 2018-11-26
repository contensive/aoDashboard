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
                Dim request As New Models.RequestModel(CP)
                Dim nodeKey As String = request.key
                If (Not String.IsNullOrWhiteSpace(nodeKey)) Then
                    Dim config2 As Models.ConfigModel = Models.ConfigModel.create(CP, CP.User.Id)
                    If (config2 IsNot Nothing) Then
                        If (config2.nodeList.ContainsKey(nodeKey)) Then
                            config2.nodeList(nodeKey).sizex = request.x
                            config2.nodeList(nodeKey).sizey = request.y
                            config2.save(CP)
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
