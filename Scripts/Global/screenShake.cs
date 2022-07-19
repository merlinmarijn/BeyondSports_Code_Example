using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class screenShake : MonoBehaviourPun
{
    public bool start = false;
    public AnimationCurve curve;
    [SerializeField] public float duration = 1f;

    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shaking(duration));
        }
    }

    [PunRPC]
    public void ShakeScreen(float duration) {
        StartCoroutine(Shaking(duration));
    }

    public IEnumerator Shaking(float duration)
    {
        //Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = transform.position + Random.insideUnitSphere * strength;
            yield return null;
        }
        //transform.position = startPosition;
    }
}
