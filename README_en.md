# GameAssistant

[中文说明](./README.md)

This program is only for learning purposes!

## Function

- Currently supports the 'Heroes v. Hordes' game's auto-play feature, allowing for automated monster-killing operations on the current level.
- It will judge the situation of no physical strength, and when there is no physical strength, it will wait for 10 minutes and then continue to try.
- You can run the program on the start page or when the game is closed, and the program will take over the subsequent operations.

## Description and Limitations

This tool simulates user mouse operations on the computer and can be used to automate certain games. It has the following limitations or requirements:

- When the program is running, it will control the movement and click operation of the computer mouse, so other operations cannot be performed at the same time.
- The computer screen needs to be kept open during operation, as closing the screen may cause the program to not function properly.
- Support running on Win10/Win11

## How to run

1. [Download](https://dotnet.microsoft.com/en-us/download) and install the `.NET SDK8.0`
2. [Download](https://mumu.163.com/) and install MuMu Simulator 12
3. Install the game `Heroes v. Hordes` in the emulator
4. Open the game `Heroes v. Hordes` and select the hero and the repeated level, staying on the page to be started.
5. Pull the project source code and execute `dotnet run` in the root directory.

> [!TIP]
> It is recommended to run the program for hang-up operation without using a computer, such as before going to bed. It is also recommended to adjust the brightness of the monitor to the lowest level.

## How to end

When the program is running, it will simulate system mouse operations, which will affect your normal operation. To end the program, you need to quickly switch to the command line tool and quickly press `Ctrl+C` to end the program.

It is recommended to perform this operation after the level is completed.

## Problem

- Currently only tested on 1.44.3 version
- When the program is running, log files will be generated in the `./logs` directory to facilitate viewing the running status.
- The program will stop after 6 hours by default, which can be set in 'Program.cs'.

> [!TIP]
> Students who understand development can make it compatible with other emulators or support other games on this basis.

## Contact

If you want to learn how to better develop and use this tool, or want to sponsor me, please contact me at [zpty@outlook.com].
