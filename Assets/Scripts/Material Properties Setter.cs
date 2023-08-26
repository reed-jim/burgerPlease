using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPropertiesSetter : MonoBehaviour
{
    public Color color;

    private MaterialPropertyBlock propertyBlock;

    private void Start()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        Renderer renderer = GetComponent<Renderer>();

        propertyBlock.SetColor("_BaseColor", color);

        renderer.SetPropertyBlock(propertyBlock);
    }

    void OnValidate()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
       
        Renderer renderer = GetComponent<Renderer>();
   
        propertyBlock.SetColor("_BaseColor", color);
      
        renderer.SetPropertyBlock(propertyBlock);
    }
}
