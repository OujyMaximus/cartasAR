﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private UnityAction<int> flipCardToPyramid;

    private PhotonView photonView;
    private int mineId;

    private bool isGameStarted;
    private bool isMenuActive;

    private int currentPlayerTurn;
    private List<int> playerIDs;
    private List<int> playerCards;

    private GameObject startGameGO;
    private GameObject giveCardGO;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);
        isGameStarted = false;
        isMenuActive = true;
        currentPlayerTurn = 0;
        playerIDs = new List<int>();
        playerCards = new List<int>();
    }

    //-----------------------------------------------------------------------------

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        startGameGO = GameObject.Find("StartGame");
        giveCardGO = GameObject.Find("GiveCard");

        startGameGO.SetActive(false);
        giveCardGO.SetActive(false);

        GameObject.Find("MainMenu").SetActive(true);
    }

    //-----------------------------------------------------------------------------

    public void ConfigurePlayerMethods(
                                        UnityAction<int> setCardInTable,
                                        UnityAction<int> addCardToDeck,
                                        UnityAction<int> addCardToPyramid,
                                        UnityAction<int> flipCardToPyramid)
    {
        this.setCardInTable = setCardInTable;
        this.addCardToDeck = addCardToDeck;
        this.addCardToPyramid = addCardToPyramid;
        this.flipCardToPyramid = flipCardToPyramid;
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
            playerIDs.Add(playerList[i].ID);

            photonView.RPC("SetPlayerId", PhotonPlayer.Find(playerList[i].ID), i);
            
            playerCards.Add(0);
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
        photonView.RPC("SetCardInTable", PhotonTargets.OthersBuffered, id, mineId);
    }

    //-----------------------------------------------------------------------------

    public void GiveCardToPlayer(int index)
    {
        if (playerIDs.Count > 1)
            currentPlayerTurn = (currentPlayerTurn + 1) % (playerIDs.Count);
        else
            currentPlayerTurn = 0;

        playerCards[currentPlayerTurn] = playerCards[currentPlayerTurn] + 1;

        photonView.RPC("AddCardToDeck", PhotonPlayer.Find(playerIDs[currentPlayerTurn]), index);
    }

    //-----------------------------------------------------------------------------

    public void AddCardToPlayersPyramid(int index)
    {
        photonView.RPC("AddCardToPyramid", PhotonTargets.All, index);
    }

    //-----------------------------------------------------------------------------

    public void FlipCardToPlayersPyramid(int index)
    {
        photonView.RPC("FlipCardToPyramid", PhotonTargets.All, index);
    }

    //-----------------------------------------------------------------------------

    [PunRPC]
    public void SetPlayerId(int id)
    {
        mineId = id;
    }

    //-----------------------------------------------------------------------------

    [PunRPC]
    public void SetCardInTable(int id, int playerId)
    {
        if(playerCards.Count > 0)
        {
            Debug.Log("playerId: " + playerId);
            Debug.Log("playerCards[playerId] before: " + playerCards[playerId]);
            playerCards[playerId] = playerCards[playerId] - 1;
        }

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

    [PunRPC]
    public void FlipCardToPyramid(int id)
    {
        flipCardToPyramid.Invoke(id);
    }

    //-----------------------------------------------------------------------------

    public bool GetIsGameStarted() => isGameStarted;

    //-----------------------------------------------------------------------------

    public bool GetIsMenuActive() => isMenuActive;

    //-----------------------------------------------------------------------------

    public List<int> GetPlayerIds() => playerIDs;
}
