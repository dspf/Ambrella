namespace AmbrellaWeb.Areas.Institution.Controllers
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapInstitutionRoutes(this IEndpointRouteBuilder routes)
        {
            routes.MapAreaControllerRoute(
                name: "Institution",
                areaName: "Institution",
                pattern: "Institution/{controller=Home}/{action=Index}/{id?}"
            );
        }
    }
}
