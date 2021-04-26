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

    private PhotonView photonView;

    private bool isGameStarted;
    private bool isMenuActive;

    private int currentPlayerTurn;
    private List<int> playerIDs;

    private GameObject startGameGO;
    private GameObject giveCardGO;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);
        isGameStarted = false;
        isMenuActive = true;
        currentPlayerTurn = 0;
        playerIDs = new List<int>();

        startGameGO = GameObject.Find("StartGame");
        giveCardGO = GameObject.Find("GiveCard");

        startGameGO.SetActive(false);
        giveCardGO.SetActive(false);
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
        //ESTO NO FUNCIONA JEJE NO SE QUE MIERDAS LE PASA XD
        List<Material> materialsList = new List<Material>(GameFunctions.cardMaterials.Keys);
        System.Random rand = new System.Random();

        Material randomMaterial = materialsList[rand.Next(materialsList.Count)];

        int index;
        GameFunctions.cardMaterials.TryGetValue(randomMaterial, out index);
        
        if (playerIDs.Count > 1)
            currentPlayerTurn = (currentPlayerTurn + 1) % (playerIDs.Count);
        else
            currentPlayerTurn = 0;
        Debug.Log("currentPlayerTurn: " + currentPlayerTurn);
        if(currentPlayerTurn != (playerIDs.Count - 1))
        {
            GameFunctions.cardMaterials.Remove(randomMaterial);
            GameFunctions.cardMaterialsPlayed.Add(randomMaterial, index);
        }


        photonView.RPC("AddCardToDeck", PhotonPlayer.Find(playerIDs[currentPlayerTurn]), index);
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

    public bool GetIsGameStarted() => isGameStarted;

    //-----------------------------------------------------------------------------

    public bool GetIsMenuActive() => isMenuActive;
}
