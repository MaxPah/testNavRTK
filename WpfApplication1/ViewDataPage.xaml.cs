using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;
using WpfApplication1.Model;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class ViewDataPage : Page
    {

         #region variables

        // Number Of Objects
        Queue<Object> list = new Queue<Object>();
               
        //Serial 
        string recieved_data;
            SerialPort sp;
            #endregion


        public ViewDataPage()
        {
           
            InitializeComponent();
            string[] listPort = SerialPort.GetPortNames();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
                     
                
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
           
        }

        private delegate void UpdateUiTextDelegate1(MessageGPGGA objGGA);
        private delegate void UpdateUiTextDelegate2(MessageGPRMC objRMC);

        private void Recieve(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                recieved_data = sp.ReadLine();
                string msg;
                 //List<Object> list = new List<Object>();
                msg = GPSParsor.splitMessage(recieved_data, list);

                Console.WriteLine(list.Count());

                if ((list.Count-1) != null)
                    if (list.Peek().GetType() == typeof(MessageGPGGA))
                        Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate1(WriteDataGGA), list.Peek());
                    else if (list.Peek().GetType() == typeof(MessageGPRMC))
                        Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate2(WriteDataRMC), list.Peek());
            }
            catch
            {
                
            }
             
          
        }

        private void WriteDataGGA(MessageGPGGA objGGA)
        {
            GridGGA.DataContext = objGGA;

            GridGGATime.Text = objGGA.timeUTC.ToString();
            GridGGALat.Text = objGGA.latitude.ToString("F6");
            GridGGALon.Text = objGGA.longitude.ToString("F6");
            GridGGAQual.Text = objGGA.gpsQuality.ToString();
            GridGGANSat.Text = objGGA.nSat.ToString();
            if (objGGA.nSat < 0)
                GridGGAnSatFather.Background = Brushes.Transparent;
            else if (objGGA.nSat >= 0 && objGGA.nSat < 3)
                GridGGAnSatFather.Background = Brushes.Red;
            else if (objGGA.nSat >= 3 && objGGA.nSat < 5)
                GridGGAnSatFather.Background = Brushes.Orange;
            else if (objGGA.nSat >= 5 && objGGA.nSat <= 14)
                GridGGAnSatFather.Background = Brushes.Green;
            else GridGGAnSatFather.Background = Brushes.Transparent;
            GridGGADil.Text = objGGA.dilution.ToString("F2");
            GridGGAAlt.Text = objGGA.altitude.ToString("F2") + objGGA.altUnit.ToString();
            GridGGAGeo.Text = objGGA.geoidal.ToString("F2") + objGGA.geoUnit.ToString();
            GridGGADGPSUTC.Text = objGGA.dGPSTime.ToString();
        }

        private void WriteDataRMC(MessageGPRMC objRMC)
        {
            GridRMC.DataContext = objRMC;

            GridRMCTime.Text = objRMC.timeUTC.ToString();
            GridRMCLat.Text = objRMC.latitude.ToString("F6");
            GridRMCLon.Text = objRMC.longitude.ToString("F6");
            GridRMCVal.Text = objRMC.status.ToString();
            GridRMCSpeed.Text = objRMC.speed.ToString("F2");
            GridRMCCap.Text = objRMC.cap.ToString();
            GridRMCDGPSUTC.Text = objRMC.date.ToString();
            GridRMCMagn.Text = objRMC.magnetic.ToString();
            GridRMCModePos.Text = objRMC.integrity.ToString();

            if (objRMC.speed != 0)
                GridRMCArcSpeed.EndAngle = (100 + objRMC.speed * 100) - 90;
            else GridRMCArcSpeed.EndAngle = -90;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           if ( OnOffButton.IsCancel == true)
           {
               sp = new SerialPort("COM1", 115200);

               if (!sp.IsOpen)
                   sp.Open();



               sp.DataReceived += new SerialDataReceivedEventHandler(Recieve);

               OnOffButton.IsCancel = false;
               OnOffButton.Background = (SolidColorBrush)this.FindResource("secondColor");
               LabelOnOffButton.Content = "On";
                OffRect.Visibility = System.Windows.Visibility.Visible;
                OnRect.Visibility = System.Windows.Visibility.Hidden;
           }
           else
           {
               try
               {
                   sp.Close();
                   OnOffButton.IsCancel = true;
                    OnOffButton.Background = (SolidColorBrush)this.FindResource("Transparent");
                    LabelOnOffButton.Content = "Off";
                    OnRect.Visibility = System.Windows.Visibility.Visible;
                    OffRect.Visibility = System.Windows.Visibility.Hidden;
                   

               }
               catch
               {
               }
           }
        }

        private void goToConnexion(object sender, MouseButtonEventArgs e)
        {
            if (sp != null)
                if (sp.IsOpen)
                    sp.Close();

            this.NavigationService.Navigate(new SettingsPage());
        }

    }
}