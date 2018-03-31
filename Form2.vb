Imports System.ComponentModel
Imports XADB.ClassXADB
Public Class Form2
    Private FSO As New Scripting.FileSystemObject
    Private deviceRunning As String
    Private deviceInfoForm As Form4
    Private deviceFileMgrForm As Form3
    Private cmdBase As String

    Private Sub Button2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button2.Click
        OpenFileDialog1.Filter = "APK|*.apk"
        OpenFileDialog1.ShowDialog()
        TextBox1.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        If FSO.FileExists(TextBox1.Text) Then
            execInShellPrintOutputBackground(cmdBase + " install " + """" + TextBox1.Text + """")
        Else
            MsgBox("File " + TextBox1.Text + " does not exist!")
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button3.Click
        RichTextBox1.Text = ""
    End Sub

    Private Sub Button4_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button4.Click
        execInShellPrintOutputBackground(cmdBase + " reboot")
    End Sub

    Private Sub Button5_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button5.Click
        execInShellPrintOutputBackground(cmdBase + " reboot recovery")
    End Sub

    Private Sub Button6_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button6.Click
        execInShellPrintOutputBackground(cmdBase + " reboot bootloader")
    End Sub

    Private Sub Button7_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button7.Click
        execInShellPrintOutputBackground(cmdBase + " shell " + """" + TextBox6.Text + """")
    End Sub

    Private Sub Button8_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button8.Click
        If deviceFileMgrForm Is Nothing OrElse deviceFileMgrForm.IsDisposed Then
            deviceFileMgrForm = New Form3(CheckBox1.Checked, CheckBox2.Checked, deviceRunning)
            deviceFileMgrForm.Show()
        Else
            deviceFileMgrForm.WindowState = FormWindowState.Normal
            deviceFileMgrForm.Select()
        End If
    End Sub

    Private Sub Form2_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Text = "XADB-" + deviceRunning
    End Sub

    Private Sub Button9_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button9.Click
        OpenFileDialog1.Filter = "All Files|*.*"
        OpenFileDialog1.ShowDialog()
        TextBox2.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub Button10_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button10.Click
        If FSO.FileExists(TextBox2.Text) OrElse FSO.FolderExists(TextBox2.Text) Then
            execInShellPrintOutputBackground(cmdBase + " push " + """" + TextBox2.Text + """" + " " + """" + TextBox3.Text + """")
        Else
            MsgBox("File/Folder " + TextBox2.Text + " does Not exist!")
        End If
    End Sub

    Private Sub Button12_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button12.Click
        FolderBrowserDialog1.ShowNewFolderButton = True
        FolderBrowserDialog1.ShowDialog()
        TextBox4.Text = FolderBrowserDialog1.SelectedPath
    End Sub

    Private Sub Button11_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button11.Click
        If FSO.FolderExists(TextBox4.Text) Then
            execInShellPrintOutputBackground(cmdBase + " pull " + """" + TextBox5.Text + """" + " " + """" + TextBox4.Text + """")
        ElseIf TextBox4.Text = ""
            execInShellPrintOutputBackground(cmdBase + " pull " + """" + TextBox5.Text + """")
        Else
            MsgBox("Folder " + TextBox4.Text + " does Not exist!")
        End If
    End Sub

    Private Sub Button13_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button13.Click
        If deviceFileMgrForm IsNot Nothing Then deviceFileMgrForm.Close()
        If deviceInfoForm IsNot Nothing Then deviceInfoForm.Close()
        Me.Close()
    End Sub

    Private Sub Button14_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button14.Click
        FolderBrowserDialog1.ShowNewFolderButton = False
        FolderBrowserDialog1.ShowDialog()
        TextBox2.Text = FolderBrowserDialog1.SelectedPath
    End Sub

    Public Sub New(ByVal device As String)
        InitializeComponent()
        deviceRunning = device
        cmdBase = Form1.ADBPath + "-s " + device
    End Sub

    Private Sub Button15_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button15.Click
        If deviceInfoForm Is Nothing OrElse deviceInfoForm.IsDisposed Then
            deviceInfoForm = New Form4(deviceRunning)
            deviceInfoForm.Show()
        Else
            deviceInfoForm.WindowState = FormWindowState.Normal
            deviceInfoForm.Select()
        End If
    End Sub

    Private Sub TextBox6_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox6.KeyPress
        If e.KeyChar = Chr(13) Then
            execInShellPrintOutputBackground(cmdBase + " shell " + """" + TextBox6.Text + """")
        End If
    End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        RichTextBox1.Text += execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + "root")
        MsgBox("Please select device again!")
        If deviceFileMgrForm IsNot Nothing Then deviceFileMgrForm.Close()
        If deviceInfoForm IsNot Nothing Then deviceInfoForm.Close()
        Me.Close()
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim cmd As String = CType(e.Argument, String)
        e.Result = execInShellReturnOutput(cmd)
    End Sub

    Private Sub execInShellPrintOutputBackground(cmd As String)
        ToolStripStatusLabel1.Text = "Executing: " + cmd
        BackgroundWorker1.RunWorkerAsync(cmd)
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        RichTextBox1.Text += Replace(e.Result, vbCr, "") + vbCrLf
        ToolStripStatusLabel1.Text = "Done"
    End Sub
End Class
