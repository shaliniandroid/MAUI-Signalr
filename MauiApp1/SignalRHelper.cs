
using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApp1
{
    
    public class SignalRHelper
    {
        HubConnection _connection;
        public async void Start()
        {

            //var certificate = new X509Certificate2("iiotsignalrservercert.pfx","Nagarro@123");

            _connection = new HubConnectionBuilder()
                 .WithUrl("https://iiotsignalrserver.centralindia.azurecontainer.io/notifications", options =>
                 {
                     options.Headers[CommonConstants.NotificationHubUserNamePropName] = "DataCollectionService";
                     options.Headers[CommonConstants.NotificationHubPasswordPropName] = "Passw0rd@123";
                     options.HttpMessageHandlerFactory = (message) =>
                     {
                         if (message is HttpClientHandler clientHandler)
                             // always verify the SSL certificate
                             clientHandler.ServerCertificateCustomValidationCallback +=
                                 (sender, certificate, chain, sslPolicyErrors) => { return true; };
                         return message;
                     };
                 })
                 .WithAutomaticReconnect()
                 .Build();
            
            

            var t = _connection.StartAsync();
            t.Wait();

            _connection.On<NotificationModel>("DeviceAdded", (model) => OnReceiveMessage(model));
            _connection.On<NotificationModel>("DeviceRemoved", (model) => OnReceiveMessage(model));
            _connection.On<NotificationModel>("DeviceStatusChange", (model) => OnReceiveMessage(model));
            _connection.On<NotificationModel>("GatewayAdded", (model) => OnReceiveMessage(model));
            _connection.On<NotificationModel>("GatewayRemoved", (model) => OnReceiveMessage(model));
            _connection.On<NotificationModel>("GatewayStatusChange", (model) => OnReceiveMessage(model));
            _connection.On<NotificationModel>("DataUpdated", (model) => OnReceiveMessage(model));

        }

        private void OnReceiveMessage(NotificationModel model)
        {
            Console.WriteLine(model.Type);
            Console.WriteLine(model.Data);
        }

    }
}