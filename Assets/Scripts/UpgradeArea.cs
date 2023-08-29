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
    public UI_Manager uiManager;
    public Util util;

    public AudioSource puttingMoneySound;

    public MeshRenderer meshRenderer;
    public Material material;
    public MaterialPropertyBlock propertyBlock;
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

    private void Awake()
    {
        puttingMoneySound.clip.LoadAudioData();
    }

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();

        initialScale = transform.localScale;

        remainRequireValue = requireValue;

        fillRateSpeed = ((float)valueInEachTime / requireValue) * FILL_RATE_RANGE;
    }

    private void OnTriggerEnter(Collider other)
    {
        isInside = true;

        valueInEachTime = (int)(requireValue / 20);
        fillRateSpeed = ((float)valueInEachTime / requireValue) * FILL_RATE_RANGE;

        if (resourceManager.money >= valueInEachTime)
        {
            StartCoroutine(
              simulator.PuttingMoneyEffect(
                  player.transform,
                  transform,
                  this,
                  valueInEachTime
              )
           );
        }

        StartCoroutine(util.ScaleEffect(transform, true, 1.15f * initialScale));

        puttingMoneySound.Play();

        simulator.tutorialArrow.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        isInside = false;

        StartCoroutine(util.ScaleEffect(transform, false, initialScale));

        puttingMoneySound.Stop();
    }

    public IEnumerator UpgradeProgress(UpgradeProgressCallback OnUpgraded)
    {
        while (fillRate < maxFillRate)
        {
            if (isInside)
            {
                if (resourceManager.money >= valueInEachTime)
                {
                    fillRate += fillRateSpeed;
                    propertyBlock.SetFloat("_FillRate", fillRate);
                    meshRenderer.SetPropertyBlock(propertyBlock);

                    resourceManager.money -= valueInEachTime;
                    remainRequireValue -= valueInEachTime;
                    simulator.upgradeMoneyTMPs[index].text = util.ToShortFormNumber(remainRequireValue);

                    uiManager.SetMainMoneyUI();
                    
                   /* moneyTMP.text = "$" + util.ToShortFormNumber(resourceManager.money);
                    moneyTMP.rectTransform.sizeDelta =
                        new Vector2(moneyTMP.preferredWidth, moneyTMP.preferredHeight);
                    simulator.moneyBackground.sizeDelta =
                        new Vector2(1.1f * simulator.moneyTMP.preferredWidth, 1.1f * simulator.moneyTMP.preferredHeight);*/
                }

                yield return new WaitForSeconds(0.04f);
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
            }
        }

        puttingMoneySound.Stop();

        OnUpgraded();
    }

    public void SetNewRequireValue(int value)
    {
        requireValue = value;
        remainRequireValue = requireValue;

        fillRateSpeed = ((float)valueInEachTime / requireValue) * FILL_RATE_RANGE;
    }

    public void SetSizeEqualTo(Vector3 targetSize, Vector3 targetScale)
    {
        Vector3 ratio = Vector3.one;
        
        ratio.x = meshRenderer.bounds.size.x / targetSize.x;
        ratio.z = meshRenderer.bounds.size.z / targetSize.z;

        transform.localScale = new Vector3(
            transform.localScale.x / ratio.x,
            transform.localScale.y,
            transform.localScale.z / ratio.z
        );

        initialScale = transform.localScale;
    }
}