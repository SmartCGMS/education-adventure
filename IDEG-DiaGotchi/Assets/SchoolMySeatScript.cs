using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolMySeatScript : UsableObject, IScriptedActionListener
{
    public GameObject TeacherObject = null;
    public GameObject StudentsObjectParent = null;

    public override void Interact()
    {
        SC_FPSController.Current.SubscribeForScriptedAction(this, 1);
        SC_FPSController.Current.SubscribeForScriptedAction(this, 2);
        SC_FPSController.Current.SubscribeForScriptedAction(this, 10009);
        gameObject.GetComponent<MeshCollider>().enabled = false; // this also disables interaction!

        base.Interact();

        SC_FPSController.Current.TeleportTo(gameObject.transform.position);
        SC_FPSController.Current.Freeze(true);

        if (TeacherObject != null)
        {
            //TeacherObject.GetComponent<Animator>().Play("Armature|Teaching1");
            TeacherObject.GetComponent<Animator>().SetBool("IsClassInProgress", true);
            SC_FPSController.Current.TalkAll(3);
        }
    }

    public void ScriptedActionPerformed(int actionId)
    {
        if (actionId == 1)
        {
            if (TeacherObject != null)
            {
                //TeacherObject.GetComponent<Animator>().Play("Armature|CheckingClassLoop");
                TeacherObject.GetComponent<Animator>().SetBool("IsCheckingClass", true);
            }

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
        else if (actionId == 2)
        {
            TeacherObject.GetComponent<Animator>().SetBool("IsCheckingClass", false);

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
        else if (actionId == 10009)
        {
            TeacherObject.GetComponent<Animator>().SetBool("IsClassInProgress", false);
        }
    }
}
