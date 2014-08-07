using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Model
{
    #region MessageGPGGA Class
    /// <summary>
    /// This is a decendant of ObjectGP used to initialize ObjectGPGGA with a trame
    /// </summary>
    public class MessageGPGGA : ObjectGP
    {
        #region fields
        public string type; // GPGGA or GPRMC (GPGGA here)
        public DateTime timeUTC; // Time UTC ULUL
        public double latitude; // Latitude
        public double longitude; // Longitude
        public byte gpsQuality; // GPS Quality 0 Bad to 4 Good
        public byte nSat; // Number of satellite viewed
        public double dilution; //
        public double altitude; // Altitude
        public char altUnit; // Altitude unit (M for meters)
        public double geoidal; //
        public char geoUnit; //
        public DateTime dGPSTime; // Diffential time : the last time station viewed ??? PAS FRANGLAIS
        public string stationRef; // 
        public string checksum; // used to check if every data are here          value.ToString("X");
        #endregion

        #region MessageGPGGA Constructor
        /// <summary>
        /// ObjectGPGGA Constructor
        /// </summary>
        /// <param name="var">Used to initialize an instance of ObjectGPGGA</param>
        public MessageGPGGA(string[] var)
        {
            this.GGAInit(this, var);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="var"></param>
        /// <returns></returns>
        public MessageGPGGA GGAInit(MessageGPGGA obj, string[] var)
        {
            Console.WriteLine(var[1]);
            string separator = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            //TYPE OF OBJECT
            obj.type = "GPGGA"; 

            //TIME UTC
            if (!var[1].Equals(string.Empty))
            {
                int h = int.Parse(var[1].Substring(0, 2));
                int m = int.Parse(var[1].Substring(2, 2));
                int s = int.Parse(var[1].Substring(4, 2));
                int ms;
                if (var[1].Length > 6)
                    ms = int.Parse(var[1].Substring(7, 2));
                else ms = 0;
                obj.timeUTC = new DateTime(year, month, day, h, m, s, ms);
            }
            else obj.timeUTC = new DateTime(1, 1, 1);

            //LATITUDE
            if (!var[2].Equals(string.Empty))
                obj.latitude = toLatitude(var[2], char.Parse(var[3]));
            else obj.latitude = 0;

            //LONGITUDE
            if (!var[4].Equals(string.Empty))
                obj.longitude = toLongitude(var[4], char.Parse(var[5]));
            else obj.longitude = 0;

            //GPS QUALITY
            if (!var[6].Equals(string.Empty))
                obj.gpsQuality = byte.Parse(var[6]);
            else obj.gpsQuality = 0;

            //NUMBER OF SATELITES
            if (!var[7].Equals(string.Empty))
                obj.nSat = byte.Parse(var[7]);
            else obj.nSat = 0;

            //DILUTION
            if (!var[8].Equals(string.Empty))
                obj.dilution = Convert.ToSingle(var[8].Replace(".", separator));
            else obj.dilution = 0;

            //ALTITUDE
            if (!var[9].Equals(string.Empty))
                obj.altitude = Convert.ToSingle(var[9].Replace(".", separator));
            else obj.altitude = 0;

            //ALTITUDE FORMAT
            if (var[10].Equals(string.Empty))
                obj.altUnit = '\0';
            else obj.altUnit = char.Parse(var[10]);

            //GEOIDAL
            if (!var[11].Equals(string.Empty))
                obj.geoidal = Convert.ToSingle(var[11].Replace(".", separator));
            else obj.geoidal = 0;

            //GEOIDAL FORMAT
            if (!var[12].Equals(string.Empty))
                obj.geoUnit = '0';
            else obj.geoUnit = char.Parse(var[12]);



            Console.WriteLine(var[13]);
            //DIFFERENTIAL TIME
            if (var[13].Equals(string.Empty))
                obj.dGPSTime = new DateTime(1,1,1);
            else obj.dGPSTime = new DateTime(year, month, day, int.Parse(var[13].Substring(0, 2)), int.Parse(var[13].Substring(2, 2)), int.Parse(var[13].Substring(4, 2)), int.Parse(var[13].Substring(7, 3)));

            //STATION VIEWED
            if (var[14].Length > 5)
                obj.stationRef = var[14].Substring(0, 4);
            else obj.stationRef = "0000";

            //CHECKSUM
            string[] check;
            check = var[14].Split('*');
            obj.checksum = check[1].Substring(0, 2);

            return obj;
        }

        /// <summary>
        /// Print informations of an ObjectGPGGA
        /// </summary>
        /// <param name="p">Object to string</param>
        public static string printGPGGA(MessageGPGGA p)
        {
            return p.type + " - " +
                p.timeUTC + " - " +
                p.latitude + "  - " +
                p.longitude + " - " +
                p.gpsQuality + " - " +
                p.nSat + " - " +
                p.dilution + " - " +
                p.altitude + " - " +
                p.altUnit + " - " +
                p.geoidal + " - " +
                p.geoUnit + " - " +
                p.dGPSTime + " - " +
                p.stationRef + " - " +
                p.checksum + "\n";

        }
        public string GetTypeTrame()
        {
            return this.type;
        }
    }
    #endregion
}