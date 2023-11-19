using NUnit.Framework;
using Runtime.CH1.Pacmom;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor
{
    [TestFixture]
    public class PacmomRapleyTest
    {
        private GameObject player;
        private Rapley rapley;
        private RapleyMovement rapleyMovement;

        [SetUp]
        public void SetUp()
        {
            player = new GameObject("Player");
            player.AddComponent<Rapley>();
            player.AddComponent<RapleyMovement>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(player);
        }

        [Test]
        public void TestObstacleDetect()
        {
            // 시작 위치에서 좌우로 뚫려있음
            player.GetComponent<RapleyMovement>().obstacleLayer = LayerMask.GetMask("Obstacle");
            Assert.AreEqual(false, player.GetComponent<RapleyMovement>().CheckRoadBlocked(new Vector2(1, 0)));
        }

        [Test]
        public void TestNoObstacleDetect()
        {
            // 시작 위치에서 위아래로 막혀있음
            player.GetComponent<RapleyMovement>().obstacleLayer = LayerMask.GetMask("Obstacle");
            Assert.AreEqual(true, player.GetComponent<RapleyMovement>().CheckRoadBlocked(new Vector2(0, 1)));
        }
    }
}
