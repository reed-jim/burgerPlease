using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public Simulator simulator;
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // Finish delivering foods, give them to customers
        if (simulator.customerStates[simulator.firstQueueCustomerIndex] == CustomerState.Waiting)
        {
            
        }


        if (other.CompareTag("Player"))
        {
            if(playerController.playerState == PlayerState.HoldingFoodMoving)
            {
                StartCoroutine(MoveFoodOneByOne());

                playerController.playerState = PlayerState.PuttingFood;
            }
        }
        else if (other.CompareTag("NPC"))
        {
            HumanController controller = other.GetComponent<HumanController>();

            controller.animator.SetBool("isHoldingFood", false);

            int numFoodBelongToCounter = 0;

            for (int i = 0; i < simulator.foods.Length; i++)
            {
                if (simulator.foodBelongTo[i] == "counter")
                {
                    numFoodBelongToCounter++;
                }
            }

            for (int i = 0; i < simulator.foods.Length; i++)
            {
                if (simulator.foodBelongTo[i] == controller.id)
                {
                    numFoodBelongToCounter++;
                    simulator.foodBelongTo[i] = "counter";
                    simulator.foodColumnIndex[i] = numFoodBelongToCounter - 1;
                    simulator.foodStates[i] = FoodState.ReachCounter;
                }
            }

            controller.ResetProperties();
        }

        IEnumerator MoveFoodOneByOne()
        {
            int capacity = playerController.capacity;
            int numFoodBelongToCounter = 0;
            int numFoodMoved = 0;

            for (int i = 0; i < simulator.foods.Length; i++)
            {
                if (simulator.foodBelongTo[i] == "counter")
                {
                    numFoodBelongToCounter++;
                }
            }

            for (int i = 0; i < simulator.foods.Length; i++)
            {
                if(numFoodMoved < capacity)
                {
                    if (simulator.foodBelongTo[i] == "player")
                    {
                        numFoodBelongToCounter++;
                        simulator.foodBelongTo[i] = "counter";
                        simulator.foodColumnIndex[i] = numFoodBelongToCounter - 1;


                        int foodIndex = i;

                        StartCoroutine(
                            simulator.CurveMove(
                                simulator.foods[i].transform,
                                simulator.foods[i].transform.position,
                                simulator.counter.transform.position +
                                new Vector3(6,
                                simulator.counterSize.y / 2 +
                                simulator.foodColumnIndex[i] * simulator.foodSize.y,
                                0),
                                12,
                                0,
                                () =>
                                {
                                    if (playerController.numberFoodHold > 0)
                                    {
                                        playerController.numberFoodHold--;
                                    }
                                    if (playerController.numberFoodHold == 0)
                                    {
                                        playerController.playerState = PlayerState.Ready;
                                    }

                                    simulator.foodStates[foodIndex] = FoodState.CustomerPick;
                                }
                            )
                        );

                        numFoodMoved++;

                        simulator.foodStates[i] = FoodState.PuttingInCounter;

                        yield return new WaitForSeconds(0.2f);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
