using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeArea : MonoBehaviour
{
    public GameObject player;
    private MeshRenderer meshRenderer;
    public Material material;
    public float fillRateSpeed = 0.002f;
    public float maxFillRate = 1f;

    private bool isInside = false;
    private float fillRate = -0.5f;    

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isInside)
        {
            if(fillRate < maxFillRate)
            {
                fillRate += fillRateSpeed;
                material.SetFloat("_FillRate", fillRate);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isInside = false;
    }
}