using System.Text;

namespace MCSM.Core.Utils
{
    public class IniObject
    {

        private Dictionary<string, Dictionary<string, string>> ini = null;

        public IniObject()
        {
            ini = new Dictionary<string, Dictionary<string, string>>();
        }

        public IniObject(string iniString)
        {
            ini = ParseIni(iniString);
        }

        private Dictionary<string, Dictionary<string, string>> ParseIni(string iniString)
        {
            Dictionary<string, Dictionary<string, string>> entries =
                new Dictionary<string, Dictionary<string, string>>();

            Dictionary<string, string> currentSection = new Dictionary<string, string>();

            string[] lines = iniString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (!string.IsNullOrEmpty(line) && !line.StartsWith(";"))
                {
                    if (line.StartsWith('\u005b') && line.EndsWith('\u005d'))
                    {
                        string sectionName = line.Substring(1, line.Length - 2);

                        currentSection = new Dictionary<string, string>();
                        entries.Add(sectionName, currentSection);
                    }
                    else
                    {
                        string[] keyValue = line.Split('=');

                        if (keyValue.Length == 2)
                        {
                            currentSection.Add(keyValue[0].Trim(), keyValue[1].Trim().Replace("\"", ""));
                        }
                    }
                }
            }

            return entries;
        }

        public string this[string section, string key]
        {
            get
            {
                if (ini.ContainsKey(section))
                {
                    if (ini[section].TryGetValue(key, out string value))
                    {
                        return value;
                    }
                }

                return null;
            }

            set
            {
                Dictionary<string, string> element;
                if (ini.ContainsKey(section)) element = ini[section];
                else element = new();

                element[key] = value;
                ini[section] = element;
            }
        }

        public Dictionary<string, string> this[string section]
        {
            get
            {
                if (ini.ContainsKey(section))
                {
                    return ini[section];
                }

                return null;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            foreach (KeyValuePair<string, Dictionary<string, string>> kv in ini)
            {
                sb.AppendLine(string.Format("[{0}]", kv.Key));

                foreach (KeyValuePair<string, string> elementkv in kv.Value)
                {
                    sb.AppendLine(string.Format("{0}=\"{1}\"", elementkv.Key, elementkv.Value));
                }
            }

            return sb.ToString();
        }
    }
}