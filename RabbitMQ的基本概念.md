## RabbitMQ的基本概念

1、集成事件是服务器间的通信，所以必须借助于第三方服务器作为事件总线。常用的消息中间件有Redis、RabbitMQ、Kafka、ActiveMQ等。

2、RabbitMQ的基本概念：

1）信道（Channel）：信道是消息的生产者、消费者和服务器进行通信的虚拟连接。TCP连接的建立是非常消耗资源的，所以RabbitMQ在TCP连接的基础上构建了虚拟的信道。我们尽量重复使用TCP连接，而信道则是可以用完了就关闭。

2）队列（Queue）：用来进行消息收发的地方，生产者把消息放到队列中，消费者从队列中获取数据。

3）交换机（exchange）：把消息路由到一个或者多个队列中。

## RabbitMQ的routing模式

![image-20220327143843346](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327143843346.png)

生产者把消息发布到交换机中，消息携带一个routingKey属性，交换机会根据routingKey的值把消息发送到一个或者多个队列；消费者会从队列中获取消息；交换机和队列都位于RabbitMQ服务器内部。优点：即使消费者不在线，消费者相关的消息也会被保存到队列中，当消费者上线之后，消费者就可以获取到离线期间错过的消息。

## RabbitMQ安装与使用

#### 第一步：下载并安装erlang

RabbitMQ服务端代码是使用并发式语言Erlang编写的，安装Rabbit MQ的前提是安装Erlang

下载地址：http://www.erlang.org/downloads

window下可以选择以上下载

![image-20220327144619474](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327144619474.png)

1.下载完就是这个东西：

![image-20220327144858098](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327144858098.png)

2.点next就可以了

![image-20220327144935629](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327144935629.png)

3.选择保存位置

![image-20220327145005385](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327145005385.png)

4.安装完事儿后要记得配置一下系统的环境变量。

此电脑-->鼠标右键“属性”-->高级系统设置-->环境变量-->“新建”系统环境变量

![image-20220327145500446](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327145500446.png)

变量名：ERLANG_HOME

变量值就是刚才erlang的安装地址，点击确定。

然后双击系统变量path

点击“新建”，将%ERLANG_HOME%\bin加入到path中。

![image-20220327145706250](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327145706250.png)

- 最后windows键+R键，输入cmd，再输入erl，看到版本号就说明erlang安装成功了。

![image-20220327145832133](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327145832133.png)

liunx下下载方式：

![image-20220327144726510](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327144726510.png)

#### 第二步：下载并安装RabbitMQ

- 下载地址：http://www.rabbitmq.com/download.html
- ![image-20220327150207365](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327150207365.png)

- 双击下载后的.exe文件，安装过程与erlang的安装过程相同。

- RabbitMQ安装好后接下来安装RabbitMQ-Plugins。打开命令行cd，输入RabbitMQ的sbin目录。

- 我的目录是：E:\development\RabbitMQ\rabbitmq_server-3.9.14\sbin

  然后在后面输入rabbitmq-plugins enable rabbitmq_management命令进行安装（管理页面的安装）

  ![image-20220327151348450](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327151348450.png)

  输入 rabbitmqctl status , 如果出现以下的图，说明安装是成功的，并且说明现在RabbitMQ Server已经启动了,运行正常。

  ![image-20220327151525665](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327151525665.png)

  打开sbin目录，双击rabbitmq-server.bat

  ![image-20220327151657934](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327151657934.png)

​      等几秒钟，访问http://localhost:15672

![image-20220327151856182](C:\Users\lurud\AppData\Roaming\Typora\typora-user-images\image-20220327151856182.png)

默认用户名和密码都是guest

登陆即可。

RabbitMQ等消息中间件的消息发布和消费的过程是异步的，也就是消息发布者将消息放入消息中间件就返回了，并不会等待消息的消费过程，因此集成事件不仅能够降低微服务之间的耦合度，也还能够起到削峰的作用，避免一个微服务中的突发请求导致其他微服务雪崩的情况出现，而且消息中间件的失败重发机制可以提高消息处理的成功率，从而保证事务的最终一致性。

最终一致性的事务：需要开发人员对流程进行精细的设计，甚至有时候需要引入人工补偿操作。不像强一致性事务那样是纯技术方案。



