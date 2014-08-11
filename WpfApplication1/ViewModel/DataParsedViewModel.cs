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
            private string link;
            //Both GGA & RMC
           private string _time;
           private string _latitude;
           private string _longitude;
           private SolidColorBrush _NSatColor; 
 
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
           private double _speedBar;
           private string _cap;
           private string _magnetic;
           private string _modepos;

            //Serial 
            string recieved_data;
            SerialPort sp;
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
            public double SpeedBar
            {
                get
                { return _speedBar; }
                set
                {
                    _speedBar = value;
                    OnPropertyChanged("SpeedBar");
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
            public SolidColorBrush NSatColor
            {
                get
                { return _NSatColor; }
                set
                {
                    _NSatColor = value;
                    OnPropertyChanged("NSatColor");
                }
            }
            #endregion
        #endregion PROPERTIES

        #region COMMANDS
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
        
        #region METHODS
            
        #endregion METHODS

        #region CONSTRUCTOR
            public DataParsedViewModel()
            {
                link = "../../Ports.xml";
                
                if (File.Exists(link))
                {
                    objports = ObjectsPorts.Charger(link);
                }

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
            private void ExecuteGoToData()
            {
                Console.WriteLine("SettingsClicked");
                //(Frame)NavigationService.Navigate(new DataParsedView());
            }
            private bool CanGoToData()
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

                //_nSat = (DateTime.Now.Second % 7).ToString();   test color

                if (int.Parse(_nSat) == 4)
                    _NSatColor = new SolidColorBrush(Colors.OrangeRed);
                else if (int.Parse(_nSat) == 5)
                    _NSatColor = new SolidColorBrush(Colors.Orange);
                else if (int.Parse(_nSat) < 14 && int.Parse(_nSat) > 5)
                    _NSatColor = new SolidColorBrush(Colors.Green);
                else  _NSatColor = new SolidColorBrush(Colors.Red);
                    OnPropertyChanged("NSatColor");

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

                /*PREVISIONNEL*/
                _speed = objRMC.speed.ToString() ;
                OnPropertyChanged("Speed");

                _speedBar = double.Parse(_speed) * 1000;
                OnPropertyChanged("SpeedBar");

                _cap = objRMC.cap.ToString(); 
                OnPropertyChanged("Cap");

                _magnetic = objRMC.magnetic.ToString(); 
                OnPropertyChanged("Magnetic");

                _modepos = objRMC.integrity; 
                OnPropertyChanged("ModePos");

            }

        #endregion RECIEVED

        #region NAVIGATION
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