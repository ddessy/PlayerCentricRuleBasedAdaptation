using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset
{
    public class Pattern
    {
        #region Fields
        private String patternName;
        private String metricName;
        private String featureName;
        private String timeSequence;
        private String valueSequence;
        private FittingLine fittingLine;
        #endregion Fields

        #region Constructors
        public Pattern()
        {
            //
        }

        public Pattern(String metricName, String featureName, String timeInterval, String values)
        {
            this.PatternName = null;
            this.MetricName = metricName;
            this.FeatureName = featureName;
            this.TimeSequence = timeInterval;
            this.ValueSequence = values;
            this.FittingLine = null;
        }

        public Pattern(String patternName, String metricName, String featureName, String timeInterval, String values)
        {
            this.PatternName = patternName;
            this.MetricName = metricName;
            this.FeatureName = featureName;
            this.TimeSequence = timeInterval;
            this.ValueSequence = values;
            this.FittingLine = null;
        }

        public Pattern(String metricName, String featureName, String timeInterval, String values, FittingLine fittingLine)
        {
            this.PatternName = null;
            this.MetricName = metricName;
            this.FeatureName = featureName;
            this.TimeSequence = timeInterval;
            this.ValueSequence = values;
            this.FittingLine = fittingLine;
        }

        public Pattern(String patternName, String metricName, String featureName, String timeSequence, String valueSequence, FittingLine fittingLine)
        {
            this.PatternName = patternName;
            this.MetricName = metricName;
            this.FeatureName = featureName;
            this.TimeSequence = timeSequence;
            this.ValueSequence = valueSequence;
            this.FittingLine = fittingLine;
        }
        #endregion Constructors

        #region Properties
        public string PatternName
        {
            get
            {
                return patternName;
            }

            set
            {
                patternName = value;
            }
        }

        public string MetricName
        {
            get
            {
                return metricName;
            }

            set
            {
                metricName = value;
            }
        }

        public string FeatureName
        {
            get
            {
                return featureName;
            }

            set
            {
                featureName = value;
            }
        }

        public string TimeSequence
        {
            get
            {
                return timeSequence;
            }

            set
            {
                timeSequence = value;
            }
        }

        public string ValueSequence
        {
            get
            {
                return valueSequence;
            }

            set
            {
                valueSequence = value;
            }
        }

        public FittingLine FittingLine
        {
            get
            {
                return fittingLine;
            }

            set
            {
                fittingLine = value;
            }
        }
        #endregion Properties

        #region Methods
        public bool Equals(Pattern pattern)
        {
            return (
                    pattern != null &&
                    this.FeatureName.Equals(pattern.FeatureName) &&
                    this.MetricName.Equals(pattern.MetricName)  &&
                    this.TimeSequence.Equals(pattern.TimeSequence) &&
                    this.ValueSequence.Equals(pattern.ValueSequence) &&
                    ((String.IsNullOrEmpty(this.PatternName) && String.IsNullOrEmpty(pattern.PatternName)) || 
                      this.PatternName.Equals(pattern.PatternName)) &&
                    ((this.FittingLine == null && pattern.FittingLine == null) ||
                     this.FittingLine.Equals(pattern.FittingLine))
                    );
        }
        #endregion Methods
    }
}
