using System.Drawing.Imaging;

namespace GameAssistant.GameEngine;

internal class HeroesHordesEngine : GameEngineBase
{
    public const string GameName = "HeroesHordes";

    public NormalizedPoint NoActionPoint { get; set; } = new(0.0202f, 0.1139f);

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

    /// <summary>
    /// 波次完成
    /// </summary>
    public NormalizedPoint WavePoint { get; set; } = new(5f, 0.7062f);

    /// <summary>
    /// play 按钮位置
    /// </summary>
    public NormalizedRect PlayRect { get; set; } = new(0.3543f, 0.7631f, 0.5061f, 0.7973f);

    public NormalizedRect EndRect { get; set; } = new(0.1498f, 0.8952f, 0.2389f, 0.9431f);

    public NormalizedRect WaveRect { get; set; } = new(0.4615f, 0.9089f, 0.5364f, 0.951f);

    public NormalizedRect RewardRect { get; set; } = new(0.3138f, 0.8998f, 0.6883f, 0.9624f);


    public byte[]? AnalyzeImage { get; set; }
    public byte[]? TimerImage { get; set; }
    public byte[]? RewardImage { get; set; }

    public HeroesHordesEngine(string windowName, int actionBarHeight = 36) : base(windowName, actionBarHeight)
    {
        SetScreenResolution(900, 1600);
    }


    public async Task RunAsync(int seconds = 0)
    {
        seconds = seconds == 0 ? 30 * 24 * 60 * 60 : seconds;
        var endTime = DateTime.Now.AddSeconds(seconds);

        await PreStartAsync();

        var consumer = Task.Run(async () =>
        {
            while (await ActionChannel.Reader.WaitToReadAsync())
            {
                if (ActionChannel.Reader.TryRead(out var message))
                {
                    var point = ToPoint(message.Point);
                    Helper.Click(point.X, point.Y);
                    await Task.Delay(message.Delay);
                }
            }
        });

        while (DateTime.Now < endTime)
        {
            if (CanRun())
            {
                var point = ToPoint(PlayPoint);
                Helper.Click(point.X, point.Y);
                await Task.Delay(1000);
                await LoopAsync();
            }
            else
            {
                await Task.Delay(20 * 1000);
            }
        }

        ActionChannel.Writer.Complete();
        await consumer;
    }

    public async Task PreStartAsync()
    {
        AnalyzeImage = await File.ReadAllBytesAsync(@$"./{GameName}/analyze.jpg");
        TimerImage = await File.ReadAllBytesAsync(@$"./{GameName}/timer.jpg");
    }

    public async Task LoopAsync()
    {
        while (!IsEnd())
        {
            if (IsWaveEnd())
            {
                await ActionChannel.Writer.WriteAsync(new ClickAction(WavePoint, 200));
                await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 200));
                await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 200));
                await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 200));
                await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 200));
                await Task.Delay(1000);
            }
            else if (IsReward())
            {
                await ActionChannel.Writer.WriteAsync(new ClickAction(RewardPoint, 200));
                await ActionChannel.Writer.WriteAsync(new ClickAction(RewardPoint, 200));

            }
            else
            {
                await ActionChannel.Writer.WriteAsync(new ClickAction(MainChosePoint, 200));
                await ActionChannel.Writer.WriteAsync(new ClickAction(MainChosePoint, 200));
                await ActionChannel.Writer.WriteAsync(new ClickAction(MainChosePoint, 200));
                await ActionChannel.Writer.WriteAsync(new ClickAction(SecondChosePoint, 200));
            }
            await Task.Delay(1000);
        }

        await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 200));
        await ActionChannel.Writer.WriteAsync(new ClickAction(NoActionPoint, 200));
    }

    public Task EndActions()
    {
        throw new NotImplementedException();
    }

    private bool IsReward()
    {
        if (RewardImage == null)
        {
            throw new ArgumentNullException(nameof(RewardImage));
        }

        var point = ToPoint(RewardRect.Start);
        var size = ToPoint(RewardRect.Size);
        var bitmap = Helper.CaptureScreen(point.X, point.Y, size.X, size.Y);
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Jpeg);
        var imageBytes = stream.ToArray();

        var similarity = Helper.GetSimilar(RewardImage, imageBytes);
        return similarity > 0.5;
    }


    /// <summary>
    /// 是否波次结束
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private bool IsWaveEnd()
    {
        if (AnalyzeImage == null)
        {
            throw new ArgumentNullException(nameof(AnalyzeImage));
        }

        var point = ToPoint(WaveRect.Start);
        var size = ToPoint(WaveRect.Size);
        var bitmap = Helper.CaptureScreen(point.X, point.Y, size.X, size.Y);
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Jpeg);
        var imageBytes = stream.ToArray();

        var similarity = Helper.GetSimilar(AnalyzeImage, imageBytes);
        return similarity > 0.5;
    }


    /// <summary>
    /// 是否结束
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private bool IsEnd()
    {
        if (AnalyzeImage == null)
        {
            throw new ArgumentNullException(nameof(AnalyzeImage));
        }

        var point = ToPoint(EndRect.Start);
        var size = ToPoint(EndRect.Size);
        var bitmap = Helper.CaptureScreen(point.X, point.Y, size.X, size.Y);
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Jpeg);
        var imageBytes = stream.ToArray();

        var similarity = Helper.GetSimilar(AnalyzeImage, imageBytes);
        return similarity > 0.5;
    }

    private bool CanRun()
    {
        var point = ToPoint(PlayRect.Start);
        var size = ToPoint(PlayRect.Size);
        var bitmap = Helper.CaptureScreen(point.X, point.Y, size.X, size.Y);
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Jpeg);
        var imageBytes = stream.ToArray();
        var text = Helper.GetTextFromOCR(imageBytes);
        return text.Contains("Play");
    }
}
