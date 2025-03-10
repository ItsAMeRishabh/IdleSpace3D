using System;
using UnityEngine;

public enum ScreenType
{
	None,
	AlwaysOn,
	IridiumClick
}

public abstract class BaseScreen : MonoBehaviour
{
	public ScreenType screenType = ScreenType.None;

	private void Awake()
	{
		if ( screenType == ScreenType.None )
		{
			Debug.LogError( $"Unassigned screen type detected on {gameObject.name}!" );
		}
	}

	public virtual void ShowScreen()
	{
		gameObject.SetActive( true );

		InitializeScreen();
	}

	public abstract void InitializeScreen();
	public abstract void UpdateScreen();

	public virtual void HideScreen()
	{
		gameObject.SetActive( false );
	}
}