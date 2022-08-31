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
    private List<DataLoader.FoodTemplate> TodaysFood_Side = new List<DataLoader.FoodTemplate>();
    private List<DataLoader.FoodTemplate> TodaysFood_Drink = new List<DataLoader.FoodTemplate>();

    public List<GameObject> Plates_Main;
    public List<GameObject> Bowls_Soup;
    public List<GameObject> Bowls_Side;
    public List<GameObject> Cups_Drink;

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
                var ids = SelectRandomN(DataLoader.Current.GetFoodIds(DataLoader.FoodCategory.MainCourse), Plates_Main.Count);
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

                    var named = obj.AddComponent<NamedObjectScript>();
                    named.ObjectDescription = Strings.Get(TodaysFood_Main[i].name_id);
                    named.ObjectInteractDescription = Strings.Get(112);

                    var cfs = obj.AddComponent<CafeteriaFoodScript>();
                    cfs.RequiredTrayState = TrayState.OnRailing_1;
                    cfs.CafeteriaFoodCategory = DataLoader.FoodCategory.MainCourse;
                    cfs.CafeteriaFoodIndex = i;
                }
            }

            // soup scope
            {
                TodaysFood_Soup.Clear();
                var ids = SelectRandomN(DataLoader.Current.GetFoodIds(DataLoader.FoodCategory.Soup), Bowls_Soup.Count);
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

                    var named = obj.AddComponent<NamedObjectScript>();
                    named.ObjectDescription = Strings.Get(TodaysFood_Soup[i].name_id);
                    named.ObjectInteractDescription = Strings.Get(112);

                    var cfs = obj.AddComponent<CafeteriaFoodScript>();
                    cfs.RequiredTrayState = TrayState.OnRailing_2;
                    cfs.CafeteriaFoodCategory = DataLoader.FoodCategory.Soup;
                    cfs.CafeteriaFoodIndex = i;
                }
            }

            // sides scope
            {
                TodaysFood_Side.Clear();
                var ids = SelectRandomN(DataLoader.Current.GetFoodIds(DataLoader.FoodCategory.Side), Bowls_Side.Count);
                foreach (var id in ids)
                    TodaysFood_Side.Add(DataLoader.Current.GetFood(id));

                // Resources/Food/side-****
                for (int i = 0; i < Bowls_Side.Count && i < Bowls_Side.Count; i++)
                {
                    var res = Resources.Load<GameObject>("Food/side-" + TodaysFood_Side[i].alias);
                    if (res == null)
                        continue;

                    var obj = Instantiate(res, Bowls_Side[i].transform);

                    obj.AddComponent<BoxCollider>();

                    var named = obj.AddComponent<NamedObjectScript>();
                    named.ObjectDescription = Strings.Get(TodaysFood_Side[i].name_id);
                    named.ObjectInteractDescription = Strings.Get(112);

                    var cfs = obj.AddComponent<CafeteriaFoodScript>();
                    cfs.RequiredTrayState = TrayState.OnRailing_3;
                    cfs.CafeteriaFoodCategory = DataLoader.FoodCategory.Side;
                    cfs.CafeteriaFoodIndex = i;
                }
            }

            // cup scope
            {
                TodaysFood_Drink.Clear();
                var ids = SelectRandomN(DataLoader.Current.GetFoodIds(DataLoader.FoodCategory.Drink), Cups_Drink.Count);
                foreach (var id in ids)
                    TodaysFood_Drink.Add(DataLoader.Current.GetFood(id));

                // Resources/Food/polevka-****
                for (int i = 0; i < Cups_Drink.Count && i < Cups_Drink.Count; i++)
                {
                    var res = Resources.Load<GameObject>("Food/piti-" + TodaysFood_Drink[i].alias);
                    if (res == null)
                        continue;

                    var obj = Instantiate(res, Cups_Drink[i].transform);

                    obj.transform.localScale = new Vector3(1.0f, 0.14f, 1.0f);

                    obj.AddComponent<BoxCollider>();

                    var named = obj.AddComponent<NamedObjectScript>();
                    named.ObjectDescription = Strings.Get(TodaysFood_Drink[i].name_id);
                    named.ObjectInteractDescription = Strings.Get(112);

                    var cfs = obj.AddComponent<CafeteriaFoodScript>();
                    cfs.RequiredTrayState = TrayState.OnRailing_4;
                    cfs.CafeteriaFoodCategory = DataLoader.FoodCategory.Drink;
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
            plate.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

            DisableInteractionForChildren(plate);

            DisableInteractionName(ref Plates_Main);
        }
        else if (category == DataLoader.FoodCategory.Soup && trayState == TrayState.OnRailing_2)
        {
            TrayObject.GetComponent<Animator>()?.SetInteger("TrayState", 4);
            SetTrayState(TrayState.OnRailing_3);

            var plate = Instantiate(Bowls_Soup[index], TrayObject.transform);
            plate.transform.localPosition = new Vector3(0.333f, 0.0118f, 0.307f);
            plate.transform.localRotation = Quaternion.Euler(0, 0, 0);
            plate.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);

            DisableInteractionForChildren(plate);

            DisableInteractionName(ref Bowls_Soup);
        }
        else if (category == DataLoader.FoodCategory.Side && trayState == TrayState.OnRailing_3)
        {
            TrayObject.GetComponent<Animator>()?.SetInteger("TrayState", 5);
            SetTrayState(TrayState.OnRailing_4);

            var plate = Instantiate(Bowls_Side[index], TrayObject.transform);
            plate.transform.localPosition = new Vector3(-0.123f, 0.045f, -0.477f);
            plate.transform.localRotation = Quaternion.Euler(0, 0, 0);
            plate.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            DisableInteractionForChildren(plate);

            DisableInteractionName(ref Bowls_Side);
        }
        else if (category == DataLoader.FoodCategory.Drink && trayState == TrayState.OnRailing_4)
        {
            var comp = TrayObject.GetComponent<TrayScript>();
            if (comp)
                comp.enabled = true;

            var comp2 = TrayObject.GetComponent<NamedObjectScript>();
            if (comp2)
                comp2.enabled = true;

            SetTrayState(TrayState.Finished);

            var plate = Instantiate(Cups_Drink[index], TrayObject.transform);
            plate.transform.localPosition = new Vector3(0.555f, 0.167f, -0.593f);
            plate.transform.localRotation = Quaternion.Euler(0, 0, 0);
            plate.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);

            DisableInteractionForChildren(plate);

            DisableInteractionName(ref Cups_Drink);
        }
    }

    void DisableInteractionForChildren(GameObject obj)
    {
        for (int cidx = 0; cidx < obj.transform.childCount; cidx++)
        {
            var named = obj.transform.GetChild(cidx).GetComponent<NamedObjectScript>();
            if (named != null)
                named.ObjectInteractDescription = "";
        }
    }

    void DisableInteractionName(ref List<GameObject> list)
    {
        foreach (var obj in list)
        {
            DisableInteractionForChildren(obj);
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
