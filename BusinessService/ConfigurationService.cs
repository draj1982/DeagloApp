using DeagloApp.BusinessServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.IO;

namespace DeagloApp.BusinessService
{
   public class ConfigurationService: IConfigurationService
    {
        //IConfiguration IConfigurationService.GetConfiguration()
        //{
            //string currentEnvironment = System.Environment.GetEnvironmentVariable("stage");
            //return new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddSystemsManager(Path: $"/Deaglo/" + currentEnvironment + "/",
            //                       reloadAfter: TimeSpan.FromMinutes(5))
            //    .AddEnvironmentVariables()
            //    .Build();
        //}
    }

   
}
