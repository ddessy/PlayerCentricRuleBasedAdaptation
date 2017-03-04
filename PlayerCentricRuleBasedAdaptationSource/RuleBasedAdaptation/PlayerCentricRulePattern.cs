/*
 * Copyright 2016 Sofia University
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union's Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset
{
    public class PlayerCentricRulePattern
    {
        private Dictionary<String, Dictionary<int, int>> _metric; // ENCAPSULATE FIELD

        public Dictionary<String, Dictionary<int, int>> Metric
        {
            get
            {
                return _metric;
            }
            set
            {
                _metric = value;
            }
        }
        private Dictionary<String, Pattern> _patternList; // ENCAPSULATE FIELD

        public Dictionary<String, Pattern> PatternList
        {
            get
            {
                return _patternList;
            }
            set
            {
                _patternList = value;
            }
        }

        //Pattern for relative values (e.g. x x+4 x+8 x-4)
        private const String RELATIVE_PATTERN = "Relative Pattern";
        //Pattern for absolute values (e.g. 2 5 100 12)
        private const String ABSOLUTE_PATTERN = "Absolute Pattern";
        //Pattern for absolute values and a fitting line (e.g. 2 5 100 12 with confidence interval = 0.7 and confidence level = 0.8)
        private const String ABSOLUTE_PATTERN_WITH_RANGE = "Absolute Pattern with range";
        //RULE_PATTERN is used only for modeling values, but not time
        private const String RULE_PATTERN = "Rule Pattern";
        //RULE_ANY_PATTERN is used only for modeling time, but not values (e.g. GT(180200) AND LT(470000))
        private const String RULE_ANY_PATTERN = "Rule Pattern for the last time value.";
        //RULE_ALL_PATTERN is used only for modeling time, but not values
        private const String RULE_ALL_PATTERN = "Rule Pattern for each one time value.";
        private const String RELATIVE_RULE_PATTERN = "Relative Rule Pattern";
        //TODO: Regular expression for patterns to be define.
        public const int VALUES_PER_SECOND = 100;

        public PlayerCentricRulePattern()
        {
            Metric = new Dictionary<string, Dictionary<int, int>>();
            PatternList = new Dictionary<string, Pattern>();
        }

        public bool RegisterMetric(String metricName)
        {
            if (Metric.ContainsKey(metricName))
            {
                AssetManagerPackage.AssetManager.Instance.Log(AssetPackage.Severity.Error, "The metric does already exist.");
                return false;
            }

            Metric.Add(metricName, new Dictionary<int, int>());
            AssetManagerPackage.AssetManager.Instance.Log(AssetPackage.Severity.Information, "The metric is registered correctly.");
            return true;
        }

        public List<String> GetRegisterMetrics()
        {
            return new List<String>(Metric.Keys);
        }

        public void PrintRegisterMetrics()
        {
            foreach (String metricName in Metric.Keys)
            {
                AssetManagerPackage.AssetManager.Instance.Log(AssetPackage.Severity.Information, "metric: " + metricName);
            }
        }

        public List<String> SetMetricValue(String metricName, int value)
        {
            Dictionary<int, int> metricValues = null;

            if (Metric.TryGetValue(metricName, out metricValues))
            {
                metricValues.Add(DateTime.Now.Millisecond, value);
            }

            return GetSuccessfulPatternsForMetric(metricName);
        }

        public List<String> SetMetricValue(String metricName, int value, int milliseconds)
        {
            Dictionary<int, int> metricValues = null;

            if (Metric.TryGetValue(metricName, out metricValues))
            {
                metricValues.Add(milliseconds, value);
            }

            return GetSuccessfulPatternsForMetric(metricName);
        }

        public Dictionary<int, int> GetMetricValues(String metricName)
        {
            Dictionary<int, int> metricValue;
            if (Metric.TryGetValue(metricName, out metricValue))
            {
                return metricValue;
            }

            return null;
        }

        public bool RegisterPattern(String patternName, String metricName, String featureName, String timeInterval, String values)
        {
            Pattern patternDefinition = new Pattern( metricName, featureName, timeInterval, values );
            return IsPatternRegister(patternName, metricName, patternDefinition);
        }

        private bool IsPatternRegister(string patternName, string metricName, Pattern patternDefinition)
        {
            if (PatternList.ContainsKey(patternName))
            {
                AssetManagerPackage.AssetManager.Instance.Log(AssetPackage.Severity.Warning, "The pattern name: " + patternName + " does already exist.");
                return false;
            }

            bool isPatternDefinitionExist = true;
            int i = 0;
            foreach (Pattern patternValue in PatternList.Values)
            {
                if (!patternValue.Equals(patternDefinition))
                {
                    isPatternDefinitionExist = false;
                    break;
                }

                i++;
            }

            if (isPatternDefinitionExist && (PatternList.Count > 0))
            {
                AssetManagerPackage.AssetManager.Instance.Log(AssetPackage.Severity.Warning, "The pattern definition does already exist.");
                return false;
            }

            if (!Metric.ContainsKey(metricName))
            {
                RegisterMetric(metricName);
            }

            PatternList.Add(patternName, patternDefinition);
            AssetManagerPackage.AssetManager.Instance.Log(AssetPackage.Severity.Information, "The pattern is registered correctly.");
            return true;
        }

        public bool IsFittingLineDefinedCorrectly(FittingLine fittingLine)
        {
            if (fittingLine != null &&
                (fittingLine.ConfidenceInterval > 1 ||
                 fittingLine.ConfidenceInterval < -1 ||
                 fittingLine.ConfidenceLevel > 1 ||
                 fittingLine.ConfidenceLevel < 0))
            {
                AssetManagerPackage.AssetManager.Instance.Log(AssetPackage.Severity.Error, "The fitting line is not defined correctly.");
                return false;
            }

            return true;
        }

        public bool RegisterPattern(String patternName, String metricName, String featureName, String timeInterval, String values, FittingLine fittingLine)
        {
            if(!IsFittingLineDefinedCorrectly(fittingLine))
            {
                AssetManagerPackage.AssetManager.Instance.Log(AssetPackage.Severity.Error, "The fitting line for the pattern " + patternName + " is not defined correctly.");
                return false;
            }

            Pattern patternDefinition = new Pattern( metricName, featureName, timeInterval, values, fittingLine );
            return IsPatternRegister(patternName, metricName, patternDefinition);
        }

        // Return true if the metric is unregistered correctly; False otherwise (if the pattern does not already exist).
        public bool UnregisterPattern(String patternName)
        {
            return PatternList.Remove(patternName);
        }

        public List<String> GetSuccessfulPatternsForMetric(String metric)
        {
            List<String> succesfulPatterns = new List<String>();
            foreach (String patternName in GetPatternsByMetric(metric))
            {
                if (IsSuccessfulPattern(patternName, metric))
                {
                    succesfulPatterns.Add(patternName);
                }
            }

            return succesfulPatterns;
        }

        public List<String> GetPatternsByMetric(String metric)
        {
            List<String> patternsByMetric = new List<String>();

            foreach (String patternName in PatternList.Keys)
            {
                String metricName = (String)GetPatternByName(patternName).MetricName;
                if (metricName != null && metricName.Equals(metric, StringComparison.CurrentCulture))
                {
                    patternsByMetric.Add(patternName);
                }
            }

            return patternsByMetric;
        }

        public bool IsSuccessfulPattern(String patternName, String patternMetric)
        {
            PatternType patternType = GetPatternType(patternName);

            Dictionary<int, int> metricValues = GetMetricValues(patternMetric);

            if (patternType != null && metricValues.Count > 0)
            {
                Pattern patternByName = GetPatternByName(patternName);

                String patternTypeByTime = patternType.TimePatternType;
                String patternValues = (String)patternByName.ValueSequence;
                String patternTimes = (String)patternByName.TimeSequence;
                String patterrnTypeByValue = patternType.ValuePatternType;

                if (ABSOLUTE_PATTERN.Equals(patternTypeByTime, StringComparison.CurrentCulture))
                {
                    return CheckTimeAbsolutePattern(patterrnTypeByValue, metricValues, patternValues, patternTimes, patternType.FittingLine);
                }
                else if (RELATIVE_PATTERN.Equals(patternTypeByTime, StringComparison.CurrentCulture))
                {
                    String[] patternTimesArray = GetStrArray(patternTimes);
                    int patternTimeLength = patternTimesArray.Length;
                    int[] metricRelatedTimeArray = new int[patternTimeLength];

                    int lastIndexInTimeInt = patternTimeLength - 1;
                    int lastIncrement = GetIncrement(patternTimesArray[lastIndexInTimeInt]);
                    int lastTime = metricValues.Keys.Last<int>();
                    int firstTime = lastTime - lastIncrement;

                    if (patternTimeLength > 1)
                    {
                        metricRelatedTimeArray[0] = firstTime;
                    }

                    metricRelatedTimeArray[patternTimeLength - 1] = lastTime;
                    for (int i = 1; i < patternTimeLength - 1; i++)
                    {
                        int currentIncrement = GetIncrement(patternTimesArray[i]);
                        metricRelatedTimeArray[i] = firstTime + currentIncrement;
                    }

                    return CheckTimeAbsolutePattern(patterrnTypeByValue, metricValues, patternValues, metricRelatedTimeArray, patternType.FittingLine);
                }
                else if (RULE_ANY_PATTERN.Equals(patternTypeByTime, StringComparison.CurrentCulture))
                {
                    List<int> metricRuleAnyTimeArray = new List<int>();
                    
                    if (IsValueSatisfyPattern(patternTimes, metricValues.Keys.Last<int>()))
                    {
                        metricRuleAnyTimeArray.Add(metricValues.Keys.Last<int>());
                    }

                    if (metricRuleAnyTimeArray.Count > 0)
                    {
                        return CheckTimeAbsolutePattern(patterrnTypeByValue, metricValues, patternValues, metricRuleAnyTimeArray.ToArray(), patternType.FittingLine);
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (RULE_ALL_PATTERN.Equals(patternTypeByTime, StringComparison.CurrentCulture))
                {
                    List<int> metricRuleAllTimeArray = new List<int>();

                    foreach (int k in metricValues.Keys)
                    {
                        if (IsValueSatisfyPattern(patternTimes, k))
                        {
                            metricRuleAllTimeArray.Add(k);
                        }
                    }

                    if (metricRuleAllTimeArray.Count > 0)
                    {
                        return CheckTimeAbsolutePattern(patterrnTypeByValue, metricValues, patternValues, metricRuleAllTimeArray.ToArray(), patternType.FittingLine);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        private int GetIncrement(String target)
        {
            int result = -1;
            if (!String.IsNullOrEmpty(target) && int.TryParse(target.Substring(target.IndexOf('+')), out result))
            {
                return result;
            }

            return -1;
        }

        public bool CheckTimeAbsolutePattern(string valueTypePattern, Dictionary<int, int> metricValues, String patternValues, string patternTimes, FittingLine fittingLine)
        {
            int[] patternTimesArray = GetIntArray(patternTimes);
            return CheckTimeAbsolutePattern(valueTypePattern, metricValues, patternValues, patternTimesArray, fittingLine);
        }

        private bool CheckValueAbsolutePatternWithRange(String patternValues, Dictionary<int, int> metricValues, int[] patternTimesArray, FittingLine fittingLine)
        {
            if (!IsFittingLineDefinedCorrectly(fittingLine))
            {
                return false;
            }

            int i = 0;
            int[] patternValueIntArray = GetIntArray(patternValues);
            double accuracyRange = 1.0 - fittingLine.ConfidenceInterval;
            double accuracyLevel = fittingLine.ConfidenceLevel;
            int numberPointsInTheTarget = 0;

            foreach (int currentPatternTime in patternTimesArray)
            {
                int valueForTime = (metricValues != null && metricValues.ContainsKey(currentPatternTime)) ? metricValues[currentPatternTime] : -1;//GetValueForTimeAndChannel(currentPatternTime, channelValue.Key, channelsValues);

                if (IsValueInRange(patternValueIntArray[i], accuracyRange, valueForTime))
                {
                    numberPointsInTheTarget++;
                }

                i++;
            }

            if ((numberPointsInTheTarget + 0.0)/patternValueIntArray.Length >= accuracyLevel)
            {
                return true;
            }

            return false;
        }

        private static bool IsValueInRange(int patternValue, double accuracyRange, int valueForTime)
        {
            return (
                       (accuracyRange < 0 && valueForTime <= (patternValue + patternValue * accuracyRange)) ||
                       (accuracyRange > 0 &&
                        valueForTime >= (patternValue - patternValue * accuracyRange) &&
                        valueForTime <= (patternValue + patternValue * accuracyRange))
                    );
        }

        private bool CheckValueAbsolutePattern(String patternValues, Dictionary<int, int> metricValues, int[] patternTimesArray)
        {
            int i = 0;
            int[] patternValueIntArray = GetIntArray(patternValues);

            foreach (int currentPatternTime in patternTimesArray)
            {
                int valueForTime = (metricValues != null && metricValues.ContainsKey(currentPatternTime)) ? metricValues[currentPatternTime] : -1;//GetValueForTimeAndChannel(currentPatternTime, channelValue.Key, channelsValues);

                if (patternValueIntArray[i] != valueForTime)
                {
                    return false;
                }

                i++;
            }

            return true;
        }

        private bool CheckValueRelativePattern(String patternValues, Dictionary<int, int> metricValues, int[] patternTimesArray)
        {
            int i = 0;
            int firstValue = (metricValues != null && metricValues.Count > 0) ? metricValues.First().Value : 0;
            String[] patternValueStrArray = GetStrArray(patternValues);

            foreach (int currentPatternTime in patternTimesArray)
            {
                int valueForTime = (metricValues != null && metricValues.ContainsKey(currentPatternTime)) ? metricValues[currentPatternTime] : -1;//GetValueForTimeAndChannel(currentPatternTime, channelValue.Key, channelsValues);

                if (i > 0 && firstValue > -1)
                {
                    int patternValueInt = (int)Evaluate(patternValueStrArray[i].Replace(@"x", @firstValue.ToString()));
                    if (patternValueInt != valueForTime)
                    {
                        return false;
                    }
                }
                else if (i == 1)
                {
                    firstValue = valueForTime;
                }

                i++;
            }

            return true;
        }

        private bool CheckValueRulePattern(String patternValues, Dictionary<int, int> metricValues, int[] patternTimesArray)
        {
            int i = 0;

            foreach (int currentPatternTime in patternTimesArray)
            {
                int valueForTime = (metricValues != null && metricValues.ContainsKey(currentPatternTime)) ? metricValues[currentPatternTime] : -1;//GetValueForTimeAndChannel(currentPatternTime, channelValue.Key, channelsValues);

                if (!IsValueSatisfyPattern(patternValues, valueForTime))
                {
                    return false;
                }

                i++;
            }

            return true;
        }

        public bool CheckTimeAbsolutePattern(string valueTypePattern, Dictionary<int, int> metricValues, String patternValues, int[] patternTimesArray, FittingLine fittingLine)
        {
            if (ABSOLUTE_PATTERN_WITH_RANGE.Equals(valueTypePattern, StringComparison.CurrentCulture))
            {
                return CheckValueAbsolutePatternWithRange(patternValues, metricValues, patternTimesArray, fittingLine);
            }
            else if (ABSOLUTE_PATTERN.Equals(valueTypePattern, StringComparison.CurrentCulture))
            {
                return CheckValueAbsolutePattern(patternValues, metricValues, patternTimesArray);
            }
            else if (RELATIVE_PATTERN.Equals(valueTypePattern, StringComparison.CurrentCulture))
            {
                return CheckValueRelativePattern(patternValues, metricValues, patternTimesArray);
            }
            else if (RULE_PATTERN.Equals(valueTypePattern, StringComparison.CurrentCulture))
            {
                return CheckValueRulePattern(patternValues, metricValues, patternTimesArray);
            }

            return true;
        }

        private bool IsValueSatisfyPattern(String patternValue, int value)
        {
            patternValue = patternValue.ToUpper();
            patternValue = patternValue.Replace("ALL(", "")
                                       .Replace("ANY(", "")
                                       .Replace("GT(", value + ">")
                                       .Replace("LT(", value + "<")
                                       .Replace("EQ(", value + "=")
                                       .Replace(")", "")
                                       .Replace("AND", "&")
                                       .Replace("OR", "|");

            if (patternValue.Contains(">") || patternValue.Contains("<") || patternValue.Contains("="))
            {
                string[] operandsAnd = patternValue.Split('&');
                string[] operandsOr = patternValue.Split('|');
                bool result = false;

                if (operandsAnd.Count() > 1)
                {
                    foreach (string expressionItem in operandsAnd)
                    {
                        if (EvaluateBool(expressionItem) == false)
                        {
                            return false;
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }

                if (operandsOr.Count() > 1)
                {
                    foreach (string expressionItem in operandsOr)
                    {
                        if (EvaluateBool(expressionItem) == true)
                        {
                            return true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }

                if(operandsAnd.Count() == 1 && operandsOr.Count() == 1)
                {
                    return EvaluateBool(operandsAnd[0]);
                }

                return result;
            }

            return true;
        }

        public static bool EvaluateBool(string expression)
        {
            if (String.IsNullOrEmpty(expression)) return false;

            string[] operands = expression.Split('>', '<', '=');
            if(expression.Contains(">"))
            {
                return Int32.Parse(operands[0], CultureInfo.InvariantCulture) > Int32.Parse(operands[1], CultureInfo.InvariantCulture);
            }
            else if (expression.Contains("<"))
            {
                return Int32.Parse(operands[0], CultureInfo.InvariantCulture) < Int32.Parse(operands[1], CultureInfo.InvariantCulture);
            }
            else if (expression.Contains("="))
            {
                return Int32.Parse(operands[0], CultureInfo.InvariantCulture) == Int32.Parse(operands[1], CultureInfo.InvariantCulture);
            }
            return false;
        }


        //Simplified version of a string expression evaluation.
        //The method support evaluation of expressions following the format: \d+('+' | '-' | '/' | '*')\d+(('+' | '-' )\d)*
        public static Object Evaluate(string expression)
        {
            expression = (!String.IsNullOrEmpty(expression)) ? expression.Replace(" ", string.Empty) : expression;
            String tmpExpression = expression;
            string[] operands = expression.Split('+', '-', '/', '*');
            int result = 0;
            if (Int32.TryParse(operands[0], out result))
            {
                for (int i = 0; i < operands.Count() - 1; i++)
                {
                    String mathOperator = tmpExpression.Substring(tmpExpression.IndexOf(operands[i]) + operands[i].Length, 1);
                    tmpExpression = ReplaceFirst(expression, operands[i], "");
                    if ("+".Equals(mathOperator))
                    {
                        result += Int32.Parse(operands[i + 1], CultureInfo.InvariantCulture);
                    }

                    if ("-".Equals(mathOperator))
                    {
                        result -= Int32.Parse(operands[i + 1], CultureInfo.InvariantCulture);
                    }


                    if ("*".Equals(mathOperator))
                    {
                        result *= Int32.Parse(operands[i + 1], CultureInfo.InvariantCulture);
                    }

                    if ("/".Equals(mathOperator))
                    {
                        result /= Int32.Parse(operands[i + 1], CultureInfo.InvariantCulture);
                    }
                }
            }
            return result;
        }

        static string ReplaceFirst(string text, string oldString, string newString)
        {
            int pos = text.IndexOf(oldString);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + newString + text.Substring(pos + oldString.Length);
        }

        /*Pattern type is defined depending on 'time' and 'values'.
        Types of patterns depending on the value of 'time' are following:
        1. { name=”GSR mean pattern”, metric=”GSR”, feature=”average”, time=”t  t+3000  t+6000  t+9000”, values=”16  20  24  20” }
        2. { name=”Happy pattern”, metric=”happiness”, feature=”moving average”, time=”120000 300000 600000”, values=”x  GT(x+10)  LT(x-20) }
        3. { name=”A quiz points rule”, metric=”Quiz result”, feature=”none”, time=”GT(180000) LT(480000)”, values=”GT(20) AND LT(30)” }
        Types of patterns depending on the value of 'values' are analogically of the above.*/
        public PatternType GetPatternType(String patternName)
        {
            Pattern patternValue = GetPatternByName(patternName);
            if (patternValue != null)
            {
                //A pattern is define by 
                String timePattern = GetTimeValuesPatternType((String)patternValue.TimeSequence);
                String valuesPattern = GetTimeValuesPatternType((String)patternValue.ValueSequence);
                FittingLine fittingLine = patternValue.FittingLine;

                if (fittingLine != null && valuesPattern != null && ABSOLUTE_PATTERN.Equals(valuesPattern))
                {
                    //ABSOLUTE_PATTERN_WITH_RANGE is valid only for y coordinates
                    valuesPattern = ABSOLUTE_PATTERN_WITH_RANGE;
                }

                if (timePattern != null && valuesPattern != null)
                {
                    return new PatternType(timePattern, valuesPattern, fittingLine);
                }
            }

            AssetManagerPackage.AssetManager.Instance.Log(AssetPackage.Severity.Warning, "The pattern is not supported.");
            return null;
        }

        //Return a pattern by Name.
        public Pattern GetPatternByName(String patternName)
        {
            if (PatternList.ContainsKey(patternName))
            {
                return PatternList[patternName];
            }

            return null;
        }

        private String GetTimeValuesPatternType(String value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                if (IsRelativeRulePattern(value))
                {
                    return RELATIVE_RULE_PATTERN;
                }

                if (IsRulePattern(value))
                {
                    return RULE_PATTERN;
                }

                if (IsRuleAnyPattern(value))
                {
                    return RULE_ANY_PATTERN;
                }

                if (IsRuleAllPattern(value))
                {
                    return RULE_ALL_PATTERN;
                }

                if (IsRelativePattern(value))
                {
                    return RELATIVE_PATTERN;
                }

                if (IsValueListOfNumbers(value))
                {
                    return ABSOLUTE_PATTERN;
                }
            }

            return null;
        }

        private static bool IsRelativePattern(string value)
        {
            value = value.ToUpper();
            return ( (value.Contains("T") && value.Trim().Contains("T+")) || 
                     (value.Contains("X") && value.Trim().Contains("X+")) );
        }

        private static bool IsRelativeRulePattern(string value)
        {
            return (IsRelativePattern(value) && IsRulePattern(value));
        }

        private static bool IsRuleAnyPattern(string value)
        {
            value = value.ToUpper();
            return ( (value.Contains("LG") || value.Contains("GT") || value.Contains("EQ")) &&
                     (value.Contains("ANY") && !value.Contains("ALL")) );
        }

        private static bool IsRuleAllPattern(string value)
        {
            value = value.ToUpper();
            return ( (value.Contains("LG") || value.Contains("GT") || value.Contains("EQ")) &&
                     (!value.Contains("ANY") && value.Contains("ALL")) );
        }

        private static bool IsRulePattern(string value)
        {
            value = value.ToUpper();
            return ( (value.Contains("LG") || value.Contains("GT") || value.Contains("EQ")) &&
                     !(value.Contains("ANY") || value.Contains("ALL")) );
        }

        private bool IsValueListOfNumbers(String value)
        {
            int[] intArray = GetIntArray(value);

            if (intArray == null || intArray.Length == 0)
            {
                return false;
            }

            return true;
        }

        private int[] GetIntArray(string value)
        {
            value = value.Trim();
            value = Regex.Replace(value, @"\s+", " ");

            String[] valueArray = value.Split(' ');
            int[] result = new int[valueArray.Length];
            int j = 0;
            foreach (String currentStr in valueArray)
            {
                int i;
                if (!int.TryParse(currentStr, out i))
                {
                    return null;
                }

                result.SetValue(i, j);
                j++;
            }

            return result;
        }

        private String[] GetStrArray(String value)
        {
            value = value.Trim();
            value = Regex.Replace(value, @"\s+", " ");

            return value.Split(' ');
        }
    }
}
