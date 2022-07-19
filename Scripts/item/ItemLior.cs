using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemLior : MonoBehaviourPun
{
    private bool windBlowing;
    private GameObject[] tornados;
    private float minSpawnX;
    private float maxSpawnX;
    private float minSpawnY;
    private float maxSpawnY;
    public bool activeStorm;
    private int activeTornados;

    [Tooltip("The speed at how fast the tornado's move")]
    [SerializeField] private float moveSpeed = 0.1f;
    [Tooltip("Time it takes for the tornado's to appear")]
    [SerializeField] private float fadeTime = 3f;
    [Tooltip("The X radius from the parent in where tornados appear")]
    [SerializeField] private int radiusX = 5;
    [Tooltip("The Y radius from the parent in where tornados appear")]
    [SerializeField] private int radiusY = 4;

    void Start()
    {
        tornados = new GameObject[transform.childCount];
        GetChildren();
        minSpawnX = transform.position.x - radiusX;
        maxSpawnX = transform.position.x + radiusX;
        minSpawnY = transform.position.x - radiusY;
        maxSpawnY = transform.position.x + radiusY;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.K))
        BlowWind();
    }
    private void FixedUpdate()
    {
        if (tornados.Length > 1 && activeStorm)
        {
            foreach(GameObject child in tornados)
            {
                if (child.transform.position.x > transform.position.x && child.transform.position.x < transform.position.x + (radiusX * 4))
                    child.transform.position = new Vector3(child.transform.position.x + moveSpeed, child.transform.position.y, child.transform.position.z);

                if (child.transform.position.x < transform.position.x && child.transform.position.x > transform.position.x - (radiusX * 4))
                    child.transform.position = new Vector3(child.transform.position.x - moveSpeed, child.transform.position.y, child.transform.position.z);

                if (child.transform.position.x <= transform.position.x - (radiusX * 4) || child.transform.position.x >= transform.position.x + (radiusX * 4))
                    child.SetActive(false);
            }
        }
        if (allInActive() && windBlowing)
        {
            photonView.RPC("BlowWind", RpcTarget.All);
            activeTornados = 0;
        }

        if (activeTornados == transform.childCount)
            activeStorm = true;
        else
            activeStorm = false;
    }

   [PunRPC]
   public void BlowWind()
   {
       windBlowing = !windBlowing;
       activeTornados = 0;
       if (tornados.Length > 1)
       {
           foreach (GameObject child in tornados)
           {
               child.transform.localPosition = new Vector3(Random.Range(minSpawnX, maxSpawnX), Random.Range(minSpawnY, maxSpawnY), 0);
  
               child.SetActive(windBlowing);
               StartCoroutine(TornadoFade(child.GetComponent<SpriteRenderer>()));
            }
        }
   }

    IEnumerator TornadoFade(SpriteRenderer renderer)
    {
        if (renderer.enabled) {
            Color tmpColor = renderer.color;
            Color tmpNone = renderer.color;
            tmpNone.a = 0f;
            renderer.color = tmpNone;

            while (tmpColor.a < 1f)
            {
                tmpColor.a += Time.deltaTime / fadeTime;
                renderer.color = tmpColor;

                if (tmpColor.a >= 1f)
                    tmpColor.a = 1f;

                yield return null;

                renderer.color = tmpColor;
            }
            activeTornados += 1;
            renderer.gameObject.GetComponent<BoxCollider2D>().enabled = windBlowing;
            // activeTornado = true;
        }
    }

    public bool allInActive()
    {
        bool response = true;
        for (int i = 0; i < tornados.Length; i++)
        {
            if (tornados[i].activeInHierarchy)
            {
                response = false;
                break;
            }
        }
        return response;
    }

    private void GetChildren()
    {
        for (int i = 0; i <= transform.childCount-1; i++){
            tornados[i] = transform.GetChild(i).gameObject;
        }
    }
}
