using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossQuestLocMarker : MonoBehaviour
{
    public ParticleSystem locShowFx;
    private void OnEnable()
    {
        locShowFx.Play();
    }
}
