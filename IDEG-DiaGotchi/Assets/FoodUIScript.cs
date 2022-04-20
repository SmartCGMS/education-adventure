using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class FoodUIScript : MonoBehaviour
{
	public GameObject FoodEntryPrefab;
	public GameObject UIParent;

	public Button PrevButton;
	public Button NextButton;

	private int CurrentPage = 0;

	private List<GameObject> FoodObjs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
#if UNITY_EDITOR
        string assetsPath = Application.dataPath;
        basePath = assetsPath.Replace("/Assets", "");
#endif

		var str = File.ReadAllText(basePath + "/ExternalData/Food/food.csv");
		var parsed = ReadCSV(str);

		int increment = 250;

		int ipos = -increment;
		bool firstlayer = true;

		foreach (var line in parsed)
        {
			//id;name;img;baseamount;portionamount;unit;calories;carbohydrates;sugar;fat;proteins;fibre

			var res = Instantiate(FoodEntryPrefab, UIParent.transform.position + new Vector3(ipos, 0, 0), Quaternion.identity, UIParent.transform);

			FoodObjs.Add(res);

			res.SetActive(firstlayer);

			double carbs = double.Parse(line["carbohydrates"]);
			double portion = double.Parse(line["portionamount"]);
			double baseamt = double.Parse(line["baseamount"]);

			double mul = portion / baseamt;

			var btn = res.transform.Find("EatButton");
			FoodRecordScript scr = btn.GetComponent<FoodRecordScript>();
			scr.FoodUI = this;
			scr.HungerValue = (float)(0.004 * portion);
			scr.CarbContent = (float)(carbs * mul);

			res.transform.Find("FoodName").GetComponent<Text>().text = line["name"];
			res.transform.Find("FoodCarbContent").GetComponent<Text>().text = CarbContentString(carbs, mul);
			res.transform.Find("FoodHungerInfo").GetComponent<Text>().text = HungerValueString(portion, 0.004 * mul);

			ipos += increment;
			if (ipos > increment)
            {
				firstlayer = false;
				ipos = -increment;
			}

			try
			{
				var imgbytes = System.IO.File.ReadAllBytes(basePath + "/ExternalData/Food/img/" + line["img"]);
				var texture = new Texture2D(2, 2);
				texture.LoadImage(imgbytes);

				res.transform.Find("FoodImage").GetComponent<RawImage>().texture = texture;
			}
			catch
            {
				Debug.LogError("Cannot load food texture of " + line["name"]);
            }
		}

		UpdatePaginatorButtons();
	}

	public void PrevButtonClicked()
	{
		for (int i = CurrentPage * 3; i < (CurrentPage + 1) * 3 && i < FoodObjs.Count; i++)
			FoodObjs[i].SetActive(false);

		CurrentPage--;

		for (int i = CurrentPage * 3; i < (CurrentPage + 1) * 3 && i < FoodObjs.Count; i++)
			FoodObjs[i].SetActive(true);

		UpdatePaginatorButtons();
	}

	public void NextButtonClicked()
	{
		for (int i = CurrentPage * 3; i < (CurrentPage + 1) * 3 && i < FoodObjs.Count; i++)
			FoodObjs[i].SetActive(false);

		CurrentPage++;

		for (int i = CurrentPage * 3; i < (CurrentPage + 1) * 3 && i < FoodObjs.Count; i++)
			FoodObjs[i].SetActive(true);

		UpdatePaginatorButtons();
	}

	private void UpdatePaginatorButtons()
    {
		PrevButton.interactable = (CurrentPage != 0);
		NextButton.interactable = (((CurrentPage + 1) * 3) < FoodObjs.Count);
    }

	string CarbContentString(double carbVal, double multiplier = 1.0)
    {
		try
		{
			double carbs = carbVal * multiplier;
			if (carbs < 20)
				return "Low carb amount";
			if (carbs < 40)
				return "Medium carb amount";
			if (carbs < 60)
				return "Higher carb amount";
			return "High carb amount";
		}
		catch
        {
			//
        }

		return "No carbs";
    }

	string HungerValueString(double hungerVal, double multiplier = 1.0)
	{
		try
		{
			double hunger = hungerVal * multiplier;
			if (hunger < 0.1)
				return "Low quantity";
			if (hunger < 0.3)
				return "Medium quantity";
			if (hunger < 0.55)
				return "Higher quantity";
			return "High quantity";
		}
		catch
		{
			//
		}

		return "Negligible amount";
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

	static string SPLIT_RE = @";(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };

	public static List<Dictionary<string, string>> ReadCSV(string fileContents)
	{
		var list = new List<Dictionary<string, string>>();

		var lines = Regex.Split(fileContents, LINE_SPLIT_RE);

		if (lines.Length <= 1) return list;

		var header = Regex.Split(lines[0], SPLIT_RE);
		for (var i = 1; i < lines.Length; i++)
		{

			var values = Regex.Split(lines[i], SPLIT_RE);
			if (values.Length == 0 || values[0] == "") continue;

			var entry = new Dictionary<string, string>();
			for (var j = 0; j < header.Length && j < values.Length; j++)
			{
				string value = values[j];
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
				string finalvalue = value;
				entry[header[j]] = finalvalue;
			}
			list.Add(entry);
		}
		return list;
	}
}
