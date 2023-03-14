using System.Text;

namespace FixIt
{
    public enum Lang
    {
        EN,
        RU,
        UA
    }

    class KeyboardLayoutDictionary
    {
        public readonly List<Keys> allowedKeys = new() {
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
            Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
            Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z, Keys.Space, Keys.OemSemicolon,
            Keys.OemCloseBrackets, Keys.OemOpenBrackets, Keys.Oemcomma, Keys.OemPeriod,
            Keys.OemQuestion, Keys.OemQuotes, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
            Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.OemMinus, Keys.Oemplus,
            Keys.Oemtilde, Keys.OemPipe, Keys.OemBackslash, Keys.Multiply, Keys.Divide,
            Keys.LShiftKey, Keys.RShiftKey, Keys.Shift, Keys.ShiftKey};

        //Transcription layout dictionaries
        private readonly Dictionary<char, char> _enToRuLower = new()
        {
            {'q', 'й'}, {'w', 'ц'}, {'e', 'у'}, {'r', 'к'}, {'t', 'е'}, {'y', 'н'},
            {'u', 'г'}, {'i', 'ш'}, {'o', 'щ'}, {'p', 'з'}, {'[', 'х'}, {']', 'ъ'},
            {'a', 'ф'}, {'s', 'ы'}, {'d', 'в'}, {'f', 'а'}, {'g', 'п'}, {'h', 'р'},
            {'j', 'о'}, {'k', 'л'}, {'l', 'д'}, {';', 'ж'}, {'\'', 'э'}, {'z', 'я'},
            {'x', 'ч'}, {'c', 'с'}, {'v', 'м'}, {'b', 'и'}, {'n', 'т'}, {'m', 'ь'},
            {',', 'б'}, {'.', 'ю'}, {' ', ' '}, {'/', '.'}  };
        private readonly Dictionary<char, char> _enToRuUpper = new()
        {
            {'Q', 'Й'}, {'W', 'Ц'}, {'E', 'У'}, {'R', 'К'}, {'T', 'Е'}, {'Y', 'Н'},
            {'U', 'Г'}, {'I', 'Ш'}, {'O', 'Щ'}, {'P', 'З'}, {'{', 'Х'}, {'}', 'Ъ'},
            {'A', 'Ф'}, {'S', 'Ы'}, {'D', 'В'}, {'F', 'А'}, {'G', 'П'}, {'H', 'Р'},
            {'J', 'О'}, {'K', 'Л'}, {'L', 'Д'}, {':', 'Ж'}, {'"', 'Э'}, {'Z', 'Я'},
            {'X', 'Ч'}, {'C', 'С'}, {'V', 'М'}, {'B', 'И'}, {'N', 'Т'}, {'M', 'Ь'},
            {'<', 'Б'}, {'>', 'Ю'}, {'?', ','}, {' ', ' '}
        };
        private readonly Dictionary<char, char> _ruToEnLower = new()
        {
            {'й', 'q'}, {'ц', 'w'}, {'у', 'e'}, {'к', 'r'}, {'е', 't'}, {'н', 'y'}, {'г', 'u'},
            {'ш', 'i'}, {'щ', 'o'}, {'з', 'p'}, {'х', '['}, {'ъ', ']'}, {'ф', 'a'}, {'ы', 's'},
            {'в', 'd'}, {'а', 'f'}, {'п', 'g'}, {'р', 'h'}, {'о', 'j'}, {'л', 'k'}, {'д', 'l'},
            {'ж', ';'}, {'э', '\''}, {'я', 'z'}, {'ч', 'x'}, {'с', 'c'}, {'м', 'v'}, {'и', 'b'},
            {'т', 'n'}, {'ь', 'm'}, {'б', ','}, {'ю', '.'}, {' ', ' '}, {'.', '/'}
        };
        private readonly Dictionary<char, char> _ruToEnUpper = new()
        {
            {'Й', 'Q'}, {'Ц', 'W'}, {'У', 'E'}, {'К', 'R'}, {'Е', 'T'}, {'Н', 'Y'}, {'Г', 'U'},
            {'Ш', 'I'}, {'Щ', 'O'}, {'З', 'P'}, {'Х', '{'}, {'Ъ', '}'}, {'Ф', 'A'}, {'Ы', 'S'},
            {'В', 'D'}, {'А', 'F'}, {'П', 'G'}, {'Р', 'H'}, {'О', 'J'}, {'Л', 'K'}, {'Д', 'L'},
            {'Ж', ':'}, {'Э', '\"'}, {'Я', 'Z'}, {'Ч', 'X'}, {'С', 'C'}, {'М', 'V'}, {'И', 'B'},
            {'Т', 'N'}, {'Ь', 'M'}, {'Б', '<'}, {'Ю', '>'}, {' ', ' '}, {',', '?'}
        };
        private readonly Dictionary<char, char> _ukToRuLower = new()
        {
            {'а', 'а'}, {'б', 'б'}, {'в', 'в'}, {'г', 'г'}, {'д', 'д'}, {'е', 'е'}, {'є', 'э'},
            {'ж', 'ж'}, {'з', 'з'}, {'и', 'и'}, {'і', 'ы'}, {'ї', 'ъ'}, {'й', 'й'}, {'к', 'к'}, {'л', 'л'},
            {'м', 'м'}, {'н', 'н'}, {'о', 'о'}, {'п', 'п'}, {'р', 'р'}, {'с', 'с'}, {'т', 'т'}, {'у', 'у'},
            {'ф', 'ф'}, {'х', 'х'}, {'ц', 'ц'}, {'ч', 'ч'}, {'ш', 'ш'}, {'щ', 'щ'}, {'ь', 'ь'}, {'ю', 'ю'},
            {'я', 'я'}};
        private readonly Dictionary<char, char> _ukToRuUpper = new()
        {
            {'А', 'А'}, {'Б', 'Б'}, {'В', 'В'}, {'Г', 'Г'}, {'Д', 'Д'}, {'Е', 'Е'}, {'Є', 'Э'},
            {'Ж', 'Ж'}, {'З', 'З'}, {'И', 'И'}, {'І', 'Ы'}, {'Ї', 'Ъ'}, {'Й', 'Й'}, {'К', 'К'}, {'Л', 'Л'},
            {'М', 'М'}, {'Н', 'Н'}, {'О', 'О'}, {'П', 'П'}, {'Р', 'Р'}, {'С', 'С'}, {'Т', 'Т'}, {'У', 'У'},
            {'Ф', 'Ф'}, {'Х', 'Х'}, {'Ц', 'Ц'}, {'Ч', 'Ч'}, {'Ш', 'Ш'}, {'Щ', 'Щ'}, {'Ь', 'Ь'}, {'Ю', 'Ю'},
            {'Я', 'Я'}
        };

        //Keys to chars dictionaries
        private readonly Dictionary<Keys, char> _ruLowerDictionary = new()
        {
        { Keys.A, 'ф' }, { Keys.B, 'и' }, { Keys.C, 'с' }, { Keys.D, 'в' }, { Keys.E, 'у' },
        { Keys.F, 'а' }, { Keys.G, 'п' }, { Keys.H, 'р' }, { Keys.I, 'ш' }, { Keys.J, 'о' },
        { Keys.K, 'л' }, { Keys.L, 'д' }, { Keys.M, 'ь' }, { Keys.N, 'т' }, { Keys.O, 'щ' },
        { Keys.P, 'з' }, { Keys.Q, 'й' }, { Keys.R, 'к' }, { Keys.S, 'ы' }, { Keys.T, 'е' },
        { Keys.U, 'г' }, { Keys.V, 'м' }, { Keys.W, 'ц' }, { Keys.X, 'ч' }, { Keys.Y, 'н' },
        { Keys.Z, 'я' }, { Keys.OemSemicolon, 'ж' }, { Keys.Oem7, 'э' }, { Keys.Oemcomma, 'б' },
        { Keys.OemPeriod, 'ю' }, { Keys.OemOpenBrackets, 'х' }, { Keys.Oem6, 'ъ' },
            {Keys.OemQuestion, '.'}, {Keys.Space, ' '}
    };
        private readonly Dictionary<Keys, char> _ruUpperDictionary = new()
        {
        { Keys.A, 'Ф' }, { Keys.B, 'И' }, { Keys.C, 'С' }, { Keys.D, 'В' }, { Keys.E, 'У' },
        { Keys.F, 'А' }, { Keys.G, 'П' }, { Keys.H, 'Р' }, { Keys.I, 'Ш' }, { Keys.J, 'О' },
        { Keys.K, 'Л' }, { Keys.L, 'Д' }, { Keys.M, 'Ь' }, { Keys.N, 'Т' }, { Keys.O, 'Щ' },
        { Keys.P, 'З' }, { Keys.Q, 'Й' }, { Keys.R, 'К' }, { Keys.S, 'Ы' }, { Keys.T, 'Е' },
        { Keys.U, 'Г' }, { Keys.V, 'М' }, { Keys.W, 'Ц' }, { Keys.X, 'Ч' }, { Keys.Y, 'Н' },
        { Keys.Z, 'Я' }, { Keys.OemSemicolon, 'Ж' }, { Keys.Oem7, 'Э' }, { Keys.Oemcomma, 'Б' },
        { Keys.OemPeriod, 'Ю' }, { Keys.OemOpenBrackets, 'Х' }, { Keys.Oem6, 'Ъ' },
            {Keys.OemQuestion, ','}, {Keys.Space, ' '}
    };
        private readonly Dictionary<Keys, char> _enLowerDictionary = new()
        {
        {Keys.A, 'a' }, { Keys.B, 'b' }, { Keys.C, 'c' }, { Keys.D, 'd' }, { Keys.E, 'e' },
        { Keys.F, 'f' }, { Keys.G, 'g' }, { Keys.H, 'h' }, { Keys.I, 'i' }, { Keys.J, 'j' },
        { Keys.K, 'k' }, { Keys.L, 'l' }, { Keys.M, 'm' }, { Keys.N, 'n' }, { Keys.O, 'o' },
        { Keys.P, 'p' }, { Keys.Q, 'q' }, { Keys.R, 'r' }, { Keys.S, 's' }, { Keys.T, 't' },
        { Keys.U, 'u' }, { Keys.V, 'v' }, { Keys.W, 'w' }, { Keys.X, 'x' }, { Keys.Y, 'y' },
        { Keys.Z, 'z' }, { Keys.OemOpenBrackets, '[' }, { Keys.OemCloseBrackets, ']' },
        {Keys.OemQuestion, '/'}, {Keys.Space, ' '}, {Keys.OemPeriod, '.' }, {Keys.Oemcomma, ',' },
        {Keys.Oem1, ';'}, {Keys.Oem7, '\''},
        };
        private readonly Dictionary<Keys, char> _enUpperDictionary = new()
        {
        { Keys.A, 'A' }, { Keys.B, 'B' }, { Keys.C, 'C' }, { Keys.D, 'D' }, { Keys.E, 'E' },
        { Keys.F, 'F' }, { Keys.G, 'G' }, { Keys.H, 'H' }, { Keys.I, 'I' }, { Keys.J, 'J' },
        { Keys.K, 'K' }, { Keys.L, 'L' }, { Keys.M, 'M' }, { Keys.N, 'N' }, { Keys.O, 'O' },
        { Keys.P, 'P' }, { Keys.Q, 'Q' }, { Keys.R, 'R' }, { Keys.S, 'S' }, { Keys.T, 'T' },
        { Keys.U, 'U' }, { Keys.V, 'V' }, { Keys.W, 'W' }, { Keys.X, 'X' }, { Keys.Y, 'Y' },
        { Keys.Z, 'Z' }, { Keys.OemOpenBrackets, '{' }, { Keys.OemCloseBrackets, '}' },
            {Keys.OemQuestion, '?'}, {Keys.Space, ' '}, {Keys.OemPeriod, '>' }, {Keys.Oemcomma, '<' },
            {Keys.Oem1, ':'}, {Keys.Oem7, '\"'},
        };
        private readonly Dictionary<Keys, char> _ukLowerDictionary = new()
        {
        { Keys.A, 'ф' }, { Keys.B, 'и' }, { Keys.C, 'с' }, { Keys.D, 'в' }, { Keys.E, 'у' },
        { Keys.F, 'а' }, { Keys.G, 'п' }, { Keys.H, 'р' }, { Keys.I, 'ш' }, { Keys.J, 'о' },
        { Keys.K, 'л' }, { Keys.L, 'д' }, { Keys.M, 'ь' }, { Keys.N, 'т' }, { Keys.O, 'щ' },
        { Keys.P, 'з' }, { Keys.Q, 'й' }, { Keys.R, 'к' }, { Keys.S, 'і' }, { Keys.T, 'е' },
        { Keys.U, 'г' }, { Keys.V, 'м' }, { Keys.W, 'ц' }, { Keys.X, 'ч' }, { Keys.Y, 'н' },
        { Keys.Z, 'я' }, { Keys.OemSemicolon, 'ж' }, { Keys.OemQuotes, 'є' }, { Keys.Oemcomma, 'б' },
        { Keys.OemPeriod, 'ю' }, { Keys.OemOpenBrackets, 'х' }, { Keys.OemCloseBrackets, 'ї' },
            {Keys.OemQuestion, '.'}, {Keys.Space, ' '}
    };
        private readonly Dictionary<Keys, char> _ukUpperDictionary = new()
            {
            { Keys.A, 'Ф' }, { Keys.B, 'И' }, { Keys.C, 'С' }, { Keys.D, 'В' }, { Keys.E, 'У' },
            { Keys.F, 'А' }, { Keys.G, 'П' }, { Keys.H, 'Р' }, { Keys.I, 'Ш' }, { Keys.J, 'О' },
            { Keys.K, 'Л' }, { Keys.L, 'Д' }, { Keys.M, 'Ь' }, { Keys.N, 'Т' }, { Keys.O, 'Щ' },
            { Keys.P, 'З' }, { Keys.Q, 'Й' }, { Keys.R, 'К' }, { Keys.S, 'І' }, { Keys.T, 'Е' },
            { Keys.U, 'Г' }, { Keys.V, 'М' }, { Keys.W, 'Ц' }, { Keys.X, 'Ч' }, { Keys.Y, 'Н' },
            { Keys.Z, 'Я' }, { Keys.OemSemicolon, 'Ж' }, { Keys.OemQuotes, 'Є' }, { Keys.Oemcomma, 'Б' },
            { Keys.OemPeriod, 'Ю' }, { Keys.OemOpenBrackets, 'Х' }, { Keys.OemCloseBrackets, 'Ї' },
                {Keys.OemQuestion, ','}, {Keys.Space, ' '} };

        private readonly Dictionary<Keys, char> _digitDictionary = new()
            {
            {Keys.D0, '0'}, {Keys.D1, '1'}, {Keys.D2, '2'}, {Keys.D3, '3'}, {Keys.D4, '4'},
            {Keys.D5, '5'}, {Keys.D6, '6'}, {Keys.D7, '7'}, {Keys.D8, '8'}, {Keys.D9, '9'},
            {Keys.OemMinus, '-'}, {Keys.Oemplus, '+'}, {Keys.Oemtilde, '`'}, {Keys.OemPipe, '|'},
            {Keys.OemBackslash, '\\'}, {Keys.Multiply, '*'}, {Keys.Divide, '/'},
            };


        public char GetChar(Keys keyPressed, string lang, CaseFlag caseFlag)
        {
            if (_digitDictionary.ContainsKey(keyPressed))
            {
                return _digitDictionary[keyPressed];
            }
            if (caseFlag == CaseFlag.LowerCase)
            {
                if (lang == "ru-RU")
                {
                    return _ruLowerDictionary[keyPressed];
                }
                else if (lang == "en-US")
                {
                    return _enLowerDictionary[keyPressed];
                }
                else
                {
                    return _ukLowerDictionary[keyPressed];
                }
            }
            else
            {
                if (lang == "ru-RU")
                {
                    return _ruUpperDictionary[keyPressed];
                }
                else if (lang == "en-US")
                {
                    return _enUpperDictionary[keyPressed];
                }
                else
                {
                    return _ukUpperDictionary[keyPressed];
                }
            }

        }

        public string Map(List<Letter> letters)
        {
            StringBuilder result = new();

            if (letters.Count > 0)
            {

                foreach (Letter l in letters)
                {
                    if (_digitDictionary.ContainsKey(l.Key))
                    {
                        char c = GetChar(l.Key, l.Lang, l.CaseFlag);
                        result.Append(c);
                        continue;
                    }
                    if (l.Key == Keys.Space)
                    {
                        result.Append(' ');
                        continue;
                    }
                    if (l.Lang == "en-US")
                    {
                        char c = GetChar(l.Key, l.Lang, l.CaseFlag);
                        char ch = l.CaseFlag == CaseFlag.UpperCase ? _enToRuUpper[c] : _enToRuLower[c];
                        result.Append(ch);
                    }
                    else if (l.Lang == "ru-RU")
                    {
                        char c = GetChar(l.Key, l.Lang, l.CaseFlag);
                        char ch = l.CaseFlag == CaseFlag.UpperCase ? _ruToEnUpper[c] : _ruToEnLower[c];
                        result.Append(ch);
                    }
                    else if (l.Lang == "uk-UA")
                    {
                        char c = GetChar(l.Key, l.Lang, l.CaseFlag);
                        char ch = l.CaseFlag == CaseFlag.UpperCase ? _ukToRuUpper[c] : _ukToRuLower[c];
                        result.Append(ch);
                    }
                }

            }
            return result.ToString();
        }
    }
}