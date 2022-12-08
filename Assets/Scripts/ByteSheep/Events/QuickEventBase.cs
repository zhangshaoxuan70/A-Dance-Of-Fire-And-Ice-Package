using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace ByteSheep.Events
{
	[Serializable]
	public abstract class QuickEventBase : ISerializationCallbackReceiver
	{
		[SerializeField]
		[FormerlySerializedAs("m_inspectorListHeight")]
		private float inspectorListHeight = 40f;

		[FormerlySerializedAs("m_persistentCalls")]
		public QuickPersistentCallGroup persistentCalls;

		protected void InvokePersistent()
		{
			for (int i = 0; i < persistentCalls.calls.Count; i++)
			{
				persistentCalls.calls[i].Invoke();
			}
		}

		protected void InvokePersistent(object dynamicArgument)
		{
			for (int i = 0; i < persistentCalls.calls.Count; i++)
			{
				persistentCalls.calls[i].SetDynamicArgument(dynamicArgument);
				persistentCalls.calls[i].Invoke();
			}
		}

		protected Type GetActionType(QuickSupportedTypes type)
		{
			switch (type)
			{
			case QuickSupportedTypes.String:
				return typeof(QuickAction<string>);
			case QuickSupportedTypes.Int:
				return typeof(QuickAction<int>);
			case QuickSupportedTypes.Float:
				return typeof(QuickAction<float>);
			case QuickSupportedTypes.Bool:
				return typeof(QuickAction<bool>);
			case QuickSupportedTypes.Color:
				return typeof(QuickAction<Color>);
			case QuickSupportedTypes.Vector2:
				return typeof(QuickAction<Vector2>);
			case QuickSupportedTypes.Vector3:
				return typeof(QuickAction<Vector3>);
			case QuickSupportedTypes.Object:
				return typeof(QuickAction<UnityEngine.Object>);
			case QuickSupportedTypes.GameObject:
				return typeof(QuickAction<GameObject>);
			case QuickSupportedTypes.Transform:
				return typeof(QuickAction<Transform>);
			default:
				return typeof(QuickAction);
			}
		}

		public int GetPersistentEventCount()
		{
			return persistentCalls.calls.Count;
		}

		public string GetPersistentMemberName(int index)
		{
			if (GetPersistentEventCount() <= 0)
			{
				return null;
			}
			return persistentCalls.calls[Mathf.Clamp(index, 0, Mathf.Max(0, GetPersistentEventCount() - 1))].memberName;
		}

		public UnityEngine.Object GetPersistentTarget(int index)
		{
			if (GetPersistentEventCount() <= 0)
			{
				return null;
			}
			return persistentCalls.calls[Mathf.Clamp(index, 0, Mathf.Max(0, GetPersistentEventCount() - 1))].target;
		}

		public void SetPersistentListenerState(int index, bool enabled)
		{
			if (GetPersistentEventCount() > 0)
			{
				persistentCalls.calls[Mathf.Clamp(index, 0, Mathf.Max(0, GetPersistentEventCount() - 1))].isCallEnabled = enabled;
				if (enabled)
				{
					OnAfterDeserialize();
				}
			}
		}

		public virtual void OnBeforeSerialize()
		{
		}

		public virtual void OnAfterDeserialize()
		{
			for (int i = 0; i < persistentCalls.calls.Count; i++)
			{
				QuickPersistentCall quickPersistentCall = persistentCalls.calls[i];
				if (!quickPersistentCall.isCallEnabled || (object)quickPersistentCall.target == null || quickPersistentCall.memberName == "" || quickPersistentCall.argument == null || quickPersistentCall.isDynamic)
				{
					continue;
				}
				Type argumentType = quickPersistentCall.argument.GetArgumentType();
				quickPersistentCall.argumentValue = quickPersistentCall.argument.GetArgumentValue();
				if (quickPersistentCall.memberType == MemberTypes.Method)
				{
					MethodInfo method = quickPersistentCall.target.GetType().GetMethod(quickPersistentCall.memberName, (argumentType == null) ? new Type[0] : new Type[1]
					{
						argumentType
					});
					if (method != null)
					{
						quickPersistentCall.actionGroup.SetDelegate(Delegate.CreateDelegate(GetActionType(quickPersistentCall.argument.supportedType), quickPersistentCall.target, method, throwOnBindFailure: true), quickPersistentCall.argument.supportedType);
					}
				}
				else if (quickPersistentCall.memberType == MemberTypes.Property)
				{
					PropertyInfo property = quickPersistentCall.target.GetType().GetProperty(quickPersistentCall.memberName);
					if (property != null)
					{
						MethodInfo setMethod = property.GetSetMethod();
						if (setMethod != null)
						{
							quickPersistentCall.actionGroup.SetDelegate(Delegate.CreateDelegate(GetActionType(quickPersistentCall.argument.supportedType), quickPersistentCall.target, setMethod, throwOnBindFailure: true), quickPersistentCall.argument.supportedType);
						}
					}
				}
				else if (quickPersistentCall.memberType == MemberTypes.Field)
				{
					quickPersistentCall.fieldInfo = quickPersistentCall.target.GetType().GetField(quickPersistentCall.memberName);
				}
			}
		}
	}
}
