namespace GenerateAst
{
    public class FieldContainer
    {
        public string FieldType { get; }
        public string MemberName { get; }
        public string ParamName { get; }

        public FieldContainer(string fieldType, string memberName, string paramName)
        {
            FieldType = fieldType;
            MemberName = memberName;
            ParamName = paramName;
        }

        public static FieldContainer Parse(string source)
        {
            string[] splits = source.Split('|');
            string memberName = splits[1].Trim();
            string[] splits2 = splits[0].Split(' ');
            List<string> filteredSplits = new();

            foreach(string s in splits2)
            {
                if (s != string.Empty)
                {
                    filteredSplits.Add(s);
                }
            }

            string fieldType = filteredSplits[0].Trim();
            string paramName = filteredSplits[1].Trim();
            return new FieldContainer(fieldType, memberName, paramName);
        }
    }
}
