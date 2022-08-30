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
    private List<DataLoader.FoodTemplate> TodaysFood_Soup = new List<DataLoader.FoodTemplate>();

    public List<GameObject> Plates_Main;
    public List<GameObject> Bowls_Soup;

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

        // Blender export for correct local transformation (and to "match plate transformation"):
        //  - Apply Transform
        //  - scale = FBX All
        //  - X Forward, Y Up
        if (generateMeals)
        {
            // main meal scope
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

                    obj.AddComponent<BoxCollider>();

                    obj.AddComponent<NamedObjectScript>().ObjectDescription = Strings.Get(TodaysFood_Main[i].name_id);

                    var cfs = obj.AddComponent<CafeteriaFoodScript>();
                    cfs.RequiredTrayState = TrayState.OnRailing_1;
                    cfs.CafeteriaFoodCategory = DataLoader.FoodCategory.MainCourse;
                    cfs.CafeteriaFoodIndex = i;
                }
            }

            // soup scope
            {
                TodaysFood_Soup.Clear();
                var ids = SelectRandomN(DataLoader.Current.GetFoodIds(DataLoader.FoodCategory.Soup), 2);
                foreach (var id in ids)
                    TodaysFood_Soup.Add(DataLoader.Current.GetFood(id));

                // Resources/Food/polevka-****
                for (int i = 0; i < Bowls_Soup.Count && i < Bowls_Soup.Count; i++)
                {
                    var res = Resources.Load<GameObject>("Food/polevka-" + TodaysFood_Soup[i].alias);
                    if (res == null)
                        continue;

                    var obj = Instantiate(res, Bowls_Soup[i].transform);

                    obj.AddComponent<BoxCollider>();

                    obj.AddComponent<NamedObjectScript>().ObjectDescription = Strings.Get(TodaysFood_Soup[i].name_id);

                    var cfs = obj.AddComponent<CafeteriaFoodScript>();
                    cfs.RequiredTrayState = TrayState.OnRailing_2;
                    cfs.CafeteriaFoodCategory = DataLoader.FoodCategory.Soup;
                    cfs.CafeteriaFoodIndex = i;
                }
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
            if (trayState == TrayState.OnRailing_1)// || trayState == TrayState.OnRailing_2 || trayState == TrayState.OnRailing_3 || trayState == TrayState.OnRailing_4)
            {
                TrayObject.GetComponent<Animator>()?.SetInteger("TrayState", 2);
            }
        }
    }

    public void SetTrayObject(GameObject go)
    {
        TrayObject = go;
    }

    public void SignalFoodTaken(DataLoader.FoodCategory category, int index)
    {
        if (category == DataLoader.FoodCategory.MainCourse && trayState == TrayState.OnRailing_1)
        {
            TrayObject.GetComponent<Animator>()?.SetInteger("TrayState", 3);
            SetTrayState(TrayState.OnRailing_2);

            var plate = Instantiate(Plates_Main[index], TrayObject.transform);
            plate.transform.localPosition = new Vector3(-1.143f, 0.09f, -0.288f);
            plate.transform.localRotation = Quaternion.Euler(0, 0, 0);
            plate.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else if (category == DataLoader.FoodCategory.Soup && trayState == TrayState.OnRailing_2)
        {
            TrayObject.GetComponent<Animator>()?.SetInteger("TrayState", 4);
            SetTrayState(TrayState.OnRailing_3);

            var plate = Instantiate(Bowls_Soup[index], TrayObject.transform);
            plate.transform.localPosition = new Vector3(0.333f, 0.0118f, 0.307f);
            plate.transform.localRotation = Quaternion.Euler(0, 0, 0);
            plate.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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
