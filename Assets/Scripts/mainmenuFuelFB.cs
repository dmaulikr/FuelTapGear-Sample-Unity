using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Facebook.MiniJSON;
using SimpleJSON;


public struct GameMatchData 
{
	private bool matchDataReady;
	private bool matchComplete;
	private string tournamentID;
	private string matchID;
	private int matchType;
	private int matchRound;
	private int matchScore;
	private string yourNickname;
	private string yourAvatarURL;
	private string theirNickname;
	private string theirAvatarURL;
	
	public bool MatchDataReady 
	{	
		get { return matchDataReady; }	
		set { matchDataReady = value; }
	}
	public bool MatchComplete 
	{	
		get { return matchComplete; }	
		set { matchComplete = value; }
	}
	public string TournamentID 
	{	
		get { return tournamentID; }	
		set { tournamentID = value; }
	}
	public string MatchID 
	{	
		get { return matchID; }	
		set { matchID = value; }
	}
	public int MatchType 
	{	
		get { return matchType; }	
		set { matchType = value; }
	}
	public int MatchRound 
	{	
		get { return matchRound; }	
		set { matchRound = value; }
	}
	public int MatchScore 
	{	
		get { return matchScore; }	
		set { matchScore = value; }
	}
	public string YourNickname 
	{	
		get { return yourNickname; }	
		set { yourNickname = value; }
	}
	public string YourAvatarURL 
	{	
		get { return yourAvatarURL; }	
		set { yourAvatarURL = value; }
	}
	public string TheirNickname 
	{	
		get { return theirNickname; }	
		set { theirNickname = value; }
	}
	public string TheirAvatarURL 
	{	
		get { return theirAvatarURL; }	
		set { theirAvatarURL = value; }
	}	
}





public class mainmenuFuelFB : MonoBehaviour 
{
	public static mainmenuFuelFB Instance { get; private set; }

	private fuelSDKListener m_listener;
	private GameMatchData m_matchData;

	public enum eFBState 
	{
		WaitForInit, 
		DataRetrived,
		LaunchWithResults,
		SystemSettled
	};
	public eFBState mFBState = eFBState.WaitForInit;

	public string get_data; 
	public string fbname;
	public string fbfirstname;//use this as nickname for now
	public string fbemail;
	public string fbgender;
	private bool fbdata_ready; 


	public const int MATCH_TYPE_SINGLE = 0;
	public const int MATCH_TYPE_MULTI = 1;


	public GameMatchData getMatchData()
	{
		return m_matchData;
	}

	public void tryLaunchFuelSDK()
	{
		if (m_matchData.MatchComplete == true && m_matchData.MatchType == MATCH_TYPE_MULTI) 
		{
			LaunchDashBoardWithResults();
		}
	}


	public void launchSinglePlayerGame()
	{
		m_matchData.MatchType = MATCH_TYPE_SINGLE;
		m_matchData.MatchDataReady = false;

		Application.LoadLevel("GamePlay");
	}

	public void LaunchMultiplayerGame(Dictionary<string, string> matchResult)
	{
		m_matchData.MatchType = MATCH_TYPE_MULTI;

		m_matchData.MatchDataReady = true;

		m_matchData.TournamentID = matchResult ["tournamentID"];
		m_matchData.MatchID = matchResult ["matchID"];

		// extract the params data
		string paramsJSON = matchResult ["paramsJSON"];
		JSONNode json = JSONNode.Parse (paramsJSON);

		m_matchData.MatchRound = json ["round"].AsInt;

		JSONClass you = json ["you"].AsObject;
		m_matchData.YourNickname = you ["name"];
		m_matchData.YourAvatarURL = you ["avatar"];

		JSONClass them = json ["them"].AsObject;
		m_matchData.TheirNickname = them ["name"];
		m_matchData.TheirAvatarURL = them ["avatar"];

		Debug.Log (	"__LaunchMultiplayerGame__" + "\n" +
		           "MatchDataReady = " + m_matchData.MatchDataReady + "\n" +
		           "TournamentID = " + m_matchData.TournamentID + "\n" +
		           "MatchID = " + m_matchData.MatchID + "\n" +
		           "MatchRound = " + m_matchData.MatchRound + "\n" +
		           "adsAllowed = " + "\n" +
		           "fairPlay = " + "\n" +
		           "YourNickname = " + m_matchData.YourNickname + "\n" +
		           "YourAvatarURL = " + m_matchData.YourAvatarURL + "\n" +
		           "TheirNickname = " + m_matchData.TheirNickname + "\n" +
		           "TheirAvatarURL = " + m_matchData.TheirAvatarURL + "\n" 
		           );


		m_matchData.MatchComplete = false;

		Application.LoadLevel("GamePlay");
	}
	
	private void sendMatchResult (long score)
	{
		Debug.Log ("sendMatchResult");
		
		Dictionary<string, object> matchResult = new Dictionary<string, object> ();
		matchResult.Add ("tournamentID", m_matchData.TournamentID);
		matchResult.Add ("matchID", m_matchData.MatchID);
		matchResult.Add ("score", m_matchData.MatchScore);
		
		PropellerSDK.LaunchWithMatchResult (matchResult, m_listener);	
	}
	

	/*
	 * Functions called from other scripts
	*/
	public void launchPropeller ()
	{
		Debug.Log ("launchPropeller");

		if (m_listener == null) 
		{
			throw new Exception();
		}
		
		PropellerSDK.Launch (m_listener);
	}

	/* Debug, Deprecated */
	public void generateScore ()
	{
		Debug.Log ("LaunchWithMatchResult - generateScore");

		if (m_listener == null) 
		{
			throw new Exception();
		}
		
		long score = (long)UnityEngine.Random.Range (0.0F, 50.0f);
		
		sendMatchResult (score);
	}
	
	public void updateLoginText (string str) 
	{
		GameObject gameObj = GameObject.Find ("LoginStatusText");
		if (gameObj) 
		{
			TextMesh tmesh = (TextMesh)gameObj.GetComponent (typeof(TextMesh)); 
			tmesh.text = str;
		}
	}
	
	
	public void StuffScore(int scoreValue)
	{
		Debug.Log ("StuffScore = " + scoreValue);

		m_matchData.MatchScore = scoreValue;

		//hmmm, better clean up this check
		if (m_matchData.MatchDataReady == true) 
		{
			m_matchData.MatchComplete = true;
		}
	}




	/*
	 * Awake
	*/
	void Awake ()
	{
		if (Instance != null && Instance != this) 
		{
			//destroy other instances
			Destroy (gameObject);
		} 
		else if( Instance == null )
		{
			m_listener = new fuelSDKListener ();
			if(m_listener == null) 
			{
				throw new Exception();
			}

			m_matchData = new GameMatchData ();
			m_matchData.MatchDataReady = false;
			m_matchData.MatchComplete = false;

			// Initialize FB SDK              
			FB.Init(SetInit, OnHideUnity);  

			fbdata_ready = false;

		}
		
		Instance = this;
		
		DontDestroyOnLoad(gameObject);



		//this init doesn't seem to work when you come in a second time (across scenes)
		/*
		if (false)//m_bInitialized == false) 
		{
			Debug.Log ("<----- Awake ----->");
			
			GameObject.DontDestroyOnLoad (gameObject);


			if (m_listener != null) 
			{
				throw new Exception();
			}
			
			if (!Application.isEditor) 
			{
				m_listener = new fuelSDKListener ();
			}
			
			if (m_listener == null) 
			{
				throw new Exception();
			}

			// Initialize FB SDK              
			FB.Init(SetInit, OnHideUnity);  

			fbdata_ready = false;
			matchdata_ready = false;

			m_listener.m_matchStatus = 0;

			
			m_bInitialized = true;


			Debug.Log ("<----- Awake Done ----->");
		} 
		else 
		{
			//not sure whay you would do this
			//GameObject.Destroy (gameObject);
		}
		*/


	}
	
	
	
	/*
	 * Start
	*/
	void Start () 
	{
		Debug.Log ("<----- Start ----->");

		PropellerSDK.SyncChallengeCounts ();

		PropellerSDK.SyncTournamentInfo ();


		// enable push notifications
		PropellerSDK.EnableNotification ( PropellerSDK.NotificationType.push );
		
		// disable local notifications
		//PropellerSDK.DisableNotification ( PropellerSDK.NotificationType.local );
		
		// validate enabled notifications - result is false since local notifications are disabled
		bool fuelEnabled = PropellerSDK.IsNotificationEnabled( PropellerSDK.NotificationType.all );	
		if (fuelEnabled) 
		{
			Debug.Log ("fuelEnabled NotificationEnabled!");
		}

		Debug.Log ("<----- Start Done ----->");

	}
	
	public void LaunchDashBoardWithResults()
	{
		sendMatchResult (m_matchData.MatchScore);
	}

	
	/*
	 * Update
	*/
	void Update () 
	{

		//change this to a message
		switch (mFBState) 
		{
			case eFBState.WaitForInit:
					if (fbdata_ready) 
					{
						PushFBDataToFuel();	
						mFBState = eFBState.DataRetrived;
					}
			break;
			
			case eFBState.DataRetrived:
				break;


			case eFBState.LaunchWithResults:
				break;

			case eFBState.SystemSettled:

				break;

			
		}
		//-------------------------

	}


	
	
	
	
	public void OnPropellerSDKChallengeCountUpdated (string count)
	{
		int countInt;
		
		if (!int.TryParse(count, out countInt))
		{
			return;
		}
		
		// Update the UI with the count
	}
	
	public void OnPropellerSDKTournamentInfo (Dictionary<string, string> tournamentInfo)
	{
		if ((tournamentInfo == null) || (tournamentInfo.Count == 0)) 
		{
			// there is no tournament currently running
		} 
		else 
		{
			//string name = tournamentInfo["name"];
			//string campaignName = tournamentInfo["campaignName"];
			//string sponsorName = tournamentInfo["sponsorName"];
			//string startDate = tournamentInfo["startDate"];
			//string endDate = tournamentInfo["endDate"];
			//string logo = tournamentInfo["logo"];
		}
		
		// Update the UI with the tournament information
	}
	
	void OnApplicationPause(bool paused)
	{
		// application entering background

		if (paused) 
		{
			#if UNITY_IPHONE

			NotificationServices.ClearLocalNotifications ();
			NotificationServices.ClearRemoteNotifications ();

			#endif
		}

	}
	
	
	
	
	
	
	
	/*
	 -----------------------------------------------------
				mainmenu-class	FACEBOOK Stuff
	 -----------------------------------------------------
	*/
	
	public void LoginButtonPressed()
	{

		if (!FB.IsLoggedIn) 
		{                                                                                                                
			FB.Login("email, publish_actions", LoginCallback); 
			//FB.Login ("public_profile, user_friends, email, publish_actions", LoginCallback); 
		}
		else
		{
			//FB.Logout();

			updateLoginText("Already Logged In");
		}

		
	}
	
	void LoginCallback(FBResult result)                                                        
	{                                                                                          
		Debug.Log("___LoginCallback___");                                                          
		

		if (FB.IsLoggedIn)                                                                     
		{                                                                                      
			OnLoggedIn();                                                                      
		}  
		                                                                                  
	}                                                                                          
	
	void OnLoggedIn()                                                                          
	{        
		//get additional data from facebook

		FB.API("me?fields=name,email,gender,first_name", Facebook.HttpMethod.GET, UserCallBack);

	}  
	
	
	
	void UserCallBack(FBResult result) 
	{

		if (result.Error != null)
		{
			get_data = result.Text;
		}
		else
		{
			get_data = result.Text;
		}


		var dict = Json.Deserialize(get_data) as IDictionary;
		fbname = dict ["name"].ToString();
		fbemail = dict ["email"].ToString();
		fbgender = dict ["gender"].ToString();
		fbfirstname = dict ["first_name"].ToString();


		fbdata_ready = true;	

	}
	

	
	public void PushFBDataToFuel()                                                                       
	{
		string provider = "facebook";
		string email = fbemail;
		string id = FB.UserId;
		string token = FB.AccessToken;
		string nickname = fbfirstname;//not available from FB using first name
		string name = fbname;
		string gender = fbgender;
		
		
		Dictionary<string, string> loginInfo = null;
		loginInfo = new Dictionary<string, string> ();
		loginInfo.Add ("provider", provider);
		loginInfo.Add ("email", email);
		loginInfo.Add ("id", id);
		loginInfo.Add ("token", token);
		loginInfo.Add ("nickname", nickname);
		loginInfo.Add ("name", name);
		loginInfo.Add ("gender", gender);
		
		Debug.Log ("__PushFBDataToFuel__" + "\n" +
		           "*** loginInfo ***" + "\n" +
				   "provider = " + loginInfo ["provider"].ToString () + "\n" +
		           "email = " + loginInfo ["email"].ToString () + "\n" +
		           "id = " + loginInfo ["id"].ToString () + "\n" +
		           "token = " + loginInfo ["token"].ToString () + "\n" +
		           "nickname = " + loginInfo ["nickname"].ToString () + "\n" +
		           "name = " + loginInfo ["name"].ToString () + "\n" +
		           "gender = " + loginInfo ["gender"].ToString () + "\n");
		
		PropellerSDK.SdkSocialLoginCompleted (loginInfo);

	}
	
	
	
	/*
	 -----------------------------------------------------
	 				FaceBook Init - private
	 -----------------------------------------------------
	*/
	private void SetInit()                                                                       
	{                                                                                            
		Debug.Log("Facebook SetInit"); 
		                                                                 
		if (FB.IsLoggedIn)                                                                       
		{                                                                                        
			Debug.Log("....Already logged in");
			
			//set onscreen button text
			updateLoginText ("Logged In");
			
			OnLoggedIn();
			
		}   
		                                                                                     
	} 
	
	
	
	
	private void OnHideUnity(bool isGameShown)                                                   
	{                                                                                            
		Debug.Log("Facebook OnHideUnity");    
		 
		                                                         
		if (!isGameShown)                                                                        
		{                                                                                        
			// pause the game - we will need to hide                                             
			Time.timeScale = 0;                                                                  
		}                                                                                        
		else                                                                                     
		{                                                                                        
			// start the game back up - we're getting focus again                                
			Time.timeScale = 1;                                                                  
		}

		                                                                                       
	}    
	
	
	
	
	/*
	 -----------------------------------------------------
	 				FaceBook Invite
	 -----------------------------------------------------
	*/

	public void onSocialInviteClicked(Dictionary<string, string> inviteInfo)                                                                                              
	{ 
		Debug.Log("onSocialInviteClicked");  
		/*
			string message,
			string[] to = null,
			List<object> filters = null,
			string[] excludeIds = null,
			int? maxRecipients = null,
			string data = "",
			string title = "",
			FacebookDelegate callback = null)
		*/
			
		FB.AppRequest ("Come On!", 
		               null, 
		               null, 
		               null, 
		               null, 
		               "Some Data", 
		               "Some Title", 
		               appRequestCallback);
		                                                                                                            
		
	}                                                                                                                              
	private void appRequestCallback (FBResult result)                                                                              
	{     

		Debug.Log("appRequestCallback");  
		                                                                                       
		if (result != null)                                                                                                        
		{    
			
			var responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;                                      
			object obj = 0;                                                                                                        
			if (responseObject.TryGetValue ("cancelled", out obj))                                                                 
			{                                                                                                                      
				Debug.Log("Request cancelled");                                                                                  
			}                                                                                                                      
			else if (responseObject.TryGetValue ("request", out obj))                                                              
			{                
				//AddPopupMessage("Request Sent", ChallengeDisplayTime);
				Debug.Log("Request sent");                                                                                       
			} 

			PropellerSDK.SdkSocialInviteCompleted();

			
		}  

	}  



	/*
	 -----------------------------------------------------
	 				FaceBook Share
	 -----------------------------------------------------
	*/
	
	public void onSocialShareClicked(Dictionary<string, string> inviteInfo)                                                                                              
	{ 
		Debug.Log("onSocialShareClicked");  
		/*
            string toId = "",
            string link = "",
            string linkName = "",
            string linkCaption = "",
            string linkDescription = "",
            string picture = "",
            string mediaSource = "",
            string actionName = "",
            string actionLink = "",
            string reference = "",
            Dictionary<string, string[]> properties = null,
            FacebookDelegate callback = null)
		*/
		
		FB.Feed (FB.UserId, 
		         "", 
		         "", 
		         "", 
		         "", 
		         "", 
		         "", 
		         "", 
		         "", 
		         "", 
		         null,
		         appFeedCallback);
		
		
	}                                                                                                                              
	private void appFeedCallback (FBResult result)                                                                              
	{     
		
		Debug.Log("appFeedCallback");  
		
		if (result != null)                                                                                                        
		{    
			
			var responseObject = Json.Deserialize(result.Text) as Dictionary<string, object>;                                      
			object obj = 0;                                                                                                        
			if (responseObject.TryGetValue ("cancelled", out obj))                                                                 
			{                                                                                                                      
				Debug.Log("Request cancelled");                                                                                  
			}                                                                                                                      
			else if (responseObject.TryGetValue ("request", out obj))                                                              
			{                
				//AddPopupMessage("Request Sent", ChallengeDisplayTime);
				Debug.Log("Request sent");                                                                                       
			} 
			
			PropellerSDK.SdkSocialShareCompleted();

		}  
		
	}  

	
	
}




