﻿using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace Player.Interfaces
{
    public interface IPlayer : IActor
    {
        Task<bool> JoinGameAsync(ActorId gameId, string playerName);
        Task<bool> MakeMoveAsync(ActorId gameId, int x, int y);
    }
}
