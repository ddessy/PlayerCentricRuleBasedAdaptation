using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOnClick : MonoBehaviour {

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MiniGame");
    }
}
