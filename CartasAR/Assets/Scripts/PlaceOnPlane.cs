using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    Camera arCamera;
    ARRaycastManager m_RaycastManager;
    GameObject m_PlacedPrefab;
    GameObject placedPrefab;
    GameObject spawnedObject;
    GameObject placementIndicator;
    GameObject buttonPlacement;
    GameObject buttonCardSelect;
    GameObject InteractionGameObject;
    PlayerDetect playerDetect;
    ButtonInteraction buttonInteraction;
    ButtonInteraction buttonCard;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    Vector2 touchPosition;
    Vector3 screenCenter;

    public PlaceOnPlane(
                        GameObject m_PlacedPrefab,
                        GameObject placedPrefab,
                        GameObject spawnedObject,
                        GameObject placementIndicator,
                        GameObject buttonPlacement,
                        GameObject buttonCardSelect,
                        GameObject InteractionGameObject,
                        PlayerDetect playerDetect,
                        Camera arCamera)
    {
        this.m_PlacedPrefab = m_PlacedPrefab;
        this.placedPrefab = placedPrefab;
        this.spawnedObject = spawnedObject;
        this.placementIndicator = placementIndicator;
        this.buttonPlacement = buttonPlacement;
        this.buttonCardSelect = buttonCardSelect;
        this.InteractionGameObject = InteractionGameObject;
        this.playerDetect = playerDetect;
        this.arCamera = arCamera;
    }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        screenCenter = new Vector3();
    }

    /*
     * 
     * INTENTAR PASAR LA DETECCION DE GESTOS A PLAYERDETECT Y DEJAR AQUI SOLO LA DETECCION DE PLANOS
     * 
     */

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
                    if (buttonCard.GetCardSelectStatus())
                    {
                        if (touch.position.x >= (touchPosition.x + 150))
                        {
                            Debug.Log("Pa la izquierda");
                        }
                        if (touch.position.x <= (touchPosition.x - 150))
                        {
                            Debug.Log("Pa la derecha");
                        }
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
}