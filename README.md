# IISExpressConfig
IISExpress 局域网调试设置工具

## 说明
VS2013内置了IISExpress。
做asp.net MVC的web项目开发时，Ctrl+F5和F5启动项目运行（后者是调试模式）的同时都会打开IISExpress，事实上本机对该web项目走的就是这个内置的server。默认情况下，该server运行的web项目不支持局域网内其他机器的访问。为了做到这一点：

1、关闭防火墙。
2、修改C:\Users\Administrator\Documents\IISExpress\config目录下的applicationhost.config的文件：
　　查找你的web的项目的名称或者你的web项目运行起来的端口号，找到该web项目的bindings项，增加一个binding配置：

<bindings>
<binding protocol="http" bindingInformation="*:3859:localhost" />a//这一行是默认生成的
<binding protocol="http" bindingInformation="*:3859:192.168.1.108" />//这一行是我为了支持外部访问而配置的，IP就是本机在局域网内的IP
</bindings>

好了，现在其他机器（包括使用了局域内的wifi的手机）也可以访问了。

##附上简单工具
