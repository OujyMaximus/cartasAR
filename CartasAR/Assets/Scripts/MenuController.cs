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

    private PhotonView photonView;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersionName);
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    public void CreateGame()
    {
        if (CreateGameInput != null && CreateGameInput.text != "")
            PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions { MaxPlayers = 2 }, null);
        else
        {
            createGameErrorText.text = "Debes introducir una ID de sala";
            createGameErrorText.gameObject.SetActive(true);
        }
    }

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

    public void OnPhotonJoinRoomFailed()
    {
        joinGameErrorText.text = "No existe una sala con esa ID";
    }

    private void OnJoinedRoom()
    {
        GameObject.Find("MainMenuCanvas").SetActive(false);
    }

    public void SendCardSetInTable(int id)
    {
        photonView.RPC("SetCardInTable", PhotonTargets.Others, id);
    }

    [PunRPC]
    public void SetCardInTable(int id)
    {
        Debug.Log("Hey");
        setCardInTable.Invoke(id);
    }

    public void SetSetCardInTable(UnityAction<int> setCardInTable)
    {
        this.setCardInTable = setCardInTable;
    }
}
