using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using IntranetEnMieux.Authorization.Utils;
using MissionDevBack.Db;
using Microsoft.Build.Evaluation;

namespace IntranetEnMieux.Authorization.Handlers;

public class ProjectsHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Project>, ICrudResourceHandler
{
    public MissionDevContext _dbContext;
    public ProjectsHandler(MissionDevContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Project project)
    {
        switch (requirement.Name)
        {
            case nameof(Operations.Create):
                HandleCreate(context, requirement);
                break;

            case nameof(Operations.Read):
                HandleRead(context, requirement);
                break;

            case nameof(Operations.Update):
                HandleUpdate(context, requirement);
                break;

            case nameof(Operations.Delete):
                HandleDelete(context, requirement);
                break;
        }
        return Task.CompletedTask;
    }

    public void HandleCreate(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
    {
        var success = true;

        if (success) {
            context.Succeed(requirement);
        }
    }

    public void HandleDelete(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
    {
        throw new NotImplementedException();
    }

    public void HandleRead(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
    {
        throw new NotImplementedException();
    }

    public void HandleUpdate(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
    {
        throw new NotImplementedException();
    }
}
