using UnityEngine;
using System.Text;

namespace Febucci.UI.Core.Parsing
{
    public struct TagRange
    {
        /// <summary>
        /// text index range of where to apply the tag
        /// </summary>
        public Vector2Int indexes;
        public ModifierInfo[] modifiers;

        public TagRange(Vector2Int indexes, params ModifierInfo[] modifiers)
        {
            this.indexes = indexes;
            this.modifiers = modifiers;
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();

            text.Append("indexes: ");
            text.Append(indexes);
            if (modifiers == null || modifiers.Length == 0)
                text.Append("\n no modifiers");
            else
            {
                for (int i = 0; i < modifiers.Length; i++)
                {
                    text.Append('\n');
                    text.Append('-');
                    text.Append(modifiers[i]);
                }
            }
            return text.ToString();
        }
    }

}