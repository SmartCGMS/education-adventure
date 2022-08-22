using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ExamMgr : MonoBehaviour, IScriptedActionListener
{
    public SC_FPSController scController = null;

    private List<int> AnswerIndexMapping = new List<int>();
    private DataLoader.ExamTemplate CurExam;
    private int CurrentQuestionIndex = 0;

    public void ScriptedActionPerformed(int actionId)
    {
        if (actionId == 1)
        {
            // TODO: load exam ID = 1

            LoadExam(1);
            LoadExamQuestion(CurrentQuestionIndex);
            SetNextButtonActive(false);

            GetComponent<Animator>()?.Play("ExamPanelAppear");
        }
    }

    private void LoadExam(int id)
    {
        CurExam = DataLoader.Current.GetExam(id);
        CurrentQuestionIndex = 0;
        SetPanelText("Name", CurExam.name_id);
    }

    private void LoadNextQuestion()
    {
        CurrentQuestionIndex++;
        LoadExamQuestion(CurrentQuestionIndex);
    }

    private void LoadExamQuestion(int index)
    {
        SetPanelText("Question", CurExam.questions[index].question_string_id);

        SetPanelText("ButtonNext/Text", (index < CurExam.questions.Count - 1) ? 84 : 85);

        Text dst = null;

        dst = gameObject.transform.Find("ExamPanel/Counter")?.GetComponent<Text>();
        if (dst != null)
            dst.text = "Question " + (index + 1) + " of " + CurExam.questions.Count;

        AnswerIndexMapping.Clear();
        for (int j = 0; j < CurExam.questions[index].answer_string_ids.Count; j++)
            AnswerIndexMapping.Add(j);
        // is this correct?
        AnswerIndexMapping.Sort((a, b) => UnityEngine.Random.Range(1, 1000).CompareTo(500));

        int i = 0;

        for (i = 0; i < 4; i++)
            SetAnswerButtonActive(i + 1, false);

        for (i = 0; i < AnswerIndexMapping.Count; i++)
        {
            if (CurExam.questions[index].answer_string_ids[AnswerIndexMapping[i]] >= 0)
                SetPanelText("ButtonAnswer"+(i+1)+"/Text", CurExam.questions[index].answer_string_ids[AnswerIndexMapping[i]]);

            SetAnswerButtonActive(i + 1, true);
        }
    }

    private bool IsLastQuestion()
    {
        return (CurrentQuestionIndex >= CurExam.questions.Count - 1);
    }

    private void SetPanelText(string name, int string_id)
    {
        Text dst = null;

        dst = gameObject.transform.Find("ExamPanel/"+name)?.GetComponent<Text>();
        if (dst != null)
            dst.text = Strings.Get(string_id);
    }

    private void SetAnswerButtonActive(int position, bool active)
    {
        var go = gameObject.transform.Find("ExamPanel/ButtonAnswer" + position)?.gameObject;
        if (go != null)
        {
            if (active)
                go.transform.localScale = Vector3.one;
            else
                go.transform.localScale = Vector3.zero;
        }
    }

    private void SetNextButtonActive(bool active)
    {
        var go = gameObject.transform.Find("ExamPanel/ButtonNext")?.gameObject;
        if (go != null)
            go.SetActive(active);
    }

    private void AnimateButton(int position, bool correct)
    {
        var go = gameObject.transform.Find("ExamPanel/ButtonAnswer" + position)?.gameObject;
        if (go != null)
        {
            var anim = go.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("IsCorrect", correct);
                anim.SetBool("IsAnswered", true);
            }
        }
    }

    private void ResetAnimationButton(int position)
    {
        var go = gameObject.transform.Find("ExamPanel/ButtonAnswer" + position)?.gameObject;
        if (go != null)
            go.GetComponent<Animator>()?.SetBool("IsAnswered", false);
    }

    void Start()
    {
        scController.SubscribeForScriptedAction(this, 1);   // exam 1
    }

    private void AnswerSelected(int idx)
    {
        if (idx >= AnswerIndexMapping.Count)
            return;

        if (AnswerIndexMapping[idx] == 0)
        {
            // TODO: add score, display "Correct!" and so on...
        }

        SetNextButtonActive(true);

        for (int i = 0; i < AnswerIndexMapping.Count; i++)
            AnimateButton(i + 1, AnswerIndexMapping[i] == 0);
    }

    public void ButtonAnswer1Clicked()
    {
        AnswerSelected(0);
    }

    public void ButtonAnswer2Clicked()
    {
        AnswerSelected(1);
    }

    public void ButtonAnswer3Clicked()
    {
        AnswerSelected(2);
    }

    public void ButtonAnswer4Clicked()
    {
        AnswerSelected(3);
    }

    public void ButtonNextClicked()
    {
        SetNextButtonActive(false);
        for (int i = 1; i <= 4; i++)
            ResetAnimationButton(i);

        if (!IsLastQuestion())
            LoadNextQuestion();
        else
        {
            GetComponent<Animator>()?.Play("ExamPanelDisappear");
            SC_FPSController.Current.PerformScriptedAction(2);
        }
    }
}
