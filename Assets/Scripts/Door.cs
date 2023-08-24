using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject player;

    private bool isOpening = false;
    private bool isClosing = false;

    private bool isStopCheckingCollision = false;

    public float deltaAngle;

    private void Start()
    {
        StartCoroutine(CheckColision());
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (!isStopCheckingCollision)
        {
            StartCoroutine(OpenDoor(true));
            StartCoroutine(StopCheckingCollision());
        }
    }*/

    /*private void OnTriggerExit(Collider other)
    {
        if (!isStopCheckingCollision)
        {
            Debug.Log("exit");
            StartCoroutine(OpenDoor(false));
        }
    }*/
    
    IEnumerator OpenDoor(bool isOpen)
    {
        float doorAngle = transform.localEulerAngles.y;
        if (doorAngle > 180)
        {
            doorAngle -= 360;
        }

        if (isOpen && !isOpening && !isClosing)
        {
            isOpening = true;

            while (doorAngle > -90 && !isClosing)
            {
                transform.localEulerAngles -= new Vector3(0, deltaAngle, 0);

                doorAngle = transform.localEulerAngles.y;
                if (doorAngle > 180)
                {
                    doorAngle -= 360;
                }

                yield return new WaitForSeconds(0.02f);
            }

            if (!isClosing)
            {
                transform.localEulerAngles = new Vector3(0, -90, 0);
            }

            isOpening = false;
        }

        if (!isOpen && !isClosing)
        {
            isClosing = true;

            yield return new WaitForSeconds(0.2f);

            while (doorAngle < 0)
            {
                transform.localEulerAngles += new Vector3(0, deltaAngle, 0);

                doorAngle = transform.localEulerAngles.y;
                if (doorAngle > 180)
                {
                    doorAngle -= 360;
                }

                yield return new WaitForSeconds(0.02f);
            }

            transform.localEulerAngles = new Vector3(0, 0, 0);

            isClosing = false;
        }
    }

    IEnumerator StopCheckingCollision()
    {
        Debug.Log("stop");
        isStopCheckingCollision = true;

        yield return new WaitForSeconds(0.05f);

        isStopCheckingCollision = false;
    }

    IEnumerator CheckColision()
    {
        while(true)
        {
            if(Mathf.Abs(player.transform.position.x - transform.position.x) < 40 
                && Mathf.Abs(player.transform.position.z - transform.position.z) < 40)
            {
                StartCoroutine(OpenDoor(true));
            }

            if (Mathf.Abs(player.transform.position.x - transform.position.x) > 41
                || Mathf.Abs(player.transform.position.z - transform.position.z) > 41)
            {
                StartCoroutine(OpenDoor(false));
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
