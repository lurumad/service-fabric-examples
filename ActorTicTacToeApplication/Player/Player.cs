using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Player.Interfaces;

namespace Player
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class Player : Actor, IPlayer
    {
        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            return Task.FromResult(true);
        }

        public Task<bool> JoinGameAsync(ActorId gameId, string playerName)
        {
            var game = ActorProxy.Create<IGame>(gameId, "fabric:/ActorTicTacToeApplication");

            return game.JoinGameAsync(Id.GetLongId(), playerName);
        }

        public Task<bool> MakeMoveAsync(ActorId gameId, int x, int y)
        {
            var game = ActorProxy.Create<IGame>(gameId, "fabric:/ActorTicTacToeApplication");

            return game.MakeMoveAsync(Id.GetLongId(), x, y);
        }
    }
}
