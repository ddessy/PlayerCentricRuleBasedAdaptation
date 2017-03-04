using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset
{
    public class PatternType
    {
        #region Fields
        private String timePatternType;
        private String valuePatternType;
        private FittingLine fittingLine;
        #endregion Fields

        #region Constructors
        public PatternType()
        {
            //
        }

        public PatternType(String timePatternType, String valuePatternType, FittingLine fittingLine)
        {
            this.TimePatternType = timePatternType;
            this.ValuePatternType = valuePatternType;
            this.FittingLine = fittingLine;
        }
        #endregion Constructors

        #region Properies
        public string TimePatternType
        {
            get
            {
                return timePatternType;
            }

            set
            {
                timePatternType = value;
            }
        }

        public string ValuePatternType
        {
            get
            {
                return valuePatternType;
            }

            set
            {
                valuePatternType = value;
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
    }
}
