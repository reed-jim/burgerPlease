using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PickState
{
    None,
    Picking,
    Picked
}

public class BurgerPickup : MonoBehaviour
{
    public GameObject player;
    public Simulator simulator;
    public GameProgressManager gameProgressManager;
    private Util util;
    public PlayerController playerController;

    public float speed = 5f;

    public int stoveIndex;


    // Start is called before the first frame update
    void Start()
    {
        util = GameObject.Find("Util").GetComponent<Util>();
    }

    private void OnTriggerExit(Collider other)
    {
        simulator.cookingSound.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        util.preventOnTriggerTwice(other.transform, simulator.foodStorages[stoveIndex].transform.position);

        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PickupFoodOneByOneByPlayer());
        }

        if (other.gameObject.CompareTag("NPC"))
        {
            HumanController controller = other.GetComponent<HumanController>();

            StartCoroutine(PickupFoodOneByOne(controller));
        }

        simulator.cookingSound.Play();
    }

    IEnumerator PickupFoodOneByOne(HumanController controller)
    {
        int capacity = controller.capacity;
        int numFoodTaken = controller.numFoodHold;

        util.SetIdleAnimation(controller.animator);
        controller.humanState = HumanState.PickingFood;

        List<int> foodToPickIndexes = simulator.FindAndSortFood("storage" + stoveIndex, false);

        for (int i = 0; i < foodToPickIndexes.Count; i++)
        {
            if(i > capacity - 1)
            {
                break;
            }

            int foodIndex = foodToPickIndexes[i];

            if (simulator.foodStates[foodIndex] == FoodState.Wait)
            {
                if (numFoodTaken < capacity)
                {
                    numFoodTaken++;

                    simulator.foodStates[foodIndex] = FoodState.Picking;
                    simulator.foodBelongTo[foodIndex] = controller.id;
                    simulator.foodColumnIndex[foodIndex] = numFoodTaken - 1;

                    Transform foodTransform = simulator.foods[foodIndex].transform;

                    StartCoroutine(
                        simulator.CurveMove
                        (
                           foodTransform,
                           foodTransform.position,
                           controller.gameObject.transform.position +
                           new Vector3(0,
                           7 + numFoodTaken * simulator.foodSize.y,
                           0) +
                           controller.gameObject.transform.forward * 8
                           ,
                           12,
                           0,
                           () =>
                           {
                               simulator.foodStates[foodIndex] = FoodState.Delivering;

                               controller.numFoodHold++;
                               simulator.numFoodOfStorage[stoveIndex]--;
                               simulator.maxCapacityTMPs[stoveIndex].gameObject.SetActive(false);

                               if(controller.numFoodHold == 1)
                               {
                                   util.SetHoldingFoodStandingAnimation(controller.animator);
                               }
                               if (controller.numFoodHold == capacity)
                               {
                                   controller.humanState = HumanState.HoldingFoodMoving;
                               }
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

        if(numFoodTaken == 0)
        {
            yield return new WaitForSeconds(2f);

            StartCoroutine(PickupFoodOneByOne(controller));
        }
        if(numFoodTaken > 0 && numFoodTaken < capacity)
        {
            controller.humanState = HumanState.HoldingFoodMoving;
        }
    }

    IEnumerator PickupFoodOneByOneByPlayer()
    {
        if (playerController.playerState == PlayerState.Ready)
        {
            int capacity = playerController.capacity;
            int numFoodTaken = 0;

            List<int> foodToPickIndexes = simulator.FindAndSortFood("storage" + stoveIndex);

            for (int i = 0; i < foodToPickIndexes.Count; i++)
            {
                int foodIndex = foodToPickIndexes[i];
                bool isLast = i == foodToPickIndexes.Count - 1 ? true : false;

                if (simulator.foodStates[foodIndex] == FoodState.Wait)
                {
                    if (numFoodTaken == 0)
                    {
                        playerController.playerState = PlayerState.PickingFood;
                    }

                    if (numFoodTaken < capacity)
                    {
                        numFoodTaken++;
                        simulator.foodStates[foodIndex] = FoodState.Picking;
                        simulator.foodBelongTo[foodIndex] = "player";
                        simulator.foodColumnIndex[foodIndex] = numFoodTaken - 1;

                        Transform foodTransform = simulator.foods[foodIndex].transform;
                     
                        StartCoroutine(
                            simulator.CurveMove
                            (
                               foodTransform,
                               foodTransform.position,
                               player.transform.position +
                               new Vector3(0,
                               7 + simulator.foodColumnIndex[foodIndex] * simulator.foodSize.y,
                               0) +
                               player.transform.forward * 4
                               ,
                               12,
                               0,
                               () =>
                               {
                                   simulator.foodStates[foodIndex] = FoodState.Delivering;

                                   playerController.numberFoodHold++;
                                   simulator.numFoodOfStorage[stoveIndex]--;
                                   simulator.maxCapacityTMPs[stoveIndex].gameObject.SetActive(false);

                                   if(playerController.numberFoodHold == capacity)
                                   {
                                       if(gameProgressManager.progressStep == ProgressStep.PickupFoodTutorial)
                                       {
                                           gameProgressManager.progressStep = ProgressStep.PutFoodOnCounterTutorialStart;
                                       }

                                       playerController.playerState = PlayerState.HoldingFoodMoving;
                                   }
                                   else if (playerController.numberFoodHold == 1)
                                   {
                                       playerController.playerState = PlayerState.HoldingFoodStanding;
                                   }
                                   
                                   if(isLast && playerController.numberFoodHold < capacity)
                                   {
                                       playerController.playerState = PlayerState.HoldingFoodMoving;
                                   }
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

            if (numFoodTaken == 0)
            {
                playerController.playerState = PlayerState.Ready;
            }
        }
    }
}
