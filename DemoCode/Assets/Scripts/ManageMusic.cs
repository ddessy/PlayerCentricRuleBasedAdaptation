using UnityEngine;

public class ManageMusic : MonoBehaviour {

    private static ManageMusic _instance;

    public static ManageMusic instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ManageMusic>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            Debug.Log("Null");
            //If I am the first instance, make me the Singleton
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {

            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
            {
                Play();
                Debug.Log("IsnotNull");
                Destroy(this.gameObject);
            }
        }

    }
    public void Update()
    {
        if (this != _instance)
        {
            _instance = null;
        }
    }

    public void Play()
    {
        GameObject soundObject = GameObject.Find("Audio Source");
        AudioSource audioSource = soundObject.GetComponent<AudioSource>();
        audioSource.Play();
        //this.gameObject.GetComponent<AudioSource>().Play();
    }


}
