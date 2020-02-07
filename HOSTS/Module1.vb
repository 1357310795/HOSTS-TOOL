Imports System.Security.Principal
Module Module1
    Public pid As Long
    Public psi As New ProcessStartInfo
    Public ps As New Process
    Public ReadOnly vbLQ As Char = Convert.ToChar(8220)
    Public ReadOnly vbRQ As Char = Convert.ToChar(8221)
    Public ReadOnly title As String = "HOSTS文件编辑小工具 v1.0"
    Public ReadOnly HOSTS_dirpath As String = "C:\Windows\System32\drivers\etc\"
    Public ReadOnly HOSTS_filepath As String = "C:\Windows\System32\drivers\etc\HOSTS"

    Public Sub Main()
        Dim pro As Process
        For Each pro In Process.GetProcesses
            If pro.ProcessName = "HOSTS" And pro.Id <> Process.GetCurrentProcess().Id Then
                MsgBox("程序不能多开！", vbExclamation, title)
                End
            End If
        Next
        Application.EnableVisualStyles()
        Dim tmp As String() = My.Application.CommandLineArgs.ToArray
        Dim ID As WindowsIdentity = WindowsIdentity.GetCurrent()
        Dim P As WindowsPrincipal = New WindowsPrincipal(ID)
        If P.IsInRole(WindowsBuiltInRole.Administrator) Then
            'MsgBox("管理员")
            MsgBox("您使用管理员权限打开了本程序，这可能会导致Drag&Drop操作无法使用。建议重新启动程序，双击即可。程序运行后再会请求管理员权限。", vbExclamation, title)
        Else
            'MsgBox("不是管理员")
        End If

        psi.WorkingDirectory = My.Application.Info.DirectoryPath & "\data"
        'psi.Verb = "runas"
        psi.Arguments = "service"
        psi.UseShellExecute = True
        psi.FileName = "HOSTS-SERVER.exe"
        ps = Process.Start(psi)
        pid = ps.Id

        'MsgBox(pid)
        'pid = ShellExecute(0, "runas", Application.ExecutablePath, "uac", vbNullString, vbNormalFocus)
        Dim forma As New Form1
        Application.Run(forma)
    End Sub
End Module
