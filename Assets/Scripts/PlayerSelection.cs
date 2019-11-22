using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSelection : MonoBehaviour
{
    public GameObject[] SelectablePlayers;

    public int playerSelectionNumber;

    // Start is called before the first frame update
    void Start()
    {
        playerSelectionNumber = 0;
        ActivatePlayer(playerSelectionNumber);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActivatePlayer(int x)
    {
        foreach (GameObject selectablePlayer in SelectablePlayers)
        {
            selectablePlayer.SetActive(false);
        }

        SelectablePlayers[x].SetActive(true);

        //setting up player selection custom property
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable() { {MultiplayerRacingGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);

    }

    public void NextPlayer()
    {
        playerSelectionNumber += 1;
        if (playerSelectionNumber>=SelectablePlayers.Length)
        {
            playerSelectionNumber = 0;
        }

        ActivatePlayer(playerSelectionNumber);
    }

    public void PreviousPlayer()
    {
        playerSelectionNumber += 1;
        if (playerSelectionNumber<0)
        {
            playerSelectionNumber = SelectablePlayers.Length - 1;
        }

        ActivatePlayer(playerSelectionNumber);
    }


}
