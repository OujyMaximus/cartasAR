using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

public class PlayerDetect : MonoBehaviour
{
    public GameObject arCamera;
    TrackedPoseDriver aRTrackedPoseDriver;

    GameObject placedTable;
    public bool isPlaced;

    float distanceTablePlayer;

    public Material iceMaterial;

    public GameObject ARSessionOriginGO;
    PlaceOnPlane placeOnPlane;

    private Vector3 playerPosition;
    private Vector3 placedTablePosition;

    void Awake()
    {
        aRTrackedPoseDriver = arCamera.GetComponent<TrackedPoseDriver>();
        placeOnPlane = ARSessionOriginGO.GetComponent<PlaceOnPlane>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = aRTrackedPoseDriver.transform.position;
        //Debug.Log("PositionPlayer: " + aRTrackedPoseDriver.transform.position);
        //Debug.Log("Rotation: " + aRTrackedPoseDriver.transform.rotation.normalized);

        if (isPlaced) {
            placedTable = placeOnPlane.spawnedObject;
            placedTablePosition = placedTable.transform.position;

            distanceTablePlayer = (placedTablePosition - playerPosition).magnitude;

            //Debug.Log("DistanceTablePlayer: " + distanceTablePlayer);

            if(distanceTablePlayer < 0.5)
            {
                placedTable.GetComponent<MeshRenderer>().material = iceMaterial;
            }
        }
    }
}