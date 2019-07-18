
Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.Addons.Dashboard.Controllers
Imports Contensive.BaseClasses

Namespace Models
    Public Class ConfigModel
        Public defaultWrapper As ConfigWrapper
        Public nodeList As Dictionary(Of String, ConfigNodeModel)

        '
        Public Class ConfigWrapper
            Public guid As String
        End Class
        '
        Public Class NameValueModel
            Public name As String
            Public value As String
        End Class
        '
        Public Enum ConfigNodeState
            closed = 1
            open = 2
        End Enum
        '
        Public Class ConfigNodeModel
            ''' <summary>
            ''' The htmlId of the node. 
            ''' Also the key into the dictionary of these objects.
            ''' Also the prefix for other html structions: 
            ''' </summary>
            Public key As String
            ''' <summary>
            ''' optional, if provided this icon will link to the content list page
            ''' </summary>
            Public contentName As String
            Public contentGUID As String
            ''' <summary>
            ''' opional, if provided this icon will link to the execution of this addon
            ''' </summary>
            Public addonGUID As String
            ''' <summary>
            ''' title for this node
            ''' </summary>
            Public title As String
            ''' <summary>
            ''' open, closed, etc - convert to an enumeration
            ''' </summary>
            Public state As ConfigNodeState
            ''' <summary>
            ''' the width
            ''' </summary>
            Public sizex As Integer
            ''' <summary>
            ''' the height
            ''' </summary>
            Public sizey As Integer
            ''' <summary>
            ''' if node is for an addon, set these request properties first
            ''' </summary>
            Public addonArgList As List(Of NameValueModel)
            ''' <summary>
            ''' The style top for the object
            ''' </summary>
            Public x As Integer
            ''' <summary>
            ''' the style left for the object
            ''' </summary>
            Public y As Integer
            ''' <summary>
            ''' The html style z index for this object
            ''' </summary>
            Public z As Integer
            ''' <summary>
            ''' If not empty, this link will be used for the dashboard icon. Use for navigator entries setup as links.
            ''' </summary>
            Public link As String
            ''' <summary>
            ''' currently not used. The id of the wrapper object
            ''' </summary>
            Public wrapperId As Integer
        End Class
        '
        '====================================================================================================
        ''' <summary>
        ''' Create a config model of the user. If not found, one is created.
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="userId"></param>
        ''' <returns></returns>
        Public Shared Function create(cp As CPBaseClass, userId As Integer) As ConfigModel
            Dim result As ConfigModel = load(cp, userId)
            If (result Is Nothing) Then
                '
                ' -- try legacy config
                result = createFromLegacyXmlData(cp, userId)
                If (result Is Nothing) Then
                    '
                    ' -- load default
                    result = load(cp, 0)
                    If (result Is Nothing) Then
                        '
                        ' -- load legacy default and create current default
                        result = createFromLegacyXmlData(cp, 0)
                        '
                        ' -- create default
                        result.save(cp, 0)
                    End If
                End If
                result.save(cp, userId)
            End If
            Return result
        End Function
        '
        '====================================================================================================
        ''' <summary>
        ''' load config for a user. Returns null if config file not found
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="userId"></param>
        ''' <returns></returns>
        Private Shared Function load(cp As CPBaseClass, userId As Integer) As ConfigModel
            Dim result As ConfigModel = Nothing
            Dim userConfigFilename As String = "dashboard\dashconfig." & userId & ".json"
            Dim jsonConfigText As String = cp.File.ReadVirtual(userConfigFilename)
            If (Not String.IsNullOrWhiteSpace(jsonConfigText)) Then
                result = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ConfigModel)(jsonConfigText)
            End If
            Return result
        End Function
        '
        '====================================================================================================
        ''' <summary>
        ''' save config for a specified user. Needed to save for the default user=0 
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="userId"></param>
        Public Sub save(cp As CPBaseClass, userId As Integer)
            cp.File.SaveVirtual("dashboard\dashconfig." & userId & ".json", Newtonsoft.Json.JsonConvert.SerializeObject(Me))
        End Sub
        '
        '====================================================================================================
        ''' <summary>
        ''' save config for the current user
        ''' </summary>
        ''' <param name="cp"></param>
        Public Sub save(cp As CPBaseClass)
            save(cp, cp.User.Id)
        End Sub
        '
        '====================================================================================================
        '
        Public Shared Function createFromLegacyXmlData(cp As CPBaseClass, userId As Integer) As ConfigModel
            Dim result As ConfigModel = Nothing
            Try
                Dim UserConfigFilename As String = If(userId <= 0, "dashboard\dashconfig.xml", "dashboard\dashconfig." & userId & ".xml")
                Dim xmlConfigText As String = cp.File.ReadVirtual(UserConfigFilename)
                If (Not String.IsNullOrWhiteSpace(xmlConfigText)) Then
                    Dim xmlConfig As New System.Xml.XmlDocument
                    xmlConfig.LoadXml(xmlConfigText)
                    If xmlConfig.HasChildNodes Then
                        result = New ConfigModel With {
                            .nodeList = New Dictionary(Of String, ConfigNodeModel)
                        }
                        '
                        ' -- review values, remove deleted nodes and get settings
                        Dim nodeKeySuffix As Integer = 0
                        For Each node As XmlElement In xmlConfig.DocumentElement.ChildNodes
                            Select Case LCase(node.Name)
                                Case "defaultwrapper"
                                    result.defaultWrapper = New ConfigWrapper() With {
                                        .guid = GenericController.getAttribute(cp, node, "guid")
                                    }
                                Case "node"
                                    Dim addonArgList As New List(Of NameValueModel)
                                    Dim optionString = Controllers.GenericController.getAttribute(cp, node, "optionstring")
                                    If (Not String.IsNullOrWhiteSpace(optionString)) Then
                                        For Each namevalue As String In optionString.Split("&"c)
                                            If (namevalue.IndexOf("="c) > 0) Then
                                                Dim pair As String() = namevalue.Split("="c)
                                                addonArgList.Add(New NameValueModel() With {
                                                    .name = pair(0),
                                                    .value = pair(1)
                                                })
                                            End If
                                        Next
                                    End If
                                    Dim nodeKey As String = "node" & nodeKeySuffix
                                    Dim nodeState As ConfigNodeState
                                    [Enum].TryParse(Controllers.GenericController.getAttribute(cp, node, "state"), nodeState)
                                    result.nodeList.Add(nodeKey, New ConfigNodeModel() With {
                                        .key = nodeKey,
                                        .addonArgList = addonArgList,
                                        .addonGUID = GenericController.getAttribute(cp, node, "addonGUID"),
                                        .contentGUID = GenericController.getAttribute(cp, node, "contentGUID"),
                                        .contentName = GenericController.getAttribute(cp, node, "contentName"),
                                        .sizex = cp.Utils.EncodeInteger(Controllers.GenericController.getAttribute(cp, node, "sizex")),
                                        .sizey = cp.Utils.EncodeInteger(GenericController.getAttribute(cp, node, "sizey")),
                                        .state = nodeState,
                                        .title = Controllers.GenericController.getAttribute(cp, node, "title"),
                                        .x = cp.Utils.EncodeInteger(GenericController.getAttribute(cp, node, "x")),
                                        .y = cp.Utils.EncodeInteger(GenericController.getAttribute(cp, node, "y"))
                                    })
                            End Select
                            nodeKeySuffix += 1
                        Next
                    End If
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
    End Class
End Namespace
