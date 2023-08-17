using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public float deltaScale;
    private float deltaTime;

    public delegate void MoveToCallback();

    // Start is called before the first frame update
    void Start()
    {
        deltaTime = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator MoveTo(Transform tf, Vector3 destination, float speed, MoveToCallback callback)
    {
        tf.LookAt(destination);

        while (Vector3.Distance(tf.position, destination) > 10)
        {
            tf.Translate(tf.forward * speed * deltaTime, Space.World);

            yield return new WaitForSeconds(0.03f);
        }

        callback();
    }

    public IEnumerator RotateOverTime(Transform tf, Vector3 expectedAngle)
    {
        int angleVelocity = 5;

        int stepX = (int)(Mathf.Abs(expectedAngle.x - tf.eulerAngles.x) / angleVelocity);
        int stepY = (int)(Mathf.Abs(expectedAngle.y - tf.eulerAngles.y) / angleVelocity);
        int stepZ = (int)(Mathf.Abs(expectedAngle.z - tf.eulerAngles.z) / angleVelocity);
        int directionX = expectedAngle.x - tf.eulerAngles.x > 0 ? 1 : -1;
        int directionY = expectedAngle.y - tf.eulerAngles.y > 0 ? 1 : -1;
        int directionZ = expectedAngle.z - tf.eulerAngles.z > 0 ? 1 : -1;

        int step = 0;
        int maxStep = Mathf.Max(stepX, stepY);

        maxStep = Mathf.Max(maxStep, stepZ);

        while (step < maxStep)
        {
            if (step < stepX)
            {
                tf.Rotate(angleVelocity * directionX, 0, 0);
            }
            if (step < stepY)
            {
                tf.Rotate(0, angleVelocity * directionY, 0);
            }
            if (step < stepZ)
            {
                tf.Rotate(0, 0, angleVelocity * directionZ);
            }

            step++;

            yield return new WaitForSeconds(0.02f);
        }
    }


    public void SetIdleAnimation(Animator animator)
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isHoldingFood", false);
        animator.SetBool("isHoldingFoodStanding", false);
    }

    public void SetMovingAnimation(Animator animator)
    {
        animator.SetBool("isMoving", true);
        animator.SetBool("isHoldingFood", false);
        animator.SetBool("isHoldingFoodStanding", false);
    }

    public void SetHoldingFoodMovingAnimation(Animator animator)
    {
        animator.SetBool("isMoving", true);
        animator.SetBool("isHoldingFood", true);
        animator.SetBool("isHoldingFoodStanding", false);
    }

    public void SetHoldingFoodStandingAnimation(Animator animator)
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isHoldingFood", true);
        animator.SetBool("isHoldingFoodStanding", true);
    }

    public void preventOnTriggerTwice(Transform tf, Vector3 objectCenter)
    {
        tf.transform.Translate(tf.forward * 1, Space.World);
    }

    public IEnumerator ScaleEffect(Transform tf, bool isScaleUp, Vector3 expectedScale)
    {
        while (true)
        {
            if(isScaleUp && tf.localScale.x - expectedScale.x > 0)
            {
                break;
            }
            if (!isScaleUp && tf.localScale.x - expectedScale.x < 0)
            {
                break;
            }

            if (isScaleUp)
            {
                tf.localScale += new Vector3(deltaScale, 0, deltaScale);
            }
            else
            {
                tf.localScale -= new Vector3(deltaScale, 0, deltaScale);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}
