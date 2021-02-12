﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GameFunctions : MonoBehaviour
{
    #region Scripts variables
    private CameraDetection cameraDetection;
    private PlayerDetect playerDetect;
    #endregion

    #region Scene GameObject variables
    public GameObject placementIndicator;
    public GameObject cardPrefab;
    private GameObject arSessionOrigin;
    public GameObject arCameraGO;
    private Camera arCamera;
    private TrackedPoseDriver aRTrackedPoseDriver;
    public GameObject cardPositionGO;
    #endregion

    #region PlayerDetect variables
    private Vector3 playerPosition;
    private GameObject placedTable;
    private float distanceTablePlayer;
    private Vector3 placedTablePosition;
    private bool isPlaced;

    public Material iceMaterial;
    #endregion

    #region CameraDetection variables
    private ARRaycastManager arRaycastManager;
    public GameObject tablePrefab;
    #endregion

    private void Start()
    {
        arSessionOrigin = this.gameObject;
        arCamera = arCameraGO.GetComponent<Camera>();
        aRTrackedPoseDriver = arCameraGO.GetComponent<TrackedPoseDriver>();
        Button[] buttons = FindObjectsOfType<Button>();

        playerDetect = new PlayerDetect(
                        ButtonPlacementPress,
                        ButtonSelectCardPress,
                        CheckPlayerTableDistance,
                        CheckCardSwitching,
                        SwitchCardInFront,
                        buttons);

        playerDetect.AwakePlayerDetect();

        arRaycastManager = arSessionOrigin.GetComponent<ARRaycastManager>();

        cameraDetection = new CameraDetection(
                        arRaycastManager,
                        tablePrefab,
                        placementIndicator,
                        playerDetect);

        cameraDetection.AwakeCameraDetection();
    }

    private void Update()
    {
        playerDetect.UpdatePlayerDetect();
        cameraDetection.UpdateCameraDetection();
    }

    //----------------------------------------------
    //GENERAL METHODS
    //----------------------------------------------

    public GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject instance;

        instance = Instantiate(prefab, position, rotation);

        return instance;
    }

    //----------------------------------------------
    //BUTTONS METHODS
    //----------------------------------------------

    public void ButtonPlacementPress(bool isActive, Button button)
    {
        Image image = button.gameObject.GetComponent<Image>();

        cameraDetection.ChangeIsPlacementSelectedStatus(isActive);
        if (isActive)
        {
            image.color = new Color(0.2926f, 1f, 0.033f, 1f);
        }
        else
        {
            image.color = new Color(1f, 0.3537f, 0.3537f, 1f);
        }
    }

    //----------------------------------------------

    //Este metodo se activa al pulsar el boton de mazo y indica si hay que actualizar la posición de las cartas
    public void ButtonSelectCardPress(bool cardSelected, GameObject[] cardsInstantiated)
    {
        Vector3 cardPosition;
        Quaternion cardRotation;

        if (cardSelected)
        {
            cardPosition = cardPositionGO.transform.position;
            cardRotation = cardPositionGO.transform.rotation;

            cardsInstantiated[0] = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
            cardsInstantiated[0].transform.SetParent(cardPositionGO.transform);

            Vector3 cardPosition2 = new Vector3(cardsInstantiated[0].transform.localPosition.x - 0.08f, cardsInstantiated[0].transform.localPosition.y, cardsInstantiated[0].transform.localPosition.z - 0.05f);

            cardsInstantiated[1] = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
            cardsInstantiated[1].transform.SetParent(cardPositionGO.transform);
            cardsInstantiated[1].transform.localPosition = cardPosition2;

            Vector3 cardPosition3 = new Vector3(cardsInstantiated[0].transform.localPosition.x - (0.08f * 2), cardsInstantiated[0].transform.localPosition.y, cardsInstantiated[0].transform.localPosition.z - 0.05f);

            cardsInstantiated[2] = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
            cardsInstantiated[2].transform.SetParent(cardPositionGO.transform);
            cardsInstantiated[2].transform.localPosition = cardPosition3;
        }
        else
        {
            if (cardsInstantiated[0] != null)
                Destroy(cardsInstantiated[0]);
            if (cardsInstantiated[1] != null)
                Destroy(cardsInstantiated[1]);
            if (cardsInstantiated[2] != null)
                Destroy(cardsInstantiated[2]);
        }
    }

    //----------------------------------------------
    //PLAYER ENVIRONMENT METHODS
    //----------------------------------------------

    //Funcion para comprobar la distancia entre el jugador y el tablero, se ejecutara en el Update de PlayerDetect
    public void CheckPlayerTableDistance()
    {
        playerPosition = aRTrackedPoseDriver.transform.position;

        if (isPlaced)
        {
            placedTable = cameraDetection.GetSpawnedObject();
            placedTablePosition = placedTable.transform.position;

            distanceTablePlayer = (placedTablePosition - playerPosition).magnitude;

            if (distanceTablePlayer < 0.5)
            {
                placedTable.GetComponent<MeshRenderer>().material = iceMaterial;
            }
        }
    }

    //----------------------------------------------
    //CARDS METHODS
    //----------------------------------------------

    //Esta funcion comprueba si el jugador esta arrastrando el dedo hacia la derecha o la izquierda si las cartas estan seleccionadas
    public void CheckCardSwitching(Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
                playerDetect.SetTouchPosition(touch.position);

            if (touch.phase == TouchPhase.Moved)
            {
                if (touch.position.x >= (touchPosition.x + 150))
                {
                    Debug.Log("Pa la izquierda");
                    playerDetect.SwitchCardInFront(-1);
                }
                if (touch.position.x <= (touchPosition.x - 150))
                {
                    Debug.Log("Pa la derecha");
                    playerDetect.SwitchCardInFront(1);
                }
            }
        }
    }

    //Este metodo se encarga de mover las cartas cuando se detecta un swipe del jugador, direccion sera 1 si va hacia la izquierda y -1 si es hacia la derecha
    public void SwitchCardInFront(GameObject[] cardsInstantiated, int direction)
    {
        int currentCardInFront;
        float movingDistance;

        currentCardInFront = 0;
        movingDistance = 0.08f * direction;

        if (!(currentCardInFront == 0 && direction == -1)&&!(currentCardInFront == cardsInstantiated.Length-1 && direction == 1))
        {
            /*
            for (int i = 0; i < cardsInstantiated.Length; i++)
            {
                if (cardsInstantiated[i].transform.localPosition.x == 0)
                {
                    currentCardInFront = i;
                    break;
                }
            }*/

            float newX, newY, newZ;

            for (int i = 0; i < cardsInstantiated.Length; i++)
            {
                newX = cardsInstantiated[i].transform.localPosition.x + movingDistance;

                Debug.Log("NewX: " + newX);

                if (newX < 0.01f && newX > -0.01f)
                    newZ = 0f;
                else
                    newZ = -0.05f;

                Debug.Log("NewY: " + newZ);

                newY = cardsInstantiated[i].transform.localPosition.y;

                Vector3 cardPosition = new Vector3(newX, newY, newZ);

                cardsInstantiated[i].transform.localPosition = cardPosition;
            }
        }
    }

    //----------------------------------------------
    //CAMERA DETECTION METHODS
    //----------------------------------------------

    public void PlaneDetection()
    {
        /*
         * 
         * BUSCAR FORMA DE TRASLADAR EL PLANE DETECTION AQUI Y REALIZAR LA LLAMADA EN CAMERADETECTION
         * 
         */
    }
}
