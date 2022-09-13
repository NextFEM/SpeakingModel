Imports NextFEMplugin

Public Class pluginS
    Implements NextFEMplugin.IPlugin

    Public Property API As NextFEMapi.API Implements IPlugin.API
    Public Property currentLoadcase As String Implements IPlugin.currentLoadcase
    Public Property currentTime As String Implements IPlugin.currentTime
    Public ReadOnly Property iconFile As String Implements IPlugin.iconFile
        Get
            Return "speak.ico"
        End Get
    End Property

    Public Sub LaunchMe(hnd As IntPtr) Implements IPlugin.LaunchMe
        Dim frm As New mainS
        frm.caller = Me
        frm.Text = Me.PluginName
        frm.Show(System.Windows.Forms.Control.FromHandle(hnd))
    End Sub

    Public ReadOnly Property PluginAuthor As String Implements IPlugin.PluginAuthor
        Get
            Return "NextFEM Support Team"
        End Get
    End Property
    Public ReadOnly Property PluginName As String Implements IPlugin.PluginName
        Get
            Return "Speaking model"
        End Get
    End Property
    Public Event undoCall(undoProperty As undoOps) Implements IPlugin.undoCall
    Public Event updateModel(sender As Object, resize As Boolean, Vstate As ViewState) Implements IPlugin.updateModel
    Friend Sub RequestScreenUpdate(sender As Object, resize As Boolean, Vstate As ViewState)
        RaiseEvent updateModel(sender, resize, Vstate)
    End Sub
    Friend Sub RequestUndo(undoProperty As undoOps)
        RaiseEvent undoCall(undoProperty)
    End Sub
End Class