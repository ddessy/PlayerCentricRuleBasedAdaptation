using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestRuleBasedAdaptation
{
    [TestClass]
    public class MetricPatternsSearching
    {
        PlayerCentricRulePattern pattern;

        [TestInitialize()]
        public void Initialize()
        {
            //arrange
            pattern = new PlayerCentricRulePattern();

            //act
            //add several patterns
            pattern.RegisterPattern("GSR mean pattern 1", "GSR", "none", "ALL(GT(180000) AND LT(480000))", "GT(20) AND LT(200)");
            pattern.RegisterPattern("GSR mean pattern 2", "GSR", "average 0", "t t+3000 t+4000 t+9000", "16 20 24 20");
            pattern.RegisterPattern("GSR mean pattern 3", "GSR", "none", "ALL(GT(180200) AND LT(470000))", "GT(100)");
            pattern.RegisterPattern("GSR mean pattern 4", "GSR", "average 1", "t t+3000 t+6000 t+9000", "16 20 24 20");
            pattern.RegisterPattern("GSR mean pattern 5", "GSR", "average 2", "3000 6000 9000", "20 24 20");
            pattern.RegisterPattern("Exception", "GSR Exception", "average 2", "3000 6000  9000", "20 24 20");
        }

        [TestMethod]
        public void GetPatternsByMetric_RegisterPatternsForAMetric_ReturnProperNumberOfRegisterredPattern()
        {
            //act
            List<string> realResult = pattern.GetPatternsByMetric("GSR");

            //assert
            List<String> properResult = new List<String> {
                                                           "GSR mean pattern 1",
                                                           "GSR mean pattern 2",
                                                           "GSR mean pattern 3",
                                                           "GSR mean pattern 4",
                                                           "GSR mean pattern 5"
                                                          };

            Assert.IsTrue(realResult.Count == properResult.Count && !realResult.Except(properResult).Any());
        }

        [TestMethod]
        public void GetSuccefulPatternsForMetric_TestRulePatterns_AllRulePatternsAreSuccessful()
        {
            //act
            //save data for the metric "GSR"
            pattern.SetMetricValue("GSR", 120, 180210);
            pattern.SetMetricValue("GSR", 150, 200100);
            pattern.SetMetricValue("GSR", 110, 460000);


            List<string> realResult = pattern.GetSuccefulPatternsForMetric("GSR");

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "GSR mean pattern 1",
                                                           "GSR mean pattern 3"
                                                          };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }

        [TestMethod]
        public void GetSuccefulPatternsForMetric_TestRulePattern_NotAllRulePatternsAreSuccessful()
        {
            //act
            //save data for the metric "GSR"
            pattern.SetMetricValue("GSR", 120, 180210);
            pattern.SetMetricValue("GSR", 150, 200100);
            pattern.SetMetricValue("GSR", 250, 300100);
            pattern.SetMetricValue("GSR", 110, 460000);


            List<string> realResult = pattern.GetSuccefulPatternsForMetric("GSR");

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "GSR mean pattern 3"
                                                          };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }

        [TestMethod]
        public void GetSuccefulPatternsForMetric_TestRelatedPatterns_AllRelatedPatternsAreSuccessful()
        {
            //act
            //save data for the metric "GSR"
            pattern.SetMetricValue("GSR", 16, 3000);
            pattern.SetMetricValue("GSR", 20, 6000);
            pattern.SetMetricValue("GSR", 24, 7000);
            pattern.SetMetricValue("GSR", 24, 9000);
            pattern.SetMetricValue("GSR", 20, 12000);


            List<string> realResult = pattern.GetSuccefulPatternsForMetric("GSR");

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "GSR mean pattern 2",
                                                           "GSR mean pattern 4"
                                                          };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }


        [TestMethod]
        public void GetSuccefulPatternsForMetric_TestRelatedPatterns_NotAllRelatedPatternsAreSuccessful()
        {
            //act
            //save data for the metric "GSR"
            pattern.SetMetricValue("GSR", 16, 3000);
            pattern.SetMetricValue("GSR", 20, 6000);
            pattern.SetMetricValue("GSR", 22, 7000);
            pattern.SetMetricValue("GSR", 24, 9000);
            pattern.SetMetricValue("GSR", 20, 12000);


            List<string> realResult = pattern.GetSuccefulPatternsForMetric("GSR");

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "GSR mean pattern 4"
                                                          };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }

        [TestMethod]
        public void GetSuccefulPatternsForMetric_TestAbsolutePatterns_AllAbsolutePatternsAreSuccessful()
        {
            //act
            //save data for the metric "GSR"
            pattern.SetMetricValue("GSR", 20, 3000);
            pattern.SetMetricValue("GSR", 24, 6000);
            pattern.SetMetricValue("GSR", 20, 9000);

            //save data for the metric "GSR Exception"
            pattern.SetMetricValue("GSR Exception", 20, 3000);
            pattern.SetMetricValue("GSR Exception", 24, 6000);
            pattern.SetMetricValue("GSR Exception", 20, 9000);


            List<string> realResult = pattern.GetSuccefulPatternsForMetric("GSR");
            realResult.AddRange(pattern.GetSuccefulPatternsForMetric("GSR Exception"));

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "GSR mean pattern 5",
                                                           "Exception"
                                                           };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }

        [TestMethod]
        public void GetSuccefulPatternsForMetric_TestAbsolutePatterns_NotAllAbsolutePatternsAreSuccessful()
        {
            //act
            //save data for the metric "GSR"
            pattern.SetMetricValue("GSR", 20, 3000);
            pattern.SetMetricValue("GSR", 24, 6000);
            pattern.SetMetricValue("GSR", 21, 9000);

            //save data for the metric "GSR Exception"
            pattern.SetMetricValue("GSR Exception", 20, 3000);
            pattern.SetMetricValue("GSR Exception", 24, 6000);
            pattern.SetMetricValue("GSR Exception", 20, 9000);


            List<string> realResult = pattern.GetSuccefulPatternsForMetric("GSR");
            realResult.AddRange(pattern.GetSuccefulPatternsForMetric("GSR Exception"));

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "Exception"
                                                           };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }
    }
}
