using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CameraDetection
{
    private ARRaycastManager arRaycastManager;
    private Camera arCamera;
    private TrackedPoseDriver aRTrackedPoseDriver;
    private GameObject tablePrefab;
    private GameObject placementIndicator;

    private PlayerDetect playerDetect;
    private GameObject spawnedObject;
    private Light boardLight;
    private GameObject boardLightGO;
    private GameObject spawnedLight;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    private Vector3 screenCenter;

    private bool isPlacementSelected;

    public CameraDetection(
                        Camera arCamera,
                        ARRaycastManager arRaycastManager,
                        TrackedPoseDriver aRTrackedPoseDriver,
                        GameObject tablePrefab,
                        GameObject placementIndicator,
                        PlayerDetect playerDetect)
    {
        this.arCamera = arCamera;
        this.arRaycastManager = arRaycastManager;
        this.aRTrackedPoseDriver = aRTrackedPoseDriver;
        this.tablePrefab = tablePrefab;
        this.placementIndicator = placementIndicator;
        this.playerDetect = playerDetect;
    }

    //----------------------------------------------

    public void StartCameraDetection()
    {
        screenCenter = new Vector3();
        isPlacementSelected = false;

        boardLightGO = new GameObject("BoardLight");
    }

    public void UpdateCameraDetection()
    {
        if(isPlacementSelected)
            PlaneDetection();

        //if (spawnedObject != null)
            //RotateTableToPlayer();
    }

    //----------------------------------------------
    //METHODS
    //----------------------------------------------

    public void PlaneDetection()
    {
        screenCenter = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        if (arRaycastManager.Raycast(screenCenter, s_Hits, TrackableType.Planes))
        {
            var hitPose = s_Hits[0].pose;

            if (spawnedObject != null)
                spawnedObject.SetActive(playerDetect.GetPlacementIndicatorStatus());

            placementIndicator.SetActive(playerDetect.GetPlacementIndicatorStatus());
            placementIndicator.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);

            
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if(touch.position.y > 150)
                    {
                        if (playerDetect.GetPlacementIndicatorStatus())
                        {
                            if (spawnedObject == null)
                            {
                                isPlacementSelected = false;
                                placementIndicator.SetActive(false);
                                playerDetect.SetPlacementIndicatorStatus(false);
                                spawnedObject = GameFunctions.Instantiate(tablePrefab, hitPose.position, hitPose.rotation);
                                
                                spawnedLight = GameFunctions.Instantiate(boardLightGO, new Vector3(hitPose.position.x, hitPose.position.y + 50, hitPose.position.z), Quaternion.Euler(new Vector3(50, -30, 0)));
                                boardLight = spawnedLight.AddComponent<Light>();
                                boardLight.type = LightType.Directional;
                                boardLight.intensity = 0.005f;
                                boardLight.color = new Color(255, 244, 214, 255);
                            }
                            else
                            {
                                isPlacementSelected = false;
                                placementIndicator.SetActive(false);
                                playerDetect.SetPlacementIndicatorStatus(false);
                                spawnedObject.transform.position = hitPose.position;
                                spawnedObject.transform.rotation = hitPose.rotation;
                                spawnedLight.transform.position = new Vector3(hitPose.position.x, hitPose.position.y + 50, hitPose.position.z);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    public void RotateTableToPlayer()
    {
        Quaternion newRotation;
        Vector3 tablePosition, devicePosition;

        tablePosition = new Vector3(spawnedObject.transform.position.x, 1, spawnedObject.transform.position.z);
        devicePosition = new Vector3(aRTrackedPoseDriver.transform.position.x, 1, aRTrackedPoseDriver.transform.position.z);
        
        newRotation = Quaternion.LookRotation(devicePosition - tablePosition);
        
        spawnedObject.transform.rotation = newRotation;
    }

    //----------------------------------------------

    public void ChangeIsPlacementSelectedStatus(bool status)
    {
        isPlacementSelected = status;
    }

    //----------------------------------------------

    public GameObject GetSpawnedObject() => spawnedObject;

    //----------------------------------------------

    public void SetIsPlacementSelected(bool status) => isPlacementSelected = status;
}