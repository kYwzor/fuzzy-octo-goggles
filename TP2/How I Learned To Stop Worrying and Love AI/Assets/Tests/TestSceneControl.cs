using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

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
	static private bool hasController = false;
	static List<SearchAlgorithm> allAlgorithms;
	static int currentAlgIndex = 0;

	void Awake(){
		DontDestroyOnLoad (gameObject);
		hasController = true;
	}

	void Start ()
    {
		allAlgorithms = GetComponents<SearchAlgorithm> ().ToList();;
		allAlgorithms.RemoveAll (s => s.isActiveAndEnabled == false);	//removes all not active
		if (allAlgorithms.Count == 0) {
			Debug.Log ("Add at least one active search algorithm to TestController!");
			return;
		}
		foreach (SearchAlgorithm alg in allAlgorithms) {
			alg.enabled = false;
		}

		SceneManager.sceneLoaded += OnSceneLoaded;
		ChangeScene ();
	}

	public static void ChangeScene() {
		if (noMoreScenes || !hasController)
			return;
		currentIndex++;
		if (currentIndex >= sceneList.Length) {
			currentAlgIndex++;
			currentIndex = 0;
			if (currentAlgIndex >= allAlgorithms.Count){
				Application.Quit (); // if we're in editor Quit is ignored
				noMoreScenes = true;
				Debug.Log ("All scenes and algorithms tested!");
				return;
			}
		}
		SceneManager.LoadScene (sceneList [currentIndex]);
	}

	static void OnSceneLoaded(Scene scene, LoadSceneMode mode){
		SearchAlgorithm copy;
		GameObject marvin;

		marvin = GameObject.FindGameObjectWithTag ("Unit");
		foreach (Component comp in marvin.GetComponents<SearchAlgorithm>()) {
			Destroy (comp);		
		}
		copy = (SearchAlgorithm) CopyComponent (allAlgorithms[currentAlgIndex], marvin);
		copy.enabled = true;
		marvin.GetComponent<Agent> ().autorun = true;
		marvin.GetComponent<Agent> ().skipAnimations = true;
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
}
