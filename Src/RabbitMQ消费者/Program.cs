using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

/**********************************************/
//消息确认Ack
//对于没有确认的消息，可以进行消息重发
/**********************************************/

var factory = new ConnectionFactory();
factory.HostName = "127.0.0.1";
factory.DispatchConsumersAsync = true;
string exchangeName = "exchange1";
string eventName = "myEvent";
using var conn = factory.CreateConnection();
using var channel = conn.CreateModel();
string queueName = "queue1";

//交换机声明
channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

//队列声明
channel.QueueDeclare(queue: queueName, durable: true,
        exclusive: false, autoDelete: false, arguments: null);

//把队列绑定交换机
channel.QueueBind(queue: queueName,
    exchange: exchangeName, routingKey: eventName);

//消费者
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.Received += Consumer_Received;

channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
Console.WriteLine("按回车退出");
Console.ReadLine();
async Task Consumer_Received(object sender, BasicDeliverEventArgs args)
{
    try
    {
        var bytes = args.Body.ToArray();
        string msg = Encoding.UTF8.GetString(bytes);
        Console.WriteLine(DateTime.Now + "收到了消息" + msg);
        channel.BasicAck(args.DeliveryTag, multiple: false); //确认消息是否处理
        await Task.Delay(800);
    }
    catch (Exception ex)
    {
        channel.BasicReject(args.DeliveryTag, true);//失败重发
        Console.WriteLine("处理收到的消息出错" + ex);
    }
}



