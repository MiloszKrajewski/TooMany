using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Playground
{
	class Program
	{
		static void Main(string[] args)
		{
			var a = new[] { 1, 2, 3, 4 };
			var b = a.ToImmutableArray();

			var c = Serialize(b);
			
			Console.WriteLine(c);

			var d = Deserialize<ImmutableArray<int>>(c);
		}

		static string Serialize(object o) => JsonConvert.SerializeObject(o);

		static T Deserialize<T>(string s) => JsonConvert.DeserializeObject<T>(s);
	}
}
