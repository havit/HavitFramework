namespace Havit.Business.BusinessLayerGenerator.Helpers.Types
{
	/// <summary>
	/// Položka výčtu.
	/// </summary>
	public class EnumMember
	{
		#region MemberID
		/// <summary>
		/// ID položky výčtu.
		/// </summary>
		public int MemberID
		{
			get { return memberID; }
		}
		private readonly int memberID;
		#endregion

		#region MemberName
		/// <summary>
		/// Název položky výčtu.
		/// </summary>
		public string MemberName
		{
			get { return memberName; }
			set { memberName = value; }
		}
		private string memberName;
		#endregion

		#region Comment
		/// <summary>
		/// Komentář
		/// </summary>
		public string Comment
		{
			get { return comment; }
		}
		private readonly string comment;
		#endregion

		#region EnumMember
		public EnumMember(int memberID, string memberName, /*string description, */string comment)
		{
			this.memberID = memberID;
			this.memberName = memberName;
			//			this.description = description;
			this.comment = comment;
		}
		#endregion
	}
}
