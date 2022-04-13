using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]

public class SC_FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    [SerializeField]
    private float rayLength = 0.2f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private bool InteractPressed = false;

    public Text RayCastText;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Unfreeze();
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

        var rayHit = Physics.Raycast(ray, out hit, 5.0f);
        if (rayHit)
        {
            var namedobj = hit.transform.GetComponent<NamedObjectScript>();
            if (namedobj != null && namedobj.ObjectDescription.Length > 0)
                RayCastText.text = namedobj.ObjectDescription;
            else
                RayCastText.text = "";
        }
        else
            RayCastText.text = "";

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

    void Update()
    {
        PerformMovement();
        PerformRaycast();
    }
}
