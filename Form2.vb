Imports XADB.ClassXADB
Public Class Form2
    Public fileext As New Scripting.FileSystemObject
    Dim deviceRunning As String

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        OpenFileDialog1.Filter = "APK|*.apk"
        OpenFileDialog1.ShowDialog()
        TextBox1.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If fileext.FileExists(TextBox1.Text) Then
            RichTextBox1.Text += execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " install " + TextBox1.Text) + vbCrLf
        Else
            MsgBox("File " + TextBox1.Text + " does not exist!")
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        RichTextBox1.Text = ""
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        RichTextBox1.Text += execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " reboot") + vbCrLf
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        RichTextBox1.Text += execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " reboot recovery") + vbCrLf
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        RichTextBox1.Text += execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " reboot bootloader") + vbCrLf
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        RichTextBox1.Text += Replace(execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell " + TextBox6.Text), vbCr, "") + vbCrLf
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Dim newForm3 As Form3 = New Form3(CheckBox1.Checked, CheckBox2.Checked, CheckBox3.Checked, deviceRunning)
        newForm3.Show()
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "XADB-" + deviceRunning
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        OpenFileDialog1.Filter = "All Files|*.*"
        OpenFileDialog1.ShowDialog()
        TextBox2.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        If fileext.FileExists(TextBox2.Text) OrElse fileext.FolderExists(TextBox2.Text) Then
            RichTextBox1.Text += execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " push " + TextBox2.Text + " " + TextBox3.Text) + vbCrLf
        Else
            MsgBox("File/Folder " + TextBox2.Text + " does not exist!")
        End If
    End Sub

    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        FolderBrowserDialog1.ShowNewFolderButton = True
        FolderBrowserDialog1.ShowDialog()
        TextBox4.Text = FolderBrowserDialog1.SelectedPath
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        If fileext.FolderExists(TextBox4.Text) OrElse (TextBox4.Text = "") Then
            RichTextBox1.Text += execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " pull " + TextBox5.Text + " " + TextBox4.Text) + vbCrLf
        Else
            MsgBox("Folder " + TextBox4.Text + " does not exist!")
        End If
    End Sub

    Private Sub DeviceFileManagerToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeviceFileManagerToolStripMenuItem.Click
        MsgBox("Device File Manager 說明......" + vbCrLf + "在資料夾上雙擊進入資料夾" + vbCrLf + "雙擊檔案Pull及打開它" _
               + vbCrLf + "按右鍵招喚選單" + vbCrLf + "開需要Root的檔案須為userdebug build/eng build" + vbCrLf + "若出現sh:......之類的錯誤試試切換「Change script directory to /sdcard」選項" _
               + vbCrLf + "出現not found之類的試試「Run with Busybox」", MsgBoxStyle.OkOnly, "Help")
    End Sub

    Private Sub InstallApkToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InstallApkToolStripMenuItem.Click
        MsgBox("Install an apk 說明......" + vbCrLf + "APK檔的路徑請避免有空格" + vbCrLf + "Success代表安裝成功" + vbCrLf + "INSTALL_FAIL_OLDER_SDK代表你的裝置不支援該程式", MsgBoxStyle.OkOnly, "Help")
    End Sub


    Private Sub PushPullToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PushPullToolStripMenuItem.Click
        MsgBox("Push 和 Pull 說明......" + vbCrLf + "路徑請避免空格" + vbCrLf + "成功不會顯示任何東西", MsgBoxStyle.OkOnly, "Help")
    End Sub

    Private Sub GoToTeamWALADBlogForMoreHelpToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GoToTeamWALADBlogForMoreHelpToolStripMenuItem.Click
        Shell("cmd.exe /c start http://blog.xuite.net/david20000612/TeamWALAD")
    End Sub

    Private Sub Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click
        Me.Close()
    End Sub

    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        FolderBrowserDialog1.ShowNewFolderButton = False
        FolderBrowserDialog1.ShowDialog()
        TextBox2.Text = FolderBrowserDialog1.SelectedPath
    End Sub
    
    Public Sub New(ByVal device As String)
        InitializeComponent()
        deviceRunning = device
    End Sub

    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        Dim newForm4 As Form4 = New Form4(deviceRunning)
        newForm4.Show()
    End Sub
End Class
