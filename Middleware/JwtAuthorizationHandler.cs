using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using DuploBackend.Data;

namespace DuploBackend.Middleware;

public class DepartmentAuthorizationHandler : AuthorizationHandler<DepartmentRequirement>
{
    private readonly MongoDbContext _context;

    public DepartmentAuthorizationHandler(MongoDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DepartmentRequirement requirement)
    {
        var idClaim = context.User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(idClaim))
        {
            return;
        }

        var department = await _context.Departments
            .Find(d => d.Id == idClaim)
            .FirstOrDefaultAsync();

        if (department != null)
        {
            context.Succeed(requirement);
        }
    }
}

public class BusinessAuthorizationHandler : AuthorizationHandler<BusinessRequirement>
{
    private readonly MongoDbContext _context;

    public BusinessAuthorizationHandler(MongoDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BusinessRequirement requirement)
    {
        var idClaim = context.User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(idClaim))
        {
            return;
        }

        var business = await _context.Businesses
            .Find(b => b.Id == idClaim)
            .FirstOrDefaultAsync();

        if (business != null)
        {
            context.Succeed(requirement);
        }
    }
}

public class DepartmentRequirement : IAuthorizationRequirement { }
public class BusinessRequirement : IAuthorizationRequirement { }
