using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntrySequence : MonoBehaviour
{
    public int count;
    public GameObject entryCams;
    public List<GameObject> canvasPanels;
    public GameObject eventSystem;
    public int maxCount;
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (count >= maxCount)
            {
                Destroy(entryCams);
                Destroy(eventSystem);
                Destroy(gameObject);
            }
            else
            {
                entryCams.transform.GetChild(count).gameObject.SetActive(false);              
                canvasPanels[count].SetActive(false);

                if (count + 1 != maxCount)
                    canvasPanels[count +1 ].SetActive(true);

                count++;
            }              
        }
    }
}
