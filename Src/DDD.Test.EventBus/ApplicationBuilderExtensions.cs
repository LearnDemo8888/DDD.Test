using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Test.EventBus
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseEventBus(this IApplicationBuilder appBuilder)
        {
            if (appBuilder.ApplicationServices.GetService(typeof(IEventBus)) == null)
            {
                throw new ApplicationException("找不到IEventBus实例");
            }

            return appBuilder;
        }
    }
}
