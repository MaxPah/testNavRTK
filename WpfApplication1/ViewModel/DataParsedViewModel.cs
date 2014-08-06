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
using WpfApplication1.Helper;

namespace WpfApplication1.ViewModel
{
    public class DataParsedViewModel : INotifyPropertyChanged
    {
        #region variables

        // Number Of Objects
        Queue<Object> list = new Queue<Object>();

        //Both GGA & RMC
       private string _time;
       private string _latitude;
       private string _longitude;
 
        //GGA
       private string _position;
       private string _quality;
       private string _nSat;
       private string _dilution;
       private string _altitude;
       private string _geoidal;
       private string _GGAlastDGPS;

        //RMC
       private string _validity;
       private string _speed;
       private string _cap;
       private string _RMCLastDGPS;
       private string _magnetic;
       private string _modepos;

        //Serial 
        string recieved_data;
        SerialPort sp;
        #endregion


        public DataParsedViewModel()
        {
            sp = new SerialPort("COM1", 115200);

            if (!sp.IsOpen)
            {
                try
                {
                    sp.Open();
                    sp.DataReceived += new SerialDataReceivedEventHandler(Recieve);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        private RelayCommand goToData;
        public ICommand GoToView
        {
            get
            {
                if (goToData == null)
                {
                    goToData = new RelayCommand(ExecuteGoToData, CanGoToData);
                }
                return goToData;
            }
        }
        private void ExecuteGoToData()
        {
            Console.WriteLine("DataClicked");
            //(Frame)NavigationService.Navigate(new DataParsedView());
        }

        private bool CanGoToData()
        {
            return true;
        }

        public delegate void UpdateUiTextDelegate1(MessageGPGGA objGGA);
        public delegate void UpdateUiTextDelegate2(MessageGPRMC objRMC);

        public void Recieve(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                recieved_data = sp.ReadLine();

                GPSParsor.splitMessage(recieved_data, list);

                if (list.Last() != null) 
                { 
                    if (list.Last().GetType() == typeof(MessageGPGGA))
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate1(WriteDataGGA), list.Last());
                    if (list.Last().GetType() == typeof(MessageGPRMC))
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate2(WriteDataRMC), list.Last());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void WriteDataGGA(MessageGPGGA objGGA)
        {

            _time = objGGA.timeUTC.ToString("d MMMM yyyy hh:mm:ss");
            OnPropertyChanged("Time");

            _latitude = objGGA.latitude.ToString("F6");
            OnPropertyChanged("Latitude");

            _longitude = objGGA.longitude.ToString("F6");
            OnPropertyChanged("Longitude");

            _position = objGGA.gpsQuality.ToString();
            OnPropertyChanged("Position");

            _nSat = objGGA.nSat.ToString();
            OnPropertyChanged("NSat");

            _dilution = objGGA.dilution.ToString("F2");
            OnPropertyChanged("Dilution");

            _altitude = objGGA.altitude.ToString("F2") + objGGA.altUnit.ToString();
            OnPropertyChanged("Altitude");

            _geoidal = objGGA.geoidal.ToString("F2") + objGGA.geoUnit.ToString();
            OnPropertyChanged("Geoidal");

            _GGAlastDGPS = objGGA.dGPSTime.ToString("d MMM yyyy");
            OnPropertyChanged("GGALastDGPS");

        }
        

        public void WriteDataRMC(MessageGPRMC objRMC)
        {
           _validity = objRMC.status.ToString(); 
            OnPropertyChanged("Validity");

            _speed = objRMC.speed.ToString("F2"); 
            OnPropertyChanged("Speed");

            _cap = objRMC.cap.ToString(); 
            OnPropertyChanged("Cap");

            _RMCLastDGPS = objRMC.date.ToString("d MMM yyyy"); 
            OnPropertyChanged("RMCLastDGPS");

            _magnetic = objRMC.magnetic.ToString(); 
            OnPropertyChanged("Magnetic");

            _modepos = objRMC.integrity; 
            OnPropertyChanged("ModePos");

        }
       
        /// <summary>
        /// Both
        /// </summary>
        #region Both
        public string Time
        {
            get
            { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged("Time");
            }
        }
        public string Latitude
        {
            get
            { return _latitude; }
            set
            {
                _latitude = value;
                OnPropertyChanged("Latitude");
            }
        }
        public string Longitude
        {
            get
            { return _longitude; }
            set
            {
                _longitude = value;
                OnPropertyChanged("Longitude");
            }
        }
        #endregion

        /// <summary>
        /// GGA
        /// </summary>
        #region Get Set GGA
        public string Position
        {
            get
            { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged("Position");
            }
        }
        public string Quality
        {
            get
            { return _quality; }
            set
            {
                _quality = value;
                OnPropertyChanged("Quality");
            }
        }
        public string NSat
        {
            get
            { return _nSat; }
            set
            {
                _nSat = value;
                OnPropertyChanged("NSat");
            }
        }
        public string Dilution
        {
            get
            { return _dilution; }
            set
            {
                _dilution = value;
                OnPropertyChanged("Dilution");
            }
        }
        public string Altitude
        {
            get
            { return _altitude; }
            set
            {
                _altitude = value;
                OnPropertyChanged("Altitude");
            }
        }
        public string Geoidal
        {
            get
            { return _geoidal; }
            set
            {
                _geoidal = value;
                OnPropertyChanged("Geoidal");
            }
        }
        public string GGALastDGPS
        {
            get
            { return _GGAlastDGPS; }
            set
            {
                _GGAlastDGPS = value;
                OnPropertyChanged("GGALastDGPS");
            }
        }
        public string Cap
        {
            get
            { return _cap; }
            set
            {
                _cap = value;
                OnPropertyChanged("Cap");
            }
        }
        #endregion GetSet

        /// <summary>
        /// RMC
        /// </summary>
        #region Get Set RMC
        public string Validity
        {
            get
            { return _validity; }
            set
            {
                _validity = value;
                OnPropertyChanged("Validity");
            }
        }
        public string Speed
        {
            get
            { return _speed; }
            set
            {
                _speed = value;
                OnPropertyChanged("Speed");
            }
        }
        public string RMCLastDGPS
        {
            get
            { return _RMCLastDGPS; }
            set
            {
                _RMCLastDGPS = value;
                OnPropertyChanged("RMCLastDGPS");
            }
        }
        public string Magnetic
        {
            get
            { return _magnetic; }
            set
            {
                _magnetic = value;
                OnPropertyChanged("Magnetic");
            }
        }
        public string ModePos
        {
            get
            { return _modepos; }
            set
            {
                _modepos = value;
                OnPropertyChanged("ModePos");
            }
        }
        #endregion


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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }

}
