using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviourPun
{
    public List<GameObject> pool = new List<GameObject>();


    public void MakePool(GameObject go, int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = PhotonNetwork.Instantiate(go.name, Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }
    public GameObject GetPooledObject(GameObject go)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        //if the pool is empty
        GameObject newObj = PhotonNetwork.Instantiate(go.name, Vector3.zero, Quaternion.identity);
        newObj.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }
}
