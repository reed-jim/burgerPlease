using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanState
{
    Ready,
    Moving,
    PickingFood,
    HoldingFood,
    PackagingFood,
}

public enum Task
{
    Nothing,
    ServingFood,
    PackagingFood,
    DeliverTakeawayFood
}

public class HumanController : MonoBehaviour
{
    private GameObject foodStorage;
    private GameObject counter;
    private Simulator simulator;
    private Util util;
    public float speed;

    public Animator animator;
    public HumanState humanState = HumanState.Ready;
    public Task task;
    public bool setOnetimeValues = true;
    public string id;
    public int capacity = 2;
    public int numFoodHold = 0;
    private Vector3 movingDestination;

    private float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        foodStorage = GameObject.Find("Food Storage");
        counter = GameObject.Find("Counter");
        simulator = GameObject.Find("Simulator").GetComponent<Simulator>();
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
                task = FindTask();

                if(task == Task.ServingFood ||
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
                            );

                            break;
                        }
                    }

                    humanState = HumanState.Moving;
                }

/*

                if (FindTask() == Task.ServingFood)
                {
                    movingDestination = new Vector3(
                        counter.transform.position.x,
                        transform.position.y,
                        counter.transform.position.z
                    );

                    humanState = HumanState.Moving;
                }
                else if (FindTask() == Task.PackagingFood)
                {
                    movingDestination = new Vector3(
                        simulator.packageTable.transform.position.x,
                        transform.position.y,
                        simulator.packageTable.transform.position.z
                    );

                    humanState = HumanState.Moving;
                }
                else if (FindTask() == Task.DeliverTakeawayFood)
                {
                    for (int i = 0; i < simulator.packages.Length; i++)
                    {
                        if(simulator.packageStates[i] == PackageState.Filled)
                        {
                            movingDestination = new Vector3(
                                simulator.packages[i].transform.position.x,
                                transform.position.y,
                                simulator.packages[i].transform.position.z
                            );

                            break;
                        }
                    }

                    humanState = HumanState.Moving;
                }*/

                yield return new WaitForSeconds(0.2f);
            }
            else if (humanState == HumanState.Moving)
            {
                if (setOnetimeValues)
                {                  
                    transform.LookAt(movingDestination);

                    animator.SetBool("isMoving", true);
                }
                else
                {
                    setOnetimeValues = false;
                }

                transform.Translate(transform.forward * speed * deltaTime, Space.World);
            }
            else if (humanState == HumanState.HoldingFood)
            {
                if (setOnetimeValues)
                {
                    if (task == Task.ServingFood)
                    {
                        movingDestination = new Vector3(
                            counter.transform.position.x,
                            transform.position.y,
                            counter.transform.position.z
                        );
                    }
                    else if (task == Task.PackagingFood)
                    {
                        movingDestination = new Vector3(
                            simulator.packageTable.transform.position.x,
                            transform.position.y,
                            simulator.packageTable.transform.position.z
                        );
                    }
                    else if(task == Task.DeliverTakeawayFood)
                    {
                        movingDestination = new Vector3(
                            simulator.takeawayCounter.transform.position.x,
                            transform.position.y,
                            simulator.takeawayCounter.transform.position.z
                        );
                    }

                    transform.LookAt(movingDestination);
                    
                    animator.SetBool("isHoldingFoodStanding", false);

                    setOnetimeValues = false;
                }
                
                transform.Translate(transform.forward * speed * deltaTime, Space.World);
            }
            else if(humanState == HumanState.PackagingFood)
            {
                yield return new WaitForSeconds(2f);

                ResetProperties();
            }

            yield return new WaitForSeconds(0.02f);
        }
    }

    Task FindTask()
    {
        for (int i = 0; i < simulator.packages.Length; i++)
        {
            if (simulator.packageStates[i] == PackageState.Filling)
            {
                return Task.PackagingFood;
            }
            if (simulator.packageStates[i] == PackageState.Filled)
            {
                return Task.DeliverTakeawayFood;
            }
        }

        /*for (int i = 0; i < simulator.foods.Length; i++)
        {
            if (simulator.foodStates[i] == FoodState.Wait)
            {
                return Task.ServingFood;
            }
        }*/

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
