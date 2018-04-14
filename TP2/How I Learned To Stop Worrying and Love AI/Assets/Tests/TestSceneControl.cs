using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneControl : MonoBehaviour {
	static private int currentIndex = -1;
    static private string[] sceneList =
        { "jumbomap",
            "Labyrinth",
            "MediumMap",
            "Small",
            "SmallLabyrinth",
            "twoRocksOneBrain",
            "urock" };
	static bool noMoreScenes = false;
	static private SearchAlgorithm sa = null;
	static private bool hasController = false;

	void Awake(){
		DontDestroyOnLoad (gameObject);
		hasController = true;
	}

	void Start ()
    {
		Component[] allAlgorithms = GetComponents<SearchAlgorithm> ();
		foreach (SearchAlgorithm alg in allAlgorithms) {
			if (alg.isActiveAndEnabled) {
				sa = alg;
				break;
			}
		}
		if (sa == null) {
			Debug.Log ("Add a search algorithm to TestController!");
			return;
		}
		sa.enabled = false;
		SceneManager.sceneLoaded += OnSceneLoaded;
		ChangeScene ();
	}

	public static void ChangeScene() {
		if (noMoreScenes || !hasController)
			return;
		currentIndex++;
		if (currentIndex >= sceneList.Length) {
			Application.Quit (); // if we're in editor Quit is ignored
			noMoreScenes = true;
			Debug.Log ("All scenes tested!");
			return;
		}
		SceneManager.LoadScene (sceneList [currentIndex]);
	}


	// http://answers.unity.com/answers/589400/view.html
	static Component CopyComponent(Component original, GameObject destination)
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = type.GetFields(); 
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}

	static void OnSceneLoaded(Scene scene, LoadSceneMode mode){
		SearchAlgorithm copy;
		GameObject marvin;

		marvin = GameObject.FindGameObjectWithTag ("Unit");
		foreach (Component comp in marvin.GetComponents<SearchAlgorithm>()) {
			Destroy (comp);		
		}
		copy = (SearchAlgorithm) CopyComponent (sa, marvin);
		copy.enabled = true;
		marvin.GetComponent<Agent> ().autorun = true;
		marvin.GetComponent<Agent> ().skipAnimations = true;
	}
}
