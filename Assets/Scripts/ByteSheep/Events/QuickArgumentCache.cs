using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ByteSheep.Events
{
	[Serializable]
	public class QuickArgumentCache
	{
		[FormerlySerializedAs("m_supportedType")]
		public QuickSupportedTypes supportedType;

		[FormerlySerializedAs("m_stringArgument")]
		public string stringArgument;

		[FormerlySerializedAs("m_intArgument")]
		public int intArgument;

		[FormerlySerializedAs("m_floatArgument")]
		public float floatArgument;

		[FormerlySerializedAs("m_boolArgument")]
		public bool boolArgument;

		[FormerlySerializedAs("m_colorArgument")]
		public Color colorArgument;

		[FormerlySerializedAs("m_vector2Argument")]
		public Vector2 vector2Argument;

		[FormerlySerializedAs("m_vector3Argument")]
		public Vector3 vector3Argument;

		[FormerlySerializedAs("m_objectArgument")]
		public UnityEngine.Object objectArgument;

		[FormerlySerializedAs("m_gameObjectArgument")]
		public GameObject gameObjectArgument;

		[FormerlySerializedAs("m_transformArgument")]
		public Transform transformArgument;

		public object GetArgumentValue()
		{
			switch (supportedType)
			{
			case QuickSupportedTypes.String:
				return stringArgument;
			case QuickSupportedTypes.Int:
				return intArgument;
			case QuickSupportedTypes.Float:
				return floatArgument;
			case QuickSupportedTypes.Bool:
				return boolArgument;
			case QuickSupportedTypes.Color:
				return colorArgument;
			case QuickSupportedTypes.Vector2:
				return vector2Argument;
			case QuickSupportedTypes.Vector3:
				return vector3Argument;
			case QuickSupportedTypes.Object:
				return objectArgument;
			case QuickSupportedTypes.GameObject:
				return gameObjectArgument;
			case QuickSupportedTypes.Transform:
				return transformArgument;
			default:
				return null;
			}
		}

		public Type GetArgumentType()
		{
			switch (supportedType)
			{
			case QuickSupportedTypes.String:
				return typeof(string);
			case QuickSupportedTypes.Int:
				return typeof(int);
			case QuickSupportedTypes.Float:
				return typeof(float);
			case QuickSupportedTypes.Bool:
				return typeof(bool);
			case QuickSupportedTypes.Color:
				return typeof(Color);
			case QuickSupportedTypes.Vector2:
				return typeof(Vector2);
			case QuickSupportedTypes.Vector3:
				return typeof(Vector3);
			case QuickSupportedTypes.Object:
				return typeof(UnityEngine.Object);
			case QuickSupportedTypes.GameObject:
				return typeof(GameObject);
			case QuickSupportedTypes.Transform:
				return typeof(Transform);
			default:
				return null;
			}
		}
	}
}
