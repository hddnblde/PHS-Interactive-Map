using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SchedulerTool : MonoBehaviour
{
	[SerializeField]
	private string path = "";

	private void Start()
	{
		ReadText();
	}

	private void ReadText()
	{
		#if UNITY_EDITOR
		TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

		Debug.Log(textAsset != null);

		if(textAsset == null)
			return;

		ShowLines(textAsset.text);
		#endif
	}

	private void ShowLines(string body)
	{
		int lineNumber = 1;
		string[] lines = body.Split("\n".ToCharArray());
		foreach(string line in lines)
		{
			Debug.Log(lineNumber + " : " + DeserializeLine(line));
			lineNumber++;
		}
	}

	private string DeserializeLine(string line)
	{
		string[] entries = line.Split("\t".ToCharArray());

		if(entries.Length < 5)
			return "";
		
		string period = entries[0];
		string timeIn = entries[1];
		string timeOut = entries[2];
		string grade = entries[3];
		string section = entries[4];

		string monday = DeserializeSchedule("MON", entries[5], entries[6], entries[7]);
		string tuesday = DeserializeSchedule("TUE", entries[8], entries[9], entries[10]);
		string wednesday = DeserializeSchedule("WED", entries[11], entries[12], entries[13]);
		string thursday = DeserializeSchedule("THU", entries[14], entries[15], entries[16]);
		string friday = DeserializeSchedule("FRI", entries[17], entries[18], entries[19]);

		string message = "[@period] (@timeIn - @timeOut) Grade @grade @section -- @mon | @tue | @wed | @thu | @fri"
			.Replace("@period", period).Replace("@timeIn", timeIn).Replace("@timeOut", timeOut)
			.Replace("@grade", grade).Replace("@section", section).Replace("@mon", monday)
			.Replace("@tue", tuesday).Replace("@wed", wednesday).Replace("@thu", thursday)
			.Replace("@fri", friday);

		return message;
	}

	private string DeserializeSchedule(string header, string subject, string room, string teacher)
	{
		string message = "@header [@subject_@room_@teacher]"
			.Replace("@header", header).Replace("@subject", subject)
			.Replace("@room", room).Replace("@teacher", teacher);
		
		return message;
	}
}
