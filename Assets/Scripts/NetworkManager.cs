using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public InputField PlayerNameInput;

    [Header("Connecting Info Panel")]
    public GameObject ConnectingInfoUIPanel;

    [Header("Creating Room Info Panel")]
    public GameObject CreatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject GameOptionsUIPanel;


    [Header("Create Room Panel")]
    public GameObject CreateRoomUIPanel;
    public InputField RoomNameInputField;
    public string GameMode;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;
    public Text RoomInfoText;
    public GameObject PlayerListPrefab;
    public GameObject PlayerListContent;
    public GameObject StartGameButton;
    public Text GameModeText;
    public Image PanelBackGround;
    public Sprite RacingBackground;
    public Sprite DeathRaceBackground;
    public GameObject[] playerSelectionUIGameObject;
    public DeathRacePlayer[] DeathRacePlayers;
    public RacingPlayer[] RacingPlayers;

    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomUIPanel;

    private Dictionary<int, GameObject> playerListGameObjects;

    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);

        PhotonNetwork.AutomaticallySyncScene = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI Callback Methods

    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(ConnectingInfoUIPanel.name);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.Log(" Player name is invalid!!! ");
            }
        }
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnCreateRoomButtonClicked()
    {
        ActivatePanel(CreatingRoomInfoUIPanel.name);
        if (GameMode!= null)
        {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room" + Random.Range(1000, 10000);
            }
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 3;

            string[] roomPropsInLobby = { "gm" }; //   gm = game mode

            //two game modes
            // 1. racing - "rc"
            // 2. death race - "dr"

            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } };

            roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }


        
    }

    public void OnJoinRandomRoomButtonClicked(string _gameMode)
    {
        GameMode = _gameMode;

        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { {"gm" , _gameMode } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnBackButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                // Racing game mode
                PhotonNetwork.LoadLevel("RacingScene");

            }
            else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                // Death race game mode
                PhotonNetwork.LoadLevel("DeathRaceScene");

            }


        }


    }

    #endregion

    #region Photon Callbacks(overrides)

    public override void OnConnected()
    {
        Debug.Log("WE CONNECTED TO INTERNET ");
    }

    public override void OnConnectedToMaster()
    {
        ActivatePanel(GameOptionsUIPanel.name);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " IS CONNECTED TO PHOTON server");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + "Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        ActivatePanel(InsideRoomUIPanel.name);

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {

            RoomInfoText.text = "Room name " + PhotonNetwork.CurrentRoom.Name + " " +
                 " playerd/Max.Players: " +
                 PhotonNetwork.CurrentRoom.Name + " / " +
                 PhotonNetwork.CurrentRoom.MaxPlayers;


            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                //racing game mode
                GameModeText.text = "LET'S RACE!";
                PanelBackGround.sprite = RacingBackground;

                for (int i = 0; i < playerSelectionUIGameObject.Length; i++)
                {
                    playerSelectionUIGameObject[i].transform.Find("PlayerName").GetComponent<Text>().text = RacingPlayers[i].playerName;
                    playerSelectionUIGameObject[i].GetComponent<Image>().sprite = RacingPlayers[i].playerSprite;
                    playerSelectionUIGameObject[i].transform.Find("PlayerProperty").GetComponent<Text>().text = "";


                }

            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                //death race game mode
                GameModeText.text = "DEATH RACE!";
                PanelBackGround.sprite = DeathRaceBackground;
                for (int i = 0; i < playerSelectionUIGameObject.Length; i++)
                {
                    playerSelectionUIGameObject[i].transform.Find("PlayerName").GetComponent<Text>().text = DeathRacePlayers[i].playerName;
                    playerSelectionUIGameObject[i].GetComponent<Image>().sprite = DeathRacePlayers[i].playerSprite;
                    playerSelectionUIGameObject[i].transform.Find("PlayerProperty").GetComponent<Text>().text = DeathRacePlayers[i].weaponName+
                        ": " + "Damage: " + DeathRacePlayers[i].damage + " FireRate: " + DeathRacePlayers[i].fireRate;


                }

            }

            if (playerListGameObjects == null)
            {
                playerListGameObjects = new Dictionary<int, GameObject>();
            }
           
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                GameObject playerListGameObject = Instantiate(PlayerListPrefab);
                playerListGameObject.transform.SetParent(PlayerListContent.transform);
                playerListGameObject.transform.localScale = Vector3.one;
                playerListGameObject.GetComponent<PlayerListEntryinitializer>().Initialize(player.ActorNumber, player.NickName);


                object isPlayerReady;
                if (player.CustomProperties.TryGetValue(MultiplayerRacingGame.PLAYER_READY, out isPlayerReady))
                {
                    playerListGameObject.GetComponent<PlayerListEntryinitializer>().SetPlayerReady((bool)isPlayerReady);
                }



                playerListGameObjects.Add(player.ActorNumber, playerListGameObject);


            }


        }

        StartGameButton.SetActive(false);

    }

    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerListGameObject;

        if (playerListGameObjects.TryGetValue(target.ActorNumber,out playerListGameObject))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(MultiplayerRacingGame.PLAYER_READY, out isPlayerReady))
            {
                playerListGameObject.GetComponent<PlayerListEntryinitializer>().SetPlayerReady((bool)isPlayerReady);

            }

        }

        StartGameButton.SetActive(CheckPlayersReady());
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomInfoText.text = "Room name " + PhotonNetwork.CurrentRoom.Name + " " +
                " playerd/Max.Players: " +
                PhotonNetwork.CurrentRoom.Name + " / " +
                PhotonNetwork.CurrentRoom.MaxPlayers;

        GameObject playerListGameObject = Instantiate(PlayerListPrefab);
        playerListGameObject.transform.SetParent(PlayerListContent.transform);
        playerListGameObject.transform.localScale = Vector3.one;
        playerListGameObject.GetComponent<PlayerListEntryinitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);

        StartGameButton.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomInfoText.text = "Room name " + PhotonNetwork.CurrentRoom.Name + " " +
                " playerd/Max.Players: " +
                PhotonNetwork.CurrentRoom.Name + " / " +
                PhotonNetwork.CurrentRoom.MaxPlayers;


        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);

        StartGameButton.SetActive(CheckPlayersReady());

    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptionsUIPanel.name);
        foreach (GameObject playerListGameObject in playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.SetActive(CheckPlayersReady());
        }
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        // if there's no room, create one
        if (GameMode != null)
        {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room" + Random.Range(1000, 10000);
            }
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 3;

            string[] roomPropsInLobby = { "gm" }; //   gm = game mode

            //two game modes
            // 1. racing - "rc"
            // 2. death race - "dr"

            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } };

            roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

  

    #endregion


    #region Public Methods
    public void ActivatePanel(string panelNameToBeActivated)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
    }

    public void SetGameMode(string _gameMode)
    {
        GameMode = _gameMode;
    }

    #endregion

    #region Private Methods

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(MultiplayerRacingGame.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
           
        }

        return true;

    }

    #endregion

}
