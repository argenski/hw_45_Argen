using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpListener listener = new HttpListener();

            listener.Prefixes.Add("http://localhost:8888/hello_world/");
            listener.Prefixes.Add("http://localhost:8888/main_page/");
            listener.Start();

            Console.WriteLine("Ждем подключений");

            HttpListenerContext context = listener.GetContext();

            HttpListenerResponse response = context.Response;
            string fileName = context.Request.Url.AbsolutePath;

            string str = "";



            if (fileName == "/hello_world/")
            {
                str = "<html><head><meta charset='utf8'><style> body{ background-color:red; color:white;} </style></head><body> <h1>hello_world</h1></body></html>";
            } 
            else if (fileName == "/main_page/")
            {
                str = "<html><head><meta charset='utf8'><style> body{background-color:gray; color:red;}</style></head><body> <h1>main_page</h1></body></html>";
            }
                






            byte[] buffer = Encoding.UTF8.GetBytes(str);

            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;

            output.Write(buffer, 0, buffer.Length);

            output.Close();

            listener.Stop();

            Console.WriteLine("Соединение закрыто");

            Console.Read();
        }
    }
}