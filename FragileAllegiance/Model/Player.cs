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

        public void AddAsteroid(Asteroid asteroid)
        {
            lock (_asteroids)
            {
                _asteroids.Add(asteroid);
            }
        }

        public void RemoveAsteroid(Asteroid asteroid)
        {
            lock (_asteroids)
            {
                _asteroids.Remove(asteroid);
            }
        }
    }


}
