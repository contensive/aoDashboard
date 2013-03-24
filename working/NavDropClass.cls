VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "NavDropClass"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True

Option Explicit

Private Main As Object
Private CSV As Object
'
'
'
Public Function Execute(CSVObject As Object, MainObject As Object, optionString As String, FilterInput As String) As String
    On Error GoTo ErrorTrap
    '
    Dim IconZIndex As Long
    Dim ItemHtmlID As String
    Dim GuidGenerator As New GuidGenerator
    'Dim Name As String
    Dim ShortcutHref As String
    Dim e As IXMLDOMElement
    Dim RequiredJS As String
    Dim common As New CommonClass
    Dim UserConfigFilename As String
    Dim CS As Long
    Dim AddonID As Long
    Set Main = MainObject
    Set CSV = CSVObject
    
    Dim Stream As String
    Dim objXML As New MSXML2.DOMDocument60
    'Dim objFSO As New kmaFileSystem3.FileSystemClass
    
    Dim Node As IXMLDOMElement
    
    Dim Config As String
    Dim NodeCount As Long
    Dim Counter As Long
    
    Dim AddonName As String
    Dim AddonGuid As String
    Dim ContentGuid As String
    Dim ContentName As String
    Dim SettingGUID As String
    Dim Title As String
    Dim SrcX As Long
    Dim SrcY As Long
    Dim State As String
    Dim SizeX As Long
    Dim SizeY As Long
    Dim Options As String
    Dim AttrCount As Long
    Dim WrapperID As Long
    Dim DefaultConfigfilename As String
    Dim ItemID As String
    Dim NodeAttribute As IXMLDOMAttribute
    Dim Copy As String
    Dim NodePtr As Long
    Dim SrcID As String
    Dim IconFileName As String
    Dim IconWidth As Long
    Dim IconHeight As Long
    Dim IconSprites As Long
    '
    SrcID = Main.GetStreamText("id")
    SrcX = Main.GetStreamInteger("x")
    SrcY = Main.GetStreamInteger("y")
    '
    Dim objFSO As Object
    Set objFSO = CreateObject("kmaFileSystem3.FileSystemClass")
    '
    Select Case LCase(Left(SrcID, 1))
        Case "a"
            '
            ' An Addon was dragged onto the desktop
            '
            AddonID = kmaEncodeInteger(Mid(SrcID, 2))
            If AddonID = 0 Then
                '
                ' Bad ID - ignore call and send 'kill' back
                '
            Else
                '
                '
                '
                CS = Main.OpenCSContentRecord("Add-ons", AddonID)
                If Not Main.IsCSOK(CS) Then
                    '
                    ' Bad ID - ignore call and send 'kill' back
                    '
                Else
                    AddonName = Main.GetCSText(CS, "name")
                    IconFileName = Main.GetCSText(CS, "IconFilename")
                    If IconFileName = "" Then
                        '
                        ' Default Icon
                        '
                        IconFileName = "/cclib/images/icons/addon.png"
                        IconWidth = 57
                        IconHeight = 59
                        IconSprites = 4
                    Else
                        '
                        ' Custom Icon
                        '
                        IconWidth = Main.GetCSInteger(CS, "IconWidth")
                        IconHeight = Main.GetCSInteger(CS, "IconHeight")
                        IconSprites = Main.GetCSInteger(CS, "IconSprites")
                    End If
                    AddonGuid = Main.GetCSText(CS, "ccguid")
                    If AddonGuid = "" Then
                        AddonGuid = GuidGenerator.CreateGUID("")
                        Call Main.SetCS(CS, "ccguid", AddonGuid)
                    End If
                    ShortcutHref = Main.ServerPage & "?addonid=" & AddonID
                    '
                    ' Add this add-on to the config and return the Icon
                    '
                    Set objXML = common.LoadConfig(Main)
                    If objXML.hasChildNodes Then
                        '
                        ' Find defaultwrapper
                        '
Dim DefaultWrapperGUID As String
                        For Each Node In objXML.documentElement.childNodes
                            If LCase(Node.baseName) = "defaultwrapper" Then
                                DefaultWrapperGUID = Node.getAttribute("guid")
                                CS = Main.OpenCSContent("wrappers", "ccguid=" & KmaEncodeSQLText(DefaultWrapperGUID))
                                If Main.IsCSOK(CS) Then
                                    WrapperID = Main.GetCSInteger(CS, "id")
                                End If
                                Call Main.CloseCS(CS)
                                Exit For
                            End If
                        Next
                        '
                        ' Create the new dodad node
                        '
                        Set e = objXML.documentElement
                        NodePtr = e.childNodes.length
                        IconZIndex = NodePtr
                        ItemHtmlID = "dashnode" & NodePtr
                        Set Node = objXML.createElement("node")
                        Call Node.setAttribute("addonGUID", AddonGuid)
                        Call Node.setAttribute("title", AddonName)
                        Call Node.setAttribute("x", Int(SrcX))
                        Call Node.setAttribute("y", Int(SrcY))
                        Call Node.setAttribute("state", "closed")
                        Call Node.setAttribute("sizex", 200)
                        Call Node.setAttribute("sizey", 300)
                        Call Node.setAttribute("optionstring", "")
                        Call e.appendChild(Node)
                        Call common.SaveConfig(Main, objXML)
                        ContentGuid = ""
                        ContentName = ""
                        Execute = common.GetDodad(Main, AddonGuid, ContentGuid, ContentName, AddonName, SrcX, SrcY, "closed", 77, 77, "", WrapperID, NodePtr, RequiredJS, IconZIndex)
                    End If
                End If
                Call Main.CloseCS(CS)
            End If
        Case "c"
            '
            ' A content link was dragged onto the desktop
            '
Dim ContentID As Long
            ContentID = kmaEncodeInteger(Mid(SrcID, 2))
            If ContentID = 0 Then
                '
                ' Bad ID - ignore call and send 'kill' back
                '
            Else
                '
                '
                '
                CS = Main.OpenCSContentRecord("Content", ContentID)
                If Not Main.IsCSOK(CS) Then
                    '
                    ' Bad ID - ignore call and send 'kill' back
                    '
                Else
                    'IconFileName = Main.GetCSText(CS, "IconFilename")
                    If IconFileName = "" Then
                        '
                        ' Default Icon
                        '
                        IconFileName = "/cclib/images/icons/content.png"
                        IconWidth = 57
                        IconHeight = 59
                        IconSprites = 4
                    Else
                        '
                        ' Custom Icon
                        '
                        IconWidth = Main.GetCSInteger(CS, "IconWidth")
                        IconHeight = Main.GetCSInteger(CS, "IconHeight")
                        IconSprites = Main.GetCSInteger(CS, "IconSprites")
                    End If
                    ContentGuid = Main.GetCSText(CS, "ccguid")
                    SizeX = 77
                    SizeY = 77
                    ShortcutHref = Main.ServerPage & "?cid=" & ContentID
                    ContentName = Main.GetCSText(CS, "name")
                    '
                    ' Add this add-on to the config and return the Icon
                    '
                    Set objXML = common.LoadConfig(Main)
                    If objXML.hasChildNodes Then
                        Set e = objXML.documentElement
                        NodePtr = e.childNodes.length
                        IconZIndex = NodePtr
                        ItemHtmlID = "dashnode" & NodePtr
                        Set Node = objXML.createElement("node")
                        Call Node.setAttribute("contentName", ContentName)
                        Call Node.setAttribute("contentGUID", ContentGuid)
                        Call Node.setAttribute("title", ContentName)
                        Call Node.setAttribute("x", SrcX)
                        Call Node.setAttribute("y", SrcY)
                        Call Node.setAttribute("state", "closed")
                        Call Node.setAttribute("sizex", SizeX)
                        Call Node.setAttribute("sizey", SizeY)
                        Call Node.setAttribute("optionstring", "")
                        Call e.appendChild(Node)
                        Call common.SaveConfig(Main, objXML)
                        Execute = common.GetDodad(Main, 0, "", ContentName, ContentName, SrcX, SrcY, "closed", SizeX, SizeY, "", 0, NodePtr, RequiredJS, IconZIndex)
                        'Execute = common.GetDodadContent(Main, 0, "", "", "closed", IconSprites, Name, IconFileName, IconWidth, IconHeight, Main.ServerFilePath, WrapperID, SizeX, SizeY, ShortcutHref)
                    End If
                End If
                Call Main.CloseCS(CS)
            End If
        Case Else
            Execute = "<!-- no object found for this id -->"
    End Select
    '
    ' Add in javascript
    '
    If RequiredJS <> "" Then
        Execute = Execute _
            & CR & "<script type=""text/javascript"">" _
            & kmaIndent(RequiredJS) _
            & CR & "</script>"
    End If
    Exit Function
ErrorTrap:
    Call HandleError("DashDragStopClass", "Init", Err.Number, Err.Source, Err.Description, True, False)
End Function
'
'
'
Private Function GetXMLAttribute(Node As IXMLDOMNode, Name As String) As String
    On Error GoTo ErrorTrap
    
    Dim NodeAttribute As IXMLDOMAttribute
    Dim ResultNode As IXMLDOMNode
    Dim UcaseName As String
    Dim Found As Boolean
    
    Found = False
    If Not (Node.Attributes Is Nothing) Then
    Set ResultNode = Node.Attributes.getNamedItem(Name)
    If (ResultNode Is Nothing) Then
        UcaseName = UCase(Name)
        For Each NodeAttribute In Node.Attributes
            If UCase(NodeAttribute.nodeName) = UcaseName Then
                GetXMLAttribute = NodeAttribute.nodeValue
                Found = True
                Exit For
                End If
            Next
        If Not Found Then
            GetXMLAttribute = ""
        End If
    Else
        GetXMLAttribute = ResultNode.nodeValue
        Found = True
    End If
    End If
    Exit Function

ErrorTrap:
    Call HandleError("DashDragStopClass", "GetXMLAttribute", Err.Number, Err.Source, Err.Description, True, True)
    Resume Next
    End Function
