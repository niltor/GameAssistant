namespace GameAssistant.GameEngine;
internal interface IGameEngine
{

    Task RunAsync(int seconds);
    Task PreStartAsync();
    Task Loop();
    Task EndActions();
    bool IsEnd();


}
