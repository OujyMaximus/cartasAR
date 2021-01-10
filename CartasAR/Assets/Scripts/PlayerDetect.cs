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

    void Awake()
    {
        aRTrackedPoseDriver = arCamera.GetComponent<TrackedPoseDriver>();
        placeOnPlane = ARSessionOriginGO.GetComponent<PlaceOnPlane>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("PositionPlayer: " + aRTrackedPoseDriver.transform.position);

        if (isPlaced) {
            placedTable = placeOnPlane.spawnedObject;
            distanceTablePlayer = (placedTable.transform.position - aRTrackedPoseDriver.transform.position).magnitude;

            Debug.Log("DistanceTablePlayer: " + distanceTablePlayer);

            if(distanceTablePlayer < 0.5)
            {
                placedTable.GetComponent<MeshRenderer>().material = iceMaterial;
            }
        }
    }
}