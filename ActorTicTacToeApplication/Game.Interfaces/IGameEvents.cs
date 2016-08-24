using Microsoft.ServiceFabric.Actors;

namespace Game.Interfaces
{
    public interface IGameEvents : IActorEvents
    {
        void NewChallengeHasArrived(string playerName);
    }
}