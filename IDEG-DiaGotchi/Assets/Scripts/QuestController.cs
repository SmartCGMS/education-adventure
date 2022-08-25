using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController
{
    private static QuestController _Current;

    public static QuestController Current
    {
        get
        {
            if (_Current == null)
                _Current = new QuestController();
            return _Current;
        }
    }

    private int CurrentQuestId = 0;

    public void FakeStart(int questId)
    {
        CurrentQuestId = questId;
        var tpl = DataLoader.Current.GetQuestTemplate(CurrentQuestId);
        if (tpl != null)
            ObjectivesMgr.Current.PushQuestObjectives(tpl.id);

        // TODO: finish prerequisites once we have repeatable quests?
    }

    public void StartNextQuest()
    {
        ObjectivesMgr.Current.ClearCompletedObjectives(ObjectiveGroups.All, CurrentQuestId);

        while (CurrentQuestId < DataLoader.Current.MaxQuestId)
        {
            CurrentQuestId++;

            // TODO: see if the quest is not completed/failed or optional

            var tpl = DataLoader.Current.GetQuestTemplate(CurrentQuestId);
            if (tpl != null)
            {
                ObjectivesMgr.Current.PushQuestObjectives(tpl.id);
                break;
            }
        }
    }
}
