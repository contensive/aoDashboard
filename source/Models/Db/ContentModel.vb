﻿
'Option Explicit On
'Option Strict On

'Imports System
'Imports System.Collections.Generic
'Imports System.Text
'Imports Contensive.BaseClasses

'Namespace Models
'    Public Class ContentModel
'        Inherits Contensive.Addons.Dashboard.Models.BaseModel
'        Implements ICloneable
'        '
'        '====================================================================================================
'        '-- const
'        Public Const contentName As String = "content"
'        Public Const contentTableName As String = "cccontent"
'        Private Shadows Const contentDataSource As String = "default"
'        '
'        '====================================================================================================
'        ' -- instance properties
'        'Public Property AdminOnly As Boolean
'        'Public Property AllowAdd As Boolean
'        'Public Property AllowContentChildTool As Boolean
'        'Public Property AllowContentTracking As Boolean
'        'Public Property AllowDelete As Boolean
'        'Public Property AllowTopicRules As Boolean
'        'Public Property AllowWorkflowAuthoring As Boolean
'        'Public Property AuthoringTableID As Integer
'        'Public Property ContentCategoryID As Integer
'        'Public Property ContentTableID As Integer
'        'Public Property DefaultSortMethodID As Integer
'        'Public Property DeveloperOnly As Boolean
'        'Public Property DropDownFieldList As String
'        'Public Property EditorGroupID As Integer
'        Public Property IconHeight As Integer
'        Public Property IconLink As String
'        Public Property IconSprites As Integer
'        Public Property IconWidth As Integer
'        'Public Property InstalledByCollectionID As Integer
'        'Public Property IsBaseContent As Boolean
'        'Public Property ParentID As Integer
'        '
'        '====================================================================================================
'        Public Overloads Shared Function add(cp As CPBaseClass) As ContentModel
'            Return add(Of ContentModel)(cp)
'        End Function
'        '
'        '====================================================================================================
'        Public Overloads Shared Function create(cp As CPBaseClass, recordId As Integer) As ContentModel
'            Return create(Of ContentModel)(cp, recordId)
'        End Function
'        '
'        '====================================================================================================
'        Public Overloads Shared Function create(cp As CPBaseClass, recordGuid As String) As ContentModel
'            Return create(Of ContentModel)(cp, recordGuid)
'        End Function
'        '
'        '====================================================================================================
'        Public Overloads Shared Function createByName(cp As CPBaseClass, recordName As String) As ContentModel
'            Return createByName(Of ContentModel)(cp, recordName)
'        End Function
'        '
'        '====================================================================================================
'        Public Overloads Sub save(cp As CPBaseClass)
'            MyBase.save(cp)
'        End Sub
'        '
'        '====================================================================================================
'        Public Overloads Shared Sub delete(cp As CPBaseClass, recordId As Integer)
'            delete(Of ContentModel)(cp, recordId)
'        End Sub
'        '
'        '====================================================================================================
'        Public Overloads Shared Sub delete(cp As CPBaseClass, ccGuid As String)
'            delete(Of ContentModel)(cp, ccGuid)
'        End Sub
'        '
'        '====================================================================================================
'        Public Overloads Shared Function createList(cp As CPBaseClass, sqlCriteria As String, Optional sqlOrderBy As String = "id") As List(Of ContentModel)
'            Return createList(Of ContentModel)(cp, sqlCriteria, sqlOrderBy)
'        End Function
'        '
'        '====================================================================================================
'        Public Overloads Shared Function getRecordName(cp As CPBaseClass, recordId As Integer) As String
'            Return BaseModel.getRecordName(Of ContentModel)(cp, recordId)
'        End Function
'        '
'        '====================================================================================================
'        Public Overloads Shared Function getRecordName(cp As CPBaseClass, ccGuid As String) As String
'            Return BaseModel.getRecordName(Of ContentModel)(cp, ccGuid)
'        End Function
'        '
'        '====================================================================================================
'        Public Overloads Shared Function getRecordId(cp As CPBaseClass, ccGuid As String) As Integer
'            Return BaseModel.getRecordId(Of ContentModel)(cp, ccGuid)
'        End Function
'        '
'        '====================================================================================================
'        Public Overloads Shared Function getCount(cp As CPBaseClass, sqlCriteria As String) As Integer
'            Return BaseModel.getCount(Of ContentModel)(cp, sqlCriteria)
'        End Function
'        '
'        '====================================================================================================
'        Public Overloads Function getUploadPath(fieldName As String) As String
'            Return MyBase.getUploadPath(Of ContentModel)(fieldName)
'        End Function
'        '
'        '====================================================================================================
'        '
'        Public Function Clone(cp As CPBaseClass) As ContentModel
'            Dim result As ContentModel = DirectCast(Me.Clone(), ContentModel)
'            result.id = cp.Content.AddRecord(contentName)
'            result.ccguid = cp.Utils.CreateGuid()
'            result.save(cp)
'            Return result
'        End Function
'        '
'        '====================================================================================================
'        '
'        Public Function Clone() As Object Implements ICloneable.Clone
'            Return Me.MemberwiseClone()
'        End Function

'    End Class
'End Namespace
