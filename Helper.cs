using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace GameAssistant;
internal class Helper
{
    public static void EnumWindows()
    {
        Process[] allProcesses = Process.GetProcesses();

        // 遍历所有进程
        foreach (Process process in allProcesses)
        {
            // 现在你可以访问 process 的属性，例如进程 ID、主窗口句柄、主窗口标题等。
            int processId = process.Id;
            IntPtr mainWindowHandle = process.MainWindowHandle;
            string mainWindowTitle = process.MainWindowTitle;
            // 在这里你可以将这些信息显示在列表中或者进行其他操作。

            if (!string.IsNullOrEmpty(mainWindowTitle))
                Console.WriteLine(mainWindowTitle + ":" + mainWindowHandle);
        }
    }

    public static HWND GetWindow(string name)
    {
        // 获取目标窗口句柄
        HWND hWnd = PInvoke.FindWindow(null, name);
        if (hWnd == IntPtr.Zero)
        {
            Console.WriteLine("未找到窗口");
        }

        // 获取窗口大小
        PInvoke.GetClientRect(hWnd, out RECT rect);
        Console.WriteLine($"窗口大小: 宽度={rect.right - rect.left}, 高度={rect.bottom - rect.top}");
        return hWnd;
    }


    public static void SetCursorAndClick(HWND targetWindowHandle, int x, int y)
    {
        PInvoke.SetCapture(targetWindowHandle);
        PInvoke.SetForegroundWindow(targetWindowHandle);
        PInvoke.SetFocus(targetWindowHandle);
        PInvoke.EnableWindow(targetWindowHandle, true);

        Thread.Sleep(500);
        var point = new Point(x, y);
        //PInvoke.ScreenToClient(targetWindowHandle, ref point);
        PInvoke.SetCursorPos(point.X, point.Y);

        Span<INPUT> inputs =
        [
            CreateMouseInput(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN, 0, 0),
            CreateMouseInput(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP, 0, 0),
        ];
        var res = PInvoke.SendInput(inputs, Marshal.SizeOf(typeof(INPUT)));

        var message = Marshal.GetLastPInvokeErrorMessage();

        Console.WriteLine("The last Win32 Error was: " + message);
        Console.WriteLine(res);
    }


    private static INPUT CreateMouseInput(MOUSE_EVENT_FLAGS eVENT_FLAGS, int x, int y)
    {
        var mouseInput = new MOUSEINPUT
        {
            dx = x,
            dy = y,
            dwFlags = eVENT_FLAGS,
            mouseData = 0,
        };

        return new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
            Anonymous = new INPUT._Anonymous_e__Union
            {
                mi = mouseInput
            }
        };
    }
    private static INPUT CreateKeyBoardInput(VIRTUAL_KEY key, KEYBD_EVENT_FLAGS? eVENT_FLAGS = null)
    {
        var keyBoardInput = new KEYBDINPUT
        {
            wVk = key,
        };
        if (eVENT_FLAGS != null)
        {
            keyBoardInput.dwFlags = eVENT_FLAGS.Value;
        }

        return new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
            Anonymous = new INPUT._Anonymous_e__Union
            {
                ki = keyBoardInput
            }
        };
    }

    public static Bitmap CaptureScreen(int x, int y, int width, int height)
    {
        Bitmap bmp = new Bitmap(width, height);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            HDC hdcDest = (HDC)g.GetHdc();
            HWND hwnd = new HWND();
            HDC hdcSrc = PInvoke.GetDC(hwnd);
            PInvoke.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, x, y, ROP_CODE.SRCCOPY);
            PInvoke.ReleaseDC(hwnd, hdcSrc);
            g.ReleaseHdc(hdcDest);
        }
        return bmp;
    }

    public static Color GetPixelColor(int x, int y)
    {
        HWND hwnd = new HWND();
        HDC hdc = PInvoke.GetDC(hwnd);
        uint pixel = PInvoke.GetPixel(hdc, x, y);
        PInvoke.ReleaseDC(hwnd, hdc);

        int r = (int)(pixel & 0x000000FF);
        int g = (int)(pixel & 0x0000FF00) >> 8;
        int b = (int)(pixel & 0x00FF0000) >> 16;
        return Color.FromArgb(r, g, b);
    }

    public static string PerformOCR(Bitmap bmp)
    {
        // 使用合适的 OCR 库或 API 进行图像文字识别
        // 这里使用了 System.Drawing.Common 和 OCR 库作为示例
        // 可以使用 Tesseract OCR 库或 Windows 自带的 OCR API
        // 以下为伪代码
        // string ocrText = OCRLibrary.PerformOCR(bmp);
        string ocrText = "示例文本"; // 仅为示例，替换为实际 OCR 代码
        return ocrText;
    }

}
