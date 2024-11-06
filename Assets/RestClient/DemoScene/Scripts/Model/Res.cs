using System;

namespace Models
{
	[Serializable]
	public class Res
	{
		public string login;

		public string password;

		public string response;

		public override string ToString(){
			return UnityEngine.JsonUtility.ToJson (this, true);
		}
	}
}

