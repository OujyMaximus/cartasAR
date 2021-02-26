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
    public GameObject cardPoser;
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

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings("VersionName");
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }


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
                        SetCardInTable,
                        SetOpositeCardInTable,
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

    public GameObject GetCardPrefab() => cardPrefab;

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
    public void ButtonSelectCardPress(bool cardSelected, List<GameObject> cardsInstantiated)
    {
        Vector3 cardPosition;
        Quaternion cardRotation;

        if (GameObject.FindGameObjectWithTag("CardPoser") != null)
            Destroy(GameObject.FindGameObjectWithTag("CardPoser"));

        if (cardSelected)
        {
            if (cardsInstantiated.Count > 0)
            {
                float newX, newZ;

                for(int i=0; i<cardsInstantiated.Count; i++)
                {
                    newX = -0.08f * i;
                    if (i == 0)
                        newZ = 0;
                    else
                        newZ = -0.05f;

                    cardPosition = new Vector3(newX, 0, newZ);
                    cardsInstantiated[i].transform.localPosition = cardPosition;
                    cardsInstantiated[i].SetActive(true);
                }
            }
            else
            {
                int numCartas;
                float newX, newZ;
                Vector3 actualCardPosition;
                List<Material> cardMaterials;

                cardMaterials = new List<Material>();
                cardMaterials.Add(greenMaterial);
                cardMaterials.Add(redMaterial);
                cardMaterials.Add(yellowMaterial);

                numCartas = 3;
                cardPosition = cardPositionGO.transform.position;
                cardRotation = cardPositionGO.transform.rotation;
                
                for (int i = 0; i < numCartas; i++)
                {
                    newX = -0.08f * i;
                    if (i == 0)
                        newZ = 0;
                    else
                        newZ = -0.05f;

                    actualCardPosition = new Vector3(newX, 0, newZ);

                    cardsInstantiated.Add(GameObject.Instantiate(cardPrefab, cardPosition, cardRotation));
                    cardsInstantiated[i].transform.SetParent(cardPositionGO.transform);
                    cardsInstantiated[i].transform.localPosition = actualCardPosition;
                    cardsInstantiated[i].GetComponentInChildren<MeshRenderer>().material = cardMaterials[i];
                }
            }
        }
        else
        {
            for(int i=0; i<cardsInstantiated.Count; i++)
            {
                cardsInstantiated[i].SetActive(false);
            }
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
        GameObject cardPoserInstantiated = GameObject.FindWithTag("CardPoser");

        if (placedTable != null)
        {
            Vector3 cardPoserPosition = cardPoserInstantiated.transform.position;

            float distanceCardPoserToPlayer = (cardPoserPosition - playerPosition).magnitude;

            if (distanceCardPoserToPlayer < 0.2)
            {
                playerDetect.SetCardInTable();
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
    public void SwitchCardInFront(List<GameObject> cardsInstantiated, int direction)
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

        for (int i = 0; i < cardsInstantiated.Count; i++)
        {
            if (cardsInstantiated[i].transform.localPosition.x < 0.01f && cardsInstantiated[i].transform.localPosition.x > -0.01f)
            {
                currentCardInFront = i;
                break;
            }
        }

        if (!(currentCardInFront == 0 && direction == -1)&&!(currentCardInFront == cardsInstantiated.Count-1 && direction == 1))
        {
            float newX, newY, newZ;

            for (int i = 0; i < cardsInstantiated.Count; i++)
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

    private float CalculateCardPoserPosition(GameObject cardPlacement)
    {
        float posX, cardPlacementNumChildren, multiplier;

        cardPlacementNumChildren = cardPlacement.transform.childCount;
        Debug.Log("NumChildren: " + cardPlacementNumChildren);

        if ((cardPlacementNumChildren-1) % 2 == 0)
            multiplier = -1;
        else
            multiplier = 1;

        posX = Mathf.CeilToInt((cardPlacementNumChildren - 1)/2);
        posX = posX * (0.12f * multiplier);

        return posX;
    }

    public void SelectCardInFront(List<GameObject> cardsInstantiated)
    {
        int currentCardInFront;
        GameObject cardPlacement, cardPoserInstantiated;

        currentCardInFront = 0;

        for (int i = 0; i < cardsInstantiated.Count; i++)
        {
            if (cardsInstantiated[i].transform.localPosition.x < 0.01f && cardsInstantiated[i].transform.localPosition.x > -0.01f)
            {
                currentCardInFront = i;
                break;
            }
        }

        {
            float posX;

            cardPlacement = GameObject.Find("CardPlacement");
            cardPoserInstantiated = GameObject.Instantiate(cardPoser);

            cardPoserInstantiated.transform.SetParent(cardPlacement.transform);
            cardPoserInstantiated.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

            posX = CalculateCardPoserPosition(cardPlacement);

            cardPoserInstantiated.transform.localPosition = new Vector3(posX, 0f, 0f);
        }

        for (int i = 0; i < cardsInstantiated.Count; i++)
        {
            if (i != currentCardInFront)
                cardsInstantiated[i].SetActive(false);
            else
            {
                float newX, newY, newZ;

                newX = cardsInstantiated[i].transform.localPosition.x;
                newY = -0.045f;
                newZ = +0.05f;

                Vector3 cardPosition = new Vector3(newX, newY, newZ);

                cardsInstantiated[i].transform.localPosition = cardPosition;
            }
        }
    }

    public void SetCardInTable(List<GameObject> cardsInstantiated)
    {
        int numCards, cardToSet;
        GameObject cardPlacement, cardSelected, cardPoser;

        cardPlacement = GameObject.Find("CardPlacement");
        cardPoser = GameObject.FindGameObjectWithTag("CardPoser");
        numCards = cardsInstantiated.Count;
        cardSelected = null;
        cardToSet = -1;

        for (int i = 0; i < numCards; i++)
        {
            float x = cardsInstantiated[i].transform.localPosition.x;

            if (x < 0.01f && x > -0.01f)
            {
                cardToSet = i;
                cardSelected = cardsInstantiated[i];
                cardsInstantiated.RemoveAt(i);
                break;
            }
        }

        cardSelected.transform.SetParent(cardPlacement.transform);
        cardSelected.transform.localPosition = cardPoser.transform.localPosition;
        cardSelected.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        Destroy(cardPoser);
        GameObject.Find("MenuController").GetComponent<MenuController>().SendCardSetInTable(cardToSet);
    }

    public void SetOpositeCardInTable(int id)
    {
        float posX;
        GameObject card, opositeCardPlacement;
        List<Material> cardMaterials;

        opositeCardPlacement = GameObject.Find("OpositeCardPlacement");
        card = GameObject.Instantiate(cardPrefab);

        {
            card.transform.SetParent(opositeCardPlacement.transform);
            card.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

            posX = CalculateCardPoserPosition(opositeCardPlacement);

            card.transform.localPosition = new Vector3(posX, 0f, 0f);
        }

        //Esto se quitara cuando se haga la BBDD de las cartas
        {
            cardMaterials = new List<Material>();
            cardMaterials.Add(greenMaterial);
            cardMaterials.Add(redMaterial);
            cardMaterials.Add(yellowMaterial);

            card.GetComponentInChildren<MeshRenderer>().material = cardMaterials[id];
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
