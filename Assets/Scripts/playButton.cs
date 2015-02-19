using UnityEngine;
using System.Collections;

public class playButton : MonoBehaviour 
{
	public Sprite imgDown;
	public AudioSource clickSound;

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
			Debug.Log ("playButton - left click.");

			clickSound.Play();
			
			GameObject _fuelHandler = GameObject.Find("FuelHandlerObject");
			FuelHandler _fuelHandlerScript = _fuelHandler.GetComponent<FuelHandler>();

			gameObject.GetComponent<SpriteRenderer>().sprite = imgDown;

			_fuelHandlerScript.launchSinglePlayerGame();
		}
	}
}