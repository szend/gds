using GenericDataStore.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace GenericDataStore.Services
{
    public class TextToCalculation
    {
        public List<string> ChartKeys = new List<string>()
        {
            "#chart",
            "chart",
            "plot",
            "diagram",
            "graph"
        };

        public List<string> FieldKeys = new List<string>
        {
            "#field",
            "field",
            "column",
            "row",
            "cell",
        };

        public List<string> ColorKeys = new List<string>
        {
            "#color",
            "color",
            "colour",
            "colored",
            "coloured",
            "background",
            "backgroundcolor"
        };


        public int CalculateLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Inicializálás  
            for (int i = 0; i <= n; i++)
                d[i, 0] = i;
            for (int j = 0; j <= m; j++)
                d[0, j] = j;

            // Elementorozza a táblázatot  
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

        public void TranslateToKeys(string text, ObjectType obj)
        {
            List<string> words = text.Split(' ').ToList();
            List<string> keywords = new List<string>();
            List<Field> mentionedfields = new List<Field>();
            foreach (var item in words)
            {
                foreach (string keyword in Keywords)
                {
                    if (CalculateLevenshteinDistance(item.ToLower(), keyword) < 2)
                    {
                        keywords.Add(keyword);
                    }
                }

                foreach (var field in obj.Field)
                {
                    if (CalculateLevenshteinDistance(item.ToLower(), field.Name.ToLower()) < 2)
                    {
                        mentionedfields.Add(field);
                    }
                }
            }

        }

        //public List<Value> GetValueKeywords(List<string> words, List<Field> mentionedfields, ObjectType obj)
        //{
        //    List<string> valuewords = new List<string>();
        //    List<bool>
        //    foreach (var word in words)
        //    {
        //        bool result;
        //        if(bool.TryParse(word, out result) == true)
        //        {
                    
        //        }
        //        foreach (var item2 in mentionedfields)
        //        {
        //            foreach (var item in obj.DataObject)
        //            {
        //                var value = item.Value.FirstOrDefault(x => x.Name == item2.Name);

        //                    if (CalculateLevenshteinDistance(word.ToLower(), value.ValueString.ToLower()) < 2)
        //                    {
        //                        valuewords.Add(value.ValueString);
        //                    }
                        
        //            }
        //        }
        //    }

        //    return valuewords;
        //}

        public string GetDescFromKeys(List<string> keys)
        {
            if (keys.Contains("chart") || keys.Contains("plot") || keys.Contains("diagram") || keys.Contains("graph"))
            {

            }
            else if (keys.Contains("field") || keys.Contains("column") || keys.Contains("row") || keys.Contains("cell"))
            {

            }
            else if ((keys.Contains("background") || keys.Contains("backgroundcolor") || keys.Contains("color") || keys.Contains("colour") || keys.Contains("colored") || keys.Contains("coloured"))
            {

            }

        }

        public string CartCalculation(List<string> keys, List<Field> mentionedfields)
        {
            if(keys.Contains("x") && keys.Contains("y"))
            {
                if(keys.IndexOf("x") < keys.IndexOf("y"))
                {

                }
                
            }
        }
    }
}
