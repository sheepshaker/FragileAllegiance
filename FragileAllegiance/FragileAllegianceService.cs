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
                var player = GetPlayer(playerName);
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
                AsteroidStateChanged.SafeInvokeAsync(this,
                    new AsteroidStateEventArgs(new[] {asteroid.AsteroidId}, AsteroidStateEventArgs.AsteroidState.Added));
                return asteroid;
            }
        }

        private Player GetPlayer(string playerName)
        {
            Player player;

            lock (_playerMap)
            {
                if (_playerMap.TryGetValue(playerName, out player) == false)
                {
                    throw new Exception("Player name doesn't exist");
                }
            }

            return player;
        }

        private void SetAsteroidsOwnership(Player player, IEnumerable<Asteroid> asteroids)
        {
            foreach (var asteroid in asteroids)
            {
                asteroid.SetOwner(player);
            }
        }

        private void ClearAsteroidsOwnership(IEnumerable<Asteroid> asteroids)
        {
            foreach (var asteroid in asteroids)
            {
                asteroid.ClearOwner();
            }
        }

        private IList<Asteroid> GetAsteroids(IEnumerable<string> asteroidIds)
        {
            lock (_asteroidMap)
            {
                var aIds = asteroidIds as IList<string> ?? asteroidIds.ToList();

                ThrowIfAnyOfAsteroidIdsDoesntExist(aIds);

                return aIds.Select(x => _asteroidMap[x]).ToList();
            }
        }

        private void ThrowIfAnyOfAsteroidIdsDoesntExist(IEnumerable<string> asteroidIds)
        {
            lock (_asteroidMap)
            {
                var missingAsteroidIds = asteroidIds.Except(_asteroidMap.Keys).ToList();
                if (missingAsteroidIds.Any())
                {
                    throw new Exception(string.Format("Asteroid Ids not found: {0}", missingAsteroidIds));
                }
            }
        }

        public void RemoveAsteroids(IEnumerable<string> asteroidIds)
        {
            var aIds = asteroidIds as IList<string> ?? asteroidIds.ToList();

            lock (_asteroidMap)
            {
                ThrowIfAnyOfAsteroidIdsDoesntExist(aIds);

                foreach (var asteroid in _asteroidMap.Where(a => aIds.Contains(a.Key)).ToList())
                {
                    _asteroidMap.Remove(asteroid.Key);
                }
            }

            AsteroidStateChanged.SafeInvokeAsync(this, new AsteroidStateEventArgs(aIds, AsteroidStateEventArgs.AsteroidState.Removed));
        }

        public void AddAsteroidsToPlayer(IEnumerable<string> asteroidIds, string playerId)
        {
            var player = GetPlayer(playerId);


            var aIds = asteroidIds as IList<string> ?? asteroidIds.ToList();
            var asteroids = GetAsteroids(aIds);

            lock (_playerMap)
            {
                player.AddAsteroids(asteroids);

                lock (_asteroidMap)
                {
                    SetAsteroidsOwnership(player, asteroids);
                }
            }

            AsteroidOwnershipChanged.SafeInvokeAsync(this,
                        new AsteroidOwnershipEventArgs(aIds, playerId,
                            AsteroidOwnershipEventArgs.AsteroidOwnershipState.PlayerGainedOwnership));
        }

        public void RemoveAsteroidsFromPlayer(IEnumerable<string> asteroidIds, string playerId)
        {
            var player = GetPlayer(playerId);


            var aIds = asteroidIds as IList<string> ?? asteroidIds.ToList();
            var asteroids = GetAsteroids(aIds);

            lock (_playerMap)
            {
                player.AddAsteroids(asteroids);

                lock (_asteroidMap)
                {
                    ClearAsteroidsOwnership(asteroids);
                }
            }

            AsteroidOwnershipChanged.SafeInvokeAsync(this, new AsteroidOwnershipEventArgs(asteroidIds, playerId, AsteroidOwnershipEventArgs.AsteroidOwnershipState.PlayerLostOwnership));
        }
    }
}
