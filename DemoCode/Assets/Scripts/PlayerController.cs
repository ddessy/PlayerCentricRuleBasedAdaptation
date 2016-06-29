using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Assets.Rage.PlayerCentricRulePatternBasedAdaptationAsset;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public Text countText;
    public Text winText;
    public Text sizeText;
    public Text timerText;

    private Rigidbody rb;
    private int count;
    private float time;

    protected bool paused;

    PatternBasedAdaptationAsset testMetricPattern;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        SetSizeText();
        winText.text = "";
        timerText.text = "";
        paused = true;

        testMetricPattern = new PatternBasedAdaptationAsset();

        testMetricPattern.RegisterMetric("Number of hits");

        testMetricPattern.RegisterPattern("Change color in blue", "Number of hits", "none", "GT(1)", "2");
        testMetricPattern.RegisterPattern("Change color in blue2", "Number of hits", "none", "GT(1)", "10");
        testMetricPattern.RegisterPattern("Change color in yellow", "Number of hits", "none", "GT(1)", "3");
        testMetricPattern.RegisterPattern("Change color in green", "Number of hits", "none", "GT(1)", "4");
        testMetricPattern.RegisterPattern("Change color in green2", "Number of hits", "none", "GT(1)", "11");
        testMetricPattern.RegisterPattern("Change color in white", "Number of hits", "none", "GT(1)", "5");
        testMetricPattern.RegisterPattern("Change color in red", "Number of hits", "none", "GT(1)", "6");
        testMetricPattern.RegisterPattern("Change color in cyan", "Number of hits", "none", "GT(1)", "7");
        testMetricPattern.RegisterPattern("Change color in black", "Number of hits", "none", "GT(1)", "8");
        testMetricPattern.RegisterPattern("Change color in magenta", "Number of hits", "none", "GT(1)", "9");

        testMetricPattern.RegisterPattern("Change size", "Number of hits", "none", "GT(1)", "GT(0)");
    }

    void Update()
    {
        if(paused)
        {
            time += Time.deltaTime;

            float minutes = time / 60; //Divide the guiTime by sixty to get the minutes.
            float seconds = time % 60;//Use the euclidean division for the seconds.
            float fraction = (time * 100) % 100;

            //update the label value
            timerText.text = string.Format("Time: {0} ", time);
        }
    }

    void FixedUpdate()
    {
        if(paused) {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            rb.AddForce(movement * speed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();

            List<string> patterns = testMetricPattern.SetMetricValue("Number of hits", count);
            Debug.Log("count: "+ count);
            testMetricPattern.PatternEventHandler(patterns, gameObject);

            SetSizeText();

            Debug.Log("scale: "+ gameObject.transform.localScale.x.ToString());
            Debug.Log("patterns: "+ patterns.Count);
        }
    }

    void SetCountText()
    {
        countText.text = "Cubes hit: " + count.ToString();
        if (count >= 12)
        {
            winText.text = "You won!";
            paused = false;
        }
    }

    void SetSizeText()
    {
        sizeText.text = "Ball size: " + gameObject.transform.localScale.x.ToString();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 90, 70, 30), "Replay"))
        {
            SceneManager.LoadScene("MiniGame");
        }

        if (GUI.Button(new Rect(10, 130, 70, 30), "Stop"))
        {
            Object[] objects = FindObjectsOfType(typeof(GameObject));
            foreach (GameObject go in objects)
            {
                go.SendMessage("OnPauseGame", SendMessageOptions.DontRequireReceiver);
            }
        }

        if (GUI.Button(new Rect(10, 170, 70, 30), "Resume"))
        {
            Object[] objects = FindObjectsOfType(typeof(GameObject));
            foreach (GameObject go in objects)
            {
                go.SendMessage("OnResumeGame", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void OnPauseGame()
    {
        paused = false;
    }

    void OnResumeGame()
    {
        paused = true;
    }
}