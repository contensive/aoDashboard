
Option Explicit On
Option Strict On

Imports Contensive.BaseClasses

Namespace Views
    Public Class DashDragStopClass
        Inherits AddonBaseClass
        ''' <summary>
        ''' remote method called when a node is dragged to a new location and stopped (mouse button released)
        ''' </summary>
        ''' <param name="CP"></param>
        ''' <returns></returns>
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim result As String = ""
            Try
                Dim request As New Models.RequestModel(CP)
                If (Not String.IsNullOrWhiteSpace(request.key)) Then
                    Dim config As Models.configModel = Models.configModel.create(CP, CP.User.Id)
                    If (config IsNot Nothing) Then
                        If (config.nodeList.ContainsKey(request.key)) Then
                            Dim configNode As Models.configModel.ConfigNodeModel = config.nodeList(request.key)
                            configNode.x = request.x
                            configNode.y = request.y
                            config.save(CP)
                            result = Controllers.genericController.getNodeHtml(CP, configNode)
                        End If
                    End If
                End If
            Catch ex As Exception
                CP.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
    End Class
End Namespace
