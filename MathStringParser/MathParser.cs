using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MathStringParser
{
    public class MathParser
    {
        private const char RightParenthesis = ')';
        private const char LeftParenthesis = '(';
        private const string InvalidParenthesisMessage = "Input is not valid. Number of closing brackets must match number of opening brackets!";
        bool _calculationPerformed;
        private int _noCalculationPerformed;

        public double ParseEntire(string input)
        {
            double total = 0;

            var firstOperatorFound = false;
            var calcString = new StringBuilder();
            var substitutedString = SubstituteCharsForOperators(input);
            var openingParenthesis = new Dictionary<int, int>();
            var closingParenthesis = new Dictionary<int, int>();

            GrabParenthesis(substitutedString, openingParenthesis, closingParenthesis);
            var subCalcs = CalculateSubsteps(substitutedString, openingParenthesis, closingParenthesis);

            if (!InputIsValid(openingParenthesis, closingParenthesis))
            {
                Console.WriteLine(InvalidParenthesisMessage);
                Console.Read();
                return 0;
            }

            string newString;

            if (substitutedString.IndexOf(LeftParenthesis) > 0)
            {
                var firstParenthesis = substitutedString.IndexOf(LeftParenthesis);
                var lastParenthesis = substitutedString.LastIndexOf(RightParenthesis);
                newString = substitutedString.Remove(firstParenthesis,
                    lastParenthesis - firstParenthesis + 1);
                newString = newString.Insert(firstParenthesis, subCalcs);
            }
            else
            {
                newString = substitutedString;
            }

            var operators = CountOperators(newString);

            if (InputIsNumeric(input))
            {
                double returnValue;
                double.TryParse(input, out returnValue);
                return returnValue;
            }

            foreach (var c in newString.ToCharArray())
            {
                if (InputIsNumeric(c.ToString(CultureInfo.InvariantCulture)))
                {
                    calcString.Append(c);
                }
                else
                {
                    if (!InputIsOperator(c)) continue;

                    if (!firstOperatorFound)
                    {
                        calcString.Append(c);
                        firstOperatorFound = true;
                    }
                    else
                    {
                        if (!_calculationPerformed)
                        {
                            total += PerformCalculation(calcString.ToString());
                            StartNewCalcString(calcString, total, c);
                        }
                        else
                        {
                            total = PerformCalculation(calcString.ToString());
                            StartNewCalcString(calcString, total, c);
                        }
                    }
                }
            }

            if (!_calculationPerformed)
            {
                total = PerformCalculation(calcString.ToString());
            }
            if (_noCalculationPerformed < operators)
            {
                total = PerformCalculation(calcString.ToString());
            }
            return total;
        }

        private static void StartNewCalcString(StringBuilder calcString, double total, char c)
        {
            calcString.Clear();
            calcString.Append(total.ToString(CultureInfo.InvariantCulture));
            calcString.Append(c);
        }

        private static int CountOperators(string newString)
        {
            return newString.Count(c => c == '+' || c == '-' || c == '*' || c == '/');
        }

        private static void GetSubStep(string substitutedString,
            int startIndex, IDictionary<int, string> steps)
        {

            for (var index = startIndex; index >= substitutedString.IndexOf(LeftParenthesis); index--)
            {
                if (substitutedString[index] == LeftParenthesis || substitutedString[index] == RightParenthesis)
                {
                    steps.Add(index, substitutedString.Substring((index + 1), startIndex - index));
                    return;
                }
            }
        }

        private static void GrabParenthesis(string substitutedString,
            IDictionary<int, int> openingParenthesis,
            IDictionary<int, int> closingParenthesis)
        {
            for (var count = 0; count < substitutedString.Length; count++)
            {
                if (substitutedString[count] == LeftParenthesis)
                {
                    openingParenthesis.Add(count, count);
                }
                if (substitutedString[count] == RightParenthesis)
                {
                    closingParenthesis.Add(count, count);
                }
            }
        }

        private static string CalculateSubsteps(string substitutedString, Dictionary<int, int> openingParenthesis, Dictionary<int, int> closingParenthesis)
        {
            if (!closingParenthesis.Any()) return string.Empty;

            var theSteps = new Dictionary<int, string>();
            BuildSubSteps(substitutedString, openingParenthesis, theSteps);

            var strSub = DoSilentParseOfSubStep(theSteps.Last().Value).ToString(CultureInfo.InvariantCulture);

            if (theSteps.Count <= 1) return strSub;
            foreach (var step in theSteps.Reverse().Skip(1))
            {
                strSub = PerformCalculationSilent(strSub + step.Value).ToString(CultureInfo.InvariantCulture);
            }
            return strSub;
        }

        private static void BuildSubSteps(string substitutedString, Dictionary<int, int> openingParenthesis, Dictionary<int, string> theSteps)
        {
            for (var currIndex = substitutedString.Length - 1; currIndex >= openingParenthesis.ElementAt(0).Value; currIndex--)
            {
                if (substitutedString[currIndex] == RightParenthesis)
                {
                    GetSubStep(substitutedString, currIndex - 1, theSteps);
                }
            }
        }

        private static bool InputIsValid(IReadOnlyCollection<KeyValuePair<int, int>> openingParenthesis,
            IReadOnlyCollection<KeyValuePair<int, int>> closingParenthesis)
        {
            return openingParenthesis.Count == closingParenthesis.Count;
        }

        private static bool InputIsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == LeftParenthesis || c == RightParenthesis;
        }

        private static bool InputIsNumeric(string input)
        {
            double result;
            return double.TryParse(input, out result);
        }

        private static string SubstituteCharsForOperators(string input)
        {
            return input
                .Replace('a', '+')
                .Replace('b', '-')
                .Replace('c', '*')
                .Replace('d', '/')
                .Replace('e', LeftParenthesis)
                .Replace('f', RightParenthesis);
        }

        private static double PerformCalculationSilent(string calculation)
        {
            double returnVal;
            double.TryParse(new DataTable().Compute(calculation, null).ToString(), out returnVal);

            return returnVal;
        }

        private double PerformCalculation(string calculation)
        {
            _calculationPerformed = true;
            _noCalculationPerformed += 1;
            double returnVal;
            double.TryParse(new DataTable().Compute(calculation, null).ToString(), out returnVal);

            return returnVal;
        }

        private static double DoSilentParseOfSubStep(string subCalc)
        {
            var calcString = new StringBuilder();
            var firstOperatorFound = false;
            double total = 0;
            var numberCalcsMadeThisRun = 0;

            foreach (var c in subCalc.ToCharArray())
            {
                if (InputIsNumeric(c.ToString(CultureInfo.InvariantCulture)))
                {
                    calcString.Append(c);
                }
                else
                {
                    if (!InputIsOperator(c)) continue;
                    if (!firstOperatorFound)
                    {
                        calcString.Append(c);
                        firstOperatorFound = true;
                    }
                    else
                    {
                        numberCalcsMadeThisRun += 1;
                        total = PerformCalculationSilent(calcString.ToString());
                        StartNewCalcString(calcString, total, c);
                    }
                }
            }
            if (numberCalcsMadeThisRun < CountOperators(subCalc))
            {
                total = PerformCalculationSilent(calcString.ToString());
            }
            return total;
        }
    }
}
