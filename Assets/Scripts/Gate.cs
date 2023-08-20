using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject doorLeft;
    public GameObject doorRight;

    private bool isClose = false;

    public float deltaAngle;

    private void OnTriggerExit(Collider other)
    {
        isClose = true;
        StartCoroutine(OpenDoor(false));
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(OpenDoor(true));
    }

    IEnumerator OpenDoor(bool isOpen)
    {
        if(doorLeft.activeInHierarchy)
        {
            if (isOpen)
            {
                while (doorLeft.transform.eulerAngles.y < 90 && !isClose)
                {
                    doorLeft.transform.eulerAngles += new Vector3(0, deltaAngle, 0);
                    doorRight.transform.eulerAngles -= new Vector3(0, deltaAngle, 0);

                    yield return new WaitForSeconds(0.02f);
                }

                if(!isClose)
                {
                    doorLeft.transform.eulerAngles = new Vector3(0, 90, 0);
                    doorRight.transform.eulerAngles = new Vector3(0, 90, 0);
                }
            }
            else
            {          
                while (doorLeft.transform.eulerAngles.y > 0 && doorLeft.transform.eulerAngles.y < 180)
                {
                    doorLeft.transform.eulerAngles -= new Vector3(0, deltaAngle, 0);
                    doorRight.transform.eulerAngles += new Vector3(0, deltaAngle, 0);

                    yield return new WaitForSeconds(0.02f);
                }

                doorLeft.transform.eulerAngles = new Vector3(0, 0, 0);
                doorRight.transform.eulerAngles = new Vector3(0, 180, 0);

                isClose = false;
            }
        }
    }
}
