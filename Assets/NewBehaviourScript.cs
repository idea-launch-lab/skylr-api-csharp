using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using InstrumentationLib;

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// New behaviour script is a class to test DataLogger API.
/// </summary>
public class NewBehaviourScript : MonoBehaviour {

	static string mode = "DOCUMENT";
	static int LoggingDensity = 5;
	string myDocPath = "";

	// Use this for initialization
	void Start () {
		Debug.LogError ("Ready");

		myDocPath = "C:\\Users\\sthakur\\Documents\\00_Projects\\1_LAS\\code\\DataLogger\\data";
		Debug.LogError("my doc path: " + myDocPath);

		Application.runInBackground = true;
	}

	// Update is called once per frame
	void Update () {

		//var random = new System.Random (System.DateTime.Now.Millisecond);
		var datetime = "" + (Math.Floor(System.DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds)).ToString();

		// Mouse presses
		if (Input.GetMouseButtonUp (0) || Input.GetMouseButtonUp (1) || Input.GetMouseButtonUp (2) )
		{
			string mouseButton = "";
			
			if(Input.GetMouseButtonUp (0) )
				mouseButton = "left_button";
			else if(Input.GetMouseButtonUp (1) )
				mouseButton = "right_button";
			else if(Input.GetMouseButtonUp (2) )
				mouseButton = "middle_button";

			string postData = "{" + 
				"\"content\": " + 
					"{  \"UserId\" : \"sthakur\", \"AppName\" : \"unity3D\", \"SysId\" : \"WIN64\", " + "\"ProjId\" : \"LAS/Instrumentation\", " + " \"EvtTime\" : " + datetime + " , \"NetAddr\" : \"152.14.3.32\" , \"GeoLoc\" : \"RALEIGH NC\" , " + " \"EvtType\" : \"mouse_click\" , \"EvtDesc\" : \"" + mouseButton  + "\" , \"EvtCoords\" : " + "{ \"x\" : " + Input.mousePosition.x + " , \"y\" : " + Input.mousePosition.y + " , \"z\" : " + Input.mousePosition.z + " } " + ", \"evt_msg\" : \"hello world\"" + " }" + 
					"}";

			DumpString(postData);

			LogData (postData);
		}

		// Key presses
		
		if( Input.GetKeyUp (KeyCode.Alpha1) ) {
			LoggingDensity = 1;
			Debug.LogError("response density: none");
		}
		else if( Input.GetKeyUp (KeyCode.Alpha2) ) {
			LoggingDensity = 2;
			Debug.LogError("response level: low");
		}
		else if( Input.GetKeyUp(KeyCode.Alpha3) ) {
			LoggingDensity = 3;
			Debug.LogError("response level: medium");
		}
		else if( Input.GetKeyUp(KeyCode.Alpha4) ) {
			LoggingDensity = 4;
			Debug.LogError("response level: high");
		}
		else if( Input.GetKeyUp(KeyCode.Alpha5) ) {
			LoggingDensity = 5;
			Debug.LogError("response level: all");
		}

		if( Input.GetKeyUp (KeyCode.B)) {
			mode = "BINARY";
			Debug.LogError("mode: " + mode);
			string postData = "{" + 
				"\"content\": [" + 
					"{  \"UserId\" : \"sthakur\", \"AppName\" : \"unity3D\", \"SysId\" : \"WIN64\", " + "\"ProjId\" : \"LAS/Instrumentation\", " + " \"EvtTime\" : " + datetime + " , \"NetAddr\" : \"152.14.3.32\" , \"GeoLoc\" : \"RALEIGH NC\" , " + " \"EvtType\" : \"key_press\" , \"EvtDesc\" : \"" + KeyCode.B  + "\" , \"EvtCoords\" : " + "{ \"x\" : " + Input.mousePosition.x + " , \"y\" : " + Input.mousePosition.y + " , \"z\" : " + Input.mousePosition.z + " } "  + " }" + 
					"]}";
			DumpString (postData);
			LogData (postData);
		}
		else if( Input.GetKeyUp (KeyCode.D)) {
			mode = "DOCUMENT";
			Debug.LogError("mode: " + mode);
			string postData = "{" + 
				"\"content\": [" + 
					"{  \"UserId\" : \"sthakur\", \"AppName\" : \"unity3D\", \"SysId\" : \"WIN64\", " + "\"ProjId\" : \"LAS/Instrumentation\", " + " \"EvtTime\" : " + datetime + " , \"NetAddr\" : \"152.14.3.32\" , \"GeoLoc\" : \"RALEIGH NC\" , " + " \"EvtType\" : \"key_press\" , \"EvtDesc\" : \"" + KeyCode.D  + "\" , \"EvtCoords\" : " + "{ \"x\" : " + Input.mousePosition.x + " , \"y\" : " + Input.mousePosition.y + " , \"z\" : " + Input.mousePosition.z + " } "  + " }" + 
					"]}";
			DumpString (postData);
			LogData (postData);
		}
		else if( Input.GetKeyUp (KeyCode.F)) {
			mode = "FILE";
			Debug.LogError("mode: " + mode);
			string postData = "{" + 
				"\"content\": [" + 
					"{  \"UserId\" : \"sthakur\", \"AppName\" : \"unity3D\", \"SysId\" : \"WIN64\", " + "\"ProjId\" : \"LAS/Instrumentation\", " + " \"EvtTime\" : " + datetime + " , \"NetAddr\" : \"152.14.3.32\" , \"GeoLoc\" : \"RALEIGH NC\" , " + " \"EvtType\" : \"key_press\" , \"EvtDesc\" : \"" + KeyCode.F  + "\" , \"EvtCoords\" : " +  "{ \"x\" : " + Input.mousePosition.x + " , \"y\" : " + Input.mousePosition.y + " , \"z\" : " + Input.mousePosition.z + " } "  + " }" + 
					"]}";
			DumpString (postData);
			LogData (postData);
		}
		else if( Input.GetKeyUp (KeyCode.M)) {
			mode = "MESSAGE";
			Debug.LogError("mode: " + mode);
			string postData = "{" + 
				"\"content\": [" + 
					"{  \"UserId\" : \"sthakur\", \"AppName\" : \"unity3D\", \"SysId\" : \"WIN64\", " + "\"ProjId\" : \"LAS/Instrumentation\", " + " \"EvtTime\" : " + datetime + " , \"NetAddr\" : \"152.14.3.32\" , \"GeoLoc\" : \"RALEIGH NC\" , " + " \"EvtType\" : \"key_press\" , \"EvtDesc\" : \"" + KeyCode.M  + "\" , \"EvtCoords\" : " + "{ \"x\" : " + Input.mousePosition.x + " , \"y\" : " + Input.mousePosition.y + " , \"z\" : " + Input.mousePosition.z + " } "  + " }" + 
					"]}";
			DumpString (postData);
			LogData (postData);
		}

	}

	void LogData (string postData) {

		if(mode == "DOCUMENT" ) {

			DumpString (postData);

			Debug.LogError ("mode: " + mode);
			Debug.LogError ("Sending data: " + postData);

			// Write out string to local file

			DataLogger logger = new DataLogger ();

			//uri = "http://skylr.renci.org/api/data/document/add";
			logger.SetResponseDensity(LoggingDensity);

			ResponseObject R = logger.LogData (postData, false);
			Debug.LogError (R.Response);
			
		}
		else if( mode == "MESSAGE" ) {
			string message = "Hello World";
			DataLogger logger = new DataLogger ();
			
			Debug.LogError ("mode: " + mode);
			Debug.LogError ("Sending message: " + message);
			
			//uri = "http://skylr.renci.org/api/data/messageQ/add";
			logger.SetResponseDensity(LoggingDensity);
			
			ResponseObject R = logger.LogData (message, true);
			
			Debug.LogError (R.Response);
		}
		else if(mode == "FILE" ) {

			Debug.LogError ("mode: " + mode);
			Debug.LogError ("Sending data: " + postData);
			
			DataLogger logger = new DataLogger ();
			
			//uri = "http://skylr.renci.org/api/data/file/add";
			logger.SetResponseDensity(LoggingDensity);
			
			ResponseObject R = logger.LogFile (postData, "C:\\file.txt");
			Debug.LogError (R.Response);
			
		}
		else if( mode == "BINARY" ) {
			byte[] byteArray = StrToBytes ("Hello World");
			DataLogger logger = new DataLogger ();

			Debug.LogError ("mode: " + mode);

			//uri = "http://skylr.renci.org/api/data/file/addBinary";
			logger.SetResponseDensity(LoggingDensity);

			ResponseObject R = logger.LogBinaryFile (byteArray);

			Debug.LogError (R.Response);
		}

	}

	/// <summary>
	/// Strings to bytes.
	/// </summary>
	/// <returns>The to bytes.</returns>
	/// <param name="str">String.</param>
	/// <value>Obtained from http://stackoverflow.com/questions/5173033/string-serialization-and-deserialization-problem</value>
	private byte[] StrToBytes(string str)
	{
		BinaryFormatter bf = new BinaryFormatter();
		
		MemoryStream ms = new MemoryStream();
		bf.Serialize(ms, str);
		ms.Seek(0, 0);
		return ms.ToArray();
	}

	/// <summary>
	/// Write string to file
	/// </summary>
	private void DumpString(string str)
	{
		string path = myDocPath + @"\data.txt";
		File.AppendAllText (path, (str + System.Environment.NewLine));
	}

}
