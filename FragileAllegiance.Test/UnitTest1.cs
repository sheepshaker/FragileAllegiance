using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using FragileAllegiance.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FragileAllegiance.Test
{
    [TestClass]
    public class UnitTest1
    {
        private FragileAllegianceService _gameService;

        [TestInitialize]
        public void TestInit()
        {
            var collection = CreateInitialPlayerMap();

            _gameService = new FragileAllegianceService(collection.Item1, collection.Item2);
        }

        private Tuple<IEnumerable<Player>, IEnumerable<Asteroid>> CreateInitialPlayerMap()
        {
            List<Asteroid> asteroids = Enumerable.Range(0, 12).Select(i => new Asteroid()).ToList();
            List<Player> players =
                Enumerable.Range(0, 4)
                    .Select(i => new Player("Player " + i, asteroids.GetRange(i*3, 1)))
                    .ToList();

            return new Tuple<IEnumerable<Player>, IEnumerable<Asteroid>>(players, asteroids);
        }

        [TestMethod]
        public void InitialisationTest()
        {
            Assert.AreEqual(4, _gameService.Players.Count());
            foreach (var player in _gameService.Players)
            {
                Assert.AreEqual(1, player.OwnedAsteroids.Count());
            }

            Assert.AreEqual(12, _gameService.Asteroids.Count());
            Assert.AreEqual(8, _gameService.Asteroids.Count(a => a.OwnerPlayer == null));
            Assert.AreEqual(4, _gameService.Asteroids.Count(a => a.OwnerPlayer != null));
        }

        [TestMethod]
        public void AddRemovePlayer()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            PlayerStateEventArgs playerState = null;

            //Add

            _gameService.PlayerStateChanged += (sender, args) =>
            {
                playerState = args;
                mre.Set();
            };

            var player = _gameService.AddPlayer("Player 5", new List<Asteroid> {new Asteroid()});

            if (mre.WaitOne(1000) == false)
            {
                Assert.Fail("Timeout");
            }

            Assert.AreEqual(PlayerStateEventArgs.PlayerState.Joined, playerState.State);
            Assert.AreEqual(player.PlayerName, playerState.PlayerIds.FirstOrDefault());

            //Remove
            mre.Reset();
            _gameService.RemovePlayer(player.PlayerName);

            if (mre.WaitOne(1000) == false)
            {
                Assert.Fail("Timeout");
            }

            Assert.AreEqual(PlayerStateEventArgs.PlayerState.Left, playerState.State);
            Assert.AreEqual(player.PlayerName, playerState.PlayerIds.FirstOrDefault());
        }

        [TestMethod]
        public void AddRemoveAsteroid()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            AsteroidStateEventArgs asteroidState = null;

            //Add

            _gameService.AsteroidStateChanged += (sender, args) =>
            {
                asteroidState = args;
                mre.Set();
            };

            var asteroid = _gameService.AddAsteroid();

            if (mre.WaitOne(1000) == false)
            {
                Assert.Fail("Timeout");
            }

            Assert.AreEqual(AsteroidStateEventArgs.AsteroidState.Added, asteroidState.State);
            Assert.AreEqual(asteroid.AsteroidId, asteroidState.AsteroidIds.FirstOrDefault());

            //Remove
            mre.Reset();
            _gameService.RemoveAsteroids(new[] {asteroid.AsteroidId});

            if (mre.WaitOne(1000) == false)
            {
                Assert.Fail("Timeout");
            }

            Assert.AreEqual(AsteroidStateEventArgs.AsteroidState.Removed, asteroidState.State);
            Assert.AreEqual(asteroid.AsteroidId, asteroidState.AsteroidIds.FirstOrDefault());
        }

        [TestMethod]
        public void SwitchOwnership()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            AsteroidOwnershipEventArgs asteroidOwnershipState = null;

            _gameService.AddPlayer("Player 5", Enumerable.Empty<Asteroid>());

            var playerWithNoAsteroids = _gameService.Players.FirstOrDefault(p => !p.OwnedAsteroids.Any());
            var playerWithAsteroids = _gameService.Players.FirstOrDefault(p => p.OwnedAsteroids.Any());

            var asteroid = _gameService.Asteroids.FirstOrDefault(a => a.OwnerPlayer != null);

            var asteroid2 = _gameService.AddAsteroid();
            var asteroid3 = _gameService.AddAsteroid();

            var removeAsteroid = playerWithAsteroids.OwnedAsteroids.FirstOrDefault();

            //Remove
            mre.Reset();
            _gameService.AsteroidOwnershipChanged += (sender, args) =>
            {
                asteroidOwnershipState = args;
                mre.Set();
            };
            _gameService.RemoveAsteroidsFromPlayer(new[] { asteroid }, playerWithAsteroids);
            if (mre.WaitOne(1000) == false)
            {
                Assert.Fail("Timeout");
            }
            Assert.AreEqual(AsteroidOwnershipEventArgs.AsteroidOwnershipState.PlayerLostOwnership, asteroidOwnershipState.State);
            CollectionAssert.AreEqual(new[] { asteroid.AsteroidId }, asteroidOwnershipState.AsteroidIds.ToList());
            Assert.AreEqual(asteroid.AsteroidId, asteroidOwnershipState.AsteroidIds.FirstOrDefault());
            Assert.AreEqual(playerWithAsteroids.PlayerName, asteroidOwnershipState.PlayerId);
            Assert.AreEqual(0, playerWithAsteroids.OwnedAsteroids.Count());

            //Add
            mre.Reset();
            var list = new[] {asteroid, asteroid2, asteroid3};
            var asteroidIds = list.Select(a => a.AsteroidId).ToList();
            _gameService.AddAsteroidsToPlayer(list, playerWithNoAsteroids);
            if (mre.WaitOne(1000) == false)
            {
                Assert.Fail("Timeout");
            }
            Assert.AreEqual(AsteroidOwnershipEventArgs.AsteroidOwnershipState.PlayerGainedOwnership, asteroidOwnershipState.State);
            CollectionAssert.AreEqual(asteroidIds, asteroidOwnershipState.AsteroidIds.ToList());
            Assert.AreEqual(playerWithNoAsteroids.PlayerName, asteroidOwnershipState.PlayerId);
            Assert.AreEqual(3, playerWithNoAsteroids.OwnedAsteroids.Count());

            //Remove
            mre.Reset();
            _gameService.RemoveAsteroidsFromPlayer(
                list, playerWithNoAsteroids);
            if (mre.WaitOne(1000) == false)
            {
                Assert.Fail("Timeout");
            }
            Assert.AreEqual(AsteroidOwnershipEventArgs.AsteroidOwnershipState.PlayerLostOwnership, asteroidOwnershipState.State);
            CollectionAssert.AreEqual(asteroidIds, asteroidOwnershipState.AsteroidIds.ToList());
            Assert.AreEqual(playerWithNoAsteroids.PlayerName, asteroidOwnershipState.PlayerId);
            Assert.AreEqual(0, playerWithNoAsteroids.OwnedAsteroids.Count());
        }
    }
}
