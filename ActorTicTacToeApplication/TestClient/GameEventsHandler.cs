using System;
using Game.Interfaces;

namespace TestClient
{
    class GameEventsHandler : IGameEvents
    {
        public void NewChallengeHasArrived(string playerName)
        {
            Console.WriteLine($"A new challenger has arrived {playerName}");
        }
    }
}
