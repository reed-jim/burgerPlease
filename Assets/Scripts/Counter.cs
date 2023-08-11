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
            playerController.gameObject.GetComponent<Animator>().SetBool("isHoldingFood", false);

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
                if (simulator.foodBelongTo[i] == "player")
                {
                    numFoodBelongToCounter++;
                    simulator.foodBelongTo[i] = "counter";
                    simulator.foodColumnIndex[i] = numFoodBelongToCounter - 1;
                    simulator.foodStates[i] = FoodState.ReachCounter;
                }
            }

            playerController.playerState = PlayerState.Ready;
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
    }
}
