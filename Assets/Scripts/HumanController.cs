using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanState
{
    Ready,
    Moving,
    CashierWaiting,
    PickingFood,
    HoldingFoodMoving,
    PickingPackage,
    HoldingPackageMoving,
    PackagingFood,
    MovedAllFoodToPackage,
}

public enum Task
{
    Nothing,
    Cashier,
    ServingFood,
    PackagingFood,
    DeliverTakeawayFood
}

public class HumanController : MonoBehaviour
{
    private GameObject foodStorage;
    private GameObject counter;

    private Simulator simulator;
    private NPC_Manager npcManager;
    private Util util;

    public Animator animator;
    public HumanState humanState = HumanState.Ready;
    public Task task;
   
    public int numFoodHold = 0;
    private Vector3 movingDestination;
    public bool setOnetimeValues = true;

    public string id;
    public float speed = 2;
    public int capacity = 2;

    private float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        foodStorage = GameObject.Find("Food Storage");
        counter = GameObject.Find("Counter");
        simulator = GameObject.Find("Simulator").GetComponent<Simulator>();
        npcManager = GameObject.Find("NPC_Manager").GetComponent<NPC_Manager>();
        util = GameObject.Find("Util").GetComponent<Util>();
        animator = GetComponent<Animator>();

        deltaTime = Time.deltaTime;

        StartCoroutine(Simulate());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Simulate()
    {
        while (true)
        {
            if (humanState == HumanState.Ready)
            {
                task = task == Task.Cashier ? task : FindTask();

                if(task == Task.Cashier)
                {
                    humanState = HumanState.CashierWaiting;
                }
                else if(task == Task.ServingFood ||
                    task == Task.PackagingFood)
                {
                    movingDestination = new Vector3(
                        foodStorage.transform.position.x,
                        transform.position.y,
                        foodStorage.transform.position.z
                    );

                    humanState = HumanState.Moving;
                }
                else if(task == Task.DeliverTakeawayFood)
                {
                    for (int i = 0; i < simulator.packages.Length; i++)
                    {
                        if (simulator.packageStates[i] == PackageState.Filled)
                        {
                            movingDestination = new Vector3(
                                simulator.packages[i].transform.position.x,
                                transform.position.y,
                                simulator.packages[i].transform.position.z
                                - simulator.packageSize.z
                            );

                            break;
                        }
                    }

                    humanState = HumanState.Moving;
                }
                else if(task == Task.Nothing)
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }
            else if (humanState == HumanState.Moving)
            {
                if (setOnetimeValues)
                {                  
                    transform.LookAt(movingDestination);

                    util.SetMovingAnimation(animator);
                }
                else
                {
                    setOnetimeValues = false;
                }

                transform.Translate(transform.forward * speed * deltaTime, Space.World);
            }
            else if (humanState == HumanState.HoldingFoodMoving)
            {
                if (setOnetimeValues)
                {
                    if (task == Task.ServingFood)
                    {
                        movingDestination = new Vector3(
                            simulator.bowl.transform.position.x,
                            transform.position.y,
                            simulator.bowl.transform.position.z
                        );
                    }
                    else if (task == Task.PackagingFood)
                    {
                        movingDestination = new Vector3(
                            simulator.packageTable.transform.position.x + 0.8f * simulator.packageTableSize.x,
                            transform.position.y,
                            simulator.packageTable.transform.position.z
                        );
                    }
                    else if(task == Task.DeliverTakeawayFood)
                    {
                        movingDestination = new Vector3(
                            simulator.takeawayCounter.transform.position.x
                            + simulator.takeawayCounterSize.x,
                            transform.position.y,
                            simulator.takeawayCounter.transform.position.z
                        );
                    }

                    transform.LookAt(movingDestination);

                    util.SetHoldingFoodMovingAnimation(animator);

                    setOnetimeValues = false;
                }
                
                transform.Translate(transform.forward * speed * deltaTime, Space.World);
            }
            else if(humanState == HumanState.MovedAllFoodToPackage)
            {
                ResetProperties();
            }

            yield return new WaitForSeconds(0.02f);
        }
    }

    Task FindTask()
    {
        bool isOtherServingFood = false;
        bool isOtherPackagingFood = false;
        bool isOtherDeliverTakeawayFood = false;
        bool isOtherPickingTrash = false;

        for (int i = 0; i < npcManager.npcs.Length; i++)
        {
            if(npcManager.npcControllers[i].task == Task.ServingFood)
            {
                isOtherServingFood = true;
            }
            if (npcManager.npcControllers[i].task == Task.PackagingFood)
            {
                isOtherPackagingFood = true;
            }
            if (npcManager.npcControllers[i].task == Task.DeliverTakeawayFood)
            {
                isOtherDeliverTakeawayFood = true;
            }
/*            if (npcManager.npcControllers[i].task == Task.P)
            {

            }*/
        }

        if(!isOtherPackagingFood || !isOtherDeliverTakeawayFood)
        {
            for (int i = 0; i < simulator.packages.Length; i++)
            {
                if (!isOtherPackagingFood && simulator.packageStates[i] == PackageState.Filling)
                {
                    return Task.PackagingFood;
                }
                if (!isOtherDeliverTakeawayFood && simulator.packageStates[i] == PackageState.Filled)
                {
                    return Task.DeliverTakeawayFood;
                }
            }
        }

        if(!isOtherServingFood)
        {
            for (int i = 0; i < simulator.foods.Length; i++)
            {
                if (simulator.foodStates[i] == FoodState.Wait)
                {
                    return Task.ServingFood;
                }
            }
        }

        return Task.Nothing;
    }

    public void ResetProperties()
    {
        util.SetIdleAnimation(animator);

        numFoodHold = 0;
        setOnetimeValues = true;
        humanState = HumanState.Ready;
    }
}
