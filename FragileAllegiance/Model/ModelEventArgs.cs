using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FragileAllegiance.Model
{
    public class PlayerStateEventArgs : BaseEventArgs
    {
        public PlayerState State { get; private set; }
        public IEnumerable<string> PlayerIds { get; private set; }

        public PlayerStateEventArgs(IEnumerable<string> players, PlayerState state)
        {
            State = state;
            PlayerIds = players;
        }

        public enum PlayerState
        {
            Joined,
            Left
        }
    }

    public class AsteroidStateEventArgs : BaseEventArgs
    {
        public AsteroidState State { get; private set; }
        public IEnumerable<string> AsteroidIds { get; private set; }

        public AsteroidStateEventArgs(IEnumerable<string> asteroids, AsteroidState state)
        {
            State = state;
            AsteroidIds = asteroids;
        }

        public enum AsteroidState
        {
            Added,
            Removed
        }
    }

    public class AsteroidOwnershipEventArgs : BaseEventArgs
    {
        public AsteroidOwnershipState State { get; private set; }
        public IEnumerable<string> AsteroidIds { get; private set; }
        public string PlayerId { get; private set; }

        public AsteroidOwnershipEventArgs(IEnumerable<string> asteroidIds, string playerId, AsteroidOwnershipState state)
        {
            State = state;
            AsteroidIds = asteroidIds;
            PlayerId = playerId;
        }

        public enum AsteroidOwnershipState
        {
            PlayerGainedOwnership,
            PlayerLostOwnership
        }
    }

    public abstract class BaseEventArgs : EventArgs
    {
        
    }
}
