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
    HoldingTrashStanding,
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

    public RectTransform joystickOuter;
    public RectTransform joystickInner;

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

            /*            Move();*/
            if(prevIsMove != isMove)
            {
                StartCoroutine(MoveJoystick());
            }
            
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
                    || playerState == PlayerState.HoldingTrashStanding
                    || playerState == PlayerState.HoldingFoodMoving
                    || playerState == PlayerState.HoldingTrashMoving
                    || playerState == PlayerState.HoldingPackageMoving
                    || playerState == PlayerState.PackagingFood
                    || playerState == PlayerState.PickingPackage
                    || playerState == PlayerState.PuttingFood)
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

    public IEnumerator MoveJoystick()
    {
        bool isFirstTouch = true;

        while(Input.GetMouseButton(0))
        {
            // (0,0) of mouse is at bottom-left
            Vector2 mousePostion = new Vector2(
                Input.mousePosition.x - Screen.currentResolution.width / 2,
                Input.mousePosition.y - Screen.currentResolution.height / 2
            );

            if(isFirstTouch)
            {
                joystickOuter.localPosition = mousePostion;
                joystickInner.localPosition = joystickOuter.localPosition;
                joystickOuter.gameObject.SetActive(true);

                isFirstTouch = false;
            }
            else
            {
                Vector2 direction = 1f * new Vector2(mousePostion.x - joystickOuter.localPosition.x,
                    mousePostion.y - joystickOuter.localPosition.y);

                if (direction.x > 100) direction.x = 100;
                if (direction.y > 100) direction.y = 100;
                if (direction.x < -100) direction.x = -100;
                if (direction.y < -100) direction.y = -100;

                joystickInner.anchoredPosition = new Vector3(
                    direction.x,
                    direction.y,
                    joystickInner.localPosition.z
                );

                Vector3 point1 = mainCamera.ScreenToWorldPoint(new Vector3(
                    joystickInner.anchoredPosition.x,
                    joystickInner.anchoredPosition.y,
                    0
                ));

                Vector3 point2 = mainCamera.ScreenToWorldPoint(Vector3.zero);

                Vector3 playerDirection = new Vector3(point1.x - point2.x, 0, point1.z - point2.z);

                if(playerDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(playerDirection);
                }

                playerRigidbody.velocity = transform.forward * speed;
            }

            yield return new WaitForSeconds(0.002f);
        }

        joystickOuter.gameObject.SetActive(false);
    }
    /*public void MoveJoystick()
    {
        // (0,0) of mouse is at bottom-left
        Vector2 mousePostion = new Vector2(
            Input.mousePosition.x - Screen.currentResolution.width / 2,
            Input.mousePosition.y - Screen.currentResolution.height / 2
        );

        Vector2 direction = 1f * new Vector2(mousePostion.x - joystickOuter.localPosition.x,
            mousePostion.y - joystickOuter.localPosition.y);

       *//* if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if(direction.x > 0)
            {
                direction.x = 100;
            }
            if (direction.x < 0)
            {
                direction.x = -100;
            }
        }
        else
        {
            if (direction.y > 0)
            {
                direction.y = 100;
            }
            if (direction.y < 0)
            {
                direction.y = -100;
            }
        }*//*

        if (direction.x > 100) direction.x = 100;
        if (direction.y > 100) direction.y = 100;
        if (direction.x < -100) direction.x = -100;
        if (direction.y < -100) direction.y = -100;

        joystickInner.anchoredPosition = new Vector3(
            direction.x,
            direction.y,
            joystickInner.localPosition.z
        );

        Vector3 point1 = mainCamera.ScreenToWorldPoint(new Vector3(
            joystickInner.anchoredPosition.x,
            joystickInner.anchoredPosition.y,
            0
        ));
        Vector3 point2 = mainCamera.ScreenToWorldPoint(Vector3.zero);

        Vector3 playerDirection = new Vector3(point1.x - point2.x, 0, point1.z - point2.z);

        transform.rotation = Quaternion.LookRotation(playerDirection);

        playerRigidbody.velocity = transform.forward * speed;
    }*/
}
