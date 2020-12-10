using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace K4os.Json.TolerantEnum
{
	internal class EnumMapper
	{
		private readonly Dictionary<long, string> _enumToString;
		private readonly Dictionary<string, long> _stringToEnum;
		private readonly Func<long, object> _longToEnum;

		public EnumMapper(Type enumType)
		{
			_enumToString = new Dictionary<long, string>();
			_stringToEnum = new Dictionary<string, long>(StringComparer.InvariantCultureIgnoreCase);
			_longToEnum = CreateLongToEnum(enumType);
			var fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (var field in fields)
				AddEnumMember(enumType, _enumToString, _stringToEnum, field);
		}

		private static Func<long, object> CreateLongToEnum(Type enumType)
		{
			var value = Expression.Parameter(typeof(long));
			var toEnum = Expression.Convert(value, enumType);
			var toObject = Expression.Convert(toEnum, typeof(object));
			var lambda = Expression.Lambda<Func<long, object>>(toObject, value);
			return lambda.Compile();
		}

		private static void AddEnumMember(
			Type enumType,
			IDictionary<long, string> enumToString,
			IDictionary<string, long> stringToEnum,
			MemberInfo field)
		{
			var fieldName = field.Name;
			var value = Convert.ToInt64(Enum.Parse(enumType, fieldName));
			var name = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? fieldName;

			Update(enumToString, value, name);
			Update(stringToEnum, name, value);
		}

		private static void Update<K, V>(IDictionary<K, V> map, K key, V value)
		{
			// this is thread safe as it happens only during construction
			// all other operations are reads
			if (!map.ContainsKey(key))
				map[key] = value;
		}

		public string GetName(long value) =>
			_enumToString.TryGetValue(value, out var name) ? name : null;

		public long? GetValue(string name) =>
			_stringToEnum.TryGetValue(name, out var value) ? value : default(long?);

		public object CastToEnum(long? value) =>
			value.HasValue ? _longToEnum(value.Value) : null;

		public long? ValidateValue(long value) =>
			_enumToString.ContainsKey(value) ? value : default(long?);

		public long EmptyValue => 0;
	}
}
