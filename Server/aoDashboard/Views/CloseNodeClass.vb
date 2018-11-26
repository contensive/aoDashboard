
Option Explicit On
Option Strict On

Imports Contensive.BaseClasses

Namespace Views
    Public Class CloseNodeClass
        Inherits AddonBaseClass
        ''' <summary>
        ''' remote method to change the state of a node to closed
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <returns></returns>
        Public Overrides Function Execute(cp As CPBaseClass) As Object
            Dim result As String = ""
            Try
                Dim request As New Models.RequestModel(cp)
                If (Not String.IsNullOrWhiteSpace(request.key)) Then
                    Dim config As Models.ConfigModel = Models.ConfigModel.create(cp, cp.User.Id)
                    If (config IsNot Nothing) Then
                        If (config.nodeList.ContainsKey(request.key)) Then
                            Dim configNode As Models.ConfigModel.ConfigNodeModel = config.nodeList(request.key)
                            configNode.state = Models.ConfigModel.ConfigNodeState.closed
                            config.save(cp)
                            result = Models.NodeModel.getNodeHtml(cp, configNode)
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

