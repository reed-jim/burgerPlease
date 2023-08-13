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
    private Util util;
    public PlayerController playerController;
    public float speed = 5f;
    public Vector3 minDistance;

    private float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        util = GameObject.Find("Util").GetComponent<Util>();

        deltaTime = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (pickState == PickState.Picking)
        {
            Transform foodTransform = simulator.foods[0].transform;

            if (Mathf.Abs(player.transform.position.x - foodTransform.position.x) > minDistance.x)
            {
                Vector3 direction = new Vector3(
                    player.transform.position.x - foodTransform.position.x,
                    player.transform.position.y - foodTransform.position.y,
                    player.transform.position.z - foodTransform.position.z
                );

                foodTransform.Translate(direction * speed * deltaTime);
            }
            else
            {
                pickState = PickState.Picked;
                player.GetComponent<Animator>().SetBool("isHoldingFood", true);
            }
        }
        if (pickState == PickState.Picked)
        {
            simulator.foods[0].transform.position =
                new Vector3(player.transform.position.x,
                player.transform.position.y + 5,
                player.transform.position.z) +
                player.transform.forward * 2;

            *//*simulator.foods[0].transform.position = new Vector3(
                player.transform.position.x + 2,
                player.transform.position.y + 6,
                player.transform.position.z + 2
            );*//*
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        util.preventOnTriggerTwice(other.transform, simulator.foodStorage.transform.position);

        Debug.Log("burger stove collide with human");
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PickupFoodOneByOneByPlayer());
        }

        if (other.gameObject.CompareTag("NPC"))
        {
            HumanController controller = other.GetComponent<HumanController>();

            StartCoroutine(PickupFoodOneByOne(controller));
        }
    }

    IEnumerator PickupFoodOneByOne(HumanController controller)
    {
        controller.animator.SetBool("isMoving", false);

        int capacity = controller.capacity;
        int numFoodTaken = controller.numFoodHold;

        for (int i = 0; i < simulator.foods.Length; i++)
        {
            if (simulator.foodStates[i] == FoodState.Wait)
            {
                int foodIndex = i;

                if (numFoodTaken == 0)
                {
                    controller.humanState = HumanState.PickingFood;
                }

                if (numFoodTaken < capacity)
                {
                    numFoodTaken++;

                    simulator.foodStates[i] = FoodState.Picking;
                    simulator.foodBelongTo[i] = controller.id;
                    simulator.foodColumnIndex[i] = numFoodTaken - 1;

                    Transform foodTransform = simulator.foods[i].transform;

                    StartCoroutine(
                        simulator.CurveMove
                        (
                           foodTransform,
                           foodTransform.position,
                           controller.gameObject.transform.position +
                           new Vector3(0,
                           7 + numFoodTaken * simulator.foodSize.y,
                           0) +
                           controller.gameObject.transform.forward * 2
                           ,
                           12,
                           0,
                           () =>
                           {
                               simulator.foodStates[foodIndex] = FoodState.Delivering;

                               controller.numFoodHold++;

                               if (controller.numFoodHold == 2)
                               {
                                   controller.animator.SetBool("isHoldingFood", true);
                                   controller.animator.SetBool("isHoldingFoodStanding", true);
                                   controller.setOnetimeValues = true;
                                   controller.humanState = HumanState.HoldingFood;
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
    }

    IEnumerator PickupFoodOneByOneByPlayer()
    {
        if (playerController.playerState == PlayerState.Ready)
        {
            int capacity = 2;
            int numFoodTaken = 0;

            for (int i = simulator.foods.Length - 1; i >= 0; i--)
            {
                if (simulator.foodStates[i] == FoodState.Wait)
                {
                    int foodIndex = i;

                    if (numFoodTaken == 0)
                    {
                        playerController.playerState = PlayerState.PickingFood;
                    }

                    if (numFoodTaken < capacity)
                    {
                        numFoodTaken++;
                        simulator.foodStates[i] = FoodState.Picking;
                        simulator.foodBelongTo[i] = "player";
                        simulator.foodColumnIndex[i] = numFoodTaken - 1;

                        Transform foodTransform = simulator.foods[i].transform;

                        StartCoroutine(
                            simulator.CurveMove
                            (
                               foodTransform,
                               foodTransform.position,
                               player.transform.position +
                               new Vector3(0,
                               7 + simulator.foodColumnIndex[i] * simulator.foodSize.y,
                               0) +
                               player.transform.forward * 2
                               ,
                               12,
                               0,
                               () =>
                               {
                                   simulator.foodStates[foodIndex] = FoodState.Delivering;

                                   playerController.numberFoodHold++;

                                   if(playerController.numberFoodHold == capacity)
                                   {
                                       playerController.isSetAnimation = true;
                                       playerController.playerState = PlayerState.HoldingFoodMoving;
                                   }
                                   else if (playerController.numberFoodHold == 1)
                                   {
                                       playerController.isSetAnimation = true;
                                       playerController.playerState = PlayerState.HoldingFood;
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
        }
    }
}
