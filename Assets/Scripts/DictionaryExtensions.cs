using System.Collections.Generic;

public static class DictionaryExtensions
{
	public static bool TryGetValueAs<Key, Value, ValueAs>(this IDictionary<Key, Value> dictionary, Key key, out ValueAs valueAs) where ValueAs : Value
	{
		if (dictionary.TryGetValue(key, out Value value))
		{
			valueAs = (ValueAs)(object)value;
			return true;
		}
		valueAs = default(ValueAs);
		return false;
	}
}
