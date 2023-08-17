using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeArea : MonoBehaviour
{
    private float FILL_RATE_RANGE = 1f;

    public GameObject player;
    public TMP_Text moneyTMP;
    public Simulator simulator;
    public ResourceManager resourceManager;
    public Util util;

    public MeshRenderer meshRenderer;
    public Material material;
    public MaterialPropertyBlock propertyBlock;
    public ParticleSystem particleSystem;
    public Vector3 initialScale;

    public float fillRate = -0.5f;
    private float fillRateSpeed = 0.002f;
    public float maxFillRate = 0.5f;

    public bool isInside = false;

    public int requireValue = 200;
    public int remainRequireValue;
    public int valueInEachTime;

    public int index;

    public delegate void UpgradeProgressCallback();

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();

        particleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();

        initialScale = transform.localScale;

        remainRequireValue = requireValue;

        fillRateSpeed = ((float)valueInEachTime / requireValue) * FILL_RATE_RANGE;
    }

    private void OnTriggerEnter(Collider other)
    {
        isInside = true;

        StartCoroutine(
           simulator.PuttingMoneyEffect(
               player.transform,
               transform,
               this
           )
        );

        StartCoroutine(util.ScaleEffect(transform, true, 1.2f * initialScale));

        simulator.tutorialArrow.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        particleSystem.gameObject.SetActive(false);

        isInside = false;

        StartCoroutine(util.ScaleEffect(transform, false, initialScale));
    }

    public IEnumerator UpgradeProgress(UpgradeProgressCallback OnUpgraded)
    {
        while (fillRate < maxFillRate)
        {
            if (isInside)
            {
                if (resourceManager.money > valueInEachTime)
                {
                    fillRate += fillRateSpeed;
                    /*material.SetFloat("_FillRate", fillRate);*/
                    propertyBlock.SetFloat("_FillRate", fillRate);
                    meshRenderer.SetPropertyBlock(propertyBlock);

                    resourceManager.money -= valueInEachTime;
                    remainRequireValue -= valueInEachTime;
                    simulator.upgradeMoneyTMPs[index].text = remainRequireValue.ToString();
                    moneyTMP.text = "$" + resourceManager.money;
                }

                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        OnUpgraded();
    }
}