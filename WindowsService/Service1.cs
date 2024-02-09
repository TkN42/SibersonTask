using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;


namespace WindowsService
{
    public partial class Service1 : ServiceBase
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Servis başladı " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 1000;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteLog("Servis durdu " + DateTime.Now);
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            string tarih = DateTime.Now.ToString();
            string bilgisayarAdi = Environment.MachineName;
            string kullaniciAdi = Environment.UserName; 

            WriteLog($"[{tarih}] [{bilgisayarAdi}] [{kullaniciAdi}]");
        }

        private void WriteLog(string mesaj)
        {
            string dosyaYolu = AppDomain.CurrentDomain.BaseDirectory + "/Logs";
            if (!Directory.Exists(dosyaYolu))
            {
                Directory.CreateDirectory(dosyaYolu);
            }

            string textYolu = AppDomain.CurrentDomain.BaseDirectory + "/Logs/servisim.txt";
            if (!Directory.Exists(textYolu))
            {
                using (StreamWriter sw = File.CreateText(textYolu))
                {
                    sw.WriteLine(mesaj);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(textYolu))
                {
                    sw.WriteLine(mesaj);
                }
            }
        }

    }
}
