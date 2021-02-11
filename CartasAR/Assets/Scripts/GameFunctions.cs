using System.Collections;
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

        playerDetect = new PlayerDetect(
                        ButtonPlacementPress,
                        ButtonSelectCardPress,
                        CheckPlayerTableDistance,
                        CheckCardSwitching);

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
    //BUTTONS METHODS
    //----------------------------------------------

    public void ButtonPlacementPress(bool isActive, Image image)
    {
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
    public GameObject[] ButtonSelectCardPress(bool cardSelected)
    {
        GameObject[] cardsInstantiated = new GameObject[3];
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

        return cardsInstantiated;
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
                }
                if (touch.position.x <= (touchPosition.x - 150))
                {
                    Debug.Log("Pa la derecha");
                }
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
