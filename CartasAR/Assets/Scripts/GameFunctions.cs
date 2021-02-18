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
    private bool isPlaced;
    #endregion

    #region CameraDetection variables
    private ARRaycastManager arRaycastManager;
    public GameObject tablePrefab;
    #endregion

    public Material iceMaterial;
    public Material greenMaterial;
    public Material redMaterial;
    public Material yellowMaterial;

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
                        SelectCardInFront,
                        buttons);

        playerDetect.StartPlayerDetect();

        arRaycastManager = arSessionOrigin.GetComponent<ARRaycastManager>();

        cameraDetection = new CameraDetection(
                        arCamera,
                        arRaycastManager,
                        aRTrackedPoseDriver,
                        tablePrefab,
                        placementIndicator,
                        playerDetect);

        cameraDetection.StartCameraDetection();
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
            for(int i = 0; i < cardsInstantiated.Length; i++)
            {
                if (cardsInstantiated[i] != null)
                    Destroy(cardsInstantiated[i]);
            }

            cardPosition = cardPositionGO.transform.position;
            cardRotation = cardPositionGO.transform.rotation;

            cardsInstantiated[0] = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
            cardsInstantiated[0].transform.SetParent(cardPositionGO.transform);
            cardsInstantiated[0].GetComponentInChildren<MeshRenderer>().material = greenMaterial;

            Vector3 cardPosition2 = new Vector3(cardsInstantiated[0].transform.localPosition.x - 0.08f, cardsInstantiated[0].transform.localPosition.y, cardsInstantiated[0].transform.localPosition.z - 0.05f);

            cardsInstantiated[1] = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
            cardsInstantiated[1].transform.SetParent(cardPositionGO.transform);
            cardsInstantiated[1].transform.localPosition = cardPosition2;
            cardsInstantiated[1].GetComponentInChildren<MeshRenderer>().material = redMaterial;

            Vector3 cardPosition3 = new Vector3(cardsInstantiated[0].transform.localPosition.x - (0.08f * 2), cardsInstantiated[0].transform.localPosition.y, cardsInstantiated[0].transform.localPosition.z - 0.05f);

            cardsInstantiated[2] = GameObject.Instantiate(cardPrefab, cardPosition, cardRotation);
            cardsInstantiated[2].transform.SetParent(cardPositionGO.transform);
            cardsInstantiated[2].transform.localPosition = cardPosition3;
            cardsInstantiated[2].GetComponentInChildren<MeshRenderer>().material = yellowMaterial;

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
        Vector3 playerPosition = aRTrackedPoseDriver.transform.position;

        GameObject placedTable = cameraDetection.GetSpawnedObject();
        Vector3 placedTablePosition = placedTable.transform.position;

        float distanceTablePlayer = (placedTablePosition - playerPosition).magnitude;

        if (distanceTablePlayer < 0.5)
        {
            placedTable.GetComponentInChildren<MeshRenderer>().material = iceMaterial;
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

            if(touch.phase == TouchPhase.Ended)
            {
                if (touch.position.y > 200)
                {
                    if (touch.position.x >= (touchPosition.x + 150))
                    {
                        playerDetect.SwitchCardInFront(-1);
                    }
                    else if (touch.position.x <= (touchPosition.x - 150))
                    {
                        playerDetect.SwitchCardInFront(1);
                    }
                    else
                    {
                        playerDetect.SelectCardInFront();
                    }
                }
            }
        }
    }

    //Este metodo se encarga de mover las cartas cuando se detecta un swipe del jugador, direccion sera 1 si va hacia la izquierda y -1 si es hacia la derecha
    public void SwitchCardInFront(GameObject[] cardsInstantiated, int direction)
    {
        /*
         * 
         * BUGAZO: SI AL PULSAR EL BOTON PRIMERO LE DAS PARA IR A LA IZQUIERDA PETA BASTANTE LOCO, puede que sea por la variable currentCardInFront(?)
         * 
         */

        int currentCardInFront;
        float movingDistance;

        currentCardInFront = 0;
        movingDistance = 0.08f * direction;

        for (int i = 0; i < cardsInstantiated.Length; i++)
        {
            if (cardsInstantiated[i].transform.localPosition.x < 0.01f && cardsInstantiated[i].transform.localPosition.x > -0.01f)
            {
                currentCardInFront = i;
                break;
            }
        }

        if (!(currentCardInFront == 0 && direction == -1)&&!(currentCardInFront == cardsInstantiated.Length-1 && direction == 1))
        {
            float newX, newY, newZ;

            for (int i = 0; i < cardsInstantiated.Length; i++)
            {
                newX = cardsInstantiated[i].transform.localPosition.x + movingDistance;

                if (newX < 0.01f && newX > -0.01f)
                    newZ = 0f;
                else
                    newZ = -0.05f;

                newY = cardsInstantiated[i].transform.localPosition.y;

                Vector3 cardPosition = new Vector3(newX, newY, newZ);

                cardsInstantiated[i].transform.localPosition = cardPosition;
            }
        }
    }

    public void SelectCardInFront(GameObject[] cardsInstantiated)
    {
        int currentCardInFront;

        currentCardInFront = 0;

        for (int i = 0; i < cardsInstantiated.Length; i++)
        {
            if (cardsInstantiated[i].transform.localPosition.x < 0.01f && cardsInstantiated[i].transform.localPosition.x > -0.01f)
            {
                currentCardInFront = i;
                break;
            }
        }

        for(int i = 0; i < cardsInstantiated.Length; i++)
        {
            if (i != currentCardInFront)
                Destroy(cardsInstantiated[i]);
            else
            {
                float newX, newY, newZ;

                newX = cardsInstantiated[i].transform.localPosition.x;
                newY = -0.05f;
                newZ = +0.02f;

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
