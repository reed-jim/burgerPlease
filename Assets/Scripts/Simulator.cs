using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public enum FoodState
{
    NotSpawn,
    Wait,
    Picking,
    Putting,
    Delivering,
    ReachCounter,
    PuttingInCounter,
    MovingWithPackage,
    CustomerPick,
    CustomerPicking,
    CustomerPicked,
    InPackage,
    InTakeawayCounter
}

public enum CustomerState
{
    NotSpawn,
    SetDestination,
    GoInside,
    Waiting,
    FirstQueue,
    PickingFood,
    ChooseTable,
    GetToTable,
    Eating,
    Leaving
}

public enum TableState
{
    NotSpawn,
    EmptyBoth,
    EmptyLeft,
    EmptyRight,
    Full,
    Dirty
}

public enum StoveState
{
    NotSpawn,
    Active,
    PauseSpawn
}

public enum PackageState
{
    NotSpawn,
    Filling,
    Moving,
    Filled,
    Putting,
    InTakeawayCounter
}

public enum CarState
{
    NotSpawn,
    Waiting,
    SetDirection,
    GoToMainLane,
    SetTurnDirection,
    GoToStore,
    WaitingOrder,
    TakingFood,
    Done
}

public enum TrashState
{
    NotSpawn,
    OnTable,
    Picking,
    Holding,
    MovingToTrashCan
}

public enum UpgradeAreaState
{
    NotSpawn,
    Active
}

public enum UpgradeAreaFunction
{
    None,
    Unidentified,
    CreateGate,
    CreateFirstTable,
    CreateTable,
    CreateStove,
    CreateCounter,
    AddCashier,
    CreatePackageTable,
    CreateTakeawayCounter,
    CreateHRUpgrade,
    CreatePlayerUpgrade
}

public enum DirectionArrowState
{
    NotSpawn,
    Set,
    Directing
}

public class Simulator : MonoBehaviour
{
    public GameObject customerPrefab;
    public GameObject foodPrefab;
    public GameObject trashPrefab;
    public GameObject packagePrefab;
    public GameObject carPrefab;
    public GameObject effectMoneyPrefab;
    public GameObject upgradeArea;
    public TMP_Text upgradeMoneyTMP;

    public GameObject player;
    public GameObject gate;
    public GameObject counter;
    public GameObject baseStove;
    public GameObject cashierPosition;
    public GameObject bowl;
    public GameObject packageTable;
    public GameObject takeawayCounter;
    public GameObject trashCan;
    public GameObject tutorialArrow;
    public GameObject directionArrow;

    public GameObject customerDialog;
    public GameObject noSeatDialog;
    public GameObject loadGameScreen;
    public GameObject loadGameScreenBackground;
    public GameObject loadBar;
    public GameObject loadBarEmpty;
    public TMP_Text tutorialText;
    public RectTransform tutorialTextBackground;
    public GameObject happyEmoji;
    public GameObject[] happyEmojis;
    public TMP_Text baseCapacityTMP;
    public TMP_Text[] maxCapacityTMPs;
    public TMP_Text moneyTMP;
    public RectTransform moneyBackground;

    public GameObject[] tables;
    public GameObject[] stoves;
    public GameObject[] foodStorages;
    public GameObject[] customers = new GameObject[10];
    public GameObject[] foods = new GameObject[36];
    public GameObject[] packages = new GameObject[10];
    public GameObject[] cars = new GameObject[10];
    private GameObject[] chairs;
    public GameObject[] officeDesk;
    public GameObject[] effectMoneys;
    public GameObject[] trashs;
    public GameObject[] upgradeAreas;
    public UpgradeArea[] upgradeAreaScripts;
    public TMP_Text[] upgradeMoneyTMPs;

    public float speed = 5f;
    public Vector3 minDistance;
    public float customerDistance;

    public MoneyPileManager moneyPileManager;
    public NPC_Manager npcManager;
    public PlayerController playerController;
    public Util util;
    public GameProgressManager gameProgressManager;
    public ResourceManager resourceManager;
    public FollowPlayerCamera cameraScript;
    private MaterialPropertyBlock propertyBlock;

    public GameObject loadBackground;
    public GameObject customerSectionFloor;
    public GameObject staffSectionFloor;

    public AudioSource upgradeCompleteSound;
    public AudioSource talkingSound;
    public AudioSource cookingSound;

    public List<int> customerList;
    public CustomerState[] customerStates;
    public int[] customerNumFoodDemand;
    public FoodState[] foodStates;
    public string[] foodBelongTo;
    public int[] foodColumnIndex;
    public TableState[] tableStates;
    public StoveState[] foodStorageStates;
    public int[] numFoodOfStorage;
    public string[] customerTableMap;
    public CarState[] carStates;
    public PackageState[] packageStates;
    public string[] packageBelongTo;
    public int[] packageColumnIndex;
    public int[] foodIndexInPackage;
    public UpgradeAreaState[] upgradeAreaStates;
    public UpgradeAreaFunction[] upgradeAreaFunctions;
    public TrashState[] trashStates;
    public string[] trashBelongTo;
    public int[] trashColumnIndex;
    public DirectionArrowState directionArrowState;

    public Vector3 stoveSize;
    public Vector3 counterSize;
    public Vector3 foodSize;
    public Vector3 tableSize;
    public Vector3 officeDeskSize;
    public Vector3 trashSize;
    public Vector3 packageSize;
    public Vector3 packageTableSize;
    public Vector3 takeawayCounterSize;

    private float deltaTime;

    public delegate void CurveMoveCallback();
    public delegate void SpawnUpgradeAreaCallback();

    void SetUISize()
    {
        RectTransform loadBarEmptyRT = loadBarEmpty.GetComponent<RectTransform>();
        RectTransform loadBarRT = loadBar.GetComponent<RectTransform>();

        loadGameScreen.GetComponent<RectTransform>().sizeDelta =
            new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        loadGameScreenBackground.GetComponent<RectTransform>().sizeDelta =
            new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        moneyTMP.rectTransform.sizeDelta =
            new Vector2(moneyTMP.preferredWidth, moneyTMP.preferredHeight);
        moneyBackground.sizeDelta =
            new Vector2(1.1f * moneyTMP.preferredWidth, 1.1f * moneyTMP.preferredHeight);
        moneyBackground.anchoredPosition = new Vector3(
            -0.5f * moneyBackground.sizeDelta.x - 0.05f * Screen.currentResolution.width,
            -0.5f * moneyBackground.sizeDelta.y - 0.05f * Screen.currentResolution.width,
            0
        );

        loadBarEmptyRT.sizeDelta =
            new Vector2(0.8f * Screen.currentResolution.width, 0.03f * Screen.currentResolution.height);
        loadBarRT.sizeDelta =
            new Vector2(0.8f * Screen.currentResolution.width, 0.03f * Screen.currentResolution.height);
        loadBarEmptyRT.anchoredPosition = new Vector3(0, 3f * loadBarEmptyRT.sizeDelta.y, 0);
        loadBarRT.anchoredPosition = new Vector3(0, 3f * loadBarRT.sizeDelta.y, 0);

        tutorialTextBackground.anchoredPosition = new Vector3(0, -4 * tutorialTextBackground.sizeDelta.y, 0);
    }

    private void Awake()
    {
        upgradeCompleteSound.clip.LoadAudioData();
        talkingSound.clip.LoadAudioData();
        cookingSound.clip.LoadAudioData();
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUISize();
        propertyBlock = new MaterialPropertyBlock();

        foodBelongTo = new string[foods.Length];
        foodColumnIndex = new int[foods.Length];
        foodStates = new FoodState[foods.Length];
        foodIndexInPackage = new int[foods.Length];

        customerStates = new CustomerState[customers.Length];
        customerNumFoodDemand = new int[customers.Length];
        customerTableMap = new string[customers.Length];

        tableStates = new TableState[tables.Length];
        chairs = new GameObject[tables.Length * 2];

        foodStorages = new GameObject[stoves.Length];
        foodStorageStates = new StoveState[stoves.Length];
        numFoodOfStorage = new int[stoves.Length];

        trashStates = new TrashState[trashs.Length];
        trashBelongTo = new string[trashs.Length];
        trashColumnIndex = new int[trashs.Length];

        packageStates = new PackageState[packages.Length];
        packageBelongTo = new string[packages.Length];
        packageColumnIndex = new int[packages.Length];

        carStates = new CarState[cars.Length];

        upgradeAreaScripts = new UpgradeArea[upgradeAreas.Length];
        upgradeAreaStates = new UpgradeAreaState[upgradeAreas.Length];
        upgradeAreaFunctions = new UpgradeAreaFunction[upgradeAreas.Length];
        upgradeMoneyTMPs = new TMP_Text[upgradeAreas.Length];
        maxCapacityTMPs = new TMP_Text[foodStorages.Length];

        deltaTime = Time.deltaTime;
        /*customerSectionFloor.GetComponent<MeshRenderer>().material.color =
            new Color(90 / 255f, 60 / 255f, 50 / 255f);
        staffSectionFloor.GetComponent<MeshRenderer>().material.color =
            new Color(1, 150 / 255f, 140 / 255f);*/


        customerList = new List<int>();
        for (int i = 0; i < customers.Length; i++)
        {
            customers[i] = Instantiate(customerPrefab);

            propertyBlock.SetColor("_BaseColor", new Color(200 / 255f, 200 / 255f, 200 / 255f));
            customers[i].transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().SetPropertyBlock(propertyBlock);

            customers[i].SetActive(false);
            customerStates[i] = CustomerState.NotSpawn;
            customerTableMap[i] = "none";
        }

        for (int i = 0; i < foods.Length; i++)
        {
            foods[i] = Instantiate(foodPrefab);
            foods[i].SetActive(false);
            foodBelongTo[i] = "none";
            foodColumnIndex[i] = i;
            foodStates[i] = FoodState.NotSpawn;
        }

        for (int i = 0; i < packages.Length; i++)
        {
            packages[i] = Instantiate(packagePrefab);
            packages[i].SetActive(false);
            packageStates[i] = PackageState.NotSpawn;
            packages[i].GetComponent<Package>().index = i;
        }

        for (int i = 0; i < cars.Length; i++)
        {
            cars[i] = Instantiate(carPrefab);
            cars[i].SetActive(false);
            carStates[i] = CarState.NotSpawn;
        }

        for (int i = 0; i < tables.Length; i++)
        {
            if (i == 0)
            {
                tableStates[i] = TableState.EmptyBoth;
            }
            else
            {
                tables[i] = Instantiate(tables[0]);
                tables[i].SetActive(false);
                tables[i].GetComponent<Table>().index = i;
                tableStates[i] = TableState.NotSpawn;
            }

            chairs[2 * i] = tables[i].transform.GetChild(1).gameObject;
            chairs[2 * i + 1] = tables[i].transform.GetChild(2).gameObject;
        }

        for (int i = 0; i < stoves.Length; i++)
        {
            stoves[i] = Instantiate(baseStove);
            stoves[i].SetActive(false);
            foodStorages[i] = stoves[i].transform.GetChild(0).gameObject;
            foodStorageStates[i] = StoveState.NotSpawn;
            numFoodOfStorage[i] = 0;
            foodStorages[i].GetComponent<BurgerPickup>().stoveIndex = i;

            maxCapacityTMPs[i] = Instantiate(baseCapacityTMP, baseCapacityTMP.rectTransform.parent);
            maxCapacityTMPs[i].gameObject.SetActive(false);
        }
        Destroy(baseStove);

        for (int i = 0; i < upgradeAreas.Length; i++)
        {
            upgradeAreas[i] = Instantiate(upgradeArea);
            upgradeAreas[i].SetActive(false);
            upgradeAreaScripts[i] = upgradeAreas[i].GetComponent<UpgradeArea>();
            upgradeAreaScripts[i].index = i;
            upgradeMoneyTMPs[i] = Instantiate(upgradeMoneyTMP, upgradeMoneyTMP.rectTransform.parent);
            upgradeMoneyTMPs[i].gameObject.SetActive(false);

            upgradeAreaStates[i] = UpgradeAreaState.NotSpawn;

            if (i == upgradeAreas.Length - 1)
            {
                Destroy(upgradeArea);
            }
        }

        for (int i = 0; i < effectMoneys.Length; i++)
        {
            effectMoneys[i] = Instantiate(effectMoneyPrefab);
            effectMoneys[i].SetActive(false);
        }

        for (int i = 0; i < trashs.Length; i++)
        {
            trashs[i] = Instantiate(trashPrefab);
            trashs[i].SetActive(false);

            trashStates[i] = TrashState.NotSpawn;
            trashColumnIndex[i] = -1;
        }

        for (int i = 0; i < happyEmojis.Length; i++)
        {
            happyEmojis[i] = Instantiate(happyEmoji, happyEmoji.transform.parent);
            happyEmojis[i].SetActive(false);

            if (i == happyEmojis.Length - 1)
            {
                Destroy(happyEmoji);
            }
        }

        directionArrowState = DirectionArrowState.NotSpawn;



        stoveSize = new Vector3(
            stoves[0].GetComponent<MeshRenderer>().bounds.size.x,
            stoves[0].GetComponent<MeshRenderer>().bounds.size.y,
            stoves[0].GetComponent<MeshRenderer>().bounds.size.z + foodStorages[0].GetComponent<MeshRenderer>().bounds.size.z
        );
        counterSize = counter.GetComponent<MeshRenderer>().bounds.size;
        for (int i = 0; i < foods[0].transform.childCount; i++)
        {
            if (i == 0)
            {
                foodSize = new Vector3(
                    foods[0].transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size.x,
                    0,
                    foods[0].transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size.z
                );
            }
            foodSize.y += foods[0].transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size.y;
        }
        packageSize = packages[0].GetComponent<MeshRenderer>().bounds.size;
        for (int i = 0; i < packageTable.transform.childCount; i++)
        {
            if (i == 0)
            {
                packageTableSize = new Vector3(
                    0,
                    packageTable.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size.y,
                    packageTable.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size.z
                );
            }

            packageTableSize.x +=
                packageTable.transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size.x;
        }
        takeawayCounterSize = takeawayCounter.GetComponent<MeshRenderer>().bounds.size;
        tableSize = tables[0].transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;

        officeDeskSize.x = officeDesk[0].transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size.x
            + officeDesk[0].transform.GetChild(1).GetComponent<MeshRenderer>().bounds.size.x;
        officeDeskSize.y = officeDesk[0].transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size.y;
        officeDeskSize.z = officeDesk[0].transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size.z;

        trashSize = trashs[0].GetComponent<MeshRenderer>().bounds.size;



        StartCoroutine(LoadGame());
        StartCoroutine(DirectPlayer());

        if (!gameProgressManager.isEnableTutorial || gameProgressManager.progressStep != ProgressStep.CreateGate)
        {
            StartCoroutine(FastDebugGame());
        }
    }

    IEnumerator FastDebugGame()
    {
        yield return new WaitForSeconds(1f);

        stoves[0].SetActive(true);

        StartCoroutine(SpawnCustomer());
        StartCoroutine(SpawnFood(0));
        StartCoroutine(SpawnPackage());
        StartCoroutine(SpawnCar());
        /*StartCoroutine(SpawnUpgradeArea());*/

        StartCoroutine(SimulateCustomerCycle());
        StartCoroutine(SimulateCustomerMoving());
        StartCoroutine(SimulateFoodCycle());
        StartCoroutine(SimulatePackageCycle());
        StartCoroutine(SimulateCarCycle());
        StartCoroutine(SimulateTrashCycle());

        StartCoroutine(packageTable.GetComponent<PackageTable>().CheckHumanAction());

        npcManager.AddStaff();
        yield return new WaitForSeconds(0.5f);
        npcManager.AddStaff();
    }

    IEnumerator SimulateCustomerCycle()
    {
        while (true)
        {
            for (int i = 0; i < customers.Length; i++)
            {
                if (customerStates[i] == CustomerState.ChooseTable)
                {
                    Vector3 destination = Vector3.zero;
                    bool isFoundTable = false;

                    for (int j = 0; j < tables.Length; j++)
                    {
                        if (tableStates[j] == TableState.EmptyBoth)
                        {
                            destination = new Vector3(
                                tables[j].transform.position.x - 11,
                                customers[i].transform.position.y,
                                tables[j].transform.position.z
                                );

                            tableStates[j] = TableState.EmptyRight;
                            customerTableMap[i] = "table" + j + "-left";
                            isFoundTable = true;
                        }
                        else if (tableStates[j] == TableState.EmptyLeft)
                        {
                            destination = new Vector3(
                                tables[j].transform.position.x - 11,
                                customers[i].transform.position.y,
                                tables[j].transform.position.z
                                );

                            tableStates[j] = TableState.Full;
                            customerTableMap[i] = "table" + j + "-left";
                            isFoundTable = true;
                        }
                        else if (tableStates[j] == TableState.EmptyRight)
                        {
                            destination = new Vector3(
                                tables[j].transform.position.x + 11,
                                customers[i].transform.position.y,
                                tables[j].transform.position.z
                                );

                            tableStates[j] = TableState.Full;
                            customerTableMap[i] = "table" + j + "-right";
                            isFoundTable = true;
                        }

                        if (isFoundTable)
                        {
                            int customerIndex = i;
                            int tableIndex = j;

                            util.SetHoldingFoodMovingAnimation(customers[i].GetComponent<Animator>());

                            StartCoroutine
                            (
                                util.MoveTo(customers[i].transform, destination, speed,
                                () =>
                                    {
                                        util.SetSittingAnimation(customers[customerIndex].GetComponent<Animator>());

                                        customers[customerIndex].transform.position += new Vector3(0, 4, 0);

                                        if (customers[customerIndex].transform.position.x > tables[tableIndex].transform.position.x)
                                        {
                                            customers[customerIndex].transform.eulerAngles = new Vector3(0, 270, 0);
                                        }

                                        StartCoroutine(MoveFoodOneByOneToTable(
                                            customerIndex,
                                            tableIndex
                                        ));
                                    }
                                )
                            );

                            for (int k = 0; k < customers.Length; k++)
                            {
                                if (customerStates[k] == CustomerState.GoInside || customerStates[k] == CustomerState.Waiting)
                                {
                                    util.SetMovingAnimation(customers[k].GetComponent<Animator>());
                                    customerStates[k] = CustomerState.SetDestination;
                                }
                            }

                            customerDialog.SetActive(false);

                            customerList.RemoveAt(0);

                            customerStates[i] = CustomerState.GetToTable;
                            break;
                        }
                    }

                    if (isFoundTable == false)
                    {
                        if (!noSeatDialog.activeInHierarchy)
                        {
                            noSeatDialog.transform.position = customers[i].transform.position
                            + new Vector3(0, 23, 0);

                            customerDialog.SetActive(false);
                            noSeatDialog.SetActive(true);
                        }
                    }
                    else
                    {
                        noSeatDialog.SetActive(false);
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator SimulateCustomerMoving()
    {
        float[] zLimits = new float[customers.Length];

        while (true)
        {
            for (int i = 0; i < customers.Length; i++)
            {
                if (customerStates[i] == CustomerState.SetDestination)
                {
                    if (customerList.Count > 1)
                    {
                        if (i == customerList[0])
                        {
                            zLimits[i] = counter.transform.position.z - 1.5f * customerDistance;
                        }
                        else
                        {
                            for (int j = 0; j < customerList.Count; j++)
                            {
                                if (i == customerList[j])
                                {
                                    zLimits[i] = counter.transform.position.z - 1.5f * customerDistance
                                        - j * customerDistance;

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        zLimits[i] = counter.transform.position.z - 1.5f * customerDistance;
                    }

                    customerStates[i] = CustomerState.GoInside;
                }
                if (customerStates[i] == CustomerState.GoInside)
                {
                    float zLimit = zLimits[i];

                    if (customers[i].transform.position.z < zLimit)
                    {
                        customers[i].transform.Translate(new Vector3(0, 0, speed * deltaTime));
                    }
                    else
                    {
                        util.SetIdleAnimation(customers[i].GetComponent<Animator>());

                        if (i == customerList[0])
                        {
                            ShowCustomerDemandDialog(i);

                            customerStates[i] = CustomerState.FirstQueue;

                            if (npcManager.npcs[0].activeInHierarchy)
                            {
                                StartCoroutine(AutoCashier(i));
                            }
                        }
                        else
                        {
                            customerStates[i] = CustomerState.Waiting;
                        }
                    }
                }
                else if (customerStates[i] == CustomerState.Leaving)
                {
                    if (customers[i].transform.position.z > -30)
                    {
                        customers[i].transform.Translate(Vector3.forward * speed * deltaTime);
                    }
                    else
                    {
                        customers[i].gameObject.SetActive(false);
                        customerStates[i] = CustomerState.NotSpawn;
                    }
                }
            }

            yield return new WaitForSeconds(0.002f);
        }
    }

    IEnumerator SimulateFoodCycle()
    {
        while (true)
        {
            for (int i = 0; i < foods.Length; i++)
            {
                if (foodStates[i] == FoodState.Delivering)
                {
                    if (foodBelongTo[i] == "player")
                    {
                        foods[i].transform.position = new Vector3
                        (
                            player.transform.position.x,
                            player.transform.position.y +
                            8 + foodColumnIndex[i] * foodSize.y,
                            player.transform.position.z
                        ) +
                            player.transform.forward * 4;
                    }
                    else if (foodBelongTo[i].Contains("npc"))
                    {
                        int npcIndex = 0;

                        for (int j = 0; j < npcManager.npcs.Length; j++)
                        {
                            if (foodBelongTo[i] == npcManager.npcControllers[j].id)
                            {
                                npcIndex = j;
                            }
                        }

                        foods[i].transform.position = new Vector3
                        (
                            npcManager.npcs[npcIndex].transform.position.x,
                            npcManager.npcs[npcIndex].transform.position.y +
                            7 + foodColumnIndex[i] * foodSize.y,
                            npcManager.npcs[npcIndex].transform.position.z
                        ) +
                            npcManager.npcs[npcIndex].transform.forward * 4;
                    }
                }
                else if (foodStates[i] == FoodState.MovingWithPackage)
                {
                    int packageIndex = int.Parse(foodBelongTo[i][7].ToString());

                    foods[i].transform.position =
                        GetFoodPositionInPackage(packageIndex, foodIndexInPackage[i]);
                }
                else if (foodStates[i] == FoodState.CustomerPicked)
                {
                    for (int j = 0; j < customers.Length; j++)
                    {
                        if (customerStates[j] == CustomerState.ChooseTable || customerStates[j] == CustomerState.GetToTable)
                        {
                            if (foodBelongTo[i] == "customer" + j)
                            {
                                foods[i].transform.position = new Vector3
                                (
                                    customers[j].transform.position.x,
                                    customers[j].transform.position.y
                                    + 7 + foodColumnIndex[i] * foodSize.y,
                                    customers[j].transform.position.z
                                ) +
                                    customers[j].transform.forward * 4;
                            }
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.000f);
        }
    }

    IEnumerator SimulatePackageCycle()
    {
        while (true)
        {
            for (int i = 0; i < packages.Length; i++)
            {
                if (packageStates[i] == PackageState.Moving)
                {
                    if (packageBelongTo[i] == "player")
                    {
                        packages[i].transform.position = player.transform.position
                            + new Vector3(0, 8, 0)
                            + player.transform.forward * 6;
                    }
                    else if (packageBelongTo[i].Contains("npc"))
                    {
                        int npcIndex = int.Parse(packageBelongTo[i][3].ToString());

                        packages[i].transform.position = npcManager.npcs[npcIndex].transform.position
                            + new Vector3(0, 8, 0)
                            + npcManager.npcs[npcIndex].transform.forward * 6;
                    }
                }
            }

            yield return new WaitForSeconds(0.005f);
        }
    }

    IEnumerator SimulateCarCycle()
    {
        Vector3[] nextPositions = new Vector3[cars.Length];

        while (true)
        {
            for (int i = 0; i < cars.Length; i++)
            {
                if (carStates[i] == CarState.SetDirection)
                {
                    int carNextToIndex;
                    if (i > 0)
                    {
                        carNextToIndex = i - 1;
                    }
                    else
                    {
                        carNextToIndex = cars.Length - 1;
                    }



                    if (carStates[carNextToIndex] == CarState.NotSpawn ||
                        cars[carNextToIndex].transform.position.z < 100)
                    {
                        nextPositions[i] = new Vector3(
                            -20,
                            cars[i].transform.position.y,
                            cars[i].transform.position.z
                        );
                    }
                    else if (cars[carNextToIndex].transform.position.z < 170)
                    {
                        nextPositions[i] = new Vector3(
                            -20 + 40,
                            cars[i].transform.position.y,
                            cars[i].transform.position.z
                        );
                    }
                    else
                    {
                        nextPositions[i] = new Vector3(
                            nextPositions[carNextToIndex].x + 40,
                            cars[i].transform.position.y,
                            cars[i].transform.position.z
                        );
                    }

                    cars[i].transform.LookAt(nextPositions[i]);

                    carStates[i] = CarState.GoToMainLane;
                }
                else if (carStates[i] == CarState.GoToMainLane)
                {
                    if (cars[i].transform.position.x > nextPositions[i].x)
                    {
                        cars[i].transform.Translate(
                            cars[i].transform.forward * speed * 15 * deltaTime, Space.World
                        );
                    }
                    else
                    {
                        cars[i].transform.position = nextPositions[i];

                        if (cars[i].transform.position.x <= -20)
                        {
                            carStates[i] = CarState.SetTurnDirection;
                        }
                        else
                        {
                            carStates[i] = CarState.Waiting;
                        }
                    }
                }
                else if (carStates[i] == CarState.SetTurnDirection)
                {
                    int carNextToIndex;
                    if (i > 0)
                    {
                        carNextToIndex = i - 1;
                    }
                    else
                    {
                        carNextToIndex = cars.Length - 1;
                    }


                    if (carStates[carNextToIndex] == CarState.NotSpawn || carStates[carNextToIndex] == CarState.Done)
                    {
                        nextPositions[i] = new Vector3(
                            -20,
                            cars[i].transform.position.y,
                            takeawayCounter.transform.position.z
                        );
                    }
                    else
                    {
                        nextPositions[i] = nextPositions[carNextToIndex] + new Vector3(0, 0, 40);
                    }

                    cars[i].transform.LookAt(nextPositions[i]);
                    carStates[i] = CarState.GoToStore;
                }
                else if (carStates[i] == CarState.GoToStore)
                {
                    if (cars[i].transform.position.z > nextPositions[i].z)
                    {
                        cars[i].transform.Translate(
                            cars[i].transform.forward * speed * 5 * deltaTime, Space.World
                        );
                    }
                    else
                    {
                        cars[i].transform.position = nextPositions[i];

                        if (cars[i].transform.position.z <= takeawayCounter.transform.position.z)
                        {
                            carStates[i] = CarState.WaitingOrder;
                        }
                        else
                        {
                            carStates[i] = CarState.Waiting;
                        }
                    }
                }
                else if (carStates[i] == CarState.WaitingOrder)
                {
                    int carIndex = i;

                    for (int j = 0; j < packages.Length; j++)
                    {
                        int packageIndex = j;

                        if (packageStates[j] == PackageState.InTakeawayCounter)
                        {
                            StartCoroutine(
                                CurveMove(
                                    packages[j].transform,
                                    packages[j].transform.position,
                                    cars[i].transform.position,
                                    12,
                                    0,
                                    () =>
                                        {
                                            ResetPackageProperties(packageIndex);

                                            for (int k = 0; k < foods.Length; k++)
                                            {
                                                if (foodBelongTo[k] == "package" + packageIndex)
                                                {
                                                    resetFoodProperties(k);
                                                }
                                            }

                                            carStates[carIndex] = CarState.Done;
                                        }
                                    )
                            );

                            carStates[i] = CarState.TakingFood;
                            break;
                        }
                    }
                }
                else if (carStates[i] == CarState.Waiting)
                {
                    int carNextToIndex;
                    if (i > 0)
                    {
                        carNextToIndex = i - 1;
                    }
                    else
                    {
                        carNextToIndex = cars.Length - 1;
                    }

                    if (cars[i].transform.position.z > 160)
                    {
                        if (carStates[carNextToIndex] != CarState.Waiting)
                        {
                            carStates[i] = CarState.SetDirection;
                        }
                    }
                    else
                    {
                        if (carStates[carNextToIndex] == CarState.Done || carStates[carNextToIndex] == CarState.GoToStore)
                        {
                            carStates[i] = CarState.SetTurnDirection;
                        }
                    }
                }
                else if (carStates[i] == CarState.Done)
                {
                    if (cars[i].transform.position.z > -50)
                    {
                        cars[i].transform.Translate(
                            cars[i].transform.forward * speed * 15 * deltaTime, Space.World
                        );
                    }
                    else
                    {
                        ResetCarProperties(i);
                    }
                }
            }

            yield return new WaitForSeconds(0.03f);
        }
    }

    IEnumerator SimulateTrashCycle()
    {
        while (true)
        {
            for (int i = 0; i < trashs.Length; i++)
            {
                if (trashStates[i] == TrashState.Holding)
                {
                    if (trashBelongTo[i] == "player")
                    {
                        trashs[i].transform.position = player.transform.position
                            + new Vector3(0, 10 + trashColumnIndex[i] * trashSize.y, 0)
                            + player.transform.forward * 4;
                    }
                    else
                    {
                        for (int j = 0; j < npcManager.npcs.Length; j++)
                        {
                            if (trashBelongTo[i] == npcManager.npcControllers[j].id)
                            {
                                trashs[i].transform.position = npcManager.npcs[j].transform.position
                                    + new Vector3(0, 10 + trashColumnIndex[i] * trashSize.y, 0)
                                    + npcManager.npcs[j].transform.forward * 4;
                            }
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.003f);
        }
    }



    IEnumerator SpawnCustomer()
    {
        while (true)
        {
            if (customerList.Count < 5)
            {
                for (int i = 0; i < customers.Length; i++)
                {
                    if (customerStates[i] == CustomerState.NotSpawn)
                    {
                        customers[i].SetActive(true);
                        customers[i].GetComponent<Animator>().SetBool("isMoving", true);
                        customers[i].transform.eulerAngles = Vector3.zero;
                        customers[i].transform.position = new Vector3(
                            counter.transform.position.x - 0.4f * counterSize.x,
                            customers[i].transform.position.y,
                            -150
                        );

                        customerStates[i] = CustomerState.SetDestination;
                        customerNumFoodDemand[i] = 2;

                        customerList.Add(i);

                        break;
                    }
                }

                yield return new WaitForSeconds(5f);
            }
            else
            {
                yield return new WaitForSeconds(2.5f);
            }
        }
    }

    IEnumerator SpawnFood(int stoveIndex)
    {
        while (true)
        {
            for (int i = 0; i < foods.Length - 14; i++)
            {
                if (foodStorageStates[stoveIndex] == StoveState.PauseSpawn)
                {
                    break;
                }
                // show max capacity
                if (numFoodOfStorage[stoveIndex] >= resourceManager.stoveCapacities[stoveIndex])
                {
                    maxCapacityTMPs[stoveIndex].transform.position =
                        foodStorages[stoveIndex].transform.position + new Vector3(0, 17, 0);
                    maxCapacityTMPs[stoveIndex].gameObject.SetActive(true);
                    break;
                }

                if (foodStates[i] == FoodState.NotSpawn)
                {
                    int row = 1;
                    int col = 2;
                    int numFoodPerLayer = col * row;

                    foodColumnIndex[i] = numFoodHoldOf("storage" + stoveIndex);

                    Vector3 foodDeliverSize = foodStorages[stoveIndex].GetComponent<MeshRenderer>().bounds.size;
                    int x = (foodColumnIndex[i] % numFoodPerLayer) % col;
                    int z = ((foodColumnIndex[i] % numFoodPerLayer) - x) / col;
                    int y = (foodColumnIndex[i] - foodColumnIndex[i] % numFoodPerLayer) / numFoodPerLayer;

                    foods[i].transform.position = new Vector3(
                        foodStorages[stoveIndex].transform.position.x - foodDeliverSize.x / 2 + (x + 1) * foodDeliverSize.x / (col + 1),
                        foodStorages[stoveIndex].transform.position.y + foodDeliverSize.y / 2 + y * foodSize.y,
                        foodStorages[stoveIndex].transform.position.z + foodDeliverSize.z / 2 - (z + 1) * foodDeliverSize.z / (row + 1)
                    );

                    foods[i].SetActive(true);

                    foodBelongTo[i] = "storage" + stoveIndex;
                    foodStates[i] = FoodState.Wait;
                    numFoodOfStorage[stoveIndex]++;

                    yield return new WaitForSeconds(3.5f);
                }
            }

            yield return new WaitForSeconds(0);
        }
    }

    IEnumerator SpawnCar()
    {
        int nextIndex = 0;

        while (true)
        {
            if (carStates[nextIndex] == CarState.NotSpawn)
            {
                cars[nextIndex].transform.position = new Vector3(
                    300, 0, 170
                    );

                cars[nextIndex].SetActive(true);
                carStates[nextIndex] = CarState.SetDirection;

                if (nextIndex < cars.Length - 1)
                {
                    nextIndex++;
                }
                else
                {
                    nextIndex = 0;
                }
            }

            yield return new WaitForSeconds(5f);
        }
    }

    void SpawnFirstTable()
    {
        tables[0].transform.position = new Vector3(
            tables[0].transform.position.x,
            tables[0].transform.position.y,
            tables[0].transform.position.z
        );

        tables[0].SetActive(true);

        StartCoroutine(util.ScaleUpDownEffect(tables[0].transform, 0.2f));

        tableStates[0] = TableState.EmptyBoth;
    }

    void SpawnNewTable()
    {
        for (int i = 0; i < tables.Length; i++)
        {
            if (tableStates[i] == TableState.NotSpawn)
            {
                int col = 2;

                int x = i % col;
                int y = (i - i % col) / col;

                tables[i].transform.position = new Vector3(
                    tables[0].transform.position.x + x * 60,
                    tables[0].transform.position.y,
                    tables[0].transform.position.z - y * 50
                );

                tables[i].SetActive(true);

                tableStates[i] = TableState.EmptyBoth;

                break;
            }
        }
    }

    void SpawnGate()
    {
        gate.SetActive(true);

        StartCoroutine(util.ScaleUpDownEffect(gate.transform, 0.2f));
    }

    public void SpawnTutorialArrow(Vector3 spawnPosition)
    {
        tutorialArrow.transform.position = spawnPosition + new Vector3(0, 40, 0);
        tutorialArrow.SetActive(true);

        StartCoroutine(TutorialArrowEffect());
    }

    public void SpawnUpgradeArea(Vector3 spawnPosition,
        UpgradeAreaFunction upgradeAreaFunction = UpgradeAreaFunction.Unidentified, int requireValue = 100)
    {
        if (upgradeAreaFunction != UpgradeAreaFunction.Unidentified)
        {
            for (int i = 0; i < upgradeAreas.Length; i++)
            {
                if (upgradeAreaFunctions[i] == upgradeAreaFunction)
                {
                    return;
                }
            }
        }

        for (int i = 0; i < upgradeAreas.Length; i++)
        {
            if (upgradeAreaStates[i] == UpgradeAreaState.NotSpawn)
            {
                int upgradeAreaIndex = i;

                upgradeAreas[i].transform.position = spawnPosition;

                if (upgradeAreaFunction == UpgradeAreaFunction.CreateTable)
                {
                    upgradeAreaScripts[i].SetSizeEqualTo(new Vector3(3 * tableSize.x, 1, 1.2f * tableSize.z),
                        tables[0].transform.localScale);
                }
                else if (upgradeAreaFunction == UpgradeAreaFunction.CreatePackageTable)
                {
                    upgradeAreaScripts[i].SetSizeEqualTo(new Vector3(packageTableSize.x, 1, packageTableSize.z),
                        packageTable.transform.localScale);
                }
                else if (upgradeAreaFunction == UpgradeAreaFunction.CreateTakeawayCounter)
                {
                    upgradeAreaScripts[i].SetSizeEqualTo(new Vector3(takeawayCounterSize.x, 1, takeawayCounterSize.z),
                        takeawayCounter.transform.localScale);
                }
                else if (upgradeAreaFunction == UpgradeAreaFunction.AddCashier)
                {
                    upgradeAreaScripts[i].SetSizeEqualTo(new Vector3(counterSize.z, 1, counterSize.z),
                        takeawayCounter.transform.localScale);
                }
                else if (upgradeAreaFunction == UpgradeAreaFunction.CreateHRUpgrade
                    || upgradeAreaFunction == UpgradeAreaFunction.CreatePlayerUpgrade)
                {
                    upgradeAreaScripts[i].SetSizeEqualTo(new Vector3(officeDeskSize.x, 1, officeDeskSize.z),
                        officeDesk[0].transform.localScale);
                }



                upgradeAreaScripts[i].SetNewRequireValue(requireValue);
                upgradeMoneyTMPs[i].rectTransform.position = upgradeAreas[i].transform.position
                    + new Vector3(0, 1, 0);

                upgradeMoneyTMPs[i].text = util.ToShortFormNumber(upgradeAreaScripts[i].remainRequireValue);

                upgradeAreas[i].SetActive(true);
                upgradeMoneyTMPs[i].gameObject.SetActive(true);

                if (gameProgressManager.isEnableTutorial)
                {
                    SpawnTutorialArrow(spawnPosition);
                }



                if (upgradeAreaFunction != UpgradeAreaFunction.Unidentified)
                {
                    upgradeAreaFunctions[i] = upgradeAreaFunction;
                }

                StartCoroutine(
                    upgradeAreas[i].GetComponent<UpgradeArea>().UpgradeProgress(
                    () =>
                        {
                            if (upgradeAreaFunctions[upgradeAreaIndex]
                                    == UpgradeAreaFunction.CreateGate)
                            {
                                OnGateCreated();
                            }
                            else if (upgradeAreaFunctions[upgradeAreaIndex]
                                == UpgradeAreaFunction.CreateFirstTable)
                            {
                                OnFirstTableCreated();
                            }
                            else if (upgradeAreaFunctions[upgradeAreaIndex]
                                == UpgradeAreaFunction.CreateStove)
                            {
                                OnStoveCreated();
                            }
                            else if (upgradeAreaFunctions[upgradeAreaIndex]
                                == UpgradeAreaFunction.CreateCounter)
                            {
                                OnCounterCreated();
                            }
                            else if (upgradeAreaFunctions[upgradeAreaIndex]
                                == UpgradeAreaFunction.CreateTable)
                            {
                                OnTableUpgraded(upgradeAreaIndex);
                            }
                            else if (upgradeAreaFunctions[upgradeAreaIndex]
                                == UpgradeAreaFunction.AddCashier)
                            {
                                npcManager.SpawnCashier();

                                ResetUpgradeAreaProperties(upgradeAreaIndex);
                            }
                            else if (upgradeAreaFunctions[upgradeAreaIndex]
                                == UpgradeAreaFunction.CreatePackageTable)
                            {
                                OnPackageTableUpgraded(upgradeAreaIndex);
                            }
                            else if (upgradeAreaFunctions[upgradeAreaIndex]
                                == UpgradeAreaFunction.CreateTakeawayCounter)
                            {
                                OnTakeawayCounterUpgraded(upgradeAreaIndex);
                            }
                            else if (upgradeAreaFunctions[upgradeAreaIndex]
                                == UpgradeAreaFunction.CreateHRUpgrade)
                            {
                                OnOfficeDeskUpgraded(upgradeAreaIndex, 0);
                            }
                            else if (upgradeAreaFunctions[upgradeAreaIndex]
                                == UpgradeAreaFunction.CreatePlayerUpgrade)
                            {
                                OnOfficeDeskUpgraded(upgradeAreaIndex, 1);
                            }
                        }
                    )
                );

                upgradeAreaStates[i] = UpgradeAreaState.Active;

                break;
            }
        }
    }

    IEnumerator SpawnPackage()
    {
        while (true)
        {
            bool isSpawn = true;
            int spawnIndex = -1;

            for (int i = 0; i < packages.Length; i++)
            {
                if (packageStates[i] == PackageState.Filling)
                {
                    isSpawn = false;
                    break;
                }

                if (packageStates[i] == PackageState.NotSpawn && spawnIndex == -1)
                {
                    spawnIndex = i;
                }
            }

            if (isSpawn)
            {
                packages[spawnIndex].transform.position = new Vector3(
                    packageTable.transform.position.x + 0.9f * packageTableSize.x - packageSize.x / 2,
                    packageTable.transform.position.y + packageTableSize.y + packageSize.y / 2,
                    packageTable.transform.position.z + packageTableSize.z / 2
                );

                packages[spawnIndex].SetActive(true);

                packageBelongTo[spawnIndex] = "package table";
                packageStates[spawnIndex] = PackageState.Filling;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    public void SpawnEmoji(Vector3 spawnPosition)
    {
        for (int i = 0; i < happyEmojis.Length; i++)
        {
            if (!happyEmojis[i].activeInHierarchy)
            {
                happyEmojis[i].transform.position = spawnPosition
                    + new Vector3(0, 23, 0);

                happyEmojis[i].SetActive(true);

                StartCoroutine(util.RotateSlowly(happyEmojis[i].transform));
                StartCoroutine(util.ScaleDown(happyEmojis[i].transform, 1f));

                break;
            }
        }
    }

    void SpawnTrash(Vector3 position, string belongTo)
    {
        int currentHighestIndex = -1;
        int newIndex;
        int numSpawn = 0;

        for (int i = 0; i < trashs.Length; i++)
        {
            if (trashBelongTo[i] == belongTo)
            {
                currentHighestIndex = Mathf.Max(currentHighestIndex, trashColumnIndex[i]);
            }
        }

        newIndex = currentHighestIndex;

        for (int i = 0; i < trashs.Length; i++)
        {
            if (trashStates[i] == TrashState.NotSpawn)
            {
                newIndex++;

                trashBelongTo[i] = belongTo;
                trashColumnIndex[i] = newIndex;

                trashs[i].transform.position = position +
                    new Vector3(
                        0,
                        (trashColumnIndex[i] + 0.5f) * trashSize.y,
                        0
                    );

                trashs[i].SetActive(true);

                trashStates[i] = TrashState.OnTable;

                numSpawn++;
            }
            if (numSpawn > 1)
            {
                break;
            }
        }
    }



    void OnGateCreated()
    {
        SpawnGate();

        upgradeCompleteSound.Play();

        SpawnEmoji(player.transform.position);

        cameraScript.cameraState = CameraState.ZoomStart;

        ResetUpgradeAreaProperties(0);

        upgradeAreaScripts[0].SetSizeEqualTo(new Vector3(3 * tableSize.x, 1, 1.2f * tableSize.z),
            tables[0].transform.localScale);

        util.SetTMPTextOnBackground(tutorialText, tutorialTextBackground, "Create a table!");

        Vector3 spawnPosition = new Vector3(
            tables[0].transform.position.x,
            0,
            tables[0].transform.position.z
        );
        SpawnUpgradeArea(spawnPosition, UpgradeAreaFunction.CreateFirstTable, 200);

        gameProgressManager.progressStep = ProgressStep.CreateFirstTable;
        directionArrowState = DirectionArrowState.Set;
    }

    void OnFirstTableCreated()
    {
        SpawnFirstTable();

        upgradeCompleteSound.Play();

        ResetUpgradeAreaProperties(0);

        upgradeAreaScripts[0].SetSizeEqualTo(stoveSize, counter.transform.localScale);

        util.SetTMPTextOnBackground(tutorialText, tutorialTextBackground, "Create food machine!");

        Vector3 spawnPosition = new Vector3(
            stoves[0].transform.position.x,
            0,
            stoves[0].transform.position.z - 5
        );
        SpawnUpgradeArea(spawnPosition, UpgradeAreaFunction.CreateStove, 1000);

        gameProgressManager.progressStep = ProgressStep.CreateStove;
        directionArrowState = DirectionArrowState.Set;
    }

    void OnTableUpgraded(int upgradeAreaIndex)
    {
        SpawnNewTable();

        ResetUpgradeAreaProperties(upgradeAreaIndex);

        if (!tables[tables.Length - 1].activeInHierarchy)
        {
            gameProgressManager.isTableUpgradeSpawn = false;
        }
    }

    void OnStoveCreated()
    {
        stoves[0].SetActive(true);
        StartCoroutine(util.ScaleUpDownEffect(stoves[0].transform, 0.2f));
        StartCoroutine(util.ScaleUpDownEffect(foodStorages[0].transform, 0.2f));

        upgradeCompleteSound.Play();

        ResetUpgradeAreaProperties(0);

        upgradeAreaScripts[0].SetSizeEqualTo(counterSize, counter.transform.localScale);

        util.SetTMPTextOnBackground(tutorialText, tutorialTextBackground, "Create a counter!");

        Vector3 spawnPosition = new Vector3(
            counter.transform.position.x,
            0,
            counter.transform.position.z + counterSize.z
        );
        SpawnUpgradeArea(spawnPosition, UpgradeAreaFunction.CreateCounter, 200);

        gameProgressManager.progressStep = ProgressStep.CreateCounter;
        directionArrowState = DirectionArrowState.Set;
    }

    void OnCounterCreated()
    {
        counter.SetActive(true);
        StartCoroutine(util.ScaleUpDownEffect(counter.transform, 0.2f));

        upgradeCompleteSound.Play();
        talkingSound.Play();

        ResetUpgradeAreaProperties(0);


        StartCoroutine(SpawnCustomer());
        StartCoroutine(SpawnFood(0));

        StartCoroutine(SimulateCustomerCycle());
        StartCoroutine(SimulateCustomerMoving());
        StartCoroutine(SimulateFoodCycle());
        StartCoroutine(SimulateTrashCycle());
        StartCoroutine(cashierPosition.GetComponent<CashierPosition>().CheckCashier());

        directionArrowState = DirectionArrowState.Set;
        gameProgressManager.progressStep = ProgressStep.PickupFoodTutorialStart;
    }

    void OnPackageTableUpgraded(int upgradeAreaIndex)
    {
        packageTable.SetActive(true);
        StartCoroutine(util.ScaleUpDownEffect(packageTable.transform, 0.2f));

        upgradeCompleteSound.Play();

        StartCoroutine(packageTable.GetComponent<PackageTable>().CheckHumanAction());
        StartCoroutine(SpawnPackage());
        StartCoroutine(SimulatePackageCycle());

        ResetUpgradeAreaProperties(upgradeAreaIndex);
    }

    void OnTakeawayCounterUpgraded(int upgradeAreaIndex)
    {
        takeawayCounter.SetActive(true);
        StartCoroutine(util.ScaleUpDownEffect(takeawayCounter.transform, 0.2f));

        upgradeCompleteSound.Play();

        StartCoroutine(SpawnCar());
        StartCoroutine(SimulateCarCycle());

        ResetUpgradeAreaProperties(upgradeAreaIndex);
    }

    void OnOfficeDeskUpgraded(int upgradeAreaIndex, int officeDeskIndex)
    {
        officeDesk[officeDeskIndex].SetActive(true);
        StartCoroutine(util.ScaleUpDownEffect(officeDesk[officeDeskIndex].transform, 0.2f));

        upgradeCompleteSound.Play();

        ResetUpgradeAreaProperties(upgradeAreaIndex);
    }




    public IEnumerator moveFoodOneByOneToCustomer(Transform customerTf, int customerIndex)
    {
        List<int> satisfiedfoodIndexes = new List<int>();

        for (int i = 0; i < foods.Length; i++)
        {
            if (foodBelongTo[i] == "counter")
            {
                satisfiedfoodIndexes.Add(i);
            }
        }

        for (int i = 0; i < satisfiedfoodIndexes.Count - 1; i++)
        {
            for (int j = i + 1; j < satisfiedfoodIndexes.Count; j++)
            {
                if (foodColumnIndex[satisfiedfoodIndexes[j]] > foodColumnIndex[satisfiedfoodIndexes[i]])
                {
                    int temp = satisfiedfoodIndexes[j];
                    satisfiedfoodIndexes[j] = satisfiedfoodIndexes[i];
                    satisfiedfoodIndexes[i] = temp;
                }
            }
        }

        for (int i = 0; i < satisfiedfoodIndexes.Count; i++)
        {
            int foodIndex = satisfiedfoodIndexes[i];

            if (numFoodHoldOf("customer" + customerIndex) < customerNumFoodDemand[customerIndex])
            {
                foodBelongTo[foodIndex] = "customer" + customerIndex;

                int numFoodCustomerHold = numFoodHoldOf("customer" + customerIndex);

                foodColumnIndex[foodIndex] = numFoodCustomerHold - 1;
                foodStates[foodIndex] = FoodState.CustomerPicking;

                StartCoroutine(
                    CurveMove(
                        foods[foodIndex].transform,
                        foods[foodIndex].transform.position,
                        customerTf.position
                        + new Vector3(0, 7 + foodColumnIndex[foodIndex] * foodSize.y, 0)
                        + customerTf.forward * 4
                        ,
                        12,
                        0,
                        () =>
                        {
                            if (numFoodCustomerHold == 1)
                            {
                                util.SetHoldingFoodStandingAnimation(customers[customerIndex].GetComponent<Animator>());
                            }

                            if (numFoodCustomerHold == customerNumFoodDemand[customerIndex])
                            {
                                if (gameProgressManager.progressStep == ProgressStep.CashierTutorial)
                                {
                                    directionArrowState = DirectionArrowState.Set;
                                    gameProgressManager.progressStep = ProgressStep.PickTrashTutorialStart;
                                }

                                customerStates[customerIndex] = CustomerState.ChooseTable;
                            }

                            foodStates[foodIndex] = FoodState.CustomerPicked;
                        }
                    )
                );

                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                break;
            }
        }
    }

    public IEnumerator moveTrashOneByOne(string owner, Transform targetTf, int tableIndex, OnTrashPickedUp onTrashPickedUp)
    {
        int capacity = 2;
        int numTake = 0;

        int lastIndexBelongToTarget = FindLastTrashIndexBelongTo(owner);
        int newIndex = lastIndexBelongToTarget;

        for (int i = 0; i < trashs.Length; i++)
        {
            int trashIndex = i;

            if (numTake >= capacity)
            {
                break;
            }

            if (trashBelongTo[i] == "table" + tableIndex)
            {
                numTake++;
                newIndex++;

                trashColumnIndex[i] = newIndex;
                trashBelongTo[i] = owner;
                trashStates[i] = TrashState.Picking;

                int numTakeLocalScope = numTake;

                StartCoroutine(
                        CurveMove(
                            trashs[i].transform,
                            trashs[i].transform.position,
                            targetTf.position
                            + new Vector3(0, 7 + trashColumnIndex[i] * trashSize.y, 0)
                            + targetTf.forward * 4
                            ,
                            12,
                            0,
                            () =>
                            {
                                onTrashPickedUp(numTakeLocalScope, trashIndex, capacity);

                                trashStates[trashIndex] = TrashState.Holding;
                            }
                        )
                );

                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public void moveTrashOneByOneByPlayer(string owner, Transform targetTf, int tableIndex)
    {
        StartCoroutine(moveTrashOneByOne(
            owner,
            targetTf,
            tableIndex,
            (numTake, trashIndex, capacity) =>
            {
                if (numTake == 1)
                {
                    playerController.playerState = PlayerState.HoldingTrashStanding;
                }
                if (numTake == capacity)
                {
                    if (gameProgressManager.progressStep == ProgressStep.PickTrashTutorial)
                    {
                        directionArrowState = DirectionArrowState.Set;
                        gameProgressManager.progressStep = ProgressStep.ThrowTrashTutorialStart;
                    }

                    playerController.playerState = PlayerState.HoldingTrashMoving;
                }
                if (numTake > 0 && trashIndex == trashs.Length - 1)
                {
                    playerController.playerState = PlayerState.HoldingTrashMoving;
                }
            }
        ));
    }

    public void moveTrashOneByOneByNPC(string owner, Transform targetTf, int tableIndex)
    {
        StartCoroutine(moveTrashOneByOne(
            owner,
            targetTf,
            tableIndex,
            (numTake, trashIndex, capacity) =>
            {
                int npcIndex = int.Parse(owner[3].ToString());

                if (numTake == capacity)
                {
                    npcManager.npcControllers[npcIndex].humanState = HumanState.HoldingAndMoving;
                }
                if (numTake > 0 && trashIndex == trashs.Length - 1)
                {
                    npcManager.npcControllers[npcIndex].humanState = HumanState.HoldingAndMoving;
                }
            }
        ));
    }

    public delegate void OnTrashPickedUp(int numTake, int trashIndex, int capacity);

    IEnumerator MoveFoodOneByOneToTable(int customerIndex, int tableIndex)
    {
        util.SetHoldingFoodStandingAnimation(customers[customerIndex].GetComponent<Animator>());

        for (int j = 0; j < foods.Length; j++)
        {
            if (foodBelongTo[j] == "customer" + customerIndex)
            {
                foodBelongTo[j] = "customer" + customerIndex + "-"
                + "table" + tableIndex;
                foodColumnIndex[j] = numFoodHoldOf("table" + tableIndex, true) - 1;

                Vector3 end = new Vector3(
                    tables[tableIndex].transform.position.x,
                    tables[tableIndex].transform.position.y + tableSize.y / 2
                    + foodColumnIndex[j] * foodSize.y,
                    tables[tableIndex].transform.position.z
                );

                StartCoroutine(
                    CurveMove(
                        foods[j].transform,
                        foods[j].transform.position,
                        end,
                        12,
                        0,
                        () => { }
                        )
                );

                yield return new WaitForSeconds(0.2f);
            }
        }

        StartCoroutine(Eat(customerIndex));
        customerStates[customerIndex] = CustomerState.Eating;
    }




    void resetFoodProperties(int index)
    {
        foods[index].SetActive(false);
        foodBelongTo[index] = "storage";
        foodColumnIndex[index] = -1;
        foodStates[index] = FoodState.NotSpawn;
    }

    public void ResetPackageProperties(int index)
    {
        packages[index].SetActive(false);
        packageBelongTo[index] = "none";
        packageStates[index] = PackageState.NotSpawn;
    }

    public void ResetCarProperties(int index)
    {
        cars[index].SetActive(false);
        carStates[index] = CarState.NotSpawn;
    }

    public void resetTrashProperties(int index)
    {
        trashs[index].SetActive(false);
        trashBelongTo[index] = "none";
        trashColumnIndex[index] = -1;
        trashStates[index] = TrashState.NotSpawn;
    }

    public void ResetUpgradeAreaProperties(int index)
    {
        UpgradeArea upgradeAreaScript = upgradeAreas[index].GetComponent<UpgradeArea>();

        upgradeAreas[index].transform.localScale = upgradeAreaScript.initialScale;
        upgradeAreas[index].SetActive(false);
        upgradeMoneyTMPs[index].gameObject.SetActive(false);
        tutorialArrow.SetActive(false);

        upgradeAreaScript.remainRequireValue = upgradeAreaScript.requireValue;
        upgradeAreaScript.isInside = false;
        upgradeAreaScript.fillRate = -0.5f;
        upgradeAreaScript.propertyBlock.SetFloat("_FillRate", upgradeAreaScript.fillRate);
        upgradeAreaScript.meshRenderer.SetPropertyBlock(upgradeAreaScript.propertyBlock);

        upgradeMoneyTMPs[index].text = upgradeAreaScript.requireValue.ToString();

        upgradeAreaStates[index] = UpgradeAreaState.NotSpawn;
        upgradeAreaFunctions[index] = UpgradeAreaFunction.None;
    }



    public int numFoodHoldOf(string ownerName, bool isContain = false)
    {
        int num = 0;

        for (int i = 0; i < foodBelongTo.Length; i++)
        {
            if (!isContain && foodBelongTo[i] == ownerName)
            {
                num++;
            }

            if (isContain && foodBelongTo[i].Contains(ownerName))
            {
                num++;
            }
        }

        return num;
    }

    public int numFoodAvailableForCustomer()
    {
        int num = 0;

        for (int i = 0; i < foods.Length; i++)
        {
            if (foodStates[i] == FoodState.CustomerPick)
            {
                num++;
            }
        }

        return num;
    }

    public int numFillOfPackage(int packageIndex)
    {
        int num = 0;

        for (int i = 0; i < foods.Length; i++)
        {
            if (foodBelongTo[i] == "package" + packageIndex)
            {
                num++;
            }
        }

        return num;
    }

    public int numPackageBelongTo(string owner)
    {
        int num = 0;

        for (int i = 0; i < packages.Length; i++)
        {
            if (packageBelongTo[i] == owner)
            {
                num++;
            }
        }

        return num;
    }



    void DisableUpgradeArea(int index)
    {
        upgradeAreas[index].SetActive(false);
        upgradeMoneyTMPs[index].gameObject.SetActive(false);

        upgradeAreaStates[index] = UpgradeAreaState.NotSpawn;
        upgradeAreaFunctions[index] = UpgradeAreaFunction.None;
    }

    public void DisableObjectsForCreateGateStep()
    {
        gate.SetActive(false);
        stoves[0].SetActive(false);
        counter.SetActive(false);
        packageTable.SetActive(false);
        takeawayCounter.SetActive(false);

        DisableArrayObjects(tables);
        DisableArrayObjects(customers);
        DisableArrayObjects(foods);
        DisableArrayObjects(packages);
        DisableArrayObjects(cars);
    }

    void DisableArrayObjects(GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetActive(false);
        }
    }



    public IEnumerator TakingMoneyEffect(Transform fromTf, Transform toTf,
        bool condition)
    {
        Vector3 moneyPileSize = fromTf.GetComponent<MeshRenderer>().bounds.size;


        bool[] isEffectMoneyUsed = new bool[effectMoneys.Length];

        for (int i = 0; i < effectMoneys.Length; i++)
        {
            isEffectMoneyUsed[i] = false;
        }


        int moneyPileGridIndex = 0;


        while (condition)
        {
            for (int i = 0; i < effectMoneys.Length; i++)
            {
                if (!condition)
                {
                    break;
                }

                int effectMoneyIndex = i;

                if (isEffectMoneyUsed[i] == false)
                {
                    int col = 3;
                    /* int row = 3;*/
                    int x = moneyPileGridIndex % col;
                    int y = (moneyPileGridIndex - moneyPileGridIndex % col) / col;

                    Vector3 spawnPosition = new Vector3(
                          fromTf.position.x - 0.5f * (x - 1) * moneyPileSize.x,
                          fromTf.position.y + 0.5f * moneyPileSize.y,
                          fromTf.position.z - 0.5f * (y - 1) * moneyPileSize.z
                    );

                    effectMoneys[i].transform.position = spawnPosition;
                    effectMoneys[i].SetActive(true);

                    StartCoroutine(
                        CurveMove(
                           effectMoneys[effectMoneyIndex].transform,
                           spawnPosition,
                           toTf.position + new Vector3(0, 10, 0),
                           15,
                           0,
                           () =>
                           {
                               effectMoneys[effectMoneyIndex].SetActive(false);

                               isEffectMoneyUsed[effectMoneyIndex] = false;
                           }
                        )
                    );

                    if (moneyPileGridIndex < 9)
                    {
                        moneyPileGridIndex++;
                    }
                    else
                    {
                        moneyPileGridIndex = 0;
                    }

                    isEffectMoneyUsed[i] = true;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        for (int i = 0; i < effectMoneys.Length; i++)
        {
            effectMoneys[i].SetActive(false);
        }
    }

    public IEnumerator PuttingMoneyEffect(Transform fromTf, Transform toTf,
        UpgradeArea upgradeAreaScript)
    {
        bool[] isEffectMoneyUsed = new bool[effectMoneys.Length];

        for (int i = 0; i < effectMoneys.Length; i++)
        {
            isEffectMoneyUsed[i] = false;
        }

        while (upgradeAreaScript.isInside)
        {
            for (int i = 0; i < effectMoneys.Length; i++)
            {
                int effectMoneyIndex = i;

                if (!upgradeAreaScript.isInside)
                {
                    break;
                }

                if (isEffectMoneyUsed[i] == false)
                {
                    effectMoneys[i].transform.position = fromTf.position + new Vector3(0, 13, 0);
                    effectMoneys[i].SetActive(true);

                    StartCoroutine(
                        CurveMove(
                           effectMoneys[effectMoneyIndex].transform,
                           effectMoneys[effectMoneyIndex].transform.position,
                           toTf.position,
                           15,
                           0,
                           () =>
                           {
                               effectMoneys[effectMoneyIndex].SetActive(false);

                               isEffectMoneyUsed[effectMoneyIndex] = false;
                           }
                        )
                    );

                    isEffectMoneyUsed[i] = true;
                }

                yield return new WaitForSeconds(0.15f);
            }
        }

        for (int i = 0; i < effectMoneys.Length; i++)
        {
            effectMoneys[i].SetActive(false);
        }
    }



    /*public void RearrangeFood(int stoveIndex)
    {
        int newPositionIndex = -1;

        for (int i = 0; i < foods.Length; i++)
        {
            if (foodStates[i] == FoodState.Wait)
            {
                newPositionIndex++;

                foodColumnIndex[i] = newPositionIndex;

                int row = 1;
                int col = 2;
                int numFoodPerLayer = col * row;

                Vector3 foodDeliverSize = foodStorages[stoveIndex].GetComponent<MeshRenderer>().bounds.size;
                int x = (newPositionIndex % numFoodPerLayer) % col;
                int z = ((newPositionIndex % numFoodPerLayer) - x) / col;
                int y = (newPositionIndex - newPositionIndex % numFoodPerLayer) / numFoodPerLayer;

                foods[i].transform.position = new Vector3(
                    foodStorages[stoveIndex].transform.position.x - foodDeliverSize.x / 2 + (x + 1) * foodDeliverSize.x / (col + 1),
                    foodStorages[stoveIndex].transform.position.y + foodDeliverSize.y / 2 + y * foodSize.y,
                    foodStorages[stoveIndex].transform.position.z + foodDeliverSize.z / 2 - (z + 1) * foodDeliverSize.z / (row + 1)
                );
            }
        }

        for (int i = 0; i < foods.Length; i++)
        {
            if (foodStates[i] == FoodState.NotSpawn)
            {
                newPositionIndex++;

                foodColumnIndex[i] = newPositionIndex;
            }
        }
    }*/



    IEnumerator TutorialArrowEffect()
    {
        Vector3 initialPos = tutorialArrow.transform.position;
        float range = 2f;
        int phase = 0;

        while (tutorialArrow.activeInHierarchy
            && tutorialArrow.transform.position.x == initialPos.x
            && tutorialArrow.transform.position.z == initialPos.z)
        {
            if (phase == 0)
            {
                if (tutorialArrow.transform.position.y > initialPos.y - range)
                {
                    tutorialArrow.transform.Translate(Vector3.down * 0.5f, Space.World);
                }
                else
                {
                    phase = 1;
                }
            }
            else if (phase == 1)
            {
                if (tutorialArrow.transform.position.y < initialPos.y + range)
                {
                    tutorialArrow.transform.Translate(Vector3.up * 0.5f, Space.World);
                }
                else
                {
                    phase = 0;
                }
            }

            yield return new WaitForSeconds(0.02f);
        }
    }

    public IEnumerator DirectPlayer()
    {
        bool isFirstTime = true;
        Vector3 nextPoint = Vector3.zero;

        while (true)
        {
            if (directionArrowState == DirectionArrowState.Set)
            {
                bool isSet = true;

                if (isFirstTime)
                {
                    yield return new WaitForSeconds(2f);

                    isFirstTime = false;
                }

                if (gameProgressManager.progressStep == ProgressStep.CreateFirstTable)
                {
                    nextPoint = tables[0].transform.position;
                }
                else if (gameProgressManager.progressStep == ProgressStep.CreateStove)
                {
                    nextPoint = stoves[0].transform.position;
                }
                else if (gameProgressManager.progressStep == ProgressStep.CreateCounter)
                {
                    nextPoint = counter.transform.position;
                }
                else if (gameProgressManager.progressStep == ProgressStep.PickupFoodTutorialStart
                    || gameProgressManager.progressStep == ProgressStep.PickupFoodTutorial)
                {
                    nextPoint = foods[0].transform.position;
                }
                else if (gameProgressManager.progressStep == ProgressStep.PutFoodOnCounterTutorialStart
                    || gameProgressManager.progressStep == ProgressStep.PutFoodOnCounterTutorial)
                {
                    nextPoint = bowl.transform.position;
                }
                else if (gameProgressManager.progressStep == ProgressStep.CashierTutorialStart
                    || gameProgressManager.progressStep == ProgressStep.CashierTutorial)
                {
                    nextPoint = cashierPosition.transform.position;
                }
                else if (gameProgressManager.progressStep == ProgressStep.PickTrashTutorialStart
                    || gameProgressManager.progressStep == ProgressStep.PickTrashTutorial)
                {
                    if (tableStates[0] == TableState.Dirty)
                    {
                        nextPoint = tables[0].transform.position;
                    }
                    else
                    {
                        isSet = false;

                        yield return new WaitForSeconds(2f);
                    }
                }
                else if (gameProgressManager.progressStep == ProgressStep.ThrowTrashTutorialStart
                    || gameProgressManager.progressStep == ProgressStep.ThrowTrashTutorial)
                {
                    nextPoint = trashCan.transform.position;
                }
                else if (gameProgressManager.progressStep == ProgressStep.TutorialComplete)
                {
                    Destroy(directionArrow);
                    break;
                }

                if (isSet)
                {
                    nextPoint += new Vector3(0, 8, 0);

                    directionArrowState = DirectionArrowState.Directing;
                }
            }
            else if (directionArrowState == DirectionArrowState.Directing)
            {
                if (Vector3.Distance(player.transform.position, nextPoint) > 50)
                {
                    if (!directionArrow.activeInHierarchy)
                    {
                        directionArrow.SetActive(true);
                    }

                    directionArrow.transform.position = player.transform.position + new Vector3(0, 25, 0);
                    directionArrow.transform.LookAt(nextPoint);

                    directionArrow.transform.eulerAngles += new Vector3(75, 0, 0);
                }
                else
                {
                    if (directionArrow.activeInHierarchy)
                    {
                        directionArrow.SetActive(false);
                    }
                }
            }

            yield return new WaitForSeconds(0.002f);
        }
    }

    IEnumerator Eat(int customerIndex)
    {
        while (customerStates[customerIndex] != CustomerState.Leaving)
        {
            yield return new WaitForSeconds(10f);

            int happyEmojiIndex = 0;

            SpawnEmoji(customers[customerIndex].transform.position);

            yield return new WaitForSeconds(3f);

            customers[customerIndex].transform.position = new Vector3(
                customers[customerIndex].transform.position.x,
                0,
                customers[customerIndex].transform.position.z
            );

            customers[customerIndex].transform.LookAt(new Vector3(
                customers[customerIndex].transform.position.x,
                customers[customerIndex].transform.position.y, -30
            ));

            happyEmojis[happyEmojiIndex].SetActive(false);

            int tableIndex = int.Parse(customerTableMap[customerIndex][5].ToString());
            string tableSeat = customerTableMap[customerIndex].Substring(7);


            List<int> sortedFoodIndexes = FindAndSortFood("table" + tableIndex, true);

            for (int i = 0; i < sortedFoodIndexes.Count; i++)
            {
                resetFoodProperties(sortedFoodIndexes[i]);

                if (i == customerNumFoodDemand[customerIndex] - 1)
                {
                    break;
                }
            }


            customerTableMap[customerIndex] = "";

            Vector3 tablePosition = tables[tableIndex].transform.position;

            bool isDirty = true;

            // check if anyone else is in the table
            for (int i = 0; i < customers.Length; i++)
            {
                if (customerTableMap[i].Contains("table" + tableIndex))
                {
                    isDirty = false;
                    break;
                }
            }

            if (isDirty)
            {
                SpawnTrash(
                    new Vector3(tablePosition.x,
                    tablePosition.y + tableSize.y / 2,
                    tablePosition.z),
                    "table" + tableIndex
                );

                RotateChairs(tableIndex, new Vector3(0, 30, 0), new Vector3(0, 210, 0));

                tableStates[tableIndex] = TableState.Dirty;
            }


            moneyPileManager.SpawnMoneyPile(
                new Vector3(
                    tables[tableIndex].transform.position.x + tableSize.x / 2,
                    0,
                    tables[tableIndex].transform.position.z - tableSize.z - 6
                ),
                tableIndex
            );

            util.SetMovingAnimation(customers[customerIndex].GetComponent<Animator>());
            customerStates[customerIndex] = CustomerState.Leaving;
        }
    }

    int FindCustomerIndex(CustomerState condition)
    {
        for (int i = 0; i < customers.Length; i++)
        {
            if (customerStates[i] == condition)
            {
                return i;
            }
        }

        return 0;
    }

    public IEnumerator CurveMove(Transform tf, Vector3 start, Vector3 end,
        float height, int phase, CurveMoveCallback callback)
    {
        Vector3 midPoint = (start + end) / 2;
        float highest = Mathf.Max(Mathf.Max(start.y, midPoint.y), end.y);
        Vector3 top = new Vector3(midPoint.x, highest + height, midPoint.z);
        Vector3 direction = Vector3.zero;
        bool isDirectionSet = false;
        bool isDeltaDistanceSet = false;

        Vector3 prevDistance = Vector3.positiveInfinity;
        Vector3 deltaDistance = Vector3.zero;

        while (true)
        {
            Vector3 from = Vector3.zero;
            Vector3 to = Vector3.zero;

            if (!isDirectionSet)
            {
                direction = phase == 0 ? top - tf.position : end - tf.position;
                isDirectionSet = true;
            }

            if (phase == 0)
            {
                from = tf.position;
                to = top;
            }
            else if (phase == 1)
            {
                from = tf.position;
                to = end;
            }

            Vector3 distance;

            distance.x = Mathf.Abs(from.x - to.x);
            distance.y = Mathf.Abs(from.y - to.y);
            distance.z = Mathf.Abs(from.z - to.z);

            if (!isDeltaDistanceSet)
            {
                /*deltaDistance = distance / 7f;*/

                deltaDistance = new Vector3(1.2f, 2f, 1.2f);

                deltaDistance.x = direction.x > 0 ? deltaDistance.x : -deltaDistance.x;
                deltaDistance.y = direction.y > 0 ? deltaDistance.y : -deltaDistance.y;
                deltaDistance.z = direction.z > 0 ? deltaDistance.z : -deltaDistance.z;

                isDeltaDistanceSet = true;
            }

            bool condition0 = true;
            bool condition1 = true;
            bool condition2 = true;

            /*         if(distance.x > 1.5f * deltaDistance.x)
                     {
                         tf.Translate(deltaDistance, Space.World);
                     }
                     else
                     {
                         condition0 = false;
                         condition1 = false;
                         condition2 = false;
                     }*/

            if (distance.x > 1f * deltaDistance.x && distance.x < prevDistance.x)
            {
                tf.Translate(new Vector3(deltaDistance.x, 0, 0));
            }
            else
            {
                condition0 = false;
            }

            if (distance.y > 1f * deltaDistance.y && distance.y < prevDistance.y)
            {
                tf.Translate(new Vector3(0, deltaDistance.y, 0));
            }
            else
            {
                condition1 = false;
            }

            if (distance.z > 1f * deltaDistance.z && distance.z < prevDistance.z)
            {
                tf.Translate(new Vector3(0, 0, deltaDistance.z));
            }
            else
            {
                condition2 = false;
            }

            prevDistance = distance;

            if (!condition0 && !condition1 && !condition2)
            {
                if (phase == 0)
                {
                    phase++;
                    prevDistance = Vector3.positiveInfinity;
                    isDirectionSet = false;
                    isDeltaDistanceSet = false;
                }
                else if (phase == 1)
                {
                    break;
                }
            }

            yield return new WaitForSeconds(0.01f);
        }

        tf.position = end;

        callback();
    }

    void arrangeFoodYPosition(float minYPos, string ownerName, bool isContain = false)
    {
        int newIndex = -1;
        bool isBelongTo;

        for (int i = 0; i < foods.Length; i++)
        {
            isBelongTo = false;

            if (!isContain && foodBelongTo[i] == ownerName)
            {
                isBelongTo = true;
            }
            if (isContain && foodBelongTo[i].Contains(ownerName))
            {
                isBelongTo = true;
            }

            foodColumnIndex[i] = newIndex;

            if (isBelongTo)
            {
                newIndex++;
                foodColumnIndex[i] = newIndex;

                foods[i].transform.position = new Vector3(
                    foods[i].transform.position.x,
                    minYPos + foodColumnIndex[i] * foodSize.y,
                    foods[i].transform.position.z);
            }
        }
    }

    public void RotateChairs(int tableIndex, Vector3 angle1, Vector3 angle2)
    {
        StartCoroutine(util.RotateOverTime(
            chairs[2 * tableIndex].transform,
            angle1
            ));

        StartCoroutine(util.RotateOverTime(
            chairs[2 * tableIndex + 1].transform,
            angle2
            ));
    }

    int FindLastTrashIndexBelongTo(string owner)
    {
        int last = -1;

        for (int i = 0; i < trashs.Length; i++)
        {
            if (trashBelongTo[i] == owner)
            {
                last = Mathf.Max(last, trashColumnIndex[i]);
            }
        }

        return last;
    }

    public Vector3 GetFoodPositionInPackage(int packageIndex, int fillIndex)
    {
        int j = fillIndex % 2;
        int i = (fillIndex - j) / 2;

        return new Vector3(
            packages[packageIndex].transform.position.x
            - packageSize.x / 2 + (j + 1) * packageSize.x / (2 + 1),
            packages[packageIndex].transform.position.y + packageSize.y / 2,
            packages[packageIndex].transform.position.z
            + packageSize.z / 2 - (i + 1) * packageSize.z / (2 + 1)
        );
    }

    IEnumerator LoadGame()
    {
        RectTransform loadBarRT = loadBar.GetComponent<RectTransform>();
        RectTransform loadBarEmptyRT = loadBarEmpty.GetComponent<RectTransform>();

        while (loadBarRT.localScale.x < loadBarEmptyRT.localScale.x)
        {
            loadBarRT.localScale += new Vector3(0.015f, 0, 0);
            loadBarRT.localPosition = new Vector3(
                loadBarEmptyRT.localPosition.x - loadBarEmptyRT.rect.width / 2
                + loadBarRT.rect.width * loadBarRT.localScale.x / 2,
                loadBarRT.localPosition.y,
                loadBarRT.localPosition.z
            );

            yield return new WaitForSeconds(0.02f);
        }

        tutorialText.gameObject.SetActive(true);
        tutorialTextBackground.gameObject.SetActive(true);

        loadGameScreen.SetActive(false);
    }

    public List<int> FindAndSortFood(string owner, bool isContain = false)
    {
        List<int> foundIndexes = new List<int>();

        for (int i = 0; i < foods.Length; i++)
        {
            if (!isContain && foodBelongTo[i] == owner)
            {
                foundIndexes.Add(i);
            }
            if (isContain && foodBelongTo[i].Contains(owner))
            {
                foundIndexes.Add(i);
            }
        }

        for (int i = 0; i < foundIndexes.Count - 1; i++)
        {
            for (int j = i + 1; j < foundIndexes.Count; j++)
            {
                if (foodColumnIndex[foundIndexes[j]] > foodColumnIndex[foundIndexes[i]])
                {
                    int temp = foundIndexes[j];
                    foundIndexes[j] = foundIndexes[i];
                    foundIndexes[i] = temp;
                }
            }
        }

        return foundIndexes;
    }

    void ShowCustomerDemandDialog(int customerIndex)
    {
        customerDialog.transform.position =
        new Vector3(
            customers[customerIndex].transform.position.x,
            customers[customerIndex].transform.position.y + 24,
        customers[customerIndex].transform.position.z
        );

        customerDialog.transform.
            GetChild(0).GetComponent<TMP_Text>().text =
            "2";

        customerDialog.SetActive(true);
    }

    IEnumerator AutoCashier(int customerIndex)
    {
        customerStates[customerIndex] = CustomerState.PickingFood;

        while (customerStates[customerIndex] == CustomerState.PickingFood)
        {
            if (numFoodHoldOf("counter") > 0)
            {
                Animator cashierAnimator = npcManager.npcControllers[0].animator;

                util.SetCashingAnimation(cashierAnimator);

                yield return new WaitForSeconds(2f);

                util.SetCashingFinishAnimation(cashierAnimator);

                StartCoroutine(util.ScaleUpDownEffect(counter.transform, 0.15f));

                StartCoroutine(moveFoodOneByOneToCustomer(customers[customerIndex].transform, customerIndex));
            }

            yield return new WaitForSeconds(3f);
        }
    }
}
