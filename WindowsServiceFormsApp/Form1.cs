using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.ServiceProcess;
using System.Windows.Forms;

namespace WindowsServiceFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartService();
            LoadLogs();
        }

        private void btnLoadLogs5_Click_1(object sender, EventArgs e)
        {
            LoadLogs();
        }

        private void LoadLogs()
        {
            string logFilePath = @"C:\Users\TkN\Source\repos\WindowsService\WindowsService\bin\Debug\Logs\servisim.txt";

            if (File.Exists(logFilePath))
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.AddRange(new DataColumn[] {
            new DataColumn("Tarih"),
            new DataColumn("BilgisayarAdi"),
            new DataColumn("KullaniciAdi")
        });

                string[] lines = File.ReadAllLines(logFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(' ');

                    string tarih = parts[0] + " " + parts[1];
                    string bilgisayarAdi = parts[2];
                    string kullaniciAdi = parts[3];

                    dataTable.Rows.Add(tarih, bilgisayarAdi, kullaniciAdi);
                }

                dataGridView5.DataSource = dataTable;
            }
            else
            {
                MessageBox.Show("Log dosyası bulunamadı.");
            }
        }



        private void StartService()
        {
            string frameworkPath = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\";
            string installUtilPath = Path.Combine(frameworkPath, "InstallUtil.exe");
            string servicePath = @"C:\Users\TkN\Source\repos\WindowsService\WindowsService\bin\Debug\WindowsService.exe";

            try
            {
                if (!IsAdmin())
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo();
                    processInfo.Verb = "runas";
                    processInfo.FileName = Application.ExecutablePath;

                    try
                    {
                        Process.Start(processInfo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Yönetici olarak çalıştırma hatası: " + ex.Message);
                    }

                    Close();
                    return;
                }

                ProcessStartInfo installProcessInfo = new ProcessStartInfo
                {
                    FileName = installUtilPath,
                    Arguments = $"/i \"{servicePath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process installProcess = Process.Start(installProcessInfo);
                installProcess.WaitForExit();

                string output = installProcess.StandardOutput.ReadToEnd();
                MessageBox.Show("Servis yükleme işlemi tamamlandı.\n\nÇıktı:\n" + output);

                string serviceName = "Service1";

                ServiceController serviceController = new ServiceController(serviceName);

                try
                {
                    if (serviceController.Status == ServiceControllerStatus.Stopped)
                    {
                        serviceController.Start();
                        serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10)); 
                        Console.WriteLine("Servis başlatıldı.");
                    }
                    else
                    {
                        Console.WriteLine("Servis zaten çalışıyor.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Servis başlatılırken bir hata oluştu: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private bool IsAdmin()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

    }
}
