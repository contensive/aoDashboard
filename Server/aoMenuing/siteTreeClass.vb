
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Contensive.Addons.Menuing
    Public Class siteTreeClass

        '
        '==============================================================================
        '
        '   Creates custom menus
        '   Stores caches of the menus
        '   Stores the menu data, and can generate different kind
        '
        '==============================================================================
        '
        Const MenuStyleFlyoutDown = 4
        Const MenuStyleFlyoutRight = 5
        Const MenuStyleFlyoutUp = 6
        Const MenuStyleFlyoutLeft = 7
        Const MenuStyleHoverDown = 8
        Const MenuStyleHoverRight = 9
        Const MenuStyleHoverUp = 10
        Const MenuStyleHoverLeft = 11
        '
        ' ----- Each menu item has an MenuEntry
        '
        Private Class NodeStrucType
            Public sectionId As Integer
            Public PageID As Integer
            Public Caption As String           ' What is displayed for this entry (does not need to be unique)
            Public CaptionImage As String      ' If present, this is an image in front of the caption
            Public Name As String              ' Unique name for this entry
            Public ParentName As String        ' Unique name of the parent entry
            Public Link As String              ' URL
            Public Image As String             ' Image
            Public ImageOver As String         ' Image Over
            Public ImageDown As String         ' Image Over
            Public ImageDownOver As String     ' Image Over
            Public NewWindow As Boolean        ' True opens link in a new window
            Public Overview As String
        End Class
        Public Class menuDataClass
            Public recordId As Integer
            Public name As String
            Public depth As Integer
            Public layout As Integer
            Public delimiter As String
            Public flyoutonhover As String
            Public flyoutdirection As String
            Public styleprefix As String
            Public topWrapper As String
            Public topList As String
            Public topItem As String
            Public tierList As String
            Public tierItem As String
            Public itemActive As String
            Public itemFirst As String
            Public itemLast As String
            Public itemHover As String
            Public flyoutParent As String
            Public useJsFlyoutCode As String
        End Class
        '
        ' ----- Local storage
        '
        'Private iMenuFilePath As String
        '
        ' ----- Menu Entry storage
        '
        Private NodeStrucCount As Integer          ' Count of Menus in the object
        Private NodeStrucSize As Integer
        Private NodeStruc() As NodeStrucType
        Private NodeStrucUsedNameList As String
        '
        'Private iTreeCount as Integer          ' Count of Tree Menus for this instance
        'Private iMenuCloseString As String  ' String returned for closing menus
        '
        'Private NodeStrucUsedNameList As String       ' String of EntryNames that have been used (for unique test)
        'Private EntryIndexName As FastIndexClass
        'Private EntryIndexID As FastIndexClass
        '
        ' ----- RollOverFlyout storage
        '
        'Private MenuFlyoutNamePrefix As String    ' Random prefix added to element IDs to avoid namespace collision
        'Private MenuFlyoutIcon_Local As String      ' string used to mark a button that has a non-hover flyout
        'Private thisandthat As String

        '
        '========================================================================
        '
        '========================================================================
        '
        Private Main As Object
        Private Csv As Object
        Private pccLocal As Variant
        'Private MenuSystem As SiteTreeNavClass
        'Private MenuSystemCloseCount as Integer
        'Private Main As Object
        '
        '=================================================================================
        '   Execute Method, v3.4 Interface
        '=================================================================================
        '
        Public Function getMenuData(cp As CPBaseClass) As menuDataClass
            Dim menu As New menuDataClass
            Try
                Dim sqlCriteria As String = ""
                menu.name = cp.Doc.GetText("create New Menu")
                If (String.IsNullOrEmpty(menu.name)) Then
                    menu.name = cp.Doc.GetText("menunew")
                End If
                If (Not String.IsNullOrEmpty(menu.name)) Then
                    sqlCriteria &= "(name=" & cp.Db.EncodeSQLText(menu.name) & ")"
                Else
                    menu.recordId = cp.Doc.GetInteger("use existing menu")
                    If (menu.recordId > 0) Then
                        sqlCriteria &= "(id=" & cp.Db.EncodeSQLNumber(menu.recordId) & ")"
                    Else
                        sqlCriteria &= "(name='default')"
                    End If
                End If
                '
                ' disable cache because
                '
                Execute = Main.ReadBake(cacheName)
                If Execute = "" Then
                    '
                    Call Main.GetPCCPtr(0, False, False)
                    pccLocal = Main.PCC
                    UseContentWatchLink = False
                    DefaultTemplateLink = Main.ServerAppRootPath & Main.ServerPage
                    '
                    ' Check for MenuID - if present, arguments are in the Dynamic Menu content - else it is old, and they are in the AddonOptionString
                    '
                    '
                    ' Open the Menu
                    '
                    CS = Main.OpenCSContent("Dynamic Menus", sqlCriteria, "ID")
                    If Not Main.IsCSOK(CS) Then
                        Call Main.CloseCS(CS)
                        menuId = Main.VerifyDynamicMenu(menuName)
                        CS = Main.OpenCSContentRecord("Dynamic Menus", menuId)
                    End If
                    If Main.IsCSOK(CS) Then
                        '
                        ' setup arguments from Content
                        '
                        innerNodes = ""
                        menuId = Main.GetCSInteger(CS, "ID")
                        menuName = Main.GetCSText(CS, "name")
                        MenuDepth = Main.GetCSInteger(CS, "Depth")
                        MenuStylePrefix = Main.GetCSText(CS, "StylePrefix")
                        menuFlyoutOnHover = Main.GetCSBoolean(CS, "flyoutOnHover")
                        menuFlyoutDirection = Main.GetCSInteger(CS, "flyoutDirection")
                        If menuFlyoutOnHover Then
                            Select Case menuFlyoutDirection
                                Case 1
                                    MenuStyle = MenuStyleHoverUp
                                Case 3
                                    MenuStyle = MenuStyleHoverRight
                                Case 4
                                    MenuStyle = MenuStyleHoverLeft
                                Case Else
                                    MenuStyle = MenuStyleHoverDown
                            End Select
                        Else
                            Select Case menuFlyoutDirection
                                Case 1
                                    MenuStyle = MenuStyleFlyoutUp
                                Case 3
                                    MenuStyle = MenuStyleFlyoutRight
                                Case 4
                                    MenuStyle = MenuStyleFlyoutLeft
                                Case Else
                                    MenuStyle = MenuStyleFlyoutDown
                            End Select
                        End If
                        '
                        jsFilename = Main.GetCSText(CS, "JSFilename")
                        If jsFilename <> "" Then
                            innerNodes = innerNodes & vbCrLf & vbTab & "<jsFile name=""" & jsFilename & """/>"
                        End If
                        '
                        jsInHead = Main.GetCSText(CS, "JavaScriptOnLoad")
                        If jsInHead <> "" Then
                            innerNodes = innerNodes & vbCrLf & vbTab & "<jsOnLoad><![CDATA[" & jsInHead & "]]></jsOnLoad>"
                        End If
                        '
                        cssLegacyFile = Main.GetCSText(CS, "stylesFilename")
                        If cssLegacyFile = "" Then
                            defaultStyles = "/* default styles added " & Now() & "*/" & vbCrLf & Main.ReadVirtualFile("aoMenuing\defaultLegacyStyles.css")
                            Call Main.SetCS(CS, "stylesFilename", defaultStyles)
                            cssLegacyFile = Main.GetCSText(CS, "stylesFilename")
                        End If
                        innerNodes = innerNodes & vbCrLf & vbTab & "<cssLegacyFile name=""" & cssLegacyFile & """/>"
                        '
                        cssFile = Main.GetCSText(CS, "listStylesFilename")
                        If cssFile = "" Then
                            defaultStyles = "/* default styles added " & Now() & "*/" & vbCrLf & Main.ReadVirtualFile("aoMenuing\defaultListStyles.css")
                            Call Main.SetCS(CS, "listStylesFilename", defaultStyles)
                            cssFile = Main.GetCSText(CS, "listStylesFilename")
                        End If
                        innerNodes = innerNodes & vbCrLf & vbTab & "<cssFile name=""" & cssFile & """/>"
                        '
                        sectionMenu = getSectionMenu(MenuDepth, MenuStyle, MenuStylePrefix, DefaultTemplateLink, menuId, menuName, UseContentWatchLink)


                        Execute = vbCrLf & vbTab & "<menu" _
                    & " recordid=""" & menuId & """" _
                    & " name=""" & kmaEncodeHTML(menuName) & """" _
                    & " depth=""" & MenuDepth & """" _
                    & " layout=""" & Main.GetCSInteger(CS, "layout") & """" _
                    & " delimiter=""" & kmaEncodeHTML(Main.GetCSText(CS, "Delimiter")) & """" _
                    & " flyoutonhover=""" & Main.GetCSInteger(CS, "FlyoutOnHover") & """" _
                    & " flyoutdirection=""" & Main.GetCSInteger(CS, "FlyoutDirection") & """" _
                    & " styleprefix=""" & kmaEncodeHTML(MenuStylePrefix) & """" _
                    & " topWrapper=""" & kmaEncodeHTML(Main.GetCSText(CS, "classTopWrapper")) & """" _
                    & " topList=""" & kmaEncodeHTML(Main.GetCSText(CS, "classTopList")) & """" _
                    & " topItem=""" & kmaEncodeHTML(Main.GetCSText(CS, "classTopItem")) & """" _
                    & " tierList=""" & kmaEncodeHTML(Main.GetCSText(CS, "classTierList")) & """" _
                    & " tierItem=""" & kmaEncodeHTML(Main.GetCSText(CS, "classTierItem")) & """" _
                    & " itemActive=""" & kmaEncodeHTML(Main.GetCSText(CS, "classItemActive")) & """" _
                    & " itemFirst=""" & kmaEncodeHTML(Main.GetCSText(CS, "classItemFirst")) & """" _
                    & " itemLast=""" & kmaEncodeHTML(Main.GetCSText(CS, "classItemLast")) & """" _
                    & " itemHover=""" & kmaEncodeHTML(Main.GetCSText(CS, "classItemHover")) & """" _
                    & " flyoutParent=""" & kmaEncodeHTML(Main.GetCSText(CS, "classFlyoutParent")) & """" _
                    & " useJsFlyoutCode=""" & Main.GetCSInteger(CS, "useJsFlyoutCode") & """" _
                    & " >" _
                & innerNodes _
                & kmaIndent(sectionMenu) _
                & vbCrLf & vbTab & "</menu>"
                        '& " tierWrapper=""" & kmaEncodeHTML(Main.GetCSText(CS, "classTierWrapper")) & """"
                    End If
                    Call Main.CloseCS(CS)
                    Call Main.SaveBake(cacheName, Execute, "page content,site sections,dynamic menus")
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return menu
        End Function
        '
        '=============================================================================
        '   Get the Section Menu
        '   MenuName blank reverse menu to legacy mode (all sections on menu)
        '=============================================================================
        '
        Friend Function getSectionMenu(DepthLimit As Integer, MenuStyle As Integer, StyleSheetPrefix As String, DefaultTemplateLink As String, menuId As Integer, menuName As String, UseContentWatchLink As Boolean) As String
            On Error GoTo ErrorTrap
            '
            Dim menucid As Integer
            Dim AdminURL As String
            Dim sectioncid As Integer
            Dim IsEditingSections As Boolean
            Dim PageActive As Boolean
            Dim TCPtr As Integer
            Dim PCCPtr As Integer
            Dim RootPageID As Integer
            Dim CSSections As Integer
            Dim CSTemplates As Integer
            Dim CSPage As Integer
            Dim SectionName As String
            Dim TemplateID As Integer
            Dim ContentID As Integer
            Dim ContentName As String
            Dim PageList_ParentBranchPointer As Integer
            Dim Link As String
            Dim sectionId As Integer
            Dim AuthoringTag As String
            Dim MenuImage As String
            Dim MenuImageOver As String
            Dim MenuImageDown As String
            Dim MenuImageDownOver As String
            'Dim SectionCount as Integer
            Dim LandingLink As String
            Dim MenuString As String
            Dim SectionCaption As String
            Dim SectionTemplateID As Integer
            Dim Criteria As String
            Dim SelectFieldList As String
            Dim ShowHiddenMenu As Boolean
            Dim HideMenu As Boolean
            Dim BuildVersion As String
            Dim PageContentCID As Integer
            Dim BlockPage As Boolean
            Dim BlockSection As Boolean
            Dim SQL As String
            Dim IsAllSectionsMenuMode As Boolean
            Dim isQuickEditingPageContent As Boolean
            Dim hint As String
            '
            ' fixed? - !! Problem: new upgraded site with old menu object (MenuName=""). We take the third option here, but later in the
            '   routine we use RootPageID because the check is on version only
            '
            BuildVersion = Main.SiteProperty_BuildVersion
            IsEditingSections = Main.isEditing("Site Sections")
            IsAllSectionsMenuMode = (menuName = "")
            PageContentCID = Main.GetContentID("Page Content")
            isQuickEditingPageContent = Main.IsQuickEditing("page content")
            '
            ' skip version check and only install on 4.0.000+
            '
            SelectFieldList = "ID, Name,TemplateID,ContentID,Caption,MenuImageFilename,MenuImageOverFilename,MenuImageDownFilename,MenuImageDownOverFilename,HideMenu,BlockSection,RootPageID"
            ShowHiddenMenu = Main.IsEditingAnything()
            If IsAllSectionsMenuMode Then
                '
                ' Section/Page connection at RootPageID, show all sections
                '
                CSSections = Main.OpenCSContent("Site Sections", , , , , , SelectFieldList)
            Else
                '
                ' Section/Page connection at RootPageID, only show sections connected to the menu
                '
                SQL = "Select Distinct S.ID" _
            & " from ((ccSections S" _
            & " left join ccDynamicMenuSectionRules R on R.SectionID=S.ID)" _
            & " left join ccDynamicMenus M on M.ID=R.DynamicMenuID)" _
            & " where M.ID=" & menuId
                Criteria = "ID in (" & SQL & ")"
                CSSections = Main.OpenCSContent("Site Sections", Criteria, , , , , SelectFieldList)
            End If
            '    If (BuildVersion > "3.3.612") Then
            '        SelectFieldList = "ID, Name,TemplateID,ContentID,Caption,MenuImageFilename,MenuImageOverFilename,MenuImageDownFilename,MenuImageDownOverFilename,HideMenu,BlockSection,RootPageID"
            '        ShowHiddenMenu = Main.IsEditingAnything()
            '        If IsAllSectionsMenuMode Then
            '            '
            '            ' Section/Page connection at RootPageID, show all sections
            '            '
            '            CSSections = Main.OpenCSContent("Site Sections", , , , , , SelectFieldList)
            '        Else
            '            '
            '            ' Section/Page connection at RootPageID, only show sections connected to the menu
            '            '
            '            SQL = "Select Distinct S.ID" _
            '                & " from ((ccSections S" _
            '                & " left join ccDynamicMenuSectionRules R on R.SectionID=S.ID)" _
            '                & " left join ccDynamicMenus M on M.ID=R.DynamicMenuID)" _
            '                & " where M.ID=" & menuId
            '            Criteria = "ID in (" & SQL & ")"
            '            CSSections = Main.OpenCSContent("Site Sections", Criteria, , , , , SelectFieldList)
            '        End If
            '    ElseIf (BuildVersion > "3.3.494") Then
            '        '
            '        ' Multiple Menus with ccDynamicMenuSectionRules
            '        '
            '        SelectFieldList = "ID, Name,TemplateID,ContentID,Caption,MenuImageFilename,MenuImageOverFilename,HideMenu,BlockSection,0 as RootPageID"
            '        ShowHiddenMenu = Main.IsEditingAnything()
            '        If IsAllSectionsMenuMode Then
            '            '
            '            ' Section/Page connection at RootPageID, show all sections
            '            '
            '            CSSections = Main.OpenCSContent("Site Sections", , , , , , SelectFieldList)
            '        Else
            '            '
            '            ' Section/Page connection at RootPageID, only show sections connected to the menu
            '            '
            '            SQL = "Select Distinct S.ID" _
            '                & " from ((ccSections S" _
            '                & " left join ccDynamicMenuSectionRules R on R.SectionID=S.ID)" _
            '                & " left join ccDynamicMenus M on M.ID=R.DynamicMenuID)" _
            '                & " where M.ID=" & menuId
            '            Criteria = "ID in (" & SQL & ")"
            '            CSSections = Main.OpenCSContent("Site Sections", Criteria, , , , , SelectFieldList)
            '        End If
            '    ElseIf Csv.IsSQLTableField("Default", "ccSections", "BlockSection") Then
            '        '
            '        ' All sections menu mode with block sections
            '        '
            '        SelectFieldList = "ID, Name,TemplateID,ContentID,MenuImageFilename,Caption,MenuImageOverFilename,HideMenu,BlockSection,0 as RootPageID"
            '        Criteria = ""
            '        ShowHiddenMenu = Main.IsEditingAnything()
            '        CSSections = Main.OpenCSContent("Site Sections", Criteria, , , , , SelectFieldList)
            '    ElseIf Csv.IsSQLTableField("Default", "ccSections", "MenuImageOverFilename") Then
            '        '
            '        ' All sections menu mode with Image Over
            '        '
            '        SelectFieldList = "ID, Name,TemplateID,ContentID,MenuImageFilename,Caption,MenuImageOverFilename,HideMenu,0 as BlockSection,0 as RootPageID"
            '        Criteria = ""
            '        ShowHiddenMenu = Main.IsEditingAnything()
            '        CSSections = Main.OpenCSContent("Site Sections", Criteria, , , , , SelectFieldList)
            '    ElseIf Csv.IsSQLTableField("Default", "ccSections", "HideMenu") Then
            '        '
            '        ' All sections menu mode with HideMenu
            '        '
            '        SelectFieldList = "ID, Name,TemplateID,ContentID,MenuImageFilename,Caption,'' as MenuImageOverFilename,HideMenu,0 as BlockSection,0 as RootPageID"
            '        Criteria = ""
            '        ShowHiddenMenu = Main.IsEditingAnything()
            '        CSSections = Main.OpenCSContent("Site Sections", Criteria, , , , , SelectFieldList)
            '    Else
            '        SelectFieldList = "ID, Name,TemplateID,ContentID,MenuImageFilename,Caption,'' as MenuImageOverFilename,0 as HideMenu,0 as BlockSection,0 as RootPageID"
            '        Criteria = ""
            '        ShowHiddenMenu = True
            '        CSSections = Main.OpenCSContent("Site Sections", Criteria, , , , , SelectFieldList)
            '    End If
            Do While Main.IsCSOK(CSSections)
                HideMenu = Csv.GetCSBoolean(CSSections, "HideMenu")
                BlockSection = Csv.GetCSBoolean(CSSections, "BlockSection")
                sectionId = Csv.GetCSInteger(CSSections, "ID")
                hint = vbCrLf & vbTab & "<!-- getSectionMenu:SectionID=" & sectionId & ",HideMenu=" & HideMenu & ",BlockSection=" & BlockSection & ", -->"
                If ShowHiddenMenu Or Not (HideMenu Or Main.isSectionBlocked(sectionId, BlockSection)) Then
                    SectionName = Trim(Csv.GetCSText(CSSections, "Name"))
                    If SectionName = "" Then
                        SectionName = "Section " & sectionId
                        Call Main.ExecuteSQL("default", "update ccSections set Name=" & KmaEncodeSQLText(SectionName) & " where ID=" & sectionId)
                    End If
                    SectionCaption = Csv.GetCSText(CSSections, "Caption")
                    If SectionCaption = "" Then
                        SectionCaption = SectionName
                        Call Main.ExecuteSQL("default", "update ccSections set Caption=" & KmaEncodeSQLText(SectionCaption) & " where ID=" & sectionId)
                    End If
                    If HideMenu Then
                        SectionCaption = "[Hidden: " & SectionCaption & "]"
                    End If
                    SectionTemplateID = Csv.GetCSInteger(CSSections, "TemplateID")
                    ContentID = Csv.GetCSInteger(CSSections, "ContentID")
                    If (ContentID <> PageContentCID) And (Not Main.IsWithinContent(ContentID, PageContentCID)) Then
                        ContentID = PageContentCID
                        Call Csv.SetCS(CSSections, "ContentID", ContentID)
                    End If
                    If ContentID = PageContentCID Then
                        ContentName = "Page Content"
                    Else
                        ContentName = Main.GetContentNameByID(ContentID)
                        If ContentName = "" Then
                            ContentName = "Page Content"
                            ContentID = Main.GetContentID(ContentName)
                            Call Main.ExecuteSQL("default", "update ccSections set ContentID=" & ContentID & " where ID=" & sectionId)
                        End If
                    End If
                    MenuImage = Csv.GetCSText(CSSections, "MenuImageFilename")
                    If MenuImage <> "" Then
                        MenuImage = Main.serverFilePath & MenuImage
                    End If
                    MenuImageOver = Csv.GetCSText(CSSections, "MenuImageOverFilename")
                    If MenuImageOver <> "" Then
                        MenuImageOver = Main.serverFilePath & MenuImageOver
                    End If
                    MenuImageDown = Csv.GetCSText(CSSections, "MenuImageDownFilename")
                    If MenuImageDown <> "" Then
                        MenuImageDown = Main.serverFilePath & MenuImageDown
                    End If
                    MenuImageDownOver = Csv.GetCSText(CSSections, "MenuImageDownOverFilename")
                    If MenuImageDownOver <> "" Then
                        MenuImageDownOver = Main.serverFilePath & MenuImageDownOver
                    End If
                    '
                    ' Get Root Page for templateID
                    '
                    TemplateID = 0
                    BlockPage = False
                    Link = ""
                    If BuildVersion < "3.3.451" Then
                        '
                        ' no blockpage,section-page connection by name
                        '
                        PCCPtr = Main.GetPCCFirstNamePtr(SectionName, Main.IsWorkflowRendering, isQuickEditingPageContent)
                    ElseIf BuildVersion < "3.3.613" Then
                        '
                        ' blockpage,section-page connection by name
                        '
                        PCCPtr = Main.GetPCCFirstNamePtr(SectionName, Main.IsWorkflowRendering, isQuickEditingPageContent)
                    Else
                        '
                        ' section-page connection by name
                        '
                        RootPageID = Csv.GetCSInteger(CSSections, "rootpageid")
                        PCCPtr = Main.GetPCCPtr(RootPageID, Main.IsWorkflowRendering, isQuickEditingPageContent)
                    End If
                    hint = hint & vbCrLf & vbTab & "<!-- getSectionMenu:PCCPtr=" & PCCPtr & " -->"
                    If PCCPtr >= 0 Then

                        RootPageID = kmaEncodeInteger(pccLocal(PCC_ID, PCCPtr))
                        TemplateID = kmaEncodeInteger(pccLocal(PCC_TemplateID, PCCPtr))
                        BlockPage = kmaEncodeBoolean(pccLocal(PCC_BlockPage, PCCPtr))
                        PageActive = kmaEncodeBoolean(pccLocal(PCC_Active, PCCPtr))
                    End If
                    hint = hint & vbCrLf & vbTab & "<!-- getSectionMenu:PageActive=" & PageActive & ",ShowHiddenMenu=" & ShowHiddenMenu & ", -->"
                    If PageActive Or ShowHiddenMenu Then
                        If PCCPtr < 0 Then
                            '
                            ' Page Missing
                            '
                            SectionCaption = "[Missing Page: " & SectionCaption & "]"
                        ElseIf Not PageActive Then
                            '
                            ' Page Inactive
                            '
                            SectionCaption = "[Inactive Page: " & SectionCaption & "]"
                        End If
                        If TemplateID = 0 Then
                            TemplateID = SectionTemplateID
                        End If
                        '
                        ' Get the link from either the template, or use the default link
                        '
                        If BuildVersion >= "4.0.228" Then
                            Link = Main.GetTemplateLink(TemplateID)
                        End If
                        If Link = "" Then
                            Link = Main.GetSiteProperty("SectionLandingLink", Main.ServerAppRootPath & Main.ServerPageDefault)
                        End If
                        Link = kmaModifyLinkQuery(Link, "sid", CStr(sectionId), True)
                        '
                        ' Get Menu, remove crlf, and parse the line with crlf
                        '
                        MenuString = GetSiteTree_PageSubMenu(RootPageID, Link, DepthLimit, MenuStyle, StyleSheetPrefix, MenuImage, MenuImageOver, MenuImageDown, MenuImageDownOver, SectionCaption, sectionId, UseContentWatchLink, BuildVersion)
                        If (MenuString <> "") Then
                            If (getSectionMenu = "") Then
                                getSectionMenu = MenuString
                            Else
                                getSectionMenu = getSectionMenu & hint & MenuString
                            End If
                        End If
                    End If
                    '
                End If
                Call Main.NextCSRecord(CSSections)
            Loop
            Call Main.CloseCS(CSSections)
            '
            '
            ' 2011/9/21 - removed for now because the two additional items screw up a well balanced page
            '
            '    If IsEditingSections Then
            '        sectioncid = Main.GetContentID("site sections")
            '        Link = kmaEncodeHTML(Main.GetSiteProperty2("adminurl") & "?af=4&cid=" & sectioncid & "&wc=MenuID%3D" & MenuID)
            '        getSectionMenu = getSectionMenu & vbCrLf & vbTab _
            '            & "<node" _
            '            & " Caption=""Add&amp;nbsp;Section""" _
            '            & " Link=""" & Link & """" _
            '            & " ></node>"
            '        menucid = Main.GetContentID("dynamic menus")
            '        Link = kmaEncodeHTML(Main.GetSiteProperty2("adminurl") & "?af=4&cid=" & menucid & "&id=" & MenuID)
            '        getSectionMenu = getSectionMenu & vbCrLf & vbTab _
            '            & "<node" _
            '            & " Caption=""Edit&amp;nbsp;Menu""" _
            '            & " Link=""" & Link & """" _
            '            & " ></node>"
            '    End If
            '
            Exit Function
ErrorTrap:
            Call HandleClassTrapError("getSectionMenu", "Trap")
        End Function
        '
        '======================================================================================
        '   Get a dynamic menu from Page Content
        '======================================================================================
        '
        Private Function GetSiteTree_PageSubMenu(RootPageRecordID As Integer, DefaultLink As String, DepthLimit As Integer, MenuStyle As Integer, StyleSheetPrefix As String, MenuImage As String, MenuImageOver As String, MenuImageDown As String, MenuImageDownOver As String, RootMenuCaption As String, sectionId As Integer, UseContentWatchLink As Boolean, BuildVersion As String) As String
            On Error GoTo ErrorTrap
            '
            Dim Overview As String
            Dim IsQuickEditing As Boolean
            Dim CSSection As Integer
            Dim PseudoChildPagesFound As Boolean
            Dim AllowInMenus As Boolean
            Dim DateExpires As Date
            Dim DateArchive As Date
            Dim PubDate As Date
            Dim PCCPtr As Integer
            Dim PageFound As Boolean
            Dim ChildPageCount As Integer
            Dim ContentName As String
            Dim AddRootButton As Boolean
            Dim TopMenuCaption As String
            Dim WorkingPageTier1MenuCaption As String
            '
            Dim WorkingPageID As Integer
            Dim BakeName As String
            Dim Criteria As String
            Dim NodeIDPrefix As String
            Dim WorkingPageChildListSortMethodID As Integer
            Dim LinkWorking As String
            Dim LinkWorkingNoRedirect As String
            Dim WorkingPageParentID As Integer
            Dim WorkingPageTemplateID As Integer
            Dim WorkingPageCCID As Integer
            Dim WorkingPageAllowChildListDisplay As Boolean
            Dim WorkingPageMenuLinkOverRide As String
            Dim ChildPagesFound As Boolean
            Dim FieldList As String
            Dim ChildPagesFoundTest As String
            Dim hint As String
            '
            hint = hint & vbCrLf & vbTab & "<!-- GetSiteTree_PageSubMenu -->"
            ContentName = "Page Content"
            IsQuickEditing = Main.IsQuickEditing(ContentName)
            If False Then
                'If (PageName = "") Or (ContentName = "") Then
                ' page is no longer identified by its name. It is called by its rootpageid from the section
                Call Err.Raise(KmaErrorInternal, App.EXEName, "GetPageMenu requires a valid page name and content name")
            Else
                '
                ' ----- Read Bake Version
                '
                BakeName = "MenuSiteTree_" & RootPageRecordID & "_" & DefaultLink & "_" & DepthLimit & "_" & MenuStyle & "_" & StyleSheetPrefix
                BakeName = Replace(BakeName, "/", "_")
                BakeName = Replace(BakeName, " ", "_")
                'GetSiteTree_PageSubMenu = Main.ReadBake(BakeName)
                'If GetSiteTree_PageSubMenu <> "" Then
                '    GetSiteTree_PageSubMenu = GetSiteTree_PageSubMenu
                'Else
                If True Then
                    '
                    ' ----- Add Root Page to Menu System
                    '
                    PCCPtr = Main.GetPCCPtr(RootPageRecordID, Main.IsWorkflowRendering, Main.IsQuickEditing("page content"))
                    PageFound = (PCCPtr >= 0)
                    '
                    ' Skip if expired, archive and non-published
                    '
                    If PageFound Then
                        DateExpires = KmaEncodeDate(pccLocal(PCC_DateExpires, PCCPtr))
                        DateArchive = KmaEncodeDate(pccLocal(PCC_DateArchive, PCCPtr))
                        PubDate = KmaEncodeDate(pccLocal(PCC_PubDate, PCCPtr))
                        PageFound = ((DateExpires = CDate(0)) Or (DateExpires > Main.PageStartTime)) And ((PubDate = CDate(0)) Or (PubDate < Main.PageStartTime))
                    End If
                    hint = hint & vbCrLf & vbTab & "<!-- GetSiteTree_PageSubMenu:PageFound=" & PageFound & " -->"
                    If Not PageFound Then
                        '
                        ' menu root was not found, just put up what we have. If the link is there, the page will be created
                        '
                        AllowInMenus = True
                        LinkWorking = DefaultLink
                        LinkWorkingNoRedirect = LinkWorking
                        LinkWorking = kmaEncodeAppRootPath(LinkWorking, Main.ServerVirtualPath, Main.ServerAppRootPath, Main.ServerHost)
                        LinkWorking = kmaModifyLinkQuery(LinkWorking, "bid", "", False)
                        NodeIDPrefix = kmaEncodeText(GetRandomInteger) & "_"
                        ' ***** just want to know what would happen here
                        WorkingPageID = RootPageRecordID
                        'WorkingPageID = 0
                        WorkingPageChildListSortMethodID = 0
                        WorkingPageParentID = 0
                        WorkingPageTemplateID = 0
                        WorkingPageAllowChildListDisplay = False
                        WorkingPageMenuLinkOverRide = ""
                        ChildPagesFound = False
                    Else
                        '
                        ' AllowInMenus does not work for root pages, which are the only pages being handled here. This menu is hidden from the section record
                        '
                        AllowInMenus = True
                        If AllowInMenus Then
                            NodeIDPrefix = kmaEncodeText(GetRandomInteger) & "_"
                            WorkingPageID = kmaEncodeInteger(pccLocal(PCC_ID, PCCPtr))
                            WorkingPageChildListSortMethodID = kmaEncodeInteger(pccLocal(PCC_ChildListSortMethodID, PCCPtr))
                            WorkingPageTier1MenuCaption = kmaEncodeText(pccLocal(PCC_MenuHeadline, PCCPtr))
                            If WorkingPageTier1MenuCaption = "" Then
                                WorkingPageTier1MenuCaption = kmaEncodeText(pccLocal(PCC_Name, PCCPtr))
                                If WorkingPageTier1MenuCaption = "" Then
                                    WorkingPageTier1MenuCaption = "Page " & CStr(WorkingPageID)
                                End If
                            End If
                            WorkingPageCCID = kmaEncodeInteger(pccLocal(PCC_ContentControlID, PCCPtr))
                            WorkingPageTemplateID = kmaEncodeInteger(pccLocal(PCC_TemplateID, PCCPtr))
                            WorkingPageAllowChildListDisplay = kmaEncodeBoolean(pccLocal(PCC_AllowChildListDisplay, PCCPtr))
                            WorkingPageMenuLinkOverRide = kmaEncodeText(pccLocal(PCC_Link, PCCPtr))
                            ChildPagesFoundTest = kmaEncodeBoolean(pccLocal(PCC_ChildPagesFound, PCCPtr))
                            If ChildPagesFoundTest = "" Then
                                '
                                ' Not initialized, assume true
                                '
                                ChildPagesFound = True
                            Else
                                ChildPagesFound = kmaEncodeBoolean(ChildPagesFoundTest)
                            End If
                            '
                            ' Use WorkingPageParentID to detect if this record needs to be called with the bid
                            '
                            WorkingPageParentID = kmaEncodeInteger(pccLocal(PCC_ParentID, PCCPtr))
                            '
                            ' Get the Link
                            '
                            LinkWorkingNoRedirect = GetPageDynamicLinkWithArgs(WorkingPageCCID, WorkingPageID, DefaultLink, True, WorkingPageTemplateID, sectionId, "", UseContentWatchLink)
                            LinkWorking = LinkWorkingNoRedirect
                            If WorkingPageMenuLinkOverRide <> "" Then
                                LinkWorking = "?rc=" & WorkingPageCCID & "&ri=" & WorkingPageID
                            End If
                            'LinkWorking = GetPageDynamicLinkWithArgs(WorkingPageCCID, WorkingPageID, DefaultLink, True, WorkingPageTemplateID, SectionID, WorkingPageMenuLinkOverRide, UseContentWatchLink)
                        End If
                    End If
                    '
                    hint = hint & vbCrLf & vbTab & "<!-- GetSiteTree_PageSubMenu:AllowInMenus=" & AllowInMenus & " -->"
                    If AllowInMenus Then
                        '
                        ' ----- Set Tier1 Menu Caption (top element of the first flyout panel)
                        '
                        If WorkingPageTier1MenuCaption = "" Then
                            WorkingPageTier1MenuCaption = RootMenuCaption
                        End If
                        '
                        ' ----- Set Top Menu Caption (clickable label that opens the menus)
                        '
                        TopMenuCaption = RootMenuCaption
                        If TopMenuCaption = "" Then
                            TopMenuCaption = WorkingPageTier1MenuCaption
                        End If
                        '
                        If LinkWorking = "" Then
                            '
                            ' ----- Blank LinkWorking, this entry has no link
                            ' ----- Add menu header, and first entry for the root page
                            '
                            Call AddNodeToStruc(sectionId, WorkingPageID, NodeIDPrefix & WorkingPageID, "", MenuImage, MenuImageOver, MenuImageDown, MenuImageDownOver, "", TopMenuCaption, "", False, Overview)
                            '
                            ' ----- Root menu only, add a repeat of the button to the first menu
                            '
                            If (MenuStyle < 8) Or (MenuStyle > 11) Then
                                '
                                ' ##### Josh says Quadrem says they dont like the repeat on hovers
                                '
                                Call AddNodeToStruc(sectionId, WorkingPageID, NodeIDPrefix & WorkingPageID & ".entry", NodeIDPrefix & WorkingPageID, "", "", "", "", "", WorkingPageTier1MenuCaption, "", False, Overview)
                            End If
                        Else
                            '
                            ' ----- LinkWorking is here, put WorkingPageID on the end of it
                            ' ----- Add menu header, and first entry for the root page
                            '
                            If WorkingPageID <> 0 Then
                                Call AddNodeToStruc(sectionId, WorkingPageID, NodeIDPrefix & WorkingPageID, "", MenuImage, MenuImageOver, MenuImageDown, MenuImageDownOver, Main.GetLinkAliasByPageID(WorkingPageID, "", LinkWorking), TopMenuCaption, "", False, Overview)
                            ElseIf (sectionId <> 0) And (RootPageRecordID <> 0) Then
                                Call AddNodeToStruc(sectionId, WorkingPageID, NodeIDPrefix & RootPageRecordID, "", MenuImage, MenuImageOver, MenuImageDown, MenuImageDownOver, Main.GetLinkAliasByPageID(RootPageRecordID, "", LinkWorking), TopMenuCaption, "", False, Overview)
                            Else
                                Call AddNodeToStruc(sectionId, WorkingPageID, NodeIDPrefix & WorkingPageID, "", MenuImage, MenuImageOver, MenuImageDown, MenuImageDownOver, LinkWorking, TopMenuCaption, "", False, Overview)
                            End If
                            '
                            ' ----- Root menu only, add a repeat of the button to the first menu
                            '
                            AddRootButton = False
                            If (MenuStyle < 8) Or (MenuStyle > 11) Then
                                '
                                ' ##### Josh says Quadrem says they dont like the repeat on hovers
                                '
                                AddRootButton = True
                                If WorkingPageParentID <> 0 Then
                                    '
                                    ' This Top-most page is not the RootPage, include the bid
                                    '
                                    LinkWorking = kmaModifyLinkQuery(LinkWorking, "bid", CStr(WorkingPageID), True)
                                Else
                                    '
                                    ' This Top-most page is the RootPage, include no bid
                                    '
                                    'Call AddNodeToStruc(NodeIDPrefix & WorkingPageID & ".entry", NodeIDPrefix & WorkingPageID, "", "", LinkWorking, WorkingPageTier1MenuCaption)
                                End If
                            End If
                        End If
                        '
                        ' Build Submenu if child pages found
                        '
                        If True Then

                            If Main.IsWorkflowRendering Then
                                '
                                ' If workflow mode, just assume there are child pages
                                '
                                ChildPageCount = GetSiteTree_PageSubMenu_AddNodesReturnCnt(WorkingPageID, ContentName, LinkWorkingNoRedirect, WorkingPageTier1MenuCaption, "," & kmaEncodeText(WorkingPageID), NodeIDPrefix, 1, DepthLimit, WorkingPageChildListSortMethodID, sectionId, AddRootButton, UseContentWatchLink, BuildVersion)
                            Else
                                '
                                ' In production mode, use the ChildPagesFound field
                                '
                                PseudoChildPagesFound = ChildPagesFound
                                If Not PseudoChildPagesFound Then
                                    '
                                    ' Even when child pages is false, try it 10% of the time anyway
                                    ' IN case something goes wrong, this will rebuild 10% of the time
                                    '
                                    Randomize()
                                    PseudoChildPagesFound = (Rnd() > 0.8)
                                    If PseudoChildPagesFound Then
                                        TopMenuCaption = TopMenuCaption
                                    End If
                                End If
                                If PseudoChildPagesFound Then
                                    '
                                    ' Child pages were found, create child menu
                                    '
                                    ChildPageCount = GetSiteTree_PageSubMenu_AddNodesReturnCnt(WorkingPageID, ContentName, LinkWorkingNoRedirect, WorkingPageTier1MenuCaption, "," & kmaEncodeText(WorkingPageID), NodeIDPrefix, 1, DepthLimit, WorkingPageChildListSortMethodID, sectionId, AddRootButton, UseContentWatchLink, BuildVersion)
                                    If (DepthLimit > 0) Then
                                        If (BuildVersion > "3.3.737") Then
                                            If (ChildPageCount = 0) And (ChildPagesFound) Then
                                                '
                                                ' ChildPagesFound flag is true, but no pages were found - clear flag
                                                '
                                                Call Csv.ExecuteSQL("default", "update ccpagecontent set ChildPagesFound=0 where id=" & WorkingPageID)
                                                'Call Main.UpdatePCCRow(WorkingPageID, Main.IsWorkflowRendering, IsQuickEditing)
                                            ElseIf (ChildPageCount <> 0) And (Not ChildPagesFound) Then
                                                '
                                                ' ChildPagesFlag is cleared, but pages were found -- set the flag
                                                '
                                                Call Csv.ExecuteSQL("default", "update ccpagecontent set ChildPagesFound=1 where id=" & WorkingPageID)
                                                'Call Main.UpdatePCCRow(WorkingPageID, Main.IsWorkflowRendering, IsQuickEditing)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                            '
                            ' ----- Get the Menu Header
                            '
                            GetSiteTree_PageSubMenu = GetSiteTree_PageSubMenu & hint & GetNodesFromStruc(NodeIDPrefix & kmaEncodeText(WorkingPageID))
                            ''
                            '' ----- Bake the completed menu
                            ''
                            'Call Main.SaveBake(BakeName, GetSiteTree_PageSubMenu, ContentName & ",Site Sections,Dynamic Menus,Dynamic Menu Section Rules")
                        End If
                    End If
                End If
            End If
            '
            Exit Function
ErrorTrap:
            Call HandleClassTrapError("GetSiteTree_PageSubMenu")
        End Function
        '
        '======================================================================================
        '   Add child pages to the menu system
        '======================================================================================
        '
        Private Function GetSiteTree_PageSubMenu_AddNodesReturnCnt(ParentMenuID As Integer, ContentName As String, DefaultLink As String, Tier1MenuCaption As String, UsedPageIDString As String, MenuNamePrefix As String, MenuDepth As Integer, MenuDepthMax As Integer, ChildListSortMethodID As Integer, sectionId As Integer, AddRootButton As Boolean, UseContentWatchLink As Boolean, BuildVersion As String) As Integer
            On Error GoTo ErrorTrap
            '
            Dim Overview As String
            Dim Active As Boolean
            Dim PseudoChildChildPagesFound As Boolean
            Dim PCCRowPtr As Integer
            Dim SortForward As Boolean
            Dim SortFieldName As String
            Dim SortPtr As Integer
            Dim Ptr As Integer
            Dim ChildPageCount As Integer
            Dim ChildPagesFoundTest As String
            Dim FieldList As String
            Dim ChildCountWithNoPubs As Integer
            Dim menuId As Integer
            Dim MenuCaption As String
            Dim ChildCount As Integer
            Dim ChildSize As Integer
            Dim ChildPointer As Integer
            Dim ChildID() As Integer
            Dim ChildAllowChild() As Boolean
            Dim ChildCaption() As String
            Dim ChildLink() As String
            Dim ChildOverview() As String
            Dim ChildSortMethodID() As Integer
            Dim ChildChildPagesFound() As Boolean
            Dim ContentID As Integer
            Dim MenuLinkOverRide As String
            Dim PageID As Integer
            Dim UsedPageIDStringLocal As String
            Dim Criteria As String
            Dim MenuDepthLocal As Integer
            Dim OrderByCriteria As String
            Dim WorkingLink As String
            Dim TemplateID As Integer
            Dim ContentControlID As Integer
            Dim Link As String
            Dim PubDate As Date
            Dim PCCPtr As Integer
            Dim DateExpires As Date
            Dim DateArchive As Date
            Dim IsIncludedInMenu As Boolean
            Dim PCCPtrs() As Integer
            Dim PtrCnt As Integer
            Dim SortSplit() As String
            Dim SortSplitCnt As Integer
            'Dim Index As FastIndexClass
            Dim PCCColPtr As Integer
            Dim PCCPtrsSorted As Variant
            Dim AllowInMenus As Boolean
            Dim IsQuickEditing As Boolean
            Dim hint As String
            '
            ' ----- Gather all child menus
            '
            hint = hint & vbCrLf & vbTab & "<!-- GetSiteTree_PageSubMenu_AddNodesReturnCnt -->"
            '
            IsQuickEditing = Main.IsQuickEditing("page content")
            If (ParentMenuID > 0) And (MenuDepth <= MenuDepthMax) Then
                MenuDepthLocal = MenuDepth + 1
                UsedPageIDStringLocal = UsedPageIDString
                OrderByCriteria = Main.GetSortMethodByID(ChildListSortMethodID)
                If OrderByCriteria = "" Then
                    OrderByCriteria = Main.GetContentProperty(ContentName, "defaultsortmethod")
                End If
                If OrderByCriteria = "" Then
                    OrderByCriteria = "ID"
                End If
                '
                ' Populate PCCPtrs()
                '
                PCCPtr = Main.GetPCCFirstChildPtr(ParentMenuID, Main.IsWorkflowRendering, IsQuickEditing)
                PtrCnt = 0
                Do While PCCPtr >= 0
                    ReDim Preserve PCCPtrs(PtrCnt)
                    PCCPtrs(PtrCnt) = PCCPtr
                    PtrCnt = PtrCnt + 1
                    PCCPtr = Main.PCCParentIDIndex.GetNextPointerMatch(CStr(ParentMenuID))
                Loop
                If PtrCnt > 0 Then
                    PCCPtrsSorted = Main.GetPCCPtrsSorted(PCCPtrs, OrderByCriteria)
                End If
                '
                Ptr = 0
                Do While Ptr < PtrCnt
                    PCCPtr = PCCPtrsSorted(Ptr)
                    DateExpires = KmaEncodeDate(pccLocal(PCC_DateExpires, PCCPtr))
                    DateArchive = KmaEncodeDate(pccLocal(PCC_DateArchive, PCCPtr))
                    PubDate = KmaEncodeDate(pccLocal(PCC_PubDate, PCCPtr))
                    MenuCaption = Trim(kmaEncodeText(pccLocal(PCC_MenuHeadline, PCCPtr)))
                    If BuildVersion < "3.3.752" Then
                        AllowInMenus = (MenuCaption <> "")
                    Else
                        AllowInMenus = kmaEncodeBoolean(pccLocal(PCC_AllowInMenus, PCCPtr))
                    End If
                    Active = kmaEncodeBoolean(pccLocal(PCC_Active, PCCPtr))
                    IsIncludedInMenu = Active And AllowInMenus And ((PubDate = CDate(0)) Or (PubDate < Main.PageStartTime)) And ((DateExpires = CDate(0)) Or (DateExpires > Main.PageStartTime))
                    'IsIncludedInMenu = Active And AllowInMenus And ((PubDate = CDate(0)) Or (PubDate < main.PageStartTime)) And ((DateExpires = CDate(0)) Or (DateExpires > main.PageStartTime)) And ((DateArchive = CDate(0)) Or (DateArchive > main.PageStartTime))
                    If IsIncludedInMenu Then
                        If MenuCaption = "" Then
                            MenuCaption = Trim(kmaEncodeText(pccLocal(PCC_Name, PCCPtr)))
                        End If
                        If MenuCaption = "" Then
                            MenuCaption = "Related Page"
                        End If
                        If (MenuCaption <> "") Then
                            PageID = kmaEncodeInteger(pccLocal(PCC_ID, PCCPtr))
                            If InStr(1, UsedPageIDStringLocal & ",", "," & kmaEncodeText(PageID) & ",") = 0 Then
                                UsedPageIDStringLocal = UsedPageIDStringLocal & "," & kmaEncodeText(PageID)
                                If ChildCount >= ChildSize Then
                                    ChildSize = ChildSize + 100
                                    ReDim Preserve ChildID(ChildSize)
                                    ReDim Preserve ChildCaption(ChildSize)
                                    ReDim Preserve ChildLink(ChildSize)
                                    ReDim Preserve ChildSortMethodID(ChildSize)
                                    ReDim Preserve ChildAllowChild(ChildSize)
                                    ReDim Preserve ChildChildPagesFound(ChildSize)
                                    ReDim Preserve ChildOverview(ChildSize)
                                End If
                                ContentControlID = kmaEncodeInteger(pccLocal(PCC_ContentControlID, PCCPtr))
                                MenuLinkOverRide = kmaEncodeText(pccLocal(PCC_Link, PCCPtr))
                                '
                                ChildCaption(ChildCount) = MenuCaption
                                ChildID(ChildCount) = PageID
                                ChildAllowChild(ChildCount) = kmaEncodeBoolean(pccLocal(PCC_AllowChildListDisplay, PCCPtr))
                                ChildSortMethodID(ChildCount) = kmaEncodeInteger(pccLocal(PCC_ChildListSortMethodID, PCCPtr))
                                Overview = kmaEncodeText(pccLocal(PCC_BriefFilename, PCCPtr))
                                If Overview <> "" Then
                                    Overview = Main.ReadVirtualFile(Overview)
                                End If
                                ChildOverview(ChildCount) = Overview
                                '
                                TemplateID = kmaEncodeInteger(pccLocal(PCC_TemplateID, PCCPtr))
                                Link = GetPageDynamicLinkWithArgs(ContentControlID, PageID, DefaultLink, False, TemplateID, sectionId, MenuLinkOverRide, UseContentWatchLink)
                                ChildLink(ChildCount) = Link
                                '
                                ChildPagesFoundTest = kmaEncodeBoolean(pccLocal(PCC_ChildPagesFound, PCCPtr))
                                If ChildPagesFoundTest = "" Then
                                    '
                                    ' Not initialized
                                    '
                                    ChildChildPagesFound(ChildCount) = True
                                Else
                                    ChildChildPagesFound(ChildCount) = kmaEncodeBoolean(ChildPagesFoundTest)
                                End If
                                ChildCount = ChildCount + 1
                            End If
                        End If
                    End If
                    Ptr = Ptr + 1
                Loop
                ChildCountWithNoPubs = Ptr
                '
                ' ----- Output menu entries
                '
                If ChildCount > 0 Then
                    '
                    ' menu entry has children, output menu entry, child menu entry, and group of child entries
                    '
                    If AddRootButton Then
                        '
                        ' Root Button is a redundent menu entry at the top of tier 1 panels that links to the root page
                        '
                        Call AddNodeToStruc(sectionId, ParentMenuID, MenuNamePrefix & ParentMenuID & ".entry", MenuNamePrefix & ParentMenuID, "", "", "", "", Main.GetLinkAliasByPageID(ParentMenuID, "", DefaultLink), Tier1MenuCaption, "", False, Overview)
                        'Call AddNodeToStruc(MenuNamePrefix & ParentMenuID & ".entry", MenuNamePrefix & ParentMenuID, "", "", GetLinkAliasByLink(DefaultLink), Tier1MenuCaption)
                    End If
                    '
                    For ChildPointer = 0 To ChildCount - 1
                        menuId = ChildID(ChildPointer)
                        MenuCaption = ChildCaption(ChildPointer)
                        WorkingLink = ChildLink(ChildPointer)
                        Overview = ChildOverview(ChildPointer)
                        Call AddNodeToStruc(sectionId, menuId, MenuNamePrefix & menuId, MenuNamePrefix & ParentMenuID, "", "", "", "", Main.GetLinkAliasByPageID(menuId, "", WorkingLink), MenuCaption, "", False, Overview)
                        'Call AddNodeToStruc(MenuNamePrefix & MenuID, MenuNamePrefix & ParentMenuID, "", "", GetLinkAliasByLink(WorkingLink), MenuCaption)
                        '
                        ' if child pages are found, print the next menu deeper
                        '
                        If (ParentMenuID > 0) And (MenuDepthLocal <= MenuDepthMax) Then
                            If Main.IsWorkflowRendering Then
                                '
                                ' Workflow mode - go get the child pages
                                '
                                ChildPageCount = GetSiteTree_PageSubMenu_AddNodesReturnCnt(menuId, ContentName, WorkingLink, MenuCaption, UsedPageIDStringLocal, MenuNamePrefix, MenuDepthLocal, MenuDepthMax, ChildSortMethodID(ChildPointer), sectionId, False, UseContentWatchLink, BuildVersion)
                            Else
                                '
                                ' Production mode - get them only if the parent record says there are child pages
                                '
                                PseudoChildChildPagesFound = ChildChildPagesFound(ChildPointer)
                                If Not PseudoChildChildPagesFound Then
                                    '
                                    ' Even when child pages is false, try it 10% of the time anyway
                                    '
                                    Randomize()
                                    PseudoChildChildPagesFound = (Rnd() > 0.8)
                                End If
                                If PseudoChildChildPagesFound Then
                                    '
                                    ' Child pages were found, create child menu
                                    '
                                    ChildPageCount = GetSiteTree_PageSubMenu_AddNodesReturnCnt(menuId, ContentName, WorkingLink, MenuCaption, UsedPageIDStringLocal, MenuNamePrefix, MenuDepthLocal, MenuDepthMax, ChildSortMethodID(ChildPointer), sectionId, False, UseContentWatchLink, BuildVersion)
                                    If (MenuDepthLocal < MenuDepthMax) Then
                                        If (BuildVersion > "3.3.737") Then
                                            If ChildChildPagesFound(ChildPointer) And (ChildPageCount = 0) Then
                                                '
                                                ' no pages were found, clear the child pages found property
                                                ' child pages found property is set at admin site when a page is saved with this as the parent id
                                                '
                                                Call Csv.ExecuteSQL("default", "update ccpagecontent set ChildPagesFound=0 where id=" & menuId)
                                                Call Main.UpdatePCCRow(menuId, Main.IsWorkflowRendering, IsQuickEditing)
                                            ElseIf (ChildPageCount > 0) And (Not ChildChildPagesFound(ChildPointer)) Then
                                                '
                                                ' pages were found, set the child pages found property
                                                '
                                                Call Csv.ExecuteSQL("default", "update ccpagecontent set ChildPagesFound=1 where id=" & menuId)
                                                Call Main.UpdatePCCRow(menuId, Main.IsWorkflowRendering, IsQuickEditing)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
            End If
            GetSiteTree_PageSubMenu_AddNodesReturnCnt = ChildCountWithNoPubs
            '
            Exit Function
ErrorTrap:
            Call HandleClassTrapError("GetSiteTree_PageSubMenu_AddNodeReturnCnt")
        End Function
        '
        '
        '
        Private Function GetPageDynamicLinkWithArgs(ContentControlID As Integer, PageID As Integer, DefaultLink As String, IsRootPage As Boolean, TemplateID As Integer, sectionId As Integer, MenuLinkOverRide As String, UseContentWatchLink As Boolean) As String
            On Error GoTo ErrorTrap
            '
            If MenuLinkOverRide <> "" Then
                GetPageDynamicLinkWithArgs = "?rc=" & ContentControlID & "&ri=" & PageID
            Else
                If True Then
                    '
                    ' Current method - all pages are in the Template, Section, Page structure
                    '
                    If TemplateID <> 0 Then
                        '
                        ' if template, use that
                        '
                        Main.GetTemplateLink(TemplateID)
                    End If
                    If (PageID = 0) Or (IsRootPage) Then
                        '
                        ' Link to Root Page, no bid, and include sectionid if not 0
                        '
                        If IsRootPage And (sectionId <> 0) Then
                            GetPageDynamicLinkWithArgs = kmaModifyLinkQuery(GetPageDynamicLinkWithArgs, "sid", CStr(sectionId), True)
                        End If
                        GetPageDynamicLinkWithArgs = kmaModifyLinkQuery(GetPageDynamicLinkWithArgs, "bid", "", False)
                    Else
                        GetPageDynamicLinkWithArgs = kmaModifyLinkQuery(GetPageDynamicLinkWithArgs, "bid", kmaEncodeText(PageID), True)
                        ' ##### no, leave this in if bid<>0
                        ' ##### take this out, in GetSectionPage, this is handled through the refreshquerystring
                        If PageID <> 0 Then
                            GetPageDynamicLinkWithArgs = kmaModifyLinkQuery(GetPageDynamicLinkWithArgs, "sid", "", False)
                        End If
                    End If
                End If
            End If
            GetPageDynamicLinkWithArgs = kmaEncodeAppRootPath(GetPageDynamicLinkWithArgs, Main.ServerVirtualPath, Main.ServerAppRootPath, Main.ServerHost)
            Exit Function
            '
ErrorTrap:
            Call HandleClassTrapError("GetPageDynamicLinkWithArgs")
        End Function
        ''
        ''========================================================================
        '' ----- Add a new DHTML menu entry
        ''========================================================================
        ''
        'Private Sub AddNodeToStruc(Name As String, ParentName As String, ImageLink As String, ImageOverLink As String, Link As String, Caption As String, StyleSheet As String, StyleSheetHover As String, NewWindow As Boolean, Overview As String)
        '    On Error GoTo ErrorTrap
        '    '
        '    Dim MethodName As String
        '    Dim Image As String
        '    Dim ImageOver As String
        '    '
        '    MethodName = "AddMenu()"
        '    '
        '    If (MenuSystem Is Nothing) Then
        '        Set MenuSystem = New SiteTreeNavClass
        '    End If
        '    If Not (MenuSystem Is Nothing) Then
        '        Image = kmaEncodeText(ImageLink)
        '        If Image <> "" Then
        '            ImageOver = kmaEncodeText(ImageOverLink)
        '            If Image = ImageOver Then
        '                ImageOver = ""
        '            End If
        '        End If
        '        Call MenuSystem.AddNodeToStruc(kmaEncodeText(Name), ParentName, Image, ImageOver, Link, Caption, "", False, Overview)
        '    End If
        '    '
        '    Exit Sub
        '    '
        '    ' ----- Error Trap
        '    '
        'ErrorTrap:
        '    Call HandleClassTrapError(MethodName)
        '    '
        'End Sub
        ''
        ''========================================================================
        '' ----- Get the menu link for the menu name specified
        ''========================================================================
        ''
        'Private Function GetNodesFromStruc(MenuName As Variant, MenuStyle As Variant, Optional StyleSheetPrefix As Variant) As String
        '    On Error GoTo ErrorTrap
        '    '
        '    Dim MethodName As String
        '    Dim OpenNodeList As String
        '    Dim OpenNode As String
        '    Dim CloseNode As String
        '    Dim MenuFlyoutIcon As String
        '    Dim iMenuName As String
        '    Const DefaultIcon = "&#187;"
        '    '
        '    MethodName = "GetNodesFromStruc()"
        '    '
        '    If Not (MenuSystem Is Nothing) Then
        '        '
        '        iMenuName = kmaEncodeText(MenuName)
        '        'MenuFlyoutIcon = Main.GetSiteProperty("MenuFlyoutIcon", DefaultIcon)
        '        'If MenuFlyoutIcon <> DefaultIcon Then
        '        '    MenuSystem.MenuFlyoutIcon = MenuFlyoutIcon
        '        'End If
        '        GetNodesFromStruc = MenuSystem.GetNodesFromStruc(iMenuName)
        '    End If
        '     '
        '    Exit Function
        '    '
        '    ' ----- Error Trap
        '    '
        'ErrorTrap:
        '    Call HandleClassTrapError(MethodName)
        'End Function
        '
        '===============================================================================
        '   Returns the menu specified, if it is in local storage
        '===============================================================================
        '
        Private Function GetNodesFromStruc(NodeName As String) As String
            On Error GoTo ErrorTrap
            '
            Dim nodePtr As Integer
            Dim UcaseNodeName As String
            Dim SubNodes As String
            '
            ' ----- Search local storage for this NodeName
            '
            If NodeStrucCount > 0 Then
                '
                ' ----- Get the menu pointer
                '
                UcaseNodeName = UCase(NodeName)
                For nodePtr = 0 To NodeStrucCount - 1
                    If NodeStruc(nodePtr).Name = UcaseNodeName Then
                        Exit For
                    End If
                Next
                If nodePtr < NodeStrucCount Then
                    SubNodes = GetNodeChildrenFromStruc(UcaseNodeName, "")
                    If SubNodes = "" Then
                        GetNodesFromStruc = vbCrLf & vbTab & GetNode(nodePtr) & "</node>"
                    Else
                        GetNodesFromStruc = "" _
                    & vbCrLf & vbTab & GetNode(nodePtr) _
                    & kmaIndent(SubNodes) _
                    & vbCrLf & vbTab & "</node>"
                    End If
                End If
            End If
            GetNodesFromStruc = GetNodesFromStruc
            '
            Exit Function
            '
ErrorTrap:
            Call HandleClassTrapError("GetNodesFromStruc")
        End Function
        '
        '===============================================================================
        '   Create a new Menu Entry
        '===============================================================================
        '
        Private Sub AddNodeToStruc(sectionId As Integer, PageID As Integer, EntryName As String, ParentNodeStrucName As String, ImageLink As String, ImageOverLink As String, ImageDownLink As String, ImageDownOverLink As String, Link As String, Caption As String, CaptionImageLink As String, NewWindow As Boolean, Overview As String)
            On Error GoTo ErrorTrap
            '
            Dim MenuEntrySize As Integer
            Dim NodeStrucName As String
            Dim UcaseNodeStrucName As String
            Dim iNewWindow As Boolean
            '
            NodeStrucName = Replace(KmaEncodeMissingText(EntryName, ""), ",", " ")
            UcaseNodeStrucName = UCase(NodeStrucName)
            '
            If (NodeStrucName <> "") And (InStr(1, NodeStrucUsedNameList & ",", "," & UcaseNodeStrucName & ",", vbBinaryCompare) = 0) Then
                If ImageLink = ImageOverLink Then
                    ImageOverLink = ""
                End If
                NodeStrucUsedNameList = NodeStrucUsedNameList & "," & UcaseNodeStrucName
                If NodeStrucCount >= NodeStrucSize Then
                    NodeStrucSize = NodeStrucSize + 10
                    ReDim Preserve NodeStruc(NodeStrucSize)
                End If
                With NodeStruc(NodeStrucCount)
                    .sectionId = sectionId
                    .PageID = PageID
                    .Link = KmaEncodeMissingText(Link, "")
                    .Image = KmaEncodeMissingText(ImageLink, "")
                    If .Image = "" Then
                        '
                        ' No image, must have a caption
                        '
                        .Caption = KmaEncodeMissingText(Caption, NodeStrucName)
                    Else
                        '
                        ' Image present, caption is extra
                        '
                        .Caption = KmaEncodeMissingText(Caption, "")
                    End If
                    .CaptionImage = KmaEncodeMissingText(CaptionImageLink, "")
                    .Name = UcaseNodeStrucName
                    .ParentName = UCase(KmaEncodeMissingText(ParentNodeStrucName, ""))
                    .ImageOver = KmaEncodeMissingText(ImageOverLink, "")
                    .ImageDown = KmaEncodeMissingText(ImageDownLink, "")
                    .ImageDownOver = KmaEncodeMissingText(ImageDownOverLink, "")
                    .NewWindow = KmaEncodeMissingBoolean(NewWindow, False)
                    .Overview = KmaEncodeMissingText(Overview, "")
                End With
                'Call EntryIndexName.SetPointer(UcaseNodeStrucName, NodeStrucCount)
                NodeStrucCount = NodeStrucCount + 1
            End If
            '
            Exit Sub
            '
ErrorTrap:
            Call HandleClassTrapError("AddNodeToStruc")
        End Sub
        '
        '===============================================================================
        '   Returns javascripts, etc. required after all menus on a page are complete
        '===============================================================================
        '
        Private Function GetNodesFromStrucClose() As String
            On Error GoTo ErrorTrap
            '
            '    GetNodesFromStrucClose = GetNodesFromStrucClose & iMenuCloseString
            '    iMenuCloseString = ""
            'If (iDQMCount > 0) And (Not iDQMCLosed) Then
            '    '
            '    ' ----- Add the DQM menu script to the end
            '    '
            '    GetNodesFromStrucClose = GetNodesFromStrucClose & "<script language=""JavaScript1.2"" src=""/cclib/clientside/dqm_script.js"" type=""text/javascript""></script>"
            '    iDQMCLosed = True
            '    End If
            'If GetNodesFromStrucClose <> "" Then
            '    GetNodesFromStrucClose = vbCrLf & GetNodesFromStrucClose & vbCrLf
            '    End If
            '
            Exit Function
            '
ErrorTrap:
            Call HandleClassTrapError("GetNodesFromStrucClose")
        End Function
        '
        '===============================================================================
        '   Returns from the cursor position to the end of line
        '   Moves the Start Position to the next line
        '===============================================================================
        '
        Private Function ReadLine(ByVal StartPosition As Integer, Source As String) As String
            On Error GoTo ErrorTrap
            '
            Dim EndOfLine As Integer
            '
            ReadLine = ""
            EndOfLine = InStr(StartPosition, Source, vbCrLf)
            If EndOfLine <> 0 Then
                ReadLine = Mid(Source, StartPosition, EndOfLine)
                StartPosition = EndOfLine + 2
            End If
            '
            Exit Function
            '
ErrorTrap:
            Call HandleClassTrapError("ReadLine")
        End Function
        '
        '
        '
        Private Sub Class_Initialize()
            ' iDQMCount = 0
            'Set EntryIndexName = New FastIndexClass
            Randomize()
            'MenuFlyoutNamePrefix = "id" & CStr(Int(9999 * Rnd()))
            'MenuFlyoutIcon_Local = "&nbsp;&#187;"
        End Sub
        '
        '===============================================================================
        '   Returns the menu specified, if it is in local storage
        '       It also creates the menu data in a close string that is returned in GetNodesFromStrucClose.
        '===============================================================================
        '
        Private Function GetNodesFromStrucFlyout(menuName As String, MenuStyle As Integer, Optional StyleSheetPrefix As Variant) As String
            On Error GoTo ErrorTrap
            Exit Function
            '
ErrorTrap:
            Call HandleClassTrapError("GetNodesFromStrucFlyout")
        End Function
        '
        '===============================================================================
        '   Gets the Menu Branch for the Default Menu
        '===============================================================================
        '
        Private Function GetNodeChildrenFromStruc(PanelName As String, NodeStrucUsedNameList As String) As String
            On Error GoTo ErrorTrap
            '
            Dim EntryPointer As Integer
            Dim SubMenuName As String
            Dim SubMenuCount As Integer
            Dim target As String
            Dim SubMenus As String
            Dim PanelChildren As String
            Dim PanelButtons As String
            Dim PanelButtonStyle As String
            Dim HotSpotHTML As String
            '
            For EntryPointer = 0 To NodeStrucCount - 1
                With NodeStruc(EntryPointer)
                    If (.ParentName = PanelName) And (.Caption <> "") Then
                        If Not InStr(1, NodeStrucUsedNameList & ",", "," & EntryPointer & ",") Then
                            NodeStrucUsedNameList = NodeStrucUsedNameList & "," & EntryPointer
                            SubMenus = GetNodeChildrenFromStruc(.Name, NodeStrucUsedNameList)
                            If SubMenus = "" Then
                                GetNodeChildrenFromStruc = GetNodeChildrenFromStruc & vbCrLf & vbTab & GetNode(EntryPointer) & "</node>"
                            Else
                                GetNodeChildrenFromStruc = GetNodeChildrenFromStruc _
                            & vbCrLf & vbTab & GetNode(EntryPointer) _
                            & kmaIndent(SubMenus) _
                            & vbCrLf & vbTab & "</node>"
                            End If
                        End If
                    End If
                End With
            Next
            '
            Exit Function
            '
ErrorTrap:
            Call HandleClassTrapError("GetNodeChildrenFromStruc")
        End Function
        ''
        ''========================================================================
        ''
        ''========================================================================
        ''
        'Private Property Get MenuFlyoutIcon() As String
        '    'MenuFlyoutIcon = MenuFlyoutIcon_Local
        'End Property
        ''
        ''========================================================================
        ''
        ''========================================================================
        ''
        'Private Property Let MenuFlyoutIcon(ByVal vNewValue As String)
        '    'MenuFlyoutIcon_Local = vNewValue
        'End Property
        '
        '
        '
        Private Function GetNode(Ptr As Integer) As String
            With NodeStruc(Ptr)
                GetNode = "<node" _
            & " SectionID=""" & CStr(.sectionId) & """" _
            & " PageID=""" & CStr(.PageID) & """" _
            & " Caption=""" & EncodeHTMLAttribute(.Caption) & """" _
            & " CaptionImage=""" & EncodeHTMLAttribute(.CaptionImage) & """" _
            & " ImageSrc=""" & EncodeHTMLAttribute(.Image) & """" _
            & " ImageOverSrc=""" & EncodeHTMLAttribute(.ImageOver) & """" _
            & " ImageDownSrc=""" & EncodeHTMLAttribute(.ImageDown) & """" _
            & " ImageDownOverSrc=""" & EncodeHTMLAttribute(.ImageDownOver) & """" _
            & " Link=""" & EncodeHTMLAttribute(.Link) & """" _
            & " Name=""" & EncodeHTMLAttribute(.Name) & """" _
            & " NewWindow=""" & kmaEncodeBoolean(.NewWindow) & """" _
            & " Overview=""" & EncodeHTMLAttribute(.Overview) & """" _
            & ">"
            End With
        End Function
        '
        '
        '
        Private Function EncodeHTMLAttribute(Source As String) As String
            Dim s As String
            '
            s = kmaEncodeHTML(Source)
            s = Replace(s, vbCrLf, "")
            s = Replace(s, vbCr, "")
            s = Replace(s, vbLf, "")
            '
            EncodeHTMLAttribute = s
            '
        End Function
        '
        '===========================================================================
        '
        '===========================================================================
        '
        Private Sub HandleClassTrapError(MethodName As String, Optional Context As String)
            '
            If Main Is Nothing Then
                Call HandleError2("unknown", Context, "aoDynamicMenu", "MenuClass", MethodName, Err.Number, Err.Source, Err.Description, True, False, "unknown")
            Else
                Call HandleError2(Main.ApplicationName, Context, "aoDynamicMenu", "MenuClass", MethodName, Err.Number, Err.Source, Err.Description, True, False, Main.ServerLink)
            End If
            '
        End Sub
        '
        '===========================================================================
        '
        '===========================================================================
        '
        Private Sub HandleClassAppendLogfile(MethodName As String, Context As String)
            If Main Is Nothing Then
                Call AppendLogFile2("", Context, "aoDynamicMenu", "MenuClass", MethodName, 0, "", "", False, True, "", "", "trace")
            Else
                Call AppendLogFile2(Main.ApplicationName, Context, "aoDynamicMenu", "MenuClass", MethodName, 0, "", "", False, True, Main.ServerLink, "", "trace")
            End If

        End Sub
    End Class
End Namespace
