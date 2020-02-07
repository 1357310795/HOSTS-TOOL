Imports System.IO
Imports System.Net.Sockets
Imports System.Net
Imports System.Text
Imports System.ComponentModel

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = ImageList1.Images(0)
        PictureBox2.Image = ImageList1.Images(2)
        PictureBox1.AllowDrop = True
        PictureBox2.AllowDrop = True
        Me.Text = title
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Shell("notepad.exe " & HOSTS_filepath, AppWinStyle.NormalFocus)
        SendCommand("notepad")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'Shell("explorer.exe " & HOSTS_dirpath, AppWinStyle.NormalFocus)
        SendCommand("explorer")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'Recover_To_ori()
        SendCommand("Recover_To_ori")
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        With SaveFileDialog1
            .Filter = "bak Files (*.bak)|*.bak"
            .Title = "请选择导出位置 - " & title
            .FileName = "HOSTS_BACKUP_" & Replace(Replace(Replace(CStr(Now), ":", "_"), " ", "_"), "/", "_")
        End With

        If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
            'File.Copy(HOSTS_filepath, SaveFileDialog1.FileName, True)
            SendCommand("Copy" & vbCrLf & HOSTS_filepath & vbCrLf & SaveFileDialog1.FileName)
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        With OpenFileDialog1
            .Filter = "All Files (*.*)|*.*"
            .Title = "请选择HOSTS文件 - " & title
            .FileName = ""
            .Multiselect = False
        End With

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim res = MsgBox("点" & vbLQ & "是" & vbRQ & "以添加到系统HOSTS文件；" & vbCrLf & "点" & vbLQ & "否" & vbRQ & "以覆盖系统HOSTS文件。", vbQuestion + vbYesNoCancel, title)
            If res = vbYes Then
                'Add_To_Hosts(OpenFileDialog1.FileName)
                SendCommand("Add_To_Hosts" & vbCrLf & OpenFileDialog1.FileName)
            ElseIf res = vbNo Then
                'Replace_Hosts(OpenFileDialog1.FileName)
                SendCommand("Replace_Hosts" & vbCrLf & OpenFileDialog1.FileName)
            End If
        End If
    End Sub


    Private Sub PictureBox1_DragDrop(sender As Object, e As DragEventArgs) Handles PictureBox1.DragDrop
        Console.WriteLine("PictureBox1_DragDrop")
        PictureBox1.Image = ImageList1.Images(0)
        Dim f = e.Data.GetData(DataFormats.FileDrop)
        If f IsNot Nothing Then
            If UBound(f) > 0 Then
                MsgBox("只能选择一个文件！", vbCritical, title)
            Else
                If File.Exists(f(0)) Then
                    Console.WriteLine(f(0))
                    'Replace_Hosts(f(0))
                    SendCommand("Replace_Hosts" & vbCrLf & f(0))
                End If
            End If
        End If
    End Sub

    Private Sub PictureBox1_DragOver(sender As Object, e As DragEventArgs) Handles PictureBox1.DragOver
        Console.WriteLine("PictureBox1_DragOver")
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub PictureBox1_DragEnter(sender As Object, e As DragEventArgs) Handles PictureBox1.DragEnter
        Console.WriteLine("PictureBox1_DragEnter")
        PictureBox1.Image = ImageList1.Images(1)
    End Sub

    Private Sub PictureBox1_DragLeave(sender As Object, e As EventArgs) Handles PictureBox1.DragLeave
        Console.WriteLine("PictureBox1_DragLeave")
        PictureBox1.Image = ImageList1.Images(0)
    End Sub

    Private Sub PictureBox2_DragDrop(sender As Object, e As DragEventArgs) Handles PictureBox2.DragDrop
        Console.WriteLine("PictureBox2_DragDrop")
        PictureBox2.Image = ImageList1.Images(2)
        Dim f = e.Data.GetData(DataFormats.FileDrop)
        If f IsNot Nothing Then
            If UBound(f) > 0 Then
                MsgBox("只能选择一个文件！", vbCritical, title)
            Else
                If File.Exists(f(0)) Then
                    Console.WriteLine(f(0))
                    'Add_To_Hosts(f(0))
                    SendCommand("Add_To_Hosts" & vbCrLf & f(0))
                End If
            End If
        End If
    End Sub

    Private Sub PictureBox2_DragOver(sender As Object, e As DragEventArgs) Handles PictureBox2.DragOver
        Console.WriteLine("PictureBox2_DragOver")
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub PictureBox2_DragEnter(sender As Object, e As DragEventArgs) Handles PictureBox2.DragEnter
        Console.WriteLine("PictureBox2_DragEnter")
        PictureBox2.Image = ImageList1.Images(3)
    End Sub

    Private Sub PictureBox2_DragLeave(sender As Object, e As EventArgs) Handles PictureBox2.DragLeave
        Console.WriteLine("PictureBox2_DragLeave")
        PictureBox2.Image = ImageList1.Images(2)
    End Sub

    Private Sub SendCommand(c As String)
        Try
            Dim bytes(1024) As Byte
            Dim s = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            Dim localEndPoint As New IPEndPoint(IPAddress.Parse("127.0.0.1"), 2345)
            s.Connect(localEndPoint)
            s.Send(Encoding.Unicode.GetBytes(c & vbCrLf))
            s.Close()
        Catch ex As Exception
            MsgBox("发送命令时出现错误：" & vbCrLf & ex.Message, vbCritical, title)
        End Try
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        SendCommand("End")
        If Not ps.HasExited Then ps.Kill()
    End Sub

    Private Sub 退出ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 退出ToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub 关于ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 关于ToolStripMenuItem.Click
        frmAbout.Show()
    End Sub
End Class