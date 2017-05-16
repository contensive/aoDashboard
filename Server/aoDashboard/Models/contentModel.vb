
Option Explicit On
Option Strict On


Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses
Imports System.Runtime.Serialization.Json

Namespace Contensive.Addons.aoDashboard.Models
    '
    '====================================================================================================
    ' simple entity model pattern
    '   factory pattern load because if a record is not found, must rturn nothing
    '   new() - to allow deserialization (so all methods must pass in cp)
    '   create( cp, id ) - to loads instance properties
    '   saveObject( cp ) - saves instance properties
    '
    Public Class contentModel
        '
        '-- const
        Public Const cnPrimaryContent As String = "content"
        '
        ' -- instance properties
        Public id As Integer
        Public name As String
        Public guid As String
        Public IconFilename As String
        Public IconWidth As Integer
        Public IconHeight As Integer
        Public IconSprites As Integer
        Public IconLink As String
        '
        ' -- publics not exposed to the UI (test/internal data)
        Public createKey As Integer
        '
        ' -- foreign key model relationship - if the id of this object is used as a foreign key in other models, use this pattern to create an on-demand list
        Public ReadOnly Property foreignModelList(cp As CPBaseClass) As List(Of dummyForeignModelClass) '<-- NOTE - set this to the model of the records included in this list, NOT contentModel
            Get
                If (_foreignModelList Is Nothing) Then
                    _foreignModelList = dummyForeignModelClass.getObjectList(cp, id) '<-- NOTE - set this to the model of the records included in this list, NOT contentModel
                End If
                Return _foreignModelList
            End Get
        End Property
        Private _foreignModelList As List(Of dummyForeignModelClass) = Nothing '<-- NOTE - set this to the model of the records included in this list, NOT contentModel
        '
        ' -- just a dummy placeholder
        Public Class dummyForeignModelClass
            Public id As Integer
            Public name As String
            Public requestKey As String ' SPECIAL EXCEPTION - this is field ccGuid
            Public Shared Function getObjectList(cp As CPBaseClass, criteria As Integer) As List(Of dummyForeignModelClass)
                Return Nothing
            End Function
        End Class
        '
        '====================================================================================================
        ''' <summary>
        ''' Create an empty object. needed for deserialization
        ''' </summary>
        Public Sub New()
            '
        End Sub
        '
        '====================================================================================================
        ''' <summary>
        ''' open an existing object
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="recordId"></param>
        Public Shared Function create(cp As CPBaseClass, recordId As Integer) As contentModel
            Dim result As contentModel = Nothing
            Try
                If recordId <> 0 Then
                    result = loadObject2(cp, "id=" & recordId.ToString())
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
                Throw
            End Try
            Return result
        End Function
        '
        '====================================================================================================
        ''' <summary>
        ''' open an existing object
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="recordGuid"></param>
        Public Shared Function createByGuid(cp As CPBaseClass, recordGuid As String) As contentModel
            Dim result As contentModel = Nothing
            Try
                If Not String.IsNullOrEmpty(recordGuid) Then
                    result = loadObject2(cp, "ccGuid=" & cp.Db.EncodeSQLText(recordGuid))
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
                Throw
            End Try
            Return result
        End Function
        '
        '====================================================================================================
        ''' <summary>
        ''' open an existing object
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="recordName"></param>
        Public Shared Function createByName(cp As CPBaseClass, recordName As String) As contentModel
            Dim result As contentModel = Nothing
            Try
                If Not String.IsNullOrEmpty(recordName) Then
                    result = loadObject2(cp, "name=" & cp.Db.EncodeSQLText(recordName))
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
                Throw
            End Try
            Return result
        End Function
        '
        '====================================================================================================
        ''' <summary>
        ''' open an existing object
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="sqlCriteria"></param>
        Private Shared Function loadObject2(cp As CPBaseClass, sqlCriteria As String) As contentModel
            Dim result As contentModel = Nothing
            Try
                Dim cs As CPCSBaseClass = cp.CSNew()
                If cs.Open(cnPrimaryContent, sqlCriteria) Then
                    result = New contentModel
                    With result
                        .id = cs.GetInteger("id")
                        .name = cs.GetText("name")
                        .guid = cs.GetText("ccGuid")
                        .createKey = cs.GetInteger("createKey")
                        .IconFilename = cs.GetText("IconFilename")
                        .IconWidth = cs.GetInteger("IconWidth")
                        .IconHeight = cs.GetInteger("IconHeight")
                        .IconSprites = cs.GetInteger("IconSprites")
                        .IconLink = cs.GetText("IconLink")
                        If (String.IsNullOrEmpty(.guid)) Then
                            .guid = Controllers.genericController.getGUID(cp)
                            cs.SetField("ccGuid", .guid)
                        End If
                    End With
                End If
                Call cs.Close()
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
                Throw
            End Try
            Return result
        End Function
        '
        '====================================================================================================
        '
        Public Function saveObject(cp As CPBaseClass) As Integer
            Try
                Dim cs As CPCSBaseClass = cp.CSNew()
                If (id > 0) Then
                    If Not cs.Open(cnPrimaryContent, "id=" & id) Then
                        id = 0
                        cs.Close()
                        Throw New ApplicationException("Unable to open record in content [" & cnPrimaryContent & "], with id [" & id & "]")
                    End If
                Else
                    If Not cs.Insert(cnPrimaryContent) Then
                        cs.Close()
                        id = 0
                        Throw New ApplicationException("Unable to insert record in content [" & cnPrimaryContent & "]")
                    End If
                End If
                If cs.OK() Then
                    id = cs.GetInteger("id")
                    Call cs.SetField("name", name)
                    Call cs.SetField("ccGuid", guid)
                    Call cs.SetField("createKey", createKey.ToString())
                    Call cs.SetField("IconFilename", IconFilename.ToString())
                    Call cs.SetField("IconWidth", IconWidth.ToString())
                    Call cs.SetField("IconHeight", IconHeight.ToString())
                    Call cs.SetField("IconSprites", IconSprites.ToString())
                    Call cs.SetField("IconLink", IconLink.ToString())
                End If
                Call cs.Close()
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
                Throw
            End Try
            Return id
        End Function
        '
        '====================================================================================================
        ''' <summary>
        ''' delete an existing object
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="someCriteria"></param>
        Public Shared Sub delete(cp As CPBaseClass, someCriteria As Integer)
            Try
                If (someCriteria > 0) Then
                    cp.Content.Delete(cnPrimaryContent, "(someCriteria=" & someCriteria & ")")
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
                Throw
            End Try
        End Sub
        '
        '====================================================================================================
        ''' <summary>
        ''' get a list of objects from this model
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="someCriteria"></param>
        ''' <returns></returns>
        Public Shared Function getObjectList(cp As CPBaseClass, someCriteria As Integer) As List(Of contentModel)
            Dim result As New List(Of contentModel)
            Try
                Dim cs As CPCSBaseClass = cp.CSNew()
                If (cs.Open(cnPrimaryContent, "(someCriteria=" & someCriteria & ")", "name", True, "id")) Then
                    Dim instance As contentModel
                    Do
                        instance = contentModel.create(cp, cs.GetInteger("id"))
                        If (instance IsNot Nothing) Then
                            result.Add(instance)
                        End If
                        cs.GoNext()
                    Loop While cs.OK()
                End If
                cs.Close()
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
    End Class
End Namespace
