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
using System.IO;

namespace WpfApplication1.ViewModel
{
    public class DataParsedViewModel : INotifyPropertyChanged
    {
        #region FIELDS
            
            Queue<ObjectGP> list = new Queue<ObjectGP>();
            private string onOffButton = "Break";// Used to stock the content of the switch button
            ObjectsPorts objports = new ObjectsPorts();//Used to be the list of ObjectPort from Ports.xml
            private string link; // Used to stock the path for Ports.xml
            string recieved_data; // Used to stock one trame of the GPS Message
            SerialPort sp; // Used to be THE SerialPort of NavRTK
            private string actualStatus = StatusEnum.StatusKO.ToString();
            /*** GPS FIELDS ***
             Used to fill in the elements of the view
             */
                //Both GGA & RMC
                private string time;
                private string latitude;
                private string longitude;
 
                //GGA
                private string position;
                private string quality;
                private string nSat;
                private string dilution;
                private string altitude;
                private string geoidal;
                private string GGAlastDGPS;

                //RMC
                private string validity;
                private string speed;
                private double speedBar;
                private string cap;
                private string magnetic;
                private string modepos;
            /*** GPS FIELDS END ***/
        #endregion FIELDS

        #region PROPERTIES
            public string OnOffButton
            {
                get
                { return onOffButton; }
                set
                {
                    onOffButton = value;
                    OnPropertyChanged("OnOffButton");
                }
            }
            
            /// <summary>
            /// Both
            /// </summary>
            #region Get Set Both
            public string Time
            {
                get
                { return time; }
                set
                {
                    time = value;
                    OnPropertyChanged("Time");
                }
            }
            public string Latitude
            {
                get
                { return latitude; }
                set
                {
                    latitude = value;
                    OnPropertyChanged("Latitude");
                }
            }
            public string Longitude
            {
                get
                { return longitude; }
                set
                {
                    longitude = value;
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
                { return position; }
                set
                {
                    position = value;
                    OnPropertyChanged("Position");
                }
            }
            public string Quality
            {
                get
                { return quality; }
                set
                {
                    quality = value;
                    OnPropertyChanged("Quality");
                }
            }
            public string NSat
            {
                get
                { return nSat; }
                set
                {
                    nSat = value;
                    OnPropertyChanged("NSat");
                }
            }
            public string Dilution
            {
                get
                { return dilution; }
                set
                {
                    dilution = value;
                    OnPropertyChanged("Dilution");
                }
            }
            public string Altitude
            {
                get
                { return altitude; }
                set
                {
                    altitude = value;
                    OnPropertyChanged("Altitude");
                }
            }
            public string Geoidal
            {
                get
                { return geoidal; }
                set
                {
                    geoidal = value;
                    OnPropertyChanged("Geoidal");
                }
            }
            public string GGALastDGPS
            {
                get
                { return GGAlastDGPS; }
                set
                {
                    GGAlastDGPS = value;
                    OnPropertyChanged("GGALastDGPS");
                }
            }
            public string Cap
            {
                get
                { return cap; }
                set
                {
                    cap = value;
                    OnPropertyChanged("Cap");
                }
            }
            public string ActualStatus
            {
                get
                { return actualStatus; }
                set
                {
                    actualStatus = value;
                    OnPropertyChanged("ActualStatus");
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
                { return validity; }
                set
                {
                    validity = value;
                    OnPropertyChanged("Validity");
                }
            }
            public string Speed
            {
                get
                { return speed; }
                set
                {
                    speed = value;
                    OnPropertyChanged("Speed");
                }
            }
            public double SpeedBar
            {
                get
                { return speedBar; }
                set
                {
                    speedBar = value;
                    OnPropertyChanged("SpeedBar");
                }
            }
            public string Magnetic
            {
                get
                { return magnetic; }
                set
                {
                    magnetic = value;
                    OnPropertyChanged("Magnetic");
                }
            }
            public string ModePos
            {
                get
                { return modepos; }
                set
                {
                    modepos = value;
                    OnPropertyChanged("ModePos");
                }
            }
            #endregion
        #endregion PROPERTIES

        #region COMMANDS
            /// <summary>
            /// Go to the other page
            /// </summary>
            private RelayCommand goToSettings;
            public ICommand GoToSettings
            {
                get
                {
                    if (goToSettings == null)
                    {
                        goToSettings = new RelayCommand(ExecuteGoToSettings, CanGoToSettings);
                    }
                    return goToSettings;
                }
            }
            /// <summary>
            /// Stop receiving data
            /// </summary>
            private RelayCommand stop;
            public ICommand Stop
            {
                get
                {
                    if (stop == null)
                    {
                        stop = new RelayCommand(ExecuteStop, CanStop);
                    }
                    return stop;
                }
            }
        #endregion COMMANDS
     
        #region CONSTRUCTOR
            public DataParsedViewModel()
            {
                link = "../../Ports.xml";
                
                if (File.Exists(link))
                {
                    objports = ObjectsPorts.Charger(link);
                }

                initSerialPort(objports);
                if(sp != null)
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
        #endregion CONSTRUCTOR

        #region COMMANDS IMPLEMENTATION
            private void ExecuteStop()
            {
                if (onOffButton == "Break")
                {
                    sp.Close();
                    onOffButton = "Read";
                }
                else
                {
                    if (!sp.IsOpen)
                        sp.Open();

                    //Sets button State and Creates function call on data recieved
                    sp.DataReceived += new SerialDataReceivedEventHandler(Recieve);

                    onOffButton = "Break";
                }
                OnPropertyChanged("OnOffButton");
            }
            private bool CanStop()
            {
                return true;
            }
            
        #endregion COMMANDS IMPLEMENTATION

        #region RECIEVED
            public delegate void UpdateUiTextDelegate1(MessageGPGGA objGGA);
            public delegate void UpdateUiTextDelegate2(MessageGPRMC objRMC);
            public void Recieve(object sender, SerialDataReceivedEventArgs e)
            {
                try
                {
                    if (list.Count() > 5)
                        list.Clear();

                    recieved_data = sp.ReadLine();

                    GPSParsor.splitMessage(recieved_data, list);

                    if (list.Last() != null  && Application.Current != null) 
                    {
                        if (list.Last().GetType() == typeof(MessageGPGGA))
                        {                            
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate1(WriteDataGGA), list.Last());
                            this.ActualStatus = StatusEnum.StatusOK.ToString();
                        }
                        else if (list.Last().GetType() == typeof(MessageGPRMC))
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate2(WriteDataRMC), list.Last());
                            this.ActualStatus = StatusEnum.StatusOK.ToString();
                        }                           
                        else
                        {
                            this.ActualStatus = StatusEnum.StatusKO.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }           

        #endregion RECIEVED

        #region METHODS
            /// <summary>
            /// Used to initialize the object port 
            /// </summary>
            /// <param name="objports">list of ObjectPort</param>
            public void initSerialPort(ObjectsPorts objports)
            {
                foreach (ObjectPort o in objports)
                {
                    if (o.Id == 0)
                    {
                        sp = new SerialPort();
                        sp.PortName = o.Name;
                        sp.BaudRate = int.Parse(o.Baudrate);
                        sp.DataBits = int.Parse(o.Databits);
                        switch (o.Stopbit)
                        {
                            case "One":
                                sp.StopBits = StopBits.One;
                                break;
                            case "Two":
                                sp.StopBits = StopBits.Two;
                                break;
                            case "OnePointFive":
                                sp.StopBits = StopBits.OnePointFive;
                                break;
                            default: sp.StopBits = StopBits.None;
                                break;
                        }
                        switch (o.Parity)
                        {
                            case "Even":
                                sp.Parity = Parity.Even;
                                break;
                            case "Mark":
                                sp.Parity = Parity.Mark;
                                break;
                            case "Odd":
                                sp.Parity = Parity.Odd;
                                break;
                            case "Space":
                                sp.Parity = Parity.Space;
                                break;
                            default: sp.Parity = Parity.None;
                                break;
                        }
                        switch (o.Handshake)
                        {
                            case "One":
                                sp.Handshake = Handshake.XOnXOff;
                                break;
                            case "Two":
                                sp.Handshake = Handshake.RequestToSend;
                                break;
                            case "OnePointFive":
                                sp.Handshake = Handshake.RequestToSendXOnXOff;
                                break;
                            default: sp.Handshake = Handshake.None;
                                break;
                        }
                    }
                }
            }
            /// <summary>
            /// Used to load GGA fields
            /// </summary>
            /// <param name="objGGA"></param>
            public void WriteDataGGA(MessageGPGGA objGGA)
            {

                time = objGGA.timeUTC.ToString("d MMMM yyyy hh:mm:ss");
                OnPropertyChanged("Time");

                latitude = objGGA.latitude.ToString("F6");
                OnPropertyChanged("Latitude");

                longitude = objGGA.longitude.ToString("F6");
                OnPropertyChanged("Longitude");

                position = objGGA.gpsQuality.ToString();
                OnPropertyChanged("Position");

                nSat = objGGA.nSat.ToString();
                OnPropertyChanged("NSat");

                //_nSat = (DateTime.Now.Second % 7).ToString();   test color

                dilution = objGGA.dilution.ToString("F2");
                OnPropertyChanged("Dilution");

                altitude = objGGA.altitude.ToString("F2") + objGGA.altUnit.ToString();
                OnPropertyChanged("Altitude");

                geoidal = objGGA.geoidal.ToString("F2") + objGGA.geoUnit.ToString();
                OnPropertyChanged("Geoidal");

                GGAlastDGPS = objGGA.dGPSTime.ToString("d MMM yyyy");
                OnPropertyChanged("GGALastDGPS");

            }
            /// <summary>
            /// Used to load RMC fields
            /// </summary>
            /// <param name="objRMC"></param>
            public void WriteDataRMC(MessageGPRMC objRMC)
            {
                validity = objRMC.status.ToString();
                OnPropertyChanged("Validity");

                /*PREVISIONNEL*/
                speed = objRMC.speed.ToString();
                OnPropertyChanged("Speed");

                speedBar = double.Parse(speed) * 1000;
                OnPropertyChanged("SpeedBar");

                cap = objRMC.cap.ToString();
                OnPropertyChanged("Cap");

                magnetic = objRMC.magnetic.ToString();
                OnPropertyChanged("Magnetic");

                modepos = objRMC.integrity;
                OnPropertyChanged("ModePos");

            }
        #endregion METHODS

        #region NAVIGATION
            private void ExecuteGoToSettings()
            {
                Console.WriteLine("SettingsClicked");
                
                
                if (sp != null)
                    if(sp.IsOpen)
                        sp.Close();

                App.Current.MainWindow.Visibility = Visibility.Hidden;
                App.Current.MainWindow = new SerialSettingsView();
                App.Current.MainWindow.Visibility = Visibility.Visible;        
               
            }
            private bool CanGoToSettings()
            {
                return true;
            }
        #endregion

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