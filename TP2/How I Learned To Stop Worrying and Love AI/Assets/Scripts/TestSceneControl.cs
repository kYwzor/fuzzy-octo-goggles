using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneControl : MonoBehaviour {

    private int flag = 1;
    private string[] sceneList =
        { "jumbomap",
            "Labyrinth",
            "MediumMap",
            "Small",
            "SmallLabyrinth",
            "twoRocksOneBrain",
            "urock" };
    private string nextScene = "SmallLabyrinth";
    public GameObject marvin;
	// Use this for initialization
	void Start ()
    {
        for(int i = 0; i < sceneList.Length; i++)
        {
            if(SceneManager.GetActiveScene().name == sceneList[i])
            {
                if (i + 1 < sceneList.Length)
                {
                    flag = 1;
                    nextScene = sceneList[i + 1];
                    break;
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (marvin.GetComponent<Agent>().fullStop)
        {
            if (flag == 1)
                SceneManager.LoadScene(nextScene);
            else
                Application.Quit();
        }
	}
}
