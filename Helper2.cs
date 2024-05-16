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
        Thread.Sleep(50); // ���һ��С�ӳ�
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
        // ʹ�ú��ʵ� OCR ��� API ����ͼ������ʶ��
        // ����ʹ���� System.Drawing.Common �� OCR ����Ϊʾ��
        // ����ʹ�� Tesseract OCR ��� Windows �Դ��� OCR API
        // ����Ϊα����
        // string ocrText = OCRLibrary.PerformOCR(bmp);
        string ocrText = "ʾ���ı�"; // ��Ϊʾ�����滻Ϊʵ�� OCR ����
        return ocrText;
    }
}
