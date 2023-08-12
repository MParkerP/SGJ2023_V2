using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopStartController : MonoBehaviour
{
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;

    public bool isLeftButtonPressed = false;
    public bool isRightButtonPressed = false;

    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private GameObject laser;
    [SerializeField] private bool isStarted = false;



    IEnumerator StartCoopLevel()
    {
        isStarted= true;
        laser.SetActive(true);
        GetComponent<AudioSource>().Play();
        OpenGate(leftDoor, "left");
        OpenGate(rightDoor, "right");
        yield return new WaitForSeconds(2);
        leftDoor.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        rightDoor.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void StartLevel()
    {
        if (!isStarted) { StartCoroutine(StartCoopLevel()); }
        
    }

    private void Update()
    {
        if (leftButton != null && rightButton != null)
        {
            if (leftButton.GetComponent<CoopStartButton>().isPressed)
            {
                isLeftButtonPressed = true;
            }
            else
            {
                isLeftButtonPressed = false;
            }

            if (rightButton.GetComponent<CoopStartButton>().isPressed)
            {
                isRightButtonPressed = true;
            }
            else
            {
                isRightButtonPressed = false;
            }

            if (isLeftButtonPressed && isRightButtonPressed)
            {
                StartLevel();
            }
        }
    }

    private void OpenGate(GameObject gate, string side)
    {
        var openJointLimit = gate.GetComponent<HingeJoint2D>().limits;
        if (side == "left") { openJointLimit.max = 50; }
        if (side == "right") { openJointLimit.max = -50; }
        gate.GetComponent<HingeJoint2D>().limits = openJointLimit;
    }
}
