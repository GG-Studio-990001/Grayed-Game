using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;

namespace Febucci.UI.Actions
{
    [System.Serializable]
    [UnityEngine.CreateAssetMenu(fileName = "Speed Action", menuName = "Text Animator/Actions/Speed", order = 1)]
    [TagInfo("speed")]
    public sealed class SpeedAction : ActionScriptableBase
    {
        /// <summary>
        /// Speed used in case the action does not have the first parameter
        /// </summary>
        [UnityEngine.Tooltip("Speed used in case the action does not have the first parameter")]
        public float defaultSpeed = 2;

        public override System.Collections.IEnumerator DoAction(ActionMarker action, TypewriterCore typewriter, TypingInfo typingInfo)
        {
            float speed = defaultSpeed;
            if(action.parameters.Length > 0)
            {
                FormatUtils.TryGetFloat(action.parameters[0], defaultSpeed, out speed);
            }

            typingInfo.speed = speed;
            yield break;
        }
    }
}