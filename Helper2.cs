using System.Drawing;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace GameAssistant;

internal class Helper2
{
    public static void SetCursorAndClick(int x, int y)
    {
        PInvoke.SetCursorPos(x, y);
        PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, (nuint)IntPtr.Zero);
        Thread.Sleep(50); // 添加一个小延迟
        PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, (nuint)IntPtr.Zero);
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
