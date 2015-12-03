using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FragileAllegiance.Model
{
    public class Space
    {
        private List<Asteroid> _asteroids;
        public event EventHandler<AsteroidSessionEventArgs> AsteroidAdded;
        public event EventHandler<AsteroidSessionEventArgs> AsteroidRemoved;

        private Space()
        {
            
        }

        public Space(List<Asteroid> asteroids)
        {
            _asteroids = asteroids;
        }
    }
}
