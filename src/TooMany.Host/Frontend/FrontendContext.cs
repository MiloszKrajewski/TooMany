using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Timer = System.Windows.Forms.Timer;

namespace TooMany.Host.Frontend
{
	internal class FrontendContext: ApplicationContext
	{
		protected ILogger Log { get; }

		public FrontendContext(IServiceProvider services)
		{
			Log = services.GetService<ILoggerFactory>().CreateLogger(GetType());
			var cancel = services.GetRequiredService<CancellationTokenSource>();
			var config = services.GetRequiredService<IConfiguration>();
			var port = config.GetValue("Host:Server:Port", 31337);
			var menu = CreateMenu(cancel, port);
			var icon = CreateIcon(menu);
			var timer = new Timer { Interval = 333, Enabled = true };

			timer.Tick += (_, _) => ExitIfCancelled(cancel.Token);
			Application.ApplicationExit += (_, _) => BeforeClose(icon);

			timer.Start();

			AfterStart();
		}

		private void AfterStart() { Log.LogInformation("Frontend started"); }

		private void BeforeClose(NotifyIcon icon)
		{
			icon.Visible = false;
			Log.LogInformation("Frontend stopped");
		}

		private NotifyIcon CreateIcon(ContextMenuStrip menu)
		{
			var image = new Icon(GetType(), "tray.ico");
			var icon = new NotifyIcon {
				Icon = image,
				Visible = true,
				ContextMenuStrip = menu,
				Text = "TooMany",
			};
			return icon;
		}

		private static ContextMenuStrip CreateMenu(CancellationTokenSource cancel, int port)
		{
			var menu = new ContextMenuStrip();
			var open = new ToolStripMenuItem("Open UI");
			var close = new ToolStripMenuItem("Shutdown");
			menu.Items.Add(open);
			menu.Items.Add(new ToolStripSeparator());
			menu.Items.Add(close);
			open.Click += (_, _) => Open(port);
			close.Click += (_, _) => Shutdown(cancel);
			return menu;
		}

		private static void Open(int port)
		{
			var info = new ProcessStartInfo {
				FileName = $"http://localhost:{port}",
				UseShellExecute = true,
				CreateNoWindow = true,
			};
			Process.Start(info);
		}

		private static void Shutdown(CancellationTokenSource cancel) => cancel.Cancel();

		private static void ExitIfCancelled(CancellationToken token)
		{
			if (!token.IsCancellationRequested) return;

			Application.Exit();
		}
	}
}
