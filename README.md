# GameAssistant

[English](./README_en.md)

本程序只可用作学习用途！

## 功能

- 目前支持`Heroes v. Hordes`游戏的挂机操作，支持对当前关卡进行自动打怪操作。
- 会判断没有体力的情况，没有体力时，会等待10分钟，然后继续尝试。
- 可在开始页面，或游戏关上运行的状态运行程序，程序会接管后续的操作。

## 说明与限制

该工具模拟用户对电脑的鼠标操作，可用于对某些游戏进行自动化操作，其有以下限制或要求：

- 程序运行时，会控制电脑鼠标的移动和点击操作，所以不能同时进行其他操作。
- 运行时需要保持电脑屏幕打开状态，屏幕关闭状态可能会导致程序无法正常运行。
- 支持在Win10/Win11 上运行

## 如何运行

1. [下载](https://dotnet.microsoft.com/en-us/download)并安装`.NET SDK8.0`.
2. [下载](https://mumu.163.com/)并安装MuMu模拟器12.
3. 在模拟器中安装游戏`Heroes v. Hordes`.
4. 打开游戏`Heroes v. Hordes`，并选择英雄和重复进行的关卡,停留在待开始的页面。
5. 拉取项目源代码，然后在根目录中执行`dotnet run`。

> [!TIP]
> 建议在不使用电脑情况下，如睡觉前，运行程序进行挂机操作。同时建议将显示器的亮度调到最低。

## 如何结束

由于程序运行时，会模拟系统鼠标操作，此时会影响到你正常的操作。想要结束程序，需要快速切换到命令行工具，然后快速按下`Ctrl+C`，以结束程序。

建议在关卡完成后进行此操作。

## 问题

- 当前仅测试过在1.44.3·版本执行
- 程序运行时，会在`./logs`目录下生成日志文件，以方便查看运行状态。
- 程序默认会在6小时后停止，可在`Program.cs`中进行设置。

> [!TIP]
> 懂开发的同学可以在此基础上兼容其他的模拟器，或支持其他的游戏。

## 联系

如果你想学习如何更好的开发和使用该工具，或者想赞助我，可[联系](mailto:zpty@outlook.com)我。
