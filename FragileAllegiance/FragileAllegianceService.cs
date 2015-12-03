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
        Space _space;
        readonly Dictionary<string, Player> _playerMap;
        public event EventHandler<PlayerSessionEventArgs> PlayerJoined;
        public event EventHandler<PlayerSessionEventArgs> PlayerLeft;

        private FragileAllegianceService()
        {
            
        }
        
        public FragileAllegianceService(Space space, Dictionary<string, Player> playerMap)
        {
            _space = space;
            _playerMap = playerMap;
        }

        public void AddPlayer(string playerName)
        {
            lock (_playerMap)
            {
                if (_playerMap.ContainsKey(playerName))
                    throw new Exception("Player name already exists");

                var player = new Player(playerName);
                _playerMap.Add(playerName, player);
                FireEvent(PlayerJoined, this, new PlayerSessionEventArgs(player));
            }
        }

        public void RemovePlayer(string playerName)
        {
            Player player;
            lock (_playerMap)
            {
                if (_playerMap.TryGetValue(playerName, out player) == false)
                {
                    throw new Exception("Player name already exists");
                }

                _playerMap.Remove(playerName);
            }
            FireEvent(PlayerLeft, this, new PlayerSessionEventArgs(player));
        }

        private void FireEvent<T>(EventHandler<T> eventHandler, object sender, T args) where T:EventArgs
        {
            eventHandler?.BeginInvoke(sender, args, null, null);
        }
    }
}
