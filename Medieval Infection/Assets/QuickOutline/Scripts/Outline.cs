//
//  Outline.cs
//  QuickOutline
//
//  Created by Chris Nolet on 3/30/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//
//  (Modified for this project)
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]

public class Outline : MonoBehaviour
{
    private static HashSet<Mesh> _registeredMeshes = new HashSet<Mesh>();

    public enum Mode
    {
        OutlineAll,
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette,
        SilhouetteOnly
    }

    public Mode OutlineMode
    {
        get => _outlineMode;
        set
        {
            _outlineMode = value;
            _needsUpdate = true;
        }
    }

    public Color OutlineColor
    {
        get => _outlineColor;
        set
        {
            _outlineColor = value;
            _needsUpdate = true;
        }
    }

    public float OutlineWidth
    {
        get => _outlineWidth;
        set
        {
            _outlineWidth = value;
            _needsUpdate = true;
        }
    }

    [Serializable]
    private class ListVector3
    {
        public List<Vector3> Data;
    }

    [SerializeField]
    private Mode _outlineMode;

    [SerializeField]
    private Color _outlineColor = Color.white;

    [SerializeField, Range(0f, 10f)]
    private float _outlineWidth = 1f;

    [Header("Optional")]

    [SerializeField, Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
    + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
    private bool _precomputeOutline;

    [SerializeField, HideInInspector]
    private List<Mesh> _bakeKeys = new List<Mesh>();

    [SerializeField, HideInInspector]
    private List<ListVector3> _bakeValues = new List<ListVector3>();

    private Renderer[] _renderers;
    private Material _outlineMaskMaterial;
    private Material _outlineFillMaterial;

    private bool _needsUpdate;

    [SerializeField]
    private bool _isOutlinePrefab = false;
    private Renderer _renderer;

    private void Awake()
    {
        if (_isOutlinePrefab)
        {
            _renderer = GetComponent<Renderer>();
            var materials = _renderer.materials;
            _outlineMaskMaterial = materials[0];
            _outlineFillMaterial = materials[1];
        }
        else
        {
            // Cache renderers
            _renderers = GetComponentsInChildren<Renderer>();

            // Instantiate outline materials
            _outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
            _outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

            _outlineMaskMaterial.name = "OutlineMask (Instance)";
            _outlineFillMaterial.name = "OutlineFill (Instance)";
        }

        // Retrieve or generate smooth normals
        LoadSmoothNormals();

        // Apply material properties immediately
        _needsUpdate = true;
    }

    private void OnEnable()
    {
        if (_isOutlinePrefab)
        {
            _renderer.enabled = true;
            return;
        }
        foreach (var renderer in _renderers)
        {

            // Append outline shaders
            var materials = renderer.sharedMaterials;

            System.Array.Resize(ref materials, materials.Length + 2);

            materials[materials.Length - 2] = _outlineMaskMaterial;
            materials[materials.Length - 1] = _outlineFillMaterial;

            renderer.materials = materials;
        }
    }

    private void OnValidate()
    {

        // Update material properties
        _needsUpdate = true;

        // Clear cache when baking is disabled or corrupted
        if (!_precomputeOutline && _bakeKeys.Count != 0 || _bakeKeys.Count != _bakeValues.Count)
        {
            _bakeKeys.Clear();
            _bakeValues.Clear();
        }

        // Generate smooth normals when baking is enabled
        if (_precomputeOutline && _bakeKeys.Count == 0)
        {
            Bake();
        }
    }

    private void Update()
    {
        if (_needsUpdate)
        {
            _needsUpdate = false;

            UpdateMaterialProperties();
        }
    }

    private void OnDisable()
    {
        if (_isOutlinePrefab)
        {
            _renderer.enabled = false;
            return;
        }
        foreach (var renderer in _renderers)
        {

            // Remove outline shaders
            var materials = renderer.sharedMaterials.ToList();

            materials.Remove(_outlineMaskMaterial);
            materials.Remove(_outlineFillMaterial);

            renderer.materials = materials.ToArray();
        }
    }

    private void OnDestroy()
    {

        // Destroy material instances
        Destroy(_outlineMaskMaterial);
        Destroy(_outlineFillMaterial);
    }

    private void Bake()
    {

        // Generate smooth normals for each mesh
        var bakedMeshes = new HashSet<Mesh>();

        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {

            // Skip duplicates
            if (!bakedMeshes.Add(meshFilter.sharedMesh))
            {
                continue;
            }

            // Serialize smooth normals
            var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

            _bakeKeys.Add(meshFilter.sharedMesh);
            _bakeValues.Add(new ListVector3() { Data = smoothNormals });
        }
    }

    private void LoadSmoothNormals()
    {

        // Retrieve or generate smooth normals
        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {

            // Skip if smooth normals have already been adopted
            if (!_registeredMeshes.Add(meshFilter.sharedMesh))
            {
                continue;
            }

            // Retrieve or generate smooth normals
            var index = _bakeKeys.IndexOf(meshFilter.sharedMesh);
            var smoothNormals = (index >= 0) ? _bakeValues[index].Data : SmoothNormals(meshFilter.sharedMesh);

            // Store smooth normals in UV3
            meshFilter.sharedMesh.SetUVs(3, smoothNormals);
        }

        // Clear UV3 on skinned mesh renderers
        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (_registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
            {
                skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];
            }
        }
    }

    private List<Vector3> SmoothNormals(Mesh mesh)
    {

        // Group vertices by location
        var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

        // Copy normals to a new list
        var smoothNormals = new List<Vector3>(mesh.normals);

        // Average normals for grouped vertices
        foreach (var group in groups)
        {

            // Skip single vertices
            if (!group.IEnumerableHasCountOver(1))
            {
                continue;
            }

            // Calculate the average normal
            Vector3 smoothNormal = Vector3.zero;

            foreach (var pair in group)
            {
                smoothNormal += mesh.normals[pair.Value];
            }

            smoothNormal.Normalize();

            // Assign smooth normal to each vertex
            foreach (var pair in group)
            {
                smoothNormals[pair.Value] = smoothNormal;
            }
        }

        return smoothNormals;
    }

    private void UpdateMaterialProperties()
    {

        // Apply properties according to mode
        _outlineFillMaterial.SetColor("_OutlineColor", _outlineColor);

        switch (_outlineMode)
        {
            case Mode.OutlineAll:
                _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMaterial.SetFloat("_OutlineWidth", _outlineWidth);
                break;

            case Mode.OutlineVisible:
                _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                _outlineFillMaterial.SetFloat("_OutlineWidth", _outlineWidth);
                break;

            case Mode.OutlineHidden:
                _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                _outlineFillMaterial.SetFloat("_OutlineWidth", _outlineWidth);
                break;

            case Mode.OutlineAndSilhouette:
                _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMaterial.SetFloat("_OutlineWidth", _outlineWidth);
                break;

            case Mode.SilhouetteOnly:
                _outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                _outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                _outlineFillMaterial.SetFloat("_OutlineWidth", 0);
                break;
        }
    }
}
