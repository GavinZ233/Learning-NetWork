# 基础知识

## 1. IP地址和端口类



### 1.1 IPAddress类
127.0.0.1代表本机地址

        //数组初始化
        byte[] ipAddress = new byte[] { 118, 102, 111, 11 };
        IPAddress ip1 = new IPAddress(ipAddress);

        //long初始化,上面的十位转16位填入
        IPAddress ip2 = new IPAddress(0x76666F0B);

        //字符串转换
        IPAddress ip3 = IPAddress.Parse("118.102.111.11");

### 1.2 IPEndPoint类
IPEndPoint类将网络端点表示为IP地址和端口号，表现为IP地址和端口号的组合

        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("118.102.111.11"),8080);

## 2. 域名解析
域名：www.baidu.com,给人看的，IP：153.3.238.110，给电脑看的。     
域名解析就是域名到IP地址的转换过程。域名的解析工作由DNS服务器完成。

>  域名系统（英文：Domain Name System，缩写：DNS）是互联网的一项服务.   
它作为将域名和IP地址相互映射的一个分布式数据库，能够使人更方便地访问互联网. 
是因特网上解决网上机器命名的一种系统，因为IP地址记忆不方便，就采用了域名系统来管理名字和IP的对应关系.   
       
C# 提供了Dns静态类，获取域名解析            

        Dns.GetHostName()//返回本机名称
        IPHostEntry entry=Dns.GetHostName("www.baidu.com")//返回目标信息


### 2.1 IPHostEntry类
主要作用：域名解析后的返回值 可以通过该对象获取IP地址、主机名等等信息
获取关联IP       成员变量:AddressList
获取主机别名列表  成员变量:Aliases
获取DNS名称      成员变量:HostName


## 3. 序列化
非字符串转字节数组

        byte[] bytes = BitConverter.GetBytes(99);

字符串转字节数组

        byte[] bytes2 = Encoding.UTF8.GetBytes("字符串");

复制byte数组

        bytes.CopyTo(bytes2,startIndex);

注意：申明长度不固定的变量时，要考虑长度，如string，长度为sizeof(int)+string的长度      

举例:   

    public class TestInfo{
        public int lev;
        public string name;
        public short atk;
        public bool sex;
    }

序列化后的byte数组长度为:

        int indexNum = sizeof(int) + //lev int类型  4
                      sizeof(int) + //代表 name字符串转换成字节数组后 数组的长度 4
                      Encoding.UTF8.GetBytes(name).Length + //字符串具体字节数组的长度
                      sizeof(short) + //atk short类型 2
                      sizeof(bool); //sex bool类型 1

## 4. 反序列化

非字符串

        int i = BitConverter.ToInt32(bytes, 0);

字符串

        string str = Encoding.UTF8.GetString(bytes2, 0, bytes2.Length);



# Socket
命名空间：System.Net.Sockets    

Socket套接字是支持TCP/IP网络通信的基本操作单位      
一个套接字对象包含以下关键信息      
1.本机的IP地址和端口        
2.对方主机的IP地址和端口    
3.双方通信的协议信息    

## 1. 构造函数入参

AddressFamily 网络寻址 枚举类型，决定寻址方案


|||
|--|--|
|**InterNetwork** | IPv4寻址|
|**InterNetwork6**| IPv6寻址|
|UNIX |UNIX本地到主机地址 |
|ImpLink   |    ARPANETIMP地址
|Ipx      |     IPX或SPX地址
|Iso   |        ISO协议的地址
|Osi     |      OSI协议的地址
|NetBios   |    NetBios地址
|Atm      |     本机ATM服务地址

SocketType 套接字枚举类型，决定使用的套接字类型

|||
|--|--|
|**Dgram**     |    支持数据报，最大长度固定的无连接、不可靠的消息(主要用于UDP通信)
|**Stream**   |    支持可靠、双向、基于连接的字节流（主要用于TCP通信）
|Raw     |      支持对基础传输协议的访问
|Rdm      |     支持无连接、面向消息、以可靠方式发送的消息
|Seqpacket   |  提供排序字节流的面向连接且可靠的双向传输

ProtocolType 协议类型枚举类型，决定套接字使用的通信协议

|||
|--|--|
|**TCP**     |      TCP传输控制协议
|**UDP**     |      UDP用户数据报协议
|IP      |      IP网际协议
 |Icmp      |    Icmp网际消息控制协议
|Igmp       |   Igmp网际组管理协议
|Ggp      |     网关到网关协议
|IPv4     |     Internet协议版本4
|Pup      |     PARC通用数据包协议
|Idp      |     Internet数据报协议
|Raw     |      原始IP数据包协议
|Ipx      |     Internet数据包交换协议
|Spx      |    顺序包交换协议
|IcmpV6    |   用于IPv6的Internet控制消息协议


常用：

        //TCP流套接字
        Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //UDP数据报套接字
        Socket socketUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


## 2. 常用属性

1. 套接字的连接状态     
   
        if(socketTcp.Connected)

2. 获取套接字的类型
   
        print(socketTcp.SocketType);
3. 获取套接字的协议类型
   
        print(socketTcp.ProtocolType);
4. 获取套接字的寻址方案
   
        print(socketTcp.AddressFamily);

5. 从网络中获取准备读取的数据数据量
   
        print(socketTcp.Available);

6. 获取本机EndPoint对象(注意 ：IPEndPoint继承EndPoint)
   
        socketTcp.LocalEndPoint as IPEndPoint
7. 获取远程EndPoint对象
   
        socketTcp.RemoteEndPoint as IPEndPoint

### 3. 常用方法

1. 用于服务端

        //  1-1:绑定IP和端口
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        socketTcp.Bind(ipPoint);
        //  1-2:设置客户端连接的最大数量
        socketTcp.Listen(10);
        //  1-3:等待客户端连入
        socketTcp.Accept();

2. 用于客户端

        //  1-1:连接远程服务端
        socketTcp.Connect(IPAddress.Parse("118.12.123.11"), 8080);

3. 客户端服务端都会用的


        //  1-1:同步发送和接收数据
        //  1-2:异步发送和接收数据
        //  1-3:释放连接并关闭Socket，先于Close调用
        socketTcp.Shutdown(SocketShutdown.Both);
        //  1-4:关闭连接，释放所有Socket关联资源
        socketTcp.Close();

