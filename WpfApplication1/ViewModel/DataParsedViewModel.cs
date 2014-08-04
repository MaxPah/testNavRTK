using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using WpfApplication1.Model;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Data;
using System.ComponentModel;

namespace WpfApplication1.ViewModel
{
    public class DataParsedViewModel : INotifyPropertyChanged
    {
        #region variables

        // Number Of Objects
        Queue<Object> list = new Queue<Object>();

        //Serial 
        string recieved_data;
        SerialPort sp;
        #endregion


        public DataParsedViewModel()
        {

            sp = new SerialPort("COM1", 115200);

            if (!sp.IsOpen)
                sp.Open();
            sp.DataReceived += new SerialDataReceivedEventHandler(Recieve);


        }

        public delegate void UpdateUiTextDelegate1(MessageGPGGA objGGA);
        public delegate void UpdateUiTextDelegate2(MessageGPRMC objRMC);

        public void Recieve(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                recieved_data = sp.ReadLine();

                GPSParsor.splitMessage(recieved_data, list);

                //  Console.WriteLine(list.Count());

                if (list.Last().GetType() == typeof(MessageGPGGA))
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate1(WriteDataGGA), list.Last());
                if (list.Last().GetType() == typeof(MessageGPRMC))
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate2(WriteDataRMC), list.Last());
            }
            catch
            {

            }


        }

        public void WriteDataGGA(MessageGPGGA objGGA)
        {
            Console.WriteLine("gga");

            position = objGGA.latitude.ToString("F8");

            //PropertyChanged(position);
            Console.WriteLine(position);

            /*
            GridGGA.DataContext = objGGA;

            GridGGATime.Text = objGGA.timeUTC.ToString("d MMMM yyyy hh:mm:ss");
            GridGGALat.Text = objGGA.latitude.ToString("F6"); 
            GridGGALon.Text = objGGA.longitude.ToString("F6");
            GridGGAQual.Text = objGGA.gpsQuality.ToString();
            GridGGANSat.Text = objGGA.nSat.ToString();
            
            if (objGGA.nSat >= 0 && objGGA.nSat < 3)
                GridGGAnSatFather.BorderBrush = Brushes.Red;
            else if (objGGA.nSat >= 3 && objGGA.nSat < 5)
                GridGGAnSatFather.BorderBrush = Brushes.Orange;
            else if (objGGA.nSat >= 5 && objGGA.nSat <= 14)
                GridGGAnSatFather.BorderBrush = Brushes.Green;
            else GridGGAnSatFather.BorderBrush = Brushes.Transparent;

            GridGGADil.Text = objGGA.dilution.ToString("F2");
            GridGGAAlt.Text = objGGA.altitude.ToString("F2") + objGGA.altUnit.ToString();
            GridGGAGeo.Text = objGGA.geoidal.ToString("F2") + objGGA.geoUnit.ToString();
            GridGGADGPSUTC.Text = objGGA.dGPSTime.ToString("d MMM yyyy");
             */
        }

        public static void WriteDataRMC(MessageGPRMC objRMC)
        {

            Console.WriteLine("rmc");
            /*
            GridRMC.DataContext = objRMC;

            GridRMCVal.Text = objRMC.status.ToString();
            GridRMCSpeed.Text = objRMC.speed.ToString("F2");
            GridRMCCap.Text = objRMC.cap.ToString();
            GridRMCDGPSUTC.Text = objRMC.date.ToString("d MMM yyyy");
            GridRMCMagn.Text = objRMC.magnetic.ToString();
            GridRMCModePos.Text = objRMC.integrity.ToString();

            if (objRMC.speed != 0)
                GridRMCArcSpeed.Value = (System.DateTime.Now.Second) * 2; 

            //if (objRMC.speed != 0)
            //    GridRMCArcSpeed.EndAngle = (System.DateTime.Now.Second) *3; 

            //else GridRMCArcSpeed.EndAngle = 0;
            */
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
           if ( OnOffButton.IsCancel == true)
           {
               sp = new SerialPort("COM1", 115200);

               if (!sp.IsOpen)
                   sp.Open();


            */

            sp.DataReceived += new SerialDataReceivedEventHandler(Recieve);

            /*
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
            */
        }

        private void goToConnexion(object sender, MouseButtonEventArgs e)
        {
            if (sp != null)
                if (sp.IsOpen)
                    sp.Close();

            // this.NavigationService.Navigate(new SerialSettingsView());
        }

        public string position { get;set; }
     

        public event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { }//throw new NotImplementedException(); }
            remove {}// throw new NotImplementedException(); }
        }
    }

}
