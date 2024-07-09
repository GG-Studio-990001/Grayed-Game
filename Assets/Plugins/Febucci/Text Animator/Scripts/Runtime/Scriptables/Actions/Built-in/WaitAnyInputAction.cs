using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using UnityEngine;

namespace Febucci.UI.Actions
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "WaitAnyInput Action", menuName = "Text Animator/Actions/Wait Any Input", order = 1)]
    [TagInfo("waitinput")]
    public sealed class WaitAnyInputAction : ActionScriptableBase
    {
        public override System.Collections.IEnumerator DoAction(ActionMarker action, TypewriterCore typewriter, TypingInfo typingInfo)
        {
            while(!Input.anyKeyDown)
                yield return null;
        }
    }
}