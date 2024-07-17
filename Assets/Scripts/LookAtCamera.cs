using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        StartCoroutine(FindCamera());
    }

    IEnumerator FindCamera()
    {
        while (camTransform == null)
        {
            yield return new WaitForSeconds(1f);
            GameObject cam = GameObject.FindWithTag("MainCamera");
            if (cam != null)
            {
                camTransform = cam.transform;
                Debug.Log("Kamera bulundu");
            }
        }
    }

    void Update()
    {
        if (camTransform != null)
        {
            transform.LookAt(transform.position + camTransform.rotation * Vector3.forward, camTransform.rotation * Vector3.up);
        }
    }
}
