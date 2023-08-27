using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public TMP_Text moneyTMP;
    public TMP_Text moneyTakenTMP;

    public RectTransform upgradeScreenRT;
    public TMP_Text upgradeScreenTitle;
    public RectTransform upgradeOptionBackgroundRT;
    public RectTransform[] upgradeOptionBackgroundRTs;
    public TMP_Text[] upgradeOptionTitles;
    public TMP_Text[] upgradeOptionTexts;
    public Button[] upgradeOptionButtons;
    public Button closeUpgradeScreenButton;

    public RectTransform customerDialogRT;

    public ResourceManager resourceManager;
    public PlayerController playerController;
    public NPC_Manager npcManager;

    Vector2 screenSize;

    // Start is called before the first frame update
    void Start()
    {
        screenSize.x = Screen.currentResolution.width;
        screenSize.y = Screen.currentResolution.height;

        moneyTMP.text = resourceManager.money.ToString();

        Initialize();
        SetUI();
    }

    void Initialize()
    {
        for (int i = 0; i < upgradeOptionBackgroundRTs.Length; i++)
        {
            upgradeOptionBackgroundRTs[i] = Instantiate(upgradeOptionBackgroundRT, upgradeOptionBackgroundRT.parent);

            upgradeOptionTitles[i] = upgradeOptionBackgroundRTs[i].GetChild(0).GetComponent<TMP_Text>();
            upgradeOptionTexts[i] = upgradeOptionBackgroundRTs[i].GetChild(1).GetComponent<TMP_Text>();
            upgradeOptionButtons[i] = upgradeOptionBackgroundRTs[i].GetChild(2).GetComponent<Button>();
        }

        upgradeOptionTitles[1].text = "Capacity";
        upgradeOptionButtons[2].onClick.AddListener(OnAddStaff);

        Destroy(upgradeOptionBackgroundRT.gameObject);
    }

    void SetUI()
    {
        RectTransform closeUpgradeScreenButtonRT = closeUpgradeScreenButton.GetComponent<RectTransform>();

        upgradeScreenRT.sizeDelta = new Vector2(0.95f * screenSize.x, 0.4f * screenSize.y);
        upgradeScreenTitle.rectTransform.sizeDelta =
            new Vector2(0.9f * upgradeScreenRT.sizeDelta.x, 0.15f * upgradeScreenRT.sizeDelta.y);
        upgradeScreenTitle.rectTransform.anchoredPosition =
           new Vector2(0, -0.5f * upgradeScreenTitle.rectTransform.sizeDelta.y);
        closeUpgradeScreenButtonRT.sizeDelta =
           new Vector2(0.9f * upgradeScreenRT.sizeDelta.x, 0.1f * upgradeScreenRT.sizeDelta.y);
        closeUpgradeScreenButtonRT.anchoredPosition = new Vector2(
            0, 0.5f * closeUpgradeScreenButtonRT.sizeDelta.y + 0.025f * upgradeScreenRT.sizeDelta.y
        );

        for (int i = 0; i < upgradeOptionBackgroundRTs.Length; i++)
        {
            upgradeOptionBackgroundRTs[i].sizeDelta = new Vector2(
                0.84f * upgradeScreenRT.sizeDelta.x / 3,
                0.7f * upgradeScreenRT.sizeDelta.y
            );

            upgradeOptionBackgroundRTs[i].anchoredPosition = new Vector3(
                (i - 1) * (upgradeOptionBackgroundRTs[i].sizeDelta.x + 0.06f * upgradeScreenRT.sizeDelta.x / 2),
                0,
                0
            );

            upgradeOptionTitles[i].rectTransform.sizeDelta = new Vector2(
                upgradeOptionBackgroundRTs[i].sizeDelta.x,
                0.3f * upgradeOptionBackgroundRTs[i].sizeDelta.y
            );

            upgradeOptionButtons[i].GetComponent<RectTransform>().sizeDelta = new Vector2(
                0.9f * upgradeOptionBackgroundRTs[i].sizeDelta.x,
                0.15f * upgradeOptionBackgroundRTs[i].sizeDelta.y
            );
        }



        customerDialogRT.localScale = 0.0025f * new Vector3(
            upgradeScreenRT.sizeDelta.x,
            upgradeScreenRT.sizeDelta.x,
            upgradeScreenRT.sizeDelta.x
        );



        closeUpgradeScreenButton.onClick.AddListener(
            () =>
            {
                upgradeScreenRT.gameObject.SetActive(false);
            }
        );
    }

    public void OpenHRUpgradeScreen()
    {
        upgradeScreenTitle.text = "HR Upgrade";

        upgradeOptionTitles[2].text = "Add Staff";

        /*upgradeOptionButtons[0].onClick.RemoveAllListeners();
        upgradeOptionButtons[1].onClick.RemoveAllListeners();
        upgradeOptionButtons[2].onClick.RemoveAllListeners();
        upgradeOptionButtons[0].onClick.AddListener(resourceManager.UpgradeMoveSpeed);
        upgradeOptionButtons[1].onClick.AddListener(resourceManager.UpgradeMoveSpeed);
        upgradeOptionButtons[2].onClick.AddListener(resourceManager.UpgradeMoveSpeed);*/

        upgradeOptionButtons[0].onClick.RemoveAllListeners();
        upgradeOptionButtons[1].onClick.RemoveAllListeners();
        upgradeOptionButtons[2].onClick.RemoveAllListeners();
        upgradeOptionButtons[2].onClick.AddListener(OnAddStaff);

        upgradeScreenRT.gameObject.SetActive(true);

        StartCoroutine(CheckUpgradeScreenClose());
    }

    public void OpenPlayerUpgradeScreen()
    {
        upgradeScreenTitle.text = "Player Upgrade";

        upgradeOptionTitles[2].text = "Profit Multiplier";

        upgradeOptionTexts[0].text = playerController.speed + " --> " + (1.1f * playerController.speed);
        upgradeOptionTexts[1].text = playerController.capacity + " --> " + (playerController.capacity + 1);
        upgradeOptionTexts[2].text = playerController.profitMultiplier + " --> " + (1.1f * playerController.profitMultiplier);

        upgradeOptionButtons[0].onClick.RemoveAllListeners();
        upgradeOptionButtons[1].onClick.RemoveAllListeners();
        upgradeOptionButtons[2].onClick.RemoveAllListeners();
        upgradeOptionButtons[0].onClick.AddListener(OnUpgradePlayerSpeed);
        upgradeOptionButtons[1].onClick.AddListener(OnUpgradePlayerCapacity);
        upgradeOptionButtons[2].onClick.AddListener(OnUpgradePlayerProfit);

        upgradeScreenRT.gameObject.SetActive(true);

        StartCoroutine(CheckUpgradeScreenClose());
    }

    IEnumerator CheckUpgradeScreenClose()
    {
        yield return new WaitForSeconds(200f);

        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                if (
                    !RectTransformUtility.RectangleContainsScreenPoint
                    (
                        upgradeScreenRT,
                        Input.mousePosition,
                        Camera.main
                    )
                )
                {
                    upgradeScreenRT.gameObject.SetActive(false);
                    break;
                }
            }

            yield return new WaitForSeconds(0.02f);
        }
    }

    void OnUpgradePlayerSpeed()
    {
        resourceManager.UpgradePlayerMoveSpeed();

        upgradeOptionTexts[0].text = (int)playerController.speed + " --> " + (int)(1.1f * playerController.speed);
    }

    void OnUpgradePlayerCapacity()
    {
        resourceManager.UpgradePlayerCapicity();

        upgradeOptionTexts[1].text = playerController.capacity + " --> " + (playerController.capacity + 1);
    }

    void OnUpgradePlayerProfit()
    {
        resourceManager.UpgradePlayerProfit();

        upgradeOptionTexts[2].text = (int)playerController.profitMultiplier + " --> " +
            (int)(1.1f * playerController.profitMultiplier);
    }

    void OnUpgradeStaffSpeed()
    {
        for (int i = 0; i < npcManager.npcs.Length; i++)
        {
            npcManager.npcControllers[i].speed *= 1.1f;
        }

        upgradeOptionTexts[1].text = (int)npcManager.npcControllers[0].speed + " --> " +
            (int)(1.1f * npcManager.npcControllers[0].speed);
    }

    void OnUpgradeStaffCapacity()
    {
        for (int i = 0; i < npcManager.npcs.Length; i++)
        {
            npcManager.npcControllers[i].capacity++;
        }

        upgradeOptionTexts[1].text = npcManager.npcControllers[0].capacity + " --> " + (npcManager.npcControllers[0].capacity + 1);
    }

    void OnAddStaff()
    {
        npcManager.AddStaff();
    }
}
