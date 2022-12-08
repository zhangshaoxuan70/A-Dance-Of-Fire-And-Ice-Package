using System.Collections.Generic;
using UnityEngine;

namespace ADOFAI
{
	public class LevelEventInfo
	{
		public string name;

		public LevelEventType type;

		public bool pro;

		public bool taroDLC;

		public Dictionary<string, PropertyInfo> propertiesInfo;

		public List<LevelEventCategory> categories;

		public LevelEventExecutionTime executionTime;

		public bool taroDLCCheck
		{
			get
			{
				if (!ADOBase.hasTaroDLC)
				{
					return !taroDLC;
				}
				return true;
			}
		}

		public bool isActive
		{
			get
			{
				if (Application.isEditor || !pro)
				{
					return taroDLCCheck;
				}
				return false;
			}
		}
	}
}
