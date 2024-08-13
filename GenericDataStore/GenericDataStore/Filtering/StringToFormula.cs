using System.Text;
using Tensorflow;

namespace GenericDataStore.Filtering
{
    public class StringToValue
    {
        private string[] _operators = { "-", "+", "/", "*", "^", "!", "|", "&", "=", "!=", "<", ">", "<=", ">=", "->", "<-" ,":-" , ":+"};
        private int[] _operatorsvalue = { 0, 0, 1, 1, 2,2,2,2,2,2,2,2,2,2,2,2,2,2 };
        private Func<object?, object, object>[] _operations = {
        (a1, a2) => double.Parse(a1.ToString().Replace('.', ',')) - double.Parse(a2.ToString().Replace('.', ',')),
        (a1, a2) => double.Parse(a1.ToString().Replace('.', ',')) + double.Parse(a2.ToString().Replace('.', ',')),
        (a1, a2) => double.Parse(a1.ToString().Replace('.', ',')) / double.Parse(a2.ToString().Replace('.', ',')),
        (a1, a2) => double.Parse(a1.ToString().Replace('.', ',')) * double.Parse(a2.ToString().Replace('.', ',')),
        (a1, a2) => Math.Pow( double.Parse(a1.ToString().Replace('.', ',')),  double.Parse(a2.ToString().Replace('.', ','))),
        (a1, a2) =>  ! bool.Parse(a2.ToString()),
        (a1, a2) => bool.Parse(a1.ToString()) ||  bool.Parse(a2.ToString()),
        (a1, a2) => bool.Parse(a1.ToString()) && bool.Parse(a2.ToString()),
        (a1, a2) =>  a1.ToString() ==  a2.ToString(),
        (a1, a2) => a1.ToString() !=  a2.ToString(),
         (a1, a2) => double.Parse(a1.ToString().Replace('.', ',')) < double.Parse(a2.ToString().Replace('.', ',')),
         (a1, a2) => double.Parse(a1.ToString().Replace('.', ',')) > double.Parse(a2.ToString().Replace('.', ',')),
         (a1, a2) => double.Parse(a1.ToString().Replace('.', ',')) <= double.Parse(a2.ToString().Replace('.', ',')),
         (a1, a2) => double.Parse(a1.ToString().Replace('.', ',')) >= double.Parse(a2.ToString().Replace('.', ',')),
         (a1, a2) => a1.ToString().Contains(a2.ToString()),
         (a1, a2) => a2.ToString().Contains(a1.ToString()),
         (a1, a2) => a1.ToString().Replace(a2.ToString(),""),
         (a1, a2) => a1.ToString() + a2.ToString(),
    };

        public object Eval(string expression)
        {
            List<string> tokens = getTokens(expression);
            Stack<object> operandStack = new Stack<object>();
            Stack<string> operatorStack = new Stack<string>();
            int tokenIndex = 0;

            while (tokenIndex < tokens.Count)
            {
                string token = tokens[tokenIndex];
                if (token == "(")
                {
                    string subExpr = getSubExpression(tokens, ref tokenIndex);
                    operandStack.Push(Eval(subExpr));
                    continue;
                }
                if (token == ")")
                {
                    throw new ArgumentException("Mis-matched parentheses in expression");
                }
                //If this is an operator  
                if (Array.IndexOf(_operators, token) >= 0)
                {
                    while (operatorStack.Count > 0 && _operatorsvalue[Array.IndexOf(_operators, token)] <= _operatorsvalue[Array.IndexOf(_operators, operatorStack.Peek())])
                    {
                        string op = operatorStack.Pop();
                        object arg2 = operandStack.Pop();
                        object arg1 = operandStack.Pop();
                        operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                    }
                    operatorStack.Push(token);
                }
                else
                {
                    operandStack.Push(token);
                }
                tokenIndex += 1;
            }

            while (operatorStack.Count > 0)
            {
                string op = operatorStack.Pop();
                object arg2 = operandStack.Pop();
                if (operandStack.Count == 0)
                {
                    if(op == "!")
                    {
                        operandStack.Push(_operations[Array.IndexOf(_operators, op)](null, arg2));
                    }
                    else if(op == "-" || op == "+")
                    {
                        operandStack.Push(_operations[Array.IndexOf(_operators, op)](0, arg2));
                    }
                    else
                    {
                        operandStack.Push(_operations[Array.IndexOf(_operators, op)]("", arg2));
                    }
                }
                else
                {
                    object arg1 = operandStack.Pop();
                    operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                }

            }
            return operandStack.Pop();
        }

        private string getSubExpression(List<string> tokens, ref int index)
        {
            StringBuilder subExpr = new StringBuilder();
            int parenlevels = 1;
            index += 1;
            while (index < tokens.Count && parenlevels > 0)
            {
                string token = tokens[index];
                if (tokens[index] == "(")
                {
                    parenlevels += 1;
                }

                if (tokens[index] == ")")
                {
                    parenlevels -= 1;
                }

                if (parenlevels > 0)
                {
                    subExpr.Append(token + " ");
                }

                index += 1;
            }

            if ((parenlevels > 0))
            {
                throw new ArgumentException("Mis-matched parentheses in expression");
            }
            return subExpr.ToString();
        }

        private List<string> getTokens(string expression)
        {
            List<string> operators = new List<string> { "(", ")", "^", "*", "/", "+", "-", "&", "|", "!", "!=", "=", "<", ">", "<=", ">=", "->", "<-", ":-", ":+" };
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (string c in expression.Split(" "))
            {
                if (operators.IndexOf(c) >= 0)
                {
                    if ((sb.Length > 0))
                    {
                        tokens.Add(sb.ToString());
                        sb.Length = 0;
                    }
                    tokens.Add(c.ToString());
                }
                else
                {
                    sb.Append(c);
                }
            }

            if ((sb.Length > 0))
            {
                tokens.Add(sb.ToString());
            }
            return tokens;
        }
    }
    public class StringToFormula
    {
        private string[] _operators = { "-", "+", "/", "*", "^" };
        private int[] _operatorsvalue = { 0, 0, 1, 1, 2 };
        private Func<double, double, double>[] _operations = {
        (a1, a2) => a1 - a2,
        (a1, a2) => a1 + a2,
        (a1, a2) => a1 / a2,
        (a1, a2) => a1 * a2,
        (a1, a2) => Math.Pow(a1, a2)
    };

        public double Eval(string expression)
        {
            List<string> tokens = getTokens(expression);
            Stack<double> operandStack = new Stack<double>();
            Stack<string> operatorStack = new Stack<string>();
            int tokenIndex = 0;

            while (tokenIndex < tokens.Count)
            {
                string token = tokens[tokenIndex];
                if (token == "(")
                {
                    string subExpr = getSubExpression(tokens, ref tokenIndex);
                    operandStack.Push(Eval(subExpr));
                    continue;
                }
                if (token == ")")
                {
                    throw new ArgumentException("Mis-matched parentheses in expression");
                }
                //If this is an operator  
                if (Array.IndexOf(_operators, token) >= 0)
                {
                    while (operatorStack.Count > 0 && _operatorsvalue[Array.IndexOf(_operators, token)] <= _operatorsvalue[Array.IndexOf(_operators, operatorStack.Peek())])
                    {
                        string op = operatorStack.Pop();
                        double arg2 = operandStack.Pop();
                        double arg1 = operandStack.Pop();
                        operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                    }
                    operatorStack.Push(token);
                }
                else
                {
                    operandStack.Push(double.Parse(token));
                }
                tokenIndex += 1;
            }

            while (operatorStack.Count > 0)
            {
                string op = operatorStack.Pop();
                double arg2 = operandStack.Pop();
                if(operandStack.Count == 0)
                {
                    operandStack.Push(_operations[Array.IndexOf(_operators, op)](0, arg2));
                }
                else {
                    double arg1 = operandStack.Pop();
                    operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                }

            }
            return operandStack.Pop();
        }

        private string getSubExpression(List<string> tokens, ref int index)
        {
            StringBuilder subExpr = new StringBuilder();
            int parenlevels = 1;
            index += 1;
            while (index < tokens.Count && parenlevels > 0)
            {
                string token = tokens[index];
                if (tokens[index] == "(")
                {
                    parenlevels += 1;
                }

                if (tokens[index] == ")")
                {
                    parenlevels -= 1;
                }

                if (parenlevels > 0)
                {
                    subExpr.Append(token);
                }

                index += 1;
            }

            if ((parenlevels > 0))
            {
                throw new ArgumentException("Mis-matched parentheses in expression");
            }
            return subExpr.ToString();
        }

        private List<string> getTokens(string expression)
        {
            string operators = "()^*/+-";
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (char c in expression.Replace(" ", string.Empty))
            {
                if (operators.IndexOf(c) >= 0)
                {
                    if ((sb.Length > 0))
                    {
                        tokens.Add(sb.ToString());
                        sb.Length = 0;
                    }
                    tokens.Add(c.ToString());
                }
                else
                {
                    sb.Append(c);
                }
            }

            if ((sb.Length > 0))
            {
                tokens.Add(sb.ToString());
            }
            return tokens;
        }
    }

    public class StringToString
    {
        private string[] _operators = { "-", "+" };
        private int[] _operatorsvalue = { 0, 0 };
        private Func<string, string, string>[] _operations = {
        (a1, a2) => a1.Replace(a2,""),
        (a1, a2) => a1 + a2,

    };

        public string Eval(string expression)
        {
            List<string> tokens = getTokens(expression);
            Stack<string> operandStack = new Stack<string>();
            Stack<string> operatorStack = new Stack<string>();
            int tokenIndex = 0;

            while (tokenIndex < tokens.Count)
            {
                string token = tokens[tokenIndex];
                if (token == "(")
                {
                    string subExpr = getSubExpression(tokens, ref tokenIndex);
                    operandStack.Push(Eval(subExpr));
                    continue;
                }
                if (token == ")")
                {
                    throw new ArgumentException("Mis-matched parentheses in expression");
                }
                //If this is an operator  
                if (Array.IndexOf(_operators, token) >= 0)
                {
                    while (operatorStack.Count > 0 && _operatorsvalue[Array.IndexOf(_operators, token)] <= _operatorsvalue[Array.IndexOf(_operators, operatorStack.Peek())])
                    {
                        string op = operatorStack.Pop();
                        string arg2 = operandStack.Pop();
                        string arg1 = operandStack.Pop();
                        operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                    }
                    operatorStack.Push(token);
                }
                else
                {
                    operandStack.Push(token);
                }
                tokenIndex += 1;
            }

            while (operatorStack.Count > 0)
            {
                string op = operatorStack.Pop();
                string arg2 = operandStack.Pop();
                if (operandStack.Count == 0)
                {
                    operandStack.Push(_operations[Array.IndexOf(_operators, op)]("", arg2));
                }
                else
                {
                    string arg1 = operandStack.Pop();
                    operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                }

            }
            return operandStack.Pop();
        }

        private string getSubExpression(List<string> tokens, ref int index)
        {
            StringBuilder subExpr = new StringBuilder();
            int parenlevels = 1;
            index += 1;
            while (index < tokens.Count && parenlevels > 0)
            {
                string token = tokens[index];
                if (tokens[index] == "(")
                {
                    parenlevels += 1;
                }

                if (tokens[index] == ")")
                {
                    parenlevels -= 1;
                }

                if (parenlevels > 0)
                {
                    subExpr.Append(token);
                }

                index += 1;
            }

            if ((parenlevels > 0))
            {
                throw new ArgumentException("Mis-matched parentheses in expression");
            }
            return subExpr.ToString();
        }

        private List<string> getTokens(string expression)
        {
            string operators = "*-";
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (char c in expression.Replace(" ", string.Empty))
            {
                if (operators.IndexOf(c) >= 0)
                {
                    if ((sb.Length > 0))
                    {
                        tokens.Add(sb.ToString());
                        sb.Length = 0;
                    }
                    tokens.Add(c.ToString());
                }
                else
                {
                    sb.Append(c);
                }
            }

            if ((sb.Length > 0))
            {
                tokens.Add(sb.ToString());
            }
            return tokens;
        }
    }

    public class StringToBool
    {
        private string[] _operators = { "!", "|", "&", "=", "!=", "<", ">" , "<=", ">=", "->" , "<-"};
        private int[] _operatorsvalue = { 0, 1, 1,1,1,1,1,1,1,1,1 };
        private Func<object?, object, bool>[] _operations = {
        (a1, a2) =>  ! bool.Parse(a2.ToString()),
        (a1, a2) => bool.Parse(a1.ToString()) ||  bool.Parse(a2.ToString()),
        (a1, a2) => bool.Parse(a1.ToString()) && bool.Parse(a2.ToString()),
        (a1, a2) =>  a1.ToString() ==  a2.ToString(),
         (a1, a2) => a1.ToString() !=  a2.ToString(),

         (a1, a2) => double.Parse(a1.ToString()) < double.Parse(a2.ToString()),
         (a1, a2) => double.Parse(a1.ToString()) > double.Parse(a2.ToString()),
         (a1, a2) => double.Parse(a1.ToString()) <= double.Parse(a2.ToString()),
         (a1, a2) => double.Parse(a1.ToString()) >= double.Parse(a2.ToString()),

         (a1, a2) => a1.ToString().Contains(a2.ToString()),
         (a1, a2) => a2.ToString().Contains(a1.ToString()),
        };

        public bool Eval(string expression)
        {
            List<string> tokens = getTokens(expression);
            Stack<object> operandStack = new Stack<object>();
            Stack<string> operatorStack = new Stack<string>();
            int tokenIndex = 0;

            while (tokenIndex < tokens.Count)
            {
                string token = tokens[tokenIndex];
                if (token == "(")
                {
                    string subExpr = getSubExpression(tokens, ref tokenIndex);
                    operandStack.Push(Eval(subExpr));
                    continue;
                }
                if (token == ")")
                {
                    throw new ArgumentException("Mis-matched parentheses in expression");
                }
                //If this is an operator  
                if (Array.IndexOf(_operators, token) >= 0)
                {
                    while (operatorStack.Count > 0 && _operatorsvalue[Array.IndexOf(_operators, token)] <= _operatorsvalue[Array.IndexOf(_operators, operatorStack.Peek())])
                    {
                        string op = operatorStack.Pop();
                        object arg2 = operandStack.Pop();
                        object arg1 = operandStack.Pop();
                        operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                    }
                    operatorStack.Push(token);
                }
                else
                {
                    operandStack.Push(token);
                }
                tokenIndex += 1;
            }

            while (operatorStack.Count > 0)
            {
                string op = operatorStack.Pop();
                object arg2 = operandStack.Pop();
                if (operandStack.Count == 0)
                {
                    operandStack.Push(_operations[Array.IndexOf(_operators, op)](null, arg2));
                }
                else
                {
                    object arg1 = operandStack.Pop();
                    operandStack.Push(_operations[Array.IndexOf(_operators, op)](arg1, arg2));
                }

            }
            return bool.Parse(operandStack.Pop().ToString());
        }

        private string getSubExpression(List<string> tokens, ref int index)
        {
            StringBuilder subExpr = new StringBuilder();
            int parenlevels = 1;
            index += 1;
            while (index < tokens.Count && parenlevels > 0)
            {
                string token = tokens[index];
                if (tokens[index] == "(")
                {
                    parenlevels += 1;
                }

                if (tokens[index] == ")")
                {
                    parenlevels -= 1;
                }

                if (parenlevels > 0)
                {
                    subExpr.Append(token);
                }

                index += 1;
            }

            if ((parenlevels > 0))
            {
                throw new ArgumentException("Mis-matched parentheses in expression");
            }
            return subExpr.ToString();
        }

        private List<string> getTokens(string expression)
        {
            string operators = "()&|!=<>>=<=-><-";
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (char c in expression.Replace(" ", string.Empty))
            {
                if (operators.IndexOf(c) >= 0)
                {
                    if ((sb.Length > 0))
                    {
                        tokens.Add(sb.ToString());
                        sb.Length = 0;
                    }
                    tokens.Add(c.ToString());
                }
                else
                {
                    sb.Append(c);
                }
            }

            if ((sb.Length > 0))
            {
                tokens.Add(sb.ToString());
            }
            return tokens;
        }
    }


}
