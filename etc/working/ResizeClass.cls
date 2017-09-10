VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "ResizeClass"
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
    Set Main = MainObject
    Set CSV = CSVObject
    
    Dim Stream As String
    Dim objXML As New MSXML2.DOMDocument60
    'Dim objFSO As New kmaFileSystem3.FileSystemClass
    
    Dim Node As IXMLDOMElement
    
    Dim Config As String
    Dim NodeCount As Long
    Dim Counter As Long
    
    Dim AddonGuid As String
    Dim ContentGuid As String
    Dim ContentName As String
    Dim SettingGUID As String
    Dim Title As String
    'Dim PosX As String
    'Dim PosY As String
    Dim State As String
    'Dim SizeX As String
    'Dim SizeY As String
    Dim Options As String
    Dim AttrCount As Long
    Dim WrapperID As Long
    Dim DefaultConfigfilename As String
    Dim UserConfigFilename As String
    Dim ItemID As String
    Dim NodeAttribute As IXMLDOMAttribute
    Dim Copy As String
    Dim common As New CommonClass
    '
    Dim NodePtr As Long
    '
    Dim objFSO As Object
    Set objFSO = CreateObject("kmaFileSystem3.FileSystemClass")
    '
    Set objXML = common.LoadConfig(Main)
    'DefaultConfigfilename = Main.PhysicalFilePath & "upload\dashboard\dashconfig.xml"
    'UserConfigFilename = Main.PhysicalFilePath & "upload\dashboard\dashconfig." & Main.memberID & ".xml"
    'Config = objFSO.ReadFile(UserConfigFilename)
    'If Config = "" Then
    '    Config = objFSO.ReadFile(DefaultConfigfilename)
    '    Call objFSO.SaveFile(UserConfigFilename, Config)
    'End If
    'objXML.loadXML (Config)
    WrapperID = 0
    If objXML.hasChildNodes Then
        NodePtr = Main.GetStreamInteger("ptr")
        Set Node = objXML.documentElement.childNodes(NodePtr)
        If Not (Node Is Nothing) Then
            If Node.nodeName = "node" Then
                Copy = Main.GetStreamText("x")
                Copy = Replace(Copy, "px", "")
                If IsNumeric(Copy) Then
                    Call Node.setAttribute("sizex", Copy)
                End If
                Copy = Main.GetStreamText("y")
                Copy = Replace(Copy, "px", "")
                If IsNumeric(Copy) Then
                    Call Node.setAttribute("sizey", Copy)
                End If
                Call common.SaveConfig(Main, objXML)
                'Config = objXML.xml
                'Call objFSO.SaveFile(UserConfigFilename, Config)
            End If
        End If
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


