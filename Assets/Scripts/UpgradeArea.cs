using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeArea : MonoBehaviour
{
    public GameObject player;
    public Simulator simulator;
    public ResourceManager resourceManager;

    private float FILL_RATE_RANGE = 1.5f;

    public Material material;

    private float fillRate = -0.5f;
    private float fillRateSpeed = 0.002f;
    public float maxFillRate = 1f;

    private bool isInside = false;

    public int requireValue = 200;
    public int remainRequireValue;
    public int valueInEachTime;

    // Start is called before the first frame update
    void Start()
    {
        remainRequireValue = requireValue;

        fillRateSpeed = ((float)valueInEachTime / requireValue) * FILL_RATE_RANGE;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInside)
        {
            if (resourceManager.money > valueInEachTime)
            {
                if (fillRate < maxFillRate)
                {
                    fillRate += fillRateSpeed;
                    material.SetFloat("_FillRate", fillRate);

                    resourceManager.money -= valueInEachTime;
                    remainRequireValue -= valueInEachTime;
                    simulator.upgradeMoneyTMP[0].text = "$" + remainRequireValue;
                }
                else
                {
                    Destroy(gameObject);
                }
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