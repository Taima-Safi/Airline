using Airline.Service.User;
using Airline.Shared.Enum;
using Microsoft.AspNetCore.Authorization;
using SendGrid.Helpers.Errors.Model;
using System.Security.Claims;

namespace Airline.Middleware;

public class AuthenticationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endPoint = context.GetEndpoint() ??
            throw new NotFoundException("end point not found..");

        if (endPoint.DisplayName == "405 HTTP Method Not Supported")
            throw new MethodNotAllowedException("Http method not supported..");

        bool? isAllowAnonymous = endPoint?.Metadata.Any(x => x.GetType() == typeof(AllowAnonymousAttribute));
        if (isAllowAnonymous == true)
        {
            await next(context);
            return;
        }
        var token = context.Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(token) || !context.User.Identity.IsAuthenticated)
            throw new UnauthorizedAccessException("UnAuthorized");

        var controllerName = context.Request.RouteValues["controller"].ToString() ?? throw new NotFoundException("endpoint not found");
        //string actionType = context.Request.RouteValues["action"].ToString() ?? throw new NotFoundException("endpoint not found");
        var userId = long.Parse(context.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).FirstOrDefault());
        //var roleName = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
        var userService = context.RequestServices.GetRequiredService<IUserService>();
        var user = await userService.GetModelByIdAsync(userId);
        // var userRepo = context.RequestServices.GetRequiredService<IUserRepo>();
        //var userRoles = await userRepo.GetUserRoleAsync(userId);

        if (controllerName.Contains("Dashboard"))
        {
            if (user.Type != UserType.Admin)
                throw new AccessViolationException("No Access");
        }
        //if (controllerName.Contains("Dashboard") || controllerName.Contains("Role"))
        //{
        //    if (!userRoles.Any(x => x.Title == RoleType.FullAccess.ToString()) && roleName != nameof(UserType.SuperAdmin))
        //        throw new AccessViolationException("No Access");
        //}
        //else
        //{
        //    var role = new CheckRoleDto();
        //    //var userRoles = await userRepo.GetUserRoleAsync(userId);
        //    foreach (var userRole in userRoles)
        //    {
        //        role.FullAccess = userRole.Title == RoleType.FullAccess.ToString();
        //        role.DepoAccess = userRole.Title == RoleType.DepoAccess.ToString();
        //        role.OrderAccess = userRole.Title == RoleType.OrderAccess.ToString();
        //        role.CuttingLandAccess = userRole.Title == RoleType.CuttingLandAccess.ToString();
        //        //role.DepoAccess |= userRole.DepoAccess;
        //        //role.CuttingLandAccess |= userRole.CuttingLandAccess;
        //    }
        //    switch (controllerName)
        //    {
        //        case nameof(ControllerNames.OrderController):
        //            if (!role.OrderAccess)
        //                throw new AccessViolationException(" No Access");
        //            break;
        //        case nameof(ControllerNames.CuttingLandController):
        //            if (!role.CuttingLandAccess)
        //                throw new AccessViolationException(" No Access");
        //            break;
        //    }
        //}
        await next(context);
        return;
    }
}
