using OpenCvSharp;

using Tesseract;

namespace GameAssistant;
public class ActionHelper
{
    internal HWND TargetWindow { get; set; }
    /// <summary>
    /// 窗口宽
    /// </summary>
    public int Width { get; private set; }
    /// <summary>
    /// 窗口高
    /// </summary>
    public int Height { get; private set; }

    public int WidthPix { get; private set; } = 900;
    public int HeightPix { get; private set; } = 1600;

    public ActionHelper(string windowName)
    {
        TargetWindow = PInvoke.FindWindow(null, windowName);
        if (TargetWindow == IntPtr.Zero)
        {
            throw new ArgumentException($"未找到窗口{windowName}");
        }
        // 获取窗口大小
        PInvoke.GetClientRect(TargetWindow, out RECT rect);

        Width = rect.right - rect.left;
        Height = rect.bottom - rect.top;

        Console.WriteLine($"窗口大小: 宽度={rect.right - rect.left}, 高度={rect.bottom - rect.top}");
    }

    /// <summary>
    /// 设置分辨率
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetScreenResolution(int width, int height)
    {
        WidthPix = width;
        HeightPix = height;
    }

    public static Process[] EnumWindows()
    {
        return Process.GetProcesses()
            .Where(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle))
            .ToArray();
    }

    public void EnableWindow()
    {
        PInvoke.EnableWindow(TargetWindow, true);
        PInvoke.SetFocus(TargetWindow);
        PInvoke.SetCapture(TargetWindow);
        PInvoke.SetForegroundWindow(TargetWindow);
    }

    /// <summary>
    /// 按键
    /// </summary>
    /// <param name="vIRTUAL_KEY"></param>
    /// <param name="pressTime">按下到松开的时间</param>
    internal void PressKey(VIRTUAL_KEY vIRTUAL_KEY, int pressTime = 5)
    {
        Span<INPUT> inputs = [
            CreateKeyBoardInput(vIRTUAL_KEY),
        ];
        var res = PInvoke.SendInput(inputs, Marshal.SizeOf(typeof(INPUT)));

        Task.Delay(pressTime).Wait();

        inputs = [
            CreateKeyBoardInput(vIRTUAL_KEY,KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
        ];
        res = PInvoke.SendInput(inputs, Marshal.SizeOf(typeof(INPUT)));

        if (res == 0)
        {
            var message = Marshal.GetLastPInvokeErrorMessage();
            Console.WriteLine("PressKey error: " + message);
        }
    }

    internal void Click(int x, int y)
    {
        ClientToScreen(ref x, ref y);
        PInvoke.SetCursorPos(x, y);
        Span<INPUT> inputs = [
            CreateMouseInput(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN, x, y),
            CreateMouseInput(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP, x, y),
        ];
        var res = PInvoke.SendInput(inputs, Marshal.SizeOf(typeof(INPUT)));
        if (res == 0)
        {
            var message = Marshal.GetLastPInvokeErrorMessage();
            Console.WriteLine("Click error: " + message);
        }
    }

    /// <summary>
    /// 鼠标输出
    /// </summary>
    /// <param name="eVENT_FLAGS"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private INPUT CreateMouseInput(MOUSE_EVENT_FLAGS eVENT_FLAGS, int x, int y)
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
    /// <summary>
    /// 键盘输入
    /// </summary>
    /// <param name="key"></param>
    /// <param name="eVENT_FLAGS"></param>
    /// <returns></returns>
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
            type = INPUT_TYPE.INPUT_KEYBOARD,
            Anonymous = new INPUT._Anonymous_e__Union
            {
                ki = keyBoardInput
            }
        };
    }

    public Bitmap CaptureScreen(int x, int y, int width, int height)
    {
        Bitmap bmp = new Bitmap(width, height);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            HDC hdcDest = (HDC)g.GetHdc();
            HDC hdcSrc = PInvoke.GetDC(TargetWindow);

            PInvoke.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, x, y, ROP_CODE.SRCCOPY);
            PInvoke.ReleaseDC(TargetWindow, hdcSrc);
            g.ReleaseHdc(hdcDest);
        }
        return bmp;
    }

    public Color GetPixelColor(int x, int y)
    {
        HDC hdc = PInvoke.GetDC(TargetWindow);
        uint pixel = PInvoke.GetPixel(hdc, x, y);
        PInvoke.ReleaseDC(TargetWindow, hdc);

        int r = (int)(pixel & 0x000000FF);
        int g = (int)(pixel & 0x0000FF00) >> 8;
        int b = (int)(pixel & 0x00FF0000) >> 16;
        return Color.FromArgb(r, g, b);
    }

    /// <summary>
    /// ocr图片识别
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public string GetTextFromOCR(byte[] bytes)
    {
        Mat image = Cv2.ImDecode(bytes, ImreadModes.Color);
        Mat grayImage = new Mat();
        Cv2.CvtColor(image, grayImage, ColorConversionCodes.RGB2GRAY);
        Cv2.Threshold(grayImage, grayImage, 200, 255, ThresholdTypes.Binary);

        using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
        {
            using (var img = Pix.LoadFromMemory(grayImage.ToBytes(".jpg")))
            {
                using (var page = engine.Process(img))
                {
                    string text = page.GetText();
                    Console.WriteLine($"识别的文本：{text}");
                    return text;
                }
            }
        }
    }

    /// <summary>
    /// 客户端转屏幕坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void ClientToScreen(ref int x, ref int y)
    {
        System.Drawing.Point point = new() { X = x, Y = y };

        PInvoke.ClientToScreen(TargetWindow, ref point);
        x = point.X;
        y = point.Y;
    }

    /// <summary>
    /// 图片相似度比较
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="compare"></param>
    public double GetSimilar(byte[] origin, byte[] compare)
    {

        // 加载两张图片
        Mat img1 = Cv2.ImDecode(origin, ImreadModes.Grayscale);
        Mat img2 = Cv2.ImDecode(compare, ImreadModes.Grayscale);

        // 计算直方图
        int[] histSize = [256];
        Rangef[] ranges = [new Rangef(0, 256)];
        var hist1 = new Mat();
        var hist2 = new Mat();
        Cv2.CalcHist([img1], [0], null, hist1, 1, histSize, ranges);
        Cv2.CalcHist([img2], [0], null, hist2, 1, histSize, ranges);

        // 归一化直方图
        Cv2.Normalize(hist1, hist1, 0, 1, NormTypes.MinMax);
        Cv2.Normalize(hist2, hist2, 0, 1, NormTypes.MinMax);

        // 比较直方图
        double similarity = Cv2.CompareHist(hist1, hist2, HistCompMethods.Correl);
        return similarity;
    }
}
