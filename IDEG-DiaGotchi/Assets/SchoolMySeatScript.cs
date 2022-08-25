using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolMySeatScript : UsableObject, IScriptedActionListener
{
    public GameObject TeacherObject = null;
    public GameObject StudentsObjectParent = null;
    public GameObject TeleportFromPlaceTarget = null;

    public override void Interact()
    {
        SC_FPSController.Current.SubscribeForScriptedAction(this, 1);       // talking done, exam started
        SC_FPSController.Current.SubscribeForScriptedAction(this, 2);       // exam finished
        SC_FPSController.Current.SubscribeForScriptedAction(this, 10009);   // finish class (first exam)

        gameObject.GetComponent<MeshCollider>().enabled = false; // this also disables interaction!

        base.Interact();

        SC_FPSController.Current.TeleportTo(gameObject.transform.position);
        SC_FPSController.Current.Freeze(true);

        TeacherObject?.GetComponent<Animator>().SetBool("IsClassInProgress", true);
        SC_FPSController.Current.TalkAll(3);
    }

    public void ScriptedActionPerformed(int actionId)
    {
        // talking done, exam started
        if (actionId == 1)
        {
            TeacherObject?.GetComponent<Animator>().SetBool("IsCheckingClass", true);

            if (StudentsObjectParent != null)
            {
                foreach (Transform tr in StudentsObjectParent.transform)
                {
                    var animator = tr.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetBool("IsWriting", true);
                        animator.SetFloat("AnimSpeedMultiplier", Random.Range(0.8f, 1.2f));
                    }
                }
            }
        }
        // exam finished
        else if (actionId == 2)
        {
            TeacherObject?.GetComponent<Animator>().SetBool("IsCheckingClass", false);

            if (StudentsObjectParent != null)
            {
                foreach (Transform tr in StudentsObjectParent.transform)
                {
                    var animator = tr.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetBool("IsWriting", false);
                        animator.SetFloat("AnimSpeedMultiplier", Random.Range(0.8f, 1.2f));
                    }
                }
            }

            SC_FPSController.Current.TalkAll(4);
        }
        // finish class (first exam)
        else if (actionId == 10009)
        {
            TeacherObject.GetComponent<Animator>().SetBool("IsClassInProgress", false);
            SC_FPSController.Current.Unfreeze();

            if (TeleportFromPlaceTarget != null)
                SC_FPSController.Current.TeleportTo(TeleportFromPlaceTarget.transform.position, TeleportFromPlaceTarget.transform.rotation);

            if (StudentsObjectParent != null)
            {
                foreach (Transform tr in StudentsObjectParent.transform)
                {
                    var animator = tr.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetInteger("SlackingOffSeed", Random.Range(1,3+1 /*exclusive*/));
                        animator.SetFloat("AnimSpeedMultiplier", Random.Range(0.5f, 1.0f));
                    }
                }
            }
        }
    }
}
