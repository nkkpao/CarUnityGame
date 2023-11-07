using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class CarView : MonoBehaviour
{
    [SerializeField] MeshRenderer carBodyMesh;
    [SerializeField] List<GameObject> wheels;
    [SerializeField] List<MeshRenderer> rimMeshes;

    /*public void SetCarColorAndRims()
    {
        Material[] carBodyMaterials = CarBodyMaterials;
        foreach (Material material in carBodyMaterials)
        {
            material.color = GameManager.CarColor;
        }
        if (GameManager.RimMaterial != null)
        {
            RimMaterial = GameManager.RimMaterial;
        }

    }*/

    public void SetCarColor(Color color)
    {
        Material[] carBodyMaterials = GetCarBodyMaterials();
        foreach (Material material in carBodyMaterials)
        {
            //material.color = GameManager.CarColor;
        }
    }

    public void SetRimMaterial(Material material)
    {
        foreach(MeshRenderer meshRenderer in rimMeshes)
        {
            Material[] materials = meshRenderer.materials;
            materials[0] = material;
            meshRenderer.materials = materials;
        }
    }

    public Material GetRimMaterial()
    {
        Material[] materials = rimMeshes[0].materials;
        return materials[0];
    }

    public Material[] GetCarBodyMaterials()
    {
        MeshRenderer meshRenderer = carBodyMesh.GetComponent<MeshRenderer>();
        return meshRenderer.materials;
    }
}
