using System.Text;
using Febucci.UI.Actions;

namespace Febucci.UI.Core.Parsing
{
    public sealed class ActionParser : TagParserBase
    {
        public ActionDatabase database;

        //--- RESULTS ---
        ActionMarker[] _results;
        public ActionMarker[] results => _results; //TODO cache

        public ActionParser(char startSymbol, char closingSymbol, char endSymbol, ActionDatabase actionDatabase) 
        : base(startSymbol, closingSymbol, endSymbol) 
        { 
            this.database = actionDatabase;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _results = new ActionMarker[0];
            if(database) database.BuildOnce();
        }

        public override bool TryProcessingTag(string textInsideBrackets, int tagLength, ref int realTextIndex, StringBuilder finalTextBuilder, int internalOrder)
        {
            if (!database)
                return false;
            
            database.BuildOnce();
            //gets the name of the action from the tag
            //if there's an equal sign, it means there are parameters
            int equalIndex = textInsideBrackets.IndexOf('=');
            string actionName = equalIndex == -1 ? textInsideBrackets : textInsideBrackets.Substring(0, equalIndex);
            actionName = actionName.ToLower(); //action names are case insensitive

            if (!database.ContainsKey(actionName)) return false; //skips unrecognized tags

            //Creates a new action
            ActionMarker textAction;

            //If the action has parameters
            if(equalIndex != -1)
            {
                string parameters = textInsideBrackets.Substring(equalIndex + 1);
                textAction = new ActionMarker(actionName, realTextIndex, internalOrder, parameters.Replace(" ", "").Split(',')); 
            }
            else
            {
                textAction = new ActionMarker(actionName, realTextIndex, internalOrder, new string[0]);
            }
            
            //adds action to results
            System.Array.Resize(ref _results, _results.Length + 1);
            _results[_results.Length - 1] = textAction;

            return true;
        }
    }
}