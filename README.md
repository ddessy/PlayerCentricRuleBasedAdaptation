# Player-centric rule-and-pattern-based adaptation asset

The Player-centric rule-and-pattern-based adaptation asset uses metrics of player's performance, emotional status and/or playing style for realization of dynamical adaptation of various game features such as adaptation of player-driven game tasks and/or game assistance, dynamic adjustment of task difficulty, and/or adjustment of properties of audio-visual content and effects. The asset receives as input registration requests of player-centric metrics together with simple formal definitions of rules and patterns of variation of these metrics during the play time or their features such as mean, deviation and moving average within a desired time window. Next, it receives values of registered metrics and checks each incoming metric value for occurrence of a rule or a pattern defined for the metric or its feature. In case of finding such an occurrence, the asset fires a triggering event about this rule or pattern and executes its event handler, which is to be defined by the game developer depending on his/her goal to adapt specific game feature(s).

## Overview

Due to the very complex and volatile nature of the player’s character, player-centric adaptation remains being a crucial issue for both entertainment and applied digital games. To be effective, player-centric adaptation should follow deterministic player models for tracking, measuring and analyzing player’s behavior. The Player-centric rule-and-pattern-based adaptation asset uses metrics of player's performance, emotions and/or playing style for realization of dynamical adaptation
of various game features such as generated content at micro and/or macro level [1], or task difficulty. More precisely, the player-centric metric may indicate three important issues:

1. player performance, which embraces knowledge and intellectual abilities of the player’s including synthetic, analytical and practical skills [2], e.g. abilities for comprehension, memorization, evaluation, reasoning, decision making, and so on;
2. player affective (emotional) status, which represents emotional experiences, feelings and motivation of the player [3]. The affective status may be inferred by measuring facial, gestural, vocal, neural and/or psychophysiological bodily reactions [4];
3. playing style, which depends on player’s personality and ways of thinking and reacting to game challenges [5].

The asset receives as input registration requests of player-centric metrics about individual performance, emotional status and/or playing style, together with simple formal definitions (using a simple but yet powerful syntax) of rules and patterns of variation of these metrics during the play time or their features such as mean, deviation and moving average within a desired time window. Next, it receives values of registered metrics and checks each incoming metric value for occurrence of a rule or a pattern defined for the metric or its feature. In case of finding such an occurrence, the asset fires a triggering event about this rule or pattern and executes its event handler, which is to be defined by the game developer depending on his/her goal to adapt specific game feature(s). Thus, the asset does not depend on any concrete digital game and provides the game developer with freedom to program any control over game adaptation. Various game features can be dynamically adapted, which fall into three main groups regarding game mechanics, dynamics and aesthetics, as follows:

- adaptation of player-driven game tasks and/or game assistance such as helping instructions [6]. As well, the managed appearance of tasks and assistance in their performing during the game flow can be adjusted [7];
- dynamic adjustment of task difficulty – like in [8] where the adaptation control is based on the player’s anxiety, or in [4] where adaptation is controlled according the skill level of the player;
- adjustment of properties of audiovisual content and effects, such as ambient light in rooms in a video game [9].

The asset works only on the client side. It can be used without any other asset provided the game developer is able to provide the player-centric metric(s) used as a basis for game adaptation. Otherwise, the developer should use another asset(s) for provisioning of the metric(s) values, such as the Real-time Emotion Detection Asset or the RT Arousal Detection Using Galvanic Skin Response Asset.

## Asset architecture and input/output

The asset initialization process includes four issues:
- Registering player-centric metrics for monitoring whose variation is to be observed by the asset – metrics regarding player’s performance (knowledge and intellectual abilities), effective status (emotional experiences, feelings and motivation) or about the playing style are to be registered by names chosen by the game developer by calling the registerMetric method selecting the metric features the developer in interested in. Features initially supported are mean, standard deviation and moving average for a predefined time window (can be changed by the game developer);
- Setting adaptation triggering rules and patterns – one or more rules or patterns of change of an already defined metric or its feature are to be defined by using a simple but yet powerful syntax, by calling the observePatternRule method for setting rules/patters. Every rule/pattern, when fired, triggers an adaptation event, which causes execution of the event handler specific for rule/pattern (one handler may be used for several riles/patterns);
- Definition of adaptation event handlers – for each adaptation event triggered upon specific rule or pattern, a specific custom event handler will be executed upon the rule/pattern name, acting as adaptation method changing some game features about game mechanics, dynamics and aesthetics [10]. The custom event handler will overwrite the default eventHandler method (the default method does only reporting the event since it does not know anything of the game);
- Setting the global time and the moving average time window at the asset – the game developer can set/reset the asset timer at any time in order to synchronize it to the game engine, as far as setting the moving average time window.

Next, game metrics values can be send to the asset by calling its method for receiving a metric value for given metric (specified by its name). If an occurrence of specific pattern/rule will be found, the asset automatically triggers the event and executes its event handler producing adaptation changes in the game desired for this pattern/rule. Therefore, no explicit output is produced by the asset. Patterns can be easily defined by the game developer for absolute or relative values of metrics or their features. As well, they can refer to absolute or relative moments of time. Using relative time vector means a cyclic loop starting at the first vector element. Example of sample definitions of patterns and rules can be the following lines:
- Pattern example with absolute feature value using relative time moments after time t namely t+3000, t+6000 and t+9000 milliseconds:
{ name=”GSR mean pattern”, metric=”GSR”, feature=”average”, time=”t t+3000 t+6000 t+9000”, values=”16 20 24 20” }
- Pattern example with relative value x of the moving average (GT stands for Greater Than, LT stands for Less Than) using absolute time (at the second, fifth and the tenth minute): { name=”Happy pattern”, metric=”happiness”, feature=”moving average”, time=”120000 300000 600000”, values=”x GT(x+10) LT(x-20)” }
- Rule example for checking if the metric about quiz result is between 20 and 30 points between the third minute (after 180000 ms) and before the eighth minute (before 480000 ms): { name=”A quiz points rule”, metric=”Quiz result”, feature=”none”, time=”GT(180000) LT(480000)”, values=”GT(20) AND LT(30)” }

<p align="center">
<img src="https://raw.githubusercontent.com/ddessy/PlayerCentricRuleBasedAdaptation/master/Docs/fig1.png"
alt= "Fig.1: Two occurrences of the 'GSR mean pattern' found at times t1=15s (points in red color) and t2=20s (points with yellow color)" width="599")
</p>
<p align="center">
<i>
Fig.1: Two occurrences of the 'GSR mean pattern' found at times t1=15s (points in red color) and t2=20s (points with yellow color).
</i>
</p>

<p align="center">
<img src="https://raw.githubusercontent.com/ddessy/PlayerCentricRuleBasedAdaptation/master/Docs/fig2.png"
alt= "Fig.2: Two possible occurrences of the “Happy pattern” (pattern points are given in yellow)" width="599")
</p>
<p align="center">
<i>
Fig.2: Two possible occurrences of the “Happy pattern” (pattern points are given in yellow).
</i>
</p>
During playing time, monitoring of registered metric can be canceled by calling the `registerMetric` method with the metric name, which will incur stopping the monitoring of this metric and evaluation of its features. For a monitored metric, any of its patterns or rules set for observing can be stopped by calling the `forgetPatternRule` method.

## Asset API

- selects the metric features the developer is interested in
```java
public bool RegisterMetric(String metricName)
```
- sets a rule or pattern
```java
public bool RegisterPattern (String patternName, String metricName, String featureName,
                             String timeInterval, String valuesRule)
```
- the method that should be overwritten
```java
public override Object PatternEventHandler(Object patternInput, Object gameObject)
```
- unregister a rule or pattern
```java
public bool UnregisterPattern(String patternName)
```
- sets/resets the asset timer at any time (the time is given in milliseconds) in order to synchronize it to the game engine
```java
public void SetGlobalTime(int synchronizationTime)
```
- sets the moving average time window
```java
public void SetTimeWindow(int milliseconds)
```

## Asset integration steps

The player-centric rule-and-pattern-based adaptation asset is implemented in C# and .NET framework, version 3.5. It is packaged in both variants: as an archive file containing necessary dll files for integration and as an archive files containing the source code. It can be integrated in a Unity project or other C# or .NET based game project as using only dll files. The main classes implementing the asset are:

- `PlayCentricStatisticExtractor` – it is an abstract class defining interfaces for communication with the asset. The method
```java
public abstract Object PatternEventHandler(Object patternInput, Object gameObject)
```
is abstract and it should be overwritten for changing game features when the pattern with a particular pattern name has found. This class contains methods related to metrics, patterns and time window.
- `PlayerCentricRulePattern` – this is the class responsible for implementing all functionality related to metrics and patterns. It includes methods for registered/unregistered of metric and patterns, for setting metrics value, for discovering patterns, etc.
- `RuleBasedAdaptation` and `RuleBasedAdaptationAssetSettings` – these classes are helping and they are responsible for setting/getting asset configuration and for defining some test cases.

The asset integration process includes following steps:

1. Download the zip file `“PlayerCentricRuleBasedAdaptationDlls.zip”` with dll files implementing the asset functionality;
2. Copy the asset dll archive file in a folder in your Unity project and uncompressed it there(see fig. 4 [here](https://github.com/ddessy/PlayerCentricRuleBasedAdaptation/blob/master/Docs/Player-centric%20rule-and-pattern-based%20adaptation%20asset-integration%20tutorial-ver0.4.pdf))
3. Import dll files to your Unity project (see fig. 5 and fig.6 [here](https://github.com/ddessy/PlayerCentricRuleBasedAdaptation/blob/master/Docs/Player-centric%20rule-and-pattern-based%20adaptation%20asset-integration%20tutorial-ver0.4.pdf))
4. Add dll files to the references of your project in Visual Studio or other IDE that you use for your
C#  (see fig. 7 [here](https://github.com/ddessy/PlayerCentricRuleBasedAdaptation/blob/master/Docs/Player-centric%20rule-and-pattern-based%20adaptation%20asset-integration%20tutorial-ver0.4.pdf));
5. Add to your game project in Visual Studio a class extending the `abstract class Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset.PlayCentricStatisticExtractor` (see fig. 8 [here](https://github.com/ddessy/PlayerCentricRuleBasedAdaptation/blob/master/Docs/Player-centric%20rule-and-pattern-based%20adaptation%20asset-integration%20tutorial-ver0.4.pdf));
6. Implement the method `public abstract Object PatternEventHandler(Object patternInput, Object gameObject)`. This method has two parameters. The first of them patterInput contains the list of founded patterns, and the second parameter – gameObject – presents the game object, where the asset is used. The method should defined what action will trigger on the game object depending on founded patterns. An example of an implementation is presented [here](https://github.com/ddessy/PlayerCentricRuleBasedAdaptation/blob/master/Docs/Player-centric%20rule-and-pattern-based%20adaptation%20asset-integration%20tutorial-ver0.4.pdf).
7. It should be create an instance of the class extending `PlayCentricStatisticExtractor` (in our example from point 5. and 6. this is `PatternBasedAdaptationAsset`) in the class related to the game object that will use the asset. In the method `void Start()` has to be defined and registered all metrics (with the method `public bool RegisterMetric(String metricName)`) and patterns (with the method `public bool RegisterPattern(String patternName, String metricName, String featureName, String timeInterval, String values)`). When a metric value is changed, the new value has to be set for the correspond metric with the method `public List<String> SetMetricValue(String metricName, int value)`. This method returns the list with all patterns founded depending on the new metric value. At the end the event handler (`public void PatternAction (List<string> patterns, UnityEngine.GameObject gameObject)`) will be executed upon the list of rule/pattern name, acting as adaptation method changing some game features about game mechanics, dynamics and aesthetics [10]. An example of using `PatternBasedAdaptationAsset` is given [here](https://github.com/ddessy/PlayerCentricRuleBasedAdaptation/blob/master/Docs/Player-centric%20rule-and-pattern-based%20adaptation%20asset-integration%20tutorial-ver0.4.pdf).

## Configuration

### Asset configuration stages
The asset configuration process includes four stages similarly to the adaptation methods described in [12]:
- **Registering player-centric metrics** - Registering player-centric metrics for monitoring whose variation is to be observed by the asset – metrics regarding player’s performance (knowledge and intellectual abilities), effective status (emotional experiences, feelings and motivation) or about the playing style are to be registered by names chosen by the game developer by calling the `public bool RegisterMetric(String metricName)` method selecting the metric features the developer in interested in. Features initially supported are average, standard deviation and moving average for a predefined time window (can be changed by the game developer).

Example:

```java
PatternBasedAdaptationAsset testMetricPattern = new PatternBasedAdaptationAsset();
testMetricPattern.RegisterMetric("Number of hits");
```

- **Setting adaptation triggering rules and patterns** - At this stage, one or more rules or patterns of change of an already defined metric or its feature are to be defined by using a simple but yet powerful syntax, by calling `public bool RegisterPattern (String patternName, String metricName, String featureName, String timeInterval, String valuesRule)` method for setting rules/patterns. Every rule/pattern, when fired, triggers an adaptation event, which causes execution of the event handler specific for rule/pattern (one handler may be used for several riles/patterns). For explanation of the types of patterns and rules, see the next section of this tutorial.
Example:

```java
PatternBasedAdaptationAsset testMetricPattern = new PatternBasedAdaptationAsset();
testMetricPattern.RegisterPattern("Change color in blue", "Number of hits", "none", "GT(1)", "2");
testMetricPattern.RegisterPattern("Change size", "Number of hits", "none", "GT(1)", "GT(0)");
```
- **Definition of adaptation event handlers** - For each adaptation event triggered upon specific rule or pattern, a specific custom event handler will be executed upon the rule/pattern name, acting as adaptation method changing some game features about game mechanics, dynamics and aesthetics [10]. The custom event handler will implement the `PatternEventHandler` method of the abstract class `PlayCentricStatisticExtractor`.
- **Setting the global time and the moving average time window for the asset** - The game developer can set/reset the asset timer at any time (the time is given in milliseconds) in order to synchronize it to the game engine by using the `public void SetGlobalTime(int synchronizationTime)` method, as far as setting the moving average time window by using the `public void SetTimeWindow(int milliseconds)` method (as explained below).

Example:
```java
PlayCentricStatisticExtractor test = new PlayCentricStatisticExtractor();
//the global time will be 2 minutes
test.SetGlobalTime (120000);
//the time window will be equal to 40 seconds
test.SetTimeWindow (40000);
```

### Configuration of asset patterns and rules
For any player-centric metric registered at the asset (and/or one of its features), the game developer can set various patterns or rules of changing the values of the defined metric or one of its features. Patterns can be easily defined by the game developer for absolute or relative values of player-centric metrics such as number of shots, number of hit objects, values of player’s arousal, etc. Game developer can use the metric value or, as well, values of any feature of this metric. For the current version of the asset, several metric features are supported, as follows:
- none – no features are applied in pattern definition for the particular metric
- average – the average value for the metric since staring registration of values of the chosen metric at the asset
- standard deviation – for the average of the metric
- moving average – the moving average for the metric values registered at the asset within a time window (the time window duration is to be set in advance before starting registration of values for all the metrics)

A pattern or rule may refer to absolute or relative moments of time. Using relative time vector means a cyclic loop starting at the first vector element. Example of sample definitions of patterns and rules can be the following lines below.
- Pattern example with absolute feature value using relative time moments after time t, namely t+3000, t+6000 and t+9000 milliseconds, i.e.
```java
{ name=”GSR mean pattern”, metric=”GSR”, feature=”average”, time=”t t+3000 t+6000 t+9000”, values=”16 20 24 20” }:
```
```java
testMetricPattern.RegisterPattern("GSR mean pattern", "GSR", "average", "t t+3000 t+6000 t+9000", "16 20 24 20");
```
- Pattern example with relative value x of the moving average using absolute time (at the second, fifth and the tenth minute), i.e.
```java
 { name=”Happy pattern”, metric=”happiness”, feature=”moving average”,
   time=”120000 300000 600000”, values=”x x+10 x-20” }:
 ```
 ```java
 testMetricPattern.RegisterPattern("Happy pattern", "happiness", "moving average",
                                   "120000 300000 600000", "x x+10 x-20");
 ```
- Rule example for checking if the metric about quiz result is between 20 and 30 points (GT stands for Greater Than, LT stands for Less Than) between the third minute (after 180000 ms) and before the eighth minute (before 480000 ms):

 ```java
 { name=”A quiz points rule”, metric=”Quiz result”, feature=”none”,
   time=”GT(180000) LT(480000)”, values=”GT(20) AND LT(30)” }
 ```
 ```java
  testMetricPattern.RegisterPattern("A quiz points rule", "Quiz result", "none",
                                    "GT(180000) LT(480000)", "GT(20) AND LT(30)");
  ```
Note that using relative time moments after given moment of time t means checking for an occurrence of the specified metric/feature vector at every moment after t, when a new value of the metric is sent to the asset, i.e. the check is repeated always after t until the metric is deregistered for monitoring. If the game developer needs to specify a cycling check of a rule for given time period, he/she can do it by using a cycle construction as follows:

`Rule example for starting to check each one minute 10 min after the start of the game within the next 5 min for the condition if the number of shots metric is greater than 100:`

```java
{ name=”A shot rule”, metric=”No of shots”, feature=”none”,
  cycletime=”GT(600000) 6000 LT(900000)”, values=”GT(100)” }
```

During playing time, monitoring of registered metric can be canceled by calling the  `public bool RegisterMetric(String metric)` method with the metric name, which will incur stopping the monitoring of this metric and evaluation of its features. For a monitored metric, any of its patterns or rules set for observing can be stopped by calling the `public bool UnregisterPattern(String patternName)` method.

## Unit tests
The source code of all unit tests can be found [here](https://github.com/ddessy/PlayerCentricRuleBasedAdaptation/tree/master/PlayerCentricRuleBasedAdaptationSource/UnitTestRuleBasedAdaptation). They can be executed with the test tool of the Visual Studio.

## Examples of Use in Games

The player-centric adaptation technology continues being very promising and attractive for both entertainment and applied video games. State current examples if it is already in use. Various event handlers can be developed for game adaptation based on changes in player’s character, namely:
- Player’s performance – including knowledge, motivation, skills and abilities
- Player’s affective state - emotions and arousal;
- Player’s style – such as killer/achiever/explorer/socializer of Bartle [11] or conqueror/manager/wanderer/participant styles of Bateman and Boon [5].

The player character is to be implicitly derived during the play process, instead of explicit ways like using self-reports. The asset can be used for adapting various features regarding game mechanics, scenarios, dynamics, or aesthetics, as programmed in the event handlers by the game developer.

## Value

It is not possible to predict all possible types of features to be adapted even for a single complex game. Therefore, for not losing generality and for preserving the blackbox communication (the asset has no information about the game logic, scenarios, dynamic variables, etc.), the game developer is free to overwrite the default event handler as he likes. Thus, the asset offers:
- Independence from game logic, dynamics, and scenarios – includes independence from the adaptivity scale (micro/macro adaptivity [1]);
- Independence from game engine used;
- Easy use and integration – no specific domain expertise is required to use the asset, however, the game developer should be able to define reasonable and consistent patterns and rules of metric variation. Thus, the asset can be used by trainers, pedagogues and psychologist for creating various adaptive applied games;
- Flexibility - patterns and rules of metric variation can be set and cancel for monitoring during run time;
- Monitoring of heterogeneous player-centric metric – asset user is able to specify various metric about player’s performance, emotional status and playing style. For example, for the playing style can be observed at given together several metric and specific playing style can be inferred based on their change.

## Dependencies
The asset can be used in isolations provided the player-centric metrics are known by the developer at run time. Otherwise, the game developers may use it together the Real-time Emotion Detection Asset or the RT Arousal Detection Using Galvanic Skin Response Asset, in order to provide emotion or arousal metric, respectively.

## Technical Details
The asset will have only a client component. Its run-time requirements are as follows:
- Windows Vista or higher;
- Microsoft .NET Framework.

Source project requirements:
- Microsoft Visual Studio 2013 Professional to import the solution (project);
- Support for C#.

## Licensing

No specific licensing is required. Licensed under the Apache License, Version 2.0.

## Reference
1. Kickmeier-Rust, M. D., Albert, D. Educationally adaptive: Balancing serious games. Int. Journal of Computer Science in Sport, 2012, 11(1), pp.1-10.
2. Tremblay, J., Bouchard, B., Bouzouane, A. Adaptive Game Mechanics for Learning Purposes-Making Serious Games Playable and Fun, Proc. of CSEDU (2), 2010.
3. Tijs, T., Brokken, D., IJsselsteijn, W. Creating an emotionally adaptive game. ICEC 2008, LNCS 5309, Springer Berlin Heidelberg. pp.122–133.
4. Fairclough, S., & Gilleade, K. Construction of the biocybernetic loop: a case study. Proc. of the 14th ACM Int. Conf. on Multimodal interaction, ACM, 2012, October, pp. 571-578.
5. Bateman, C., Boon, R. 21st Century Game Design, vol. 1. Charles River Media, Londan, 2005.
6. Murphy, C., Chertoff, D., Guerrero, M., Moffitt, K. Design Better Games: Flow, Motivation, and Fun. Design and Development of Training Games: Practical Guidelines from a Multidisciplinary Perspective, 2014, p.1773.
7. Sweetser, P., Johnson, D. M., Wyeth, P. Revisiting the GameFlow model with detailed heuristics. Journal : Creative Technologies, 2012 (3).
8. Rani P., Sarkar N., Liu C. Maintaining optimal challenge in computer games through real-time physiological feedback. Proc. of the 11th Int. Conf. on Human Computer Interaction, 2005, pp.184–192.
9. Grigore, O., Gavat, I., Cotescu, M., Grigore, C., 2008. Stochastic algorithms for adaptive lighting control using psycho-physiological features. Int. J. of Biology and Biomedical Engineering 2, pp.9–18.
10. Parnandi, A., Gutierrez-Osuna, R. A comparative study of game mechanics and control laws for an adaptive physiological game. J. on Multimodal User Interfaces, 2014, pp.1-12.
11. Bartle, R. Hearts, Clubs, Diamonds, Spades: Players Who suit MUDs, 1996, http://mud.co.uk/richard/hcds.htm
12. Bontchev, B. Methods of adaptation control by implicit derivation of the player character during the game play, Deliverable D3, Version 1.0, ADAPTIMES – WP3 – D3
