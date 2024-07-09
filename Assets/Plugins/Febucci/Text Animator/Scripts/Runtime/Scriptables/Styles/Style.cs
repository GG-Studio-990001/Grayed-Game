using UnityEngine;

namespace Febucci.UI.Styles
{
    [System.Serializable]
    public struct Style
    {
        public string styleTag;

        [TextArea] public string openingTag;
        [TextArea] public string closingTag;
        
        public Style(string styleTag, string openingTag, string closingTag)
        {
            this.styleTag = styleTag;
            this.openingTag = openingTag;
            this.closingTag = closingTag;
        }
    }
}