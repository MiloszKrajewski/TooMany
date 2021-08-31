using Microsoft.AspNetCore.Mvc;

namespace TooMany.WebServer
{
	public class WebUiController: Controller
	{
		[Route("/")]
		public ActionResult Index() => File("index.html", "text/html");
	}
}
