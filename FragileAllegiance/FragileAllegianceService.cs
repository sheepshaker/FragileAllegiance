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
        
        public FragileAllegianceService(IEnumerable<Player> players, IEnumerable<Asteroid> asteroids)
        {
            _asteroidMap = asteroids.ToDictionary(a => a.AsteroidId);
            _playerMap = players.ToDictionary(p => p.PlayerName);
        }

        public IEnumerable<Player> Players
        {
            get { return _playerMap.Values; }
        }

        public IEnumerable<Asteroid> Asteroids
        {
            get { return _asteroidMap.Values; }
        }

        public Player AddPlayer(string playerName, IEnumerable<Asteroid> asteroids)
        {
            lock (_playerMap)
            {
                if (_playerMap.ContainsKey(playerName))
                    throw new Exception("Player name already exists");

                var player = new Player(playerName, asteroids);
                _playerMap.Add(playerName, player);
                PlayerStateChanged.SafeInvokeAsync(this, new PlayerStateEventArgs(new[] { player.PlayerName }, PlayerStateEventArgs.PlayerState.Joined));
                return player;
            }
        }

        public void RemovePlayer(string playerName)
        {
            lock (_playerMap)
            {
                var player = GetPlayer(playerName);
                _playerMap.Remove(playerName);
                player.RemoveAllAsteroids();
                PlayerStateChanged.SafeInvokeAsync(this, new PlayerStateEventArgs(new[] { player.PlayerName }, PlayerStateEventArgs.PlayerState.Left));
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

        public Player GetPlayer(string playerName)
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

        public Asteroid GetAsteroid(string asteroidId)
        {
            Asteroid asteroid;

            lock (_playerMap)
            {
                if (_asteroidMap.TryGetValue(asteroidId, out asteroid) == false)
                {
                    throw new Exception("Asteroid Id doesn't exist");
                }
            }

            return asteroid;
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

        private List<Asteroid> GetAsteroids(IEnumerable<string> asteroidIds)
        {
            lock (_asteroidMap)
            {
                var aIds = asteroidIds as IList<string> ?? asteroidIds.ToList();

                return _asteroidMap.Where(x => aIds.Contains(x.Key)).Select(x => x.Value).ToList();
            }
        }

        //private void ThrowIfAnyOfAsteroidIdsDoesntExist(IEnumerable<Asteroid> asteroids)
        //{
        //    lock (_asteroidMap)
        //    {
        //        var missingAsteroids = asteroids.Except(_asteroidMap.Values).ToList();
        //        if (missingAsteroids.Any())
        //        {
        //            throw new Exception(string.Format("Asteroid Ids not found: {0}", missingAsteroids));
        //        }
        //    }
        //}

        public void RemoveAsteroids(IEnumerable<string> asteroidIds)
        {
            var aIds = asteroidIds as IList<string> ?? asteroidIds.ToList();

            lock (_asteroidMap)
            {
                foreach (var asteroid in _asteroidMap.Where(a => aIds.Contains(a.Key)).ToList())
                {
                    _asteroidMap.Remove(asteroid.Key);
                }
            }

            AsteroidStateChanged.SafeInvokeAsync(this, new AsteroidStateEventArgs(aIds, AsteroidStateEventArgs.AsteroidState.Removed));
        }

        public void AddAsteroidsToPlayer(IEnumerable<Asteroid> asteroids, Player player)
        {
            var asteroidList = asteroids as IList<Asteroid> ?? asteroids.ToList();

            lock (_playerMap)
            {
                player.AddAsteroids(asteroidList);

                lock (_asteroidMap)
                {
                    SetAsteroidsOwnership(player, asteroidList);
                }
            }

            AsteroidOwnershipChanged.SafeInvokeAsync(this,
                        new AsteroidOwnershipEventArgs(asteroidList.Select(a => a.AsteroidId), player.PlayerName,
                            AsteroidOwnershipEventArgs.AsteroidOwnershipState.PlayerGainedOwnership));
        }

        public void RemoveAsteroidsFromPlayer(IEnumerable<Asteroid> asteroids, Player player)
        {
            var asteroidList = asteroids as IList<Asteroid> ?? asteroids.ToList();

            lock (_playerMap)
            {
                player.RemoveAsteroids(asteroidList);

                lock (_asteroidMap)
                {
                    ClearAsteroidsOwnership(asteroidList);
                }
            }

            AsteroidOwnershipChanged.SafeInvokeAsync(this,
                new AsteroidOwnershipEventArgs(asteroidList.Select(a => a.AsteroidId), player.PlayerName,
                    AsteroidOwnershipEventArgs.AsteroidOwnershipState.PlayerLostOwnership));
        }
    }
}
