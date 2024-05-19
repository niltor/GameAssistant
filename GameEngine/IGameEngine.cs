namespace GameAssistant.GameEngine;
internal interface IGameEngine
{
    Task RunAsync(int seconds);
    Task PreStartAsync();
    Task LoopActionAsync();
    Task EndActionsAsync();
    bool IsEnd();

}
