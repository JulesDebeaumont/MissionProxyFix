using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace IntranetEnMieux.Authorization.Utils;

interface ICrudResourceHandler {
    void HandleCreate(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement);
    void HandleRead(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement);
    void HandleUpdate(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement);
    void HandleDelete(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement);
}
