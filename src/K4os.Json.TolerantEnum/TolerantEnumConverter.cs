using System;
using System.Collections.Concurrent;
using System.Linq;
using Newtonsoft.Json;

namespace K4os.Json.TolerantEnum
{
	public class TolerantEnumConverter: JsonConverter
	{
		private static readonly ConcurrentDictionary<Type, EnumMapper> Mappers =
			new ConcurrentDictionary<Type, EnumMapper>();

		private static Type InnerType(Type type) => Nullable.GetUnderlyingType(type) ?? type;

		private static EnumMapper GetMapper(Type type) => Mappers.GetOrAdd(type, NewMapper);

		private static EnumMapper NewMapper(Type type) => new EnumMapper(type);

		public override bool CanConvert(Type objectType) => InnerType(objectType).IsEnum;

		public override object? ReadJson(
			JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			var token = reader.TokenType;
			var enumType = InnerType(objectType);
			var isNullable = enumType != objectType;
			if (isNullable && token == JsonToken.Null) return null;
		
			var mapper = GetMapper(enumType);
			var result = ReadJson(token, reader, mapper);
			
			// ReSharper disable once ArrangeRedundantParentheses
			return mapper.CastToEnum(isNullable ? result : (result ?? mapper.EmptyValue));
		}

		private static long? ReadJson(JsonToken token, JsonReader reader, EnumMapper map) =>
			token switch {
				JsonToken.String => map.GetValue(reader.Value!.ToString()),
				JsonToken.Integer => map.ValidateValue(Convert.ToInt64(reader.Value)),
				JsonToken.Null => null,
				_ => throw NotSupportedToken(token)
			};

		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			if (value is null)
			{
				writer.WriteNull();
				return;
			}
			
			var objectType = value.GetType();
			var enumType = InnerType(objectType);
			var mapper = GetMapper(enumType);
			var name = mapper.GetName(Convert.ToInt64(value));

			writer.WriteValue(name);
		}
		
		private static Exception NotSupportedToken(JsonToken token) =>
			new NotSupportedException(
				$"Token {token} is not supported by {nameof(TolerantEnumConverter)}");
	}
}
