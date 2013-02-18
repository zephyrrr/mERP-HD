using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ServiceMonitor
{
    /// <summary>
    /// 控制台帮助类
    /// </summary>
    public class ConsoleHelper
    {
        /// <summary>
        /// 启动控制台
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        /// <summary>
        /// 释放控制台
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        /// <summary>
        /// 把当前应用程序的控制台连接到它的父进程上
        /// 但是当有输出重定向需求时,无法实现:例如: a.exe => a.text
        /// </summary>
        /// <param name="dwProcessId"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(int dwProcessId);
        public const int ATTACH_PARENT_PROCESS = -1;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);
        public enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        /// <summary>
        /// 找出运行的窗口
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 移动窗口
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="bRepaint"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, int bRepaint);

        /// <summary>
        /// 改变Parent，hWndNewParent = 0为单独窗体
        /// </summary>
        /// <param name="hWndChild"></param>
        /// <param name="hWndNewParent"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="wMsg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 打开外部程序或文件
        /// </summary>
        /// <param name="hwnd">指定父窗口句柄</param>
        /// <param name="operation">指定动作, 如: open、print</param>
        /// <param name="fileName">指定要打开的文件或程序</param>
        /// <param name="parameters">给要打开的程序指定参数; 如果打开的是文件这里应该是null</param>
        /// <param name="directory">缺省目录</param>
        /// <param name="showCmd">打开选项</param>
        /// <returns>执行成功会返回应用程序句柄; 如果这个值小于等于32, 表示执行错误
        /// 0 = 内存不足；2 = 文件名错误；3 = 路径名错误；11 = EXE文件无效；26 = 发生共享错误；27 = 文件名不完全或无效；
        /// 28 = 超时；29 = DDE事务失败；30 = 正在处理其他DDE事务而不能完成该DDE事务；31 = 没有相关联的应用程序
        /// </returns>
        [DllImport("shell32.dll")]
        public static extern int ShellExecute(IntPtr hwnd, string operation, string fileName, string parameters, string directory, int showCmd);
        /// <summary>
        /// 隐藏
        /// </summary>
        public const int SE_HIDE = 0;
        /// <summary>
        /// 用最近的大小和位置显示, 激活
        /// </summary>
        public const int SE_SHOWNORMAL = 1;
        /// <summary>
        /// 同 SE_SHOWNORMAL
        /// </summary>
        public const int SE_NORMAL = 1;
        /// <summary>
        /// 最小化, 激活
        /// </summary>
        public const int SE_SHOWMINIMIZED = 2;
        /// <summary>
        /// 最大化, 激活
        /// </summary>
        public const int SE_SHOWMAXIMIZED = 3;
        /// <summary>
        /// 同 SE_SHOWMAXIMIZED
        /// </summary>
        public const int SE_MAXIMIZE = 3;
        /// <summary>
        /// 用最近的大小和位置显示, 不激活
        /// </summary>
        public const int SE_SHOWNOACTIVATE = 4;
        /// <summary>
        /// 同 SE_SHOWNORMAL
        /// </summary>
        public const int SE_SHOW = 5;
        /// <summary>
        /// 最小化, 不激活
        /// </summary>
        public const int SE_MINIMIZE = 6;
        /// <summary>
        /// 同 SE_MINIMIZE
        /// </summary>
        public const int SE_SHOWMINNOACTIVE = 7;
        /// <summary>
        /// 同 SE_SHOWNOACTIVATE
        /// </summary>
        public const int SE_SHOWNA = 8;
        /// <summary>
        /// 同 SE_SHOWNORMAL
        /// </summary>
        public const int SE_RESTORE = 9;
        /// <summary>
        /// 同 SE_SHOWNORMAL
        /// </summary>
        public const int SE_SHOWDEFAULT = 10;
        /// <summary>
        /// 同 SE_SHOWNORMAL
        /// </summary>
        public const int SE_MAX = 10;

        /// <summary>
        /// 取出窗口运行的菜单
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="bRevert"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        public extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

        /// <summary>
        /// 灰掉按钮
        /// </summary>
        /// <param name="hMenu"></param>
        /// <param name="uPosition"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        public extern static IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        /// <summary>
        /// 改变标题
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll")]
        public static extern bool SetConsoleTitle(string strMessage);

        /// <summary>
        /// 窗口产生动画效果方法。
        /// </summary>
        /// <param name="whnd">产生动画的窗口的句柄</param>
        /// <param name="dwtime">动画持续的时间（以微秒计），完成一个动画的标准时间为200微秒。</param>
        /// <param name="dwflag">动画类型</param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern bool AnimateWindow(IntPtr whnd, int dwtime, int dwflag);
        /// <summary>
        /// 从左到右
        /// </summary>
        public const Int32 AW_HOR_POSITIVE = 0x00000001;
        /// <summary>
        /// 从右到左
        /// </summary>
        public const Int32 AW_HOR_NEGATIVE = 0x00000002;
        /// <summary>
        /// 从上到下
        /// </summary>
        public const Int32 AW_VER_POSITIVE = 0x00000004;
        /// <summary>
        /// 从下到上
        /// </summary>
        public const Int32 AW_VER_NEGATIVE = 0x00000008;
        /// <summary>
        /// 从中间到四周
        /// </summary>
        public const Int32 AW_CENTER = 0x00000010;
        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public const Int32 AW_HIDE = 0x00010000;
        /// <summary>
        /// 淡入淡出效果
        /// </summary>
        public const Int32 AW_BLEND = 0x00080000;
        /// <summary>
        /// 示窗口
        /// </summary>
        public const Int32 AW_ACTIVATE = 0x00020000;

        public static void ProcessConsole()
        {
            //Process cmd = new Process();
            //cmd.StartInfo.FileName = "cmd.exe";
            //cmd.StartInfo.UseShellExecute = false; //此处必须为false否则引发异常
            //cmd.StartInfo.RedirectStandardInput = true; //标准输入
            //cmd.StartInfo.RedirectStandardOutput = false; //标准输出
            //cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            //cmd.Start(); //启动进程

            //Process.Start("cmd.exe");
        }

        /// <summary>
        /// AllocConsole的关闭按钮不可用
        /// </summary>
        /// <param name="parentIntPtr"></param>
        public static void AllocConsoleNoCloseBox()
        {
            AllocConsole();
            IntPtr windowHandle = FindWindow(null, Process.GetCurrentProcess().MainModule.FileName);
            RemoveCloseBox(windowHandle);
        }

        /// <summary>
        /// 在Parent中显示AllocConsole
        /// </summary>
        /// <param name="parentIntPtr"></param>
        public static void AllocConsoleToParent(IntPtr parentIntPtr)
        {
            AllocConsole();
            IntPtr windowHandle = FindWindow(null, Process.GetCurrentProcess().MainModule.FileName);
            RemoveCloseBox(windowHandle);
            SetParent(windowHandle, parentIntPtr);
            MoveWindow(windowHandle, 0, 0, 250, 250, 1);
        }

        /// <summary>
        /// 关闭按钮不可用
        /// </summary>
        /// <param name="windowHandle"></param>
        public static void RemoveCloseBox(IntPtr windowHandle)
        {
            uint SC_CLOSE = 0xF060;
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }

        #region 输出不同颜色的内容
        public static void WriteLineYellow(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void WriteLineGreen(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void WriteLineRed(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        #endregion
    }
}
