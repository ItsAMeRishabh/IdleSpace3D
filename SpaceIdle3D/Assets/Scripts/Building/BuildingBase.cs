using System;
using System.Collections;
using UnityEngine;

public abstract class BuildingBase : MonoBehaviour
{
	[ SerializeField ] private float clickTimeout = 0.2f;
	private WaitForSeconds clickWait;
	private Coroutine clickTimeoutCoroutine;
	private bool clickExpired = false;
	private bool clickedOnMe = false;

	public event Action<BuildingBase> ClickedOnBuilding;

	public virtual void Initialize()
	{
		clickWait = new WaitForSeconds( clickTimeout );
	}

	private void OnMouseDown()
	{
		if ( DetectClickOnUI.IsPointerOverUIElement() )
		{
			return;
		}

		if ( clickTimeoutCoroutine != null )
		{
			StopCoroutine( clickTimeoutCoroutine );
			clickTimeoutCoroutine = null;
		}

		clickTimeoutCoroutine = StartCoroutine( ClickTimeoutCoroutine() );
		clickedOnMe = true;
	}

	private void OnMouseUp()
	{
		if ( !clickExpired && clickedOnMe )
		{
			ClickedOnBuilding?.Invoke( this );
		}

		clickedOnMe = false;
	}

	private IEnumerator ClickTimeoutCoroutine()
	{
		clickExpired = false;

		yield return clickWait;

		clickExpired = true;
	}
}