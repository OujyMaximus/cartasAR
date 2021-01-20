using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }
    public GameObject placementIndicator;

    private Vector3 screenCenter;

    public GameObject button;

    public GameObject InteractionGameObject;
    private PlayerDetect playerDetect;

    private Vector2 touchPosition;

    [SerializeField]
    private Camera arCamera;

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        buttonInteraction = button.GetComponent<ButtonInteraction>();
        screenCenter = new Vector3();
        playerDetect = InteractionGameObject.GetComponent<PlayerDetect>();
    }

    void Update()
    {
        screenCenter = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        if (m_RaycastManager.Raycast(screenCenter, s_Hits, TrackableType.Planes))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;
            var cameraForward = Camera.current.transform.forward;

            if (spawnedObject != null)
                spawnedObject.SetActive(buttonInteraction.isActive);

            placementIndicator.SetActive(buttonInteraction.isActive);
            placementIndicator.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    touchPosition = touch.position;
                    Debug.Log("TouchPosition: " + touchPosition);

                    if (buttonInteraction.isActive)
                    {
                        if (spawnedObject == null)
                        {
                            spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                            playerDetect.isPlaced = true;
                        }
                        else
                        {
                            spawnedObject.transform.position = hitPose.position;
                            spawnedObject.transform.rotation = hitPose.rotation;
                        }
                    }
                }

                if(touch.phase == TouchPhase.Moved)
                {
                    if (touch.position.x >= (touchPosition.x+150))
                    {
                        Debug.Log("Pa la izquierda");
                    }
                    if (touch.position.x <= (touchPosition.x-150))
                    {
                        Debug.Log("Pa la derecha");
                    }
                }

                if (touch.phase == TouchPhase.Ended)
                {

                }
            }
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
    ButtonInteraction buttonInteraction;
}