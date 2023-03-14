namespace FixIt
{
    public class Letter
    {
        private string _lang;
        private Keys _key;
        private CaseFlag _caseFlag;
        private char _lowerChar;
        private char _upperChar;
        public string Lang { get { return _lang; } set { _lang = value; } }
        public CaseFlag CaseFlag { get { return _caseFlag; } }
        public Keys Key { get { return _key; } }
        public Letter(Keys key, string language, CaseFlag caseFlag)
        {
            _caseFlag = caseFlag;
            _key = key;
            _lang = language;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Letter other = (Letter)obj;
            return _key == other._key && _lang == other._lang && _caseFlag == other._caseFlag;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + _key.GetHashCode();
            hash = hash * 23 + _lang.GetHashCode();
            hash = hash * 23 + _caseFlag.GetHashCode();
            return hash;
        }
    }
    public enum CaseFlag
    {
        LowerCase, UpperCase
    }
}
