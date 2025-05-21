using System.Collections.Generic;
using System;
using System.Data;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Lab4
{
    public class ExpressionParser
    {
        private Dictionary<string, double> variables = new Dictionary<string, double>();

        public double Evaluate(string expression)
        {
            // Обработка присваиваний переменных (a=5)
            var assignmentMatch = Regex.Match(expression, @"^\s*([a-z])\s*=\s*(.+)$", RegexOptions.IgnoreCase);
            if (assignmentMatch.Success)
            {
                string varName = assignmentMatch.Groups[1].Value.ToLower();
                double value = Evaluate(assignmentMatch.Groups[2].Value);
                variables[varName] = value;
                return value;
            }

            // Подстановка значений переменных
            expression = Regex.Replace(expression, @"([a-z])", m =>
                variables.TryGetValue(m.Value.ToLower(), out double val) ? val.ToString() : m.Value);

            // Замена констант и функций
            expression = expression.Replace("pi", Math.PI.ToString())
                                 .Replace("e", Math.E.ToString())
                                 .ToLower();

            // Обработка функций (sin, cos, tan, log, exp, sqrt)
            expression = Regex.Replace(expression, @"(sin|cos|tan|log|exp|sqrt)\(([^)]+)\)",
                m => MathFunction(m.Groups[1].Value, m.Groups[2].Value));

            // Вычисление выражения
            return Convert.ToDouble(new DataTable().Compute(expression, null));
        }

        private string MathFunction(string func, string arg)
        {
            double value = Evaluate(arg);
            switch (func)
            {
                case "sin": return Math.Sin(value).ToString();
                case "cos": return Math.Cos(value).ToString();
                case "tan": return Math.Tan(value).ToString();
                case "log": return Math.Log10(value).ToString();
                case "exp": return Math.Exp(value).ToString();
                case "sqrt": return Math.Sqrt(value).ToString();
                default: return value.ToString();
            }
        }
    }
}
