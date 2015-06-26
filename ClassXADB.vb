Public Class ClassXADB
    Shared Function execInShellReturnOutput(ByVal command As String) As String
        Dim oProcess As New Process()
        Dim oStartInfo As New ProcessStartInfo("cmd.exe", "/c " + command)
        oStartInfo.UseShellExecute = False
        oStartInfo.RedirectStandardOutput = True
        oStartInfo.CreateNoWindow = True
        oProcess.StartInfo = oStartInfo
        oProcess.Start()
        Dim sOutput As String
        Using oStreamReader As System.IO.StreamReader = oProcess.StandardOutput
            sOutput = oStreamReader.ReadToEnd()
        End Using
        Return sOutput
    End Function
End Class
