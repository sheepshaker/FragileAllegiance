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

        public Player(string name, List<Asteroid> asteroids)
        {
            PlayerName = name;
            _asteroids = asteroids;
        }

        private Player()
        {
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

        public IEnumerable<Asteroid> OwnedAsteroids
        {
            get { return _asteroids; }
        }
    }


}
