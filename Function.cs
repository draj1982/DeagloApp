using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization.SystemTextJson;
using System.IO;
using Amazon.Lambda;
using Amazon;
using Amazon.Lambda.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DeagloApp.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DeagloApp.BusinessServiceInterfaces;
using DeagloApp.BusinessService;





// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DeagloApp
{
    public class Functions
    {
        private IServiceProvider _serviceProvider;
        private static IConfigurationService _configservice;
        LoggerManager _logger = LoggerManager.Instance;
        string awsRegion = "us-east-2";
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            _serviceProvider = ConfigureServices();
        }

        public static Func<IServiceProvider> ConfigureServices = () =>
         {
             var servicecollection = new ServiceCollection();
             servicecollection.AddSingleton<IConfigurationService, ConfigurationService>();
             using(var _services = servicecollection.BuildServiceProvider())
             {
                 _configservice = _services.GetService<IConfigurationService>();
             }
             //string connectionstring = _configservice.GetConfiguration()["connectionstring:Npgsql"];
             //servicecollection.AddTransient<IDbConnection>((sp) = new npg(connectionstring));
             return servicecollection.BuildServiceProvider();
         };

        

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Get Request\n");
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = "Hello AWS Serverless",
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } },
                IsBase64Encoded = false

            };
            return response;
        }
       

        public static APIGatewayProxyResponse APISuccessResponse(int statusCode = 200, string responseBody = null, string contentType = "application/json")
        {
            //constructing suceess response headers
            var apiResponse = new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = responseBody,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", contentType },
                    { "X-Requested-With", "*" },
                    { "Access-Control-Allow-Headers", "Content-Type,X-Amz-Date,Authorization,X-Api-Key,x-requested-with,Content-Type,Cache-Control,Pragma,X-Fullname,X-Username,X-Mail-Id" },
                                     { "Access-Control-Allow-Origin", "*"}, //headerorigin
                    { "Access-Control-Allow-Methods", "POST,GET,OPTIONS,PATCH" },
                    { "Access-Control-Allow-Credentials", "true" },
                },
                IsBase64Encoded = true
            };
            return apiResponse;
        }

        

        public Task<APIGatewayProxyResponse> CallLambdaPython(APIGatewayProxyRequest request, ILambdaContext context)
        {
            using (AmazonLambdaClient client = new AmazonLambdaClient(RegionEndpoint.GetBySystemName(awsRegion)))
            {
                TestModel model = new TestModel();
                model.years = 2; model.Base = "USD"; model.term = "CAD";model.quadrant = "TL";
                _logger.CreateContext(context);
                _logger.LogInfo("invoke", "Step1 Calling ");
                JObject body = new JObject();
                body.Add("body", JsonConvert.SerializeObject(model));
                var lambdaRequest = new Amazon.Lambda.Model.InvokeRequest
                {
                    FunctionName = "arn:aws:lambda:us-east-2:180376143480:function:plot_ccy",
                    Payload = body.ToString()
                };
                var response = client.InvokeAsync(lambdaRequest);
                var res = response.Result;
                _logger.LogInfo("invoke", "Step2 Calling ");
                int httpStatusCode = (int)response.Result.HttpStatusCode;
                //json.loads(res['Payload'].read().decode("utf-8"))
                using (var sr = new StreamReader(res.Payload))
                {
                    var r = APISuccessResponse(httpStatusCode, sr.ReadToEnd());
                    return Task.FromResult(r);

                }
            }

        }


    }
}
