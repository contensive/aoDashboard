
Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Views
    Public Class DelNodeClass
        Inherits AddonBaseClass
        ''' <summary>
        ''' remote method called when a node is deleted on the desktop (click the delete link on the handle)
        ''' </summary>
        ''' <param name="CP"></param>
        ''' <returns></returns>
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                Dim nodeKey As String = CP.Doc.GetText("node")
                If (Not String.IsNullOrWhiteSpace(nodeKey)) Then
                    Dim config2 As Models.configModel = Models.configModel.create(CP, CP.User.Id)
                    If (config2 IsNot Nothing) Then
                        If (config2.nodeList.ContainsKey(nodeKey)) Then
                            config2.nodeList.Remove(nodeKey)
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
