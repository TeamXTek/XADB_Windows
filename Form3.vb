Imports XADB.ClassXADB
Public Class Form3
    Dim SelectedFloder As String, shellex As String, pro As New Process, GoToPath As String, GoToPathBak As String, ParentPath As String
    Dim ChkTypePC As String = My.Application.Info.DirectoryPath + "\XADBChkType.sh", copyfrom As String, copyto As String
    Dim createdir As String, execpart As String, execwobb As String, chktypeandroid As String
    Dim useRoot As Boolean, useBusybox As Boolean, changeToSd As Boolean
    Dim fileext As Scripting.FileSystemObject = New Scripting.FileSystemObject
    Dim deviceRunning As String

    Private Sub ListBox1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListBox1.DoubleClick
        shellex = execpart
        SelectedFloder = ListBox1.SelectedItem
        GoToPathBak = GoToPath
        GoToPath = Replace(GoToPath + SelectedFloder, vbCr, "")
        shellex = shellex + " sh" + chktypeandroid + GoToPath
        Dim sOutput As String = execInShellReturnOutput(shellex)
        If sOutput.StartsWith("file") Then
            Call DeviceFileBrowser_PullFile()
        ElseIf sOutput.StartsWith("folder") Then
            GoToPath = GoToPath + "/"
            Call DeviceFileBrowser_EnterFloder()
        Else
            If GoToPath.Contains(" ") Then
                MsgBox("Couldn't load " + GoToPath + vbCrLf + "Spaces are not allowed in the name of file / folder.")
            Else
                MsgBox("Couldn't load " + GoToPath + vbCrLf + sOutput)
            End If
        End If
    End Sub

    Private Sub Form3_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "XADB-Device File Manager-" + deviceRunning
        If changeToSd = True Then
            chktypeandroid = " /sdcard/XADBChkType.sh "
        Else
            chktypeandroid = " /data/XADBChkType.sh "
        End If
        execpart = Form1.ADBPath + " -s " + deviceRunning + " shell "
        If useRoot = True Then
            execpart = execpart + " su -c "
            ActionToolStripMenuItem.Visible = True
            shellex = Form1.ADBPath + "-s " + deviceRunning + " shell getprop ro.build.type"
            Dim sOutput2 As String = execInShellReturnOutput(shellex)
            ToolStripStatusLabel1.Text = "Running with root"
            If sOutput2.StartsWith("userdebug") Then
                Call ADBRootAndRemount()
                ToolStripStatusLabel1.Text = ToolStripStatusLabel1.Text + " on userdebug build"
            ElseIf sOutput2.StartsWith("eng") Then
                Call ADBRootAndRemount()
                ToolStripStatusLabel1.Text = ToolStripStatusLabel1.Text + " on eng build"
            End If
        Else
            ToolStripStatusLabel1.Text = "Running without root premission"
        End If
        execwobb = execpart
        If useBusybox = True Then execpart = execpart + " busybox "
        GoToPath = "/"
        shellex = execwobb
        shellex = shellex + " ls"
        Dim sOutput As String = execInShellReturnOutput(shellex)
        Dim soutputspilt() As String = Split(sOutput, vbCrLf)
        Dim a As Byte
        For a = 0 To soutputspilt.GetUpperBound(0)
            ListBox1.Items.Add(soutputspilt(a))
        Next
        ListBox1.SelectedIndex = 0
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


    Private Sub DeviceFileBrowser_EnterFloder()
        TextBox1.Text = GoToPath
        shellex = execwobb + " ls " + GoToPath
        Dim sOutput As String = execInShellReturnOutput(shellex)
        Dim soutputspilt() As String = Split(sOutput, vbCrLf)
        Dim a As Byte
        ListBox1.Items.Clear()
        For a = 0 To soutputspilt.GetUpperBound(0)
            ListBox1.Items.Add(soutputspilt(a))
        Next
        ListBox1.SelectedIndex = 0
    End Sub
    Private Sub DeviceFileBrowser_PullFile()
        shellex = Form1.ADBPath + "-s " + deviceRunning + " pull " + GoToPath
        execInShellReturnOutput(shellex)
        shellex = "start " + My.Application.Info.DirectoryPath + "\" + SelectedFloder
        execInShellReturnOutput(shellex)
    End Sub

    Private Sub ListBox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles ListBox1.KeyPress
        If e.KeyChar = Chr(13) Then
            shellex = execpart
            SelectedFloder = ListBox1.SelectedItem
            GoToPathBak = GoToPath
            GoToPath = Replace(GoToPath + SelectedFloder, vbCr, "")
            shellex = shellex + " sh" + chktypeandroid + GoToPath
            Dim sOutput As String = execInShellReturnOutput(shellex)
            If sOutput.StartsWith("file") Then
                Call DeviceFileBrowser_PullFile()
            ElseIf sOutput.StartsWith("folder") Then
                GoToPath = GoToPath + "/"
                Call DeviceFileBrowser_EnterFloder()
            Else
                If GoToPath.Contains(" ") Then
                    MsgBox("Couldn't load " + GoToPath + vbCrLf + "Spaces are not allowed in the name of file / folder.")
                Else
                    MsgBox("Couldn't load " + GoToPath + vbCrLf + sOutput)
                End If
            End If
        End If
    End Sub

    Public Sub PushChkTypeScript()
        If fileext.FileExists(ChkTypePC) = False Then
            MsgBox("Type checking script was not found!" + vbCrLf + "Please put XADBChkType.sh in " + vbCrLf + My.Application.Info.DirectoryPath)
        End If
        shellex = "/c " + Form1.ADBPath + "-s " + deviceRunning + " push " + ChkTypePC + chktypeandroid
        execInShellReturnOutput(shellex)
    End Sub

    Public Sub DeleteFileOrFolder()
        shellex = execpart
        SelectedFloder = ListBox1.SelectedItem
        GoToPathBak = GoToPath
        GoToPath = Replace(GoToPath + SelectedFloder, vbCr, "")
        shellex = shellex + " sh" + chktypeandroid + GoToPath
        Dim sOutput As String = execInShellReturnOutput(shellex)
        If sOutput.StartsWith("file") Then
            shellex = execpart
            shellex = shellex + " rm " + GoToPath
            execInShellReturnOutput(shellex)
            GoToPath = GoToPathBak
        ElseIf sOutput.StartsWith("folder") Then
            shellex = execpart
            shellex = shellex + " rmdir -r " + GoToPath
            Dim prosi2 As New ProcessStartInfo("cmd.exe", shellex)
            execInShellReturnOutput(shellex)
            GoToPath = GoToPathBak
        Else
            MsgBox(sOutput)
            GoToPath = GoToPathBak
        End If
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteToolStripMenuItem.Click
        If MsgBox("Delete " + Replace(ListBox1.SelectedItem, vbCr, "") + " ?", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            Call DeleteFileOrFolder()
            Call DeviceFileBrowser_EnterFloder()
        End If

    End Sub
    Public Sub ADBRootAndRemount()
        shellex = "/c " + Form1.ADBPath + "-s " + deviceRunning + " root"
        execInShellReturnOutput(shellex)
        shellex = "/c " + Form1.ADBPath + "-s " + deviceRunning + " remount"
        execInShellReturnOutput(shellex)
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
        If SelectedFloder.Contains(" ") Then
            MsgBox("No spaces are allowed in name!")
        Else
            copyfrom = Replace(GoToPath + SelectedFloder, vbCr, "")
            PasteToolStripMenuItem.Visible = True
        End If
    End Sub

    Private Sub PasteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteToolStripMenuItem.Click
        shellex = execpart
        SelectedFloder = ListBox1.SelectedItem
        GoToPathBak = GoToPath
        GoToPath = Replace(GoToPath + SelectedFloder, vbCr, "")
        shellex = shellex + " sh" + chktypeandroid + GoToPath
        Dim sOutput As String = execInShellReturnOutput(shellex)
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
            shellex = execpart
            shellex = shellex + "cp " + copyfrom + " " + copyto
            execInShellReturnOutput(shellex)
            Call DeviceFileBrowser_EnterFloder()
        End If
    End Sub

    Private Sub RenameToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RenameToolStripMenuItem.Click
        SelectedFloder = ListBox1.SelectedItem
        If SelectedFloder.Contains(" ") Then
            MsgBox("No spaces are allowed in name!")
        Else
            Dim renameto As String = GoToPath + InputBox("Rename " + SelectedFloder + " to")
            Dim renamefrom As String = GoToPath + Replace(SelectedFloder, vbCr, "")
            If renameto.Contains(" ") Then
                MsgBox("No spaces are allowed in name!")
            Else
                If MsgBox("Rename " + renamefrom + " to " + renameto + "?", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
                    shellex = execpart
                    shellex = shellex + " mv " + renamefrom + " " + renameto
                    execInShellReturnOutput(shellex)
                    Call DeviceFileBrowser_EnterFloder()
                End If
            End If
        End If
    End Sub

    Private Sub NewFolderToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewFolderToolStripMenuItem.Click
        createdir = GoToPath + InputBox("Enter a name...")
        If createdir.Contains(" ") Then
            MsgBox("No spaces are allowed in name!")
        Else
            If MsgBox("Create " + createdir + "?", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
                shellex = execpart
                shellex = shellex + " mkdir " + createdir
                execInShellReturnOutput(shellex)
                Call DeviceFileBrowser_EnterFloder()
            End If
        End If
    End Sub

    Private Sub SystemAsRwToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SystemAsRwToolStripMenuItem.Click
        shellex = "/c " + Form1.ADBPath + "-s " + deviceRunning + " shell su -c mount -ro remount,rw /system"
        execInShellReturnOutput(shellex)
    End Sub

    Private Sub AsRwToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AsRwToolStripMenuItem.Click
        shellex = "/c " + Form1.ADBPath + "-s " + deviceRunning + " shell su -c mount -ro remount,rw /"
        execInShellReturnOutput(shellex)
    End Sub
    Public Sub New(ByVal root As Boolean, ByVal bbox As Boolean, ByVal chtosd As Boolean, ByVal device As String)
        InitializeComponent()
        useRoot = root
        useBusybox = bbox
        changeToSd = chtosd
        deviceRunning = device
    End Sub
End Class