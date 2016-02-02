// ***********************************************************************
// Assembly         : RuleBasedAdaptation
// Author           : ddessy
// Created          : 01-25-2016
//
// Last Modified By : ddessy
// Last Modified On : 02-02-2016
// ***********************************************************************
// <copyright file="PlayCentricStatisticExtractor.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using AssetManagerPackage;
using AssetPackage;

namespace Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset
{
    /// <summary>
    /// Define Player-centric rule-and-pattern-based adaptation asset class.
    /// </summary>
    public abstract class PlayCentricStatisticExtractor : BaseAsset
    {
        #region Fields
        /// <summary>
        /// The length of time window.
        /// </summary>
        private int _timeWindow; // ENCAPSULATE FIELD BY CODEIT.RIGHT

        public int TimeWindow
        {
            get
            {
                return _timeWindow;
            }
            set
            {
                _timeWindow = value;
            }
        }
        /// <summary>
        /// List of metrics.
        /// </summary>
        private Dictionary<String, Dictionary<int, int>> _metric; // ENCAPSULATE FIELD 

        public Dictionary<String, Dictionary<int, int>> Metric
        {
            get
            {
                return _metric;
            }
            set
            {
                _metric = value;
            }
        }
        /// <summary>
        /// List of patterns.
        /// </summary>
        private PlayerCentricRulePattern _patterns; // ENCAPSULATE FIELD BY CODEIT.RIGHT

        public PlayerCentricRulePattern Patterns
        {
            get
            {
                return _patterns;
            }
            set
            {
                _patterns = value;
            }
        }
        /// <summary>
        /// Save the time when for the last time the game is started.
        /// </summary>
        private int _lastStartTime; // ENCAPSULATE FIELD BY CODEIT.RIGHT

        public int LastStartTime
        {
            get
            {
                return _lastStartTime;
            }
            set
            {
                _lastStartTime = value;
            }
        }
        /// <summary>
        /// All time in when the gamer has played the game (The time from the start of the game without pauses).
        /// </summary>
        private int _assetModelTime; // ENCAPSULATE FIELD BY CODEIT.RIGHT

        public int AssetModelTime
        {
            get
            {
                return _assetModelTime;
            }
            set
            {
                _assetModelTime = value;
            }
        }
        /// <summary>
        /// All time in when the gamer has played the game (The time from the start of the game without pauses).
        /// </summary>
        private int _globalTime; // ENCAPSULATE FIELD BY CODEIT.RIGHT

        public int GlobalTime
        {
            get
            {
                return _globalTime;
            }
            set
            {
                _globalTime = value;
            }
        }
        /// <summary>
        /// Flag showing if the current status of game is 'Pause' or 'Play'.
        /// </summary>
        private int _isPaused; // ENCAPSULATE FIELD BY CODEIT.RIGHT

        public int IsPaused
        {
            get
            {
                return _isPaused;
            }
            set
            {
                _isPaused = value;
            }
        }
        #endregion Fields

        #region Constructors
        protected PlayCentricStatisticExtractor()
        {
            Patterns = new PlayerCentricRulePattern();

        }
        #endregion Constructors

        #region Properties
        public int GetTimeWindow()
        {
            return TimeWindow;
        }

        public void SetTimeWindow(int milliseconds)
        {
            this.TimeWindow = milliseconds;
        }

        public int GetModelTime()
        {
            return AssetModelTime;
        }

        //Returns all the registered metrics.
        public List<String> GetRegisterMetrics()
        {
            return Patterns.GetRegisterMetrics();
        }

        public List<String> SetMetricValue(String metricName, int value)
        {
            return Patterns.SetMetricValue(metricName, value);
        }

        //Return all registered patterns.
        public Dictionary<String, List<Object>> GetRegisterPattern()
        {
            return Patterns.GetRegisterPattern();
        }

        //Return the current global time.
        public int GetGlobalTime()
        {
            return GlobalTime;
        }
        #endregion Properties

        #region Methods
        public bool RegisterMetric(String metric)
        {
            return Patterns.RegisterMetric(metric);
        }

        public void PrintRegisterMetrics()
        {
            Patterns.PrintRegisterMetrics();
        }

        // Return true if the metric is registered correctly; False otherwise (if the pattern does already exist).
        public bool RegisterPattern(String patternName, String metricName, String featureName, String timeInterval, String values)
        {
            return Patterns.RegisterPattern(patternName, metricName, featureName, timeInterval, values);
        }

        // Return true if the metric is unregistered correctly; False otherwise (if the pattern does not already exist).
        public bool UnregisterPattern(String patternName)
        {
            return Patterns.UnregisterPattern(patternName);
        }

        // Return true if the pattern is found in the signal record; False otherwise (if the pattern is not found).
        public List<String> FindPattern(String metricName)
        {
            return Patterns.GetSuccefulPatternsForMetric(metricName);
        }

        //Model_time_start is in milliseconds (usually zero).
        public void Start()
        {
            IsPaused = 0;
            LastStartTime = DateTime.Now.Millisecond;
            AssetModelTime = 0;
        }

        //Start the global time (in milliseconds). It can be synchronize with asset model time trough the parameter synchronizationTime.
        public void SetGlobalTime(int synchronizationTime)
        {
            GlobalTime = DateTime.Now.Millisecond + synchronizationTime;
        }

        //At the moment of pause assetModelTime is frozen.
        public void Pause()
        {
            IsPaused = 1;
            AssetModelTime += DateTime.Now.Millisecond - LastStartTime;
        }

        //Restart the last asset start time. Makes the asset to resume after a pause; when called without being paused, the method has no effect. 
        public void Reset()
        {
            if (IsPaused == 1)
            {
                IsPaused = 0;
                LastStartTime = DateTime.Now.Millisecond;
            }
        }

        //Makes the asset to stop; when stopped the asset model time does not advance and the asset does not set metric values nor detects patterns & rules.
        //After been stopped the asset cannot be resumed but only can be started.
        public void Stop()
        {
            IsPaused = -1;
            AssetModelTime += DateTime.Now.Millisecond - LastStartTime;
        }

        //Should be overwritten code for changing game features when the pattern with patternName has found.
        public abstract Object PatternEventHandler(Object patternInput, Object game);

        #endregion Methods
    }
}
