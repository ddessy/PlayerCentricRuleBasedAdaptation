
using System;

namespace Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset
{
    class PatternBasedAdaptationAsset : Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset.PlayCentricStatisticExtractor
    {
        public override Object PatternEventHandler(Object patternInput, Object gameObject)
        {
            PatternAction((System.Collections.Generic.List<string>)patternInput, (UnityEngine.GameObject) gameObject);
            return null;
        }

        public void PatternAction(System.Collections.Generic.List<string> patterns, UnityEngine.GameObject gameObject)
        {
            if (patterns.Contains("Change color in blue"))
            {
                gameObject.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.blue;
            }
            else if (patterns.Contains("Change color in green"))
            {
                gameObject.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.green;
            }
            else if (patterns.Contains("Change color in green2"))
            {
                gameObject.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.green;
            }
            else if (patterns.Contains("Change color in yellow"))
            {
                gameObject.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.yellow;
            }
            else if (patterns.Contains("Change color in white"))
            {
                gameObject.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.white;
            }
            else if (patterns.Contains("Change color in red"))
            {
                gameObject.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.red;
            }
            else if (patterns.Contains("Change color in cyan"))
            {
                gameObject.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.cyan;
            }
            else if (patterns.Contains("Change color in black"))
            {
                gameObject.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.black;
            }
            else if (patterns.Contains("Change color in magenta"))
            {
                gameObject.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.magenta;
            }
            else if (patterns.Contains("Change color in blue2"))
            {
                gameObject.GetComponent<UnityEngine.Renderer>().material.color = UnityEngine.Color.blue;
            }

            if(patterns.Contains("Change size"))
            {
                UnityEngine.Vector3 currentScale = gameObject.transform.localScale;
                gameObject.transform.localScale = currentScale - currentScale * 0.15f;
            }
        }
    }
}
