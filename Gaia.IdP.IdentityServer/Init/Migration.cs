using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Gaia.IdP.IdentityServer.Init
{
    public static class Migration
    {
        public static void MigrateDbContext<T>(this IApplicationBuilder app) where T : Microsoft.EntityFrameworkCore.DbContext
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<T>();
                dbContext.Database.Migrate();
            }
        }
    }
}
