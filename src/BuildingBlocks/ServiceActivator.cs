using Microsoft.Extensions.DependencyInjection;


namespace BuildingBlocks
{
    public static class ServiceActivator
    {
        private static IServiceProvider ServiceProvider;

        /// <summary>
        /// Configure ServiceActivator with full serviceProvider
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Configure(this IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider.CreateScope().ServiceProvider;
        }
        public static IServiceProvider IServiceProvider => ServiceProvider;
    }
}
