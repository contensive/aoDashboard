Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Controllers
    Public Class GenericController
        '
        '====================================================================================================
        ''' <summary>
        ''' create addon icon
        ''' </summary>
        ''' <param name="AdminURL"></param>
        ''' <param name="IconWidth"></param>
        ''' <param name="IconHeight"></param>
        ''' <param name="IconSprites"></param>
        ''' <param name="IconIsInline"></param>
        ''' <param name="IconImgID"></param>
        ''' <param name="IconFilename"></param>
        ''' <param name="serverFilePath"></param>
        ''' <param name="IconAlt"></param>
        ''' <param name="IconTitle"></param>
        ''' <param name="ACInstanceID"></param>
        ''' <param name="IconSpriteColumn"></param>
        ''' <returns></returns>
        Public Shared Function getAddonIconImg(AdminURL As String, IconWidth As Long, IconHeight As Long, IconSprites As Long, IconIsInline As Boolean, IconImgID As String, IconFilename As String, serverFilePath As String, IconAlt As String, IconTitle As String, ACInstanceID As String, IconSpriteColumn As Long) As String
            '
            If IconAlt = "" Then
                IconAlt = "Add-on"
            End If
            If IconTitle = "" Then
                IconTitle = "Rendered as Add-on"
            End If
            If IconFilename = "" Then
                '
                ' No icon given, use the default
                '
                If IconIsInline Then
                    IconFilename = "/ccLib/images/IconAddonInlineDefault.png"
                    IconWidth = 62
                    IconHeight = 17
                    IconSprites = 0
                Else
                    IconFilename = "/ccLib/images/IconAddonBlockDefault.png"
                    IconWidth = 57
                    IconHeight = 59
                    IconSprites = 4
                End If
            ElseIf InStr(1, IconFilename, "://") <> 0 Then
                '
                ' icon is an Absolute URL - leave it
                '
            ElseIf Left(IconFilename, 1) = "/" Then
                '
                ' icon is Root Relative, leave it
                '
            Else
                '
                ' icon is a virtual file, add the serverfilepath
                '
                IconFilename = serverFilePath & IconFilename
            End If
            'IconFilename = kmaEncodeJavascript(IconFilename)
            If (IconWidth = 0) Or (IconHeight = 0) Then
                IconSprites = 0
            End If

            If IconSprites = 0 Then
                '
                ' just the icon
                '
                getAddonIconImg = "<img" _
                    & " border=0" _
                    & " id=""" & IconImgID & """" _
                    & " onDblClick=""window.parent.OpenAddonPropertyWindow(this,'" & AdminURL & "');""" _
                    & " alt=""" & IconAlt & """" _
                    & " title=""" & IconTitle & """" _
                    & " src=""" & IconFilename & """"
                'GetAddonIconImg = "<img" _
                '    & " id=""AC,AGGREGATEFUNCTION,0," & FieldName & "," & ArgumentList & """" _
                '    & " onDblClick=""window.parent.OpenAddonPropertyWindow(this);""" _
                '    & " alt=""" & IconAlt & """" _
                '    & " title=""" & IconTitle & """" _
                '    & " src=""" & IconFilename & """"
                If IconWidth <> 0 Then
                    getAddonIconImg = getAddonIconImg & " width=""" & IconWidth & "px"""
                End If
                If IconHeight <> 0 Then
                    getAddonIconImg = getAddonIconImg & " height=""" & IconHeight & "px"""
                End If
                If IconIsInline Then
                    getAddonIconImg = getAddonIconImg & " style=""vertical-align:middle;display:inline;"" "
                Else
                    getAddonIconImg = getAddonIconImg & " style=""display:block"" "
                End If
                If ACInstanceID <> "" Then
                    getAddonIconImg = getAddonIconImg & " ACInstanceID=""" & ACInstanceID & """"
                End If
                getAddonIconImg = getAddonIconImg & ">"
            Else
                '
                ' Sprite Icon
                '
                getAddonIconImg = getIconSprite(IconImgID, IconSpriteColumn, IconFilename, IconWidth, IconHeight, IconAlt, IconTitle, "window.parent.OpenAddonPropertyWindow(this,'" & AdminURL & "');", IconIsInline, ACInstanceID)
                '        GetAddonIconImg = "<img" _
                '            & " border=0" _
                '            & " id=""" & IconImgID & """" _
                '            & " onMouseOver=""this.style.backgroundPosition='" & (-1 * IconSpriteColumn * IconWidth) & "px -" & (2 * IconHeight) & "px'""" _
                '            & " onMouseOut=""this.style.backgroundPosition='" & (-1 * IconSpriteColumn * IconWidth) & "px 0px'""" _
                '            & " onDblClick=""window.parent.OpenAddonPropertyWindow(this,'" & AdminURL & "');""" _
                '            & " alt=""" & IconAlt & """" _
                '            & " title=""" & IconTitle & """" _
                '            & " src=""/ccLib/images/spacer.gif"""
                '        ImgStyle = "background:url(" & IconFilename & ") " & (-1 * IconSpriteColumn * IconWidth) & "px 0px no-repeat;"
                '        ImgStyle = ImgStyle & "width:" & IconWidth & "px;"
                '        ImgStyle = ImgStyle & "height:" & IconHeight & "px;"
                '        If IconIsInline Then
                '            'GetAddonIconImg = GetAddonIconImg & " align=""middle"""
                '            ImgStyle = ImgStyle & "vertical-align:middle;display:inline;"
                '        Else
                '            ImgStyle = ImgStyle & "display:block;"
                '        End If
                '
                '
                '        'Return_IconStyleMenuEntries = Return_IconStyleMenuEntries & vbCrLf & ",["".icon" & AddonID & """,false,"".icon" & AddonID & """,""background:url(" & IconFilename & ") 0px 0px no-repeat;"
                '        'GetAddonIconImg = "<img" _
                '        '    & " id=""AC,AGGREGATEFUNCTION,0," & FieldName & "," & ArgumentList & """" _
                '        '    & " onMouseOver=""this.style.backgroundPosition=\'0px -" & (2 * IconHeight) & "px\'""" _
                '        '    & " onMouseOut=""this.style.backgroundPosition=\'0px 0px\'""" _
                '        '    & " onDblClick=""window.parent.OpenAddonPropertyWindow(this);""" _
                '        '    & " alt=""" & IconAlt & """" _
                '        '    & " title=""" & IconTitle & """" _
                '        '    & " src=""/ccLib/images/spacer.gif"""
                '        If ACInstanceID <> "" Then
                '            GetAddonIconImg = GetAddonIconImg & " ACInstanceID=""" & ACInstanceID & """"
                '        End If
                '        GetAddonIconImg = GetAddonIconImg & " style=""" & ImgStyle & """>"
                '        'Return_IconStyleMenuEntries = Return_IconStyleMenuEntries & """]"
            End If
        End Function
        '
        '====================================================================================================
        ''' <summary>
        ''' create an icon as a 4 image sprite
        ''' </summary>
        ''' <param name="TagID"></param>
        ''' <param name="SpriteColumn"></param>
        ''' <param name="IconSrc"></param>
        ''' <param name="IconWidth"></param>
        ''' <param name="IconHeight"></param>
        ''' <param name="IconAlt"></param>
        ''' <param name="IconTitle"></param>
        ''' <param name="onDblClick"></param>
        ''' <param name="IconIsInline"></param>
        ''' <param name="ACInstanceID"></param>
        ''' <returns></returns>
        Public Shared Function getIconSprite(TagID As String, SpriteColumn As Long, IconSrc As String, IconWidth As Long, IconHeight As Long, IconAlt As String, IconTitle As String, onDblClick As String, IconIsInline As Boolean, ACInstanceID As String) As String
            '
            Dim ImgStyle As String
            '
            getIconSprite = "<img" _
            & " border=0" _
            & " id=""" & TagID & """" _
            & " onMouseOver=""this.style.backgroundPosition='" & (-1 * SpriteColumn * IconWidth) & "px -" & (2 * IconHeight) & "px';""" _
            & " onMouseOut=""this.style.backgroundPosition='" & (-1 * SpriteColumn * IconWidth) & "px 0px'""" _
            & " onDblClick=""" & onDblClick & """" _
            & " alt=""" & IconAlt & """" _
            & " title=""" & IconTitle & """" _
            & " src=""/ccLib/images/spacer.gif"""
            ImgStyle = "background:url(" & IconSrc & ") " & (-1 * SpriteColumn * IconWidth) & "px 0px no-repeat;"
            ImgStyle = ImgStyle & "width:" & IconWidth & "px;"
            ImgStyle = ImgStyle & "height:" & IconHeight & "px;"
            If IconIsInline Then
                ImgStyle = ImgStyle & "vertical-align:middle;display:inline;"
            Else
                ImgStyle = ImgStyle & "display:block;"
            End If
            If ACInstanceID <> "" Then
                getIconSprite = getIconSprite & " ACInstanceID=""" & ACInstanceID & """"
            End If
            getIconSprite = getIconSprite & " style=""" & ImgStyle & """>"
        End Function
        '
        '====================================================================================================
        ''' <summary>
        ''' get xml attribute
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="node"></param>
        ''' <param name="attrName"></param>
        ''' <returns></returns>
        Friend Shared Function getAttribute(cp As CPBaseClass, node As XmlNode, attrName As String) As String
            Dim result As String = ""
            Try
                Dim attr As XmlAttribute = node.Attributes(attrName)
                If (attr IsNot Nothing) Then
                    result = attr.Value
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
    End Class

End Namespace
