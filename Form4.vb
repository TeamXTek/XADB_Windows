Imports XADB.ClassXADB
Public Class Form4
    Private deviceRunning As String
    Public Sub New(ByVal device As String)
        InitializeComponent()
        deviceRunning = device
    End Sub
    Private Sub Form4_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.Text = "XADB-Device Info-" + deviceRunning
        RichTextBox3.Text = execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell getprop")
        RichTextBox1.Text = Replace(execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell cat /proc/cpuinfo"), vbCr, "")
        refreshMemInfo()
    End Sub
    Private Sub refreshMemInfo()
        RichTextBox2.Text = Replace(execInShellReturnOutput(Form1.ADBPath + "-s " + deviceRunning + " shell cat /proc/meminfo"), vbCr, "")
    End Sub

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
        refreshMemInfo()
    End Sub

End Class