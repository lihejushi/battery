using System;
using System.Net;
using XT.MVC.Core.Common;
using XT.MVC.Core.Infrastructure;

namespace TuoKe.Framework.Queue
{
    public class EQueueManager : IQueueManager
    {
        private readonly Producer _producer;

        public EQueueManager()
        {
            ECommonConfiguration
                .Create()
                .UseAutofac()
                .RegisterCommonComponents()
                //.UseLog4Net()
                .UseJsonNet()
                .RegisterUnhandledExceptionHandler()
                .RegisterEQueueComponents()
                .SetDefault<IQueueSelector, QueueAverageSelector>();
            string address = ConfigHelper.GetBranchValue("\\App_Data\\Queue.Config", "Broker.Address");
            var brokerAddress = string.IsNullOrEmpty(address) ? SocketUtils.GetLocalIPV4() : IPAddress.Parse(address);
            int producerPort = Convert.ToInt32(ConfigHelper.GetBranchValue("\\App_Data\\Queue.Config", "Broker.ProducerPort"));
            int adminPort = Convert.ToInt32(ConfigHelper.GetBranchValue("\\App_Data\\Queue.Config", "Broker.AdminPort"));
            var setting = new ProducerSetting
            {
                BrokerAddress = new IPEndPoint(brokerAddress, producerPort),
                BrokerAdminAddress = new IPEndPoint(brokerAddress, adminPort)
            };
            _producer = new Producer(setting).Start();
        }

        public bool SendMessage(string topic, string message,int code = 1, string tag = null)
        {
            if (_producer != null)
            {
                try
                {
                    var payload = System.Text.Encoding.UTF8.GetBytes(message);

                    var _message = new Message(topic, code, payload, tag);
                    var result = _producer.Send(_message, code.ToString(), 500);
                    return result.SendStatus == SendStatus.Success;
                }
                catch (Exception ex)
                {
                    XT.MVC.Core.Logging.ILogger _logger = EngineContext.Current.Resolve<XT.MVC.Core.Logging.ILogger>("DefaultLogger");
                    _logger.Error(ex, ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
