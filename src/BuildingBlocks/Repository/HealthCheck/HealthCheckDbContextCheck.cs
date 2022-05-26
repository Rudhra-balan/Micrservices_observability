using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Repository.HealthCheck
{
    public class HealthCheckDbContextCheck<TDbContext> : IHealthCheck where TDbContext : DbContext
    {
        private readonly IDbContextProvider<TDbContext> _dbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public HealthCheckDbContextCheck(
            IDbContextProvider<TDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _dbContextProvider = dbContextProvider;
            _unitOfWorkManager = unitOfWorkManager;
        }


        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    if (await _dbContextProvider.GetDbContext().Database.CanConnectAsync(cancellationToken))
                    {
                        return HealthCheckResult.Healthy("HealthCheckExampleDbContext could connect to database");
                    }
                }
                return HealthCheckResult.Unhealthy("HealthCheckExampleDbContext could not connect to database");
            }
            catch (System.Exception e)
            {
                return HealthCheckResult.Unhealthy("Error when trying to check HealthCheckExampleDbContext. ", e);
            }
        }
    }
}