using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset
{
    public class FittingLine
    {
        #region Fields
        /// <summary>
        /// Procent of acceptable deviation of a sample point from the estimate point.
        /// It means that the confidence interval includes values: (estimate point - estimate point * confidenceInterval, estimate point + estimate point * confidenceInterval).
        /// The acceptable values are numbers between -1 and 1.
        /// If the value is negative confidence interval includes values: (estimate point - estimate point * |confidenceInterval|, estimate point].
        /// </summary>
        private double confidenceInterval;
        /// <summary>
        /// Percentage of all sample points belonging to its confidence interval (for each one sample point there is a different interval depending on estimated point and confidence interval). 
        /// The acceptable values are numbers in the range (0, 1].
        /// </summary>
        private double confidenceLevel;
        #endregion Fields

        #region Constructors
        public FittingLine()
        {
            //
        }
        public FittingLine(double confidenceInterval, double confidenceLevel)
        {
            this.ConfidenceInterval = confidenceInterval;
            this.ConfidenceLevel = confidenceLevel;
        }
        #endregion Constructors
        public FittingLine(double confidenceInterval)
        {
            this.ConfidenceInterval = confidenceInterval;
            this.ConfidenceLevel = 1;
        }


        #region Properties
        public double ConfidenceInterval
        {
            get
            {
                return confidenceInterval;
            }

            set
            {
                confidenceInterval = value;
            }
        }

        public double ConfidenceLevel
        {
            get
            {
                return confidenceLevel;
            }

            set
            {
                confidenceLevel = value;
            }
        }
        #endregion Properties

        #region Methods
        public bool Equals(FittingLine fitLine)
        {
            return (fitLine != null && this.ConfidenceLevel.Equals(fitLine.ConfidenceLevel) && 
                    this.ConfidenceInterval.Equals(fitLine.ConfidenceInterval));
        }
        #endregion Methods
    }
}
