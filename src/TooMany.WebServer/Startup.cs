using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using K4os.Json.KnownTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace TooMany.WebServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration) { Configuration = configuration; }

		private IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddControllers()
				.AddNewtonsoftJson(o => UpdateSerializationSettings(o.SerializerSettings));
			services.AddSignalR();
		}

		private static void UpdateSerializationSettings(JsonSerializerSettings settings)
		{
			settings.SerializationBinder = KnownTypesSerializationBinder.Default;
			settings.NullValueHandling = NullValueHandling.Ignore;
			settings.TypeNameHandling = TypeNameHandling.Auto;
			settings.Formatting = Formatting.None;
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseDeveloperExceptionPage();
			app.UseRouting();
			app.UseStaticFiles();
			app.UseCors(policy => policy.WithOrigins(AllowedOrigins()));
			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
				endpoints.MapHub<MonitorHub>("/monitor");
			});
		}

		private string[] AllowedOrigins() => 
			Configuration.GetSection("Host:Server:Cors").Get<string[]>();
	}
}
