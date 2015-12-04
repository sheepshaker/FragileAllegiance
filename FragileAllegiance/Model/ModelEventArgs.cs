using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FragileAllegiance.Model
{
    public class PlayerStateEventArgs : EventArgs
    {
        public PlayerState State { get; private set; }
        public IEnumerable<Player> Players { get; private set; }

        public PlayerStateEventArgs(IEnumerable<Player> players, PlayerState state)
        {
            State = state;
            Players = players;
        }

        public enum PlayerState
        {
            Joined,
            Left
        }
    }

    public class AsteroidStateEventArgs : EventArgs
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

    public class AsteroidOwnershipEventArgs : EventArgs
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
}
