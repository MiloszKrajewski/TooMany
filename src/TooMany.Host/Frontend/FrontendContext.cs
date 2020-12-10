using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
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
			var menu = CreateMenu(cancel);
			var icon = CreateIcon(menu);
			var timer = new Timer { Interval = 333, Enabled = true };

			timer.Tick += (s, e) => ExitIfCancelled(cancel.Token);
			Application.ApplicationExit += (s, e) => BeforeClose(icon);
			
			timer.Start();

			AfterStart();
		}

		private void AfterStart()
		{
			Log.LogInformation("Frontend started");
		}

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

		private static ContextMenuStrip CreateMenu(CancellationTokenSource cancel)
		{
			var menu = new ContextMenuStrip();
			var close = new ToolStripMenuItem("Shutdown");
			menu.Items.Add(close);
			close.Click += (s, e) => cancel.Cancel();
			return menu;
		}

		private static void ExitIfCancelled(CancellationToken token)
		{
			if (!token.IsCancellationRequested) return;

			Application.Exit();
		}
	}
}
