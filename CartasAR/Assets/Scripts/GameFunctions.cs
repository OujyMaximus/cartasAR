using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Linq;

public class GameFunctions : MonoBehaviour
{
    #region Scripts variables
    private CameraDetection cameraDetection;
    private PlayerDetect playerDetect;
    private MenuController menuController;
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
    private List<int> pyramidIndexes;
    private GameObject giveCardGO;
    private GameObject makePyramidGO;
    private GameObject flipCardGO;
    private GameObject finalRoundGO;
    #endregion

    #region PlayerDetect variables
    private bool isPlaced;
    private PhotonView photonView;
    #endregion

    #region CameraDetection variables
    private ARRaycastManager arRaycastManager;
    public GameObject tablePrefab;
    #endregion

    #region Card variables
    public List<Material> cardMaterialsToPlace;

    private Dictionary<Material, int> cardMaterials = new Dictionary<Material, int>();
    private Dictionary<Material, int> cardMaterialsPlayed = new Dictionary<Material, int>();
    #endregion

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
                        AddCardToDeck,
                        GiveCardToPlayer,
                        MakePyramid,
                        AddCardToPyramid,
                        FindCardToFlipInPyramid,
                        FlipCardInPyramid,
                        PrepareFinalRound,
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

        menuController = GameObject.Find("MenuController").GetComponent<MenuController>();

        giveCardGO = GameObject.Find("GiveCard");
        makePyramidGO = GameObject.Find("MakePyramid");
        flipCardGO = GameObject.Find("FlipCard");
        finalRoundGO = GameObject.Find("FinalRound");

        makePyramidGO.SetActive(false);
        flipCardGO.SetActive(false);
        finalRoundGO.SetActive(false);

        photonView = GetComponent<PhotonView>();

        for (int i = 0; i < cardMaterialsToPlace.Count; i++)
        {
            cardMaterials.Add(cardMaterialsToPlace[i], i);
        }

        pyramidIndexes = new List<int>();

        //DEBUGGING
#if !UNITY_EDITOR
        GameObject.Find("ButtonSelectCard").SetActive(false);
        GameObject.Find("ButtonSetCard").SetActive(false);
#endif
    }

    private void Update()
    {
        playerDetect.UpdatePlayerDetect();

        if(!menuController.GetIsMenuActive())
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

    //-----------------------------------------------------------------------------

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
            /*ESTO RULA DESCOMENTAR PARA PROBAR LAS CARTAS
            else
            {
                int numCartas;
                float newX, newZ;
                Vector3 actualCardPosition;

                numCartas = 10;
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

                    foreach (KeyValuePair<Material, int> kvp in cardMaterials)
                    {
                        if(kvp.Value == i)
                        {
                            Debug.Log("kvp.Value: " + kvp.Value);
                            cardsInstantiated[i].GetComponentInChildren<MeshRenderer>().material = kvp.Key;
                        }
                    }

                }
            }*/
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

        if (placedTable != null && cardPoserInstantiated != null)
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

    //-----------------------------------------------------------------------------

    /*private float CalculateCardPoserPosition(GameObject cardPlacement)
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
    }*/

    private GameObject CalculateCardPoserParent()
    {
        GameObject pyramid = GameObject.Find("Pyramid");

        if(pyramid != null)
        {
            for (int i = 9; i >= 0; i--)
            {
                GameObject auxCard;

                if(pyramid.transform.GetChild(i).transform.childCount > 0)
                {
                    auxCard = pyramid.transform.GetChild(i).transform.GetChild(0).gameObject;

                    if(auxCard.transform.localEulerAngles.x == 90f)
                    {
                        return auxCard.transform.parent.gameObject;
                    }
                }
            }
        }

        return null;
    }

    //-----------------------------------------------------------------------------

    public void SelectCardInFront(List<GameObject> cardsInstantiated)
    {
        int currentCardInFront;
        GameObject cardPoserInstantiated;

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
            System.Random rand = new System.Random();

            //TODO: Aqui habra que comprobar que si la posicion es zero no se pueda colocar la carta
            GameObject cardPoserParent;

            cardPoserParent = CalculateCardPoserParent();

            if(cardPoserParent != null)
            {
                cardPoserInstantiated = GameObject.Instantiate(cardPoser);
                cardPoserInstantiated.transform.SetParent(cardPoserParent.transform);
                cardPoserInstantiated.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                cardPoserInstantiated.transform.localPosition = Vector3.zero;
            }
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
                newZ = 0.03f;

                Vector3 cardPosition = new Vector3(newX, newY, newZ);

                cardsInstantiated[i].transform.localPosition = cardPosition;
            }
        }
    }

    //-----------------------------------------------------------------------------

    public void SetCardInTable(List<GameObject> cardsInstantiated)
    {
        int numCards, cardToSet;
        GameObject cardSelected, cardPoser;

        cardPoser = GameObject.FindGameObjectWithTag("CardPoser");
        numCards = cardsInstantiated.Count;
        cardSelected = null;
        cardToSet = -1;

        for (int i = 0; i < numCards; i++)
        {
            float x = cardsInstantiated[i].transform.localPosition.x;

            if (x < 0.01f && x > -0.01f)
            {
                Material material = cardsInstantiated[i].GetComponentInChildren<MeshRenderer>().material;

                foreach (KeyValuePair<Material, int> kvp in cardMaterialsPlayed)
                {
                    if (kvp.Key.mainTexture.name == material.mainTexture.name)
                    {
                        cardToSet = kvp.Value;
                        break;
                    }
                }

                if(cardToSet == -1)
                {
                    foreach (KeyValuePair<Material, int> kvp in cardMaterials)
                    {
                        if (kvp.Key.mainTexture.name == material.mainTexture.name)
                        {
                            cardToSet = kvp.Value;
                            break;
                        }
                    }
                }

                cardSelected = cardsInstantiated[i];
                cardsInstantiated.RemoveAt(i);
                break;
            }
        }

        System.Random rand = new System.Random();

        cardSelected.transform.SetParent(cardPoser.transform.parent);
        cardSelected.transform.localEulerAngles = new Vector3(90f, rand.Next(-10, 10), 0f);
        cardSelected.transform.localPosition = new Vector3(0f, 0.001f * (cardPoser.transform.parent.childCount-1), (float)(rand.NextDouble() * (0.005 - -0.005) + -0.005));
        Destroy(cardPoser);
        menuController.SendCardSetInTable(cardToSet);
    }

    //-----------------------------------------------------------------------------

    public void SetOpositeCardInTable(int id)
    {
        GameObject card;

        card = GameObject.Instantiate(cardPrefab);

        {
            GameObject cardPoserParent;
            System.Random rand = new System.Random();

            cardPoserParent = CalculateCardPoserParent();

            card.transform.SetParent(cardPoserParent.transform);
            card.transform.localEulerAngles = new Vector3(90f, rand.Next(-10, 10), 0f);
            card.transform.localPosition = new Vector3(0f, 0.001f * (cardPoserParent.transform.childCount - 1), (float)(rand.NextDouble() * (0.005 - -0.005) + -0.005));
        }

        {
            Material materialToSet = null;

            foreach (KeyValuePair<Material, int> kvp in cardMaterials)
            {
                if (kvp.Value == id)
                {
                    materialToSet = kvp.Key;
                }
            }

            if(materialToSet == null)
            {
                foreach (KeyValuePair<Material, int> kvp in cardMaterialsPlayed)
                {
                    if (kvp.Value == id)
                    {
                        materialToSet = kvp.Key;
                    }
                }
            }

            card.GetComponentInChildren<MeshRenderer>().material = materialToSet;
        }
    }

    //-----------------------------------------------------------------------------

    public void GiveCardToPlayer()
    {
        List<Material> materialsList = new List<Material>(cardMaterials.Keys);
        System.Random rand = new System.Random();

        Material randomMaterial = materialsList[rand.Next(materialsList.Count)];

        int index;
        cardMaterials.TryGetValue(randomMaterial, out index);

        cardMaterials.Remove(randomMaterial);
        cardMaterialsPlayed.Add(randomMaterial, index);

        menuController.GiveCardToPlayer(index);

        if (cardMaterialsPlayed.Count == (menuController.GetPlayerIds().Count * 4))
        {
            giveCardGO.SetActive(false);
            makePyramidGO.SetActive(true);
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

    //----------------------------------------------
    //PYRAMID METHODS
    //----------------------------------------------

    public void AddCardToDeck(List<GameObject> cardsInstantiated, int id)
    {
        bool cardRemoved;
        Material randomMaterial = null;

        foreach (KeyValuePair<Material, int> kvp in cardMaterials)
        {
            if (kvp.Value == id)
            {
                randomMaterial = kvp.Key;
            }
        }

        if (randomMaterial == null)
        {
            foreach (KeyValuePair<Material, int> kvp in cardMaterialsPlayed)
            {
                if (kvp.Value == id)
                {
                    randomMaterial = kvp.Key;
                    break;
                }
            }
        }

        cardRemoved = cardMaterials.Remove(randomMaterial);

        if(cardRemoved)
        {
            cardMaterialsPlayed.Add(randomMaterial, id);
        }

        int cardIndex = cardsInstantiated.Count - 1;
        float newZ;

        if (cardIndex == 0)
            newZ = 0;
        else
            newZ = -0.05f;

        Vector3 cardPosition = new Vector3(-0.08f * cardIndex, 0, newZ);

        GameObject newCard = GameObject.Instantiate(cardPrefab);
        newCard.transform.SetParent(cardPositionGO.transform);
        newCard.transform.localPosition = cardPosition;
        newCard.transform.rotation = cardPositionGO.transform.rotation;
        newCard.GetComponentInChildren<MeshRenderer>().material = randomMaterial;

        cardsInstantiated.Add(newCard);
    }

    //-----------------------------------------------------------------------------

    public void AddCardToPyramid(int id)
    {
        pyramidIndexes.Add(id);

        if (pyramidIndexes.Count == 10)
            AddCardsToPyramid(pyramidIndexes);
    }

    //-----------------------------------------------------------------------------

    public void AddCardsToPyramid(List<int> ids)
    {
        GameObject pyramid = GameObject.Find("Pyramid");

        for(int i = 0; i < pyramid.transform.childCount; i++)
        {
            GameObject cardToPlace = pyramid.transform.GetChild(i).gameObject;

            Material randomMaterial = null;

            foreach (KeyValuePair<Material, int> kvp in cardMaterials)
            {
                if (kvp.Value == ids[i])
                {
                    randomMaterial = kvp.Key;
                    break;
                }
            }

            if(randomMaterial == null)
            {
                foreach (KeyValuePair<Material, int> kvp in cardMaterialsPlayed)
                {
                    if (kvp.Value == ids[i])
                    {
                        randomMaterial = kvp.Key;
                        break;
                    }
                }
            }
            else
            {
                cardMaterialsPlayed.Add(randomMaterial, ids[i]);
                cardMaterials.Remove(randomMaterial);
            }


            GameObject newCard = GameObject.Instantiate(cardPrefab);
            newCard.transform.SetParent(cardToPlace.transform);
            newCard.transform.localPosition = Vector3.zero;
            newCard.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
            newCard.GetComponentInChildren<MeshRenderer>().material = randomMaterial;
        }
    }

    //-----------------------------------------------------------------------------

    public void MakePyramid()
    {
        System.Random rand = new System.Random();
        
        for (int i = 0; i < 10; i++)
        {
            List<Material> materialsList = new List<Material>(cardMaterials.Keys);

            Material randomMaterial = materialsList[rand.Next(materialsList.Count)];

            int index;
            cardMaterials.TryGetValue(randomMaterial, out index);

            cardMaterials.Remove(randomMaterial);
            cardMaterialsPlayed.Add(randomMaterial, index);

            menuController.AddCardToPlayersPyramid(index);
        }

        makePyramidGO.SetActive(false);
        flipCardGO.SetActive(true);
    }

    //-----------------------------------------------------------------------------

    public void FindCardToFlipInPyramid()
    {
        GameObject pyramid = GameObject.Find("Pyramid");

        for (int i = 0; i < 10; i++)
        {
            GameObject auxCard;

            auxCard = pyramid.transform.GetChild(i).gameObject;

            if(auxCard.transform.GetChild(0).localEulerAngles.x == 270f)
            {
                menuController.FlipCardToPlayersPyramid(i);
                
                if(i == 9)
                {
                    flipCardGO.SetActive(false);
                    finalRoundGO.SetActive(true);
                }

                break;
            }
        }
    }

    //-----------------------------------------------------------------------------

    public void FlipCardInPyramid(int id)
    {
        GameObject pyramid = GameObject.Find("Pyramid");

        GameObject cardToFlip;

        cardToFlip = pyramid.transform.GetChild(id).gameObject.transform.GetChild(0).gameObject;

        cardToFlip.transform.localEulerAngles = new Vector3(90, 0, 0);
    }

    //-----------------------------------------------------------------------------

    public void PrepareFinalRound()
    {

    }
}
