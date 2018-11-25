Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Controllers
    Public Class genericController

        Public Shared Function GetGridWrapper(cp As CPBaseClass, Title As String, Content As String, Width As Integer, Height As Integer) As String
            Dim result As String = ""
            Try
                Dim SpacerHeight As Integer = cp.Utils.EncodeInteger(Height) - 38
                Dim InsideWidth As Integer = cp.Utils.EncodeInteger(Width)
                Dim InsideHeight As Integer = cp.Utils.EncodeInteger(Height) - 70
                result = result & "<table cellpadding=""0"" cellspacing=""0"" border=""0"">"
                result = result & "<tr>"
                result = result & "<td><img src=""/dashboard/container_01.png""></td>"
                result = result & "<td width=""" & InsideWidth & """><img src=""/dashboard/container_02.png"" height=""13"" width=""100%""></td>"
                result = result & "<td><img src=""/dashboard/container_03.png""></td>"
                result = result & "</tr>"
                result = result & "<tr>"
                result = result & "<td style=""BACKGROUND: url(/dashboard/container_04.png) repeat-y left top"">"
                result = result & "<img src=""/ccLib/images/spacer.gif"" height=""" & SpacerHeight & """ width=""12"">"
                result = result & "</td>"
                result = result & "<td valign=""top"" style=""BACKGROUND: url(/dashboard/container_05.png) left top"">"
                result = result & "<div class=""dodadTitle"">" & Title & "</div>"
                result = result & "<div style=""overflow: auto; width: 100%; height: " & InsideHeight & ";"">"
                result = result & Content
                result = result & "</div>"
                result = result & "</td>"
                result = result & "<td style=""BACKGROUND: url(/dashboard/container_06.png) repeat-y left top"">"
                result = result & "<img src=""/ccLib/images/spacer.gif"" height=""" & SpacerHeight & """ width=""18"">"
                result = result & "</td>"
                result = result & "</tr>"
                result = result & "<tr>"
                result = result & "<td><img src=""/dashboard/container_07.png""></td>"
                result = result & "<td width=""" & InsideWidth & """><img src=""/dashboard/container_08.png"" height=""22"" width=""100%""></td>"
                result = result & "<td><img src=""/dashboard/container_09.png""></td>"
                result = result & "</tr>"
                result = result & "</table>"
            Catch ex As ArgumentException
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function


        '
        Public Shared Function encodeSqlTextLike(cp As CPBaseClass, source As String) As String
            Dim sqlText As String = cp.Db.EncodeSQLText(source)
            If sqlText.Length <= 2 Then
                Return String.Empty
            Else
                Return "'%" & sqlText.Substring(1, source.Length - 2) & "%'"
            End If
        End Function
        '
        '====================================================================================================
        ''' <summary>
        ''' return a normalized guid in registry format
        ''' </summary>
        ''' <param name="CP"></param>
        ''' <param name="registryFormat"></param>
        ''' <returns></returns>
        Public Shared Function getGUID(ByVal CP As BaseClasses.CPBaseClass, Optional ByRef registryFormat As Boolean = False) As String
            Dim result As String = ""
            Try
                Dim g As Guid = Guid.NewGuid
                If g <> Guid.Empty Then
                    result = g.ToString
                    '
                    If result <> "" Then
                        result = If(registryFormat, result, "{" & result & "}")
                    End If
                End If
            Catch ex As Exception
                CP.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
        '
        '
        '
        Public Shared Function GetAddonIconImg(AdminURL As String, IconWidth As Long, IconHeight As Long, IconSprites As Long, IconIsInline As Boolean, IconImgID As String, IconFilename As String, serverFilePath As String, IconAlt As String, IconTitle As String, ACInstanceID As String, IconSpriteColumn As Long) As String
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
                GetAddonIconImg = "<img" _
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
                    GetAddonIconImg = GetAddonIconImg & " width=""" & IconWidth & "px"""
                End If
                If IconHeight <> 0 Then
                    GetAddonIconImg = GetAddonIconImg & " height=""" & IconHeight & "px"""
                End If
                If IconIsInline Then
                    GetAddonIconImg = GetAddonIconImg & " style=""vertical-align:middle;display:inline;"" "
                Else
                    GetAddonIconImg = GetAddonIconImg & " style=""display:block"" "
                End If
                If ACInstanceID <> "" Then
                    GetAddonIconImg = GetAddonIconImg & " ACInstanceID=""" & ACInstanceID & """"
                End If
                GetAddonIconImg = GetAddonIconImg & ">"
            Else
                '
                ' Sprite Icon
                '
                GetAddonIconImg = GetIconSprite(IconImgID, IconSpriteColumn, IconFilename, IconWidth, IconHeight, IconAlt, IconTitle, "window.parent.OpenAddonPropertyWindow(this,'" & AdminURL & "');", IconIsInline, ACInstanceID)
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
        Public Shared Function GetIconSprite(TagID As String, SpriteColumn As Long, IconSrc As String, IconWidth As Long, IconHeight As Long, IconAlt As String, IconTitle As String, onDblClick As String, IconIsInline As Boolean, ACInstanceID As String) As String
            '
            Dim ImgStyle As String
            '
            GetIconSprite = "<img" _
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
                GetIconSprite = GetIconSprite & " ACInstanceID=""" & ACInstanceID & """"
            End If
            GetIconSprite = GetIconSprite & " style=""" & ImgStyle & """>"
        End Function
        '
        ' -- getAttribute
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
        '
        ' -- build an attribute
        Friend Shared Function createAttribute(doc As XmlDocument, attrName As String, attrValue As String) As XmlAttribute
            Dim returnAttr As XmlAttribute = doc.CreateAttribute(attrName)
            returnAttr.Value = attrValue
            Return returnAttr
        End Function
    End Class

End Namespace
