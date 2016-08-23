using System;
using System.Threading.Tasks;
using Game.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Player.Interfaces;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var playerOne = ActorProxy.Create<IPlayer>(ActorId.CreateRandom(), "fabric:/ActorTicTacToeApplication");
            var playerTwo = ActorProxy.Create<IPlayer>(ActorId.CreateRandom(), "fabric:/ActorTicTacToeApplication");
            var gameId = ActorId.CreateRandom();
            var game = ActorProxy.Create<IGame>(gameId, "fabric:/ActorTicTacToeApplication");
            var rand = new Random();

            var resultOne = playerOne.JoinGameAsync(gameId, "Player 1");
            var resultTwo = playerTwo.JoinGameAsync(gameId, "Player 2");

            if (!resultOne.Result || !resultTwo.Result)
            {
                Console.WriteLine("Failed to join the game.");
                return;
            }

            var playerOneTask = Task.Run(() => MakeMove(playerOne, game, gameId));
            var playerTwoTask = Task.Run(() => MakeMove(playerTwo, game, gameId));
            var gameTask = Task.Run(() =>
            {
                string winner;

                do
                {
                    var board = game.GetGameBoardAsync().Result;
                    PrintBoard(board);
                    winner = game.GetWinnerAsync().Result;
                    Task.Delay(1000).Wait();
                }
                while (winner == String.Empty);

                Console.WriteLine("Winner is: " + winner);
            });

            gameTask.Wait();
            Console.Read();
        }

        private static async Task MakeMove(IPlayer player, IGame game, ActorId gameId)
        {
            var rand = new Random();

            while (true)
            {
                await player.MakeMoveAsync(gameId, rand.Next(0, 3), rand.Next(0, 3));
                await Task.Delay(rand.Next(500, 2000));
            }
        }

        private static void PrintBoard(int[] board)
        {
            Console.Clear();

            for (var i = 0; i < board.Length; i++)
            {
                switch (board[i])
                {
                    case -1:
                        Console.Write(" X ");
                        break;
                    case 1:
                        Console.Write(" O ");
                        break;
                    default:
                        Console.Write(" . ");
                        break;
                }

                if ((i + 1)%3 == 0)
                {
                    Console.WriteLine();
                }
            }
        }
    }
}
