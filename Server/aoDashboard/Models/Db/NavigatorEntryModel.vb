
Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses

Namespace Models
    Public Class NavigatorEntryModel
        Inherits BaseModel
        Implements ICloneable
        '
        '====================================================================================================
        '-- const
        Public Const contentName As String = "Navigator Entries"
        Public Const contentTableName As String = "ccMenuEntries"
        Private Shadows Const contentDataSource As String = "default"
        '
        '====================================================================================================
        ''' <summary>
        ''' The link to the entries action
        ''' </summary>
        ''' <returns></returns>
        Public Property linkpage As String
        Public Property addonId As Integer
        Public Property contentId As Integer

        '
        '====================================================================================================
        Public Overloads Shared Function add(cp As CPBaseClass) As NavigatorEntryModel
            Return add(Of NavigatorEntryModel)(cp)
        End Function
        '
        '====================================================================================================
        Public Overloads Shared Function create(cp As CPBaseClass, recordId As Integer) As NavigatorEntryModel
            Return create(Of NavigatorEntryModel)(cp, recordId)
        End Function
        '
        '====================================================================================================
        Public Overloads Shared Function create(cp As CPBaseClass, recordGuid As String) As NavigatorEntryModel
            Return create(Of NavigatorEntryModel)(cp, recordGuid)
        End Function
        '
        '====================================================================================================
        Public Overloads Shared Function createByName(cp As CPBaseClass, recordName As String) As NavigatorEntryModel
            Return createByName(Of NavigatorEntryModel)(cp, recordName)
        End Function
        '
        '====================================================================================================
        Public Overloads Sub save(cp As CPBaseClass)
            MyBase.save(cp)
        End Sub
        '
        '====================================================================================================
        Public Overloads Shared Sub delete(cp As CPBaseClass, recordId As Integer)
            delete(Of NavigatorEntryModel)(cp, recordId)
        End Sub
        '
        '====================================================================================================
        Public Overloads Shared Sub delete(cp As CPBaseClass, ccGuid As String)
            delete(Of NavigatorEntryModel)(cp, ccGuid)
        End Sub
        '
        '====================================================================================================
        Public Overloads Shared Function createList(cp As CPBaseClass, sqlCriteria As String, Optional sqlOrderBy As String = "id") As List(Of NavigatorEntryModel)
            Return createList(Of NavigatorEntryModel)(cp, sqlCriteria, sqlOrderBy)
        End Function
        '
        '====================================================================================================
        Public Overloads Shared Function getRecordName(cp As CPBaseClass, recordId As Integer) As String
            Return BaseModel.getRecordName(Of NavigatorEntryModel)(cp, recordId)
        End Function
        '
        '====================================================================================================
        Public Overloads Shared Function getRecordName(cp As CPBaseClass, ccGuid As String) As String
            Return BaseModel.getRecordName(Of NavigatorEntryModel)(cp, ccGuid)
        End Function
        '
        '====================================================================================================
        Public Overloads Shared Function getRecordId(cp As CPBaseClass, ccGuid As String) As Integer
            Return BaseModel.getRecordId(Of NavigatorEntryModel)(cp, ccGuid)
        End Function
        '
        '====================================================================================================
        Public Overloads Shared Function getCount(cp As CPBaseClass, sqlCriteria As String) As Integer
            Return BaseModel.getCount(Of NavigatorEntryModel)(cp, sqlCriteria)
        End Function
        '
        '====================================================================================================
        Public Overloads Function getUploadPath(fieldName As String) As String
            Return MyBase.getUploadPath(Of NavigatorEntryModel)(fieldName)
        End Function
        '
        '====================================================================================================
        '
        Public Function clone(cp As CPBaseClass) As NavigatorEntryModel
            Dim result As NavigatorEntryModel = DirectCast(Me.Clone(), NavigatorEntryModel)
            result.id = cp.Content.AddRecord(contentName)
            result.ccguid = cp.Utils.CreateGuid()
            result.save(cp)
            Return result
        End Function
        '
        '====================================================================================================
        '
        Public Function Clone() As Object Implements ICloneable.Clone
            Return Me.MemberwiseClone()
        End Function

    End Class
End Namespace
