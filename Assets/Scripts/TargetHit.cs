using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHit : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Next Level");

        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            StartCoroutine(WaitBeforeNextLevel());
        }
    }

    IEnumerator WaitBeforeNextLevel()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.LoadNextLevel();
    }
}
