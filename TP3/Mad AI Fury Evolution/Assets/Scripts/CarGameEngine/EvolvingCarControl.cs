﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SimulationInfo 
{
	public GameObject sim;
	public Controller playerc;
	public int individual_index;

	public SimulationInfo(GameObject sim, Controller playerc, int individual_index){
		this.sim = sim;
		this.playerc = playerc;
		this.individual_index = individual_index;
	}

}

public class EvolvingCarControl : MonoBehaviour {

	// instances
	public static EvolvingCarControl instance = null;
	public Text infoText;
	public bool simulating = false;
	public GameObject simulationPrefab;
	public int nSims;
	public int seed;

	private MetaHeuristic metaengine;
	private GameObject bestSimulation;
	private List<SimulationInfo> simsInfo;
	private bool goNextGen = false;
	private int sims_done = 0;
	private int indiv_index = 0;
	private bool allFinished = false;


	void Awake(){
		// deal with the singleton part
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy (gameObject);    
		}

		DontDestroyOnLoad(gameObject);
		initMetaHeuristic ();
		BatchmodeConfig.HandleArgs (this, metaengine);
		Random.InitState (seed);
		init ();

	}
		
	void init(){
		createSimulationGrid (); 
		startSimulations ();
	}

	void initMetaHeuristic(){
		MetaHeuristic[] metaengines = this.GetComponentsInParent<MetaHeuristic> ();
		// check which one is active..
		foreach(MetaHeuristic tmp in metaengines){
			if(tmp.enabled){
				metaengine = tmp;
				metaengine.InitPopulation();
				break;
			}
		}
	}


	private void createSimulationGrid ()
	{
		simsInfo = new List<SimulationInfo> ();
		int ncols = metaengine.populationSize == 1 ? 1 : 5;
		float spacing = 1.0f / (float) ncols;
		float sim_height = 1f / (float) ncols;
		float start_x = 0.0f, start_y = 0.0f;

		for (int i = 0; i < nSims && i < metaengine.populationSize; i++,indiv_index++) {
			if (i > 0 && i % ncols == 0) {
				start_x = 0.0f;
				start_y += sim_height;
			}
			simsInfo.Add (createSimulation (i, new Rect (start_x, start_y, spacing, sim_height), indiv_index));
			start_x += spacing;
		}
			
		Time.timeScale = 24;
	}

	private SimulationInfo createSimulation(int sim_i, Rect location, int indiv_i)
	{
		GameObject sim = Instantiate (simulationPrefab, transform.position + new Vector3 (0, 0, (sim_i * 1000)), transform.rotation);
		Controller player_script = sim.GetComponentInChildren<Controller> ();
		player_script.GetComponentInChildren<Camera> ().rect = location;
		player_script.neuralController = metaengine.Population[indiv_i].getIndividualController();

		// handle destroy walls
		DestroyTrigger[] triggers = sim.transform.Find ("DeathWalls").gameObject.GetComponentsInChildren<DestroyTrigger> ();
		foreach (DestroyTrigger t in triggers) {
			t.car = player_script;
		}
			
		return new SimulationInfo (sim, sim.GetComponentInChildren<Controller> (), indiv_i);
	}


	private void startSimulations()
	{
		foreach (SimulationInfo info in simsInfo) {
			info.playerc.running = true;
		}
		simulating = true;
		sims_done = 0;
	}


	void Update () {

		if (!allFinished) {
			// evolve solution.. Simulate
			if (simulating) {
			
				for (int i = 0; i < simsInfo.Count; i++) {
					if (simsInfo [i] != null && !simsInfo [i].playerc.running && simsInfo [i].playerc.gameOver) { 

						// FITNESS ASSIGNEMENT 
						if (!metaengine.Population [simsInfo [i].individual_index].Evaluated) {
							metaengine.Population [simsInfo [i].individual_index].Fitness = simsInfo [i].playerc.GetScore ();
						} 
						//

						// end the simulation for indivividual at position i of the simulation grid
						Destroy (simsInfo [i].sim);
						// deploy another in its place
						if (indiv_index < metaengine.populationSize) {
							simsInfo [i] = createSimulation (i, new Rect (simsInfo [i].playerc.GetComponentInChildren<Camera> ().rect), indiv_index);
							simsInfo [i].playerc.running = true;
							indiv_index++;
						} else {
							simsInfo [i] = null;
						}
						sims_done++;
					}
				}

				infoText.text = "Generation: " + metaengine.generation + "/" + metaengine.numGenerations + "\nSimulation: " + sims_done + "/" + metaengine.populationSize + "\nCurrent Pop Avg Fitness: "+ metaengine.PopAvg + " Current Best: " +metaengine.GenerationBest.Fitness;
				if (sims_done == metaengine.populationSize) {
					// clear simsInfo array..
					simsInfo.Clear ();
					goNextGen = true;
					simulating = false;
				}
			}
		} else {
			//check if we are in headless mode
			if (BatchmodeConfig.batchmode) {
				Application.Quit ();
			}
			// display best..
			simulateBest ();

		}
	}

	public void FixedUpdate(){
		if (goNextGen) {
			goNextGen = false;
			if (metaengine.generation < metaengine.numGenerations -1) {

				// Perform an evolutionary algorithm step
				metaengine.Step ();
				// reset simulation grid variables
				sims_done = 0;
				indiv_index = 0;
				// Init grids and start simulations again
				init ();
			} else {
				if (!allFinished) {
					allFinished = true;
					simulating = false;
					metaengine.updateReport ();
					metaengine.dumpStats ();
				}
			}
		}

	}

	private void simulateBest(){
		infoText.text = "Best Individual Simulation. Fitness:"+metaengine.overallBest.Fitness;
		// show best.. in loop
		if (!simulating) {
			//Debug.Log (metaengine.overallBest.ToString ());
			//Debug.Log ("best fitness !" + metaengine.overallBest.Fitness);
			SimulationInfo info = createSimulation (0, new Rect (0.0f, 0.0f, 1f, 1f), 0);
			info.playerc.neuralController = metaengine.overallBest.getIndividualController ();
			info.playerc.running = true;
			bestSimulation = info.sim;

			Time.timeScale = 24;
			simulating = true;

		} else if (simulating) {

			if (!bestSimulation.GetComponentInChildren<Controller> ().running && bestSimulation.GetComponentInChildren<Controller> ().gameOver) {
				simulating = false;
				Destroy (bestSimulation);
			}
		}
	}

}
