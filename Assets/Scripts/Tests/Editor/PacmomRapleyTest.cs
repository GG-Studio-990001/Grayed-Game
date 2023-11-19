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

    }
}
