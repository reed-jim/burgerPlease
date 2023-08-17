using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPropertiesSetter : MonoBehaviour
{
    public float fillRate;

    //The material property block we pass to the GPU
    private MaterialPropertyBlock propertyBlock;

    void OnValidate()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        Renderer renderer = GetComponent<Renderer>();

        propertyBlock.SetFloat("_FillRate", fillRate);

        renderer.SetPropertyBlock(propertyBlock);
    }
}
