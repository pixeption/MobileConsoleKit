using System.Text;

namespace MobileConsole
{
    public static class StringExtension
    {
        public static string VarName(this string self)
        {
            if (string.IsNullOrEmpty(self))
                return null;

            return System.Char.ToLowerInvariant(self[0]) + self.Substring(1);
        }

        public static string GetReadableName(this string name)
        {
            // Handle special cases
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            else if (name.Length == 1)
            {
                return name.ToUpper();
            }
            else if (name.Length == 2 && name == "m_")
            {
                return "M";
            }

            // Calculate start index
            int startIndex = 0;
            if (name.StartsWith("m_")) // m_variable or m_Variable
            {
                startIndex = 2;
            }
            else if (name[0] == '_') // _variable or _Variable
            {
                startIndex = 1;
            }
            else if ((name[0] == 'm' || name[0] == 'k') && name[1].IsUpper()) // mVariable or kVariable
            {
                startIndex = 1;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(name[startIndex].ToUpper());
            for (int i = startIndex + 1; i < name.Length; i++)
            {
                if (name[i] == '_')
                {
                    sb.Append(" ");
                    continue;
                }

                sb.Append(name[i]);
                    
                if (i < name.Length - 1)
                {
                    if ((name[i].IsLower() || name[i].IsNumber()) && name[i + 1].IsUpper())
                    {
                        sb.Append(" ");
                    }
                    else if (!name[i].IsNumber() && name[i + 1].IsNumber())
                    {
                        sb.Append(" ");
                    }
                }
            }

            return sb.ToString();
        }

        public static bool IsUpper(this char c)
        {
            return char.IsUpper(c);
        }

        public static bool IsLower(this char c)
        {
            return char.IsLower(c);
        }

        public static bool IsNumber(this char c)
        {
            return char.IsNumber(c);
        }

        public static char ToUpper(this char c)
        {
            return char.ToUpper(c);
        }
    }
}