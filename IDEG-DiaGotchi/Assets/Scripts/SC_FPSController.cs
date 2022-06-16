using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]

public class SC_FPSController : MonoBehaviour
{
    public static SC_FPSController Current;

    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    [SerializeField]
    private float rayLength = 5.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private bool InteractPressed = false;
    private bool ControllerDisplayPressed = false;
    private bool IsControllerOpened = false;

    private bool PumpDisplayPressed = false;
    private bool IsPumpOpened = false;

    public Text RayCastText;
    public Text RayCastInteractText;
    public Text TalkText;

    public GameObject ControllerDisplayPanel;
    public GameObject PumpDisplayPanel;



    private class TalkRecord
    {
        public string text;
        public float durationTimer;
        public DataLoader.TalkAction actionBefore = DataLoader.TalkAction.None;
        public int actionParam = 0;
    }

    private List<TalkRecord> Talks = new List<TalkRecord>();

    void Start()
    {
        Current = this;

        characterController = GetComponent<CharacterController>();
        TalkText.text = "";

        // will unfreeze after talks
        Freeze();

        Talk(Strings.Get(7));
        Talk(Strings.Get(8));
        Talk(Strings.Get(9), DataLoader.TalkAction.StartNextQuest);
        Talk(Strings.Get(10), DataLoader.TalkAction.Unfreeze);
    }

    public void Freeze()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        canMove = false;
    }

    public void Unfreeze()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        canMove = true;
    }

    public void Talk(string text, DataLoader.TalkAction action = DataLoader.TalkAction.None, int actionParam = 0, float talkTime = -1.0f)
    {
        if (talkTime < 0.5f)
            talkTime = 2.5f + Mathf.Max(text.Length - 30.0f, 0.0f) * 0.08f; // 0.08 second for every character over 30

        Talks.Add(new TalkRecord
        {
            text = text,
            actionBefore = action,
            durationTimer = talkTime,
            actionParam = actionParam
        });
    }

    void PerformMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            moveDirection.y = jumpSpeed;
        else
            moveDirection.y = movementDirectionY;

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void PerformRaycast()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        int layerMask = 0xFF - (1 << 2) - (1 << 6);

        var rayHit = Physics.Raycast(ray, out hit, rayLength, layerMask);
        if (rayHit)
        {
            var namedobj = hit.transform.GetComponent<NamedObjectScript>();
            if (namedobj != null && namedobj.ObjectDescription.Length > 0)
            {
                RayCastText.text = namedobj.ObjectDescription;
                if (namedobj.ObjectInteractDescription.Length > 0)
                    RayCastInteractText.text = "[E] " + namedobj.ObjectInteractDescription;
                else
                    RayCastInteractText.text = "";
            }
            else
            {
                RayCastText.text = "";
                RayCastInteractText.text = "";
            }
        }
        else
        {
            RayCastText.text = "";
            RayCastInteractText.text = "";
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!InteractPressed)
            {
                InteractPressed = true;

                if (rayHit)
                {
                    Transform objectHit = hit.transform;

                    //Debug.Log("Ray hit: " + objectHit.name + ", " + hit.distance);

                    var cmp = objectHit.GetComponent<TogglerScript>();
                    if (cmp != null)
                        cmp.ToggleState();

                    var interactive = objectHit.GetComponent<InteractiveObject>();
                    if (interactive != null)
                        interactive.Interact();

                }
            }
        }
        else if (InteractPressed)
            InteractPressed = false;
    }

    void PerformControl()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!ControllerDisplayPressed)
            {
                ControllerDisplayPressed = true;

                if (IsPumpOpened)
                {
                    PumpDisplayPanel.GetComponent<Animator>().Play("PumpPanelDisappear");
                    IsPumpOpened = !IsPumpOpened;
                    Unfreeze();
                }

                if (IsControllerOpened)
                {
                    ControllerDisplayPanel.GetComponent<Animator>().Play("ControllerAnimationDisappear");
                }
                else
                {
                    ControllerDisplayPanel.GetComponent<Animator>().Play("ControllerAnimation");
                }

                IsControllerOpened = !IsControllerOpened;
            }
        }
        else if (ControllerDisplayPressed)
            ControllerDisplayPressed = false;

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!PumpDisplayPressed)
            {
                PumpDisplayPressed = true;

                if (IsControllerOpened)
                {
                    ControllerDisplayPanel.GetComponent<Animator>().Play("ControllerAnimationDisappear");
                    IsControllerOpened = !IsControllerOpened;
                }

                if (IsPumpOpened)
                {
                    PumpDisplayPanel.GetComponent<Animator>().Play("PumpPanelDisappear");
                    Unfreeze();
                }
                else
                {
                    PumpDisplayPanel.GetComponent<Animator>().Play("PumpPanelAppear");
                    Freeze();
                }

                IsPumpOpened = !IsPumpOpened;
            }
        }
        else if (PumpDisplayPressed)
            PumpDisplayPressed = false;
    }

    void PerformTalk()
    {
        if (Talks.Count == 0)
            return;

        TalkRecord cur = Talks[0];

        if (TalkText.text != cur.text)
            TalkText.text = cur.text;

        cur.durationTimer -= Time.deltaTime;
        if (cur.durationTimer <= 0.0f)
        {
            Talks.RemoveAt(0);

            if (Talks.Count == 0)
            {
                TalkText.text = "";
            }
            else
            {
                cur = Talks[0];
                TalkText.text = cur.text;

                if (cur.actionBefore != DataLoader.TalkAction.None)
                {
                    switch (cur.actionBefore)
                    {
                        case DataLoader.TalkAction.Freeze:
                            Freeze();
                            break;
                        case DataLoader.TalkAction.Unfreeze:
                            Unfreeze();
                            break;
                        case DataLoader.TalkAction.StartNextQuest:
                            QuestController.Current.StartNextQuest(); // if no quest currently pushed, the first one will be selected
                            break;
                    }
                }
            }
        }
    }

    void Update()
    {
        PerformMovement();
        PerformRaycast();
        PerformControl();
        PerformTalk();
    }

    //////////
    ///// Persistent state part
    //////////

    public bool ToiletUseFlag = false;

}
