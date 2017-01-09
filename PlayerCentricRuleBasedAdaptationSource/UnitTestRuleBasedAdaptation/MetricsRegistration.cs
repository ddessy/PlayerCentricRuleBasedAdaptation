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
