﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FragileAllegiance.Model
{
    public class Asteroid
    {
        private static int _id;
        public string AsteroidId { get; private set; }

        public Asteroid()
        {
            AsteroidId = _id++.ToString(); //Guid.NewGuid().ToString();
        }

        public void SetOwner(Player player)
        {
            if (OwnerPlayer != null)
            {
                throw new Exception("Asteroid has already got an owner");
            }

            OwnerPlayer = player;
        }

        public void ClearOwner()
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
