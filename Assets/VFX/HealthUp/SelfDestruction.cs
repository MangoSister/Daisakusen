using UnityEngine;
using System.Collections;

public class SelfDestruction : MonoBehaviour
{
    public float lifeSpan;

    private void Awake()
    {
        StartCoroutine(SelfDestructCoroutine());
    }

    private IEnumerator SelfDestructCoroutine()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }
}
