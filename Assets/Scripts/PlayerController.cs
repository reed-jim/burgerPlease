using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState {
    Ready,
    PickingFood,
    PuttingFood,
    HoldingFood,
    HoldingFoodMoving,
    HoldingTrash,
    HoldingTrashMoving,
    PackagingFood,
    PickingPackage,
    HoldingPackageMoving
}

public class PlayerController : MonoBehaviour
{
    public Simulator simulator;
    public Util util;
    public Animator playerAnimator;
    public float speed = 5;
    public float rotateSpeed = 5;

    private Camera mainCamera;
    private float deltaTime;

    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    float rayLength;

    private bool prevIsMove = false;
    public bool isSetAnimation = false;
    public PlayerState playerState;
    private PlayerState prevState;
    public int capacity = 2;
    public int numberFoodHold = 0;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        deltaTime = Time.deltaTime;

        playerState = PlayerState.Ready;
        prevState = playerState;
    }

    // Update is called once per frame
    void Update()
    {
        bool moveCondition = playerState == PlayerState.Ready
                || playerState == PlayerState.HoldingFoodMoving
                || playerState == PlayerState.HoldingPackageMoving
                || playerState == PlayerState.HoldingTrashMoving;

        bool isMove = Input.GetMouseButton(0) && moveCondition;

        if(playerState != prevState || isMove != prevIsMove)
        {
            isSetAnimation = true;
        }



        // Movement
        if (isMove)
        {
            if(isSetAnimation)
            {
                if (playerState == PlayerState.Ready)
                {
                    util.SetMovingAnimation(playerAnimator);
                }
                else if(playerState == PlayerState.HoldingFoodMoving
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
            if(isSetAnimation)
            {
                if (playerState == PlayerState.Ready)
                {
                    util.SetIdleAnimation(playerAnimator);
                }
                else if (playerState == PlayerState.PickingFood)
                {
                    util.SetIdleAnimation(playerAnimator);
                }
                else if (playerState == PlayerState.HoldingFood
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

            transform.Translate(transform.forward * speed * deltaTime, Space.World);
        }
    }
}
