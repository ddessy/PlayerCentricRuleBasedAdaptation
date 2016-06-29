using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestRuleBasedAdaptation
{
    [TestClass]
    public class PatternsRegistration
    {
        [TestMethod]
        public void RegisterPattern_AddNewPattern_ReturnTrue()
        {
            //arrange
            PlayerCentricRulePattern pattern = new PlayerCentricRulePattern();

            //act
            bool isPatternRegistered = pattern.RegisterPattern("Happy pattern", "happiness", "moving average", "120000 300000 600000", "x  GT(x+10)  LT(x-20)"); 

            //assert
            Assert.IsTrue(isPatternRegistered);
        }

        [TestMethod]
        public void RegisterPattern_AddAlreadyExistingPattern_ReturnFalse()
        {
            //arrange
            PlayerCentricRulePattern pattern = new PlayerCentricRulePattern();

            //act
            pattern.RegisterPattern("Happy pattern", "happiness", "moving average", "120000 300000 600000", "x  GT(x+10)  LT(x-20)");
            bool isPatternRegistered = pattern.RegisterPattern("Happy pattern", "happiness", "moving average", "120000 300000 600000", "x  GT(x+10)  LT(x-20)"); 

            //assert
            Assert.IsFalse(isPatternRegistered);
        }

        [TestMethod]
        public void GetRegisterPattern_AddSeveralPatterns_ReturnAllRegisteredPattern()
        {
            //arrange
            PlayerCentricRulePattern pattern = new PlayerCentricRulePattern();

            //act
            //add several patterns
            pattern.RegisterPattern("Happy pattern", "happiness", "moving average", "120000 300000 600000", "x  x+10)  x-20");
            pattern.RegisterPattern("A quiz points rule", "Quiz result", "none", "GT(180000) LT(480000)", "GT(20) AND LT(30)");
            pattern.RegisterPattern("A shot rule", "No of shots", "none", "GT(600000)  6000  LT(900000)", "GT(100)");
            pattern.RegisterPattern("GSR mean pattern", "GSR", "average", "t  t+3000  t+6000  t+9000", "16 20 24 20");
            pattern.RegisterPattern("Simple GSR mean pattern", "GSR", "average", "3000 6000  9000", "20 24 20");

            //assert
            Assert.AreEqual(pattern.GetRegisterPattern().Count, 5);
        }

        [TestMethod]
        public void GetPatternsByMetric_AddSeveralPatternsForAMetric_ReturnAllPatternsForAMetric()
        {
            //arrange
            PlayerCentricRulePattern pattern = new PlayerCentricRulePattern();

            //act
            //add several patterns for the metric "GSR"
            pattern.RegisterPattern("GSR mean pattern", "GSR", "average", "t  t+3000  t+6000  t+9000", "16 20 24 20");
            pattern.RegisterPattern("Simple GSR mean pattern", "GSR", "average", "3000 6000  9000", "20 24 20");

            //assert
            Assert.AreEqual(pattern.GetPatternsByMetric("GSR").Count, 2);
        }

        [TestMethod]
        public void GetPatternByName_GetExistingPattern_ReturnThePattern()
        {
            //arrange
            PlayerCentricRulePattern pattern = new PlayerCentricRulePattern();
            List<Object> targetPattern = new List<Object> { "GSR", "average", "t t+3000 t+6000 t+9000", "16 20 24 20" };
            pattern.RegisterPattern("Simple GSR mean pattern", "GSR", "average", "3000 6000  9000", "20 24 20");

            //act
            //add a pattern for the metric "GSR"
            pattern.RegisterPattern("GSR pattern", "GSR", "average", "t t+3000 t+6000 t+9000", "16 20 24 20");
            List<Object> checkedPattern = pattern.GetPatternByName("GSR pattern");

            //assert
            Assert.IsTrue(checkedPattern.Count == targetPattern.Count && !checkedPattern.Except(targetPattern).Any());
        }

        [TestMethod]
        public void GetPatternByName_GetNoExistingPattern_ReturnNull()
        {
            //arrange
            PlayerCentricRulePattern pattern = new PlayerCentricRulePattern();

            //assert
            Assert.IsNull(pattern.GetPatternByName("GSR pattern"));
        }
    }
}
