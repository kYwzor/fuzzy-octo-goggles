﻿using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;


[RequireComponent(typeof (CarController))]
public class Controller : MonoBehaviour
{
	public CarController m_Car; // the car controller we want to use

	[Header("Sensors")]
	public Vector3 frontSensorPosition = new Vector3(0, 1.0f, 2.0f);
	public float sideSensorPosition = .5f;
	public float sensorLength = 50f;
	public float angle = 30f;
	public float[] frontSensorValues;
	public int tookHit = 0;
	public bool gameOver = false;
	public bool running = false;

	private Vector3 startPos;
	private Vector3 previousPos;
	public GameObject[] checkpoints;
	private float checkPointRayAngle = 30f;


	// Available Information 
	public int numberOfLaps = 0;
	public int numberOfCheckpoints = 0;
	public float driveTime = 0;
	public float prevDriveTime = 0;
	public Vector3 prevPosition;
	public float deltaDistance= 0;
    public float deltaSum = 0;
    public float distanceTravelled = 0.0f;
    public float maxCheckpointDistance = 0.0f;
    public int prevCheckPoints = -1;
    public float prevCheckDistance = 0f;
	public float avgSpeed = 0.0f;
	public float maxSpeed = 0.0f;
	public float currentSpeed = 0.0f;
    public float fitCheck = 0.0f;
	public float distanceToNextCheckpoint = 0.0f;
	public float distanceToStartingPoint = 0.0f;
	public float currentDistance = 0.0f;
    public float totalDistanceToCheckpoint = 0;
	//

	public NeuralNetwork neuralController;

	private void Awake()
	{
		// get the car controller
		m_Car = GetComponent<CarController>();
		frontSensorValues = new float[3];
		startPos = m_Car.transform.position;
		previousPos = startPos;
		prevPosition = startPos;
        prevCheckDistance = 10000f;
			
	}


	private void FixedUpdate()
	{
		if (running) {
			//TODO: I think we can use this to speed things up
			Time.timeScale = 6;	
			// updating sensors
			SensorHandling ();

			// move the car
			float[] result = this.neuralController.process (frontSensorValues);
			float h = result [0];
			float v = result [1];
			m_Car.Move (h, v, v, 0);

			// updating race status
			updateRaceStatus ();

			//if we do not move for too long, we stop the simulation
			//or if we are simmulating for too long, we stop the simulation
			// You can modify this to change the length of the simulation.
      
            if(prevCheckPoints < numberOfCheckpoints)
            {
                totalDistanceToCheckpoint = distanceToNextCheckpoint;
                prevCheckPoints = numberOfCheckpoints;
            }
			if ((currentDistance <= 0.1 && driveTime > 10) || driveTime > 150) {
                wrapUp();
            }

            /*
            if (maxCheckpointDistance < distanceToNextCheckpoint) maxCheckpointDistance = distanceToNextCheckpoint;
            if (driveTime - prevDriveTime > 5)
            {
                prevDriveTime = driveTime;
                    //Debug.Log("Previous: " + prevCheckDistance + " " + prevCheckPoints +  " vs Current: " + distanceToNextCheckpoint + " " + numberOfCheckpoints);
                if (distanceToNextCheckpoint > prevCheckDistance && prevCheckPoints >= numberOfCheckpoints)
                    wrapUp();
                else
                {
                    prevCheckDistance = distanceToNextCheckpoint;
                    prevCheckPoints = numberOfCheckpoints;
                }
            }
            */
            //PEDRO:
            /*Old news
            //Different rules so we prevent stuckage
            //Every 10 seconds we check how much we have moved			
            if(driveTime - prevDriveTime > 10){
                deltaDistance =  Vector3.Distance(m_Car.transform.position, prevPosition);
                deltaSum += deltaDistance;
                prevPosition = m_Car.transform.position;
                prevDriveTime = driveTime;
            }
            // if distance is tiny we wrap up. This is used so my dudes don't get stuck
            if ((deltaDistance <= 1 && driveTime > 10) || driveTime > 300) {
                Debug.Log("Distance: " + deltaDistance + "\nTime: " + driveTime);
                wrapUp ();
            }
            */
        }
    }

	public void SensorHandling(){
		 RaycastHit hit;

		Vector3 sensorStartPos = transform.position;
		sensorStartPos += transform.forward * frontSensorPosition.z;
		sensorStartPos += transform.up * frontSensorPosition.y;

		// frontal
		if (Physics.Raycast (sensorStartPos, transform.forward, out hit, sensorLength)) {
			Debug.DrawLine (sensorStartPos, hit.point);
			frontSensorValues [0] = (sensorStartPos - hit.point).magnitude;
		} else {
			frontSensorValues [0] = 0;
		}


		// direita 
		sensorStartPos += transform.right * sideSensorPosition;
		if (Physics.Raycast (sensorStartPos, Quaternion.AngleAxis(angle, transform.up) * transform.forward, out hit, sensorLength)) {
			Debug.DrawLine (sensorStartPos, hit.point);
			frontSensorValues [1] = (sensorStartPos - hit.point).magnitude;
		}else {
			frontSensorValues [1] = 0;
		}

		// esquerda
		sensorStartPos -= transform.right * 2 * sideSensorPosition;
		if (Physics.Raycast (sensorStartPos, Quaternion.AngleAxis(-angle, transform.up) * transform.forward, out hit, sensorLength)) {
			Debug.DrawLine (sensorStartPos, hit.point);
			frontSensorValues [2] = (sensorStartPos - hit.point).magnitude;
		}else {
			frontSensorValues [2] = 0;
		}

	}


	public void updateRaceStatus()
	{
		driveTime += Time.deltaTime;
		currentDistance = Vector3.Distance(previousPos, m_Car.transform.position);
		distanceTravelled += currentDistance * (m_Car.isReversing ? -1 : 1);

		previousPos = m_Car.transform.position;

		distanceToStartingPoint = Vector3.Distance (m_Car.transform.position, startPos);

		RaycastHit hit;
		for (int i = 0; i * angle <= 360f; i++) {
			// check the distance to the nearest point of the checkpoint 
			if (Physics.Raycast (m_Car.transform.position, Quaternion.AngleAxis(-checkPointRayAngle * i, transform.up) * transform.forward, out hit, Mathf.Infinity, (1 << 2))) {
				distanceToNextCheckpoint = (m_Car.transform.position - hit.point).magnitude;
				break;
			}
		}

		// speed takes into account the direction of the car: if we are reversing it is negative
		avgSpeed += m_Car.CurrentSpeed * (m_Car.isReversing ? -1 : 1); 
		currentSpeed = m_Car.CurrentSpeed * (m_Car.isReversing ? -1 : 1);
		maxSpeed = (currentSpeed > maxSpeed ? currentSpeed : maxSpeed);


	}

	public float GetScore() {
        // Fitness function. You should modify this.  
        //Se pudermos aumentar o range usar isto
        float sensorDiff = Mathf.Abs(frontSensorValues[1] - frontSensorValues[2]);
        Debug.Log("=====");
        Debug.Log("Number of Checkpoints: " + numberOfCheckpoints);
        Debug.Log("Total Distance To Checkpoint: " + totalDistanceToCheckpoint);
        Debug.Log("Distance to next checkpoint: " + distanceToNextCheckpoint);
        Debug.Log("-----");
        Debug.Log("Average speed: " + avgSpeed);
        Debug.Log("Rate: " + (numberOfCheckpoints + ((totalDistanceToCheckpoint - distanceToNextCheckpoint) / totalDistanceToCheckpoint)));
        if(avgSpeed < 0 && (numberOfCheckpoints + ((totalDistanceToCheckpoint - distanceToNextCheckpoint) / totalDistanceToCheckpoint)) < 0)
        {

        Debug.Log("Fitness: " + -((numberOfCheckpoints + ((totalDistanceToCheckpoint - distanceToNextCheckpoint) / totalDistanceToCheckpoint))) * avgSpeed);
        Debug.Log("=====");
        return  -(numberOfCheckpoints + ((totalDistanceToCheckpoint - distanceToNextCheckpoint) / totalDistanceToCheckpoint)) * avgSpeed; 
        }
        else
        {
        Debug.Log("Fitness: " + ((numberOfCheckpoints + ((totalDistanceToCheckpoint - distanceToNextCheckpoint) / totalDistanceToCheckpoint))) * avgSpeed);
        Debug.Log("=====");
            
        return  (numberOfCheckpoints + ((totalDistanceToCheckpoint - distanceToNextCheckpoint) / totalDistanceToCheckpoint)) * avgSpeed; 
        }
        //experimentar average speed
        //return Mathf.Pow((numberOfCheckpoints + 1) * maxSpeed * driveTime * maxCheckpointDistance  / (distanceToNextCheckpoint + 1), numberOfLaps + 1);
	}

	public void wrapUp () {
		avgSpeed = avgSpeed / driveTime;
		gameOver = true;
		running = false;
	}		

	public void updateCheckPoints()
	{
		numberOfCheckpoints++;
		if (numberOfCheckpoints % checkpoints.Length == 0) {
			numberOfLaps++;
		}

	}


}