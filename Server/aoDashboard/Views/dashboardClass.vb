
Option Explicit On
Option Strict On

Imports Contensive.BaseClasses

Namespace Views
    Public Class DashboardClass
        Inherits AddonBaseClass
        ''' <summary>
        ''' addon that generates the dashboard html
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <returns></returns>
        Public Overrides Function Execute(ByVal cp As CPBaseClass) As Object
            Dim result As String = ""
            Try
                Dim iconZIndexTop As Integer = 0
                '
                ' -- create dashboard html and additional javascript
                Dim config As Models.ConfigModel = Models.ConfigModel.create(cp, cp.User.Id)
                Dim Dashboard As String = ""
                For Each kvp In config.nodeList
                    Dashboard &= Models.NodeModel.getNodeHtml(cp, kvp.Value)
                    iconZIndexTop = If((iconZIndexTop > kvp.Value.z), iconZIndexTop, kvp.Value.z)
                Next
                '
                ' - add to page
                cp.Doc.AddBodyEnd("<script type=""text/javascript"">/* set ztop */ var iconZIndexTop=" & iconZIndexTop & ";</script>")
                result = cp.Html.div(Dashboard, "dashboard", "dashBoardWrapper", "dashBoardWrapper")
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
    End Class
End Namespace
