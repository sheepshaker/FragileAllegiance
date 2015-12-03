using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FragileAllegiance.Model
{
    public class Player
    {
        private List<Asteroid> _asteroids;
        public string PlayerName { get; private set; }

        public Player(string name)
        {
            PlayerName = name;
        }

        private Player()
        {
            
        }

        public void AddAsteroid(Asteroid asteroid)
        {
            
        }

        public void RemoveAsteroid(Asteroid asteroid)
        {
            
        }
    }


}
