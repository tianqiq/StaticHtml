# staticHtml使用教程

## 阅读目录

[TOC]

##简介

staticHtml是asp.net平台的一个开源，高效，易扩展的网页静态化组件，不依赖任何第三方库，简单小巧dll文件不足20k。

## 特性

1. 无需更改您任何代码，只需要简单的配置，即可使您站点具有html静态化的能力，提高您网站的性能，
2. 完全使用[GZip](http://www.baidu.com/w?d=)存储，输出，减少服务器硬盘消耗，大大减少网络带宽
3. 支持内存缓存，您可以按照您的需要，将部分缓存页面， 直接缓存到内存，提高性能，比如说常用的首页
4. 自动判断30*，50*等输出头，不进行缓存
5. 缓存策略灵活，系统支持时间过期策略，支持查询数据库过期策略

## 如何使用

### 1.  在web.config文件configSections节点中添加配置读取节点如下
```xml
<section name="staticHtml" type="StaticHtml.StaticHtmlSection,StaticHtml"/>
```
### 2. 在system.web/httpmodules节点中添加如下节点
```xml
<add name="staticnet" type="StaticHtml.HttpModule"/>
```
### 3. 在config节点中添加如下节点
```xml
<staticHtml skip="admin/" run="on">
	<rule name="index">
		<patten type="StaticHtml.RegexPatten,StaticHtml" pars="RegPatten=index.aspx$"/>
		<store type="StaticHtml.MemStore,StaticHtml"/>
	</rule>
	<rule name="Content">
		<patten type="StaticHtml.RegexPatten,StaticHtml" pars="RegPatten=Content/"/>
		<store type="StaticHtml.FileStore,StaticHtml" pars="Path=cacheHtml_Content/"/>
		<expire type="StaticHtml.TimeExpire,StaticHtml" pars="Second=180"/>
		<genkey type="StaticHtml.UrlMd5GenKey,StaticHtml"/>
	</rule>
</staticHtml>
```
### 4. over 
配置完成，当您访问您的页面domain/index.aspx时候，系统将缓存您的页面到内存，每5分钟刷新一次。当您访问的您的网页，url中如果保护有Content/的时候，缓存180秒，缓存内容保存到cacheHtml_Content目录下面，采用访问的url md5加密的形式作为系统的唯一的key，也是文件存储的名字。 

## 如何扩展
或许大家看到配置文件的时候也猜到一些东西，对，上面那些配置都是可以自定义的，可以简单的实现一些接口，即可实现您自己的功能。事实上，这些配置就是为了让您回答这么几个问题

1. 您需要缓存什么样的页面
2. 您缓存的东西存在哪里
3. 你采用一个什么样的缓存过期策略
4. 您通过什么样的方式，将你缓存的页面，生成一个系统唯一的key， 在系统里面， 这个key就代表了这个页面。 也将用这个key生成的缓存文件的文件名

这4个问题，您都可以自定义的回答，只需要简单的继承系统里面的接口，配置上去就可以

* 您需要缓存什么样的页面
```csharp
	public interface IPatten
    {
        /// <summary>
        /// 将HttpRequest判断是否匹配Rule规则
        /// </summary>
        /// <param name="request">HttpRequest请求</param>
        /// <returns>是否匹配</returns>
        bool IsPatten(HttpRequest request);
    }
```
* 您缓存的东西存在哪里
```csharp
 	/// <summary>
    /// 存储生成的Html缓存内容接口
    /// </summary>
    public interface IStore
    {
        /// <summary>
        /// 存储Html
        /// </summary>
        /// <param name="key">HttpRequest生成的key</param>
        /// <param name="repInfo">内容</param>
        void Save(String key, Stream repInfo);
       
        /// <summary>
        /// 获取缓存的Html
        /// </summary>
        /// <param name="key">HttpRequest生成的key</param>
        /// <returns>内容</returns>
        Stream Get(String key);

        /// <summary>
        /// 查询缓存信息
        /// </summary>
        /// <param name="key">HttpRequest生成的key</param>
        /// <returns>HtmlInfo包含存储时间，大小等</returns>
        CacheInfo Query(String key);
    }
```
*  你采用一个什么样的缓存过期策略
```csharp
  	/// <summary>
    /// 该缓存内容是否过期判断接口
    /// </summary>
    public interface IExpire
    {
        /// <summary>
        /// 是否过期
        /// </summary>
        /// <param name="req">当前HttpReq</param>
        /// <param name="info">缓存信息</param>
        /// <returns>是否过期</returns>
        bool IsExpire(HttpRequest req, CacheInfo info);
    }
```
* 您通过什么样的方式，将你缓存的页面，生成一个系统唯一的key
```csharp
	/// <summary>
    /// 将HttpRequest生成唯一key接口
    /// </summary>
    public interface IGenKey
    {
        /// <summary>
        /// 根据HttpRequest生成唯一key
        /// </summary>
        /// <param name="request">HttpRequest请求</param>
        /// <returns>唯一key</returns>
        String GenKey(HttpRequest request);
    }
```
大家可以查看源码里面上面的配置文件的配置项，即可查看系统是如何实现这些接口的。

## 其他
首先感谢大家花这么多的时间，看完这篇文档，因为我知道大家都很忙。 做这个花费了我很多的时间，开发过程中，碰到了各种问题，本来以为是个很简单的东西，但是却碰到一些奇怪的问题，比方说高并发测试的时候性能非常低，甚至直接去卡死整个站点，无法接受任何请求，找出这些问题，我调试了很多。由于是多线程应用，调试起来也非常麻烦。可能也是因为功力不够吧，网页静态化，在某些场合还是很合适的，它能提高一些服务器的吞吐量。也希望能大家测试与使用，并给我反馈，当然了如果您愿意共享您的智慧，那就更好了，源码我托管在gzip。有什么问题，可以发送邮件到这个邮箱[tianqiq@gmai.com](mailto:tianqiq@gmail.com)。谢谢！