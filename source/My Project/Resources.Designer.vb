﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'This class was auto-generated by the StronglyTypedResourceBuilder
    'class via a tool like ResGen or Visual Studio.
    'To add or remove a member, edit your .ResX file then rerun ResGen
    'with the /str option, or rebuild your VS project.
    '''<summary>
    '''  A strongly-typed resource class, for looking up localized strings, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("Contensive.Addons.Dashboard.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Overrides the current thread's CurrentUICulture property for all
        '''  resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to {
        '''  &quot;defaultWrapper&quot;: {
        '''    &quot;guid&quot;: &quot;{9182DCA4-F88B-4593-A5A0-D9151A7FF342}&quot;
        '''  },
        '''  &quot;nodeList&quot;: {
        '''    &quot;node1&quot;: {
        '''      &quot;key&quot;: &quot;node1&quot;,
        '''      &quot;contentName&quot;: &quot;&quot;,
        '''      &quot;contentGUID&quot;: &quot;&quot;,
        '''      &quot;addonGUID&quot;: &quot;{9A7A1567-CC4A-4AF8-B828-2AD7F9389D25}&quot;,
        '''      &quot;title&quot;: &quot;Current Activity&quot;,
        '''      &quot;state&quot;: 2,
        '''      &quot;sizex&quot;: 231,
        '''      &quot;sizey&quot;: 308,
        '''      &quot;addonArgList&quot;: [],
        '''      &quot;x&quot;: 20,
        '''      &quot;y&quot;: 20,
        '''      &quot;z&quot;: 0,
        '''      &quot;wrapperId&quot;: 0
        '''    },
        '''    &quot;node4&quot;: {
        '''      &quot;key&quot;: &quot;node4&quot;,
        '''      &quot;contentNa [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property defaultConfigJson() As String
            Get
                Return ResourceManager.GetString("defaultConfigJson", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
