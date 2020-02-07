Imports System.Net.Sockets
Imports System.Net
Imports System.Text
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports System.Security.Principal
Module Module1
    Public Declare Function ShellExecute Lib "shell32.dll" Alias "ShellExecuteA" (ByVal hwnd As Long, ByVal lpOperation As String, ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, ByVal nShowCmd As Long) As Long
    Public ReadOnly vbLQ As Char = Convert.ToChar(8220)
    Public ReadOnly vbRQ As Char = Convert.ToChar(8221)
    Public ReadOnly title As String = "HOSTS文件编辑小工具"
    Public ReadOnly HOSTS_dirpath As String = "C:\Windows\System32\drivers\etc\"
    Public ReadOnly HOSTS_filepath As String = "C:\Windows\System32\drivers\etc\HOSTS"
    'Public ReadOnly HOSTS_filepath As String = "D:\新建文本文档.txt"
    Public ReadOnly ori As String = "# Copyright (c) 1993-2009 Microsoft Corp." & vbCrLf &
    "#" & vbCrLf &
    "# This is a sample HOSTS file used by Microsoft TCP/IP for Windows." & vbCrLf &
    "#" & vbCrLf &
    "# This file contains the mappings of IP addresses to host names. Each" & vbCrLf &
    "# entry should be kept on an individual line. The IP address should" & vbCrLf &
    "# be placed in the first column followed by the corresponding host name." & vbCrLf &
    "# The IP address and the host name should be separated by at least one" & vbCrLf &
    "# space." & vbCrLf &
    "#" & vbCrLf &
    "# Additionally, comments (such as these) may be inserted on individual" & vbCrLf &
    "# lines or following the machine name denoted by a '#' symbol." & vbCrLf &
    "#" & vbCrLf &
    "# For example:" & vbCrLf &
    "#" & vbCrLf &
    "#      102.54.94.97     rhino.acme.com          # source server" & vbCrLf &
    "#       38.25.63.10     x.acme.com              # x client host" & vbCrLf &
    "" & vbCrLf &
    "# localhost name resolution is handled within DNS itself." & vbCrLf &
    "#       127.0.0.1       localhost" & vbCrLf &
    "#       ::1             localhost" & vbCrLf
    Public Sub Main()
        Application.EnableVisualStyles()
        Dim tmp As String() = My.Application.CommandLineArgs.ToArray
        If tmp.Length = 0 Then
            MsgBox("这个程序不能单独运行，请打开另一个！", vbExclamation, title)
        ElseIf tmp.Length = 1 And tmp(0) = "service" Then
            WaitData()
        Else
            MsgBox("参数错误！这个程序不能单独运行，请打开另一个！", vbCritical, title)
            End
        End If
    End Sub
    Public Sub WaitData()
        Dim s As Socket = Nothing
        Dim c As String() = Nothing
        Try
            s = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) '使用TCP协议
            Dim localEndPoint As New IPEndPoint(IPAddress.Parse("127.0.0.1"), 2345)  '指定IP和Port
            s.Bind(localEndPoint)        '绑定到该Socket
            s.Listen(100)     '侦听，最多接受100个连接
        Catch ex As Exception
            MsgBox("初始化服务时出现错误：" & vbCrLf & ex.Message, vbCritical, title)
        End Try
        'MsgBox("ok")
        While (True)
            Dim bytes(1024) As Byte   '用来存储接收到的字节
            Try
                Dim ss As Socket = s.Accept()  '若接收到,则创建一个新的Socket与之连接
                ss.Receive(bytes)    '接收数据，若用ss.send(Byte()),则发送数据
            Catch ex As Exception
                MsgBox("接收命令时出现错误：" & vbCrLf & ex.Message, vbCritical, title)
            End Try
            'Console.WriteLine(Encoding.Unicode.GetString(bytes)) '将其插入到列表框的第一项之前
            'MsgBox(Encoding.Unicode.GetString(bytes))
            c = Split(Encoding.Unicode.GetString(bytes), vbCrLf) '若使用Encoding.ASCII.GetString(bytes),则接收到的中文字符不能正常显示
            'MsgBox(c(0) = "notepad")
            Select Case c(0)
                Case "notepad"
                    Shell("notepad.exe " & HOSTS_filepath, AppWinStyle.NormalFocus)
                Case "explorer"
                    Shell("explorer.exe " & HOSTS_dirpath, AppWinStyle.NormalFocus)
                Case "Recover_To_ori"
                    Recover_To_ori()
                Case "Copy"
                    File.Copy(c(1), c(2), True)
                Case "Add_To_Hosts"
                    Add_To_Hosts(c(1))
                Case "Replace_Hosts"
                    Replace_Hosts(c(1))
                Case "End"
                    End
            End Select
            'Dim WINDOW_HANDLER As Integer = FindWindow(Nothing, title)
            'If WINDOW_HANDLER = 0 Then End
        End While
    End Sub

    Private Sub Recover_To_ori()
        Dim mess As String = "成功恢复到原版HOSTS文件"
        Dim messicon As MsgBoxStyle = MsgBoxStyle.Information
        Try
            Dim fi As New FileInfo(HOSTS_filepath)
            Dim f1 As StreamWriter = fi.CreateText()
            f1.Write(ori)
            f1.Close()
        Catch ex As Exception
            mess = "发生错误：" & vbCrLf & ex.Message
            messicon = MsgBoxStyle.Critical
        End Try
        MsgBox(mess, messicon, title)
    End Sub

    Private Sub Add_To_Hosts(f1 As String)
        Dim mess As String = "操作成功！"
        Dim messicon As MsgBoxStyle = MsgBoxStyle.Information
        Try
            Dim dat As String = File.ReadAllText(f1, Encoding.Default)
            Dim f As StreamWriter = New StreamWriter(HOSTS_filepath, True)
            f.WriteLine()
            f.Write(dat)
            f.Close()
        Catch ex As Exception
            mess = "发生错误：" & vbCrLf & ex.Message
            messicon = MsgBoxStyle.Critical
        End Try
        MsgBox(mess, messicon, title)
    End Sub

    Private Sub Replace_Hosts(f1 As String)
        Dim mess As String = "操作成功！"
        Dim messicon As MsgBoxStyle = MsgBoxStyle.Information
        Try
            File.Copy(f1, HOSTS_filepath, True)
        Catch ex As Exception
            mess = "发生错误：" & vbCrLf & ex.Message
            messicon = MsgBoxStyle.Critical
        End Try
        MsgBox(mess, messicon, title)
    End Sub
End Module
