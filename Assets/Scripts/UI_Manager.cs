using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public RectTransform moneyTMPBackground;
    public TMP_Text moneyTMP;
    public RectTransform moneyImageRT;
    public TMP_Text moneyTakenTMP;

    public RectTransform upgradeScreenRT;
    public TMP_Text upgradeScreenTitle;
    public RectTransform upgradeOptionBackgroundRT;
    public RectTransform[] upgradeOptionBackgroundRTs;
    public TMP_Text[] upgradeOptionTitles;
    public TMP_Text[] upgradeOptionTexts;
    public Button[] upgradeOptionButtons;
    public TMP_Text[] upgradeOptionButtonTexts;
    public Button closeUpgradeScreenButton;

    public RectTransform moneyTakenRT;
    public RectTransform customerDialogRT;
    public RectTransform noSeatDialogRT;
    public RectTransform maxCapacityRT;
    public RectTransform maxStorageCapacityRT;
    public RectTransform happyEmojiRT;

    public TMP_Text tutorialText;

    public ResourceManager resourceManager;
    public PlayerController playerController;
    public NPC_Manager npcManager;
    public Util util;

    Vector2 screenSize;

    PlayerState savedPlayerState;

    // Start is called before the first frame update
    void Start()
    {
        screenSize.x = Screen.currentResolution.width;
        screenSize.y = Screen.currentResolution.height;

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
            upgradeOptionButtonTexts[i] = upgradeOptionButtons[i].GetComponent<RectTransform>()
                .GetChild(0).GetComponent<TMP_Text>();
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



        closeUpgradeScreenButton.onClick.AddListener(
            () =>
            {
                upgradeScreenRT.gameObject.SetActive(false);
                playerController.playerState = savedPlayerState;
            }
        );


        /* customerDialogRT.localScale = 0.0025f * new Vector3(
             upgradeScreenRT.sizeDelta.x,
             upgradeScreenRT.sizeDelta.x,
             upgradeScreenRT.sizeDelta.x
         );
 */

        /*  moneyTakenRT.localScale = new Vector3(
              0.1f*screenSize.x,0.05f*screenSize.y,0
          );

          noSeatDialogRT.sizeDelta = new Vector3(
              0.12f * screenSize.x, 0.08f * screenSize.y, 0
          );

          customerDialogRT.sizeDelta = new Vector3(
              0.12f * screenSize.x, 0.06f * screenSize.y, 0
          );

          maxCapacityRT.sizeDelta = new Vector3(
             0.1f * screenSize.x, 0.05f * screenSize.y, 0
          );

          maxStorageCapacityRT.sizeDelta = new Vector3(
             0.1f * screenSize.x, 0.05f * screenSize.y, 0
          );

          happyEmojiRT.sizeDelta = new Vector3(
             0.07f * screenSize.x, 0.07f * screenSize.y, 0
          );*/

        customerDialogRT.localScale = 0.0015f * new Vector3(
           screenSize.x,
           screenSize.x,
           screenSize.x
        );

        noSeatDialogRT.localScale = 0.001f * new Vector3(
           screenSize.x,
           screenSize.x,
           screenSize.x
        );

        SetFontSize();

        moneyImageRT.sizeDelta = new Vector3(
            1.3f * moneyTMP.preferredHeight, 1.3f * 0.6f * moneyTMP.preferredHeight, 0
        );

        SetMainMoneyUI();
    }

    public void SetFontSize()
    {
        moneyTMP.fontSize = (int)(screenSize.x / 25);
        tutorialText.fontSize = (int)(screenSize.x / 25);
        upgradeScreenTitle.fontSize = (int)(screenSize.x / 25);

        for (int i = 0; i < upgradeOptionBackgroundRTs.Length; i++)
        {
            upgradeOptionTitles[i].fontSize = (int)(screenSize.x / 25);
            upgradeOptionTexts[i].fontSize = (int)(screenSize.x / 25);
            upgradeOptionButtonTexts[i].fontSize = (int)(screenSize.x / 30);
        }

        closeUpgradeScreenButton.GetComponent<RectTransform>()
            .GetChild(0).GetComponent<TMP_Text>().fontSize = (int)(screenSize.x / 25);
    }

    public void OpenHRUpgradeScreen()
    {
        savedPlayerState = playerController.playerState;
        playerController.playerState = PlayerState.InteratingUI;

        upgradeScreenTitle.text = "HR Upgrade";

        upgradeOptionTitles[2].text = "Add Staff";

        upgradeOptionTexts[0].text = npcManager.npcControllers[0].speed +
            " --> " + (npcManager.npcControllers[0].speed + 1);
        upgradeOptionTexts[1].text = npcManager.npcControllers[0].capacity +
            " --> " + (npcManager.npcControllers[0].capacity + 1);
        upgradeOptionTexts[2].text = "";

        for (int i = 0; i < upgradeOptionButtons.Length; i++)
        {
            upgradeOptionButtonTexts[i].text = "$" + resourceManager.staffUpgradeCosts[i];
        }

        upgradeOptionButtons[0].onClick.RemoveAllListeners();
        upgradeOptionButtons[1].onClick.RemoveAllListeners();
        upgradeOptionButtons[2].onClick.RemoveAllListeners();
        upgradeOptionButtons[0].onClick.AddListener(OnUpgradeStaffSpeed);
        upgradeOptionButtons[1].onClick.AddListener(OnUpgradeStaffCapacity);
        upgradeOptionButtons[2].onClick.AddListener(OnAddStaff);

        upgradeScreenRT.gameObject.SetActive(true);

        StartCoroutine(CheckUpgradeScreenClose());
    }

    public void OpenPlayerUpgradeScreen()
    {
        savedPlayerState = playerController.playerState;
        playerController.playerState = PlayerState.InteratingUI;

        upgradeScreenTitle.text = "Player Upgrade";

        upgradeOptionTitles[2].text = "Profit Multiplier";

        upgradeOptionTexts[0].text = playerController.speed + " --> " + playerController.speed + 1;
        upgradeOptionTexts[1].text = playerController.capacity + " --> " + (playerController.capacity + 1);
        upgradeOptionTexts[2].text = playerController.profitMultiplier + " --> " + (1.1f * playerController.profitMultiplier);

        for (int i = 0; i < upgradeOptionButtons.Length; i++)
        {
            upgradeOptionButtonTexts[i].text = "$" + resourceManager.playerUpgradeCosts[i];
        }

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
        if (resourceManager.money >= resourceManager.playerUpgradeCosts[0])
        {
            resourceManager.UpgradePlayerMoveSpeed();

            resourceManager.OnSpendingInGameMoney(resourceManager.playerUpgradeCosts[0]);

            resourceManager.playerUpgradeCosts[0] += (int)0.8f * resourceManager.playerUpgradeCosts[0];

            upgradeOptionTexts[0].text = (int)playerController.speed + " --> " + (int)(playerController.speed + 1);
            upgradeOptionButtonTexts[0].text = "$" + util.ToShortFormNumber((int)resourceManager.playerUpgradeCosts[0]);
        }
    }

    void OnUpgradePlayerCapacity()
    {
        if (resourceManager.money >= resourceManager.playerUpgradeCosts[1])
        {
            resourceManager.UpgradePlayerCapicity();

            resourceManager.OnSpendingInGameMoney(resourceManager.playerUpgradeCosts[1]);

            resourceManager.playerUpgradeCosts[1] += (int)0.8f * resourceManager.playerUpgradeCosts[1];

            upgradeOptionTexts[1].text = playerController.capacity + " --> " + (playerController.capacity + 1);
            upgradeOptionButtonTexts[1].text = "$" + util.ToShortFormNumber((int)resourceManager.playerUpgradeCosts[1]);
        }
    }

    void OnUpgradePlayerProfit()
    {
        if (resourceManager.money >= resourceManager.playerUpgradeCosts[2])
        {
            resourceManager.UpgradePlayerProfit();

            resourceManager.OnSpendingInGameMoney(resourceManager.playerUpgradeCosts[2]);

            resourceManager.playerUpgradeCosts[2] += (int)0.8f * resourceManager.playerUpgradeCosts[2];

            upgradeOptionTexts[2].text = (int)playerController.profitMultiplier + " --> " +
                (int)(1.1f * playerController.profitMultiplier);
            upgradeOptionButtonTexts[2].text = "$" + util.ToShortFormNumber((int)resourceManager.playerUpgradeCosts[2]);
        } 
    }

    void OnUpgradeStaffSpeed()
    {
        if (resourceManager.money >= resourceManager.staffUpgradeCosts[0])
        {
            for (int i = 0; i < npcManager.npcs.Length; i++)
            {
                npcManager.npcControllers[i].speed++;
            }

            resourceManager.OnSpendingInGameMoney(resourceManager.staffUpgradeCosts[0]);

            resourceManager.staffUpgradeCosts[0] += (int)0.8f * resourceManager.staffUpgradeCosts[0];

            upgradeOptionTexts[1].text = (int)npcManager.npcControllers[0].speed + " --> " +
            (int)(npcManager.npcControllers[0].speed + 1);
            upgradeOptionButtonTexts[0].text = "$" + util.ToShortFormNumber((int)resourceManager.staffUpgradeCosts[0]);
        }
    }

    void OnUpgradeStaffCapacity()
    {
        if (resourceManager.money >= resourceManager.staffUpgradeCosts[1])
        {
            for (int i = 0; i < npcManager.npcs.Length; i++)
            {
                npcManager.npcControllers[i].capacity++;
            }

            resourceManager.OnSpendingInGameMoney(resourceManager.staffUpgradeCosts[1]);

            resourceManager.staffUpgradeCosts[1] += (int)0.8f * resourceManager.staffUpgradeCosts[1];

            upgradeOptionTexts[1].text = (int)npcManager.npcControllers[0].capacity + " --> " +
            (int)(npcManager.npcControllers[0].capacity + 1);
            upgradeOptionButtonTexts[1].text = "$" + util.ToShortFormNumber((int)resourceManager.staffUpgradeCosts[1]);
        }
    }

    void OnAddStaff()
    {
        if (resourceManager.money >= resourceManager.staffUpgradeCosts[2])
        {
            npcManager.AddStaff();

            resourceManager.OnSpendingInGameMoney(resourceManager.staffUpgradeCosts[2]);

            resourceManager.staffUpgradeCosts[2] += (int)0.8f * resourceManager.staffUpgradeCosts[2];

            upgradeOptionButtonTexts[2].text = "$" + util.ToShortFormNumber(resourceManager.staffUpgradeCosts[2]);
        }
    }

/*    void OnUpgrade(int upgradeCost)
    {
        if (resourceManager.money >= upgradeCost)
        {
            for (int i = 0; i < npcManager.npcs.Length; i++)
            {
                npcManager.npcControllers[i].capacity++;
            }

            resourceManager.OnSpendingInGameMoney(upgradeCost);

            resourceManager.staffUpgradeCosts[1] += 0.8f * upgradeCost;

            upgradeOptionTexts[1].text = (int)npcManager.npcControllers[0].capacity + " --> " +
            (int)(npcManager.npcControllers[0].capacity + 1);
            upgradeOptionButtonTexts[1].text = "$" + util.ToShortFormNumber((int)resourceManager.staffUpgradeCosts[1]);
        }
    }*/

    public void SetMainMoneyUI()
    {
        moneyTMP.text = util.ToShortFormNumber(resourceManager.money);
        moneyTMP.rectTransform.sizeDelta = new Vector3(
            moneyTMP.preferredWidth, moneyTMP.preferredHeight, 0
        );

        moneyTMPBackground.sizeDelta = 1.2f * new Vector3(
            0.5f * moneyImageRT.sizeDelta.x + moneyTMP.preferredWidth,
            moneyTMP.preferredHeight,
            0
        );

        moneyTMPBackground.anchoredPosition =
            -0.5f * new Vector3(moneyTMPBackground.sizeDelta.x, moneyTMPBackground.sizeDelta.y, 0)
            - 0.05f * new Vector3(screenSize.x, screenSize.x, 0);

        moneyTMP.rectTransform.anchoredPosition = new Vector3(
           -0.6f * moneyTMP.rectTransform.sizeDelta.x,
           0,
           0
        );
    }
}
