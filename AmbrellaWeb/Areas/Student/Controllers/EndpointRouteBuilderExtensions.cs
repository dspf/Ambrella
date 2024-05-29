namespace AmbrellaWeb.Areas.Student.Controllers
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapStudentRoutes(this IEndpointRouteBuilder routes)
        {
            routes.MapAreaControllerRoute(
                name: "Student",
                areaName: "Student",
                pattern: "Student/{controller=Home}/{action=Index}/{id?}"
            );
        }
    }
}
