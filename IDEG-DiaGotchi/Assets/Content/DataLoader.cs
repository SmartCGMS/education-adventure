using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DataLoader
{
    private static DataLoader _Current;

    public static DataLoader Current
    {
        get
        {
            if (_Current == null)
            {
                _Current = new DataLoader();
                _Current.Initialize();
            }
            return _Current;
        }
    }

    public class ObjectTemplate
    {
        public int id;
        public string name;
    }

    public class QuestTemplate
    {
        public int id;
        public int name_id;
        public int end_talk_id;
        public bool autostart_next;

        public List<ObjectiveTemplate> objectives = new List<ObjectiveTemplate>();
    }

    public class ObjectiveTemplate
    {
        public int id;
        public int quest_id;
        public Objectives type;
        public int name_id;
        public int object_id;
        public ObjectiveGroups group;
    }

    public class TalkTemplate
    {
        public int id;
        public int position;
        public int string_id;
        public float time;
        public TalkAction action;
        public int actionParam;
    }

    public enum TalkAction
    {
        None,

        Freeze,
        Unfreeze,
        StartNextQuest
    }

    private Dictionary<int, ObjectTemplate> _Objects = new Dictionary<int, ObjectTemplate>();
    private Dictionary<int, QuestTemplate> _Quests = new Dictionary<int, QuestTemplate>();
    private Dictionary<int, List<TalkTemplate>> _Talks = new Dictionary<int, List<TalkTemplate>>();

    public int MaxQuestId { get; private set; } = 0;

    public void Initialize()
    {
        LoadObjects();
        LoadQuests();
        LoadObjectives();
        LoadTalks();
    }

    private void LoadObjects()
    {
        var res = CSVLoader.ReadResourceCSV("objects");

        foreach (var r in res)
        {
            int id = Int32.Parse(r["id"]);
            string name = r["name"];

            _Objects.Add(id, new ObjectTemplate { id = id, name = name });
        }
    }

    private void LoadQuests()
    {
        var res = CSVLoader.ReadResourceCSV("quests");

        foreach (var r in res)
        {
            int id = Int32.Parse(r["id"]);
            int name = Int32.Parse(r["name_id"]);
            int talk = Int32.Parse(r["end_talk_id"]);
            int autostnext = Int32.Parse(r["autostart_next"]);

            _Quests.Add(id, new QuestTemplate { id = id, name_id = name, end_talk_id = talk, autostart_next = (autostnext != 0) });

            if (id > MaxQuestId)
                MaxQuestId = id;
        }
    }

    private void LoadObjectives()
    {
        var res = CSVLoader.ReadResourceCSV("objectives");

        foreach (var r in res)
        {
            int id = Int32.Parse(r["id"]);
            int qid = Int32.Parse(r["quest_id"]);
            int nid = Int32.Parse(r["name_id"]);
            int oid = Int32.Parse(r["object_id"]);
            string typeName = r["type"];

            Objectives otype = Objectives.None;
            if (typeName == "collect")
                otype = Objectives.Collect;
            else if (typeName == "use")
                otype = Objectives.Use;

            string grpName = r["group"];

            ObjectiveGroups ogrp = ObjectiveGroups.None;
            if (grpName == "home")
                ogrp = ObjectiveGroups.Home;
            else if (grpName == "school")
                ogrp = ObjectiveGroups.School;

            if (!_Quests.ContainsKey(qid))
                continue;

            _Quests[qid].objectives.Add(new ObjectiveTemplate {
                id = id,
                quest_id = qid,
                name_id = nid,
                object_id = oid,
                type = otype,
                group = ogrp
            });
        }
    }

    private void LoadTalks()
    {
        //id;position;string_id;time;action;action_param
        //1;1;13;-1;none;0

        var res = CSVLoader.ReadResourceCSV("talks");

        foreach (var r in res)
        {
            int id = Int32.Parse(r["id"]);
            int pos = Int32.Parse(r["position"]);
            int sid = Int32.Parse(r["string_id"]);
            float time = float.Parse(r["time"]);

            string actionStr = r["action"];
            TalkAction act = TalkAction.None;
            if (actionStr == "freeze")
                act = TalkAction.Freeze;
            else if (actionStr == "unfreeze")
                act = TalkAction.Unfreeze;
            else if (actionStr == "startquest")
                act = TalkAction.StartNextQuest;

            int apar = Int32.Parse(r["action_param"]);

            if (!_Talks.ContainsKey(id))
                _Talks.Add(id, new List<TalkTemplate>());

            _Talks[id].Add(new TalkTemplate { id = id, position = pos, string_id = sid, time = time, action = act, actionParam = apar });
        }

        foreach (var talks in _Talks)
            talks.Value.Sort((TalkTemplate a, TalkTemplate b) => { return a.position.CompareTo(b.position); });
    }

    public ObjectTemplate GetObjectTemplate(int id)
    {
        if (_Objects.ContainsKey(id))
            return _Objects[id];
        return null;
    }

    public QuestTemplate GetQuestTemplate(int id)
    {
        if (_Quests.ContainsKey(id))
            return _Quests[id];
        return null;
    }

    public List<TalkTemplate> GetTalk(int id)
    {
        if (_Talks.ContainsKey(id))
            return _Talks[id];
        return null;
    }
}
