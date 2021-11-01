using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseWindowLightsManager : MonoBehaviour, IEnumerable<Material>
{
    [SerializeField, ReadOnlyOnInspector]
    private Renderer _renderer;

    private static Color _emissionColor = default;
    private static readonly int _EmissionColorID;
    static HouseWindowLightsManager()
    {
        _EmissionColorID = Shader.PropertyToID("_EmissionColor");
    }

    public Material this[int index] => _renderer.sharedMaterials[index+2];

    private Renderer GetChildRenderer
    {
        get
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf) return child.GetComponent<Renderer>();
            }
            return null;
        }
    }



    [Zenject.Inject]
    public void Construct(DayNightCycle dayCycle)
    {
        dayCycle.UpdateValuesEvent.AddListener(ChangeLights);
    }

    private void Awake()
    {
        SetMaterials(_renderer = GetChildRenderer);
    }



    private void ChangeLights()
    {
        foreach (var material in this)
        {
            material.SetColor(_EmissionColorID, _emissionColor);
        }
        var materials = _renderer.sharedMaterials;
        int n = Random.Range(1, materials.Length - 2);
        for(int i = 0; i < n; i++)
        {
            materials[Random.Range(2, materials.Length)].SetColor(_EmissionColorID, Color.black);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<Material> GetEnumerator()
    {
        var materials = _renderer.sharedMaterials;
        for (int i = 2; i < materials.Length; ++i)
        {
            yield return materials[i];
        }
    }

    private void SetMaterials(Renderer renderer)
    {
        var materials = renderer.sharedMaterials;
        for (int i = 2; i < renderer.sharedMaterials.Length; ++i)
        {
            materials[i] = Instantiate(materials[i]);
        }
        renderer.sharedMaterials = materials;

        if(_emissionColor == default) _emissionColor = materials[2].GetColor(_EmissionColorID);
    }
}
