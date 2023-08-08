using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanState
{
    Ready,
    Moving,
    PickingFood,
    HoldingFood
}

public class HumanController : MonoBehaviour
{
    private GameObject foodStorage;
    private GameObject counter;
    public float speed;

    public Animator animator;
    public HumanState humanState = HumanState.Moving;
    public bool setOnetimeValues = true;
    public string id;
    public int capacity = 2;
    public int numFoodHold = 0;

    private float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        foodStorage = GameObject.Find("Food Storage");
        counter = GameObject.Find("Counter");
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
            /* if (humanState == HumanState.Ready)
             {
                 transform.LookAt(
                     new Vector3(
                         foodStorage.transform.position.x,
                     transform.position.y,
                     foodStorage.transform.position.z)
                     );

                 animator.SetBool("isMoving", true);

                 humanState = HumanState.Moving;

                 yield return new WaitForSeconds(0.02f);
             }*/
            if (humanState == HumanState.Moving)
            {
                if (setOnetimeValues)
                {
                    transform.LookAt(
                                new Vector3(
                                    foodStorage.transform.position.x,
                                transform.position.y,
                                foodStorage.transform.position.z)
                                );

                    animator.SetBool("isMoving", true);
                }
                else
                {
                    setOnetimeValues = false;
                }

                transform.Translate(transform.forward * speed * deltaTime, Space.World);

                yield return new WaitForSeconds(0.02f);
            }
            else if (humanState == HumanState.HoldingFood)
            {
                if (setOnetimeValues)
                {
                    transform.LookAt(
                    new Vector3(
                        counter.transform.position.x,
                    transform.position.y,
                    counter.transform.position.z)
                    );

                    animator.SetBool("isHoldingFoodStanding", false);

                    setOnetimeValues = false;
                }

                transform.Translate(transform.forward * speed * deltaTime, Space.World);

                yield return new WaitForSeconds(0.02f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
