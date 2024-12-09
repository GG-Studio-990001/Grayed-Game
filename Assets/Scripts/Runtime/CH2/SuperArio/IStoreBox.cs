namespace Runtime.CH2.SuperArio
{
    public interface IStoreBox
    {
        public bool IsUsed { get; set; }
        public void Check();
        public void Use();
        public void SetColorGray();
        public void ResetColor();
    }
}