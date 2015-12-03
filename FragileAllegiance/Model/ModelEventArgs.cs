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
        public IEnumerable<Asteroid> Asteroid { get; private set; }

        public AsteroidStateEventArgs(IEnumerable<Asteroid> asteroid, AsteroidState state)
        {
            State = state;
            Asteroid = asteroid;
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
        public IEnumerable<Asteroid> Asteroids { get; private set; }
        public Player Player { get; private set; }

        public AsteroidOwnershipEventArgs(IEnumerable<Asteroid> asteroids, Player player, AsteroidOwnershipState state)
        {
            State = state;
            Asteroids = asteroids;
            Player = player;
        }

        public enum AsteroidOwnershipState
        {
            PlayerGainedOwnership,
            PlayerLostOwnership
        }
    }
}
