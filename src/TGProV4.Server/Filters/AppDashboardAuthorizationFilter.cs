namespace TGProV4.Server.Filters;

public class AppDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        return httpContext.User.IsInRole(ApplicationPermissions.Hangfires.View);
    }
}
