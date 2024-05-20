// See https://aka.ms/new-console-template for more information
using GameAssistant.GameEngine;

var engine = new HeroesHordesEngine("MuMu模拟器12");

var seconds = (int)TimeSpan.FromHours(4).TotalSeconds;
await engine.RunAsync(seconds);
