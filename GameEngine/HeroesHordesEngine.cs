using System.Drawing.Imaging;

namespace GameAssistant.GameEngine;

internal class HeroesHordesEngine : GameEngineBase
{
    public NormalizedPoint NoActionPoint { get; set; } = new(0.0202f, 0.113f);

    /// <summary>
    /// 主要点击
    /// </summary>
    public NormalizedPoint MainChosePoint { get; set; } = new(0.7996f, 0.5524f);

    /// <summary>
    /// 次要点击
    /// </summary>
    public NormalizedPoint SecondChosePoint { get; set; } = new(0.7895f, 0.41f);

    /// <summary>
    /// play按钮位置
    /// </summary>
    public NormalizedPoint PlayPoint { get; set; } = new(0.4453f, 0.7973f);

    /// <summary>
    /// 奖励点击坐标
    /// </summary>
    public NormalizedPoint RewardPoint { get; set; } = new(0.5061f, 0.9282f);

    public NormalizedPoint OpenBoxPoint { get; set; } = new(0.5061f, 0.7745f);

    /// <summary>
    /// 波次完成
    /// </summary>
    public NormalizedPoint WavePoint { get; set; } = new(0.5f, 0.7062f);

    /// <summary>
    /// play 按钮位置
    /// </summary>
    public NormalizedRect PlayRect { get; set; } = new(0.3543f, 0.7631f, 0.5061f, 0.7973f);

    public NormalizedRect EndRect { get; set; } = new(0.1417f, 0.0513f, 0.8603f, 0.1139f);

    public NormalizedRect WaveRect { get; set; } = new(0.2652f, 0.672f, 0.7368f, 0.7312f);

    public NormalizedRect RewardRect { get; set; } = new(0.3138f, 0.8998f, 0.6883f, 0.9624f);

    public NormalizedRect NoEnergyRect { get; set; } = new(0.332f, 0.2802f, 0.6802f, 0.3235f);

    public NormalizedRect DialogCloseRect { get; set; } = new(0.8158f, 0.1481f, 0.8927f, 0.1913f);

    public byte[]? EndImage { get; set; }
    public byte[]? WaveImage { get; set; }
    public byte[]? TimerImage { get; set; }
    public byte[]? RewardImage { get; set; }
    public byte[]? NoEnergyImage { get; set; }
    public byte[]? CloseImage { get; set; }

    private static bool IsPausing = false;
    private static bool IsRunning = false;

    public HeroesHordesEngine(string windowName, int actionBarHeight = 36) : base(windowName, actionBarHeight)
    {
        GameName = "HeroesHordes";
        SetScreenResolution(900, 1600);
    }

    public async Task RunAsync(int seconds = 0)
    {
        Log("=== 🚀 Start Run ===");
        seconds = seconds == 0 ? 30 * 24 * 60 * 60 : seconds;
        var endTime = DateTime.Now.AddSeconds(seconds);
        await PreStartAsync();
        var consumer = Task.Run(async () =>
        {
            while (await ActionChannel.Reader.WaitToReadAsync())
            {
                if (ActionChannel.Reader.TryRead(out var action))
                {
                    var point = ToPoint(action.Point);
                    Helper.Click(point.X, point.Y);
                    await Task.Delay(action.Delay);
                }
            }
        });

        int i = 1;
        while (DateTime.Now < endTime)
        {
            Log($"第[{i}]次运行...");
            i++;
            await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 200));

            if (!await StartAsync())
            {
                continue;
            }

            SetFlag(ref IsRunning, true);
            SetFlag(ref IsPausing, false);
            await EventHandlerAsync();
            SetFlag(ref IsRunning, false);
            SetFlag(ref IsPausing, true);

            await Task.Delay(10 * 1000);
        }

        ActionChannel.Writer.Complete();
        await consumer;
    }

    /// <summary>
    /// 1 加载资源
    /// </summary>
    /// <returns></returns>
    public async Task PreStartAsync()
    {
        Log("Loading assets...");
        EndImage = await File.ReadAllBytesAsync(@$"./assets/{GameName}/end.jpg");
        TimerImage = await File.ReadAllBytesAsync(@$"./assets/{GameName}/timer.jpg");
        RewardImage = await File.ReadAllBytesAsync(@$"./assets/{GameName}/reward.jpg");
        WaveImage = await File.ReadAllBytesAsync(@$"./assets/{GameName}/wave.jpg");
        NoEnergyImage = await File.ReadAllBytesAsync(@$"./assets/{GameName}/noEnergy.jpg");
        CloseImage = await File.ReadAllBytesAsync(@$"./assets/{GameName}/close.jpg");
    }

    /// <summary>
    /// 2 开始游戏
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<bool> StartAsync()
    {
        if (WaitToPlay())
        {
            await ActionChannel.Writer.WriteAsync(new ClickAction(PlayPoint, 200));

            if (IsMatchImage(DialogCloseRect, CloseImage))
            {
                Console.WriteLine("😓 Has Dialog!");
                var point = DialogCloseRect.Start;
                point.X += 0.015f;
                point.Y += 0.01f;
                await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 200));
                await ActionChannel.Writer.WriteAsync(new ClickAction(point, 200));
            }

            if (IsMatchImage(NoEnergyRect, NoEnergyImage))
            {
                await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 200));
                Console.WriteLine("❌ 没有体力,等待10分钟");
                await Task.Delay(10 * 60 * 1000);
                return false;
            }

            Log("🚀 New Round!");
            await Task.Delay(3500);
        }
        return true;
    }

    /// <summary>
    /// 3 运行状态处理
    /// </summary>
    /// <returns></returns>
    public async Task EventHandlerAsync()
    {
        while (!IsEnd())
        {
            var waitSeconds = 3600;
            if (IsWaveEnd())
            {
                ClearQueue();
                Log("Wave End");
                await ActionChannel.Writer.WriteAsync(new ClickAction(WavePoint, 200));
                await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 200));
                await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 200));
                await Task.Delay(2000);

            }
            else if (IsReward())
            {
                ClearQueue();
                Log("Reward");
                await ActionChannel.Writer.WriteAsync(new ClickAction(RewardPoint, 500));
                await ActionChannel.Writer.WriteAsync(new ClickAction(RewardPoint, 500));
                await Task.Delay(3000);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    await LoopActionAsync();
                }
            }
            await Task.Delay(waitSeconds);
        }
        Log("✅ End Status...");
        await EndActionsAsync();
    }

    /// <summary>
    /// 循环操作
    /// </summary>
    /// <returns></returns>
    public async Task LoopActionAsync()
    {
        await ActionChannel.Writer.WriteAsync(new ClickAction(MainChosePoint, 200));
        await ActionChannel.Writer.WriteAsync(new ClickAction(SecondChosePoint, 200));
        await ActionChannel.Writer.WriteAsync(new ClickAction(OpenBoxPoint, 200));
    }

    /// <summary>
    /// 结束操作
    /// </summary>
    /// <returns></returns>
    public async Task EndActionsAsync()
    {
        ClearQueue();
        Log("End");
        await ActionChannel.Writer.WriteAsync(new ClickAction(OpenBoxPoint, 200));
        await Task.Delay(2500);
        await ActionChannel.Writer.WriteAsync(new ClickAction(RewardPoint, 200));
        await Task.Delay(500);
        await ActionChannel.Writer.WriteAsync(new ClickAction(RewardPoint, 200));
        await Task.Delay(6000);
        for (int i = 0; i < 4; i++)
        {
            await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 500));
        }
    }

    // 清空队列 
    private void ClearQueue()
    {
        _ = ActionChannel.Reader.ReadAllAsync();
    }

    private bool IsReward()
    {
        return IsMatchImage(RewardRect, RewardImage);
    }

    /// <summary>
    /// 是否波次结束
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private bool IsWaveEnd()
    {
        return IsMatchImage(WaveRect, WaveImage);
    }


    /// <summary>
    /// 是否结束
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private bool IsEnd()
    {
        return IsMatchImage(EndRect, EndImage);
    }


    private bool IsMatchImage(NormalizedRect rect, byte[]? compareBytes)
    {
        ArgumentNullException.ThrowIfNull(compareBytes);

        var point = ToPoint(rect.Start);
        var size = ToSize(rect.Size);
        var bitmap = Helper.CaptureScreen(point.X, point.Y, size.X, size.Y);
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Jpeg);
        var imageBytes = stream.ToArray();
        stream.Close();
        var similarity = ActionHelper.GetSimilar(imageBytes, compareBytes);
        return similarity > 0.5;
    }

    private bool WaitToPlay()
    {
        var point = ToPoint(PlayRect.Start);
        var size = ToSize(PlayRect.Size);
        var bitmap = Helper.CaptureScreen(point.X, point.Y, size.X, size.Y);
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Jpeg);
        var imageBytes = stream.ToArray();

        stream.Close();
        var text = Helper.GetTextFromOCR(imageBytes);

        return text.Contains("PLAY", StringComparison.InvariantCultureIgnoreCase);
    }

    private void SetFlag(ref bool flag, bool val)
    {
        Volatile.Write(ref flag, val);
    }
}
