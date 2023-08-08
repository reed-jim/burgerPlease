using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FoodState
{
    NotSpawn,
    Wait,
    Picking,
    Delivering,
    ReachCounter,
    PuttingInCounter,
    CustomerPicking,
    CustomerPicked,
    TESTING
}

public enum CustomerState
{
    NotSpawn,
    GoInside,
    Waiting,
    PickingFood,
    GetToTable,
    Eating,
    Leaving
}

public class Simulator : MonoBehaviour
{
    public GameObject customerDialog;

    public GameObject player;
    public GameObject customerPrefab;
    public GameObject foodPrefab;
    public GameObject counter;
    public GameObject foodDeliver;
    public GameObject[] tables;
    public float speed = 5f;
    public Vector3 minDistance;
    public float customerDistance;

    public MoneyPileManager moneyPileManager;
    public NPC_Manager npcManager;
    public PlayerController playerController;

    private GameObject[] customers = new GameObject[10];
    public GameObject[] foods = new GameObject[6];
    public CustomerState[] customerStates = new CustomerState[10];
    public int[] customerFoodMap;
    public FoodState[] foodStates = new FoodState[6];
    public string[] foodBelongTo;
    public int[] foodColumnIndex;
    public int firstQueueCustomerIndex = 0;



    public Vector3 counterSize;
    public Vector3 foodSize;


    private bool isShowCustomerDialog = false;

    private float deltaTime;

    public delegate void CurveMoveCallback();

    // Start is called before the first frame update
    void Start()
    {
        customerFoodMap = new int[customers.Length];
        foodBelongTo = new string[foods.Length];
        foodColumnIndex = new int[foods.Length];

        for (int i = 0; i < customerFoodMap.Length; i++)
        {
            customerFoodMap[i] = -1;
        }

        deltaTime = Time.deltaTime;

        for (int i = 0; i < customers.Length; i++)
        {
            customers[i] = Instantiate(customerPrefab);
            customers[i].SetActive(false);
            customerStates[i] = CustomerState.NotSpawn;
        }

        for (int i = 0; i < foods.Length; i++)
        {
            foods[i] = Instantiate(foodPrefab);
            foods[i].SetActive(false);
            foodStates[i] = FoodState.NotSpawn;
        }

        counterSize = counter.GetComponent<MeshRenderer>().bounds.size;

        for (int i = 0; i < foods[0].transform.childCount; i++)
        {
            foodSize += foods[0].transform.GetChild(i).GetComponent<MeshRenderer>().bounds.size;
        }

        StartCoroutine(SpawnCustomer());
        StartCoroutine(SpawnFood());

        customerDialog.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < customers.Length; i++)
        {
            if (customerStates[i] == CustomerState.GoInside)
            {
                float zLimit = counter.transform.position.z - customerDistance;

                if (i == 0)
                {
                    if (customerStates[customerStates.Length - 1] == CustomerState.Waiting)
                    {
                        zLimit = customers[customerStates.Length - 1].transform.position.z - customerDistance;
                    }
                }
                else
                {
                    if (customerStates[i - 1] == CustomerState.Waiting)
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

                    if (!isShowCustomerDialog)
                    {
                        customerDialog.transform.position =
                            new Vector3(
                                customers[i].transform.position.x,
                                customers[i].transform.position.y + 25,
                            customers[i].transform.position.z
                            );

                        customerDialog.SetActive(true);
                        isShowCustomerDialog = true;
                    }
                }
            }
            else if (customerStates[i] == CustomerState.GetToTable)
            {
                Vector3 tablePosition = tables[0].transform.position;

                if (Vector3.Distance(customers[i].transform.position, tablePosition) > 1)
                {
                    customers[i].transform.position = Vector3.MoveTowards(customers[i].transform.position,
                    tablePosition, speed * deltaTime);
                }
                else
                {
                    int foodIndex = customerFoodMap[i];

                    customers[i].transform.position = new Vector3(
                        tablePosition.x - 4, customers[i].transform.position.y, tablePosition.z
                        );
                    customers[i].GetComponent<Animator>().SetBool("isMoving", false);
                    foods[foodIndex].transform.position = tablePosition;

                    StartCoroutine(Eat(i));
                    customerStates[i] = CustomerState.Eating;
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
                    GameObject npc = new GameObject();

                    for (int j = 0; j < npcManager.npcs.Length; j++)
                    {
                        if (foodBelongTo[i] == npcManager.npcControllers[j].id)
                        {
                            npc = npcManager.npcs[j];
                        }
                    }

                    foods[i].transform.position = new Vector3
                    (
                        npc.transform.position.x,
                        npc.transform.position.y +
                        7 + foodColumnIndex[i] * foodSize.y,
                        npc.transform.position.z
                    ) +
                        npc.transform.forward * 2;
                }
            }

            if (foodStates[i] == FoodState.ReachCounter)
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
                            foodStates[index] = FoodState.CustomerPicking;

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
                        }
                    )
                );

                foodStates[i] = FoodState.PuttingInCounter;
            }

            if (foodStates[i] == FoodState.CustomerPicking)
            {
                for (int j = 0; j < customers.Length; j++)
                {
                    if (customerStates[j] == CustomerState.Waiting &&
                        j == firstQueueCustomerIndex)
                    {
                        Transform customerTf = customers[j].transform;
                        int foodIndex = i;
                        int custommerIndex = j;

                        StartCoroutine(
                            CurveMove(
                                foods[i].transform,
                                foods[i].transform.position,
                                customerTf.position + new Vector3(0, 7, 0)
                                + customerTf.forward * 4
                                ,
                                12,
                                0,
                                () =>
                                {
                                    customerStates[custommerIndex] = CustomerState.GetToTable;
                                    customers[custommerIndex].transform.LookAt(tables[0].transform);
                                    customers[custommerIndex].GetComponent<Animator>().SetBool("isMoving", true);

                                    setNewFirstQueueCustomerIndex();

                                    foodStates[foodIndex] = FoodState.CustomerPicked;

                                    customerFoodMap[custommerIndex] = foodIndex;

                                    for (int k = 0; k < customers.Length; k++)
                                    {
                                        if (customerStates[k] == CustomerState.Waiting)
                                        {
                                            customers[k].GetComponent<Animator>().SetBool("isMoving", true);
                                            customerStates[k] = CustomerState.GoInside;
                                        }
                                    }
                                }
                            )
                        );

                        customerStates[j] = CustomerState.PickingFood;

                        break;
                    }
                }
            }

            if (foodStates[i] == FoodState.CustomerPicked)
            {
                for (int j = 0; j < customers.Length; j++)
                {
                    if (customerStates[j] == CustomerState.GetToTable)
                    {
                        foods[i].transform.position = new Vector3
                    (
                        customers[j].transform.position.x,
                        customers[j].transform.position.y + 6,
                        customers[j].transform.position.z
                    ) +
                        customers[j].transform.forward * 3;

                        break;
                    }
                }
            }
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
            yield return new WaitForSeconds(3f);
            customers[customerIndex].transform.LookAt(new Vector3(
                customers[customerIndex].transform.position.x,
                customers[customerIndex].transform.position.y, -30
                ));
            moneyPileManager.SpawnMoneyPile(new Vector3(
                tables[0].transform.position.x,
                0,
                tables[0].transform.position.z - 10
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
}
