using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CH3.Main
{
    [Serializable]
    public class ItemStack
    {
        public Item item;
        public int count;

        public ItemStack(Item item, int count)
        {
            this.item = item;
            this.count = count;
        }
    }

    public class Inventory : MonoBehaviour, IInventoryOps
    {
        public const int HotbarSize = 7;
        public const int Width = 7;
        public const int Height = 4; // includes hotbar row
        public const int Capacity = Width * Height; // 28
        public const int MaxStack = 999;

        [SerializeField] private List<ItemStack> slots = new List<ItemStack>(Capacity);

        public IReadOnlyList<ItemStack> Slots => slots;

        public event Action<int, ItemStack> OnSlotChanged;

        private void Awake()
        {
            if (slots.Count != Capacity)
            {
                slots.Clear();
                for (int i = 0; i < Capacity; i++)
                {
                    slots.Add(new ItemStack(null, 0));
                }
            }
        }

        public bool IsFull()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var s = slots[i];
                if (s.item == null || s.count < MaxStack)
                {
                    return false;
                }
            }
            return true;
        }

        public bool TryAdd(Item item, int amount)
        {
            if (item == null || amount <= 0) return false;

            // First, fill existing stacks of same item
            for (int i = 0; i < slots.Count && amount > 0; i++)
            {
                var s = slots[i];
                if (s.item == item && s.count < MaxStack)
                {
                    int canTake = Mathf.Min(MaxStack - s.count, amount);
                    s.count += canTake;
                    amount -= canTake;
                    OnSlotChanged?.Invoke(i, s);
                }
            }

            // Then, place new stacks into empty slots
            for (int i = 0; i < slots.Count && amount > 0; i++)
            {
                var s = slots[i];
                if (s.item == null)
                {
                    int put = Mathf.Min(MaxStack, amount);
                    s.item = item;
                    s.count = put;
                    amount -= put;
                    OnSlotChanged?.Invoke(i, s);
                }
            }

            return amount == 0;
        }

        public void ClearSlot(int index)
        {
            if (index < 0 || index >= slots.Count) return;
            var s = slots[index];
            s.item = null;
            s.count = 0;
            OnSlotChanged?.Invoke(index, s);
        }

        public void SetSlot(int index, ItemStack newStack)
        {
            if (index < 0 || index >= slots.Count) return;
            var s = slots[index];
            s.item = newStack?.item;
            s.count = newStack?.count ?? 0;
            OnSlotChanged?.Invoke(index, s);
        }

        public ItemStack GetSlot(int index)
        {
            if (index < 0 || index >= slots.Count) return null;
            var s = slots[index];
            return new ItemStack(s.item, s.count);
        }

        public void SwapSlots(int a, int b)
        {
            if (a == b) return;
            if (a < 0 || b < 0 || a >= slots.Count || b >= slots.Count) return;
            var aVal = GetSlot(a);
            var bVal = GetSlot(b);
            SetSlot(a, bVal);
            SetSlot(b, aVal);
        }

        public void MoveOrMerge(int from, int to)
        {
            if (from == to) return;
            if (from < 0 || to < 0 || from >= slots.Count || to >= slots.Count) return;

            var src = slots[from];
            var dst = slots[to];

            // If same item and not full, merge
            if (src.item != null && dst.item == src.item && dst.count < MaxStack)
            {
                int canTake = Mathf.Min(MaxStack - dst.count, src.count);
                dst.count += canTake;
                src.count -= canTake;
                if (src.count <= 0)
                {
                    src.item = null;
                    src.count = 0;
                }
                OnSlotChanged?.Invoke(from, src);
                OnSlotChanged?.Invoke(to, dst);
                return;
            }

            // otherwise swap
            var temp = new ItemStack(dst.item, dst.count);
            dst.item = src.item;
            dst.count = src.count;
            src.item = temp.item;
            src.count = temp.count;
            OnSlotChanged?.Invoke(from, src);
            OnSlotChanged?.Invoke(to, dst);
        }

        public bool TryConsumeAt(int index, int amount)
        {
            if (index < 0 || index >= slots.Count || amount <= 0) return false;
            var s = slots[index];
            if (s.item == null || s.count <= 0) return false;
            int consume = Mathf.Min(amount, s.count);
            s.count -= consume;
            if (s.count <= 0)
            {
                s.item = null;
                s.count = 0;
            }
            OnSlotChanged?.Invoke(index, s);
            return true;
        }
    }
}


