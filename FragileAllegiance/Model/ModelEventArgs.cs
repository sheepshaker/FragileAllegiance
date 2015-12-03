using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FragileAllegiance.Model
{
    public class PlayerSessionEventArgs : ModelEventArgs<Player>
    {
        public PlayerSessionEventArgs(Player player) : base(player)
        {

        }
    }

    public class AsteroidSessionEventArgs : ModelEventArgs<Asteroid>
    {
        public AsteroidSessionEventArgs(Asteroid asteroid) : base(asteroid)
        {

        }
    }

    public class ModelEventArgs<T> : EventArgs
    {
        public T Model { get; private set; }

        public ModelEventArgs(T model)
        {
            Model = model;
        }
    }
}
