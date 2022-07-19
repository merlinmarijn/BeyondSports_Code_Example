using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePhotonManager : MonoBehaviourPunCallbacks
{

    public List<GameObject> PlayerPrefabList;
	public GameObject PlayerPrefab;

    //private List<PlayerData> playerState = new List<PlayerData>();

    [SerializeField]
    private List<GameObject> SpawnPoints;


    [SerializeField]
    private GameObject StartButton;
    [SerializeField]
    private GameObject StartPlatform;
    [SerializeField]
    private float fadeTime=5f;
    public bool gameStarted = false;

    //public Transform PlayerCage;

    private GameObject LocalPlayer;
    [HideInInspector] public GamemodeManager GM;
    AudioHandler audio;

    private void Start()
    {
        int index = Array.IndexOf(PhotonNetwork.PlayerList, PhotonNetwork.LocalPlayer);
        int SelectedPlayer = PlayerPrefs.GetInt("selectedCharacter");
        LocalPlayer = PhotonNetwork.Instantiate(PlayerPrefabList[SelectedPlayer].name, SpawnPoints[index].transform.position, Quaternion.identity, 0);
        LocalPlayer.GetComponent<PlayerData>().setGameManager(this);
        Player[] playerList = PhotonNetwork.PlayerList;
        GM = GetComponent<GamemodeManager>();
        if (!PhotonNetwork.IsMasterClient)
        {
            StartButton.SetActive(false);
        }
        audio = gameObject.GetComponent<AudioHandler>();
    }

    public void StartGame()
    {
        photonView.RPC("RPC_StartGame", RpcTarget.All);
        StartButton.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    [PunRPC]
    void RPC_StartGame()
    {
        GetComponent<PlatformGeneration>().enabled = true;
        StartCoroutine(fadePlatform(StartPlatform.GetComponent<SpriteRenderer>()));
        gameStarted = true;
    }

    IEnumerator fadePlatform(SpriteRenderer renderer)
    {
        Color tmpColor = renderer.color;

        while (tmpColor.a > 0f){

            tmpColor.a -= Time.deltaTime / fadeTime;
            renderer.color = tmpColor;

            if (tmpColor.a <= 0f)
                tmpColor.a = 0.0f;

                yield return null;

            renderer.color = tmpColor;
        }
        renderer.transform.DetachChildren();
        renderer.gameObject.SetActive(false);
    }

    //private void Update()
    //{
    //    if (UnityEngine.Input.GetKeyDown(KeyCode.P))
    //    {
    //        Call_Disconnect();
    //    }
    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        Debug.Log($"Rounds: {PhotonNetwork.CurrentRoom.CustomProperties["rounds"]}, Played: {PlayerPrefs.GetInt("PlayedRounds")}");
    //    }
    //}

    public void Call_Disconnect()
    {
        StartCoroutine(Disconnect());
    }

    private IEnumerator Disconnect()
    {
        PhotonNetwork.Disconnect();
        Cursor.lockState = CursorLockMode.None;
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        PhotonNetwork.LoadLevel(0);
    }

    public IEnumerator CloseRoom()
    {
        photonView.RPC("RPC_DisconnectOthers", RpcTarget.Others);
        yield return new WaitForSeconds(2f);
        Call_Disconnect();
    }

    [PunRPC]
    void RPC_DisconnectOthers()
    {
        Call_Disconnect();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.CurrentRoom.IsVisible = true;
    }

    public GameObject getPlayer()
    {
        return LocalPlayer;
    }
}
