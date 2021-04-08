using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class PlayerController : MonoBehaviour 
{
	public AudioSource sortSound, buttonSound, addSound;
	public string curSelectedReceiptBundle;
	public Dictionary<string, string> receiptBundles = new Dictionary<string, string>();
	public UIPopupList receiptBundlePopupList, categoryPopupList, selectedReceiptBundlePopupList;
	public List<string> receiptBundleNameList = new List<string>(), curCategories = new List<string>();
	public UIInput crName, crMonth, crDay, crPrice, ntbName, ntbYear,
	receiptBundleInput, receiptBundleNameInput, addCategoryInput;

	void Start()
	{
		StringBuilder sb = new StringBuilder(generateRandomReceipt());
		for (int i = 0; i < 300; i++) 
		{
			sb.Append(generateRandomReceipt());
		}
			
		receiptBundles.Add("RandomReceiptBundle:300", sb.ToString());

		sb = new StringBuilder(generateRandomReceipt());
		for (int i = 0; i < 200; i++)
		{
			sb.Append(generateRandomReceipt());
		}
		receiptBundles.Add("RandomReceiptBundle:200", sb.ToString());

		string str = "Ness 5 25 67.33\n" + "Ninten 2 26 68.33\n" + 
"Paula 8 25 12.00\n" + "Poo 4 25 90.60\n" + "Onett 10 25 134\n" + 
"Twoson 1 25 16.98\n" + "Fourside 12 25 27.45\n" + "Threed 1 25 17.98\n" + "Fourside 12 26 28.45\n";

		receiptBundles.Add("SampleReceiptBundle#1", str);
		receiptBundleNameList.Add("SampleReceiptBundle#1");
		receiptBundleNameList.Add("RandomReceiptBundle:200");
		receiptBundleNameList.Add("RandomReceiptBundle:300");
		receiptBundlePopupList.items = receiptBundleNameList;

	}



	void Update ()
	{
		//receiptBundlePopupList.
		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			crName.selectOnTab = null;
			crMonth.selectOnTab = crName.gameObject;
			crDay.selectOnTab = crMonth.gameObject;
			crPrice.selectOnTab = crDay.gameObject;

			ntbName.selectOnTab = null;
			ntbYear.selectOnTab = ntbName.gameObject;

		}
		else if(Input.GetKeyUp(KeyCode.LeftShift))
		{
			crName.selectOnTab = crMonth.gameObject;
			crMonth.selectOnTab = crDay.gameObject;
			crDay.selectOnTab = crPrice.gameObject;
			crPrice.selectOnTab = null;

			ntbName.selectOnTab = ntbYear.gameObject;
			ntbYear.selectOnTab = null;
		}
	}

	public string generateRandomReceipt() 
	{
		string name = "    Random       ";
		int month = Random.Range(1, 13);
		int day = Random.Range(1, 29);
		float price = Random.Range(0.1f, 100.0f);
		Receipt r = new Receipt(name, month, day, price);
		
		return r.toString();
	}
	public void saveReceiptBundle() 
	{
		string name = receiptBundleNameInput.value.Replace(" ", string.Empty);
		if (receiptBundles.ContainsKey(name))
		{
			receiptBundles[name] = receiptBundleInput.value;
		}
		else 
		{
			receiptBundles.Add(name, receiptBundleInput.value);
			receiptBundleNameList.Add(name);
		}
		receiptBundlePopupList.items = receiptBundleNameList;
	}

	public void loadReceiptBundle()
	{
		playButtonSound();

		// always save first
		saveReceiptBundle();

		if (receiptBundles[receiptBundlePopupList.value].Length > 10000)
		{
			receiptBundleInput.value = receiptBundles[receiptBundlePopupList.value].Substring(0, 10000);
		}
		else 
		{
			receiptBundleInput.value = receiptBundles[receiptBundlePopupList.value];
		}
		
		receiptBundleNameInput.value = receiptBundlePopupList.value;
		curSelectedReceiptBundle = receiptBundles[receiptBundlePopupList.value];
		receiptBundleNameInput.UpdateLabel();
		receiptBundleInput.UpdateLabel();
	}

	public void AddToReceiptBundle()
	{
		StringBuilder receipt = new StringBuilder(), helper = new StringBuilder();

		helper.Append(crName.value);
		helper.Append(" ");
		receipt.Append(helper);
		helper.Clear();

		helper.Append(crMonth.value);
		helper.Append(" ");
		receipt.Append(helper);
		helper.Clear();

		helper.Append(crDay.value);
		helper.Append(" ");
		receipt.Append(helper);
		helper.Clear();

		helper.Append(crPrice.value);
		helper.Append(" \n");
		receipt.Append(helper);
		helper.Clear();
		
		helper.Append(receiptBundleInput.value);
		if (!Regex.IsMatch(receipt.ToString(), @"^\s*$"))
		{
			helper.Append(receipt); 
		}
		
		receiptBundleInput.value = helper.ToString();
	}

	public void AddCategory() 
	{
		//addCategoryInput.value
	}

	public void AddReceiptBundleToCategory() 
	{
		//selectedReceiptBundlePopupList.value
		//categoryPopupList.value
	}

	public void SortReceiptBundle() 
	{
		saveReceiptBundle();
		string name = receiptBundleNameInput.value;
		string receipts = receiptBundles[receiptBundleNameInput.value];

		ReceiptBundle rb = ConvertToReceiptBundle(name, receipts);
		if (rb == null)
		{
			Debug.Log("Sort failed. Try it again.");
		}
		else 
		{
			float startTime = Time.realtimeSinceStartup;
			Sort(ref rb);
			Debug.Log("Time to sort: " + (Time.realtimeSinceStartup - startTime));
			sortSound.Stop();
			sortSound.Play();

			string rbStr = ConvertReceiptBundleToString(rb);

			receiptBundleInput.value = rbStr;
		}

	}

	public void Sort(ref ReceiptBundle rb) 
	{

		for (int i = 0; i < rb.receipts.Count; i++) 
		{
			for (int j = 0; j < rb.receipts.Count - 1; j++) 
			{
				if (rb.receipts[j].month > rb.receipts[j + 1].month) 
				{
					rb.SwapReceipts(j);
				}
				else if (rb.receipts[j].month == rb.receipts[j + 1].month) 
				{
					if (rb.receipts[j].day > rb.receipts[j + 1].day)
					{
						rb.SwapReceipts(j);
					}
				}
			}
		}

	}

	/*
	public int[] MergeSort(int[] numbers)
	{

		if (numbers.Length > 1)
		{
			int[] firsthalf = new int[numbers.Length / 2];
			for (int i = 0; i < firsthalf.Length; i++) 
			{
				firsthalf[i] = numbers[i];
			}
				
			int[] secondhalf = new int[numbers.Length - firsthalf.Length];
			for (int i = 0; i < secondhalf.Length; i++) 
			{
				secondhalf[i] = numbers[firsthalf.Length + i];
			}
				

			firsthalf = MergeSort(firsthalf);
			secondhalf = MergeSort(secondhalf);
			numbers = Merge(firsthalf, secondhalf);
		}
		return numbers;
	}

	public static int[] Merge(int[] first, int[] second)
	{

		int totalLength = first.Length + second.Length;
		int[] answer = new int[totalLength];
		int firstcounter = 0;
		int secondcounter = 0;

		for (int i = 0; i < answer.Length; i++)
		{

			// Deciding whether to take next smallest number from 
			// first array or second.
			if ((secondcounter == second.Length) || ((firstcounter < first.Length) && (first[firstcounter] < second[secondcounter])) )
			{
				answer[i] = first[firstcounter];
				firstcounter++;
			}
			else
			{
				answer[i] = second[secondcounter];
				secondcounter++;
			}
		}
		return answer;
	}*/

	public string ConvertReceiptBundleToString(ReceiptBundle rb) 
	{
		StringBuilder sb = new StringBuilder();
		int max = rb.receipts.Count;

		for (int i = 0; i < max; i++) 
		{
			sb.Append(rb.receipts[i].toString());
		}


		return sb.ToString();
	}

	public ReceiptBundle ConvertToReceiptBundle(string name, string receipts)
	{
		string[] splitInput = receipts.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		ReceiptBundle rb = new ReceiptBundle(name, new List<Receipt>());
		try
		{
			for (int i = 0; i < splitInput.Length; i++)
			{
			
				StringBuilder sb = new StringBuilder();
				bool spaceFound = false;
				for(int j = 0; j < splitInput[i].Length; j++)
				{
					if (splitInput[i][j] == ' ')
					{
						if (!spaceFound) 
						{
							sb.Append(splitInput[i][j]);
						} 
						spaceFound = true;
					}
					else 
					{
						spaceFound = false;
					}
					if (!spaceFound) 
					{
						sb.Append(splitInput[i][j]);
					}
				
				}
				if (sb[0] == ' ') 
				{
					sb = sb.Remove(0, 1);
				}
				//Debug.Log("sb is: ." + sb + ".");

				//string[] splitLine = Regex.Split(sb.ToString(), @" ");
				string[] splitLine = sb.ToString().Split(' ');
			
					string tName = splitLine[0];
					int tMonth = int.Parse(splitLine[1]);
					int tDay = int.Parse(splitLine[2]);
					float tPrice = float.Parse(splitLine[3]);
					Receipt r = new Receipt(tName, tMonth, tDay, tPrice);
					rb.AddReceipt(r);
			
			
			
			}
		}
		catch
		{
			//Debug.Log("at index: " + i);
			//splitLine.print();
			//Debug.Log("error sb is: ." + sb + ".");
			Debug.Log("error sorting!");
			rb = null;
		}
		return rb;
	}

	public void playButtonSound()
	{
		buttonSound.Stop();
		buttonSound.Play();
	}

	public void playAddSound()
	{
		addSound.Stop();
		addSound.Play();
	}

}

public struct Receipt
{
	public string name;
	public int month, day;
	public float price;

	public Receipt(string tName, int tMonth, int tDay, float tPrice)
	{
		name = tName;
		month = tMonth;
		day = tDay;
		price = tPrice;
	}

	public string toString() 
	{
		StringBuilder sb = new StringBuilder(name);
		
		sb.Append(" ");
		sb.Append(month);
		sb.Append(" ");
		sb.Append(day);
		sb.Append(" ");
		sb.Append(price);
		sb.Append("\n");

		return sb.ToString();
	}

}

public class ReceiptBundle
{
	public string name;
	public List<Receipt> receipts;

	public ReceiptBundle(string tName, List<Receipt> tReceipts)
	{
		name = tName;
		receipts = tReceipts;
	}

	public void AddReceipt(string tName, int tMonth, int tDay, float tPrice)
	{
		receipts.Add(new Receipt(tName, tMonth, tDay, tPrice));
	}

	public void AddReceipt(Receipt r)
	{
		receipts.Add(r);
	}

	public int[] GetMonths() 
	{
		int[] arr = new int[receipts.Count];

		for (int i = 0; i < arr.Length; i++) 
		{
			arr[i] = receipts[i].month;
		}

		return arr;
	}

	public int[] GetDays() 
	{
		int[] arr = new int[receipts.Count];
		for (int i = 0; i < arr.Length; i++) 
		{
			arr[i] = receipts[i].day;
		}

		return arr;
	}

	public void SwapReceipts(int index) 
	{
		Receipt temp = receipts[index];
		receipts[index] = receipts[index + 1];
		receipts[index + 1] = temp;
	}

}
