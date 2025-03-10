using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewUIManager : MonoBehaviour
{
	[ SerializeField ] private List<BaseScreen> allScreens = new List<BaseScreen>();
	private List<BaseScreen> activeScreens = new List<BaseScreen>();

	private void Start()
	{
		ValidateScreenTypes();

		BaseScreen alwaysOnUI = allScreens.Find( s => s.screenType == ScreenType.AlwaysOn );
		if ( alwaysOnUI != null )
		{
			alwaysOnUI.ShowScreen();
			alwaysOnUI.InitializeScreen();
			activeScreens.Add( alwaysOnUI );
		}
	}

	private void Update()
	{
		foreach ( BaseScreen screen in activeScreens )
		{
			screen.UpdateScreen();
		}
	}

	private void ValidateScreenTypes()
	{
		HashSet<ScreenType> existingScreenTypes = new HashSet<ScreenType>();

		foreach ( BaseScreen screen in allScreens )
		{
			bool addSuccessful = existingScreenTypes.Add( screen.screenType );

			if ( !addSuccessful )
			{
				Debug.LogError( $"Duplicate screen type found: {screen.screenType} in {gameObject.name}" );
			}
		}
	}
}