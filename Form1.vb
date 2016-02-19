Imports XADB.ClassXADB
Public Class Form1
    Public ADBPath As String
    Private FSO As New Scripting.FileSystemObject
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        updateADBDevices()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Label4.Text = "XADB version " + String.Format(My.Application.Info.Version.ToString) + vbCrLf
        ADBPath = My.Application.Info.DirectoryPath + "\adb.exe "
        If FSO.FileExists(ADBPath) Then
            execInShellReturnOutput(ADBPath + "start-server")
            updateADBDevices()
        ElseIf execInShellReturnOutput("adb version").Contains("version")
            ADBPath = "adb "
            execInShellReturnOutput(ADBPath + "start-server")
            updateADBDevices()
        Else
            MsgBox("ADB Not found!!" + vbCrLf + "Copy ADB files To " + My.Application.Info.DirectoryPath + " or add adb to %PATH% first!")
            Me.Close()
        End If
        Label4.Text+=execInShellReturnOutput(ADBPath + "version")
    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If ComboBox1.Text = "" Then
            MsgBox("Select a device first.")
        ElseIf ComboBox1.Text.EndsWith("offline") Then
            MsgBox("Device Is offline.")
        ElseIf ComboBox1.Text.EndsWith("unauthorized") Then
            MsgBox("Device Is unauthorized." + vbCrLf + "Please check the confirmation dialog On your device.")
        ElseIf ComboBox1.Text.EndsWith("recovery") Then
            MsgBox("Device Is In recovery mode!" + vbCrLf + "Reboot your device into Android first!")
        Else
            Dim newForm2 As Form2 = New Form2(ComboBox1.Text)
            newForm2.Show()
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        execInShellReturnOutput(ADBPath + "connect " + InputBox("Enter the IP address And port Of the wireless device."))
        updateADBDevices()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        execInShellReturnOutput(ADBPath + "kill-server")
        execInShellReturnOutput(ADBPath + "start-server")
    End Sub

    Private Sub updateADBDevices()
        Dim sOutput As String = execInShellReturnOutput(ADBPath + "devices")
        RichTextBox1.Text = sOutput
        Dim sOutputSpilt As Array = Split(sOutput, vbCrLf)
        ComboBox1.Items.Clear()
        Dim a As Byte
        For a = 1 To sOutputSpilt.GetUpperBound(0)
            Dim temp As String = Replace(Replace(Replace(sOutputSpilt(a), "device", ""), " ", ""), vbTab, "")
            If temp <> "" Then ComboBox1.Items.Add(temp)
        Next
    End Sub
End Class
