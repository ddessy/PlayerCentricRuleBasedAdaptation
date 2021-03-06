﻿/*
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset;

namespace UnitTestRuleBasedAdaptation
{
    [TestClass]
    public class PlayerCentricStatisticExtractorTest
    {
        PlayerCentricStatisticExtractorDefault pattern;

        [TestInitialize()]
        public void Initialize()
        {
            //arrange
            pattern = new PlayerCentricStatisticExtractorDefault();

            //act
            //add several patterns
            pattern.RegisterPattern("GSR mean pattern 1", "GSR", "none", "ALL(GT(180000) AND LT(480000))", "GT(20) AND LT(200)");
            pattern.RegisterPattern("GSR mean pattern 2", "GSR", "average 0", "3000 6000 7000 12000", "x x+4 x+8 x+4");
            pattern.RegisterPattern("GSR mean pattern 3", "GSR", "none", "ALL(GT(180200) AND LT(470000))", "GT(100)");
            pattern.RegisterPattern("GSR mean pattern 4", "GSR", "average 1", "t t+3000 t+6000 t+9000", "16 20 24 20");
            pattern.RegisterPattern("GSR mean pattern 5", "GSR", "average 2", "3000 6000 9000", "20 24 20");
            pattern.RegisterPattern("Exception", "GSR Exception", "average 2", "3000 6000  9000", "20 24 20");
            pattern.RegisterPattern("GSR examine equals milliseconds", "GSR equals", "none", "ALL(EQ(180))", "GT(100)");
            pattern.RegisterPattern("GSR examine equals value", "GSR equals", "none", "ALL(GT(180))", "EQ(100)");
        }

        [TestMethod]
        public void GetRegisterMetrics_RegisterMetrics_ReturnAllRegisterredMetrics()
        {
            //act
            List<string> realResult = pattern.GetRegisterMetrics();

            //assert
            List<String> properResult = new List<String> {
                                                           "GSR",
                                                           "GSR Exception",
                                                           "GSR equals"
                                                          };

            Assert.IsTrue(realResult.Count == properResult.Count && !(realResult.Except(properResult).Any()));
        }

        [TestMethod]
        public void FindPattern_TestPatternMatched_AllRulePatternsAreSuccessful()
        {
            //act
            //save data for the metric "GSR"
            pattern.SetMetricValue("GSR", 120, 180210);
            pattern.SetMetricValue("GSR", 150, 200100);
            pattern.SetMetricValue("GSR", 110, 460000);


            List<string> realResult = pattern.FindPattern("GSR");

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "GSR mean pattern 1",
                                                           "GSR mean pattern 3"
                                                          };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }

        [TestMethod]
        public void FindPattern_TestEQPatternMatched_OneRulePatternsAreSuccessful()
        {
            //act
            //save data for the metric "GSR equals"
            pattern.SetMetricValue("GSR equals", 120, 180);
            pattern.SetMetricValue("GSR equals", 110, 181);
            pattern.SetMetricValue("GSR equals", 105, 182);


            List<string> realResult = pattern.FindPattern("GSR equals");

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "GSR examine equals milliseconds"
                                                          };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }

        [TestMethod]
        public void FindPattern_TestEQPatternMatched_AllRulePatternsAreSuccessful()
        {
            //act
            //save data for the metric "GSR equals"
            pattern.SetMetricValue("GSR equals", 100, 183);
            pattern.SetMetricValue("GSR equals", 100, 181);
            pattern.SetMetricValue("GSR equals", 105, 180);


            List<string> realResult = pattern.FindPattern("GSR equals");

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "GSR examine equals milliseconds",
                                                           "GSR examine equals value"
                                                          };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }

        [TestMethod]
        public void FindPattern_TestRelativeTimePatternMatched_AllRulePatternsAreSuccessful()
        {
            //act
            //save data for the metric "GSR equals"
            pattern.SetMetricValue("GSR", 16, 3000);
            pattern.SetMetricValue("GSR", 20, 6000);
            pattern.SetMetricValue("GSR", 24, 9000);    
            pattern.SetMetricValue("GSR", 20, 12000);

            List<string> realResult = pattern.FindPattern("GSR");

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "GSR mean pattern 4"
                                                          };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }

        [TestMethod]
        public void FindPattern_TestRelativeValuePatternMatched_AllRulePatternsAreSuccessful()
        {
            //act
            //save data for the metric "GSR equals"
            pattern.SetMetricValue("GSR", 16, 3000);
            pattern.SetMetricValue("GSR", 20, 6000);
            pattern.SetMetricValue("GSR", 24, 7000);
            pattern.SetMetricValue("GSR", 20, 12000);

            List<string> realResult = pattern.FindPattern("GSR");

            //assert
            //define expected result
            List<String> expectedResult = new List<String> {
                                                           "GSR mean pattern 2"
                                                          };
            Assert.IsTrue(realResult.Count == expectedResult.Count && !realResult.Except(expectedResult).Any());
        }

        [TestMethod]
        public void FindPattern_TestPatternMatched_AllRulePatternsAreSuccessfu()
        {
            Assert.IsTrue(pattern.PatternEventHandler(null, null) == null);
        }
    }
}
