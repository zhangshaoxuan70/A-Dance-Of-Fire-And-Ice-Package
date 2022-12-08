using System.Collections.Generic;

public class DeltaDictionary<TKey, TValue>
{
	private enum ActionType
	{
		Add,
		Remove
	}

	private struct Change
	{
		public ActionType actionType;

		public TKey key;

		public TValue value;
	}

	private readonly List<Change> changes = new List<Change>();

	public TValue this[TKey key]
	{
		set
		{
			Add(key, value);
		}
	}

	public void Add(TKey key, TValue value)
	{
		changes.Add(new Change
		{
			actionType = ActionType.Add,
			key = key,
			value = value
		});
	}

	public void Remove(TKey key)
	{
		changes.Add(new Change
		{
			actionType = ActionType.Remove,
			key = key
		});
	}

	public void Clear()
	{
		changes.Clear();
	}

	public void Apply(IEnumerable<Dictionary<TKey, TValue>> dicts)
	{
		foreach (Dictionary<TKey, TValue> dict in dicts)
		{
			Apply(dict);
		}
	}

	private void Apply(Dictionary<TKey, TValue> dict)
	{
		foreach (Change change in changes)
		{
			switch (change.actionType)
			{
			case ActionType.Add:
				if (dict.ContainsKey(change.key))
				{
					dict[change.key] = change.value;
				}
				else
				{
					dict.Add(change.key, change.value);
				}
				break;
			case ActionType.Remove:
				if (dict.ContainsKey(change.key))
				{
					dict.Remove(change.key);
				}
				break;
			}
		}
	}
}
