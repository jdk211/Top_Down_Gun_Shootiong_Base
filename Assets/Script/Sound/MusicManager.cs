using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

    public AudioClip mainTheme;
    public AudioClip menuTheme;

    string sceneName;

    void Start()
    {
        OnLevelWasLoaded(0); // 처음 씬에서는 OnLevelWasLoaded가 실행 안되므로 Start함수에서 한번 실행한다.(첫 씬에도 적용하고 싶으면)
    }

    private void OnLevelWasLoaded(int sceneIndex) // scene이 변경될 경우 실행
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if(newSceneName != sceneName)
        {
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f); // 0.2초 후 실행
        }
    }

    void PlayMusic()
    {
        AudioClip clipToPlay = null;

        if(sceneName == "Menu")
        {
            clipToPlay = menuTheme;
        }
        else if(sceneName == "Main")
        {
            clipToPlay = mainTheme;
        }

        if(clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }
    }
}
