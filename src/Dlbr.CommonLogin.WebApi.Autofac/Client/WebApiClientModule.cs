using Autofac;
using Dlbr.CommonLogin.WebApi.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Autofac.Client
{
    public class WebApiClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacTokenProviderFactory>().As<ITokenProviderFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceAccountTokenProvider>().As<ITokenProvider>().AsSelf();
            builder.RegisterType<IntegratedTokenProvider>().As<ITokenProvider>().AsSelf();
            builder.RegisterGeneric(typeof(WebApiClient<>)).As(typeof(IWebApiClient<>));

            builder.Register(context =>
            {
                var proxyConfiguration = context.ResolveOptional<IProxyConfiguration>();
                if (proxyConfiguration != null && proxyConfiguration.Address != null)
                {
                    var handler = new HttpClientHandler()
                    {
                        Proxy = new WebProxy(proxyConfiguration.Address, false),
                        UseProxy = true
                    };
                    return new HttpClient(handler);
                }
                return new HttpClient();
            }).As<HttpClient>().InstancePerDependency();
        }
    }
}
