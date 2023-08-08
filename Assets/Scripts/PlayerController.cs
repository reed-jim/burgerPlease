using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState {
    Ready,
    PickingFood,
    HoldingFood
}

public class PlayerController : MonoBehaviour
{
    public Simulator simulator;
    public Animator playerAnimator;
    public float speed = 5;
    public float rotateSpeed = 5;

    private Camera mainCamera;
    private float deltaTime;

    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    float rayLength;

    private bool isSetAnimation = false;
    public PlayerState playerState = PlayerState.Ready;
    public int numberFoodHold = 0;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        deltaTime = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        if (Input.GetMouseButton(0) && playerState != PlayerState.PickingFood)
        {
            Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            Vector3 pointToLook = Vector3.zero;

            if (groundPlane.Raycast(cameraRay, out rayLength))
            {
                pointToLook = cameraRay.GetPoint(rayLength);

                transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
            }
            
            transform.LookAt(new Vector3(
                pointToLook.x, transform.position.y, pointToLook.z
                ));

            transform.Translate(transform.forward * speed * deltaTime, Space.World);

            if(!isSetAnimation)
            {
                if(playerState == PlayerState.Ready)
                {
                    playerAnimator.SetBool("isMoving", true);
                }
                else if(playerState == PlayerState.HoldingFood)
                {
                    playerAnimator.SetBool("isHoldingFoodStanding", false);
                }
                isSetAnimation = true;
            }
        }
        else
        {
            if (playerState == PlayerState.Ready)
            {
                playerAnimator.SetBool("isMoving", false);
            }
            else if (playerState == PlayerState.HoldingFood)
            {
                playerAnimator.SetBool("isHoldingFoodStanding", true);
            }
            isSetAnimation = false;
        }
    }
}