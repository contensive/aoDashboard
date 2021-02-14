
Imports Contensive.BaseClasses

Namespace Views
    Public Class OpenNodeClass
        Inherits AddonBaseClass
        ''' <summary>
        ''' change a node state to open and return the html
        ''' </summary>
        ''' <param name="CP"></param>
        ''' <returns></returns>
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim result As String = ""
            Try
                Dim request As New Models.RequestModel(CP)
                If (Not String.IsNullOrWhiteSpace(request.key)) Then
                    Dim config As Models.ConfigModel = Models.ConfigModel.create(CP, CP.User.Id)
                    If (config IsNot Nothing) Then
                        If (config.nodeList.ContainsKey(request.key)) Then
                            Dim configNode As Models.ConfigNodeModel = config.nodeList(request.key)
                            configNode.state = Models.ConfigNodeState.open
                            config.save(CP)
                            result = Models.NodeModel.getNodeHtml(CP, configNode)
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
