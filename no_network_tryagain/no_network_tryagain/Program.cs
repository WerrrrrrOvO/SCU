using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

class InternetChecker
{
    static Timer _timer;

    static void Main(string[] args)
    {
       
        Console.WriteLine("互联网检测程序启动，每20分钟检查一次...");

        // 立即执行一次检查
        CheckInternetConnection();

        // 创建定时器，每10分钟触发一次
        _timer = new Timer(CheckInternetConnectionCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(20));

        Console.WriteLine("按任意键退出程序...");
        Console.ReadKey();

        // 退出程序时，停止定时器
        _timer.Dispose();
        Console.WriteLine("程序已退出。");
    }

    static void CheckInternetConnectionCallback(object state)
    {
        CheckInternetConnection();
    }

    static void CheckInternetConnection()
    {
        Console.WriteLine($"{DateTime.Now}: 开始检查互联网连接...");

        string testUrl = "http://www.baidu.com"; // 百度首页URL
        if (!IsConnectedToInternet(testUrl))
        {
            Console.WriteLine("未连接到互联网，尝试打开Edge浏览器...");
            OpenEdgeBrowser();
        }
        else
        {
            Console.WriteLine("已连接到互联网。");
        }

        Console.WriteLine("检查结束。");
    }

    static bool IsConnectedToInternet(string url)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 5000; // 设置请求超时时间（毫秒）
            request.Method = "GET"; // 使用GET方法获取页面内容

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string pageContent = reader.ReadToEnd();
                        // 使用正则表达式检查页面内容是否包含“百度一下”
                        return Regex.IsMatch(pageContent, "百度一下", RegexOptions.IgnoreCase);
                    }
                }
            }
        }
        catch (WebException ex)
        {
            Console.WriteLine($"检查互联网连接时发生错误: {ex.Message}");
        }
        return false;
    }


    static void OpenEdgeBrowser()
    {
        try
        {
            // 指定Edge浏览器的可执行文件路径
            string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";

            // 使用Process.Start来启动Edge浏览器
            // 这里不传递任何URL参数，因此Edge将打开其默认的起始页
            Process edgeProcess = Process.Start(new ProcessStartInfo
            {
                FileName = edgePath,
                UseShellExecute = true // 设置为true以使用shell执行
            });

            Console.WriteLine($"{DateTime.Now}: Edge浏览器已启动。");

            // 等待2分钟后尝试关闭浏览器
            Thread.Sleep(TimeSpan.FromMinutes(2));
            Console.WriteLine($"{DateTime.Now}: 2分钟已过，尝试关闭Edge浏览器...");

            // 尝试优雅关闭Edge浏览器
            if (!edgeProcess.HasExited)
            {
                edgeProcess.CloseMainWindow();
                // 给浏览器一点时间来响应关闭请求
                edgeProcess.WaitForExit(5000); // 等待5秒

                if (edgeProcess.HasExited)
                {
                    Console.WriteLine($"{DateTime.Now}: Edge浏览器已成功关闭。");
                }
                else
                {
                    // 如果浏览器没有响应关闭请求，则强制结束进程
                    edgeProcess.Kill();
                    Console.WriteLine($"{DateTime.Now}: Edge浏览器已被强制关闭。");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"无法打开Edge浏览器: {ex.Message}");
        }
    }
}