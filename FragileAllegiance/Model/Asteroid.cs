using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FragileAllegiance.Model
{
    public class Asteroid
    {
        public string AsteroidId { get; private set; }

        public Asteroid()
        {
            AsteroidId = Guid.NewGuid().ToString();
        }

        public void AddOwner(Player player)
        {
            if (OwnerPlayer != null)
            {
                throw new Exception("Asteroid has already got an owner");
            }
        }

        public void RemoveOwner()
        {
            if (OwnerPlayer == null)
            {
                throw new Exception("Asteroid has not got an owner");
            }

            OwnerPlayer = null;
        }

        public Player OwnerPlayer { get; private set; }
    }
}
