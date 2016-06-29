// ***********************************************************************
// Assembly         : RuleBasedAdaptation
// Author           : ddessy
// Created          : 01-25-2016
//
// Last Modified By : ddessy
// Last Modified On : 01-25-2016
// ***********************************************************************
// <copyright file="RuleBasedAdaptation.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using AssetManagerPackage;
using AssetPackage;

namespace Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset
{
    public class RuleBasedAdaptation : BaseAsset
    {
        #region Fields
        /// <summary>
        /// Options for controlling the operation.
        /// </summary>
        private RuleBasedAdaptationAssetSettings settings = null;
        #endregion Fields
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the RuleBasedAdaptation class.
        /// </summary>
        public RuleBasedAdaptation() : base()
        {
            //! Create Settings and let its BaseSettings class assign Defaultvalues where it can.
            // 
            settings = new RuleBasedAdaptationAssetSettings();
        }
        #endregion Constructors
        #region Properties
        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        ///
        /// <remarks>   Besides the toXml() and fromXml() methods, we never use this property but use
        ///                it's correctly typed backing field 'settings' instead. </remarks>
        /// <remarks> This property should go into each asset having Settings of its own. </remarks>
        /// <remarks>   The actual class used should be derived from BaseSettings (and not directly from
        ///             ISetting). </remarks>
        ///
        /// <value>
        /// The settings.
        /// </value>
        public override ISettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                settings = (value as RuleBasedAdaptationAssetSettings);
            }
        }
        #endregion Properties
        #region Methods
        // Your code goes here. 
        // Try to keep only API code to be used by the Game-Engine here 
        // and put all other code in separate classes.
        #endregion Methods
    }
}
