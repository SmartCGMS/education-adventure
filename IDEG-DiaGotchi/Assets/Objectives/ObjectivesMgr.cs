using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ObjectivesMgr : MonoBehaviour
{
    public static ObjectivesMgr Current;

    private class ObjectiveRecord
    {
        public Objectives type;
        public string name;
        public int objectIdentifier;

        public ObjectiveGroups group = ObjectiveGroups.None;
        public bool completed = false;
        public int questId = 0;
    }

    private List<ObjectiveRecord> CurrentObjectives = new List<ObjectiveRecord>();

    public void Start()
    {
        Current = this;

        Initialize();
    }

    public void Initialize()
    {
        // TODO: load stored game from file

        UpdateGUI();
    }

    public void PushQuestObjectives(int questId)
    {
        DataLoader.QuestTemplate tpl = DataLoader.Current.GetQuestTemplate(questId);

        if (tpl == null)
            return;

        foreach (var obj in tpl.objectives)
            PushObjective(obj.type, Strings.Get(obj.name_id), obj.object_id, obj.group, obj.quest_id);
    }

    public void PushObjective(Objectives objective, string text, int objectId = 0, ObjectiveGroups group = ObjectiveGroups.None, int questId = 0)
    {
        CurrentObjectives.Add(new ObjectiveRecord {
            type = objective,
            name = text,
            objectIdentifier = objectId,
            group = group,
            completed = false,
            questId = questId
        });

        UpdateGUI();
    }

    public void SignalObjective(Objectives objective, int objectId = 0, ObjectiveGroups group = ObjectiveGroups.All)
    {
        bool update = false;

        Dictionary<int, bool> completeMap = new Dictionary<int, bool>();

        foreach (var obj in CurrentObjectives)
        {
            if (obj.type == objective && (obj.group == group || group == ObjectiveGroups.All) && obj.objectIdentifier == objectId)
            {
                if (!obj.completed)
                {
                    obj.completed = true;
                    update = true;
                }
            }

            completeMap[obj.questId] = true;
        }

        foreach (var obj in CurrentObjectives)
        {
            if (!obj.completed)
                completeMap[obj.questId] = false;
        }

        bool allComplete = true;
        bool allAutostart = true;
        foreach (var comp in completeMap)
        {
            var qt = DataLoader.Current.GetQuestTemplate(comp.Key);

            if (comp.Value)
            {
                if (qt != null && qt.end_talk_id > 0)
                {
                    var talk = DataLoader.Current.GetTalk(qt.end_talk_id);
                    if (talk != null && talk.Count > 0)
                    {
                        foreach (var t in talk)
                            SC_FPSController.Current.Talk(Strings.Get(t.string_id), t.action, t.actionParam, t.time);
                    }
                }
            }
            else
                allComplete = false;

            if (qt != null && !qt.autostart_next)
                allAutostart = false;
        }

        if (allComplete && allAutostart)
            QuestController.Current.StartNextQuest();

        if (update)
            UpdateGUI();
    }

    public void ClearCompletedObjectives(ObjectiveGroups group = ObjectiveGroups.All, int questId = 0)
    {
        CurrentObjectives.RemoveAll(x => (x.group == group || group == ObjectiveGroups.All) && (x.questId == questId || questId == 0) && x.completed);

        UpdateGUI();
    }

    public void UpdateGUI()
    {
        var obj = GameObject.Find("ObjectivesUIPanel");

        if (obj == null)
            return;

        for (int i = 0; i < 5; i++)
        {
            var oitem = obj.transform.Find("Objective" + (i+1));

            oitem.gameObject.SetActive(false);
        }

        var titletxt = obj.transform.Find("Title");
        bool first = false;

        if (titletxt != null)
            titletxt.transform.gameObject.SetActive(false);

        int cursor = 0;
        foreach (var sobj in CurrentObjectives)
        {
            if (!first)
            {
                first = true;
                
                if (titletxt != null)
                {
                    titletxt.transform.gameObject.SetActive(true);

                    var qt = DataLoader.Current.GetQuestTemplate(sobj.questId);
                    var ttxt = titletxt.GetComponent<Text>();
                    if (qt != null && ttxt != null && qt.name_id > 0)
                        ttxt.text = Strings.Get(qt.name_id);
                    else
                        ttxt.text = "Objectives";
                }
            }

            var oitem = obj.transform.Find("Objective" + (cursor + 1));

            oitem.gameObject.SetActive(true);

            var txt = oitem.gameObject.GetComponent<Text>();
            if (txt != null)
            {
                txt.text = sobj.name;

                if (sobj.completed)
                    txt.color = Color.green;
                else
                    txt.color = Color.white;
            }

            cursor++;
            if (cursor >= 5)
                break;
        }
    }
}
