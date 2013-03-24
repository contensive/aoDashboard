VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "DashboardClass"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True

Option Explicit

Private Main As Object
Private CSV As Object
Private GlobalJS As String

Public Function Execute(CSVObject As Object, MainObject As Object, optionString As String, FilterInput As String) As String
    On Error GoTo ErrorTrap

    Call Init(MainObject)
    
    Execute = GetContent(optionString)

    Exit Function
ErrorTrap:
    Call HandleError("DashboardClass", "Execute", Err.Number, Err.Source, Err.Description, True, False)
    'Call HandleError("DashboardClass", "Init", Err.Number, Err.Source, Err.Description, True, False)
End Function

Private Sub Init(MainObject As Object)

    Set Main = MainObject

    Exit Sub
ErrorTrap:
    Call HandleError("DashboardClass", "Init", Err.Number, Err.Source, Err.Description, True, False)
End Sub

Private Function GetContent(optionString As String) As String
    On Error GoTo ErrorTrap
    '
    Dim hint As String
    Dim CS As Long
    Dim IconZIndex As Long
    Dim RequiredJS As String
    Dim Dashboard As String
    Dim objXML As New MSXML2.DOMDocument60
    Dim Node As IXMLDOMElement
    Dim ParentNode As IXMLDOMElement
    Dim Config As String
    Dim NodeCount As Long
    Dim Counter As Long
    Dim AddonGuid As String
    Dim ContentGuid As String
    Dim ContentName As String
    Dim SettingGUID As String
    Dim Title As String
    Dim PosX As Long
    Dim PosY As Long
    Dim State As String
    Dim SizeX As Long
    Dim SizeY As Long
    Dim Options As String
    Dim AttrCount As Long
    Dim WrapperID As Long
    Dim DefaultWrapperGUID As String
    Dim DefaultConfigfilename As String
    Dim UserConfigFilename As String
    Dim ItemID As String
    Dim NodePtr As Long
    Dim common As New CommonClass
    
    Dim ConfigChanged As Boolean
    '
    If Main.SiteProperty_BuildVersion >= "3.4.060" Then
        Set objXML = common.LoadConfig(Main)
        WrapperID = 0
        If objXML.hasChildNodes Then
            '
            ' review values, remove deleted nodes and get settings
            '
            ConfigChanged = False
            For Each Node In objXML.documentElement.childNodes
                Select Case LCase(Node.baseName)
                    Case "defaultwrapper"
                        DefaultWrapperGUID = Node.getAttribute("guid")
                        CS = Main.OpenCSContent("wrappers", "ccguid=" & KmaEncodeSQLText(DefaultWrapperGUID))
                        If Main.IsCSOK(CS) Then
                            WrapperID = Main.GetCSInteger(CS, "id")
                        End If
                        Call Main.CloseCS(CS)
                    Case "node"
                        AddonGuid = kmaEncodeText(Node.getAttribute("addonGUID"))
                        If AddonGuid = "" Then
                            ContentGuid = kmaEncodeText(Node.getAttribute("contentGUID"))
                            If ContentGuid = "" Then
                                ContentName = kmaEncodeText(Node.getAttribute("contentName"))
                            End If
                        End If
                        SizeY = kmaEncodeInteger(Node.getAttribute("sizey"))
                        If ((AddonGuid = "") And (ContentGuid = "") And (ContentName = "")) Or kmaEncodeBoolean(Node.getAttribute("deleted")) Then
                            '
                            ' delete any nodes marked as delete
                            ' this is actually a problem if the user has mulitple windows open,
                            ' and he deletes an entry from one window, refreshes, then moves
                            ' or deletes anything from the other window
                            ' - need to get rid of the 'ptr' scheme and go to an ID
                            '
                            Set ParentNode = Node.ParentNode
                            Call ParentNode.removeChild(Node)
                            ConfigChanged = True
                        Else
                            If kmaEncodeInteger(Node.getAttribute("x")) < 0 Then
                                Call Node.setAttribute("x", 0)
                                ConfigChanged = True
                            End If
                            If kmaEncodeInteger(Node.getAttribute("y")) < 0 Then
                                Call Node.setAttribute("y", 0)
                                ConfigChanged = True
                            End If
                        End If
                End Select
            Next
hint = "400"
            If ConfigChanged Then
                Call common.SaveConfig(Main, objXML)
            End If
            '
            ' draw the nodes that are left
            '
            NodePtr = 0
            For Each Node In objXML.documentElement.childNodes
                Select Case LCase(Node.baseName)
                    Case "node"
                        RequiredJS = ""
                        AddonGuid = common.GetXMLAttribute(Node, "addonGUID")
                        ContentGuid = common.GetXMLAttribute(Node, "contentGUID")
                        ContentName = common.GetXMLAttribute(Node, "contentName")
                        Title = common.GetXMLAttribute(Node, "title")
                        PosX = kmaEncodeInteger(common.GetXMLAttribute(Node, "x"))
                        PosY = kmaEncodeInteger(common.GetXMLAttribute(Node, "y"))
                        State = common.GetXMLAttribute(Node, "state")
                        SizeX = kmaEncodeInteger(common.GetXMLAttribute(Node, "sizex"))
                        SizeY = kmaEncodeInteger(common.GetXMLAttribute(Node, "sizey"))
                        Options = common.GetXMLAttribute(Node, "optionstring")
                        Dashboard = Dashboard & common.GetDodad(Main, AddonGuid, ContentGuid, ContentName, Title, PosX, PosY, State, SizeX, SizeY, Options, WrapperID, NodePtr, RequiredJS, IconZIndex)
                        GlobalJS = GlobalJS & RequiredJS
                        IconZIndex = IconZIndex + 1
                End Select
                NodePtr = NodePtr + 1
            Next
        End If
    Else
        Dashboard = Dashboard & CR & "<div class=""ccError"" style=""padding: 20px;"">In order to run the dashboard, this site needs to be running Contensive version 3.4.060 or higher. Currently it is running version " & Main.SiteProperty_BuildVersion & ".</div>"
    End If
    '
    ' Add JQuery UI dropable to desktop
    '
    GlobalJS = GlobalJS _
        & CR & "var iconZIndexTop=" & IconZIndex & ";" _
        & CR & "var nodeCnt=" & NodePtr & ";" _
        & CR & "$(""#desktop"").droppable({" _
            & "tolerance: 'fit'" _
        & "});"
    '
    ' Assemble final page
    '
    GetContent = "" _
        & CR & "<div id=""dashBoardWrapper"" class=""dashBoardWrapper"">" _
        & kmaIndent("&nbsp;" & Dashboard) _
        & CR & "</div>" _
        & CR & "<script type=""text/javascript"">" _
        & kmaIndent(GlobalJS) _
        & CR & "</script>"

    Exit Function
ErrorTrap:
    Call HandleError("DashboardClass", "GetContent, hint=" & hint, Err.Number, Err.Source, Err.Description, True, False)
    End Function
