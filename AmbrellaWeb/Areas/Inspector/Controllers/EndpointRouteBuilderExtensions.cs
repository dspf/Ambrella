namespace AmbrellaWeb.Areas.Inspector.Controllers
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapInspectorRoutes(this IEndpointRouteBuilder routes)
        {
            routes.MapAreaControllerRoute(
                name: "Inspector",
                areaName: "Inspector",
                pattern: "Inspector/{controller=Home}/{action=Index}/{id?}"
            );
        }
    }
}
