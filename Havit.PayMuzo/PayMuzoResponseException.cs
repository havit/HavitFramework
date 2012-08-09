using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.PayMuzo
{
	/// <summary>
	/// Výjimka vyhazováná při zpracování odpovědi PayMuzo.
	/// </summary>
	[global::System.Serializable]
	public class PayMuzoResponseException : PayMuzoException
	{
		#region Response
		/// <summary>
		/// Odpověď PayMUZO.
		/// </summary>
		public PayMuzoResponse Response
		{
			get { return _response; }
			set { _response = value; }
		}
		private PayMuzoResponse _response;
		#endregion

		#region Default exception constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="PayMuzoResponseException"/> class.
		/// </summary>
		public PayMuzoResponseException() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="PayMuzoResponseException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public PayMuzoResponseException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="PayMuzoResponseException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="inner">The inner.</param>
		public PayMuzoResponseException(string message, Exception inner) : base(message, inner) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="PayMuzoResponseException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected PayMuzoResponseException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="PayMuzoResponseException"/> class.
		/// </summary>
		public PayMuzoResponseException(PayMuzoResponse response)
		{
			_response = response;
		}
		#endregion
	}
}
