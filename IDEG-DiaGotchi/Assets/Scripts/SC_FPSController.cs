using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
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
    private float rayLength = 2.0f; // NOTE: this should be further adjusted for objects with larger interact distance

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;
    [HideInInspector]
    public bool canRotate = true;

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
    public GameObject MainCameraObject;

    [Header("Fake starting")]
    public bool PerformFakeStart = true;
    public int FakeStartQuestId = 4;
    public GameObject FakeStartGameObject = null;


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

        if (PerformFakeStart)
        {
            QuestController.Current.FakeStart(FakeStartQuestId);
            if (FakeStartGameObject != null)
                TeleportTo(FakeStartGameObject.transform.position, FakeStartGameObject.transform.rotation);

            Unfreeze();
        }
        else
        {
            // will unfreeze after talks
            Freeze();

            Talk(Strings.Get(7));
            Talk(Strings.Get(8));
            Talk(Strings.Get(9), DataLoader.TalkAction.StartNextQuest);
            Talk(Strings.Get(10), DataLoader.TalkAction.Unfreeze);

            MainCameraObject.GetComponent<Animator>().enabled = false;
        }
    }

    public void Freeze(bool allowRotation = false)
    {
        if (allowRotation)
            canRotate = true;
        else
        {
            canRotate = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        canMove = false;
    }

    public void Unfreeze()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        canMove = true;
        canRotate = true;
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

    public void TalkAll(int talkId)
    {
        var talk = DataLoader.Current.GetTalk(talkId);
        if (talk != null && talk.Count > 0)
        {
            foreach (var t in talk)
                SC_FPSController.Current.Talk(Strings.Get(t.string_id), t.action, t.actionParam, t.time);
        }
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

        if (canRotate)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void PerformRaycast()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        int layerMask = 0xFF - (1 << 2) - (1 << 6);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, rayLength, layerMask);

        int realHitIndex = -1;
        bool nameAssigned = false;

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                var namedobj = hit.transform.GetComponent<NamedObjectScript>();
                if (namedobj != null && namedobj.enabled && namedobj.ObjectDescription.Length > 0)
                {
                    realHitIndex = i;
                    nameAssigned = true;
                    RayCastText.text = namedobj.ObjectDescription;
                    if (namedobj.ObjectInteractDescription.Length > 0)
                        RayCastInteractText.text = "[E] " + namedobj.ObjectInteractDescription;
                    else
                        RayCastInteractText.text = "";

                    break;
                }
            }
        }

        if (!nameAssigned)
        {
            RayCastText.text = "";
            RayCastInteractText.text = "";
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Talks.Count > 0)
            {
                TalkRecord cur = Talks[0];
                cur.durationTimer = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!InteractPressed)
            {
                InteractPressed = true;

                if (hits.Length > 0 && realHitIndex >= 0)
                {
                    RaycastHit hit = hits[realHitIndex];

                    Transform objectHit = hit.transform;

                    var interactiveCond = objectHit.GetComponents<InteractiveObjectCondition>();
                    bool prevent = false;
                    foreach (var cond in interactiveCond)
                    {
                        if ((cond as MonoBehaviour).enabled && cond.PreventInteract())
                        {
                            prevent = true;
                            break;
                        }
                    }

                    if (!prevent)
                    {
                        var cmp = objectHit.GetComponent<TogglerScript>();
                        if (cmp != null && cmp.enabled)
                            cmp.ToggleState();

                        var interactive = objectHit.GetComponent<InteractiveObject>();
                        if (interactive != null && (interactive as MonoBehaviour).enabled)
                            interactive.Interact();

                        var named = objectHit.GetComponent<NamedObjectScript>();
                        if (named != null && named.enabled)
                            ObjectivesMgr.Current.SignalObjective(Objectives.Use, named.ObjectIdentifier, ObjectiveGroups.All);
                    }
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
                PerformScriptedAction(10013);
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
                PerformScriptedAction(10015);
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
                        case DataLoader.TalkAction.ScriptedAction:
                            PerformScriptedAction(cur.actionParam);
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

    public void TeleportTo(float x, float y, float z)
    {
        TeleportTo(new Vector3(x, y, z));
    }

    public void TeleportTo(Vector3 targetPos)
    {
        characterController.enabled = false;
        gameObject.transform.position = targetPos;
        characterController.enabled = true;
    }

    public void TeleportTo(float x, float y, float z, float rotX, float rotY, float rotZ, float rotW)
    {
        TeleportTo(new Vector3(x, y, z), new Quaternion(rotX, rotY, rotZ, rotW));
    }

    public void TeleportTo(Vector3 targetPos, Quaternion targetRot)
    {
        characterController.enabled = false;
        gameObject.transform.position = targetPos;
        gameObject.transform.rotation = targetRot;
        characterController.enabled = true;
    }

    private GameObject HeldObject = null;

    [HideInInspector]
    public int HeldObjectId { get; private set; }

    public GameObject CreateHeldObject(int objectId, GameObject objTemplate, Vector3 offset, Quaternion rotation, Vector3 scale)
    {
        if (HeldObject != null)
            ClearHeldObjects();

        HeldObjectId = objectId;

        GameObject obj = Instantiate(objTemplate, gameObject.transform) as GameObject;

        obj.transform.parent = gameObject.transform;

        obj.transform.localPosition = offset;
        obj.transform.localRotation = rotation;
        obj.transform.localScale = scale;

        HeldObject = obj;

        return obj;
    }

    public void ClearHeldObjects()
    {
        HeldObjectId = 0;
        Destroy(HeldObject);
        HeldObject = null;
    }

    public bool IsHoldingObject()
    {
        return (HeldObject != null);
    }

    public GameObject TransferHeldObject(GameObject newParent)
    {
        if (!IsHoldingObject())
            return null;

        GameObject held = HeldObject;
        held.transform.SetParent(newParent.transform);
        HeldObject = null;
        HeldObjectId = 0;

        return held;
    }

    //////////
    ///// Persistent state part
    //////////

    [HideInInspector]
    public bool ToiletUseFlag = false;

    //////////
    ///// Scripted talk actions part
    ///// TODO: move this to separate file?
    //////////

    private Dictionary<int, List<IScriptedActionListener>> _ScriptedActionListeners = new Dictionary<int, List<IScriptedActionListener>>();

    public void SubscribeForScriptedAction(IScriptedActionListener listener, int actionId)
    {
        if (!_ScriptedActionListeners.ContainsKey(actionId))
        {
            _ScriptedActionListeners[actionId] = new List<IScriptedActionListener> { listener };
            return;
        }

        if (_ScriptedActionListeners[actionId].Contains(listener))
            return;

        _ScriptedActionListeners[actionId].Add(listener);
    }

    public void SubscribeForScriptedActions(IScriptedActionListener listener, IEnumerable<int> actionIds)
    {
        foreach (int actionId in actionIds)
            SubscribeForScriptedAction(listener, actionId);
    }

    public void UnsubscribeFromScriptedAction(IScriptedActionListener listener, int actionId)
    {
        if (!_ScriptedActionListeners[actionId].Contains(listener))
            return;

        _ScriptedActionListeners[actionId].Remove(listener);

        // remove empty listener lists
        if (_ScriptedActionListeners[actionId].Count == 0)
            _ScriptedActionListeners.Remove(actionId);
    }

    public void UnsubscribeFromAllScriptedActions(IScriptedActionListener listener)
    {
        foreach (var pair in _ScriptedActionListeners)
            UnsubscribeFromScriptedAction(listener, pair.Key);
    }

    public void BroadcastScriptedAction(int actionId)
    {
        if (_ScriptedActionListeners.ContainsKey(actionId))
        {
            foreach (var listener in _ScriptedActionListeners[actionId])
                listener.ScriptedActionPerformed(actionId);
        }
    }

    public void PerformScriptedAction(int actionId)
    {
        // this object could be considered an "implicit listener of all actions"
        // others need to be subscribed to be later notified
        BroadcastScriptedAction(actionId);

        switch (actionId)
        {
            case 1: // teacher finishes talk about diabetes - signal quest objective
                ObjectivesMgr.Current.SignalObjective(Objectives.Misc, -1);
                Freeze(false);
                break;
            case 2: // exam 1 finishes - signal quest objective
                ObjectivesMgr.Current.SignalObjective(Objectives.Misc, -2);
                Unfreeze();
                Freeze(true);
                break;
            case 3: // lunch picked - signal quest objective
                ObjectivesMgr.Current.SignalObjective(Objectives.Misc, -3);
                break;
            case 4: // PE class, laps finished - signal quest objective
                ObjectivesMgr.Current.SignalObjective(Objectives.Misc, -4);
                break;
            case 5: // listen to doctor
                ObjectivesMgr.Current.SignalObjective(Objectives.Misc, -5);
                break;
            case 6: // sensor learn
                ObjectivesMgr.Current.SignalObjective(Objectives.Misc, -6);
                break;
            case 7: // pump learn
                ObjectivesMgr.Current.SignalObjective(Objectives.Misc, -7);
                break;
            case 8: // eating script
                if (PlayerStatsScript.Current.HungerValue < 0.1)
                {
                    ObjectivesMgr.Current.SignalObjective(Objectives.Misc, -8);
                }
                break;
            case 9: // maintain glycaemia script
                ObjectivesMgr.Current.SignalObjective(Objectives.Misc, -9);
                break;
        }
    }

    private class RegisteredCollectibleToReset
    {
        public int ObjectIdentifier;
        public Transform transform;
        public ObjectiveGroups objectiveGroup;
    }

    private List<RegisteredCollectibleToReset> registeredCollectiblesToReset = new List<RegisteredCollectibleToReset>();

    public void RegisterCollectibleToReset(int objectIdentifier, Transform transform, ObjectiveGroups objGroup = ObjectiveGroups.All)
    {
        registeredCollectiblesToReset.Add(new RegisteredCollectibleToReset { ObjectIdentifier = objectIdentifier, transform = transform, objectiveGroup = objGroup });
    }

    public void ResetCollectibles(ObjectiveGroups objGroup)
    {
        List<RegisteredCollectibleToReset> toRemove = new List<RegisteredCollectibleToReset>();

        foreach (var col in registeredCollectiblesToReset)
        {
            if (col.objectiveGroup == objGroup || objGroup == ObjectiveGroups.All)
            {
                toRemove.Add(col);

                col.transform.GetComponent<Renderer>().enabled = true;
                col.transform.GetComponent<Collider>().enabled = true;
            }
        }

        foreach (var col in toRemove)
            registeredCollectiblesToReset.Remove(col);
    }

    public void ResetCollectibles(int objectIdentifier)
    {
        List<RegisteredCollectibleToReset> toRemove = new List<RegisteredCollectibleToReset>();

        foreach (var col in registeredCollectiblesToReset)
        {
            if (col.ObjectIdentifier == objectIdentifier)
            {
                toRemove.Add(col);

                col.transform.GetComponent<Renderer>().enabled = true;
                col.transform.GetComponent<Collider>().enabled = true;
            }
        }

        foreach (var col in toRemove)
            registeredCollectiblesToReset.Remove(col);
    }
}
