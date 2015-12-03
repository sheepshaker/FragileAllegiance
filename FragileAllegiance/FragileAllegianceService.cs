using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FragileAllegiance.Model;

namespace FragileAllegiance
{
    public class FragileAllegianceService
    {
        private readonly Dictionary<string, Player> _playerMap;
        private readonly Dictionary<string, Asteroid> _asteroidMap;
        public event EventHandler<PlayerStateEventArgs> PlayerStateChanged;
        public event EventHandler<AsteroidStateEventArgs> AsteroidStateChanged;
        public event EventHandler<AsteroidOwnershipEventArgs> AsteroidOwnershipChanged;


        private FragileAllegianceService()
        {
            
        }
        
        public FragileAllegianceService(Dictionary<string, Player> playerMap, Dictionary<string, Asteroid> asteroidMap)
        {
            _asteroidMap = asteroidMap;
            _playerMap = playerMap;
        }

        public Player AddPlayer(string playerName, List<Asteroid> asteroids)
        {
            lock (_playerMap)
            {
                if (_playerMap.ContainsKey(playerName))
                    throw new Exception("Player name already exists");

                var player = new Player(playerName, asteroids);
                _playerMap.Add(playerName, player);
                PlayerStateChanged.SafeInvokeAsync(this, new PlayerStateEventArgs(new[] { player }, PlayerStateEventArgs.PlayerState.Joined));
                return player;
            }
        }

        public void RemovePlayer(string playerName)
        {
            lock (_playerMap)
            {
                Player player;
                if (_playerMap.TryGetValue(playerName, out player) == false)
                {
                    throw new Exception("Player name already exists");
                }

                _playerMap.Remove(playerName);
                PlayerStateChanged.SafeInvokeAsync(this, new PlayerStateEventArgs(new[] { player}, PlayerStateEventArgs.PlayerState.Left));
            }

            
        }

        public Asteroid AddAsteroid()
        {
            lock (_asteroidMap)
            {
                var asteroid = new Asteroid();
                _asteroidMap[asteroid.AsteroidId] = asteroid;
                AsteroidStateChanged.SafeInvokeAsync(this, new AsteroidStateEventArgs(new[] { asteroid}, AsteroidStateEventArgs.AsteroidState.Added));
                return asteroid;
            }
        }

        public void RemoveAsteroid(string asteroidId)
        {
            lock (_asteroidMap)
            {
                Asteroid asteroid;
                if (_asteroidMap.TryGetValue(asteroidId, out asteroid) == false)
                {
                  throw new Exception("Asteroid ID doesn't exist");  
                }

                _asteroidMap.Remove(asteroidId);
                AsteroidStateChanged.SafeInvokeAsync(this, new AsteroidStateEventArgs(new [] { asteroid}, AsteroidStateEventArgs.AsteroidState.Removed));
            }
        }

        public void AddAsteroidToPlayer(Asteroid asteroid, Player player)
        {
            lock (_asteroidMap)
            {
                lock (_playerMap)
                {
                    
                }
            }
        }

        public void RemoveAsteroidFromPlayer(Asteroid asteroid, Player player)
        {
            lock (_asteroidMap)
            {
                lock (_playerMap)
                {

                }
            }
        }
    }
}
