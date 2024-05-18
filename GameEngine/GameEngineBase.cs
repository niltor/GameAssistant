﻿using System.Threading.Channels;

namespace GameAssistant.GameEngine;
internal class GameEngineBase(string windowName, int actionBarHight)
{
    public ActionHelper Helper { get; set; } = new ActionHelper(windowName);

    public Channel<ClickAction> ActionChannel { get; set; } = Channel.CreateBounded<ClickAction>(100);

    /// <summary>
    /// 设置分辨率
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetScreenResolution(int width, int height)
    {
        Helper.SetScreenResolution(width, height);
    }

    public Point ToPoint(NormalizedPoint point)
    {
        var x = (int)(point.X * Helper.Width);
        var y = (int)(point.Y * (Helper.Height - actionBarHight)) + actionBarHight;
        return new Point(x, y);
    }

    public Point ToSize(NormalizedPoint point)
    {
        var x = (int)(point.X * Helper.Width);
        var y = (int)(point.Y * (Helper.Height - actionBarHight));
        return new Point(x, y);
    }
}

struct NormalizedPoint
{
    public float X;
    public float Y;

    public NormalizedPoint(float x, float y)
    {
        if (x > 1 || y > 1)
        {
            throw new ArgumentException("x和y必须在0-1之间");
        }
        X = x;
        Y = y;
    }
}

struct NormalizedRect
{
    public NormalizedPoint Start;
    public NormalizedPoint End;

    public readonly NormalizedPoint Size => new(End.X - Start.X, End.Y - Start.Y);

    public NormalizedRect(NormalizedPoint start, NormalizedPoint end)
    {
        if (start.X > end.X || start.Y > end.Y)
        {
            throw new ArgumentException("start和end必须是左上角和右下角");
        }
        Start = start;
        End = end;
    }

    public NormalizedRect(float startX, float startY, float endX, float endY)
    {
        Start = new NormalizedPoint(startX, startY);
        End = new NormalizedPoint(endX, endY);
    }
}

/// <summary>
/// 点击事件
/// </summary>
struct ClickAction(NormalizedPoint point, int delay = 200)
{
    public NormalizedPoint Point = point;
    public int Delay = delay;
}