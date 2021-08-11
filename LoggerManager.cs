using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace DeagloApp
{
   public class LoggerManager
    {
        public static readonly LoggerManager Instance = new LoggerManager();
        private LoggerManager()
        {
        }
        private ILambdaContext _context;

        public void CreateContext(ILambdaContext context)
        {
            _context = context;
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
                IsBase64Encoded= false
            };
        return apiResponse;
    }

        public void LogInfo(string actionMethod, string actionPath, dynamic message = null)
        {
            var logMessage = JsonConvert.SerializeObject(new { actionArea = actionMethod, actionPath = actionPath, lambdaRequestId = _context?.AwsRequestId, Message = message });
            _context.Logger.LogLine("Information:" + logMessage);
        }
   }
}
