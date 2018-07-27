
using System;
using System.Text;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using System.Collections.Generic;
using System.IO;

public class Porttest
{
    /// <summary>
    /// 짜깁기한 이 : 수우
    /// 밑에 기본 검색 기능 출처 : 인터넷....
    /// 메인 함수 : argument를 받아서 해당 텍스트 파일에 저장된 ip주소들에 tcp 요청을 보내 각각 포트 오픈 여부 확인
    /// 생략할 경우 기본 포트 : 80
    /// </summary>
    /// <param name="args">Usage : CheckPort.exe [] [Port]"</param>
    public static void Main(string[] args)
    {
        string ipTextListPath = args.Length > 0 ? args[0] : "";
        if (args.Length < 2 || !Int32.TryParse(args[1], out int port) || string.IsNullOrEmpty(args[1])) port = 80;


#if DEBUG
        Console.Write("파일 경로를 입력 (생략시 기본값):");
        ipTextListPath = Console.ReadLine();
        if (string.IsNullOrEmpty(ipTextListPath)) ipTextListPath = @"C:\Users\USER\Desktop\작업\work\porttest\bin\Debug\iplist.txt";
        Console.Write("포트 번호를 입력(생략시 80):");
        if (!int.TryParse(Console.ReadLine(), out port)) port = 80;
#endif
        pingTestAll(ipTextListPath, port);

#if DEBUG
        Console.ReadKey();
#endif

    }

    //텍스트 파일에서 ip 주소 추출 후에 테스트 진행
    private static void pingTestAll(string filePath, int port)
    {
        string[] ipArray;
        if (!File.Exists(filePath))
        {
            Console.WriteLine("파일 경로가 잘못되었습니다.");
            return;
        }
            
        using (var tr = new System.IO.StreamReader(filePath))
        {
            //줄바꿈 키로 리스트를 분할
            ipArray = tr.ReadToEnd().Split(Environment.NewLine.ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
        }

        foreach (var a in ipArray)
        {
            pingTest(a, port);
        }
    }

    private static void pingTest(string ip, int port)
    {
        try
        {
            //if (PingTest(ip))
            //{
            //    Console.Write("{0} PING OK \t", ip);

                if (ConnectTest(ip, port))
                {
                    Console.WriteLine("{0}:{1} is open.", ip, port);
                }
                else
                {
                    Console.WriteLine("{0}:{1} is closed.", ip, port);
                }
            //}
            //elses
            //{
            //    Console.WriteLine("{0} PING NG", ip);
            //}
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }


    }


    private static bool PingTest(string ip)
    {
        bool result = false;
        try
        {
            Ping pp = new Ping();
            PingOptions po = new PingOptions();

            po.DontFragment = true;

            byte[] buf = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaa");

            PingReply reply = pp.Send(
                IPAddress.Parse(ip),
                10, buf, po
            );

            if (reply.Status == IPStatus.Success)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        catch
        {
            throw;
        }
    }


    private static bool ConnectTest(string ip, int port)
    {
        bool result = false;

        Socket socket = null;
        try
        {
            socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            socket.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.DontLinger,
                false
            );


            IAsyncResult ret = socket.BeginConnect(ip, port, null, null);

            //timeout뒤에 강제로 소켓을닫음.
            result = (ret.AsyncWaitHandle.WaitOne(2000, true));
            
        }
        catch { }
        finally
        {
            if (socket != null)
            {
                socket.Close();
            }
        }
        return result;
    }
}
