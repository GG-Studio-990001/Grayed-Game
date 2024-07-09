using System.Collections.Generic;
using System.Text;
using Febucci.UI.Styles;

namespace Febucci.UI.Core.Parsing
{
    /// <summary>
    /// Rules how to parse a rich text tag that has an opening and ending
    /// </summary>
    public class StylesParser : TagParserBase
    {
        StyleSheetScriptable sheet;
        List<string> openedTags;

        //--- CONSTRUCTORS ---
        public StylesParser(char startSymbol, char closingSymbol, char endSymbol, StyleSheetScriptable sheet) : base(startSymbol, closingSymbol, endSymbol)
        {
            this.sheet = sheet;
            openedTags = new List<string>();
        }
        
        public override bool TryProcessingTag(string textInsideBrackets, int tagLength, ref int realTextIndex, StringBuilder finalTextBuilder, int internalOrder)
        {
            if (!sheet) return false;

            textInsideBrackets = textInsideBrackets.ToLower(); //styles are case insensitive

            //Makes sure the sheet is built
            sheet.BuildOnce();

            //If the first character is a closing symbol, then it's a closing tag
            bool isClosing = textInsideBrackets[0] == closingSymbol;
            int tagStart = isClosing ? 1 : 0;

            string fullTag = textInsideBrackets.Substring(tagStart);
            
            // adds the tag to the list
            if(sheet.TryGetStyle(fullTag, out var style))
            {
                if (isClosing)
                {
                    //only pastes text to tags that have been opened
                    if (openedTags.Contains(fullTag)) 
                    {
                        finalTextBuilder.Append(style.closingTag);
                        openedTags.Remove(fullTag);
                    }
                }
                else
                {
                    finalTextBuilder.Append(style.openingTag);
                    openedTags.Add(fullTag);
                }
                
                // removes the tag from the text anyways
                return true; 
            }

            return false;
        }
    }
}