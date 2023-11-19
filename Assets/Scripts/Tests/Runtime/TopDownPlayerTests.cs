using NUnit.Framework;
using Runtime.CH1.Main;
using Runtime.CH1.Main.PlayerFunction;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Runtime
{
    [TestFixture]
    public class TopDownPlayerTests
    {
        private GameObject _playerObject;
        private TopDownPlayer _player;
        private TopDownMovement _movement;
        private TopDownAnimation _animation;
        private TopDownInteraction _interaction;
        
        [UnitySetUp]
        public IEnumerator Setup()
        {
            _playerObject = new GameObject("Player");
            Animator animator = _playerObject.AddComponent<Animator>();
            
            // TODO 에섯 변경되는대로 수정
            animator.runtimeAnimatorController = Resources.Load("Sample/Player") as RuntimeAnimatorController;
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogError("Animator Controller is null");
            }
            
            _player = _playerObject.AddComponent<TopDownPlayer>();
            _player.transform.position = Vector3.zero;
            
            yield return null;
            
            _movement = new TopDownMovement(5.0f, _player.transform);
            _animation = new TopDownAnimation(_player.GetComponent<Animator>(), 0.5f);
            _interaction = new TopDownInteraction(_player.transform, LayerMask.GetMask("Object"), 2f);
        }
        
        [UnityTest]
        public IEnumerator TestPlayerMovement()
        {
            Vector2 movementInput = new Vector2(1.0f, 0.0f);
            
            _movement.Move(movementInput);
            
            yield return null;
            
            Assert.AreNotEqual(Vector3.zero, _player.transform.position);
        }
        
        [UnityTest]
        public IEnumerator TestPlayerAnimation()
        {
            Animator animator = _playerObject.GetComponent<Animator>();
            _animation.SetMovementAnimation(new Vector2(1f, 0f)); 
            
            bool isMoving = animator.GetBool("IsMoving");
            
            yield return null;
            
            Assert.IsTrue(isMoving);
        }
        
        [UnityTest]
        public IEnumerator TestPlayerInteraction()
        {
            GameObject interactionObject = new GameObject("InteractionTestObject");
            interactionObject.AddComponent<CircleCollider2D>();
            interactionObject.AddComponent<NpcInteraction>();
            interactionObject.layer = LayerMask.NameToLayer("Object");
            
            interactionObject.transform.position = new Vector3(1.0f, 0.0f, 0.0f);
            
            yield return null;
            
            Assert.IsTrue(_interaction.Interact(Vector2.right));
            
            GameObject.Destroy(interactionObject);
        }
        
        [UnityTest]
        public IEnumerator TestPlayerInteractionFail()
        {
            GameObject interactionObject = new GameObject("InteractionTestObject");
            interactionObject.AddComponent<CircleCollider2D>();
            interactionObject.AddComponent<NpcInteraction>();
            interactionObject.layer = LayerMask.NameToLayer("Object");
            
            interactionObject.transform.position = new Vector3(0.0f, 3.0f, 0.0f);
            
            yield return null;
            
            Assert.IsFalse(_interaction.Interact(Vector2.right));
            
            GameObject.Destroy(interactionObject);
        }
        
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            GameObject.Destroy(_playerObject);
            
            yield return null;
        }
    }
}