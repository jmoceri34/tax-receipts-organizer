using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class ExtensionMethods
{

	public static string toString(this List<string> list)
	{
		StringBuilder str = new StringBuilder();
		for(int i = 0; i < list.Count; i++)
		{
			StringBuilder helper = new StringBuilder();
			helper.Append(list[i]);
			helper.Append("\n");
			str.Append(helper);
		}
		return str.ToString();
	}

	public static void Clear(this StringBuilder sb)
	{
		sb.Length = 0;
		sb.Capacity = 16;
	}

	public static void print(this List<string> list)
	{
		for(int i = 0; i < list.Count; i++)
		{
			Debug.Log(list[i]);
		}
	}

	public static void print(this ReceiptBundle rb) 
	{
		for (int i = 0; i < rb.receipts.Count; i++) 
		{
			Debug.Log("Start Receipt:");
			Debug.Log("Name: " + rb.receipts[i].name);
			Debug.Log("Month: " + rb.receipts[i].month);
			Debug.Log("Day: " + rb.receipts[i].day);
			Debug.Log("Price: " + rb.receipts[i].price);
			Debug.Log("End Receipt:");
		}
	}

	public static void printJustMonths(this ReceiptBundle rb) 
	{
		for (int i = 0; i < rb.receipts.Count; i++) 
		{
			Debug.Log("Start Month:");
			Debug.Log("Month: " + rb.receipts[i].month);
			Debug.Log("End Month:");
		}
	}

	public static void print(this string[] str) 
	{
		for (int i = 0; i < str.Length; i++) 
		{
			Debug.Log(str[i]);
		}
	}

}
