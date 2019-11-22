using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RacingModeGameManager : MonoBehaviour
{
    public GameObject[] PlayerPrefabs;
    public Transform[] InstantiatePosition;

    public Text TimeUIText;
    public GameObject[] FinishOrderUIGameObjects;

    public List<GameObject> laptriggers = new List<GameObject>();

    //Singleton implementation
    public static RacingModeGameManager instance = null;

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;

        }

        //if instance already excists and it's not !this
        else if(instance != this)
        {
            // Then destroy this. This enforces our singleton pattern, meaning that there can only ever be one instance of a GameObject
            Destroy(gameObject);
        }

        //To not be destroyed on loading scene
        DontDestroyOnLoad(gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerRacingGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber+" player selection nymber");

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiateposition = InstantiatePosition[actorNumber-1].position;

                PhotonNetwork.Instantiate(PlayerPrefabs[(int)playerSelectionNumber].name, instantiateposition, Quaternion.identity);


            }

        }

        foreach (GameObject gm in FinishOrderUIGameObjects)
        {
            gm.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
