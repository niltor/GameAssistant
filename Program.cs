// See https://aka.ms/new-console-template for more information
using GameAssistant;

var allProcesses = ActionHelper.EnumWindows();
foreach (Process process in allProcesses)
{
    // 输出 process 信息,如果有多个窗口，全部输出 
    Console.WriteLine($"[{process.Id}]:{process.ProcessName},({process.MainWindowHandle}), => {process.MainWindowTitle}");
}



await Test();
//var helper = new ActionHelper("MuMu模拟器12");
//var helper = new ActionHelper("dict.component.html - ClientApp - Visual Studio Code");

//helper.EnableWindow();
//await Task.Delay(200);

//helper.PressKey(VIRTUAL_KEY.VK_A);
//helper.PressKey(VIRTUAL_KEY.VK_A);

//helper.Click(500, 500);



static async Task Test()
{
    var helper = new ActionHelper("MuMu模拟器12");
    helper.EnableWindow();
    await Task.Delay(200);
    var ocrPath = @"./test.jpg";
    // read image as bytes 
    var image = File.ReadAllBytes(ocrPath);
    helper.GetTextFromOCR(image);


}



