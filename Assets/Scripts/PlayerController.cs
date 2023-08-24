using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum PlayerState
{
    Ready,
    PickingFood,
    PuttingFood,
    HoldingFoodStanding,
    HoldingFoodMoving,
    HoldingTrash,
    HoldingTrashMoving,
    PackagingFood,
    PickingPackage,
    HoldingPackageMoving
}

public class PlayerController : MonoBehaviour
{
    public TMP_Text maxCapacityTMP;

    public Simulator simulator;
    public Util util;
    public Animator playerAnimator;
    private Rigidbody playerRigidbody;

    private Camera mainCamera;
    private float deltaTime;

    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    float rayLength;

    private bool prevIsMove = false;
    public bool isSetAnimation = false;
    public PlayerState playerState;
    private PlayerState prevState;
    public int numberFoodHold = 0;

    public float speed = 5;
    public int capacity = 2;
    public float profitMultiplier;
    public float rotateSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        deltaTime = Time.deltaTime;

        playerRigidbody = GetComponent<Rigidbody>();

        playerState = PlayerState.Ready;
        prevState = playerState;

        StartCoroutine(ShowMaxCapacityText());
    }

    // Update is called once per frame
    void Update()
    {
        bool moveCondition = playerState == PlayerState.Ready
                || playerState == PlayerState.HoldingFoodMoving
                || playerState == PlayerState.HoldingPackageMoving
                || playerState == PlayerState.HoldingTrashMoving;

        bool isMove = Input.GetMouseButton(0) && moveCondition;

        if (playerState != prevState || isMove != prevIsMove)
        {
            isSetAnimation = true;
        }



        // Movement
        if (isMove)
        {
            if (isSetAnimation)
            {
                if (playerState == PlayerState.Ready)
                {
                    util.SetMovingAnimation(playerAnimator);
                }
                else if (playerState == PlayerState.HoldingFoodMoving
                    || playerState == PlayerState.HoldingPackageMoving
                    || playerState == PlayerState.HoldingTrashMoving)
                {
                    util.SetHoldingFoodMovingAnimation(playerAnimator);
                }

                isSetAnimation = false;
            }

            Move();

            prevIsMove = true;
        }
        else
        {
            playerRigidbody.velocity = Vector3.zero;

            if (isSetAnimation)
            {
                if (playerState == PlayerState.Ready
                    || playerState == PlayerState.PickingFood)
                {
                    util.SetIdleAnimation(playerAnimator);
                }
                else if (playerState == PlayerState.HoldingFoodStanding
                    || playerState == PlayerState.HoldingTrash
                    || playerState == PlayerState.HoldingFoodMoving
                    || playerState == PlayerState.PackagingFood
                    || playerState == PlayerState.PickingPackage
                    || playerState == PlayerState.PuttingFood
                    || playerState == PlayerState.HoldingPackageMoving)
                {
                    util.SetHoldingFoodStandingAnimation(playerAnimator);
                }

                isSetAnimation = false;
            }

            prevIsMove = false;
        }

        prevState = playerState;
    }

    public void ResetProperties()
    {
        util.SetIdleAnimation(playerAnimator);

        numberFoodHold = 0;
        playerState = PlayerState.Ready;
    }

    void Move()
    {
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 pointToLook;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            pointToLook = cameraRay.GetPoint(rayLength);

            transform.LookAt(new Vector3(pointToLook.x, 0, pointToLook.z));

            playerRigidbody.velocity = transform.forward * speed;

            /*transform.Translate(transform.forward * speed * deltaTime, Space.World);*/
        }
    }

    public IEnumerator ShowMaxCapacityText()
    {
        while (true)
        {
            if (numberFoodHold == capacity)
            {
                if (!maxCapacityTMP.gameObject.activeInHierarchy)
                {
                    maxCapacityTMP.gameObject.SetActive(true);
                }

                maxCapacityTMP.gameObject.transform.position = transform.position + new Vector3(0, 18, 0);
            }
            else
            {
                maxCapacityTMP.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(0.000f);
        }
    }
}
