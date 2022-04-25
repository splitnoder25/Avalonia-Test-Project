using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaApplication9
{
    public class MainWindow : Window
    {
        Thread ScanLocalArea = null;
        string IP;
        //int count;
        //private Button scan_button;
        private TextBox ipAddressControl1;
        private NumericUpDown numericUpDown1;
        private ProgressBar progressBar1;
        private TextBox ListBox1;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            //scan_button = this.FindControl<Button>("scan_button");
            ipAddressControl1 = this.FindControl<TextBox>("ipAddressControl1");
            numericUpDown1 = this.FindControl<NumericUpDown>("numericUpDown1");
            progressBar1 = this.FindControl<ProgressBar>("progressBar1");
            ListBox1 = this.FindControl<TextBox>("ListBox1");
        }

        async void Scan(string subnet)
        {
            Ping myPing;
            PingReply reply;
            System.Net.IPAddress addr;
            IPHostEntry host;
            var pbMin = 0;
            await Dispatcher.UIThread.InvokeAsync(() => pbMin = Convert.ToInt32(ipAddressControl1.Text.Substring(ipAddressControl1.Text.LastIndexOf('.') + 1)));
            //if (pbMin == 0) pbMin = pbMin + 1;
            var pbMax = 0;
            await Dispatcher.UIThread.InvokeAsync(() => pbMax = Convert.ToInt32(numericUpDown1.Text) + 1);

            string pbValue = (IP.Substring(0, IP.LastIndexOf('.')));
            await Dispatcher.UIThread.InvokeAsync(() =>
             {
                 progressBar1.Minimum = Convert.ToInt32(pbMin);
                 progressBar1.Maximum = Convert.ToInt32(pbMax);
                 progressBar1.Value = Convert.ToInt32(pbMin);
                //ListBox1.Items.Clear();
            });
            for (int i = Convert.ToInt32(pbMin); i < Convert.ToInt32(pbMax); i++)
            {
                string subnetn = "." + i.ToString();
                myPing = new Ping();
                reply = myPing.Send(subnet + subnetn, 900);

                if (reply.Status == IPStatus.Success)
                {
                    try
                    {
                        addr = System.Net.IPAddress.Parse(subnet + subnetn);
                        host = Dns.GetHostEntry(addr);
                        await Dispatcher.UIThread.InvokeAsync(() =>
                         {
                             ListBox1.Text = ListBox1.Text + (host.HostName.Replace(".local", "").Replace(".home", "") + "   (" + addr + ")") + "\n";
                         });
                    }

                    catch { }
                }
                await Dispatcher.UIThread.InvokeAsync(() =>
                 {
                     progressBar1.Value += 1;
                 });
            }
            await Dispatcher.UIThread.InvokeAsync(() =>
             {
                 ListBox1.IsEnabled = true;

                 ListBox1.Text = ListBox1.Text + (DateTime.Now.ToLongTimeString() + " : Поиск компьютеров завершен! Найдено: " + Environment.NewLine);
                 progressBar1.Minimum = 0;
                 progressBar1.Value = 0;


             });
            //ScanLocalArea.Suspend();
            //ScanLocalArea.Abort();
            ScanLocalArea = null;

        }

        private void Button_Scan(object sender, RoutedEventArgs e)
        {
            IP = ipAddressControl1.Text;
            if ((IP == "0.0.0.0") || (IP == "...") || (numericUpDown1.Text == ""))
            {

                
            }
            else
            {

                IP = ipAddressControl1.Text.Substring(0, ipAddressControl1.Text.LastIndexOf('.'));
                ScanLocalArea = new Thread(() => Scan(IP));
                ScanLocalArea.IsBackground = true;
                ScanLocalArea.Start();
                if (ScanLocalArea.IsAlive == true)
                {
                    ListBox1.IsEnabled = false;
                   
                    ListBox1.Text = ListBox1.Text + (DateTime.Now.ToLongTimeString() + " : Запуск сканирования заданного диапазона сети" + Environment.NewLine);
                }
            }
        }

    }
}
