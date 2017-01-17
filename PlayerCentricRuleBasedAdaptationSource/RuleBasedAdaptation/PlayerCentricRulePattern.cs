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
        private Dictionary<String, List<Object>> _patternList; // ENCAPSULATE FIELD

        public Dictionary<String, List<Object>> PatternList
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
        private const String RELATIVE_PATTERN = "Relative Pattern";
        private const String ABSOLUTE_PATTERN = "Absolute Pattern";
        //RULE_PATTERN is used only for modeling values, but not time
        private const String RULE_PATTERN = "Rule Pattern";
        //RULE_ANY_PATTERN is used only for modeling time, but not values
        private const String RULE_ANY_PATTERN = "Rule Pattern for the last time value.";
        //RULE_ALL_PATTERN is used only for modeling time, but not values
        private const String RULE_ALL_PATTERN = "Rule Pattern for each one time value.";
        private const String RELATIVE_RULE_PATTERN = "Relative Rule Pattern";
        //TODO: Regular expression for patterns to be define.
        public const int VALUES_PER_SECOND = 100;

        public PlayerCentricRulePattern()
        {
            Metric = new Dictionary<string, Dictionary<int, int>>();
            PatternList = new Dictionary<string, List<Object>>();
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

            return GetSuccefulPatternsForMetric(metricName);
        }

        public List<String> SetMetricValue(String metricName, int value, int miliseconds)
        {
            Dictionary<int, int> metricValues = null;

            if (Metric.TryGetValue(metricName, out metricValues))
            {
                metricValues.Add(miliseconds, value);
            }

            return GetSuccefulPatternsForMetric(metricName);
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
            if (PatternList.ContainsKey(patternName))
            {
                AssetManagerPackage.AssetManager.Instance.Log(AssetPackage.Severity.Warning, "The pattern name: " + patternName + " does already exist.");
                return false;
            }

            List<Object> patternDefinition = new List<Object> { metricName, featureName, timeInterval, values };
            bool isPatternDefinitionExist = true;
            int i = 0;
            foreach (List<Object> patternValue in PatternList.Values)
            {
                foreach (Object patternMember in patternValue)
                {
                    if (!patternMember.Equals(patternDefinition.ElementAt(i)))
                    {
                        isPatternDefinitionExist = false;
                        break;
                    }

                    i++;
                }
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

        // Return true if the metric is unregistered correctly; False otherwise (if the pattern does not already exist).
        public bool UnregisterPattern(String patternName)
        {
            return PatternList.Remove(patternName);
        }

        public List<String> GetSuccefulPatternsForMetric(String metric)
        {
            List<String> succesfulPatterns = new List<String>();
            foreach (String patternName in GetPatternsByMetric(metric))
            {
                if (IsSuccesfulPattern(patternName, metric))
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
                String metricName = (String)GetPatternByName(patternName).ElementAt<Object>(0);
                if (metricName != null && metricName.Equals(metric, StringComparison.CurrentCulture))
                {
                    patternsByMetric.Add(patternName);
                }
            }

            return patternsByMetric;
        }

        public bool IsSuccesfulPattern(String patternName, String patternMetric)
        {
            Dictionary<String, String> patternType = GetPatternType(patternName);

            Dictionary<int, int> metricValues = GetMetricValues(patternMetric);

            if (patternType.Count > 0 && metricValues.Count > 0)
            {
                List<Object> patternByName = GetPatternByName(patternName);

                String patternTypeByTime = patternType.ElementAt<KeyValuePair<string, string>>(0).Value;
                String patternValues = (String)patternByName.ElementAt<Object>(3);
                String patternTimes = (String)patternByName.ElementAt<Object>(2);
                String patterrnTypeByValue = patternType.ElementAt<KeyValuePair<string, string>>(1).Value;

                if (ABSOLUTE_PATTERN.Equals(patternTypeByTime, StringComparison.CurrentCulture))
                {
                    return CheckTimeAbsolutePattern(patterrnTypeByValue, metricValues, patternValues, patternTimes);
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

                    return CheckTimeAbsolutePattern(patterrnTypeByValue, metricValues, patternValues, metricRelatedTimeArray);
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
                        return CheckTimeAbsolutePattern(patterrnTypeByValue, metricValues, patternValues, metricRuleAnyTimeArray.ToArray());
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
                        return CheckTimeAbsolutePattern(patterrnTypeByValue, metricValues, patternValues, metricRuleAllTimeArray.ToArray());
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

        public bool CheckTimeAbsolutePattern(string valueTypePattern, Dictionary<int, int> metricValues, String patternValues, string patternTimes)
        {
            int[] patternTimesArray = GetIntArray(patternTimes);
            return CheckTimeAbsolutePattern(valueTypePattern, metricValues, patternValues, patternTimesArray);
        }

        public bool CheckTimeAbsolutePattern(string valueTypePattern, Dictionary<int, int> metricValues, String patternValues, int[] patternTimesArray)
        {
            int i = 0;

            foreach (int currentPatternTime in patternTimesArray)
            {
                int valueForTime = metricValues.ContainsKey(currentPatternTime) ? metricValues[currentPatternTime] : -1;//GetValueForTimeAndChannel(currentPatternTime, channelValue.Key, channelsValues);
                if (ABSOLUTE_PATTERN.Equals(valueTypePattern, StringComparison.CurrentCulture))
                {
                    int[] patternValueIntArray = GetIntArray(patternValues);
                    if (patternValueIntArray[i] != valueForTime)
                    {
                        return false;
                    }
                }
                else if (RELATIVE_PATTERN.Equals(valueTypePattern, StringComparison.CurrentCulture))
                {
                    String[] patternValueStrArray = GetStrArray(patternValues);
                    int firstValue = metricValues.First().Value;
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
                }
                else if (RULE_PATTERN.Equals(valueTypePattern, StringComparison.CurrentCulture))
                {
                    if (!IsValueSatisfyPattern(patternValues, valueForTime))
                    {
                        return false;
                    }
                }
                i++;
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
            string[] operands = expression.Split('>', '<', '=');
            if(expression.Contains(">"))
            {
                return Int32.Parse(operands[0]) > Int32.Parse(operands[1]);
            }
            else if (expression.Contains("<"))
            {
                return Int32.Parse(operands[0]) < Int32.Parse(operands[1]);
            }
            else if (expression.Contains("="))
            {
                return Int32.Parse(operands[0]) == Int32.Parse(operands[1]);
            }
            return false;
        }


        //Simplified version of a string expression evaluation.
        //The method support evaluation of expressions following the format: \d+('+' | '-' | '/' | '*')\d+(('+' | '-' )\d)*
        public static Object Evaluate(string expression)
        {
            expression = expression.Replace(" ", string.Empty);
            String tmpExpression = expression;
            string[] operands = expression.Split('+', '-', '/', '*');
            int result = 0;
            Int32.TryParse(operands[0], out result);
            for (int i = 0; i < operands.Count() - 1; i++)
            {
                String mathOperator = tmpExpression.Substring(tmpExpression.IndexOf(operands[i]) + operands[i].Length, 1);
                tmpExpression = ReplaceFirst(expression, operands[i], "");
                if ("+".Equals(mathOperator))
                {
                    result += Int32.Parse(operands[i + 1]);
                }

                if ("-".Equals(mathOperator))
                {
                    result -= Int32.Parse(operands[i + 1]);
                }


                if ("*".Equals(mathOperator))
                {
                    result *= Int32.Parse(operands[i + 1]);
                }

                if ("/".Equals(mathOperator))
                {
                    result /= Int32.Parse(operands[i + 1]);
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
        public Dictionary<String, String> GetPatternType(String patternName)
        {
            List<Object> patternValue = GetPatternByName(patternName);
            if (patternValue != null)
            {
                String timePattern = GetTimeValuesPatternType((String)patternValue.ElementAt<Object>(2));
                String valuesPattern = GetTimeValuesPatternType((String)patternValue.ElementAt<Object>(3));

                if (timePattern != null && valuesPattern != null)
                {
                    return new Dictionary<string, string>() {
                        { "timePattern", timePattern },
                        { "valuesPattern", valuesPattern}
                    };
                }
            }

            AssetManagerPackage.AssetManager.Instance.Log(AssetPackage.Severity.Warning, "The pattern is not supported.");
            return null;
        }

        //Return all registered patterns.
        public Dictionary<String, List<Object>> GetRegisterPattern()
        {
            return PatternList;
        }

        //Return a pattern by Name.
        public List<Object> GetPatternByName(String patternName)
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
