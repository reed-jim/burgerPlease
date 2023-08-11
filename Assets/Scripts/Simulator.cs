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
    InPackage
}

public enum CustomerState
{
    NotSpawn,
    GoInside,
    Waiting,
    PickingFood,
    ChooseTable,
    GetToTable,
    Eating,
    Leaving
}

public enum TableState
{
    EmptyBoth,
    EmptyLeft,
    EmptyRight,
    Full,
    Dirty
}

public enum PackageState
{
    NotSpawn,
    Filling,
    Moving,
    Filled
}

public enum TrashState
{
    NotSpawn,
    Picking,
    Holding,
    MovingToTrashCan
}

public class Simulator : MonoBehaviour
{
    public GameObject customerDialog;
    public GameObject noSeatDialog;

    public GameObject player;
    public GameObject customerPrefab;
    public GameObject foodPrefab;
    public GameObject trashPrefab;
    public GameObject packagePrefab;
    public GameObject counter;
    public GameObject foodDeliver;
    public GameObject packageTable;
    public GameObject takeawayCounter;
    public GameObject[] tables;
    private GameObject[] chairs;
    public GameObject[] trashs;
    public float speed = 5f;
    public Vector3 minDistance;
    public float customerDistance;

    public MoneyPileManager moneyPileManager;
    public NPC_Manager npcManager;
    public PlayerController playerController;
    public Util util;

    private GameObject[] customers = new GameObject[10];
    public GameObject[] foods = new GameObject[6];
    public GameObject[] packages = new GameObject[10];
    public CustomerState[] customerStates;
    public int[] customerNumFoodDemand;
    public FoodState[] foodStates = new FoodState[6];
    public string[] foodBelongTo;
    public int[] foodColumnIndex;
    public TableState[] tableStates;
    public string[] customerTableMap;

    public PackageState[] packageStates;
    public string[] packageBelongTo;
    public int[] foodIndexInPackage;

    public TrashState[] trashStates;
    public string[] trashBelongTo;
    public int[] trashColumnIndex;

    public int firstQueueCustomerIndex = 0;



    public Vector3 counterSize;
    public Vector3 foodSize;
    public Vector3 tableSize;
    public Vector3 trashSize;
    public Vector3 packageSize;
    public Vector3 packageTableSize;
    public Vector3 takeawayCounterSize;

    private bool isShowCustomerDialog = false;

    private float deltaTime;

    public delegate void CurveMoveCallback();

    // Start is called before the first frame update
    void Start()
    {
        foodBelongTo = new string[foods.Length];
        foodColumnIndex = new int[foods.Length];
        customerStates = new CustomerState[customers.Length];
        customerNumFoodDemand = new int[customers.Length];
        tableStates = new TableState[tables.Length];
        customerTableMap = new string[customers.Length];
        chairs = new GameObject[tables.Length * 2];

        trashs = new GameObject[10];
        trashStates = new TrashState[trashs.Length];
        trashBelongTo = new string[trashs.Length];
        trashColumnIndex = new int[trashs.Length];
        packageStates = new PackageState[packages.Length];
        packageBelongTo = new string[packages.Length];
        foodIndexInPackage = new int[packages.Length];


        deltaTime = Time.deltaTime;

        for (int i = 0; i < customers.Length; i++)
        {
            customers[i] = Instantiate(customerPrefab);
            customers[i].SetActive(false);
            customerStates[i] = CustomerState.NotSpawn;
            customerTableMap[i] = "none";
        }

        for (int i = 0; i < foods.Length; i++)
        {
            foods[i] = Instantiate(foodPrefab);
            foods[i].SetActive(false);
            foodStates[i] = FoodState.NotSpawn;
        }

        for (int i = 0; i < packages.Length; i++)
        {
            packages[i] = Instantiate(packagePrefab);
            packages[i].SetActive(false);
            packageStates[i] = PackageState.NotSpawn;
            packages[i].GetComponent<Package>().index = i;
        }

        for (int i = 0; i < tables.Length; i++)
        {
            chairs[2 * i] = tables[i].transform.GetChild(1).gameObject;
            chairs[2 * i + 1] = tables[i].transform.GetChild(2).gameObject;
            tableStates[i] = TableState.EmptyBoth;
        }

        for (int i = 0; i < trashs.Length; i++)
        {
            trashs[i] = Instantiate(trashPrefab);
            trashs[i].SetActive(false);

            trashStates[i] = TrashState.NotSpawn;
            trashColumnIndex[i] = -1;
        }

        counterSize = counter.GetComponent<MeshRenderer>().bounds.size;
        for (int i = 0; i < foods[0].transform.childCount; i++)
        {
            foodSize += foods[0].transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size;
        }
        packageSize = packages[0].GetComponent<MeshRenderer>().bounds.size;
        packageTableSize = packageTable.GetComponent<MeshRenderer>().bounds.size;
        takeawayCounterSize = takeawayCounter.GetComponent<MeshRenderer>().bounds.size;
        tableSize = tables[0].transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;
        trashSize = trashs[0].GetComponent<MeshRenderer>().bounds.size;

        StartCoroutine(SpawnCustomer());
        StartCoroutine(SpawnFood());
        StartCoroutine(SpawnPackage());

        StartCoroutine(SimulateCustomerCycle());
        StartCoroutine(SimulateFoodCycle());
        StartCoroutine(SimulatePackageCycle());
        StartCoroutine(SimulateTrashCycle());

        customerDialog.SetActive(false);
    }

    IEnumerator SimulateCustomerCycle()
    {
        while (true)
        {
            for (int i = 0; i < customers.Length; i++)
            {
                if (customerStates[i] == CustomerState.GoInside)
                {
                    float zLimit = counter.transform.position.z - customerDistance;

                    if (i == 0)
                    {
                        if (customerStates[customerStates.Length - 1] == CustomerState.Waiting ||
                            customerStates[customerStates.Length - 1] == CustomerState.ChooseTable)
                        {
                            zLimit = customers[customerStates.Length - 1].transform.position.z - customerDistance;
                        }
                    }
                    else
                    {
                        if (customerStates[i - 1] == CustomerState.Waiting ||
                            customerStates[i - 1] == CustomerState.ChooseTable ||
                            customerStates[i - 1] == CustomerState.PickingFood)
                        {
                            zLimit = customers[i - 1].transform.position.z - customerDistance;
                        }
                    }

                    if (customers[i].transform.position.z < zLimit)
                    {
                        customers[i].transform.Translate(new Vector3(0, 0, speed * deltaTime));
                    }
                    else
                    {
                        customers[i].GetComponent<Animator>().SetBool("isMoving", false);
                        customerStates[i] = CustomerState.Waiting;

                        if (i == firstQueueCustomerIndex)
                        {
                            customerDialog.transform.position =
                                new Vector3(
                                    customers[i].transform.position.x,
                                    customers[i].transform.position.y + 20,
                                customers[i].transform.position.z
                                );

                            customerDialog.transform.
                                GetChild(0).GetComponent<TMP_Text>().text =
                                "2";
                            customerDialog.SetActive(true);

                            isShowCustomerDialog = true;
                        }
                    }

                    yield return new WaitForSeconds(0.05f);
                }
                else if (customerStates[i] == CustomerState.Waiting)
                {
                    if (i == firstQueueCustomerIndex &&
                        numFoodAvailableForCustomer() >= customerNumFoodDemand[i])
                    {
                        StartCoroutine(moveFoodOneByOneToCustomer(
                            customers[i].transform, i
                            ));

                        customerStates[i] = CustomerState.PickingFood;
                    }
                }
                else if (customerStates[i] == CustomerState.ChooseTable)
                {
                    Vector3 destination = Vector3.zero;
                    bool isFoundTable = false;

                    for (int j = 0; j < tables.Length; j++)
                    {
                        if (tableStates[j] == TableState.EmptyBoth)
                        {
                            destination = new Vector3(
                                tables[j].transform.position.x - 6,
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
                                tables[j].transform.position.x,
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
                                tables[j].transform.position.x + tableSize.x,
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

                            customers[i].transform.LookAt(tables[0].transform);
                            customers[i].GetComponent<Animator>().SetBool("isMoving", true);
                            customers[i].GetComponent<Animator>().SetBool("isHoldingFoodStanding", false);

                            StartCoroutine
                            (
                                util.MoveTo(customers[i].transform, destination, speed,
                                () =>
                                {
                                    customers[customerIndex].GetComponent<Animator>().SetBool("isMoving", false);
                                    customers[customerIndex].GetComponent<Animator>().SetBool("isHoldingFood", false);

                                    for (int j = 0; j < foods.Length; j++)
                                    {
                                        if (foodBelongTo[j] == "customer" + customerIndex)
                                        {
                                            foodBelongTo[j] = "customer" + customerIndex + "-"
                                            + "table" + tableIndex;
                                            foodColumnIndex[j] = numFoodHoldOf("table" + tableIndex, true) - 1;

                                            Vector3 end = new Vector3(
                                            tables[tableIndex].transform.position.x + tableSize.x / 2,
                                            tables[tableIndex].transform.position.y + tableSize.y
                                            + foodColumnIndex[j] * foodSize.y,
                                            tables[tableIndex].transform.position.z + tableSize.z / 2
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
                                        }
                                    }

                                    StartCoroutine(Eat(customerIndex));
                                    customerStates[customerIndex] = CustomerState.Eating;
                                }
                                )
                            );

                            setNewFirstQueueCustomerIndex();

                            for (int k = 0; k < customers.Length; k++)
                            {
                                if (customerStates[k] == CustomerState.Waiting)
                                {
                                    customers[k].GetComponent<Animator>().SetBool("isMoving", true);
                                    customerStates[k] = CustomerState.GoInside;
                                }
                            }

                            customerDialog.SetActive(false);

                            customerStates[i] = CustomerState.GetToTable;
                            break;
                        }
                    }

                    if (isFoundTable == false)
                    {
                        if (!noSeatDialog.activeInHierarchy)
                        {
                            noSeatDialog.transform.position = customers[i].transform.position
                            + new Vector3(0, 20, 0);

                            customerDialog.SetActive(false);
                            noSeatDialog.SetActive(true);
                        }
                    }
                    else
                    {
                        noSeatDialog.SetActive(false);
                    }
                }
                else if (customerStates[i] == CustomerState.GetToTable)
                {
                    /*Vector3 tablePosition = tables[0].transform.position;

                    if (Vector3.Distance(customers[i].transform.position, tablePosition) > 1)
                    {
                        customers[i].transform.position = Vector3.MoveTowards(customers[i].transform.position,
                        tablePosition, speed * deltaTime);
                    }
                    else
                    {
                        customers[i].transform.position = new Vector3(
                            tablePosition.x - 4, customers[i].transform.position.y, tablePosition.z
                            );
                        customers[i].GetComponent<Animator>().SetBool("isMoving", false);

                        for (int j = 0; j < foods.Length; j++)
                        {
                            if (foodBelongTo[j] == "customer" + i)
                            {
                                foods[j].transform.position = new Vector3(
                                    tablePosition.x,
                                    tablePosition.y + foodColumnIndex[j] * foodSize.y,
                                    tablePosition.z
                                    );
                            }
                        }

                        StartCoroutine(Eat(i));
                        customerStates[i] = CustomerState.Eating;
                    }*/
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

                    yield return new WaitForSeconds(0.05f);
                }
            }

            yield return new WaitForSeconds(0.03f);
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
                            7 + foodColumnIndex[i] * foodSize.y,
                            player.transform.position.z
                        ) +
                            player.transform.forward * 2;
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
                            npcManager.npcs[npcIndex].transform.forward * 2;
                    }
                }
                else if (foodStates[i] == FoodState.ReachCounter)
                {
                    int index = i;

                    StartCoroutine(
                            CurveMove(
                                foods[i].transform,
                                foods[i].transform.position,
                                counter.transform.position +
                                new Vector3(6,
                                counterSize.y / 2 +
                                foodColumnIndex[i] * foodSize.y,
                                0),
                                12,
                                0,
                                () =>
                                {
                                    if (playerController.numberFoodHold > 0)
                                    {
                                        playerController.numberFoodHold--;
                                    }
                                    else
                                    {
                                        playerController.playerState = PlayerState.Ready;
                                        playerController.playerAnimator.SetBool("isHoldingFood", false);
                                        playerController.playerAnimator.SetBool("isHoldingFoodStanding", false);
                                    }

                                    foodStates[index] = FoodState.CustomerPick;
                                }
                            )
                        );

                    foodStates[i] = FoodState.PuttingInCounter;
                }
                else if(foodStates[i] == FoodState.MovingWithPackage)
                {
                    int packageIndex = int.Parse(foodBelongTo[i][7].ToString());

                    foods[i].transform.position = GetFoodPositionInPackage(packageIndex, i);
                }
                else if (foodStates[i] == FoodState.CustomerPick)
                {
                    /*for (int j = 0; j < customers.Length; j++)
                    {
                        if (customerStates[j] == CustomerState.Waiting &&
                            j == firstQueueCustomerIndex)
                        {
                            Transform customerTf = customers[j].transform;
                            int foodIndex = i;
                            int customerIndex = j;

                            foodBelongTo[i] = "customer" + customerIndex;

                            int numFoodCustomerHold = numFoodHoldOf("customer" + customerIndex);

                            foodColumnIndex[i] = numFoodCustomerHold - 1;
                            foodStates[i] = FoodState.CustomerPicking;

                            StartCoroutine(
                                CurveMove(
                                    foods[i].transform,
                                    foods[i].transform.position,
                                    customerTf.position
                                    + new Vector3(0, 7 + foodColumnIndex[i] * foodSize.y, 0)
                                    + customerTf.forward * 2
                                    ,
                                    12,
                                    0,
                                    () =>
                                    {
                                        if (numFoodCustomerHold == 1)
                                        {
                                            customers[customerIndex].GetComponent<Animator>().SetBool("isHoldingFood", true);
                                            customers[customerIndex].GetComponent<Animator>().SetBool("isHoldingFoodStanding", true);
                                        }

                                        if (numFoodCustomerHold == customerNumFoodDemand[customerIndex])
                                        {
                                            customerStates[customerIndex] = CustomerState.ChooseTable;

                                            setNewFirstQueueCustomerIndex();

                                            for (int k = 0; k < customers.Length; k++)
                                            {
                                                if (customerStates[k] == CustomerState.Waiting)
                                                {
                                                    customers[k].GetComponent<Animator>().SetBool("isMoving", true);
                                                    customerStates[k] = CustomerState.GoInside;
                                                }
                                            }
                                        }

                                        foodStates[foodIndex] = FoodState.CustomerPicked;
                                    }
                                )
                            );

                            yield return new WaitForSeconds(0.5f);

                            break;
                        }
                    }*/
                }
                else if (foodStates[i] == FoodState.CustomerPicked)
                {
                    for (int j = 0; j < customers.Length; j++)
                    {
                        if (customerStates[j] == CustomerState.GetToTable &&
                            foodBelongTo[i] == "customer" + j)
                        {

                            foods[i].transform.position = new Vector3
                                (
                                    customers[j].transform.position.x,
                                    customers[j].transform.position.y
                                    + 7 + foodColumnIndex[i] * foodSize.y,
                                    customers[j].transform.position.z
                                ) +
                                    customers[j].transform.forward * 2;

                            break;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.03f);
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
                    if (packageBelongTo[i].Contains("npc"))
                    {
                        int npcIndex = int.Parse(packageBelongTo[i][3].ToString());

                        packages[i].transform.position = npcManager.npcs[npcIndex].transform.position
                            + new Vector3(0, 7, 0)
                            + npcManager.npcs[npcIndex].transform.forward * 2;
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
                            + new Vector3(0, 7 + trashColumnIndex[i] * trashSize.y, 0)
                            + player.transform.forward * 2;
                    }
                }
            }

            yield return new WaitForSeconds(0.03f);
        }
    }

    IEnumerator SpawnCustomer()
    {
        while (true)
        {
            for (int i = 0; i < customers.Length; i++)
            {
                if (customerStates[i] == CustomerState.NotSpawn)
                {
                    customers[i].SetActive(true);
                    customers[i].GetComponent<Animator>().SetBool("isMoving", true);
                    customers[i].transform.position = new Vector3(
                        counter.transform.position.x, customers[i].transform.position.y,
                        Random.Range(-30, -20));

                    customerStates[i] = CustomerState.GoInside;
                    customerNumFoodDemand[i] = 2;
                    break;
                }
            }

            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator SpawnFood()
    {
        while (true)
        {
            for (int i = 0; i < foods.Length; i++)
            {
                if (foodStates[i] == FoodState.NotSpawn)
                {
                    int row = 2;
                    int col = foods.Length / row;

                    Vector3 foodDeliverSize = foodDeliver.GetComponent<MeshRenderer>().bounds.size;
                    int x = i % col;
                    int y = (i - x) / col;

                    foods[i].transform.position = new Vector3(
                        foodDeliver.transform.position.x - foodDeliverSize.x / 2 + (x + 1) * foodDeliverSize.x / (col + 1),
                        foodDeliver.transform.position.y + (foodDeliverSize.y + foodSize.y) / 2,
                        foodDeliver.transform.position.z + foodDeliverSize.z / 2 - (y + 1) * foodDeliverSize.z / (row + 1)
                    );

                    foods[i].SetActive(true);
                    foodStates[i] = FoodState.Wait;

                    break;
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Eat(int customerIndex)
    {
        while (customerStates[customerIndex] != CustomerState.Leaving)
        {
            yield return new WaitForSeconds(10f);

            customers[customerIndex].transform.LookAt(new Vector3(
                customers[customerIndex].transform.position.x,
                customers[customerIndex].transform.position.y, -30
                ));

            int tableIndex = int.Parse(customerTableMap[customerIndex][5].ToString());
            string tableSeat = customerTableMap[customerIndex].Substring(7);


            for (int j = 0; j < foods.Length; j++)
            {
                if (foodBelongTo[j] == "customer" + customerIndex + "-table" + tableIndex)
                {
                    resetFoodProperties(j);
                }
            }

            arrangeFoodYPosition(tables[tableIndex].transform.position.y
                + tableSize.y + foodSize.y / 2,
                "table" + tableIndex, true);


            RotateChairs(tableIndex, new Vector3(0, 30, 0), new Vector3(0, 210, 0));
            tableStates[tableIndex] = TableState.Dirty;

            Vector3 tablePosition = tables[tableIndex].transform.position;

            if (numFoodHoldOf("table" + tableIndex, true) == 0)
            {
                SpawnTrash(
                    new Vector3(tablePosition.x + tableSize.x / 2,
                    tablePosition.y + tableSize.y,
                    tablePosition.z + tableSize.z / 2),
                    "table" + tableIndex
                    );
            }

            /*if (numFoodHoldOf("table" + tableIndex, true) == 2)
            {
                rotateChair(tableIndex);
                tableStates[tableIndex] = TableState.Dirty;
            }
            else
            {
                if (tableStates[tableIndex] == TableState.Full)
                {
                    if (tableSeat == "left")
                    {
                        tableStates[tableIndex] = TableState.EmptyLeft;
                    }
                    if (tableSeat == "right")
                    {
                        tableStates[tableIndex] = TableState.EmptyRight;
                    }
                }
                else
                {
                    tableStates[tableIndex] = TableState.EmptyBoth;
                }
            }*/

            moneyPileManager.SpawnMoneyPile(new Vector3(
                tables[tableIndex].transform.position.x,
                0,
                tables[tableIndex].transform.position.z + tableSize.z + 10
                ));

            setCustomerMoving(customerIndex, true);
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

    void setNewFirstQueueCustomerIndex()
    {
        if (firstQueueCustomerIndex < customers.Length - 1)
        {
            firstQueueCustomerIndex++;
        }
        else
        {
            firstQueueCustomerIndex = 0;
        }
    }

    public IEnumerator CurveMove(Transform tf, Vector3 start, Vector3 end,
        float height, int phase, CurveMoveCallback callback)
    {
        Vector3 midPoint = (start + end) / 2;
        Vector3 top = new Vector3(midPoint.x, midPoint.y + height, midPoint.z);
        Vector3 direction;

        if (phase == 0)
        {
            direction = top - tf.position;

            while (Vector3.Distance(tf.position, top) > 1)
            {
                tf.Translate(direction * speed / 1f * deltaTime);

                yield return new WaitForSeconds(0.02f);
            }

            StartCoroutine(CurveMove(tf, start, end, height, 1, callback));
        }
        else if (phase == 1)
        {
            direction = end - tf.position;

            while (Vector3.Distance(tf.position, end) > 1)
            {
                tf.Translate(direction * speed / 1f * deltaTime);

                yield return new WaitForSeconds(0.02f);
            }

            callback();
        }

        /*while (Vector3.Distance(tf.position, end) > 0f)
        {
            if(phase == 0)
            {
                if (tf.position.y < top.y)
                {
                    direction = top - start;
                }
                else
                {
                    
                }
            }
            

            if (Mathf.Abs(tf.position.x - start.x) < Mathf.Abs(end.x - start.x) / 2)
            {
                direction = top - start;
            }
            else
            {

                direction = end - top;
            }

            tf.Translate(direction * speed / 3f * deltaTime);

            yield return new WaitForSeconds(0.02f);
        }*/
    }

    void setCustomerMoving(int index, bool isMoving)
    {
        customers[index].GetComponent<Animator>().SetBool("isMoving", isMoving);
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

    void resetFoodProperties(int index)
    {
        foods[index].SetActive(false);
        foodBelongTo[index] = "storage";
        foodColumnIndex[index] = -1;
        foodStates[index] = FoodState.NotSpawn;
    }

    IEnumerator moveFoodOneByOneToCustomer(Transform customerTf, int customerIndex)
    {
        for (int i = 0; i < foods.Length; i++)
        {

            if (numFoodHoldOf("customer" + customerIndex) < 2)
            {
                if (foodStates[i] == FoodState.CustomerPick)
                {
                    int foodIndex = i;

                    foodBelongTo[i] = "customer" + customerIndex;

                    int numFoodCustomerHold = numFoodHoldOf("customer" + customerIndex);

                    foodColumnIndex[i] = numFoodCustomerHold - 1;
                    foodStates[i] = FoodState.CustomerPicking;

                    StartCoroutine(
                        CurveMove(
                            foods[i].transform,
                            foods[i].transform.position,
                            customerTf.position
                            + new Vector3(0, 7 + foodColumnIndex[i] * foodSize.y, 0)
                            + customerTf.forward * 2
                            ,
                            12,
                            0,
                            () =>
                            {
                                if (numFoodCustomerHold == 1)
                                {
                                    customers[customerIndex].GetComponent<Animator>().SetBool("isHoldingFood", true);
                                    customers[customerIndex].GetComponent<Animator>().SetBool("isHoldingFoodStanding", true);
                                }

                                if (numFoodCustomerHold == customerNumFoodDemand[customerIndex])
                                {
                                    customerStates[customerIndex] = CustomerState.ChooseTable;
                                }

                                foodStates[foodIndex] = FoodState.CustomerPicked;
                            }
                        )
                    );
                }
            }
            else
            {
                break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator moveTrashOneByOne(string owner, Transform targetTf, int tableIndex)
    {
        int capacity = 2;

        int lastIndexBelongToTarget = FindLastTrashIndexBelongTo(owner);
        int newIndex = lastIndexBelongToTarget;

        for (int i = 0; i < trashs.Length; i++)
        {
            if (trashBelongTo[i] == "table" + tableIndex)
            {
                newIndex++;

                trashColumnIndex[i] = newIndex;
                trashBelongTo[i] = owner;
                trashStates[i] = TrashState.Picking;

                StartCoroutine(
                        CurveMove(
                            trashs[i].transform,
                            trashs[i].transform.position,
                            targetTf.position
                            + new Vector3(0, 7 + trashColumnIndex[i] * trashSize.y, 0)
                            + targetTf.forward * 2
                            ,
                            12,
                            0,
                            () =>
                            {
                                trashStates[i] = TrashState.Holding;
                            }
                        )
                    );
            }
            else
            {
                break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void resetTrashProperties(int index)
    {
        trashs[index].SetActive(false);
        trashBelongTo[index] = "none";
        trashColumnIndex[index] = -1;
        trashStates[index] = TrashState.NotSpawn;
    }

    int numFoodAvailableForCustomer()
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

    void SpawnTrash(Vector3 position, string belongTo)
    {
        int currentHighestIndex = trashColumnIndex.Max();
        int newIndex = currentHighestIndex;
        int numSpawn = 0;

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
                        trashColumnIndex[i] * trashSize.y,
                        0
                    );

                trashs[i].SetActive(true);

                numSpawn++;
            }
            if (numSpawn > 1)
            {
                break;
            }
        }
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

    IEnumerator SpawnPackage()
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
                packageTable.transform.position.x + packageTableSize.x / 4,
                packageTable.transform.position.y + (packageTableSize.y + packageSize.y) / 2,
                packageTable.transform.position.z
            );

            packages[spawnIndex].SetActive(true);

            packageBelongTo[spawnIndex] = "package table";
            packageStates[spawnIndex] = PackageState.Filling;
        }

        yield return new WaitForSeconds(0.05f);
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

    public int numFillOfPackage(int packageIndex)
    {
        int num = 0;

        for (int i = 0; i < foods.Length; i++)
        {
            if(foodBelongTo[i] == "package" + packageIndex)
            {
                num++;
            }
        }

        return num;
    }
}
