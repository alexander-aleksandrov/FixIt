using System.Text;

namespace FixIt
{
    public enum Lang
    {
        EN,
        RU,
        UA
    }

    class Language
    {
        private InputLanguage _inputLanguage;

        private Dictionary<char, char> enToRu = new Dictionary<char, char>{
    {'q', 'й'},
    {'w', 'ц'},
    {'e', 'у'},
    {'r', 'к'},
    {'t', 'е'},
    {'y', 'н'},
    {'u', 'г'},
    {'i', 'ш'},
    {'o', 'щ'},
    {'p', 'з'},
    {'[', 'х'},
    {']', 'ъ'},
    {'a', 'ф'},
    {'s', 'ы'},
    {'d', 'в'},
    {'f', 'а'},
    {'g', 'п'},
    {'h', 'р'},
    {'j', 'о'},
    {'k', 'л'},
    {'l', 'д'},
    {';', 'ж'},
    {'\'', 'э'},
    {'z', 'я'},
    {'x', 'ч'},
    {'c', 'с'},
    {'v', 'м'},
    {'b', 'и'},
    {'n', 'т'},
    {'m', 'ь'},
    {',', 'б'},
    {'.', 'ю'}};

        private Dictionary<char, char> ruToEn = new Dictionary<char, char>{
    {'й', 'q'},
    {'ц', 'w'},
    {'у', 'e'},
    {'к', 'r'},
    {'е', 't'},
    {'н', 'y'},
    {'г', 'u'},
    {'ш', 'i'},
    {'щ', 'o'},
    {'з', 'p'},
    {'х', '['},
    {'ъ', ']'},
    {'ф', 'a'},
    {'ы', 's'},
    {'в', 'd'},
    {'а', 'f'},
    {'п', 'g'},
    {'р', 'h'},
    {'о', 'j'},
    {'л', 'k'},
    {'д', 'l'},
    {'ж', ';'},
    {'э', '\''},
    {'я', 'z'},
    {'ч', 'x'},
    {'с', 'c'},
    {'м', 'v'},
    {'и', 'b'},
    {'т', 'n'},
    {'ь', 'm'},
    {'б', ','},
    {'ю', '.'}
};

        public string Transcribe(string str)
        {
            byte[] format = GetFormat(str);
            str = str.ToLower();
            StringBuilder result = new StringBuilder();

            if (str != null)
            {

                foreach (char c in str)
                {
                    //определяяем относится ли символ к к латинице илил кирилице 
                    bool isLatin = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
                    bool isCyrillic = (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я');

                    if (isLatin)
                    {
                        result.Append(enToRu[c]);
                    }

                    else if (isCyrillic)
                    {
                        result.Append(ruToEn[c]);
                    }
                    else if (c == ' ')
                    {
                        result.Append(' ');
                    }
                }

            }
            return ApplyFormat(format, result.ToString());
        }




        private byte[] GetFormat(string str)
        {
            byte[] result = new byte[str.Length];
            StringBuilder sb = new(str);

            for (int i = 0; i < str.Length; i++)
            {

                if (Char.IsUpper(sb[i]))
                {
                    result[i] = 1;
                }
                else
                {
                    result[i] = 0;
                }
            }
            return result;
        }
        private string ApplyFormat(byte[] format, string str)
        {
            StringBuilder res = new StringBuilder();


            for (int i = 0; i < format.Length; i++)
            {
                if (format[i] == 0)
                {

                    res.Append(char.ToLower(str[i]));
                }
                else
                {
                    res.Append(char.ToUpper(str[i]));
                }
            }
            return res.ToString();
        }
    }
}
