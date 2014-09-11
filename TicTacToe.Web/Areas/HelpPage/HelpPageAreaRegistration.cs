using System.Web.Http;
using System.Web.Mvc;

namespace TicTacToe.Web.Areas.HelpPage
{
    public class HelpPageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HelpPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HelpPage_Default",
                "HelpMaafaka/{action}/{apiId}",
                new { controller = "Help", action = "Index", apiId = UrlParameter.Optional },
                new[] { "TicTacToe.Web.Areas.HelpPage" });

            HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}