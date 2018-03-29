using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Agent : MonoBehaviour {

	enum Energies {Force, Expanded};


	public int totalEnergy = 5;
	public float currentForce;
	public float currentEnergyExpanded;
	public bool reachedTargetPoint;
	public Color agentColor = Color.white;
	public List<GameObject> targets;
	public bool debugPath = true;
	public bool debugExpandedNodes = true;
	public GameObject healthExpandedBar;
	public GameObject healthForce;
	public float regenRate = 10;
	public int nodesExpanded = 0;
	public int nodesVisited = 0;
	public int pathCost = 0;



	private Text uniText; 
	private GameObject currentTarget;
	private SearchAlgorithm search;
	private bool moveToNext = false;
	private bool isMoving;
	private bool isDead;
	private bool isAtTarget;
	private int updateForceInterval;
	private int updateEnergyExpandedInterval;
	private int currentCost = 0;
	private bool agentRunning = false;
	protected List<Node> path = null;
	[HideInInspector]protected bool algorithmFinished = false;

	public void Awake() {
		
	}
	// Use this for initialization
	void Start () {
		search = GetSearchAlgorithm();
		targets = GetAllTargets ();
		targets.Sort (GridMap.SortByName);
		UpdateStartTargetPositions ();


		Node start_pos = GridMap.instance.NodeFromWorldPoint (transform.position);
		transform.position = start_pos.worldPosition + new Vector3(0f,1f,0f);

		currentCost = 0;
		moveToNext = false;
		isMoving = false;
		isDead = false;
		isAtTarget = false;
		currentForce = totalEnergy;
		currentEnergyExpanded = totalEnergy;
		updateEnergyExpandedInterval = Mathf.Max( Mathf.RoundToInt (search.GetMaxNumberOfExpandedNodes() / totalEnergy),1);
		updateForceInterval = Mathf.Max( Mathf.RoundToInt (search.GetListSizeLimit() / (ulong) totalEnergy),1);
		UpdateEnergyBars (healthForce, (int) currentForce, Color.blue);
		UpdateEnergyBars (healthExpandedBar,(int) currentEnergyExpanded, Color.yellow);
		gameObject.GetComponent<Renderer> ().material.color = agentColor;

	}
	
	// Update is called once per frame
	void Update () {
		if (!isDead) {
			
			if (Input.GetKeyDown (KeyCode.Space) && !agentRunning) {
				agentRunning = true;
				search.StartRunning ();
				uniText = GameObject.Find (GridMap.instance.prefixUiText + this.name).GetComponent<Text> ();
				uniText.text = this.name + ": Searching... ";
				//print (GridMap.instance.NodeFromWorldPoint(search.startPos));
			}

			if (debugExpandedNodes && search.GetRunning () && !search.Finished ()) {
				GridMap.instance.ColorNodes (search.GetVisitedNodes (), agentColor);
				nodesExpanded = search.GetNumberOfNodesExpanded ();
				nodesVisited = search.GetVisitedNodes ().Count;
				pathCost = search.pathCost;
				uniText.text = this.name + ": Searching... ";
				uniText.text += "expanded: " + nodesExpanded + " visited: " + nodesVisited;
			}

			if (search.Finished () && !algorithmFinished) {
				uniText.text = this.name + ": Moving... ";
				nodesExpanded = search.GetNumberOfNodesExpanded ();
				nodesVisited = search.GetVisitedNodes ().Count;
				pathCost = search.pathCost;
				uniText.text += "expanded: " + nodesExpanded + " visited: " + nodesVisited + " pathcost: " + pathCost;
				if (path == null) {
					if (search.FoundPath ()) {
						path = search.RetracePath ();
						if (debugPath) {
							
							GridMap.instance.ColorNodes (path, agentColor);
						}
					} else {
						algorithmFinished = true;
					}
				} 

				if (targets.Count > 0 && moveToNext) {
					// clear visited nodes
					if (debugExpandedNodes) {
						GridMap.instance.ClearColorNodes (search.GetVisitedNodes ());
					}
					//move to next target
					GridMap.instance.ClearColorNode (GridMap.instance.NodeFromWorldPoint (search.targetPos));
					UpdateStartTargetPositions ();
					search.StartRunning ();

					path = null;
					currentCost = 0;
					moveToNext = false;
					isAtTarget = false;
				}
			}
			if (!isMoving) {
				UpdateEnergy ();
				if(isAtTarget && IsFullyLoaded())
					moveToNext = true;
			}

		}

	}

	public void FixedUpdate() {
		Time.timeScale = 0.1f;
		if(path != null) {
			Move ();
		}
		
	}

	/// <summary>
	/// Rotates the agent towards the next position.
	/// </summary>
	/// <param name="nextPos">Next position.</param>
	public void rotateAgent(Node nextPos)
	{
		if (nextPos.worldPosition.x > transform.position.x) {
			transform.forward = new Vector3 (0f, 0f, -1f);
		}else {
			if (nextPos.worldPosition.x < transform.position.x) {
				transform.forward = new Vector3 (0f, 0f, 1f);
			}
		}
		if (nextPos.worldPosition.z > transform.position.z){
			transform.forward = new Vector3 (1f, 0f, 0f);
		}else{
			if (nextPos.worldPosition.z < transform.position.z)
				transform.forward = new Vector3 (-1f, 0f, 0f);
		}
	}
	 
	public void Move() {
		if (path.Count > 0) {
			isMoving = true;
			GridMap.instance.ClearColorNode (GridMap.instance.NodeFromWorldPoint (search.startPos));
			Node nextPos = path [0];
			rotateAgent (nextPos);
			transform.position = nextPos.worldPosition + new Vector3(0, 1f, 0);
			if (debugPath) {
				//Destroy (GameObject.Find (name + " " + nextPos.gridX + "," + nextPos.gridY));
				GridMap.instance.ClearColorNode (nextPos);
			}
			currentCost += nextPos.gCost;
			path.Remove (nextPos);
		} else {
			if (isMoving) {
				// just update once
				currentTarget.GetComponent<Renderer> ().material.color = new Color (
					currentTarget.GetComponent<Renderer> ().material.color.r - .5f,
					currentTarget.GetComponent<Renderer> ().material.color.g - .5f,
					currentTarget.GetComponent<Renderer> ().material.color.b - .5f
				);
			}
			isAtTarget = true;
			isMoving = false;
		}
	}

	void UpdateStartTargetPositions() {
		search.startPos = transform.position;
		GridMap.instance.ColorNode (GridMap.instance.NodeFromWorldPoint (search.startPos), agentColor, 1);
		currentTarget = (GameObject) targets[0];
		targets.RemoveAt (0);
		search.targetPos = currentTarget.transform.position;
		//currentTarget.GetComponent<Renderer> ().material.color = Color.red; 
		currentTarget.GetComponent<Renderer> ().material.color = new Color (
			currentTarget.GetComponent<Renderer> ().material.color.r + .15f,
			currentTarget.GetComponent<Renderer> ().material.color.g + .15f,
			currentTarget.GetComponent<Renderer> ().material.color.b + .15f
		);
		GridMap.instance.ColorNode (GridMap.instance.NodeFromWorldPoint (search.targetPos), Color.black, 1);
	}

	void UpdateEnergy() {
		if (search.Finished() && isAtTarget && !isMoving) {
			if (currentEnergyExpanded < totalEnergy) {
				float increment = ((float)totalEnergy / search.GetNumberOfNodesExpanded () * regenRate);
				currentEnergyExpanded = Mathf.Min((currentEnergyExpanded + increment), totalEnergy);
			}
				
			if (currentForce < totalEnergy) {
				float increment = ((float)totalEnergy / search.GetMaxListSize () * regenRate);
				currentForce = Mathf.Min((currentForce + increment), totalEnergy);
			}
		} else {
			if (search.GetRunning ()) {
				currentEnergyExpanded = totalEnergy - (int)( search.GetNumberOfNodesExpanded () / updateEnergyExpandedInterval);
				currentForce = totalEnergy - ((int) search.GetMaxListSize() / updateForceInterval);
					
				if (currentEnergyExpanded <= 0 || currentForce <= 0) {
					search.setRunning (false);
					isDead = true;
					MakeDead ((currentEnergyExpanded <= 0) ? Energies.Expanded : Energies.Force);

				}
			}
		}
		UpdateEnergyBars (healthForce, (int) currentForce, Color.blue);
		UpdateEnergyBars (healthExpandedBar, (int) currentEnergyExpanded, Color.yellow);
	}

	void MakeDead(Energies energy) {
		gameObject.GetComponent<Renderer> ().material.color = Color.black;
		uniText.text = this.name + ": Dead... ";
		nodesExpanded = search.GetNumberOfNodesExpanded ();
		ulong nodesOnList = search.GetMaxListSize();
		pathCost = search.pathCost;
		if(energy == Energies.Expanded)
			uniText.text += "expanded: " + nodesExpanded + " >= "+ search.maxNumberOfExpanded + "(maxNumberOfExpansions)";
		else
			uniText.text += "onlist: " + nodesOnList + " >= " + search.GetListSizeLimit() + "(GetListSizeLimit)";
	}

	bool IsFullyLoaded() {
		return currentEnergyExpanded == totalEnergy && currentForce == totalEnergy;
	}


	void UpdateEnergyBars(GameObject component, int energyValue, Color color) {
		TextMesh tm = component.GetComponent<TextMesh> ();
		tm.text = new string ('-', Mathf.Max(energyValue,0));
		tm.GetComponent<Renderer> ().material.color = color;
	}
		

	public SearchAlgorithm GetSearchAlgorithm() {
		Component[] allAlgorithms = GetComponents<SearchAlgorithm> ();
		SearchAlgorithm firstActiveAlgorithm = null;
		foreach (SearchAlgorithm alg in allAlgorithms)
		{
			if (alg.isActiveAndEnabled) {
				firstActiveAlgorithm = alg;
				break;
			}
		}
		return firstActiveAlgorithm;
	}

	public List<GameObject> GetAllTargets()
	{
		return new List<GameObject> (GameObject.FindGameObjectsWithTag ("Target"));
	}




}
