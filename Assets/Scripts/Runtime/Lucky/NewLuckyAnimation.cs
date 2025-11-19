using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Lucky
{
    public enum LuckyAnim
    {
        Idle = 0,
        Close,
        Curious,
        Laugh,
        Sad,
        Show,
    }

    public class NewLuckyAnimation
    {
        private readonly Animator _animator;

        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Close = Animator.StringToHash("Close");
        private static readonly int Curious = Animator.StringToHash("Curious");
        private static readonly int Laugh = Animator.StringToHash("Laugh");
        private static readonly int Sad = Animator.StringToHash("Sad");
        private static readonly int Show = Animator.StringToHash("Show");

        private static readonly Dictionary<LuckyAnim, int> ParamMap =
            new()
            {
            { LuckyAnim.Idle,    Idle },
            { LuckyAnim.Close,   Close },
            { LuckyAnim.Curious, Curious },
            { LuckyAnim.Laugh,   Laugh },
            { LuckyAnim.Sad,     Sad },
            { LuckyAnim.Show,    Show }
            };

        public NewLuckyAnimation(Animator animator)
        {
            _animator = animator;
        }

        public void ResetAll()
        {
            foreach (var param in ParamMap.Values)
                _animator.SetBool(param, false);
        }

        public void Play(LuckyAnim anim)
        {
            ResetAll();
            _animator.SetBool(ParamMap[anim], true);
        }
    }
}