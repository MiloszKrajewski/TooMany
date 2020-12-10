using Google.Protobuf;
using Newtonsoft.Json;
using Proto.Remote;

namespace Proto.Serialization.Json
{
	/// <summary>
	/// Proto.Actor adapter for Newtonsoft.Json serializer.
	/// <example>
	/// Serialization.RegisterSerializer(new NewtonsoftSerializer(), true);
	/// </example>
	/// </summary>
	/// <seealso cref="ISerializer" />
	public class NewtonsoftSerializer: ISerializer
	{
		private readonly JsonSerializerSettings _settings;

		/// <summary>Initializes a new instance of the <see cref="NewtonsoftSerializer"/> class.</summary>
		/// <param name="settings">The serialization settings.</param>
		public NewtonsoftSerializer(JsonSerializerSettings settings = null)
		{
			_settings = settings ?? new JsonSerializerSettings {
				TypeNameHandling = TypeNameHandling.Auto,
				NullValueHandling = NullValueHandling.Ignore
			};
		}

		/// <summary>Serializes the specified object.</summary>
		/// <param name="obj">The object.</param>
		/// <returns>Serialized object as bytes.</returns>
		public ByteString Serialize(object obj) =>
			ByteString.CopyFromUtf8(JsonConvert.SerializeObject(obj, typeof(object), _settings));

		/// <summary>Deserializes the specified bytes.</summary>
		/// <param name="bytes">The bytes.</param>
		/// <param name="typeName">Name of the type (ignored).</param>
		/// <returns>Deserialized object.</returns>
		public object Deserialize(ByteString bytes, string typeName) =>
			JsonConvert.DeserializeObject(bytes.ToStringUtf8(), _settings);

		/// <summary>Gets the name of the type.</summary>
		/// <param name="message">The message.</param>
		/// <returns>Empty string. This value is irrelevant for Newtonsoft serializer.</returns>
		public string GetTypeName(object message) => string.Empty;
	}
}
