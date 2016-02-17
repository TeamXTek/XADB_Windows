Imports XADB.ClassXADB
Public Class Form4
    Private deviceRunning As String
    Public Sub New(ByVal device As String)
        InitializeComponent()
        deviceRunning = device
    End Sub
    Private Sub Form4_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "XADB-Device Info-" + deviceRunning
        Label2.Text = execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell getprop ro.product.manufacturer") _
        + execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell getprop ro.product.device") _
        + execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell getprop ro.build.version.release") _
        + execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell getprop ro.build.type") _
        + execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell getprop ro.build.date") _
        + execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell getprop ro.build.description")
        Dim cmVer As String = execInShellReturnOutput("adb -s " + deviceRunning + " shell getprop ro.cm.version")
        If Replace(cmVer, vbCrLf, "").TrimEnd <> "" Then
            Label1.Text = Label1.Text + vbCrLf + vbCrLf + "Cyanogenmod version:"
            Label2.Text = Label2.Text + cmVer
            Label1.Text = Label1.Text + vbCrLf + vbCrLf + "Cyanogenmod codename:"
            Label2.Text = Label2.Text + execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell getprop ro.cm.device")
        End If
        RichTextBox1.Text = Replace(execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell cat /proc/cpuinfo"), vbCr, "")
        refreshMemInfo()
    End Sub
    Private Sub refreshMemInfo()
        RichTextBox2.Text = Replace(execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell cat /proc/meminfo"), vbCr, "")
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        refreshMemInfo()
    End Sub
End Class