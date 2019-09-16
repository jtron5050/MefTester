using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MefTester.Abstractions
{
    public interface IPlugin : IHostedService
    {
        void RegisterServices(IServiceCollection services);
    }
}
