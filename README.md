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


## 3. Socket通信流程


|Client|通信|Server
|--|--|--|
|||创建Socket
|||Bind将套接字与本地地址绑定
|||Listen监听
|创建Socket||Accept等待客户端连接
|Connect链接服务端|网络连接（三次握手）|建立连接Accept返回新套接字
|用Send和Receive收发数据|网络通信|用Send和Receive收发数据
|Shotdown释放连接||Shotdown释放连接
|Close关闭套接字|（四次挥手）|Close关闭套接字






##  数据类结构（后面总结完考虑加入【封装Socket】）

### 1. BaseData

作为基础数据类基类，满足基本的序列化与反序列化需求，可读取比特流数据

|成员|作用|逻辑
|--|--|--|
|public abstract int GetBytesNum()|获取数据类的比特流长度|根据字段计算总长度
|    public abstract byte[] Writing()|将自身数据输出成比特数组|根据字段序列化成比特流，并在每一步记录当前索引
|    protected void WriteShort(byte[] bytes,short value,ref int index)|将value转成比特流|将value转成比特流接到bytes索引处，并修改传入的index使得比特数组的写入索引后移
|其他值类型Write()方法同理||
|    protected void WriteString(byte[] bytes, string value, ref int index)|读取比特数组中的string|将string通过Encoding.UTF8.GetBytes转成比特流，并在长度计算上额外考虑string 的比特结构（int长度+string本身长度）
|  protected void WriteData(byte[] bytes,BaseData data,ref int index)|将继承自己的类转成比特流|调用Writing方法，后移索引
|    public abstract int Reading(byte[] bytes,int beginIndex=0)|读取比特流|无
|    protected int ReadInt(byte[] bytes,ref int index)|读取比特流转int|从索引处读取比特流转成int，并后移索引
|Reading方法与Write同理||
|    protected T ReadData<T>(byte[] bytes,ref int index) where T:BaseData,new()|读取BaseData类的比特流|创建T类，调用T的Reading方法，并后移索引

>上述方法中多次提到后移索引，目的就是记录当前读取后的索引位置，作为下次读取的开头。

### 2. BaseMsg

作为客户端和服务端传输的消息类，继承自BaseData，特殊点在于，Msg的比特流会在前面添加一个int表示Msg的类型。               

|成员|作用|逻辑
|--|--|--|
|    public override int GetBytesNum()|同BaseData|
|    public override int Reading(byte[] bytes, int beginIndex = 0)|同BaseData|
|    public override byte[] Writing()|同BaseData|
|    public virtual int GetID()|获取该Msg的类型ID|每个Msg类都拥有独立的ID，在类里写死


|`消息结构`|消息ID|消息体|
|--|--|--|
|字节索引|0-3|4-Length-1|



##  封装Socket

### 类结构

#### 1. NetMgr

|成员|作用|逻辑
|--|--|--|
|||
|||
|||
|||



#### 2. ServerSocket

|成员|作用|逻辑
|--|--|--|
|||
|||
|||
|||



###  分包黏包

#### 1. 什么是分包黏包       
分包：一条消息被分成多条发送。          
黏包：多条消息被黏在一起发送。  
两种情况可能同时发生            

#### 2. 解决思路     
##### 1. 标记长度
消息头部添加`长度`，目前已有的头部信息是`ID`表示消息类型，现在额外添加长度，头部消息为8字节，`ID`+`长度`    
读取消息时，可以根据头部信息确定消息长度，作为字节流分割的参考。
|`消息字节流结构`|消息ID|消息长度|消息体|
|--|--|--|--|
|字节索引|0-3|4-7|8-Length-1|

##### 2. 引入缓存区

只有黏包的情况，对收到的字节流进行消息分割即可。                
但出现分包时，收到的字节流是不完整的消息，无法正常处理。          
将每次收到的字节流记入缓存区，不完整的消息也可以借此拼接成完整的消息。     
处理到分包时，将前半部消息记入缓存区，并移动到缓存区前面，后面满足长度时依照黏包的逻辑执行。

当然，后面还需要注意缓存区大小与空间利用。（可以利用头尾双标记，尝试将缓存区做成环状结构，避免缓存区频繁的数据迁移）



##### 3. 分包黏包情况举例



当出现`AB黏包且B包分包`时，缓存区如下

|`缓存区内容`|消息A ID|消息A长度|消息体A|消息B ID|消息B长度|消息体B前半部分|
|--|--|--|--|--|--|--|
|字节索引|0-3|4-7|8-12|13-16|17-20|21-22

此时的消息A可读取且完整，只需要做索引标记到`13`，即可开始读取消息B，但B只有前半部分，需要等待后续读取的字节流加入到缓存区，将B组合完成，再读出B消息。           

当`B消息完整`时，缓存区如下


|`缓存区内容`|消息B ID|消息B长度|消息体B前半部分|消息体B后半部分|
|--|--|--|--|--|
|字节索引 |0-3|4-7|8-9|10-15


#### 3. 代码逻辑        



      private void HandleReceiveMsg(byte[] receiveBytes,int receiveNum)
    {
        int msgID = 0;
        int msgLength = 0;
        int nowIndex = 0;//当前访问索引

        //收到消息时将消息记入缓存区
        receiveBytes.CopyTo(cacheBytes, cacheNum);
        cacheNum += receiveNum;

        while (true)
        {
            //每次将长度设置为-1 是避免上一次解析的数据 影响这一次的判断
            msgLength = -1;
            //是否满足一条消息的完整长度
            if (cacheNum - nowIndex >= 8)
            {
                //解析ID
                msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
                //解析长度
                msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
            }
            //缓存区的数据足够读取完整的消息体
            if (cacheNum - nowIndex >= msgLength && msgLength != -1)
            {
                //解析消息体
                BaseMsg baseMsg = null;
                switch (msgID)
                {
                    case 1001:
                        PlayerMsg msg = new PlayerMsg();
                        msg.Reading(cacheBytes, nowIndex);
                        baseMsg = msg;
                        break;
                }
                if (baseMsg != null)
                    receiveQueue.Enqueue(baseMsg);
                nowIndex += msgLength;
                if (nowIndex == cacheNum)
                {
                    cacheNum = 0;
                    break;
                }
            }
            else //不满足,证明有分包
            {
                //如果进行了 id和长度的解析 但是 没有成功解析消息体 那么需要减去nowIndex移动的位置
                if (msgLength != -1)
                    nowIndex -= 8;
                //把剩余没有解析的字节数组内容 移到前面来 用于缓存下次继续解析
                Array.Copy(cacheBytes, nowIndex, cacheBytes, 0, cacheNum - nowIndex);
                cacheNum = cacheNum - nowIndex;
                break;
            }
        }
    }





###  客户端主动断开连接 

当前只实现了客户端与服务端的连接，未实现客户端的断开逻辑。              

1. 客户端需要关闭时先发送`QuitMsg`通知服务端。 
2. 客户端调用`Disconect()`做断开
3. 服务端需要对客户端的断开操作做处理，清理这边对应的记录

其中`QuitMsg`是没有消息体的消息，只有头部ID和长度信息。
                
|`断开流程`|客户端|服务端|
|--|--|--|
||客户端发送QuitMsg|服务端接收QuitMsg
||客户端调用Shutdown()|服务端记录该socket
||客户端调用`Disconect()`|服务端Shutdown该socket
||客户端关闭socket|服务端关闭该socket
|||服务端清理socket信息






###  心跳包







