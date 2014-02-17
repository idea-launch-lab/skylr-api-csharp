//------------------------------------------------------------------------------
//	Sylr API C#
//	
//	Class: ResponseObject
// 	ResponseObject is a class to contorl and wrap server's response object in 
//	an HTTPWebRequest communication.
//------------------------------------------------------------------------------

namespace InstrumentationLib 
{
	/// <summary>
	/// Class to customize server response object.
	/// </summary>
	public class ResponseObject 
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InstrumentationLib.ResponseObject"/> class.
		/// </summary>
		public ResponseObject() {
			Response = "[no response yet]";
			Type = (int)ResponseType.NOTICE;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InstrumentationLib.ResponseObject"/> class with an
		/// existing object of type ResponseObject.
		/// </summary>
		/// <param name="R">An object of type ResponseObject.</param>
		public ResponseObject(ResponseObject R) {
			Response = R.Response;
			Type = (int)R.Type;
		}

		/// <summary>
		/// Type of response
		/// </summary>
		/// <value>The response type returned after completion of logging action.</value>
		public enum ResponseType : int {ERROR=1, WARNING, INFORMATION, NOTICE};


		/// <summary>
		/// The response.
		/// </summary>
		/// <value>String containing response from server after logging action.</value>
		public string Response;

		/// <summary>
		/// The type of the response.
		/// </summary>
		/// <value>Type of response set by logging server. The type of response is one of following: ERROR, WARNING, INFORMATION, and NOTICE.</value>
		public int Type;
	}
}
