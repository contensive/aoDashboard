
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Contensive.Addons.Menuing
    Public Class liMenuClass
        Inherits AddonBaseClass
        '
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String
            Try
                Dim classAttr As String
                Dim Ptr As Integer
                Dim nodeCnt As Integer
                Dim cssFile As String
                Dim jsFile As String
                Dim jsOnLoad As String
                Dim menuName As String
                Dim ClickIndicator As String
                Dim FlyoutOnHover As Boolean
                Dim JSOnOut As String
                Dim JSOnHover As String
                Dim JSOnClick As String
                Dim SubMenu As String
                Dim HotSpot As String
                Dim MenuStyle As String
                Dim HorizontalLayout As Boolean
                Dim aTag As String
                Dim MenuNew As String
                Dim SiteTree As siteTreeClass
                Dim SiteTreeData As String
                Dim doc As DOMDocument30
                Dim LoopPtr As Integer
                Dim Content As fastStringClass
                Dim TopNode As IXMLDOMNode
                Dim TabNode As IXMLDOMNode
                Dim node As IXMLDOMNode
                Dim IsFound As Boolean
                Dim Caption As String
                Dim ImageSrc As String
                Dim ImageOverSrc As String
                Dim ImageDownScr As String
                Dim ImageDownOverSrc As String
                Dim MenuSectionID As Integer
                Dim menuPageId As Integer
                Dim renderedSectionId As Integer
                Dim renderedPageId As Integer
                Dim StylePrefix As String
                Dim FlyoutDirection As Integer
                Dim menuId As Integer
                Dim menuHtmlId As String
                '
                ' For Page Addons, return the result
                '
                Randomize()
                '
                SiteTree = New siteTreeClass
                SiteTreeData = SiteTree.Execute(CsvObject, MainObject, OptionString, FilterInput)
                If SiteTreeData <> "" Then
        Set doc = New DOMDocument30
        doc.loadXML(SiteTreeData)
                    Do While doc.readyState <> 4 And LoopPtr < 50
                        Sleep(100)
                        DoEvents
                        LoopPtr = LoopPtr + 1
                    Loop
                    If doc.readyState <> 4 Then
                        '
                        ' error
                        '
                        Call Main.ReportError("There was a problem with the Dynamic Menu SiteTree. The SiteTree object timed out.")
                    ElseIf doc.parseError.errorCode <> 0 Then
                        '
                        ' error
                        '
                        Call Main.ReportError("There was a problem with the Dynamic Menu SiteTree. " & doc.parseError.reason)
                    Else
                        '
                        ' data is OK
                        '
                        renderedPageId = Main.renderedPageId
                        renderedSectionId = Main.renderedSectionId
                        If LCase(doc.documentElement.baseName) <> "menu" Then
                            '
                            ' error - Need a way to reach the user that submitted the file
                            '
                            Content.Add "<div class=ccError style=""margin:10px;padding:10px;background-color:white;"">There was a problem with the Setting Page you requested.</div>"
            Else
                            '
                            ' ----- Process Requests
                            '
                            menuId = kmaEncodeInteger(GetXMLAttribute(IsFound, doc.documentElement, "recordid", "0"))
                            menuHtmlId = "menu" & menuId
                            menuName = GetXMLAttribute(IsFound, doc.documentElement, "name", "")
                            StylePrefix = GetXMLAttribute(IsFound, doc.documentElement, "styleprefix", "ccFlyout")
                            HorizontalLayout = kmaEncodeBoolean(GetXMLAttribute(IsFound, doc.documentElement, "layout", "1") = 1)
                            FlyoutOnHover = kmaEncodeBoolean(GetXMLAttribute(IsFound, doc.documentElement, "flyoutonhover", "1") = 1)
                            If HorizontalLayout Then
                                FlyoutDirection = 2
                            Else
                                FlyoutDirection = 3
                            End If
                            'FlyoutDirection = kmaEncodeInteger(GetXMLAttribute(IsFound, doc.documentElement, "flyoutdirection", "2"))
                            'FlyoutDirection = kmaEncodeBoolean(GetXMLAttribute(IsFound, doc.documentElement, "flyoutdirection", "1") = 1)
                            'jsFile = GetXMLAttribute(IsFound, doc.documentElement, "jsFile", "")
                            'jsOnLoad = GetXMLAttribute(IsFound, doc.documentElement, "jsInHead", "")
                            'cssFile = GetXMLAttribute(IsFound, doc.documentElement, "cssFilename", "")
                            'useJsFlyoutCode = kmaEncodeBoolean(GetXMLAttribute(IsFound, doc.documentElement, "usejsflyoutcode", "false"))
                            '
                            ClickIndicator = "&nbsp;&#187;"
                            Execute = Execute & GetButtons(doc, StylePrefix, HorizontalLayout, FlyoutOnHover, FlyoutDirection, ClickIndicator, isEditing, renderedPageId, renderedSectionId, menuId, isAdvancedEditing, menuName, menuHtmlId)
                            'With doc.documentElement
                            '    For Each TopNode In .childNodes
                            '        If LCase(TopNode.baseName) = "node" Then
                            '            Execute = Execute & GetButtons(TopNode, StylePrefix, HorizontalLayout, FlyoutOnHover, FlyoutDirection, ClickIndicator)
                            '        End If
                            '    Next
                            'End With
                            '
                            ' Add top list
                            '
                            If Execute <> "" Then
                                With doc.documentElement
                                    nodeCnt = doc.documentElement.childNodes.length
                                    If nodeCnt > 0 Then
                                        For Ptr = 0 To nodeCnt - 1
                                Set node = doc.documentElement.childNodes(Ptr)
                                If LCase(node.baseName) = "jsfile" Then
                                                jsFile = GetXMLAttribute(IsFound, node, "name", "")
                                                If jsFile <> "" Then
                                                    Call Main.AddHeadScriptLink(Main.serverFilePath & jsFile, "menuing")
                                                End If
                                            End If
                                            If LCase(node.baseName) = "jsonload" Then
                                                jsOnLoad = node.Text
                                                If jsOnLoad <> "" Then
                                                    Call Main.AddOnLoadJavascript2(jsOnLoad, "menuing")
                                                End If
                                            End If
                                            If LCase(node.baseName) = "cssfile" Then
                                                cssFile = GetXMLAttribute(IsFound, node, "name", "")
                                                If cssFile <> "" Then
                                                    Call Main.AddStylesheetLink2(Main.serverFilePath & cssFile, "menuing")
                                                End If
                                            End If
                                        Next
                                    End If
                                End With
                                classAttr = GetXMLAttribute(IsFound, doc.documentElement, "topList", "")
                                If classAttr <> "" Then
                                    classAttr = " class=""" & Trim(classAttr) & """"
                                Else
                                    classAttr = ""
                                End If
                                Execute = "" _
                        & vbCrLf & vbTab & "<ul id=""" & menuHtmlId & "List""" & classAttr & ">" _
                        & kmaIndent(Execute) _
                        & vbCrLf & vbTab & "</ul>"
                            End If
                        End If
                    End If
                End If
                If Execute <> "" Then
                    '
                    ' add option wrapper(s)
                    '
                    If HorizontalLayout Then
                        Execute = "" _
                & vbCrLf & vbTab & "<div id=""" & menuHtmlId & """ class=""layoutHorizontal"">" _
                & kmaIndent(Execute) _
                & vbCrLf & vbTab & "</div>"
                    Else
                        Execute = "" _
                & vbCrLf & vbTab & "<div id=""" & menuHtmlId & """ class=""layoutVertical"">" _
                & kmaIndent(Execute) _
                & vbCrLf & vbTab & "</div>"
                    End If
                    '
                    ' add top wrapper
                    '
                    classAttr = GetXMLAttribute(IsFound, doc.documentElement, "topWrapper", "")
                    If classAttr <> "" Then
                        classAttr = " class=""" & Trim(classAttr) & """"
                    End If

                    Execute = "" _
            & vbCrLf & vbTab & "<div" & classAttr & ">" _
            & kmaIndent(Execute) _
            & vbCrLf & vbTab & "</div>"
                End If
            Catch ex As Exception
                errorReport(CP, ex, "execute")
                returnHtml = "Visual Studio 2005 Contensive Addon - Error response"
            End Try
            Return returnHtml
        End Function
        '
        '===============================================================================
        '   Returns the menu specified, if it is in local storage
        '       It also creates the menu data in a close string that is returned in GetMenuClose.
        '===============================================================================
        '
        Private Function GetButtons(doc As DOMDocument30, StylePrefix As String, HorizontalLayout As Boolean, FlyoutOnHover As Boolean, FlyoutDirection As Integer, ClickIndicator As String, isEditing As Boolean, renderedPageId As Integer, renderedSectionId As Integer, menuId As Integer, isAdvancedEditing As Boolean, menuName As String, topMenuHtmlId As String) As String
            On Error GoTo ErrorTrap
            '
            Dim classAttr As String
            Dim panelContainsActivePage As Boolean
            Dim menuPageId As Integer
            Dim aTag As String
            Dim Link As String
            Dim EntryPointer As Integer
            Dim panelHtmlId As String
            Dim MenuEntries As String
            Dim target As String
            Dim itemStyle As String
            Dim HotSpotHTML As String
            Dim HotSpotHTMLHover As String
            Dim FlyoutPanel As String
            Dim MouseClickCode As String
            Dim MouseOverCode As String
            Dim MouseOutCode As String
            Dim ImageID As String
            Dim JavaCode As String
            Dim PanelButtonCount As Integer
            Dim IsTextHotSpot As Boolean
            Dim IsFound As Boolean
            Dim MenuSectionID As Integer
            Dim Caption As String
            Dim CaptionImageSrc As String
            Dim ImageSrc As String
            Dim ImageOverSrc As String
            Dim node As IXMLDOMNode
            Dim ButtonCnt As Integer
            Dim ButtonPtr As Integer
            Dim topItemClass As String
            'Dim tierWrapperClass As String
            Dim tierListClass As String
            Dim tierItemClass As String
            Dim activeItemClass As String
            Dim firstItemClass As String
            Dim lastItemClass As String
            Dim flyoutParentClass As String
            Dim useJsFlyoutCode As Boolean
            Dim nodePtr As Integer
            Dim itemHtmlIdAttr As String
            Dim Copy As String
            Dim hoverItemClass As String
            '
            topItemClass = GetXMLAttribute(IsFound, doc.documentElement, "topItem", "")
            'tierWrapperClass = GetXMLAttribute(IsFound, doc.documentElement, "tierWrapper", "")
            tierListClass = GetXMLAttribute(IsFound, doc.documentElement, "tierList", "")
            tierItemClass = GetXMLAttribute(IsFound, doc.documentElement, "tierItem", "")
            activeItemClass = GetXMLAttribute(IsFound, doc.documentElement, "itemActive", "")
            firstItemClass = GetXMLAttribute(IsFound, doc.documentElement, "itemFirst", "")
            lastItemClass = GetXMLAttribute(IsFound, doc.documentElement, "itemLast", "")
            hoverItemClass = GetXMLAttribute(IsFound, doc.documentElement, "itemHover", "")
            flyoutParentClass = GetXMLAttribute(IsFound, doc.documentElement, "flyoutParent", "")
            useJsFlyoutCode = kmaEncodeBoolean(GetXMLAttribute(IsFound, doc.documentElement, "useJsFlyoutCode", ""))

            With doc.documentElement
                ButtonCnt = doc.documentElement.childNodes.length
                If ButtonCnt > 0 Then
                    nodePtr = 0
                    For ButtonPtr = 0 To ButtonCnt - 1
                Set node = doc.documentElement.childNodes(ButtonPtr)
                If LCase(node.baseName) = "node" Then
                            'Execute = Execute & GetButtons(TopNode, StylePrefix, HorizontalLayout, FlyoutOnHover, FlyoutDirection, ClickIndicator)
                            HotSpotHTML = ""
                            MouseClickCode = ""
                            MouseOverCode = ""
                            MouseOutCode = ""
                            ImageID = "img" & CStr(GetRandomInteger) & "s"
                            itemStyle = topItemClass
                            'panelHtmlId = CStr(GetRandomInteger)
                            Link = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "link", ""))
                            MenuSectionID = kmaEncodeInteger(GetXMLAttribute(IsFound, node, "SectionID", "menu"))
                            menuPageId = kmaEncodeInteger(GetXMLAttribute(IsFound, node, "PageID", "menu"))
                            panelHtmlId = topMenuHtmlId & "Page" & menuPageId & "List"
                            Caption = GetXMLAttribute(IsFound, node, "Caption", "menu")
                            CaptionImageSrc = GetXMLAttribute(IsFound, node, "CaptionImage", "")
                            itemHtmlIdAttr = " id=""" & topMenuHtmlId & "Page" & menuPageId & """"
                            If nodePtr = 0 Then
                                If itemStyle = "" Then
                                    itemStyle = firstItemClass
                                Else
                                    itemStyle = itemStyle & " " & firstItemClass
                                End If
                            End If
                            If (Not isEditing) And (nodePtr = (ButtonCnt - 1)) Then
                                If itemStyle = "" Then
                                    itemStyle = lastItemClass
                                Else
                                    itemStyle = itemStyle & " " & lastItemClass
                                End If
                            End If

                            If MenuSectionID = Main.renderedSectionId Then
                                If itemStyle = "" Then
                                    itemStyle = activeItemClass
                                Else
                                    itemStyle = itemStyle & " " & activeItemClass
                                End If
                                ImageSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageDownSrc", ""))
                                If ImageSrc = "" Then
                                    '
                                    ' no image down, if no over down, try image over
                                    '
                                    ImageSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageSrc", ""))
                                    ImageOverSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageDownOverSrc", ""))
                                    If (ImageOverSrc = "") Then
                                        ImageOverSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageOverSrc", ""))
                                    End If
                                Else
                                    '
                                    ' image down ok, if no over down, use image down
                                    '
                                    ImageOverSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageDownOverSrc", ""))
                                End If
                            Else
                                ImageSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageSrc", ""))
                                ImageOverSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageOverSrc", ""))
                            End If
                            If ImageSrc <> "" Then
                                '
                                ' Create hotspot from image
                                '
                                If ImageOverSrc = "" Then
                                    ImageOverSrc = ImageSrc
                                End If
                                HotSpotHTML = "<img src=""" & ImageSrc & """ border=""0"" alt=""" & Caption & """ ID=" & ImageID & " Name=" & ImageID & ">"
                                If ImageOverSrc <> "" Then
                                    JavaCode = JavaCode _
                            & "var " & ImageID & "n=new Image; " _
                            & ImageID & "n.src='" & ImageSrc & "'; " _
                            & "var " & ImageID & "o=new Image; " _
                            & ImageID & "o.src='" & ImageOverSrc & "'; "
                                    MouseOverCode = MouseOverCode & " document." & ImageID & ".src=" & ImageID & "o.src;"
                                    MouseOutCode = MouseOutCode & " document." & ImageID & ".src=" & ImageID & "n.src;"
                                End If
                            ElseIf Caption <> "" Then
                                '
                                ' Create hotspot text
                                '
                                If CaptionImageSrc <> "" Then
                                    HotSpotHTML = "<img alt=""" & Caption & """ src=""" & CaptionImageSrc & """ border=""0"">"
                                End If
                                HotSpotHTML = HotSpotHTML & Caption
                                IsTextHotSpot = True
                            Else
                                '
                                ' Create hotspot from name
                                '
                                HotSpotHTML = "Section " & MenuSectionID
                                IsTextHotSpot = True
                            End If
                            '
                            FlyoutPanel = GetPanel(node, panelHtmlId, "", StylePrefix, FlyoutOnHover, PanelButtonCount, ClickIndicator, tierListClass, tierItemClass, activeItemClass, firstItemClass, lastItemClass, renderedPageId, useJsFlyoutCode, panelContainsActivePage, menuId, flyoutParentClass, hoverItemClass, topMenuHtmlId)
                            'FlyoutPanel = GetPanel(node, panelHtmlId, "", StylePrefix, FlyoutOnHover, PanelButtonCount, ClickIndicator, tierWrapperClass, tierListClass, tierItemClass, activeItemClass, firstItemClass, lastItemClass, renderedPageId, useJsFlyoutCode, panelContainsActivePage, menuId, flyoutParentClass, hoverItemClass)
                            If (FlyoutPanel <> "") Then
                                itemStyle = Trim(itemStyle & " " & flyoutParentClass)
                            End If
                            '
                            ' do not fix the navigation menus by making an exception with the menu object. It is also used for Record Add tags, which need a flyout of 1.
                            '   make the exception in the menuing code above this.
                            '
                            If useJsFlyoutCode Then
                                If PanelButtonCount > 0 Then
                                    If FlyoutOnHover Then
                                        MouseOverCode = MouseOverCode & " lmHover(event,'" & hoverItemClass & "'); return lmOpenChildList(event, '" & panelHtmlId & "','" & FlyoutDirection & "','" & StylePrefix & "',true,'" & hoverItemClass & "');"
                                        MouseOutCode = MouseOutCode & " lmUnHover(event,'" & hoverItemClass & "');"
                                    Else
                                        If IsTextHotSpot Then
                                            HotSpotHTML = HotSpotHTML & ClickIndicator
                                        End If
                                        MouseClickCode = MouseClickCode & " return lmOpenChildList(event, '" & panelHtmlId & "','" & FlyoutDirection & "','" & StylePrefix & "',false,'" & hoverItemClass & "');"
                                        MouseOverCode = MouseOverCode & " lmButtonHover(event,'" & panelHtmlId & "','" & FlyoutDirection & "','false','" & hoverItemClass & "');"
                                    End If
                                Else
                                    If FlyoutOnHover Then
                                        MouseOverCode = MouseOverCode & " lmHover(event,'" & hoverItemClass & "');"
                                        MouseOutCode = MouseOutCode & " lmUnHover(event,'" & hoverItemClass & "');"
                                    Else
                                        If IsTextHotSpot Then
                                            HotSpotHTML = HotSpotHTML & ClickIndicator
                                        End If
                                        'MouseClickCode = MouseClickCode & " return lmOpenChildList(event, '" & panelHtmlId & "','" & FlyoutDirection & "','" & StylePrefix & "',false,'" & hoverItemClass & "');"
                                        MouseOverCode = MouseOverCode & " lmButtonHover(event,'" & panelHtmlId & "','" & FlyoutDirection & "','false','" & hoverItemClass & "');"
                                    End If
                                    '
                                    '        If FlyoutOnHover Then
                                    '            MouseOverCode = MouseOverCode & " lmSetHoverMode(1); return lmOpenChildList(event, '" & panelHtmlId & "','" & FlyoutDirection & "','" & StylePrefix & "','true');"
                                    '            MouseOutCode = MouseOutCode & " lmSetHoverMode(0);"
                                    '        Else
                                    '            If IsTextHotSpot Then
                                    '                HotSpotHTML = HotSpotHTML & ClickIndicator
                                    '            End If
                                    '            MouseClickCode = MouseClickCode & " return lmOpenChildList(event, '" & panelHtmlId & "','" & FlyoutDirection & "','" & StylePrefix & "');"
                                    '            MouseOverCode = MouseOverCode & " lmButtonHover(event,'" & panelHtmlId & "','" & FlyoutDirection & "','false');"
                                    '        End If
                                End If
                            End If
                            '
                            ' Convert js code to action
                            '
                            If MouseClickCode <> "" Then
                                MouseClickCode = " onClick=""" & MouseClickCode & """"
                            End If
                            If MouseOverCode <> "" Then
                                MouseOverCode = " onMouseOver=""" & MouseOverCode & """"
                            End If
                            If MouseOutCode <> "" Then
                                MouseOutCode = " onMouseOut=""" & MouseOutCode & """"
                            End If
                            '
                            itemStyle = Trim(itemStyle)
                            If itemStyle <> "" Then
                                classAttr = " class=""" & itemStyle & """"
                            Else
                                classAttr = ""
                            End If
                            '
                            aTag = "<a" & classAttr & " title=""" & Caption & """ href=""" & Link & """>" & HotSpotHTML & "</a>"
                            'aTag = "<a" & classAttr & " title=""" & Caption & """ href=""" & Link & """" & MouseOutCode & MouseOverCode & MouseClickCode & ">" & HotSpotHTML & "</a>"
                            If isEditing Then
                                aTag = Main.GetRecordEditLink("site sections", MenuSectionID) & aTag
                            End If
                            aTag = aTag & kmaIndent(FlyoutPanel)
                            If PanelButtonCount = 0 Then
                                GetButtons = GetButtons _
                            & vbCrLf & vbTab & "<li" & itemHtmlIdAttr & classAttr & MouseOutCode & MouseOverCode & MouseClickCode & ">" & aTag & "</li>"
                            Else
                                GetButtons = GetButtons _
                            & vbCrLf & vbTab & "<li" & itemHtmlIdAttr & classAttr & MouseOutCode & MouseOverCode & MouseClickCode & ">" & aTag _
                            & vbCrLf & vbTab & "</li>"
                            End If
                            nodePtr = nodePtr + 1
                        End If
                    Next
                    If isEditing Then
                        itemStyle = topItemClass
                        If nodePtr = 0 Then
                            If itemStyle = "" Then
                                itemStyle = firstItemClass
                            Else
                                itemStyle = firstItemClass & " " & itemStyle
                            End If
                        End If
                        If Not isAdvancedEditing Then
                            If itemStyle = "" Then
                                itemStyle = lastItemClass
                            Else
                                itemStyle = lastItemClass & " " & itemStyle
                            End If
                        End If
                        If Trim(itemStyle) <> "" Then
                            classAttr = " class=""" & Trim(itemStyle) & """"
                        Else
                            classAttr = ""
                        End If

                        Dim copyTag As String
                        Dim editLink As String

                        editLink = Main.SiteProperty_AdminURL & "?cid=" & Main.GetContentID("site sections") & "&amp;af=4&amp;aa=2&amp;ad=1&amp;wc=menuId%3D" & menuId
                        Copy = "" _
                    & "<span style=""white-space:nowrap;"" class=""ccRecordLinkCon"">" _
                    & "<a class=""ccRecordEditLink "" href=""" & editLink & """ tabindex=""-1"">" _
                    & "<img width=""18"" height=""22"" border=""0"" title=""Add Section"" alt=""Add Section"" src=""/ccLib/images/IconContentAdd.gif"">" _
                    & "</a>" _
                    & "</span>" _
                    & "<a class=""ccRecordEditLink " & itemStyle & """ href=""" & editLink & """ tabindex=""-1"" style=""padding-left:20px;"">" _
                    & "Add&nbsp;Section" _
                    & "</a>" _
                    & ""
                        GetButtons = GetButtons _
                    & vbCrLf & vbTab & "<li" & classAttr & ">" _
                    & kmaIndent(Copy) _
                    & vbCrLf & vbTab & "</li>"
                        '                GetButtons = GetButtons _
                        '                    & vbCrLf & vbTab & "<li" & classAttr & ">" _
                        '                    & kmaIndent(Main.GetRecordAddLink("site sections", "menuId=" & menuId) & "<a href=""#""" & classAttr & ">Add Section</a>") _
                        '                    & vbCrLf & vbTab & "</li>"
                        If isAdvancedEditing Then
                            itemStyle = topItemClass
                            If itemStyle = "" Then
                                itemStyle = lastItemClass
                            Else
                                itemStyle = lastItemClass & " " & itemStyle
                            End If
                            If Trim(itemStyle) <> "" Then
                                classAttr = " class=""" & Trim(itemStyle) & """"
                            Else
                                classAttr = ""
                            End If
                            Caption = "Edit Menu [" & menuName & "]"
                            Caption = Replace(Caption, " ", "&nbsp;")
                            'Caption = "<span style=""padding-left:20px;"">" & Caption & "</span>"
                            editLink = Main.SiteProperty_AdminURL & "?cid=" & Main.GetContentID("dynamic menus") & "&af=4&aa=2&ad=1&id=" & menuId
                            Copy = "" _
                        & "<span style=""white-space:nowrap;"" class=""ccRecordLinkCon"">" _
                        & "<a class=""ccRecordEditLink "" href=""" & editLink & """ tabindex=""-1"">" _
                        & "<img width=""18"" height=""22"" border=""0"" title=""Edit Menu"" alt=""Edit Menu"" src=""/ccLib/images/IconContentEdit.gif"">" _
                        & "</a>" _
                        & "</span>" _
                        & "<a class=""ccRecordEditLink " & itemStyle & """ href=""" & editLink & """ tabindex=""-1"" style=""padding-left:20px;"">" _
                        & Caption _
                        & "</a>" _
                        & ""
                            GetButtons = GetButtons _
                        & vbCrLf & vbTab & "<li" & classAttr & ">" _
                        & kmaIndent(Copy) _
                        & vbCrLf & vbTab & "</li>"
                        End If
                    End If
                    JavaCode = "" _
                & JavaCode _
                & "document.addEventListener(""mousedown"", lmPageClick);" _
                & ""
                    '            JavaCode = "" _
                    '                & JavaCode _
                    '                & "document.addEventListener(""mousedown"", function(){lmPageClick(event," & hoverItemClass & ")}, true);" _
                    '                & ""
                End If
            End With
            '
            ' Add in the inline java code if required
            '
            If JavaCode <> "" Then
                GetButtons = "" _
        & vbCrLf & vbTab & "<SCRIPT language=javascript type=text/javascript>" & JavaCode & "</script>" _
        & GetButtons
            End If
            Exit Function
            '
ErrorTrap:
            'Call HandleClassError("GetButtons", Err.Number, Err.Source, Err.Description)
        End Function
        '
        '===============================================================================
        '   Gets the Menu Branch for the Default Menu
        '===============================================================================
        '
        Private Function GetPanel(ParentNode As IXMLDOMNode, panelHtmlId As String, UsedEntries As String, StyleSheetPrefix As String, FlyoutHover As Boolean, PanelButtonCnt As Integer, ClickIndicator As String, tierListClass As String, tierItemClass As String, activeItemClass As String, firstItemClass As String, lastItemClass As String, renderedPageId As Integer, useJsFlyoutCode As Boolean, ByRef return_PanelContainsActivePage As Boolean, menuId As Integer, flyoutParentClass As String, hoverItemClass As String, topMenuHtmlId As String) As String
            'Private Function GetPanel(ParentNode As IXMLDOMNode, panelHtmlId As String, UsedEntries As String, StyleSheetPrefix As String, FlyoutHover As Boolean, PanelButtonCnt as Integer, ClickIndicator As String, tierWrapperClass As String, tierListClass As String, tierItemClass As String, activeItemClass As String, firstItemClass As String, lastItemClass As String, renderedPageId as Integer, useJsFlyoutCode As Boolean, ByRef return_PanelContainsActivePage As Boolean, menuId as Integer, flyoutParentClass As String, hoverItemClass As String) As String
            On Error GoTo ErrorTrap
            '
            Dim thisPanelContainsActivePage As Boolean
            Dim childPanelContainsActivePage As Boolean
            '
            Dim mouseOverAttr As String
            Dim classAttr As String
            'Dim wrapperClass As String
            Dim listClass As String
            Dim ChildButtonCnt As Integer
            Dim aTag As String
            Dim EntryPointer As Integer
            Dim iUsedEntries As String
            Dim SubMenuName As String
            Dim SubMenuCount As Integer
            Dim target As String
            Dim SubMenus As String
            Dim PanelChildren As String
            Dim PanelButtons As String
            Dim PanelButtonStyle As String
            Dim HotSpotHTML As String
            Dim node As IXMLDOMNode
            Dim NewWindow As Boolean
            Dim IsFound As Boolean
            Dim Link As String
            Dim MenuSectionID As Integer
            Dim menuPageId As Integer
            Dim Caption As String
            Dim ImageSrc As String
            Dim ImageOverSrc As String
            Dim CaptionImageSrc As String
            Dim childPanelHtmlId As String
            'Dim PanelButtonClass As String
            Dim PanelButtonPtr As Integer
            Dim itemClass As String
            Dim itemHtmlIdAttr As String
            '
            PanelButtonCnt = ParentNode.childNodes.length
            thisPanelContainsActivePage = False
            If PanelButtonCnt > 0 Then
                For PanelButtonPtr = 0 To PanelButtonCnt - 1
    'For Each Node In ParentNode.childNodes
        Set node = ParentNode.childNodes(PanelButtonPtr)
        If LCase(node.baseName) = "node" Then
                        itemClass = tierItemClass
                        If PanelButtonPtr = 0 Then
                            itemClass = Trim(firstItemClass & " " & itemClass)
                        End If
                        If PanelButtonPtr = (PanelButtonCnt - 1) Then
                            itemClass = Trim(lastItemClass & " " & itemClass)
                        End If
                        'childPanelHtmlId = CStr(Main.GetRandomLong)
                        'PanelButtonStyle = "PanelButton"
                        NewWindow = kmaEncodeBoolean(GetXMLAttribute(IsFound, node, "NewWindow", "menu"))
                        Link = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "link", ""))
                        MenuSectionID = kmaEncodeInteger(GetXMLAttribute(IsFound, node, "SectionID", "menu"))
                        menuPageId = kmaEncodeInteger(GetXMLAttribute(IsFound, node, "pageID", "menu"))
                        childPanelHtmlId = topMenuHtmlId & "Page" & menuPageId & "List"
                        Caption = GetXMLAttribute(IsFound, node, "Caption", "menu")
                        CaptionImageSrc = GetXMLAttribute(IsFound, node, "CaptionImage", "menu")
                        '
                        itemHtmlIdAttr = " id=""" & topMenuHtmlId & "Page" & menuPageId & """"
                        childPanelContainsActivePage = False
                        PanelChildren = GetPanel(node, childPanelHtmlId, iUsedEntries, StyleSheetPrefix, FlyoutHover, ChildButtonCnt, ClickIndicator, tierListClass, tierItemClass, activeItemClass, firstItemClass, lastItemClass, renderedPageId, useJsFlyoutCode, childPanelContainsActivePage, menuId, flyoutParentClass, hoverItemClass, topMenuHtmlId)
                        If (PanelChildren <> "") And (flyoutParentClass <> "") Then
                            itemClass = Trim(itemClass & " " & flyoutParentClass)
                        End If
                        If childPanelContainsActivePage Then
                            thisPanelContainsActivePage = True
                        End If
                        '
                        If childPanelContainsActivePage Or (renderedPageId = menuPageId) Then
                            thisPanelContainsActivePage = True
                            itemClass = activeItemClass & " " & itemClass
                            ImageSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageDownScr", ""))
                            If ImageSrc = "" Then
                                '
                                ' no image down, if no over down, try image over
                                '
                                ImageSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageSrc", ""))
                                ImageOverSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageDownOverSrc", ""))
                                If (ImageOverSrc = "") Then
                                    ImageOverSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageOverSrc", ""))
                                End If
                            Else
                                '
                                ' image down ok, if no over down, use image down
                                '
                                ImageOverSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageDownOverSrc", ""))
                            End If
                        Else
                            ImageSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageSrc", ""))
                            ImageOverSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, node, "ImageOverSrc", ""))
                        End If
                        If ImageOverSrc = "" Then
                            ImageOverSrc = ImageSrc
                        End If
                        '            If MenuSectionID = Main.renderedSectionId Then
                        '                ImageSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, Node, "ImageDownScr", ""))
                        '                ImageOverSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, Node, "ImageDownOverSrc", ""))
                        '            Else
                        '                ImageSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, Node, "ImageSrc", ""))
                        '                ImageOverSrc = kmaEncodeHTML(GetXMLAttribute(IsFound, Node, "ImageOverSrc", ""))
                        '            End If

                        target = ""
                        If NewWindow Then
                            target = " target=""_blank"""
                        End If
                        If ImageSrc <> "" Then
                            HotSpotHTML = "<img src=""" & ImageSrc & """ border=""0"" alt=""" & Caption & """>"
                        Else
                            HotSpotHTML = Caption
                        End If
                        If Trim(itemClass) <> "" Then
                            classAttr = " class=""" & Trim(itemClass) & """"
                        Else
                            classAttr = ""
                        End If
                        '            childPanelContainsActivePage = False
                        '            PanelChildren = GetPanel(Node, childPanelHtmlId, iUsedEntries, StyleSheetPrefix, FlyoutHover, ChildButtonCnt, ClickIndicator, tierWrapperClass, tierListClass, tierItemClass, activeItemClass, firstItemClass, lastItemClass, renderedPageId, childPanelContainsActivePage)
                        '            If childPanelContainsActivePage Then
                        '                thisPanelContainsActivePage = True
                        '            End If
                        If PanelChildren = "" Then
                            If Link = "" Then
                                '
                                ' ----- no link and no child panel
                                '
                                'PanelButtons = PanelButtons & "<SPAN" & classAttr & ">" & HotSpotHTML & "</SPAN>"
                            Else
                                '
                                ' ----- Link but no child panel
                                '
                                aTag = "<a title=""" & Caption & """" & classAttr & "  href=""" & kmaEncodeHTML(Link) & """" & target & ">" & HotSpotHTML & "</a>"
                            End If
                            If useJsFlyoutCode Then
                                GetPanel = GetPanel & vbCrLf & vbTab & "<li" & itemHtmlIdAttr & classAttr & "  onmouseover=""lmHover(event,'" & hoverItemClass & "');"" onmouseout=""lmUnHover(event,'" & hoverItemClass & "');"">" & aTag & "</li>"
                            Else
                                GetPanel = GetPanel & vbCrLf & vbTab & "<li" & itemHtmlIdAttr & classAttr & " >" & aTag & "</li>"
                            End If
                            '                If Link = "" Then
                            '                    '
                            '                    ' ----- no link and no child panel
                            '                    '
                            '                Else
                            '                    '
                            '                    ' ----- Link but no child panel
                            '                    '
                            '                    If useJsFlyoutCode Then
                            '                        aTag = "<a title=""" & Caption & """" & classAttr & "  href=""" & kmaEncodeHTML(Link) & """" & Target & "  onmouseover=""lmHover(event,'" & hoverItemClass & "');"" onmouseout=""lmUnHover(event,'" & hoverItemClass & "');"">" & HotSpotHTML & "</a>"
                            '                    Else
                            '                        aTag = "<a title=""" & Caption & """" & classAttr & "  href=""" & kmaEncodeHTML(Link) & """" & Target & ">" & HotSpotHTML & "</a>"
                            '                    End If
                            '                End If
                            '                GetPanel = GetPanel & vbCrLf & vbTab & "<li" & itemHtmlIdAttr & classAttr & " >" & aTag & "</li>"
                        Else
                            If Link = "" Then
                                '
                                ' ----- Child Panel and no link, block the href so menu "parent" buttons will not be clickable
                                '
                                aTag = "<a title=""" & Caption & """" & classAttr & " onclick=""return false;"" href=""#""" & target & ">" & HotSpotHTML & ClickIndicator & "</a>"
                                '                    If Not useJsFlyoutCode Then
                                '                        aTag = "<a title=""" & Caption & """" & classAttr & " onclick=""return false;"" href=""#""" & Target & ">" & HotSpotHTML & ClickIndicator & "</a>"
                                '                    Else
                                '                        If FlyoutHover Then
                                '                            aTag = "<a title=""" & Caption & """" & classAttr & " onmouseover=""lmHover(event,'" & hoverItemClass & "'); lmListItemHover(event,'" & childPanelHtmlId & "','" & StyleSheetPrefix & "','" & hoverItemClass & "','" & tierItemClass & "','" & tierListClass & "');"" onmouseout=""lmUnHover(event,'" & hoverItemClass & "');"" onclick=""return false;"" href=""#""" & Target & ">" & HotSpotHTML & ClickIndicator & "</a>"
                                '                        Else
                                '                            aTag = "<a title=""" & Caption & """" & classAttr & " onmouseover=""lmListItemHover(event,'" & childPanelHtmlId & "','" & StyleSheetPrefix & "','" & hoverItemClass & "','" & tierItemClass & "','" & tierListClass & "');"" onclick=""return false;"" href=""#""" & Target & ">" & HotSpotHTML & ClickIndicator & "</a>"
                                '                        End If
                                '                    End If
                                ' onmouseover="lmListHover(event,'ccFlyout')
                            Else
                                '
                                ' ----- Child Panel and a link
                                '
                                aTag = "<a title=""" & Caption & """" & classAttr & " href=""" & kmaEncodeHTML(Link) & """" & target & ">" & HotSpotHTML & ClickIndicator & "</a>"
                                '                    If FlyoutHover Then
                                '                        aTag = "<a title=""" & Caption & """" & classAttr & " href=""" & kmaEncodeHTML(Link) & """" & Target & ">" & HotSpotHTML & ClickIndicator & "</a>"
                                '                    Else
                                '                        aTag = "<a title=""" & Caption & """" & classAttr & " href=""" & kmaEncodeHTML(Link) & """" & Target & ">" & HotSpotHTML & ClickIndicator & "</a>"
                                '                        'ATag = "<a title=""" & Caption & """" & classAttr & "  onmouseover=""lmListItemHover(event,'" & childPanelHtmlId & "','" & StyleSheetPrefix & "');"" href=""" & kmaEncodeHTML(Link) & """" & Target & ">" & HotSpotHTML & ClickIndicator & "</a>"
                                '                    End If
                            End If
                            If Not useJsFlyoutCode Then
                                GetPanel = GetPanel _
                        & vbCrLf & vbTab & "<li" & itemHtmlIdAttr & classAttr & ">" & aTag _
                        & kmaIndent(PanelChildren) _
                        & vbCrLf & vbTab & "</li>"
                            Else
                                If FlyoutHover Then
                                    GetPanel = GetPanel _
                            & vbCrLf & vbTab & "<li" & itemHtmlIdAttr & classAttr & " onmouseover=""lmHover(event,'" & hoverItemClass & "'); lmListItemHover(event,'" & childPanelHtmlId & "','" & StyleSheetPrefix & "','" & hoverItemClass & "','" & tierItemClass & "','" & tierListClass & "');""  onmouseout=""lmUnHover(event,'" & hoverItemClass & "');"" >" & aTag _
                            & kmaIndent(PanelChildren) _
                            & vbCrLf & vbTab & "</li>"
                                Else
                                    GetPanel = GetPanel _
                            & vbCrLf & vbTab & "<li" & itemHtmlIdAttr & classAttr & " onmouseover=""lmHover(event,'" & hoverItemClass & "'); lmListItemHover(event,'" & childPanelHtmlId & "','" & StyleSheetPrefix & "','" & hoverItemClass & "','" & tierItemClass & "','" & tierListClass & "');"" >" & aTag _
                            & kmaIndent(PanelChildren) _
                            & vbCrLf & vbTab & "</li>"
                                End If
                            End If
                        End If
                    End If
                Next
                '
                '
                '
                If GetPanel <> "" Then
                    '
                    ' ----- If panel buttons are returned, wrap them in a UL
                    '
                    listClass = tierListClass
                    'wrapperClass = tierWrapperClass
                    If thisPanelContainsActivePage Then
                        listClass = activeItemClass & " " & listClass
                        'wrapperClass = activeItemClass & " " & wrapperClass
                    End If
                    '
                    If Trim(listClass) = "" Then
                        classAttr = ""
                    Else
                        classAttr = " class=""" & Trim(listClass) & """"
                    End If
                    If Not useJsFlyoutCode Then
                        mouseOverAttr = ""
                    Else
                        mouseOverAttr = " onmouseover=""lmListHover(event,'" & StyleSheetPrefix & "','" & hoverItemClass & "');"""
                    End If
                    GetPanel = "" _
            & vbCrLf & vbTab & vbTab & "<ul id=""" & panelHtmlId & """" & classAttr & mouseOverAttr & ">" _
            & kmaIndent(GetPanel) _
            & vbCrLf & vbTab & vbTab & "</ul>" _
            & ""
                    '        '
                    '        If Trim(wrapperClass) = "" Then
                    '            classAttr = ""
                    '        Else
                    '            classAttr = " class=""" & wrapperClass & """"
                    '        End If
                    '        If Not useJsFlyoutCode Then
                    '            mouseOverAttr = ""
                    '        Else
                    '            mouseOverAttr = " onmouseover=""lmListHover(event,'" & StyleSheetPrefix & "');"""
                    '        End If
                    '        GetPanel = "" _
                    '            & vbCrLf & vbTab & "<div" & classAttr & " id=""" & panelHtmlId & """" & mouseOverAttr & ">" _
                    '            & kmaIndent(GetPanel) _
                    '            & vbCrLf & vbTab & "</div>" _
                    '            & ""
                End If
                '
                return_PanelContainsActivePage = thisPanelContainsActivePage
            End If
            '
            Exit Function
            '
ErrorTrap:
            'Call HandleClassError("GetPanel", Err.Number, Err.Source, Err.Description)
        End Function
        '
        '========================================================================
        ' ----- Get an XML nodes attribute based on its name
        '========================================================================
        '
        Private Function GetXMLAttribute(found As Boolean, node As IXMLDOMNode, Name As String, DefaultIfNotFound As String) As String
            On Error GoTo ErrorTrap
            '
            Dim NodeAttribute As IXMLDOMAttribute
            Dim REsultNode As IXMLDOMNode
            Dim UcaseName As String
            '
            found = False
    Set REsultNode = node.Attributes.getNamedItem(Name)
    If (REsultNode Is Nothing) Then
                UcaseName = UCase(Name)
                For Each NodeAttribute In node.Attributes
                    If UCase(NodeAttribute.NodeName) = UcaseName Then
                        GetXMLAttribute = NodeAttribute.nodeValue
                        found = True
                        Exit For
                    End If
                Next
            Else
                GetXMLAttribute = REsultNode.nodeValue
                found = True
            End If
            If Not found Then
                GetXMLAttribute = DefaultIfNotFound
            End If
            Exit Function
            '
            ' ----- Error Trap
            '
ErrorTrap:
            'HandleError
        End Function

    End Class
End Namespace
