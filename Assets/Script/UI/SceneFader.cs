using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour {

    public Image fadePlane;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(Fade(new Color(0, 0, 0, 0.95f), Color.clear, 2.0f, false));
    }

    public void FadeTo(string sceneName)
    {
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.95f), 2.0f, true, sceneName));
    }

    IEnumerator Fade(Color from, Color to, float time, bool isChangeScene = true, string sceneName = "Main")
    {
        fadePlane.enabled = true;

        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;  
            fadePlane.color = Color.Lerp(from, to, percent);

            yield return null;
        }

        if (isChangeScene)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            fadePlane.enabled = false;
        }
    }
}
