//------------------------------------------------------------------------------
//	Sylr API C#
//	
//	Class: DataLogger
// 	DataLogger is a class to log data. Client applications interface with a 
//	back end logging server using this library.
//------------------------------------------------------------------------------

// Namespaces defined in .NET
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using UnityEngine;


// Namespace of this library
namespace InstrumentationLib
{

	
	/// <summary>
	/// Logging response levels
	/// </summary>
	/// <value>The logging response level.</value>
	enum LoggingResponseDensity : int {NONE=1, LOW, MEDIUM, HIGH, ALL};
	
	
	///<summary>Types of logged data.</summary>
	///<value>Types of data that can be logged.</value>
	enum LoggingDataType : int {DOCUMENT=1, FILE, BINARY, MESSAGE};


	/// <summary>
	/// Data Logging Library Class.
	/// </summary>
	public class DataLogger : MonoBehaviour
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InstrumentationLib.DataLogger"/> class.
		/// </summary>
		public DataLogger () {
			ResponseDensity = (int)LoggingResponseDensity.ALL;
		}



		/// <summary>
		/// The response level.
		/// </summary>
		/// <value>Density of information returned by server. Density can be one of following: NONE, LOW, MEDIUM, HIGH, ALL.</value>
		private int ResponseDensity;



		/// <summary>
		/// Sets the density of responses
		/// </summary>
		/// <param name="DensityLevel">Response density level specified by user application. Value ranges from 0 - 4 and represent one of following: NONE (0), LOW (1), MEDIUM (2), HIGH (3), ALL (4).</param>
		public void SetResponseDensity (int DensityLevel) {

			switch (DensityLevel) {
				case 1: ResponseDensity = (int)LoggingResponseDensity.NONE; break;
				case 2: ResponseDensity = (int)LoggingResponseDensity.LOW; break;
				case 3: ResponseDensity = (int)LoggingResponseDensity.MEDIUM; break;
				case 4: ResponseDensity = (int)LoggingResponseDensity.HIGH; break;
				case 5: ResponseDensity = (int)LoggingResponseDensity.ALL; break;
			}

		}



		/// <summary>
		/// Logs data to server.
		/// </summary>
		/// <returns>A response object that contains response string and response type that are set by server.</returns>
		/// <param name="data">JSON string data.</param>
		/// <param name="isMessage">Flag data as message.</param> 
		/// <value>Logs data to server. Internally, LogData calls functions to log json data or string message. Depending on type of data the input data are sent to different URIs.</value>
		public ResponseObject LogData ( string data, bool isMessage = false ) {

			ResponseObject R = new ResponseObject();
			string uri = "";

			if ( isMessage == false ) {
				uri = "http://skylr.renci.org/api/data/document/add";
				R = ( LogEvent (uri, data) );
			}
			else if ( isMessage == true ) {
				uri = "http://skylr.renci.org/api/data/messageQ/add";
				R = ( LogMessage (uri, data) );
			}
			return R;
		}



		/// <summary>
		/// Logs a file to server.
		/// </summary>
		/// <returns>A response object that contains response string and response type that are set by server.</returns>
		/// <param name="data">JSON string data.</param>
		/// <param name="filePath">Path of file to be uploaded.</param> 
		/// <value>Logs data to server. Internally, LogData calls functions to log json data or string message. Depending on type of data the input data are sent to different URIs.</value>
		public ResponseObject LogFile ( string data, string filePath ) {
			
			ResponseObject R = new ResponseObject();

			string contentType = "text/html";
			string paramName = "file";
			string uri = "http://skylr.renci.org/api/data/file/add";
			R = ( LogFileData (uri, filePath, paramName, contentType, data) );
			
			return R;
		}



		/// <summary>
		/// Logs the input binary data.
		/// </summary>
		/// <returns>A response object that contains response string and response type that are set by server.</returns>
		/// <param name="data">Byte array data (binary).</param>
		public ResponseObject LogBinaryFile ( byte[] data ) {
			string uri = "http://skylr.renci.org/api/data/file/addBinary";
			return( LogBinaryData(uri, "none", "fileObj", "multipart/form-data", data) );
		}



		/// <summary>
		/// Logs the input JSON data.
		/// </summary>
		/// <param name="uri">URI.</param>
		/// <param name="data">Data.</param>
		/// <returns>A response object that contains response string and response type that are set by server.</returns>
		protected ResponseObject LogEvent ( string uri, string data ) {
			// Set up an instance of Http Web Request 
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (uri);
			
			// Set up array to hold data
			byte[] byteArray = Encoding.UTF8.GetBytes ((string)data);
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Set attributes of the HTTP web request object
			
			// Request Method
			request.Method = WebRequestMethods.Http.Post;
			
			// Content Type
			request.ContentType = "application/json"; //"text/html"
			
			// Content length proprty
			request.ContentLength = byteArray.Length;
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			
			
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Declare request stream and write to it
			
			Stream dataStream = request.GetRequestStream ();
			
			// Check if data stream is null
			if (dataStream == null) {
				ResponseObject RFail = new ResponseObject ();
				switch(ResponseDensity) {
				case 1: RFail.Response = " "; break;
				case 2: case 3: case 4: case 5: RFail.Response = "[DataLogger] Error (stream could not be initialized).\n"; break;
				}
				RFail.Type = (int)ResponseObject.ResponseType.ERROR;
				return RFail;
			}
			
			// Write data to the request stream and close it
			dataStream.Write (byteArray, 0, byteArray.Length);
			dataStream.Close ();
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			
			
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Get response from server  
			
			ResponseObject R = new ResponseObject ();
			
			try
			{
				using (WebResponse response = request.GetResponse ())
				{
					HttpWebResponse _resp = (HttpWebResponse)response;
					
					if (_resp.StatusCode == HttpStatusCode.OK) {
						switch(ResponseDensity) {
						case 1: R.Response = " "; break;
						case 2: R.Response = " " + _resp.StatusCode + "\n"; break;
						case 3: case 4: case 5: R.Response = "[DataLogger] Server response: " + byteArray.Length + " bytes sent.\n"; break;
						}
						R.Type = (int)ResponseObject.ResponseType.NOTICE;
						_resp.Close ();
					}
				}
			}
			
			catch (WebException e)
			{
				using (WebResponse response = e.Response)
				{
					HttpWebResponse _resp = (HttpWebResponse)response;
					
					using (Stream _data = _resp.GetResponseStream ())
					{
						string _text = new StreamReader (_data).ReadToEnd();
						_resp.Close();
						switch(ResponseDensity) {
						case 1: R.Response = " "; break;
						case 2: R.Response = " " + _resp.StatusCode + "\n"; break;
						case 3: case 4: case 5: R.Response = "[DataLogger] Return code: " + _resp.StatusCode + " while sending data. Data size: " + data.Length + " Server Response: " + _text + "\n"; break;
						}
						R.Type = (int)ResponseObject.ResponseType.ERROR;
					}
				}
			}

			finally {
				request = null;
			}

			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			
			return R;
		}



		/// <summary>
		/// Logs the input message data.
		/// </summary>
		/// <param name="uri">URI.</param>
		/// <param name="data">Data.</param>
		/// <returns>A response object that contains response string and response type that are set by server.</returns>
		protected ResponseObject LogMessage ( string uri, string data ) {
			// Set up an instance of Http Web Request 
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (uri);
			
			// Set up array to hold data
			byte[] byteArray = Encoding.UTF8.GetBytes ((string)data);
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Set attributes of the HTTP web request object
			
			// Request Method
			request.Method = WebRequestMethods.Http.Post;
			
			// Content Type
			request.ContentType = "text/html";
			
			// Content length proprty
			request.ContentLength = byteArray.Length;
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			
			
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Declare request stream and write to it
			
			Stream dataStream = request.GetRequestStream ();
			
			// Check if data stream is null
			if (dataStream == null) {
				ResponseObject RFail = new ResponseObject();
				switch(ResponseDensity) {
				case 1: RFail.Response = " "; break;
				case 2: case 3: case 4: case 5: RFail.Response = "[DataLogger] Error (stream could not be initialized).\n"; break;
				}
				RFail.Type = (int)ResponseObject.ResponseType.ERROR;
				return RFail;
			}
			
			// Write data to the request stream and close it
			dataStream.Write (byteArray, 0, byteArray.Length);
			dataStream.Close ();
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			
			
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Get response from server  
			
			ResponseObject R = new ResponseObject ();
			
			try
			{
				using (WebResponse response = request.GetResponse ())
				{
					HttpWebResponse _resp = (HttpWebResponse)response;
					
					if(_resp.StatusCode == HttpStatusCode.OK) {
						switch (ResponseDensity) {
						case 1: R.Response = " "; break;
						case 2: R.Response = " " + _resp.StatusCode + "\n"; break;
						case 3: case 4: case 5: R.Response = "[DataLogger] Server response: " + byteArray.Length + " bytes sent.\n"; break;
						}
						R.Type = (int)ResponseObject.ResponseType.NOTICE;
						_resp.Close();
					}
				}
			}
			
			catch (WebException e)
			{
				using (WebResponse response = e.Response)
				{
					HttpWebResponse _resp = (HttpWebResponse)response;
					
					using (Stream _data = _resp.GetResponseStream ())
					{
						string _text = new StreamReader(_data).ReadToEnd();
						_resp.Close();
						switch (ResponseDensity) {
						case 1: R.Response = " "; break;
						case 2: R.Response = " " + _resp.StatusCode + "\n"; break;
						case 3: case 4: case 5: R.Response = "[DataLogger] Return code: " + _resp.StatusCode + " while sending data. Data size: " + data.Length + " Server Response: " + _text + "\n"; break;
						}
						R.Type = (int)ResponseObject.ResponseType.ERROR;
					}
				}
			}
			
			finally {
				request = null;
			}
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			
			return R;
		}




		/// <summary>
		/// Logs the input binary data.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="file">File.</param>
		/// <param name="paramName">Parameter name.</param>
		/// <param name="contentType">Content type.</param>
		/// <param name="data">Data.</param>
		// <returns>A response object that contains response string and response type that are set by server.</returns>
		protected ResponseObject LogBinaryData ( string url, string file, string paramName, string contentType, byte[] data ) {

			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

			// Set up an instance of Http Web Request 
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Set attributes of the HTTP web request object
			
			// Request Method
			request.Method = WebRequestMethods.Http.Post;

			// Request Content Type
			request.ContentType = "multipart/form-data; boundary=" + boundary;

			// Request keep alive flag
			request.KeepAlive = true;

			// Request credentials
			request.Credentials = System.Net.CredentialCache.DefaultCredentials;
			

			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Declare request stream and write to it

			Stream dataStream = request.GetRequestStream();

			// Check if data stream is null
			if (dataStream == null) {
				ResponseObject RFail = new ResponseObject ();
				switch (ResponseDensity) {
				case 1: RFail.Response = " "; break;
				case 2: case 3: case 4: case 5: RFail.Response = "[DataLogger] Error (stream could not be initialized).\n"; break;
				}
				RFail.Type = (int)ResponseObject.ResponseType.ERROR;
				return RFail;
			}

			// Write out boundary bytes
			byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes ("\r\n--" + boundary + "\r\n");
			dataStream.Write(boundarybytes, 0, boundarybytes.Length);


			// Write out header bytes
			string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			string header = string.Format (headerTemplate, paramName, file, contentType);
			byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes (header);
			dataStream.Write(headerbytes, 0, headerbytes.Length);

			// Write out data
			byte[] byteArray = data;
			dataStream.Write(byteArray, 0, byteArray.Length);


			// Write out tail bytes
			byte[] trailer = System.Text.Encoding.ASCII.GetBytes ("\r\n--" + boundary + "--\r\n");
			dataStream.Write(trailer, 0, trailer.Length);
			dataStream.Close();

			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Get response from server  
			
			ResponseObject R = new ResponseObject ();
			
			try
			{
				using (WebResponse response = request.GetResponse ())
				{
					HttpWebResponse _resp = (HttpWebResponse)response;
					
					if (_resp.StatusCode == HttpStatusCode.OK) {
						switch (ResponseDensity) {
						case 1: R.Response = " "; break;
						case 2: R.Response = " " + _resp.StatusCode + "\n"; break;
						case 3: case 4: case 5: R.Response = "[DataLogger] Server response: " + byteArray.Length + " bytes sent.\n"; break;
						}
						R.Type = (int)ResponseObject.ResponseType.NOTICE;
						_resp.Close ();
					}
				}
			}
			
			catch (WebException e)
			{
				using (WebResponse response = e.Response)
				{
					HttpWebResponse _resp = (HttpWebResponse)response;
					
					using (Stream _data = _resp.GetResponseStream ())
					{
						string _text = new StreamReader(_data).ReadToEnd();
						_resp.Close();
						switch (ResponseDensity) {
						case 1: R.Response = " "; break;
						case 2: R.Response = " " + _resp.StatusCode + "\n"; break;
						case 3: case 4: case 5: R.Response = "[DataLogger] Return code: " + _resp.StatusCode + " while sending data. Data size: " + data.Length + " Server Response: " + _text + "\n"; break;
						}
						R.Type = (int)ResponseObject.ResponseType.ERROR;
					}
				}
			}

			finally {
				request = null;
			}

			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//

			return R;
		}


		/// <summary>
		/// Logs the input binary data.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="filePath">Location of file on computer.</param>
		/// <param name="paramName">Parameter name.</param>
		/// <param name="contentType">Content type.</param>
		/// <param name="data">Data.</param>
		// <returns>A response object that contains response string and response type that are set by server.</returns>
		protected ResponseObject LogFileData ( string url, string filePath, string paramName, string contentType, string data ) {
			
			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			
			// Set up an instance of Http Web Request 
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Set attributes of the HTTP web request object
			
			// Request Method
			request.Method = WebRequestMethods.Http.Post;
			
			// Request Content Type
			request.ContentType = "multipart/form-data; boundary=" + boundary;
			
			// Request keep alive flag
			request.KeepAlive = true;
			
			// Request credentials
			request.Credentials = System.Net.CredentialCache.DefaultCredentials;
			
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Declare request stream and write to it
			
			Stream dataStream = request.GetRequestStream();
			
			// Check if data stream is null
			if (dataStream == null) {
				ResponseObject RFail = new ResponseObject ();
				switch (ResponseDensity) {
				case 1: RFail.Response = " "; break;
				case 2: case 3: case 4: case 5: RFail.Response = "[DataLogger] Error (stream could not be initialized).\n"; break;
				}
				RFail.Type = (int)ResponseObject.ResponseType.ERROR;
				return RFail;
			}
			
			// Write out boundary bytes
			byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes ("\r\n--" + boundary + "\r\n");
			dataStream.Write(boundarybytes, 0, boundarybytes.Length);
			
			
			// Write out header bytes
			string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			string header = string.Format (headerTemplate, paramName, filePath, contentType);
			byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes (header);
			dataStream.Write(headerbytes, 0, headerbytes.Length);
			
			// Write out file data
			FileStream fileStream = new FileStream (filePath, FileMode.Open, FileAccess.Read);
			byte[] byteArray = new byte[4096];
			int bytesRead = 0;
			while ((bytesRead = fileStream.Read (byteArray, 0, byteArray.Length)) != 0) {
				dataStream.Write (byteArray, 0, bytesRead);
			}
			fileStream.Close();
			
			// Write out tail bytes
			byte[] trailer = System.Text.Encoding.ASCII.GetBytes ("\r\n--" + boundary + "--\r\n");
			dataStream.Write (trailer, 0, trailer.Length);
			dataStream.Close ();
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			// Get response from server  
			
			ResponseObject R = new ResponseObject ();
			
			try
			{
				using (WebResponse response = request.GetResponse ())
				{
					HttpWebResponse _resp = (HttpWebResponse)response;
					
					if (_resp.StatusCode == HttpStatusCode.OK) {
						switch (ResponseDensity) {
						case 1: R.Response = " "; break;
						case 2: R.Response = " " + _resp.StatusCode + "\n"; break;
						case 3: case 4: case 5: R.Response = "[DataLogger] Server response: " + byteArray.Length + " bytes sent.\n"; break;
						}
						R.Type = (int)ResponseObject.ResponseType.NOTICE;
						_resp.Close ();
					}
				}
			}
			
			catch (WebException e)
			{
				using (WebResponse response = e.Response)
				{
					HttpWebResponse _resp = (HttpWebResponse)response;
					
					using (Stream _data = _resp.GetResponseStream ())
					{
						string _text = new StreamReader(_data).ReadToEnd();
						_resp.Close();
						switch (ResponseDensity) {
						case 1: R.Response = " "; break;
						case 2: R.Response = " " + _resp.StatusCode + "\n"; break;
						case 3: case 4: case 5: R.Response = "[DataLogger] Return code: " + _resp.StatusCode + " while sending data. Data size: " + data.Length + " Server Response: " + _text + "\n"; break;
						}
						R.Type = (int)ResponseObject.ResponseType.ERROR;
					}
				}
			}
			
			finally {
				request = null;
			}
			
			//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=//
			
			return R;
		}

		/// <summary>
		/// Logs the input image data.
		/// </summary>
		/// <param name="uri">URI.</param>
		/// <param name="data">Data.</param>
		/// <returns>A response object that contains response string and response type that are set by server.</returns>
		protected ResponseObject LogImageData( string uri, string data ) {
			// TO DO
			return new ResponseObject();
		}



		/// <summary>
		/// Logs the input audio data.
		/// </summary>
		/// <param name="uri">URI.</param>
		/// <param name="data">Data.</param>
		/// <returns>A response object that contains response string and response type that are set by server.</returns>
		protected ResponseObject LogAudioData ( string uri, string data ) {
			// TO DO
			return new ResponseObject ();
		}



	}
	
}

