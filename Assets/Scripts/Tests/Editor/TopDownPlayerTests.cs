using NUnit.Framework;
using Runtime.CH1.Main;
using UnityEngine;

namespace Tests.Editor
{
    [TestFixture]
    public class TopDownPlayerTests
    {
        private GameObject _playerObject;
        private TopDownPlayer _player;
        private TopDownMovement _movement;
        private TopDownAnimation _animation;
        
        [SetUp]
        public void SetUp()
        {
            _playerObject = new GameObject("Player");
            
            _player = _playerObject.AddComponent<TopDownPlayer>();
            Animator animator = _playerObject.AddComponent<Animator>();
            
            _movement = new TopDownMovement(5.0f, _player.transform);
            _animation = new TopDownAnimation(_player.GetComponent<Animator>(), 0.5f);
            
            // TODO 에섯 변경되는대로 수정
            animator.runtimeAnimatorController = Resources.Load("Sample/Player") as RuntimeAnimatorController;
            
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogError("Animator Controller is null");
            }
        }
        
        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_playerObject);
        }
        
        [Test]
        public void TestPlayerMovement()
        {
            Vector2 movementInput = new Vector2(1.0f, 0.0f);
            
            _movement.Move(movementInput);
            
            Assert.AreNotEqual(Vector3.zero, _player.transform.position);
        }
        
        [Test]
        public void TestPlayerAnimation()
        {
            Animator animator = _playerObject.GetComponent<Animator>();
            _animation.SetMovementAnimation(new Vector2(1f, 0f)); 
            
            bool isMoving = animator.GetBool("IsMoving");
            
            Assert.IsTrue(isMoving);
        }
    }
}