using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {

    public Text levelNumber;

    public void SelectLevel()
    {
        StageManager.Instance().LevelNumber = int.Parse(levelNumber.text);
        GameObject menu = GameObject.Find("Menu Manager");
        menu.GetComponent<Menu>().Play();
    }
}
