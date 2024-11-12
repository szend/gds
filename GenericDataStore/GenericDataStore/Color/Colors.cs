namespace GenericDataStore.Color
{
    public  class Colors
    {
        public List<int> UsedIndex { get; set; } = new List<int>();
        public List<List<string>> ColorList { get; set; } = new List<List<string>>()
        {
            new List<string> { "#950404FF", "#E04B28FF", "#C38961FF", "#9F5630FF", "#388F30FF", "#0F542FFF", "#007D82FF", "#004042FF" },
            
            new List<string> { "#FED789FF", "#023743FF", "#72874EFF", "#476F84FF", "#A4BED5FF", "#453947FF" },

            new List<string> { "#527E87FF", "#B88244FF", "#B8B69EFF", "#B48F2CFF", "#A37903FF" },

            new List<string> { "#0F1926FF", "#025940FF", "#02734AFF", "#038C33FF", "#03A62CFF" },

            new List<string> { "#491212FF", "#F27127FF", "#F24C27FF", "#BF281BFF", "#CE471CFF" },

            new List<string> { "#00010DFF", "#032CA6FF", "#034AA6FF", "#035AA6FF", "#93ABBFFF" },

            new List<string> { "#0476D9FF", "#01260EFF", "#618C03FF", "#BFB304FF", "#A68B03FF" },

            new List<string> { "#FF9933FF", "#C24841FF", "#FFFF33FF", "#8B5B45FF", "#87AFD1FF", "#EEB05AFF", "#DBC5A0FF" },
            
            new List<string> { "#693829FF", "#894B33FF", "#A56A3EFF", "#CFB267FF", "#D9C5B6FF", "#9CA9BAFF", "#5480B5FF", "#3D619DFF", "#405A95FF", "#345084FF" },
            
            new List<string> { "#240E31FF", "#CB6BCEFF", "#468892FF", "#74F3D3FF", "#751C6DFF", "#FDC067FF", "#AC9ECEFF", "#6EC5ABFF" },
            
            new List<string> { "#EBCF2EFF", "#B4BF3AFF", "#88AB38FF", "#5E9432FF", "#3B7D31FF", "#225F2FFF", "#244422FF", "#252916FF" },
            
            new List<string> { "#855C75FF", "#D9AF6BFF", "#AF6458FF", "#736F4CFF", "#526A83FF", "#625377FF", "#68855CFF", "#9C9C5EFF", "#A06177FF", "#8C785DFF", "#467378FF", "#7C7C7CFF" },
            
            new List<string> { "#774762FF", "#BA6E1DFF", "#D6BB3BFF", "#755028FF","#F2DD78FF", "#205F4BFF", "#913914FF", "#585854FF", "#F0A430FF", "#768048FF", "#800000FF", "#1B3A54FF" },
            
            new List<string> { "#F24D98FF", "#813B7CFF", "#59D044FF", "#F3A002FF", "#F2F44DFF" },

            new List<string> { "#93AACAFF", "#F6B8BDFF", "#24DABEFF", "#C95657FF", "#89699CFF", "#71F8D3FF", "#BF3847FF", "#19090FFF" },
            
            new List<string> { "#F7FEAEFF", "#B7E6A5FF", "#7CCBA2FF", "#46AEA0FF", "#089099FF", "#00718BFF", "#045275FF" },
           
            new List<string> { "#1E8E99FF", "#51C3CCFF", "#99F9FFFF", "#B2FCFFFF", "#CCFEFFFF", "#E5FFFFFF", "#FFE5CCFF", "#FFCA99FF", "#FFAD65FF", "#FF8E32FF", "#CC5800FF", "#993F00FF" },
            
            new List<string> { "#290AD8FF", "#264DFFFF", "#3FA0FFFF", "#72D9FFFF", "#AAF7FFFF", "#E0FFFFFF", "#FFFFBFFF", "#FFE099FF", "#FFAD72FF", "#F76D5EFF", "#D82632FF", "#A50021FF" },
            
            new List<string> { "#0000FFFF", "#3333FFFF", "#6565FFFF", "#9999FFFF", "#B2B2FFFF", "#CBCBFFFF", "#E5E5FFFF", "#E5FFE5FF", "#CBFFCBFF", "#B2FFB2FF", "#99FF99FF", "#65FF65FF", "#33FF33FF", "#00FF00FF" },
            
            new List<string> { "#EA3DF6FF", "#DD2F24FF", "#D6822FFF", "#F9D449FF", "#DCDBEAFF" },

           new List<string> { "#E13A3EFF", "#C4CED4FF", "#000000FF" },

           new List<string> { "#C8102EFF", "#010101FF", "#373A36FF" },

           new List<string> { "#007A33FF", "#010101FF", "#BA0C2FFF" },

           new List<string> { "#990099FF", "#009900FF" },

           new List<string> { "#418FDEFF", "#003DA5FF", "#D50032FF" },

           new List<string> { "#D40F7DFF", "#E35205FF", "#000000FF" },

           new List<string> { "#41B6E6FF", "#DB3EB1FF", "#000000FF" },

           new List<string> { "#004488FF", "#DDAA33FF", "#BB5566FF" },

           new List<string> { "#003DA5FF", "#010101FF" },

           new List<string> { "#707372FF", "#010101FF" },

           new List<string> { "#012169FF", "#C8102EFF" },

           new List<string> { "#088158FF", "#BA2F2AFF" },

           new List<string> { "#b30000" },

           new List<string> { "#7c1158"},

           new List<string> { "#4421af" },

           new List<string> { "#1a53ff" },

           new List<string> { "#2e2b28" },

           new List<string> { "#6b506b" },

           new List<string> { "#ffb400" },

           new List<string> { "#a57c1b"},

           new List<string> { "#363445" },

           new List<string> { "#5e569b" },

           new List<string> { "#54bebe" },

           new List<string> { "#c80064" },

           new List<string> { "#e27c7c" },

           new List<string> { "#6cd4c5"},

           new List<string> { "#d0f400" },

           new List<string> { "#76c68f" },

           new List<string> { "#bd7ebe" },

           new List<string> { "#dc0ab4" },

           new List<string> { "#ffa300" },

           new List<string> { "#9b19f5"},

           new List<string> { "#e6d800" },

           new List<string> { "#50e991" },

           new List<string> { "#0bb4ff" },

           new List<string> { "#e60049" },


        };

        public string GetRandomColor()
        {
            Random random = new Random();
            int randomColorListIndex = random.Next(0, ColorList.Count);
            List<string> colorList = ColorList[randomColorListIndex];
            int randomColorIndex = random.Next(0, colorList.Count);
            return colorList[randomColorIndex];
        }

        public List<string> GetColorList(int size)
        {
            if(size > 14)
            {
                List<string> random = new List<string>();
                while (random.Count < size)
                {
                    random.AddRange(GetRandomList());
                }
                return random;
            }

            
            var list = ColorList.Where(x => x.Count == size && !UsedIndex.Contains(ColorList.IndexOf(x))).ToList();
           
            if(list.Count == 0)
            {
                var big = GetColorList(size + 1);
                if (big.Count > 0)
                {
                    return big;
                }
                else
                {
                    while(big.Count < size)
                    {
                        big.AddRange(GetRandomList());
                    }
                    return big;
                }
            }

            Random random1 = new Random();

            return list[random1.Next(0,list.Count)];
        }

        public List<string> GetRandomList()
        {
            Random random = new Random();
            int randomColorListIndex = random.Next(0, ColorList.Count);
            List<string> colorList = ColorList[randomColorListIndex];
            return colorList;
        }


        public string GetRandomColor(int colorListIndex)
        {
            Random random = new Random();
            List<string> colorList = ColorList[colorListIndex];
            int randomColorIndex = random.Next(0, colorList.Count);
            return colorList[randomColorIndex];
        }

        public string GetRandomColor(int colorListIndex, int colorIndex)
        {
            List<string> colorList = ColorList[colorListIndex];
            return colorList[colorIndex];
        }

        public string GetRandomColor(int colorListIndex, int colorIndex, int alpha)
        {
            List<string> colorList = ColorList[colorListIndex];
            string color = colorList[colorIndex];
            return color.Substring(0, color.Length - 2) + alpha.ToString("X2");
        }
    }
}
