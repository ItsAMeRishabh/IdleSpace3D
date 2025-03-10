using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlwaysOnUI : BaseScreen
{
	[ SerializeField ] private TMP_Text iridiumText;
	[ SerializeField ] private TMP_Text darkElixirText;

	private int counter = 0;

	public override void InitializeScreen()
	{
		iridiumText.text = "200";
	}

	public override void UpdateScreen()
	{
		darkElixirText.text = counter.ToString();
		counter++;
	}
}