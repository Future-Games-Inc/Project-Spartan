using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("SkillPoints")]
	public class ES3UserType_SaveData : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SaveData() : base(typeof(SaveData)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (SaveData)obj;
			
			writer.WriteProperty("SkillPoints", instance.SkillPoints, ES3Type_int.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (SaveData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "SkillPoints":
						instance.SkillPoints = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_SaveDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SaveDataArray() : base(typeof(SaveData[]), ES3UserType_SaveData.Instance)
		{
			Instance = this;
		}
	}
}