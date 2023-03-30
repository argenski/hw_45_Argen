﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lesson45
{
    internal class Server
    {
        private Thread _serverThread;
        private string _siteDirectory;
        private HttpListener _listener;
        private int _port;
        public Server(string path, int port)
        {
            Initialize(path, port);
        }

        private void Initialize(string path, int port)
        {
            _siteDirectory = path;
            _port = port;
            _serverThread = new Thread(Listen);
            _serverThread.Start();
            Console.WriteLine("Сервер запущен на порту " + _port);
            Console.WriteLine("файлы лежат в папке " + _siteDirectory);
        }

        private void Listen(object? obj)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{_port}/");
            _listener.Start();
            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void Process(HttpListenerContext context)
        {
            string fileName = context.Request.Url.AbsolutePath;
            Console.WriteLine(fileName);
            if(fileName == "/")
            {
                fileName = _siteDirectory + fileName+"index.html";
                if (File.Exists(fileName))
                {
                    try
                    {
                        Stream fileStream = new FileStream(fileName, FileMode.Open);
                        context.Response.ContentType = GetContentType(fileName);
                        context.Response.ContentLength64 = fileStream.Length;
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        byte[] buffer = new byte[16 * 1024];
                        int dataLength;
                        do
                        {
                            dataLength = fileStream.Read(buffer, 0, buffer.Length);
                            context.Response.OutputStream.Write(buffer, 0, dataLength);
                        }
                        while (dataLength > 0);
                        fileStream.Close();
                        context.Response.OutputStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            else if(fileName == "/white_rabbit/"|| fileName == "/white_rabbit/styles.css")
            {
                if(fileName == "/white_rabbit/styles.css")
                {
                    fileName = fileName.Remove(0, 13);
                    fileName = _siteDirectory + fileName;
                }
                else
                {
                    fileName = fileName.Remove(fileName.Length - 1);
                    fileName = _siteDirectory + fileName + ".html";
                }
                
                

                if (File.Exists(fileName))
                {
                    try
                    {
                        Stream fileStream = new FileStream(fileName, FileMode.Open);
                        context.Response.ContentType = GetContentType(fileName);
                        context.Response.ContentLength64 = fileStream.Length;
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        byte[] buffer = new byte[16 * 1024];
                        int dataLength;
                        do
                        {
                            dataLength = fileStream.Read(buffer, 0, buffer.Length);
                            context.Response.OutputStream.Write(buffer, 0, dataLength);
                        }
                        while (dataLength > 0);
                        fileStream.Close();
                        context.Response.OutputStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }
                }
            }
            else
            {
                fileName = "";
                fileName = _siteDirectory + "/501.html";
                if (File.Exists(fileName))
                {
                    

                    Stream fileStream = new FileStream(fileName, FileMode.Open);
                    context.Response.ContentType = GetContentType(fileName);
                    context.Response.ContentLength64 = fileStream.Length;
                    context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    byte[] buffer = new byte[16 * 1024];
                    int dataLength;
                    do
                    {
                        dataLength = fileStream.Read(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Write(buffer, 0, dataLength);
                    }
                    while (dataLength > 0);
                    fileStream.Close();
                    context.Response.OutputStream.Flush();
                }
            }
            
            context.Response.OutputStream.Close();
        }

        private string GetContentType(string fileName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                {".css", "text/css"},
                {".html", "text/html"},
                {".ico", "image/x-icon" },
                {".js", "application/x-javascript" },
                {".json", "application/json" },
                {".png", "image/png" }
            };
            string contentType = "";
            string fileExt = Path.GetExtension(fileName);
            dictionary.TryGetValue(fileExt, out contentType);
            return contentType;
        }

        public void Stop() {
            _serverThread.Abort();
            _listener.Stop();
        }
    }
}
