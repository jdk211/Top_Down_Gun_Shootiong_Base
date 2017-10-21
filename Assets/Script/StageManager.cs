using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour {
    
    #region Singleton
    private const string MANAGER_NAME = "StageManager";
    private static StageManager g_Instance;

    public static StageManager Instance()
    {
        GameObject obj;

        if(g_Instance == null)
        {
            obj = GameObject.Find(MANAGER_NAME);
            if(obj == null)
            {
                obj = new GameObject(MANAGER_NAME);
            }
            g_Instance = obj.GetComponent<StageManager>();
            if(g_Instance == null)
            {
                g_Instance = obj.AddComponent<StageManager>();
            }
        }

        return g_Instance;
    }
    #endregion

    private int levelNumber = 1; // default 1
    public int LevelNumber
    {
        get{ return levelNumber; }
        set{ levelNumber = value; }
    }

    private List<Wave> waves;
    public List<Wave> Waves
    {
        get { return waves; }
    }


    private void Awake()
    {
        if (g_Instance == null)
        {
            g_Instance = this;
        }
        else if (g_Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        CreateLevel();
        CreateLevelButton();
    }

    private void CreateLevel()
    {
        waves = new List<Wave>();

        List<Dictionary<string, object>> data = CSVReader.Read("Level");

        for (int i = 2; i < data.Count; i++)
        {
            Wave emptyWave = new Wave();
            emptyWave.level = (int)data[i]["Level"];
            emptyWave.infinite = (bool)data[i]["infinite"];
            emptyWave.enemyCount = (int)data[i]["enemyCount"];
            emptyWave.timeBetweenSpawns = (float)data[i]["timeBetweenSpawns"];
            emptyWave.moveSpeed = (float)data[i]["moveSpeed"];
            emptyWave.damage = (int)data[i]["damage"];
            emptyWave.enemyHealth = (int)data[i]["enemyHealth"];
            emptyWave.skinColor.r = (float)data[i]["skinColorR"];
            emptyWave.skinColor.g = (float)data[i]["skinColorG"];
            emptyWave.skinColor.b = (float)data[i]["skinColorB"];
            waves.Add(emptyWave);
        }
    }

    private void CreateLevelButton()
    {
        GameObject levelButton = Resources.Load("Prefabs/LevelButton") as GameObject;
        GameObject Content = GameObject.Find("Content");

        for(int i = 0; i < waves.Count; i++)
        {
            GameObject button = Instantiate(levelButton, Content.transform);
            button.GetComponent<LevelButton>().levelNumber.text = "" + (i + 1);

        }
    }
}

public class Wave
{
    public int level;
    public bool infinite;
    public int enemyCount;
    public float timeBetweenSpawns;

    public float moveSpeed;
    public int damage;
    public int enemyHealth;
    public Color skinColor;
}