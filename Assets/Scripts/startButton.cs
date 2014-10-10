using UnityEngine;
using System.Collections;



public class startButton : MonoBehaviour 
{
	
	void Start () 
	{
	}
	
	void Update () 
	{
		
	}
	
	void OnMouseOver () 
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			Debug.Log ("FBLogoutButton - left click.");
			
			GameObject _fuelHandler = GameObject.Find("FuelHandlerObject");
			FuelHandler _fuelHandlerScript = _fuelHandler.GetComponent<FuelHandler>();

			
			_fuelHandlerScript.LogoutButtonPressed();
		}
	}
	
	
}
