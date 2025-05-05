using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SelfServicePortal.Core.Entities.Identity;
using SelfServicePortal.Core.Interfaces;
using SelfServicePortal.Infrastructure.Persistence;
using SelfServicePortal.Infrastructure.Services;
namespace SelfServicePortal.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<TicketDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
             .AddEntityFrameworkStores<TicketDbContext>()
             .AddDefaultTokenProviders()
             .AddUserStore<UserStore<ApplicationUser, ApplicationRole, TicketDbContext, Guid>>()
             .AddRoleStore<RoleStore<ApplicationRole, TicketDbContext, Guid>>();


            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITicketDbContext, TicketDbContext>();

            return services;
        }
    }
}
