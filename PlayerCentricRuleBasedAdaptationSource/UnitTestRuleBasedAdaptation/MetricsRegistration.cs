using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset;

namespace UnitTestRuleBasedAdaptation
{
    [TestClass]
    public class MetricsRegistration
    {
        [TestMethod]
        public void RegisterMetric_AddNewMetric_ReturnTrue()
        { 
            //arrange
            PlayerCentricRulePattern pattern = new PlayerCentricRulePattern();

            //act
            bool isMetric1Registered = pattern.RegisterMetric("metric1");

            //assert
            Assert.IsTrue(isMetric1Registered);
        }

        [TestMethod]
        public void RegisterMetric_AddAlreadyExistingMetric_ReturnFalse()
        {
            //arrange
            PlayerCentricRulePattern pattern = new PlayerCentricRulePattern();

            //act
            //add the metric "metric1"
            pattern.RegisterMetric("metric1");
            //again add the metric "metric1"
            bool isMetric1Registered = pattern.RegisterMetric("metric1");

            //assert
            Assert.IsFalse(isMetric1Registered);
        }

        [TestMethod]
        public void RegisterMetric_AddSeveralMetrics_ReturnAllRegisteredMetrics()
        {
            //arrange
            PlayerCentricRulePattern pattern = new PlayerCentricRulePattern();

            //act
            //add several metrics
            pattern.RegisterMetric("metric1");
            pattern.RegisterMetric("metric2");
            pattern.RegisterMetric("metric3");
            pattern.RegisterMetric("metric4");
            pattern.RegisterMetric("metric5");

            //assert
            Assert.AreEqual(pattern.GetRegisterMetrics().Count, 5);
        }

        [TestMethod]
        public void GetMetricValues_SetMetricValue_ReturnSettedMetricValue()
        {
            // arrange
            PlayerCentricRulePattern pattern = new PlayerCentricRulePattern();
            //add a metric
            pattern.RegisterMetric("metric1");

            //act
            //set a metric value
            pattern.SetMetricValue("metric1", 5);

            //assert
            Assert.IsTrue(pattern.GetMetricValues("metric1").ContainsValue(5));
        }
    }
}
