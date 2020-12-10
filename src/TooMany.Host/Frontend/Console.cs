using System.Runtime.InteropServices;

namespace TooMany.Host.Frontend
{
	public class Console
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AllocConsole();
	}
}
