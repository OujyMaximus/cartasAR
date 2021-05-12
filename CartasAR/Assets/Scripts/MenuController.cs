using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    //AppId: a7ca51f2-e429-4447-8e3d-c74ac3d4e958

    [SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject ConnectPanel;

    [SerializeField] private InputField CreateGameInput;
    [SerializeField] private Text createGameErrorText;
    [SerializeField] private InputField JoinGameInput;
    [SerializeField] private Text joinGameErrorText;

    private UnityAction<int> setCardInTable;
    private UnityAction<int> addCardToDeck;
    private UnityAction<int> addCardToPyramid;

    private PhotonView photonView;

    private bool isGameStarted;
    private bool isMenuActive;

    private int currentPlayerTurn;
    private List<int> playerIDs;

    private GameObject startGameGO;
    private GameObject giveCardGO;
    private GameObject makePyramidGO;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);
        isGameStarted = false;
        isMenuActive = true;
        currentPlayerTurn = 0;
        playerIDs = new List<int>();

        startGameGO = GameObject.Find("StartGame");
        giveCardGO = GameObject.Find("GiveCard");
        makePyramidGO = GameObject.Find("MakePyramid");

        startGameGO.SetActive(false);
        giveCardGO.SetActive(false);
        makePyramidGO.SetActive(false);
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    //-----------------------------------------------------------------------------

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    //-----------------------------------------------------------------------------

    public void CreateGame()
    {
        if (CreateGameInput != null && CreateGameInput.text != "")
        {
            PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions { MaxPlayers = 0 , PublishUserId = true}, null);
            startGameGO.SetActive(true);
        }
        else
        {
            createGameErrorText.text = "Debes introducir una ID de sala";
            createGameErrorText.gameObject.SetActive(true);
        }
    }

    //-----------------------------------------------------------------------------

    public void JoinGame()
    {
        bool succed;

        if (JoinGameInput != null && JoinGameInput.text != "")
        {
            succed = PhotonNetwork.JoinRoom(JoinGameInput.text);
        }
        else
        {
            joinGameErrorText.text = "Debes introducir una ID de sala";
            joinGameErrorText.gameObject.SetActive(true);
        }
    }

    //-----------------------------------------------------------------------------

    public void StartGame()
    {
        isGameStarted = true;

        PhotonPlayer[] playerList = PhotonNetwork.playerList;

        for(int i = 0; i < playerList.Length; i++)
        {
            Debug.Log("playerList[i].ID: " + playerList[i].ID);
            playerIDs.Add(playerList[i].ID);
        }

        startGameGO.SetActive(false);
        giveCardGO.SetActive(true);
    }

    //-----------------------------------------------------------------------------

    public void OnPhotonJoinRoomFailed()
    {
        joinGameErrorText.text = "No existe una sala con esa ID";
    }

    //-----------------------------------------------------------------------------

    private void OnJoinedRoom()
    {
        GameObject.Find("MainMenu").SetActive(false);
        isMenuActive = false;
    }

    //-----------------------------------------------------------------------------

    public void SendCardSetInTable(int id)
    {
        photonView.RPC("SetCardInTable", PhotonTargets.Others, id);
    }

    //-----------------------------------------------------------------------------

    public void GiveCardToPlayer()
    {
        List<Material> materialsList = new List<Material>(GameFunctions.cardMaterials.Keys);
        System.Random rand = new System.Random();

        Material randomMaterial = materialsList[rand.Next(materialsList.Count)];

        int index;
        GameFunctions.cardMaterials.TryGetValue(randomMaterial, out index);
        
        if (playerIDs.Count > 1)
            currentPlayerTurn = (currentPlayerTurn + 1) % (playerIDs.Count);
        else
            currentPlayerTurn = 0;

        if(currentPlayerTurn != (playerIDs.Count - 1))
        {
            GameFunctions.cardMaterials.Remove(randomMaterial);
            GameFunctions.cardMaterialsPlayed.Add(randomMaterial, index);
        }

        photonView.RPC("AddCardToDeck", PhotonPlayer.Find(playerIDs[currentPlayerTurn]), index);

        if(GameFunctions.cardMaterialsPlayed.Count == (playerIDs.Count * 4))
        {
            giveCardGO.SetActive(false);
            makePyramidGO.SetActive(true);
        }
    }

    //-----------------------------------------------------------------------------

    public void MakePyramid()
    {
        for (int i = 0; i < 10; i++)
        {
            List<Material> materialsList = new List<Material>(GameFunctions.cardMaterials.Keys);
            System.Random rand = new System.Random();

            Material randomMaterial = materialsList[rand.Next(materialsList.Count)];

            int index;
            GameFunctions.cardMaterials.TryGetValue(randomMaterial, out index);

            GameFunctions.cardMaterials.Remove(randomMaterial);
            GameFunctions.cardMaterialsPlayed.Add(randomMaterial, index);

            photonView.RPC("AddCardToPyramid", PhotonTargets.All, index);
        }
    }

    //-----------------------------------------------------------------------------

    [PunRPC]
    public void SetCardInTable(int id)
    {
        setCardInTable.Invoke(id);
    }

    //-----------------------------------------------------------------------------

    [PunRPC]
    public void AddCardToDeck(int id)
    {
        addCardToDeck.Invoke(id);
    }

    //-----------------------------------------------------------------------------

    [PunRPC]
    public void AddCardToPyramid(int id)
    {
        addCardToPyramid.Invoke(id);
    }

    //-----------------------------------------------------------------------------

    public void SetSetCardInTable(UnityAction<int> setCardInTable)
    {
        this.setCardInTable = setCardInTable;
    }

    //-----------------------------------------------------------------------------

    public void SetAddCardToDeck(UnityAction<int> addCardToDeck)
    {
        this.addCardToDeck = addCardToDeck;
    }

    //-----------------------------------------------------------------------------

    public void SetAddCardToPyramid(UnityAction<int> addCardToPyramid)
    {
        this.addCardToPyramid = addCardToPyramid;
    }

    //-----------------------------------------------------------------------------

    public bool GetIsGameStarted() => isGameStarted;

    //-----------------------------------------------------------------------------

    public bool GetIsMenuActive() => isMenuActive;
}
