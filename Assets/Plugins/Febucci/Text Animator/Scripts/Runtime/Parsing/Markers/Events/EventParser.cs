using System.Text;

namespace Febucci.UI.Core.Parsing
{
    public class EventParser : TagParserBase
    {
        const char eventSymbol = '?';

        public EventParser(char openingBracket, char closingBracket, char closingTagSymbol) 
            : base(openingBracket, closingBracket, closingTagSymbol){ }

        EventMarker[] _results;

        public EventMarker[] results => _results;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _results = new EventMarker[0];
        }

        public override bool TryProcessingTag(string textInsideBrackets, int tagLength, ref int realTextIndex, StringBuilder finalTextBuilder, int internalOrder)
        {
            //If the first character is not the event symbol, skips
            if (textInsideBrackets[0] != eventSymbol)
                return false;
            
            //Creates a new event
            EventMarker textEvent;

            //If the event has parameters
            int indexOfEquals = textInsideBrackets.IndexOf('=');
            if(indexOfEquals != -1)
            {
                string eventName = textInsideBrackets.Substring(1, indexOfEquals - 1);
                string parameters = textInsideBrackets.Substring(indexOfEquals + 1);

                //TODO fast strip
                textEvent = new EventMarker(eventName, realTextIndex, internalOrder, parameters.Replace(" ", "").Split(',')); 
            }
            else
            {
                textEvent = new EventMarker(textInsideBrackets.Substring(1), realTextIndex, internalOrder, new string[0]);
            }

            System.Array.Resize(ref _results, _results.Length + 1);
            _results[_results.Length - 1] = textEvent;

            return true;
        }
    }
}