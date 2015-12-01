using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    public Transform target;
	// Update is called once per frame
	private void Update ()
    {
        if (target != null)
            transform.LookAt(target);
	}
}
