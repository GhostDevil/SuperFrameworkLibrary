using System;
using System.Runtime.InteropServices;  //DllImport名空间
using System.Windows.Forms; //MouseEventHandler名空间
using System.Reflection; //Assembly名空间
using System.ComponentModel; //Win32Exception名空间
using System.Diagnostics;//PROCESS命名空间
using static SuperFramework.SuperHook.SuperHookStruct;

namespace SuperFramework.SuperHook
{
    public class SuperMouseKeyHook
    {
        object objlock = new object();
        /// <summary>
        /// 安装钩子
        /// </summary>
        /// <param name="idHook">钩子类型 13键盘和14鼠标,用来对底层输入事件监视</param>
        /// <param name="lpfn">函数指针</param>
        /// <param name="hMod">包含SetWindowsHookEx函数的模块地址,一般来说是你钩子回调函数所在的应用程序实例模块句柄</param>
        /// <param name="dwThreadId">0表示系统钩子</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        [DllImport("user32")]
        private static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleA", SetLastError = true, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = true)]
        public static extern IntPtr GetModuleHandleA(string lpModuleName);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
        public static extern IntPtr GetModuleHandleW(string lpModuleName);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32.dll", SetLastError= true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        //消息参数的值
        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE = 7;
        private const int WH_KEYBOARD = 2;
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;
        private const int WM_MOUSEWHEEL = 0x020A;

        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        private const byte VK_SHIFT = 0x10;
        private const byte VK_CAPITAL = 0x14;
        private const byte VK_NUMLOCK = 0x90;
        
        public SuperMouseKeyHook()
        {
            //Start(intPtr, InstallMouseHook, InstallKeyboardHook, theadId);
        }

        ~SuperMouseKeyHook()
        {
            Stop(true, true, false);
        }

        public event MouseEventHandler OnMouseActivity; //MouseEventHandler是委托，表示处理窗体、控件或其他组件的 MouseDown、MouseUp 或 MouseMove 事件的方法。
        public event KeyEventHandler KeyDown;
        public event KeyPressEventHandler KeyPress;
        public event KeyEventHandler KeyUp;

        private int hMouseHook = 0; //标记mouse hook是否安装
        private int hKeyboardHook = 0;

        private static HookProc MouseHookProcedure;
        private static HookProc KeyboardHookProcedure;

        public void Start()
        {
            Start(IntPtr.Zero, true, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="intPtr">安装对象句柄，IntPtr.Zero为本程序</param>
        /// <param name="InstallMouseHook"></param>
        /// <param name="InstallKeyboardHook"></param>
        /// <param name="theadId"></param>
        /// <exception cref="Win32Exception"></exception>
        public void Start(IntPtr intPtr, bool InstallMouseHook=true, bool InstallKeyboardHook=true,int theadId=0)
        {
            lock (objlock)
            {
                IntPtr HM = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
                if (intPtr == IntPtr.Zero)
                    intPtr = GetModuleHandleW(Process.GetCurrentProcess().MainModule.ModuleName);//本进程模块句柄
                var mar = LoadLibrary("user32.dll");
                if (hMouseHook == 0 && InstallMouseHook)
                {
                    MouseHookProcedure = new HookProc(MouseHookProc);//钩子的处理函数
                    hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProcedure, mar,/*intPtr,*/ theadId);
                    if (hMouseHook == 0)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        Stop(true, false, false);
                        throw new Win32Exception(errorCode);
                    }
                }

                if (hKeyboardHook == 0 && InstallKeyboardHook)
                {
                    KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                    hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, /*Marshal.GetHINSTANCE( Assembly.GetExecutingAssembly().GetModules()[0]),*/ mar, theadId);
                    if (hKeyboardHook == 0)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        Stop(false, true, false);
                        throw new Win32Exception(errorCode);
                    }
                }
            }
        }
        
        public void Stop(bool UninstallMouseHook=true, bool UninstallKeyboardHook = true, bool ThrowExceptions = true)
        {
            lock (objlock)
            {
                if (hMouseHook != 0 && UninstallMouseHook)
                {
                    int retMouse = UnhookWindowsHookEx(hMouseHook);
                    hMouseHook = 0;
                    if (retMouse == 0 && ThrowExceptions)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Win32Exception(errorCode);
                    }
                }

                if (hKeyboardHook != 0 && UninstallKeyboardHook)
                {
                    int retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                    hKeyboardHook = 0;
                    if (retKeyboard == 0 && ThrowExceptions)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Win32Exception(errorCode);
                    }
                }
            }
        }
        
        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if ((nCode >= 0) && (OnMouseActivity != null))
            {
                MouseLLHookStruct mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));

                MouseButtons button = MouseButtons.None;
                short mouseDelta = 0;
                int clickCount = 0;
                switch (wParam)
                {
                    case WM_LBUTTONDOWN://513出现了
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONDOWN://516出现了
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONDBLCLK://515  doubleclick没有出现过
                        button = MouseButtons.XButton1;
                        clickCount = 2;
                        break;
                    case WM_RBUTTONDBLCLK://518
                        button = MouseButtons.XButton1;
                        clickCount = 2;
                        break;
                    case WM_MOUSEMOVE://512 出现了
                        button = MouseButtons.XButton2;
                        clickCount = 0;
                        break;
                    case WM_MOUSEWHEEL://522 没试
                        mouseDelta = (short)((mouseHookStruct.mouseData >> 16) & 0xffff);
                        clickCount = 0;
                        break;
                }

                MouseEventArgs e = new(button, clickCount, mouseHookStruct.pt.x, mouseHookStruct.pt.y, mouseDelta);
                OnMouseActivity(this, e);//转给委托函数
            }
            return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        }
        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            bool handled = false;
            if ((nCode >= 0) && (KeyDown != null || KeyUp != null || KeyPress != null))
            {
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                if (KeyDown != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new(keyData);
                    KeyDown(this, e);               //转给委托函数
                    handled = handled || e.Handled;
                }

                if (KeyPress != null && wParam == WM_KEYDOWN)
                {
                    bool isDownShift = (GetKeyState(VK_SHIFT) & 0x80) == 0x80;
                    bool isDownCapslock = GetKeyState(VK_CAPITAL) != 0;

                    byte[] keyState = new byte[256];
                    GetKeyboardState(keyState);
                    byte[] inBuffer = new byte[2];
                    if (ToAscii(MyKeyboardHookStruct.vkCode, MyKeyboardHookStruct.scanCode, keyState, inBuffer, MyKeyboardHookStruct.flags) == 1)
                    {
                        char key = (char)inBuffer[0];
                        if ((isDownCapslock ^ isDownShift) && char.IsLetter(key)) key = char.ToUpper(key);
                        KeyPressEventArgs e = new(key);
                        KeyPress(this, e);
                        handled = handled || e.Handled;
                    }
                }

                if (KeyUp != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new(keyData);
                    KeyUp(this, e);
                    handled = handled || e.Handled;
                }

            }

            if (handled)
                return 1;
            else
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }
    }
}
