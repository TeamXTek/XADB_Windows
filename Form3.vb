Imports XADB.ClassXADB
Public Class Form3
    Private SelectedFloder As String, GoToPath As String, GoToPathBak As String
    Private ChkTypePC As String = My.Application.Info.DirectoryPath + "\XADBChkType.sh", copyfrom As String, copyto As String
    Private createdir As String, execpart As String, execwobb As String, chktypeandroid As String
    Private useRoot As Boolean, useBusybox As Boolean, changeToData As Boolean
    Private FSO As Scripting.FileSystemObject = New Scripting.FileSystemObject
    Private deviceRunning As String

    Private Sub ListBox1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListBox1.DoubleClick
        DeviceFileBrowser_CheckTypeAndEnter()
    End Sub

    Private Sub Form3_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "XADB-Device File Manager-" + deviceRunning
        If changeToData = True Then
            chktypeandroid = " /data/XADBChkType.sh "
        Else
            chktypeandroid = " /sdcard/XADBChkType.sh "
        End If
        execpart = Form1.ADBPath + " -s " + deviceRunning + " shell "
        If useRoot = True Then
            execpart += " su -c "
            ActionToolStripMenuItem.Visible = True
            ToolStripStatusLabel1.Text = "Running file commands as root"
        Else
            ToolStripStatusLabel1.Text = "Running without root premission"
        End If
        execwobb = execpart
        If useBusybox = True Then execpart += " busybox "
        GoToPath = "/"
        DeviceFileBrowser_EnterFloder()
        Call PushChkTypeScript()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If GoToPath <> "/" Then
            Dim backfo As String = GoToPath.TrimEnd("/")
            Dim deletesince = InStrRev(backfo, "/")
            GoToPath = backfo.Remove(deletesince)
            Call DeviceFileBrowser_EnterFloder()
        End If
    End Sub
    
    Private Sub DeviceFileBrowser_CheckTypeAndEnter()
        SelectedFloder = ListBox1.SelectedItem
        GoToPathBak = GoToPath
        GoToPath = Replace(GoToPath + SelectedFloder, vbCr, "")
        Dim sOutput As String = execInShellReturnOutput(execpart + " sh" + chktypeandroid + """" + GoToPath + """")
        If sOutput.StartsWith("file") Then
            Call DeviceFileBrowser_PullFile()
        ElseIf sOutput.StartsWith("folder") Then
            GoToPath += "/"
            Call DeviceFileBrowser_EnterFloder()
        Else
            MsgBox("Couldn't load " + GoToPath + vbCrLf + sOutput)
        End If
    End Sub
    
    Private Sub DeviceFileBrowser_EnterFloder()
        TextBox1.Text = GoToPath
        Dim soutputspilt() As String = Split(execInShellReturnOutput(execwobb + " ls " + """" + GoToPath + """"), vbCrLf)
        Dim a As Byte
        ListBox1.Items.Clear()
        For a = 0 To soutputspilt.GetUpperBound(0)
            ListBox1.Items.Add(soutputspilt(a))
        Next
        ListBox1.SelectedIndex = 0
    End Sub
    
    Private Sub DeviceFileBrowser_PullFile()
        execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " pull " + """" + GoToPath + """")
        execInShellReturnOutput("start " + My.Application.Info.DirectoryPath + "\" + SelectedFloder)
    End Sub

    Private Sub ListBox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles ListBox1.KeyPress
        If e.KeyChar = Chr(13) Then
            DeviceFileBrowser_CheckTypeAndEnter()
        End If
    End Sub

    Private Sub PushChkTypeScript()
        If FSO.FileExists(ChkTypePC) = False Then
            MsgBox("Type checking script was not found!" + vbCrLf + "Please put XADBChkType.sh in " + vbCrLf + My.Application.Info.DirectoryPath)
        End If
        execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " push " + ChkTypePC + chktypeandroid)
    End Sub

    Private Sub DeleteFileOrFolder()
        SelectedFloder = ListBox1.SelectedItem
        GoToPathBak = GoToPath
        GoToPath = Replace(GoToPath + SelectedFloder, vbCr, "")
        execInShellReturnOutput(execpart + " rm -r " + """" + GoToPath + """")
        GoToPath = GoToPathBak
    End Sub

    Private Sub DataAsRwToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DataAsRwToolStripMenuItem.Click
        execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell su -c mount -ro remount,rw /data")
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteToolStripMenuItem.Click
        If MsgBox("Delete " + Replace(ListBox1.SelectedItem, vbCr, "") + " ?", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            Call DeleteFileOrFolder()
            Call DeviceFileBrowser_EnterFloder()
        End If
    End Sub

    Private Sub PullToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PullToolStripMenuItem.Click
        If MsgBox("Pull " + Replace(ListBox1.SelectedItem, vbCr, "") + "?", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            SelectedFloder = ListBox1.SelectedItem
            GoToPathBak = GoToPath
            GoToPath = Replace(GoToPath + SelectedFloder, vbCr, "")
            Call DeviceFileBrowser_PullFile()
        End If
    End Sub

    Private Sub CopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyToolStripMenuItem.Click
        SelectedFloder = ListBox1.SelectedItem
        copyfrom = Replace(GoToPath + SelectedFloder, vbCr, "")
        PasteToolStripMenuItem.Visible = True
    End Sub

    Private Sub PasteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteToolStripMenuItem.Click
        SelectedFloder = ListBox1.SelectedItem
        GoToPathBak = GoToPath
        GoToPath = Replace(GoToPath + SelectedFloder, vbCr, "")
        Dim sOutput As String = execInShellReturnOutput(execpart + " sh" + chktypeandroid + """" + GoToPath + """")
        If sOutput.StartsWith("file") Then
            GoToPath = GoToPathBak
            copyto = GoToPath
        ElseIf sOutput.StartsWith("folder") Then
            copyto = GoToPath
            GoToPath = GoToPathBak
        Else
            MsgBox(sOutput)
            GoToPath = GoToPathBak
        End If
        If MsgBox("Copy " + copyfrom + " to " + copyto + "?", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            execInShellReturnOutput(execpart + "cp " + """" + copyfrom + """" + " " + """" + copyto + """")
            Call DeviceFileBrowser_EnterFloder()
        End If
    End Sub

    Private Sub RenameToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RenameToolStripMenuItem.Click
        SelectedFloder = ListBox1.SelectedItem
        Dim renameto As String = GoToPath + InputBox("Rename " + SelectedFloder + " to")
        Dim renamefrom As String = GoToPath + Replace(SelectedFloder, vbCr, "")
        If MsgBox("Rename " + renamefrom + " to " + renameto + "?", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            execInShellReturnOutput(execpart + " mv " + """" + renamefrom + """" + " " + """" + renameto + """")
            Call DeviceFileBrowser_EnterFloder()
        End If
    End Sub

    Private Sub NewFolderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewFolderToolStripMenuItem.Click
        createdir = GoToPath + InputBox("Enter a name...")
        If MsgBox("Create " + createdir + "?", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            execInShellReturnOutput(execpart + " mkdir " + """" + createdir + """")
            Call DeviceFileBrowser_EnterFloder()
        End If
    End Sub

    Private Sub SystemAsRwToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SystemAsRwToolStripMenuItem.Click
        execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell su -c mount -ro remount,rw /system")
    End Sub

    Private Sub AsRwToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AsRwToolStripMenuItem.Click
        execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell su -c mount -ro remount,rw /")
    End Sub

    Public Sub New(ByVal root As Boolean, ByVal bbox As Boolean, ByVal chtodata As Boolean, ByVal device As String)
        InitializeComponent()
        useRoot = root
        useBusybox = bbox
        changeToData = chtodata
        deviceRunning = device
    End Sub
End Class
