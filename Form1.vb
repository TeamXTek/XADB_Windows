Imports XADB.ClassXADB
Public Class Form1
    Public ADBPath As String = My.Application.Info.DirectoryPath + "\adb.exe "
    Public ADBDeviceSelected(10) As String, DeviceIndex As Integer
    Dim ADBexist As New Scripting.FileSystemObject
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        updateADBDevices()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Label4.Text = "Version:" + String.Format(My.Application.Info.Version.ToString)
        If ADBexist.FileExists(ADBPath) Then
            execInShellReturnOutput(ADBPath + "start-server")
            MsgBox("ADB found!!" + vbCrLf + "Now the server is started")
        Else
            MsgBox("ADB not found!!" + vbCrLf + "Copy ADB files to " + My.Application.Info.DirectoryPath + " first!")
            Me.Close()
        End If
    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If ComboBox1.Text <> "" Then
            If ComboBox1.Text.EndsWith("offline") Then
                MsgBox("Device is offline.")
            ElseIf ComboBox1.Text.EndsWith("unauthorized") Then
                MsgBox("Device is unauthorized." + vbCrLf + "Please check the confirmation dialog on your device.")
            ElseIf ComboBox1.Text.EndsWith("recovery") Then
                MsgBox("Device is in recovery mode!" + vbCrLf + "Reboot your device into Android first!")
            ElseIf checkIfDeviceAlreadySelected(ComboBox1.Text) Then
                MsgBox("Window of that device already created!" + vbCrLf + "Just use the window ""XADB-" + ComboBox1.Text + """")
            Else
                ADBDeviceSelected.SetValue(ComboBox1.Text, DeviceIndex)
                DeviceIndex += 1
                Dim newForm2 As Form2 = New Form2(ComboBox1.Text)
                newForm2.Show()
            End If
        Else
            MsgBox("Select a device first")
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        execInShellReturnOutput(ADBPath + "connect " + InputBox("Enter the IP of the wireless device."))
        updateADBDevices()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        execInShellReturnOutput(ADBPath + "kill-server")
        execInShellReturnOutput(ADBPath + "start-server")
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        MsgBox(execInShellReturnOutput(ADBPath + "version"))
    End Sub

    Public Function checkIfDeviceAlreadySelected(ByVal device As String) As Boolean
        For i = ADBDeviceSelected.GetLowerBound(0) To ADBDeviceSelected.GetUpperBound(0)
            If device = ADBDeviceSelected(i) Then Return True
        Next
        Return False
    End Function
    
    Public Sub updateADBDevices()
        RichTextBox1.Text = execInShellReturnOutput(ADBPath + "devices")
        Dim sOutputSpilt As Array = Split(sOutput, vbCrLf)
        ComboBox1.Items.Clear()
        Dim a As Byte
        For a = 1 To sOutputSpilt.GetUpperBound(0)
            If Replace(Replace(sOutputSpilt(a), "device", ""), " ", "") <> "" Then
                ComboBox1.Items.Add(Replace(Replace(sOutputSpilt(a), "device", ""), " ", "").TrimEnd)
            End If
        Next
    End Sub
End Class
