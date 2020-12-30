using Microsoft.AspNetCore.Mvc;

namespace TooMany.WebServer
{
	public class WebUiController: Controller
	{
		[Route("/")]
		public ActionResult Index() => Redirect("index.html");
	}
}
