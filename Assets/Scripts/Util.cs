using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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

        Vector3 prevDistance = Vector3.positiveInfinity;

        while (true)
        {
            Vector3 distance;

            distance.x = Mathf.Abs(tf.position.x - destination.x);
            distance.y = Mathf.Abs(tf.position.y - destination.y);
            distance.z = Mathf.Abs(tf.position.z - destination.z);

            bool condition0 = true;
            bool condition1 = true;
            bool condition2 = true;

            if (distance.x > 0 && distance.x < prevDistance.x)
            {
                tf.Translate(new Vector3(
                    tf.forward.x * speed * deltaTime,
                    0,
                    0
                ), Space.World);
            }
            else
            {
                condition0 = false;
            }

            if (distance.y > 0 && distance.y < prevDistance.y)
            {
                tf.Translate(new Vector3(
                    0,
                    tf.forward.y * speed * deltaTime,
                    0
                ), Space.World);
            }
            else
            {
                condition1 = false;
            }

            if (distance.z > 0 && distance.z < prevDistance.z)
            {
                tf.Translate(new Vector3(
                    0,
                    0,
                    tf.forward.z * speed * deltaTime
                ), Space.World);
            }
            else
            {
                condition2 = false;
            }

            if (!condition0 && !condition1 && !condition2)
            {
                break;
            }

            tf.Translate(tf.forward * speed * deltaTime, Space.World);

            prevDistance = distance;

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
        animator.SetBool("isSitting", false);
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

    public void SetSittingAnimation(Animator animator)
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isHoldingFood", false);
        animator.SetBool("isHoldingFoodStanding", false);
        animator.SetBool("isSitting", true);
    }

    public void SetStandingUpAnimation(Animator animator)
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isHoldingFood", false);
        animator.SetBool("isHoldingFoodStanding", false);
        animator.SetBool("isSitting", false);
    }

    public void preventOnTriggerTwice(Transform tf, Vector3 objectCenter)
    {
        tf.transform.Translate(tf.forward * 1, Space.World);
    }

    public IEnumerator ScaleEffect(Transform tf, bool isScaleUp, Vector3 expectedScale)
    {
        while (true)
        {
            if (isScaleUp && tf.localScale.x - expectedScale.x > 0)
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

    public IEnumerator ScaleUpDownEffect(Transform tf, float scalePercent)
    {
        int phase = 1;
        Vector3 initialScale = tf.localScale;
        Vector3 deltaScale = initialScale / 5;

        while (phase < 4)
        {
            if (phase % 2 == 0)
            {
                if (tf.localScale.x < initialScale.x * (1 + scalePercent / phase))
                {
                    tf.localScale += deltaScale;
                }
                else
                {
                    phase++;
                }
            }
            else
            {
                if (tf.localScale.x > initialScale.x / (1 + scalePercent / phase))
                {
                    tf.localScale -= deltaScale;
                }
                else
                {
                    phase++;
                }
            }

            yield return new WaitForSeconds(0.02f);
        }

        tf.localScale = initialScale;
    }

    public void SetTMPTextOnBackground(TMP_Text tmp, RectTransform backgroundRT, string text)
    {
        tmp.text = text;
        backgroundRT.sizeDelta = 1.1f * new Vector2(tmp.preferredWidth, tmp.preferredHeight);
    }

    public string ToShortFormNumber(int number)
    {
        if (number < 1000)
        {
            return number.ToString();
        }
        else if (number >= 1000 && number < 1000000)
        {
            return Math.Round(number / 1000f, 2) + "K";
        }
        else
        {
            return Math.Round(number / 1000f, 2) + "M";
        }
    }
}
