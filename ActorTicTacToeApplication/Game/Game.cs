using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Game.Interfaces;

namespace Game
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
    internal class Game : Actor, IGame
    {
        private const string StateName = "State";

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            var state = await StateManager.TryGetStateAsync<GameState>(StateName);

            if (!state.HasValue)
            {
                var gameState = new GameState
                {
                    Board = new int[9],
                    NextPlayerIndex = 0,
                    NumberOfMoves = 0,
                    Players = new List<Tuple<long, string>>(),
                    Winner = String.Empty
                };

                await StateManager.SetStateAsync(StateName, gameState);
            }
        }

        public async Task<bool> JoinGameAsync(long playerId, string playerName)
        {
            var gameState = await StateManager.GetStateAsync<GameState>(StateName);

            if (gameState.Players.Count >= 2 
                || gameState.Players.Any(p => p.Item2 == playerName))
            {
                return false;
            }

            gameState.Players.Add(new Tuple<long, string>(playerId, playerName));

            await StateManager.SetStateAsync(StateName, gameState);

            return true;
        }

        public async Task<int[]> GetGameBoardAsync()
        {
            var gameState = await StateManager.GetStateAsync<GameState>(StateName);

            return gameState.Board;
        }

        public async Task<string> GetWinnerAsync()
        {
            var gameState = await StateManager.GetStateAsync<GameState>(StateName);

            return gameState.Winner;
        }

        public async Task<bool> MakeMoveAsync(long playerId, int x, int y)
        {
            var gameState = await StateManager.GetStateAsync<GameState>(StateName);

            if (x < 0 || x > 2  || y < 0 || y > 2
                || gameState.Players.Count != 2
                || gameState.NumberOfMoves >= 9
                || gameState.Winner != String.Empty)
            {
                return false;
            }

            var index = gameState.Players.FindIndex(p => p.Item1 == playerId);

            if (index != gameState.NextPlayerIndex)
            {
                return false;
            }

            if (gameState.Board[y*3 + x] != 0)
            {
                return false;
            }

            var piece = index*2 - 1;
            gameState.Board[y*3 + x] = piece;
            gameState.NumberOfMoves++;

            if (await HasWon(piece*3))
            {
                gameState.Winner = gameState.Players[index].Item2 + "(" + (piece == -1 ? "X" : "O") + ")";
            }
            else if (gameState.Winner == String.Empty && gameState.NumberOfMoves >= 9)
            {
                gameState.Winner = "TIE";
            }

            gameState.NextPlayerIndex = (gameState.NextPlayerIndex + 1) % 2;

            await StateManager.SetStateAsync(StateName, gameState);

            return true;
        }

        private async Task<bool> HasWon(int sum)
        {
            var gameState = await StateManager.GetStateAsync<GameState>(StateName);

            return gameState.Board[0] + gameState.Board[1] + gameState.Board[2] == sum 
                || gameState.Board[3] + gameState.Board[4] + gameState.Board[5] == sum 
                || gameState.Board[6] + gameState.Board[7] + gameState.Board[8] == sum 
                || gameState.Board[0] + gameState.Board[3] + gameState.Board[6] == sum 
                || gameState.Board[1] + gameState.Board[4] + gameState.Board[7] == sum 
                || gameState.Board[2] + gameState.Board[5] + gameState.Board[8] == sum 
                || gameState.Board[0] + gameState.Board[4] + gameState.Board[8] == sum 
                || gameState.Board[2] + gameState.Board[4] + gameState.Board[6] == sum;
        }
    }
}
