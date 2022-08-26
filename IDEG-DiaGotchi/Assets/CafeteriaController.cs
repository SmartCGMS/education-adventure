using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeteriaController : MonoBehaviour
{
    public static CafeteriaController Current { get; private set; }

    public enum TrayState
    {
        None,
        OnRailing_1,
        OnRailing_2,
        OnRailing_3,
        OnRailing_4,
        Finished,
    }

    public TrayState trayState { get; private set; } = TrayState.None;

    private List<DataLoader.FoodTemplate> TodaysFood_Main = new List<DataLoader.FoodTemplate>();

    public List<GameObject> Plates_Main;

    public List<GameObject> TrayLines;

    private GameObject TrayObject = null;

    public void ResetCafeteria(bool generateMeals = true)
    {
        trayState = TrayState.None;
        if (TrayObject != null)
            Destroy(TrayObject);
        TrayObject = null;

        if (TrayLines != null && TrayLines.Count > 0)
        {
            var cm = TrayLines[0].GetComponent<NamedObjectScript>();
            if (cm != null)
                cm.enabled = true;
        }

        if (generateMeals)
        {
            TodaysFood_Main.Clear();
            var ids = SelectRandomN(DataLoader.Current.GetFoodIds(DataLoader.FoodCategory.MainCourse), 3);
            foreach (var id in ids)
                TodaysFood_Main.Add(DataLoader.Current.GetFood(id));

            // Resources/Food/jidlo-****
            for (int i = 0; i < Plates_Main.Count && i < TodaysFood_Main.Count; i++)
            {
                var res = Resources.Load<GameObject>("Food/jidlo-" + TodaysFood_Main[i].alias);
                if (res == null)
                    continue;

                var obj = Instantiate(res, Plates_Main[i].transform);
                // Blender export for correct local transformation (and to "match plate transformation"):
                //  - Apply Transform
                //  - scale = FBX All
                //  - X Forward, Y Up

                var collider = obj.AddComponent<BoxCollider>();

                var named = obj.AddComponent<NamedObjectScript>();
                named.ObjectDescription = Strings.Get(TodaysFood_Main[i].name_id);

                var cfs = obj.AddComponent<CafeteriaFoodScript>();
                cfs.RequiredTrayState = TrayState.OnRailing_1;
                cfs.CafeteriaFoodIndex = i;
            }
        }
    }

    private List<int> SelectRandomN(List<int> inputList, int count)
    {
        List<int> chosen = new List<int>();

        for (int i = 0; i < count; i++)
        {
            int r = Random.Range(0, inputList.Count - i);

            // swap to the end, so we won't choose it again and also won't drop the ID from the list entirely
            int vc = inputList[r];
            inputList[r] = inputList[inputList.Count - 1 - i];
            inputList[inputList.Count - 1 - i] = vc;

            chosen.Add(vc);
        }

        return chosen;
    }

    public void SetTrayState(TrayState state)
    {
        trayState = state;

        if (TrayObject != null)
        {
            if (trayState == TrayState.OnRailing_1 || trayState == TrayState.OnRailing_2 || trayState == TrayState.OnRailing_3 || trayState == TrayState.OnRailing_4)
            {
                TrayObject.transform.localPosition = new Vector3(-0.0637f, 0.1432f, 0.1514f);
                TrayObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    public void SetTrayObject(GameObject go)
    {
        TrayObject = go;
    }

    public void SignalFoodTaken(int index)
    {
        if (trayState == TrayState.OnRailing_1)
        {
            if (TrayObject != null)
                TrayObject.transform.SetParent(TrayLines[1].transform);
            SetTrayState(TrayState.OnRailing_2);

            var plate = Instantiate(Plates_Main[index], TrayObject.transform);
            plate.transform.localPosition = new Vector3(0,0, 0.0009f);
            plate.transform.localRotation = Quaternion.Euler(0,90,90);
            plate.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        }
    }

    void Start()
    {
        Current = this;
    }

    void Update()
    {
        //
    }
}
