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

    public class AreaTriggerTemplate
    {
        public int id;
        public int name_id;
    }

    public class ExamTemplate
    {
        public int id;
        public int name_id;
        public int finishedScriptedActionId;

        public List<ExamQuestionTemplate> questions = new List<ExamQuestionTemplate>();
    }

    public class ExamQuestionTemplate
    {
        public int exam_id;
        public int position;
        public int question_string_id;
        public List<int> answer_string_ids = new List<int>(); // index 0 is always the right answer
    }

    public class FoodTemplate
    {
        public int id;
        public string alias;
        public int name_id;
        public FoodCategory category;
        public int base_amount;
        public int portion_amount;
        public string unit;
        public int kilocalories;
        public int carbs;
        public int sugar;
        public int fat;
        public int proteins;
        public int fibre;
    }

    public enum TalkAction
    {
        None,

        Freeze,
        Unfreeze,
        StartNextQuest,
        ScriptedAction,
    }

    public enum FoodCategory
    {
        None,

        MainCourse,
        Soup,
        Side,
        Drink,

        All
    }

    private Dictionary<int, ObjectTemplate> _Objects = new Dictionary<int, ObjectTemplate>();
    private Dictionary<int, QuestTemplate> _Quests = new Dictionary<int, QuestTemplate>();
    private Dictionary<int, List<TalkTemplate>> _Talks = new Dictionary<int, List<TalkTemplate>>();
    private Dictionary<int, AreaTriggerTemplate> _AreaTriggers = new Dictionary<int, AreaTriggerTemplate>();
    private Dictionary<int, ExamTemplate> _Exams = new Dictionary<int, ExamTemplate>();
    private Dictionary<int, FoodTemplate> _Food = new Dictionary<int, FoodTemplate>();

    public int MaxQuestId { get; private set; } = 0;

    public void Initialize()
    {
        LoadObjects();
        LoadQuests();
        LoadObjectives();
        LoadTalks();
        LoadAreaTriggers();
        LoadExams();
        LoadFood();
    }

    private void LoadObjects()
    {
        var res = CSVLoader.ReadResourceCSV("objects.en");

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
            else if (typeName == "areatrigger")
                otype = Objectives.AreaTrigger;
            else if (typeName == "misc")
                otype = Objectives.Misc;

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
            else if (actionStr == "scripted")
                act = TalkAction.ScriptedAction;

            int apar = Int32.Parse(r["action_param"]);

            if (!_Talks.ContainsKey(id))
                _Talks.Add(id, new List<TalkTemplate>());

            _Talks[id].Add(new TalkTemplate { id = id, position = pos, string_id = sid, time = time, action = act, actionParam = apar });
        }

        foreach (var talks in _Talks)
            talks.Value.Sort((TalkTemplate a, TalkTemplate b) => { return a.position.CompareTo(b.position); });
    }

    private void LoadAreaTriggers()
    {
        var res = CSVLoader.ReadResourceCSV("areatriggers");

        foreach (var r in res)
        {
            int id = Int32.Parse(r["id"]);
            int name = Int32.Parse(r["name_id"]);

            _AreaTriggers.Add(id, new AreaTriggerTemplate { id = id, name_id = name });
        }
    }

    private void LoadExams()
    {
        var res = CSVLoader.ReadResourceCSV("exams");

        foreach (var r in res)
        {
            int id = Int32.Parse(r["id"]);
            int name = Int32.Parse(r["name_id"]);
            int fid = Int32.Parse(r["finished_scripted_action_id"]);

            _Exams.Add(id, new ExamTemplate { id = id, name_id = name, finishedScriptedActionId = fid });
        }

        var rq = CSVLoader.ReadResourceCSV("examquestions");

        foreach (var r in rq)
        {
            var id = Int32.Parse(r["id"]);

            if (!_Exams.ContainsKey(id))
                continue;

            int pos = Int32.Parse(r["position"]);
            int q_id = Int32.Parse(r["question_string_id"]);
            int cor_id = Int32.Parse(r["answer0_correct_string_id"]);

            ExamQuestionTemplate qtpl = new ExamQuestionTemplate() { exam_id = id, position = pos, question_string_id = q_id };

            qtpl.answer_string_ids.Add(cor_id);

            for (int i = 1; i <= 3; i++)
            {
                int ans_id = Int32.Parse(r["answer"+i+"_string_id"]);
                if (ans_id > 0)
                    qtpl.answer_string_ids.Add(ans_id);
            }

            _Exams[id].questions.Add(qtpl);
        }

        foreach (var exam in _Exams)
            exam.Value.questions.Sort((ExamQuestionTemplate a, ExamQuestionTemplate b) => { return a.position.CompareTo(b.position); });
    }

    private void LoadFood()
    {
        var res = CSVLoader.ReadResourceCSV("food");

        foreach (var r in res)
        {
            var cat = r["category"];
            FoodCategory fc = FoodCategory.None;
            if (cat == "main")
                fc = FoodCategory.MainCourse;
            else if (cat == "soup")
                fc = FoodCategory.Soup;
            else if (cat == "side")
                fc = FoodCategory.Side;
            else if (cat == "drink")
                fc = FoodCategory.Drink;

            var fd = new FoodTemplate() {
                id = Int32.Parse(r["id"]),
                alias = r["alias"],
                name_id = Int32.Parse(r["name_id"]),
                category = fc,
                base_amount = TryParseInt32(r["baseamount"], 100),
                portion_amount = TryParseInt32(r["portionamount"], 100),
                unit = r["unit"],
                kilocalories = TryParseInt32(r["kcal"], 0),
                carbs = TryParseInt32(r["carbohydrates"], 0),
                sugar = TryParseInt32(r["sugar"]),
                fat = TryParseInt32(r["fat"]),
                proteins = TryParseInt32(r["proteins"]),
                fibre = TryParseInt32(r["fibre"])
            };

            _Food.Add(fd.id, fd);
        }
    }

    private Int32 TryParseInt32(string input, int defaultVal = 0)
    {
        int result = 0;
        if (Int32.TryParse(input, out result))
            return result;

        return defaultVal;
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

    public AreaTriggerTemplate GetAreaTriggerTemplate(int id)
    {
        if (_AreaTriggers.ContainsKey(id))
            return _AreaTriggers[id];
        return null;
    }

    public ExamTemplate GetExam(int id)
    {
        if (_Exams.ContainsKey(id))
            return _Exams[id];
        return null;
    }

    public FoodTemplate GetFood(int id)
    {
        if (_Food.ContainsKey(id))
            return _Food[id];
        return null;
    }

    public List<int> GetFoodIds(FoodCategory cat = FoodCategory.All)
    {
        if (cat == FoodCategory.All)
            return _Food.Keys.ToList();

        List<int> ids = new List<int>();
        foreach (var tp in _Food)
        {
            if (tp.Value.category == cat)
                ids.Add(tp.Key);
        }

        return ids;
    }
}
