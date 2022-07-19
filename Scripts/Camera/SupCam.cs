using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class SupCam : MonoBehaviourPunCallbacks
{
    public FocusPlayer focusPlayer;

    public List<GameObject> players;

    public float DepthUpdateSpeed = 5f;
    public float AngleUpdateSpeed = 7f;
    public float PositionUpdateSpeed = 5f;

    public float DepthMax = -10f;
    public float DepthMin = -22f;

    public float AngleMax = 11f;
    public float AngleMin = 3f;
    [SerializeField] private float camNormalSpeed = 0.003f;
    [SerializeField] private float camFasterSpeed = 0.004f;

    private float CameraEulerX;
    private Vector3 CameraPosition;

    private GamePhotonManager GPM;

    private void Start() {
        GPM = GameObject.FindGameObjectWithTag("NetworkController").GetComponent<GamePhotonManager>();
    }
    // Update is called once per frame
    private void LateUpdate()
    {
        if(PhotonNetwork.IsMasterClient)
    {
        players = players.Where(item => item != null).ToList();
        if (focusPlayer && players.Count > 0)
        {
            CalculateCameraLocations();
            MoveCamera();
        }

        GameObject HighestPlayer = players.OrderByDescending(LowestPlayer => LowestPlayer.transform.position.y).First() ? 
        players.OrderByDescending(LowestPlayer => LowestPlayer.transform.position.y).First() : null;

        if (focusPlayer.gameObject && focusPlayer.gameObject != HighestPlayer)
        {
            focusPlayer = HighestPlayer.GetComponent<FocusPlayer>();
        }

        if (PhotonNetwork.IsMasterClient) {

            //Checking if lowest player is out of bounds of focusplayer
            GameObject LowestPlayer = players.OrderByDescending(LowestPlayer => LowestPlayer.transform.position.y).Last();
            if (!focusPlayer.FocusBounds.Contains(LowestPlayer.transform.position))
            {
            //Player is lowest and is out of bounds do something
                //Debug.Log($"Player id: {LowestPlayer.GetComponent<PhotonView>().ViewID}, Height: {LowestPlayer.transform.position.y}");
                photonView.RPC("RPC_OutOfBounds", RpcTarget.All, LowestPlayer.GetComponent<PhotonView>().ViewID);
                LowestPlayer.GetComponent<PlayerMovement2D>().photonView.RPC("RPC_GetJailed", LowestPlayer.GetComponent<PhotonView>().Owner);
            }

            //Checking if focusplayer is out of bounds of second lowest player
            //if (LowestPlayer.GetComponent<PhotonView>().Owner == PhotonNetwork.MasterClient && players.Count>1)
            //{
            //    GameObject secondlowest = players.OrderByDescending(LowestPlayer => LowestPlayer.transform.position.y).Last(secondlowest => secondlowest.GetComponent<PhotonView>().Owner != PhotonNetwork.MasterClient);    
            //    if (!secondlowest.GetComponent<FocusPlayer>().FocusBounds.Contains(LowestPlayer.transform.position))
            //    {
            //        //Focus player is out of bounds do something
            //        //Debug.Log($"Player id: {LowestPlayer.GetComponent<PhotonView>().ViewID}, Height: {LowestPlayer.transform.position.y}");
            //        photonView.RPC("RPC_OutOfBounds", RpcTarget.All, LowestPlayer.GetComponent<PhotonView>().ViewID);
            //        LowestPlayer.GetComponent<PlayerMovement2D>().photonView.RPC("RPC_GetJailed", LowestPlayer.GetComponent<PhotonView>().Owner);
            //    }
            //}
        }
    }
    }

  public GameObject GetLowestPlayer()
    {
        GameObject LP = players.OrderByDescending(LowestPlayer => LowestPlayer.transform.position.y).Last();
        return LP;
    }
    public GameObject GetHighestPlayer() 
    {
        GameObject HP = players.OrderByDescending(LowestPlayer => LowestPlayer.transform.position.y).First();
        return HP;
    }

    [PunRPC]
    private void SetPosition(Vector3 Pos) {
    this.transform.position = Pos;
    }

    private void MoveCamera()
    {
        Vector3 position = gameObject.transform.position;
        if(position!= CameraPosition)
        {
            Vector3 targetPosition = Vector3.zero;
            targetPosition.x = Mathf.MoveTowards(position.x, CameraPosition.x, PositionUpdateSpeed * Time.deltaTime);
            //targetPosition.y = Mathf.MoveTowards(position.y, CameraPosition.y, PositionUpdateSpeed * Time.deltaTime);

            if (GPM.gameStarted)
            {
                float dist = Vector3.Distance(GetHighestPlayer().transform.position, GetLowestPlayer().transform.position);
                if (dist > 1) {
                    targetPosition.y = position.y + camFasterSpeed;
                }
                else
                    targetPosition.y = position.y + camNormalSpeed;
            }

            targetPosition.z = Mathf.MoveTowards(position.z, CameraPosition.z, DepthUpdateSpeed * Time.deltaTime);
            // gameObject.transform.position = targetPosition;
            photonView.RPC("SetPosition", RpcTarget.All, targetPosition);

        }

        Vector3 localEulerAngles = gameObject.transform.localEulerAngles;
        if(localEulerAngles.x != CameraEulerX)
        {
            Vector3 targetEulerAngles = new Vector3(CameraEulerX, localEulerAngles.y, localEulerAngles.z);
            gameObject.transform.localEulerAngles = Vector3.MoveTowards(localEulerAngles, targetEulerAngles, AngleUpdateSpeed * Time.deltaTime);
        }
    }

    private void CalculateCameraLocations()
    {
        Vector3 averageCenter = Vector3.zero;
        Vector3 totalPositions = Vector3.zero;
        Bounds playerBounds = new Bounds();

        for(int i = 0; i < players.Count; i++)
        {
            Vector3 playerPosition = players[i].transform.position;

            if (!focusPlayer.FocusBounds.Contains(playerPosition))
            {
                float playerX = Mathf.Clamp(playerPosition.x, focusPlayer.FocusBounds.min.x, focusPlayer.FocusBounds.max.x);
                float playerY = Mathf.Clamp(playerPosition.y, focusPlayer.FocusBounds.min.y, focusPlayer.FocusBounds.max.y);
                float playerZ = Mathf.Clamp(playerPosition.z, focusPlayer.FocusBounds.min.z, focusPlayer.FocusBounds.max.z);
                playerPosition = new Vector3(playerX, playerY, playerZ);
            }

            totalPositions += playerPosition;
            playerBounds.Encapsulate(playerPosition);
        }

        averageCenter = (totalPositions / players.Count);

        float extents = (playerBounds.extents.x + playerBounds.extents.y);
        float lerpPercent = Mathf.InverseLerp(0, (focusPlayer.HalfXBounds + focusPlayer.HalfYBounds) / 1, extents);

        float depth = Mathf.Lerp(DepthMax, DepthMin, lerpPercent);
        float angle = Mathf.Lerp(AngleMax, AngleMin, lerpPercent);

        CameraEulerX = angle;
        CameraPosition = new Vector3(averageCenter.x, averageCenter.y, depth);
    }

    public void AddPlayer(GameObject player)
    {
        players.Add(player);
    }

    [PunRPC]
    void RPC_OutOfBounds(int id)
    {
        GameObject obj = PhotonView.Find(id).gameObject;
        players.Remove(obj);
    }

    [PunRPC]
    void RPC_GetUnjailed(int id)
    {
        GameObject obj = PhotonView.Find(id).gameObject;
        players.Add(obj);
    }
}
