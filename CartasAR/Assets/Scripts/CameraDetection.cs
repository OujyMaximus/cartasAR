using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CameraDetection : MonoBehaviour
{
    private ARRaycastManager arRaycastManager;
    private GameObject tablePrefab;
    private GameObject placementIndicator;

    private PlayerDetect playerDetect;
    private GameObject spawnedObject;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    private Vector3 screenCenter;

    private bool isPlacementSelected;

    public CameraDetection(
                        ARRaycastManager arRaycastManager,
                        GameObject tablePrefab,
                        GameObject placementIndicator,
                        PlayerDetect playerDetect)
    {
        this.arRaycastManager = arRaycastManager;
        this.tablePrefab = tablePrefab;
        this.placementIndicator = placementIndicator;
        this.playerDetect = playerDetect;
    }

    public void AwakeCameraDetection()
    {
        screenCenter = new Vector3();
        isPlacementSelected = true;
    }

    public void UpdateCameraDetection()
    {
        if(isPlacementSelected)
            PlaneDetection();
    }

    public void PlaneDetection()
    {
        screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

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
                    if (playerDetect.GetPlacementIndicatorStatus())
                    {
                        if (spawnedObject == null)
                        {
                            spawnedObject = Instantiate(tablePrefab, hitPose.position, hitPose.rotation);
                        }
                        else
                        {
                            spawnedObject.transform.position = hitPose.position;
                            spawnedObject.transform.rotation = hitPose.rotation;
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

    public void ChangeIsPlacementSelectedStatus(bool status)
    {
        isPlacementSelected = status;
    }

    public GameObject GetSpawnedObject() => spawnedObject;
}