using Microsoft.AspNetCore.Mvc.Routing;

namespace AmbrellaWeb.Areas.Landlord.Controllers
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapLandlordRoutes(this IEndpointRouteBuilder routes)
        {
            routes.MapAreaControllerRoute(
                name: "Landlord",
                areaName: "Landlord",
                pattern: "Landlord/{controller=Home}/{action=Index}/{id?}"
            );
        }
    }
}
