namespace Febucci.UI.Core
{
    /// <summary>
    /// A way to store information about the typing progress between coroutines,
    /// also allowing to keep track of time between frames and characters/words showed
    /// </summary>
    public class TypingInfo
    {
        public float speed = 1;
        public float timePassed { get; internal set; } = 0;

        public TypingInfo()
        {
            this.speed = 1;
            this.timePassed = 0;
        }
    }
}