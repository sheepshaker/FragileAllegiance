using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FragileAllegiance.Model
{
    public class Player
    {
        private readonly List<Asteroid> _asteroids;
        public string PlayerName { get; private set; }

        public Player(string name, IEnumerable<Asteroid> asteroids)
        {
            PlayerName = name;
            _asteroids = asteroids.ToList();

            SetAsteroidOwnership(_asteroids);
        }

        private Player()
        {
        }

        private void SetAsteroidOwnership(IEnumerable<Asteroid> asteroids)
        {
            foreach (var asteroid in asteroids)
            {
                asteroid.SetOwner(this);
            }
        }

        public void AddAsteroids(IEnumerable<Asteroid> asteroids)
        {
            lock (_asteroids)
            {
                _asteroids.AddRange(asteroids);
            }
        }

        public void RemoveAsteroids(IEnumerable<Asteroid> asteroids)
        {
            lock (_asteroids)
            {
                _asteroids.RemoveAll(asteroids.Contains);
            }
        }
        
        public void RemoveAllAsteroids()
        {
            lock (_asteroids)
            {
                _asteroids.Clear();
            }
        }

        public IEnumerable<Asteroid> OwnedAsteroids
        {
            get { return _asteroids; }
        }
    }
}
