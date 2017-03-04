using Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestRuleBasedAdaptation
{
    [TestClass]
    public class AbsolutePatternsWithRangeSearching
    {
        PlayerCentricRulePattern pattern;

        [TestInitialize()]
        public void Initialize()
        {
            //arrange
            pattern = new PlayerCentricRulePattern();
            FittingLine fittingLine1 = new FittingLine(0.7, 0.8);
            FittingLine fittingLine2 = new FittingLine(0.8, 1);
            FittingLine fittingLine3 = new FittingLine(-0.9, 1);
            FittingLine fittingLine4 = new FittingLine(-0.5, 0.6);

            //act
            //add several patterns
            pattern.RegisterPattern("Fitting Line With positive confidential interval 1", "Positive", "average 0", "t t+10 t+20 t+30 t+40 t+50 t+60 t+70 t+80 t+90", "10 20 30 40 50 60 70 80 90 100", fittingLine1);
            pattern.RegisterPattern("Fitting Line With positive confidential interval 2", "Positive", "average 1", "t t+10 t+20 t+30 t+40 t+50 t+60 t+70 t+80 t+90", "10 15 20 25 30 35 44 52 50 55", fittingLine2);
            pattern.RegisterPattern("Fitting Line With negative confidential interval 1", "Negative", "average 2", "10 20 40 70 50", "10 20 40 70 50", fittingLine3);
            pattern.RegisterPattern("Fitting Line With negative confidential interval 2", "Negative", "average 3", "ALL(GT(1))", "19 29 70 8 12", fittingLine4);
        }

        [TestMethod]
        public void GetSuccefulPatternsForMetric_TestAbsolutePatterns_AllAbsolutePatternsWithRangeAreSuccessful()
        {
            //act
            //save data for the metric "Positive"
            pattern.SetMetricValue("Positive", 9, 10);
            pattern.SetMetricValue("Positive", 16, 20);
            pattern.SetMetricValue("Positive", 23, 30);
            pattern.SetMetricValue("Positive", 29, 40);
            pattern.SetMetricValue("Positive", 36, 50);
            pattern.SetMetricValue("Positive", 42, 60);
            pattern.SetMetricValue("Positive", 50, 70);
            pattern.SetMetricValue("Positive", 57, 80);
            //This value is out of the range of fitting line 
            //defined for the pattern "Fitting Line With positive confidential interval 1"
            pattern.SetMetricValue("Positive", 53, 90);
            //This value is out of the range of fitting line 
            //defined for the pattern "Fitting Line With positive confidential interval 1"
            pattern.SetMetricValue("Positive", 62, 100);

            //save data for the metric "Negative"
            pattern.SetMetricValue("Negative", 10, 10);
            pattern.SetMetricValue("Negative", 19, 20);
            pattern.SetMetricValue("Negative", 37, 30);
            //This value is out of the range of fitting line 
            //defined for the pattern "Fitting Line With negative confidential interval 2"
            pattern.SetMetricValue("Negative", 64, 40);
            //This value is out of the range of fitting line 
            //defined for the pattern "Fitting Line With negative confidential interval 2"
            pattern.SetMetricValue("Negative", 47, 50);


            List<string> realResult = pattern.GetSuccessfulPatternsForMetric("Positive");
            realResult.AddRange(pattern.GetSuccessfulPatternsForMetric("Negative"));

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "Fitting Line With positive confidential interval 1",
                                                           "Fitting Line With positive confidential interval 2",
                                                           "Fitting Line With negative confidential interval 1",
                                                           "Fitting Line With negative confidential interval 2"
                                                           };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }

        [TestMethod]
        public void GetSuccefulPatternsForMetric_TestAbsolutePatterns_NotAllAbsolutePatternsWithRangeAreSuccessful()
        {
            //act
            //save data for the metric "Positive"
            pattern.SetMetricValue("Positive", 9, 10);
            pattern.SetMetricValue("Positive", 16, 20);
            pattern.SetMetricValue("Positive", 23, 30);
            pattern.SetMetricValue("Positive", 29, 40);
            pattern.SetMetricValue("Positive", 36, 50);
            pattern.SetMetricValue("Positive", 42, 60);
            pattern.SetMetricValue("Positive", 50, 70);
            //This value is out of the range of fitting line 
            //defined for the pattern "Fitting Line With positive confidential interval 1"
            pattern.SetMetricValue("Positive", 50, 80);
            //This value is out of the range of fitting line 
            //defined for the pattern "Fitting Line With positive confidential interval 1"
            pattern.SetMetricValue("Positive", 53, 90);
            //This value is out of the range of fitting line 
            //defined for the pattern "Fitting Line With positive confidential interval 1"
            pattern.SetMetricValue("Positive", 62, 100);

            //save data for the metric "Negative"
            pattern.SetMetricValue("Negative", 10, 10);
            pattern.SetMetricValue("Negative", 19, 20);
            pattern.SetMetricValue("Negative", 37, 30);
            //This value is out of the range of fitting line 
            //defined for the pattern "Fitting Line With negative confidential interval 2"
            pattern.SetMetricValue("Negative", 64, 40);
            //This value is out of the range of fitting line 
            //defined for the pattern "Fitting Line With negative confidential interval 2"
            pattern.SetMetricValue("Negative", 47, 50);


            List<string> realResult = pattern.GetSuccessfulPatternsForMetric("Positive");
            realResult.AddRange(pattern.GetSuccessfulPatternsForMetric("Negative"));

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "Fitting Line With positive confidential interval 2",
                                                           "Fitting Line With negative confidential interval 1",
                                                           "Fitting Line With negative confidential interval 2"
                                                           };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }
    }
}
