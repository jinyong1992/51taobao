using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using WpfApplication1.Http_sock;
using System.Text.RegularExpressions;
using System.Collections;


/// <summary>
/// 作者:jinyong
/// 建项时间:2015-9-23 14:18
/// QQ: 757210476
/// </summary>

namespace _51taobao
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int mode);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int handle);

        const int STD_INPUT_HANDLE = -10;
        const int ENABLE_QUICK_EDIT_MODE = 0x40 | 0x80;
        /// <summary>
        /// 击鼠标右键粘贴
        /// </summary>
        static void Modeln()
        {
            int mode;
            IntPtr handle = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(handle, out mode);
            mode |= ENABLE_QUICK_EDIT_MODE;
            SetConsoleMode(handle, mode);
        }
        /// <summary>
        /// 检查宝贝链接
        /// </summary>
        static void Conttao()
        {
           // Stream inputStream = Console.OpenStandardInput(90000);
//Console.SetIn(new StreamReader(inputStream));
            Console.WriteLine("宝贝地址:");
            string taolink = Console.ReadLine();

            while (!taolink.Contains("51zwd"))
            {
                Console.WriteLine("重新输入,宝贝地址:");
                taolink = Console.ReadLine();
                

            }
            Console.WriteLine(@"按两次回车，则宝贝信息将会保存在桌面, 或输入保存路径例如""C:\taoinfo\""" + "\r\n");
            Console.WriteLine("文件保存路径:");
             sdatapath = Console.ReadLine();

            if (taolink.Contains(','))
            {
                //批量处理
                string[] ai = taolink.Split(',');
                foreach(string lnke in ai)
                {
                    Gethtmlcode(lnke);
                }
            }
            else
            {
                Gethtmlcode(taolink);
            }
                

        }
        static public string sdatapath { get; set; }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="taolink"></param>
        static void Gethtmlcode(string taolink)
        {
            GpHttpapp ghttp = new GpHttpapp();
            ghttp.GetHttp(taolink);
            if(ghttp.getcode !=null)
            {
                taoinfo.link = taolink;
                Getinfo(ghttp.getcode);
                Getimglink(ghttp.getcode);
            }

        }
        /// <summary>
        /// 取宝贝信息
        /// </summary>
        static void Getinfo(string taocode)
        {
            Regex reinfo_title = new Regex(@" content="",([^,]+)", RegexOptions.IgnoreCase);
           
          Match  matche_title = reinfo_title.Match(taocode);
            taoinfo.title = matche_title.Value.Split(',')[1];
          //  taoinfo.title = taoinfo.title.Split(',')[1];

            Regex reinfo_costprice = new Regex(@"&yen;</span>[0-9]*", RegexOptions.IgnoreCase);
            Match matche_costprice = reinfo_costprice.Match(taocode);
            taoinfo.costprice = matche_costprice.Value.Split('>')[1];
            

            Regex reinfo_stallsid = new Regex(@"(?:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;)(.*?)</span>", RegexOptions.IgnoreCase);
            Match matche_stallsid = reinfo_stallsid.Match(taocode);
            taoinfo.Stallsid = matche_stallsid.Value.Split(';')[6].Split('<')[0];

            Regex reinfo_taotime = new Regex(@"(fontColor12"">&nbsp;&nbsp;&nbsp;)(.*?)</span>", RegexOptions.IgnoreCase);
            MatchCollection matches_info = reinfo_taotime.Matches(taocode);
            Console.WriteLine(matches_info[3]);
            taoinfo.taotime = matches_info[3].Value.Split(';')[3].Split('<')[0];
            Console.WriteLine(matches_info[3]);

            // System.Windows.Forms.MessageBox.Show(taoinfo.title);


        }
        static void getoption_box(string taocode)
        {
            Regex optionbox = new Regex(@"<li>(.*?)<li>", RegexOptions.IgnoreCase);
            MatchCollection mbox = optionbox.Matches(taocode);
            //   System.Windows.Forms.MessageBox.Show(taocode);
            int i=0;
            //string[] sd = new string[mbox.Count];
            taoinfo.option_box = new string[mbox.Count];
         foreach(Match s in mbox)
            {
                if(!s.Value.Contains("img"))
                {
                     taoinfo.option_box[i] = s.Value.Replace("<li>", "");
                    i++;
                }
            }
          //  System.Windows.Forms.MessageBox.Show(mbox[1].Value.Replace("<li>",""));
        }
        /// <summary>
        /// 正则取图片地址
        /// </summary>
        /// <param name="taocode"></param>
        static void  Getimglink(string taocode)
        {
            Console.WriteLine(taocode+"\r\n");
            Console.WriteLine("解析成功......\r\n");
           // Regex reimg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>
                                                //[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            Regex reimg = new Regex(@"(https|http)://[^'""]+\.(jpg|png)", RegexOptions.IgnoreCase);

            MatchCollection matches = reimg.Matches(taocode);
            int i = 0;
            ArrayList simglist = new ArrayList();


            if (matches.Count > 1)
            {
                foreach(Match match in matches)
                {
                    string imgv = match.Value;
                    //string imgv=   match.Groups["imgUrl"].Value;
                    if (imgv.Contains("460x460"))
                    {
                        //System.Windows.Forms.MessageBox.Show(imgv.ToString());
                        simglist.Add(imgv.Replace("_460x460.jpg", ""));
                    }
                    else if (imgv.Contains("240x240") || imgv.Contains("180x180")|imgv.Contains("51zwd"))
                    {
                       //过虑 
                    }
                    else
                    {
                        simglist.Add(imgv);

                    }
                   
                }
                i = 0;
                string[] imglist = new string[simglist.Count];
                foreach (string s in simglist)
                {
                    //动态数组转静态
                    imglist[i] = s;
                    i++;
                }
                getoption_box(taocode);
                Downloadimg(ref imglist);
            }

        }
        static void Downloadimg(ref string[]  imglink)
        {
           
            //string sdatapath=sdatapath;
            string datapath = sdatapath;
            FileStream create;
            StreamWriter log;
            if (!datapath.Contains("\\") )
            {
                //判断有没有输入路径，没有则保存在桌面
              datapath=  Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"\\"+"详情页"+"\\"+taoinfo.title+"\\"+ "images\\";
                Directory.CreateDirectory(datapath);
                create = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "详情页"+"\\" + taoinfo.title +"\\"+ "taoinfo.txt", FileMode.Create, FileAccess.Write);
                create.Close();
                log = File.AppendText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + "详情页"+"\\" + taoinfo.title +"\\"+ "taoinfo.txt");
        }
               else
            {
                if (!datapath.EndsWith("\\"))
                {
                    //判断输入的路径 后面有没有“\”符号,没有则执行
                    datapath = Directory.CreateDirectory(datapath + "\\" + taoinfo.title + "\\").FullName;
                    create = new FileStream(sdatapath + "\\" + "taoinfo.txt", FileMode.Create, FileAccess.Write);
        create.Close();
                    log = File.AppendText(sdatapath + "\\" + "taoinfo.txt");

                }
                else {
                    string taoinfopath = Directory.CreateDirectory(datapath).FullName;
                     create = new FileStream(taoinfopath + "taoinfo.txt", FileMode.Create, FileAccess.Write);
                      create.Close();
                    log = File.AppendText(taoinfopath + "taoinfo.txt");

                    datapath = Directory.CreateDirectory(datapath + taoinfo.title + "\\").FullName;
                }


      }
//else
//{
//    //if (!datapath.EndsWith("\\"))
//    //{
//    //    datapath = Directory.CreateDirectory(datapath+"\\" + taoinfo.title + "\\").FullName;
//    //    create = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + taoinfo.title + "taoinfo.txt", FileMode.Create, FileAccess.Write);
//    //    create.Close();
//    //    log = File.AppendText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + taoinfo.title + "taoinfo.txt");
//    //}





//###############################################################################
//FileStream create = new FileStream(datapath + "taoinfo.txt", FileMode.Create,FileAccess.Write);
//create.Close();
//StreamWriter log = new StreamWriter(datapath + "taoinfo.txt");



            log.Write("标题   "+taoinfo.title+ "\r\n");
            log.Write("\r\n");
            log.Write("地址   "+taoinfo.link + "\r\n");
            log.Write("\r\n");
            log.Write("档口上款时间  " + taoinfo.taotime);
            log.Write("\r\n");
            log.Write("拿货价  " + taoinfo.costprice +"元"+ "\r\n");
            log.Write("\r\n");
            log.Write("档口   " + taoinfo.Stallsid + "\r\n");
            log.Write("\r\n");
            //###############################################################################
            foreach (string s in taoinfo.option_box)
            {
                log.WriteLine(s + "\r\n");
            }
            log.Write("#######################################" + "\r\n");
            log.Write("详情页图片地址:" + "\r\n");
            log.Write("\r\n");
            WebClient mywebclient = new WebClient();

            for (int i =1;i < imglink.Length;i++)
            {
                mywebclient.DownloadFile(imglink[i - 1], datapath+ i + ".jpg");
                //mywebclient.DownloadFile(imglink[i-1],@"c:" +i.ToString()+".jpg");
                log.Write(imglink[i-1]+"\r\n");
                Console.WriteLine(imglink[i-1] + "\r\n");
       
            }
            Console.WriteLine("下载成功" + imglink.Length.ToString() + "张");
            log.Close();
            Console.WriteLine("");
          //  Conttao();
        }

        /// <summary>
        /// 突破Console.ReadLive()的256个字符输入长度限制
        /// </summary>
        static void inputStream()
        {
            Stream inputStream = Console.OpenStandardInput(5000);
            Console.SetIn(new StreamReader(inputStream));
        }
        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Title = "51zwd.com 详情页图片下载 by jinyong 2015-9-24";
            inputStream();
            Modeln();

            Conttao();
         
        }

        
    }

    class taoinfo
    {
        /// <summary>
        /// 标题
        /// </summary>
       static public string title { get; set; }
        /// <summary>
        /// 宝贝地址
        /// </summary>
      static  public string link { get; set; }
        /// <summary>
        /// 拿货价
        /// </summary>
       static public string costprice { get; set; }
        /// <summary>
        /// 档口号
        /// </summary>
       static public string Stallsid { get; set; }
        /// <summary>
        /// /属性
        /// </summary>
        static public string[] option_box { get ; set; }
        /// <summary>
        /// 上款时间
        /// </summary>
        static public string taotime { get; set; }

    }
}
