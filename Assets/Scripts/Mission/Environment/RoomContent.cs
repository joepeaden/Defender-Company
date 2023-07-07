using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For any decoration, object, or furniture in a room that needs to be toggled when the room is entered or exited.
/// </summary>
public class RoomContent : MonoBehaviour, ISetActive
{
    [SerializeField] private Material transparentMaterial;

    private Material[] defaultMaterials;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        defaultMaterials = meshRenderer.materials;

        DeActivate();
    }

    /// <summary>
    /// Set all the materials for the MeshRenderer to the transparent material, or the opposite.
    /// </summary>
    public void Activate()
    {
        meshRenderer.materials = defaultMaterials;
    }

    public void DeActivate()
    {
        int len = meshRenderer.materials.Length;
        Material[] newMat = new Material[len];

        for (int i = 0; i < len; i++)
        {
            newMat[i] = transparentMaterial;
        }

        meshRenderer.materials = newMat;
    }
}
