using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodUIScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseFoodUI()
    {
        var anim = GetComponent<Animator>();
        if (anim != null)
        {
            SC_FPSController.Current.Unfreeze();
            anim.Play("UIPanelDisappear");
        }
    }

    public void CloseButtonClicked()
    {
        CloseFoodUI();
    }
}
