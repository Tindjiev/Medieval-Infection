using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampLightControl : MonoBehaviour
{
    private readonly static int _timeOffsetID, _centerID;

    static LampLightControl()
    {
        _timeOffsetID = Shader.PropertyToID("_TimeOffset");
        _centerID = Shader.PropertyToID("_Centre");
    }

    private void Awake()
    {
        Material emmisionMaterial = GetComponent<Renderer>().material;
        emmisionMaterial.SetFloat(_timeOffsetID, Random.Range(0f, 1f));
        emmisionMaterial.SetVector(_centerID, transform.position); //assuming GameObject is static
        Destroy(this);
    }
}
