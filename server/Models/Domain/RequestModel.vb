
Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports Contensive.BaseClasses

Namespace Models
    Public Class RequestModel
        ''' <summary>
        ''' the node being acted on
        ''' </summary>
        ''' <returns></returns>
        Public Property key As String
        ''' <summary>
        ''' the x (left) position of the node being acted on
        ''' </summary>
        ''' <returns></returns>
        Public Property x As Integer
        ''' <summary>
        ''' the y (top) position of the node being acted on
        ''' </summary>
        ''' <returns></returns>
        Public Property y As Integer
        ''' <summary>
        ''' A key for new nodes being added. 
        ''' Created by Admin Navigator (and maybe others later)
        ''' For addons, the key is a followed by the addon Id
        ''' </summary>
        ''' <returns></returns>
        Public Property id As String
        ''' <summary>
        ''' constructor
        ''' </summary>
        ''' <param name="cp"></param>
        Public Sub New(cp As CPBaseClass)
            key = cp.Doc.GetText("key")
            x = cp.Utils.EncodeInteger(cp.Doc.GetText("x").ToLower().Replace("px", ""))
            y = cp.Utils.EncodeInteger(cp.Doc.GetText("y").ToLower().Replace("px", ""))
            id = cp.Doc.GetText("id")
        End Sub
    End Class
End Namespace
