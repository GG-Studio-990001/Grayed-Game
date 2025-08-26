namespace Runtime.CH3.Main
{
    public interface IInventoryOps
    {
        ItemStack GetSlot(int index);
        void MoveOrMerge(int from, int to);
        void ClearSlot(int index);
    }
}


