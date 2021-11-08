using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseWindowLightsManager : MonoBehaviour, IEnumerable<Material>
{
    private Renderer _renderer;

    [SerializeField, HideInInspector]
    private Vector4[] _centresAndDistancesSq;

    private static Color _emissionColor = default;
    private static readonly int _EmissionColorID, _TimeOffsetID, _CentreID, _DistanceSqID;
    static HouseWindowLightsManager()
    {
        _EmissionColorID = Shader.PropertyToID("_EmissionColor");
        _TimeOffsetID = Shader.PropertyToID("_TimeOffset");
        _CentreID = Shader.PropertyToID("_Centre");
        _DistanceSqID = Shader.PropertyToID("_DistanceSq");
    }

    public Material this[int index] => _renderer.sharedMaterials[index+2];

    private Renderer GetChildRenderer()
    {
        if (TryGetComponent(out Renderer renderer)) return renderer;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf) return child.GetComponent<Renderer>();
        }
        return null;
    }

    [Zenject.Inject]
    public void Construct(DayNightCycle dayCycle)
    {
        dayCycle.UpdateValuesEvent.AddListener(ChangeLights);
    }

    private void Awake()
    {
        SetMaterials(_renderer = GetChildRenderer());
    }



    private void ChangeLights()
    {
        if (!gameObject.activeSelf) return;
        foreach (var material in this)
        {
            material.SetColor(_EmissionColorID, Color.black);
        }
        var materials = _renderer.sharedMaterials;
        int n = Random.Range(1, materials.Length - 2);
        for(int i = 0; i < n; i++)
        {
            SetMaterialPropertiesDaily(materials[Random.Range(2, materials.Length)]);
        }
    }

    private void SetMaterialPropertiesDaily(Material material)
    {
        material.SetColor(_EmissionColorID, _emissionColor);
        material.SetFloat(_TimeOffsetID, Random.Range(0f, 1f));
    }

    private void SetMaterials(Renderer renderer)
    {
        var materials = renderer.sharedMaterials;

        if (_centresAndDistancesSq != null && _centresAndDistancesSq.Length != 0)
        {
            for (int i = 2; i < renderer.sharedMaterials.Length; ++i)
            {
                materials[i] = Instantiate(materials[i]);
                materials[i].SetVector(_CentreID, _centresAndDistancesSq[i - 2]);
                materials[i].SetFloat(_DistanceSqID, _centresAndDistancesSq[i - 2].w);
            }
        }
        else
        {
            var transformMatrix = renderer.transform.localToWorldMatrix;
            var mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
            for (int i = 2; i < renderer.sharedMaterials.Length; ++i)
            {
                materials[i] = Instantiate(materials[i]);
                var bounds = mesh.GetSubMesh(i).bounds;
                Vector4 centre = bounds.center;
                centre.w = 1f;
                materials[i].SetVector(_CentreID, transformMatrix * centre);

                float d = (bounds.extents.x + bounds.extents.z) * 0.5f * 1.1f;
                if (d < 0.4f) d = 0.4f;
                materials[i].SetFloat(_DistanceSqID, d * d);
            }
        }

        renderer.sharedMaterials = materials;

        if(_emissionColor == default) _emissionColor = materials[2].GetColor(_EmissionColorID);
    }

    //private static Vector4 CalculateCentreOfSubMesh(Mesh mesh, int subMeshIndex)
    //{
    //    int[] triangles = mesh.GetTriangles(subMeshIndex);
    //    Vector3[] vertices = mesh.vertices;
    //    Vector3 centre = default;
    //    for (int i = 0; i < triangles.Length; ++i)
    //    {
    //        centre += vertices[triangles[i]];
    //    }
    //    return centre / triangles.Length;
    //}

    public void SetCentres()
    {
        var renderer = GetChildRenderer();
        var mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
        int length = renderer.sharedMaterials.Length - 2;
        var transformMatrix = renderer.transform.localToWorldMatrix;

        _centresAndDistancesSq = new Vector4[length];
        for (int i = 0; i < length; ++i)
        {
            _centresAndDistancesSq[i] = CalculateCentreAndDistanceSq(transformMatrix, mesh.GetSubMesh(i + 2).bounds);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
#endif

    }

    private Vector4 CalculateCentreAndDistanceSq(in Matrix4x4 transformMatrix, in Bounds bounds)
    {
        Vector4 centre = bounds.center;
        centre.w = 1f;
        centre = transformMatrix * centre;

        float d = (bounds.extents.x + bounds.extents.z) * 0.5f * 1.1f;
        if (d < 0.4f) d = 0.4f;
        centre.w = d * d;

        return centre;
    }

    public IEnumerator<Material> GetEnumerator()
    {
        var materials = _renderer.sharedMaterials;
        for (int i = 2; i < materials.Length; ++i)
        {
            yield return materials[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
