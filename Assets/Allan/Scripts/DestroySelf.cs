using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    private void Start ()
    {
        StartCoroutine(Die());
    }
    IEnumerator Die ()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        yield return null;
    }
}
