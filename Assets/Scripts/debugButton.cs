using UnityEngine;
using System.Collections;

public class debugButton : MonoBehaviour 
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
			GameObject _fuelHandler = GameObject.Find("FuelHandlerObject");
			FuelHandler _fuelHandlerScript = _fuelHandler.GetComponent<FuelHandler>();
			
			_fuelHandlerScript.launchPropeller();
		}
	}
	
}