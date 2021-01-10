using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageManager : MonoBehaviour
{
    #region["Variables"]

    [SerializeField]
    private GameObject[] arObjectsToPlace;
    private GameObject spawnedObject;

    private ARTrackedImageManager m_TrackedImageManager;

    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();

    private TrackedPoseDriver trackedPoseDriver;
    
    public Text imagePositionText;
    public Text devicePositionText;
    public Text imageLocalPositionText;

    #endregion

    void Awake()
    {
        m_TrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        trackedPoseDriver = FindObjectOfType<TrackedPoseDriver>();

        foreach (GameObject arObject in arObjectsToPlace)
        {
            GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
            newARObject.SetActive(false);
            newARObject.name = arObject.name;
            arObjects.Add(arObject.name, newARObject);
        }
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            arObjects[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage)
    {
        AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position, trackedImage.transform.localScale, trackedImage.transform.localPosition, trackedImage.transform.rotation);
    }

    void AssignGameObject(string name, Vector3 newPosition, Vector3 imageScale, Vector3 localPosition, Quaternion rotation)
    {
        if (arObjectsToPlace != null)
        {
            /*
            GameObject goARObject = arObjects[name];
            goARObject.SetActive(true);
            goARObject.transform.position = newPosition;
            goARObject.transform.localScale = imageScale;
            */

            if (spawnedObject == null)
                spawnedObject = Instantiate(arObjectsToPlace[0], newPosition, rotation);
            else
                spawnedObject.transform.SetPositionAndRotation(newPosition, rotation);

            foreach (GameObject go in arObjects.Values)
            {
                imagePositionText.text = "Image Position: " + newPosition;

                imageLocalPositionText.text = "Image Local Position: " + localPosition;

                devicePositionText.text = "Pose Driver Position: " + trackedPoseDriver.transform.position;
            }
        }
    }
}
