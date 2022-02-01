using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHitDetector : MonoBehaviour
{
    public Material hitMat;

    private bool hasHit = false;



    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Target" && !hasHit)
        {
            MeshRenderer renderer = collision.gameObject.GetComponent<MeshRenderer>();
            if(renderer != null)
            {
                hasHit = true;
                StartCoroutine(ColourchangeTimer(renderer.sharedMaterial, renderer));
                renderer.sharedMaterial = hitMat;
            }
        }

    }

    IEnumerator ColourchangeTimer(Material originalMat, MeshRenderer renderer)
    {
        yield return new WaitForSeconds(1f);

        renderer.sharedMaterial = originalMat;
    }


}
