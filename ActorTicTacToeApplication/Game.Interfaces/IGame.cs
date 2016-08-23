﻿using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace Game.Interfaces
{
    public interface IGame : IActor
    {
        Task<bool> JoinGameAsync(long playerId, string playerName);
        Task<int[]> GetGameBoardAsync();
        Task<string> GetWinnerAsync();
        Task<bool> MakeMoveAsync(long playerId, int x, int y);
    }
}
