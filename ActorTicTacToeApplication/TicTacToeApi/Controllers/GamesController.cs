using System.Collections.Generic;
using System.Web.Http;
using Game.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace TicTacToeApi.Controllers
{
    [RoutePrefix("api/games")]
    public class GamesController : ApiController
    {
        //[HttpPost]
        //[Route("")]
        //public IHttpActionResult Create()
        //{
        //    var gameId = ActorId.CreateRandom();

        //    ActorProxy.Create<IGame>()
        //}
    }
}
