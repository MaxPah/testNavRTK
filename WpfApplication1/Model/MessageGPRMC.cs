using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Model
{
    #region ObjectGPRMC Class
    /// <summary>
    /// This is a decendant of ObjectGP used to initialize ObjectGPRMC with a trame
    /// </summary>
    public class MessageGPRMC : ObjectGP
    {
        #region fields
        public string type;
        public DateTime timeUTC;
        public char status;
        public double latitude;
        public double longitude;
        public double speed;
        public string cap;
        public DateTime date;
        public string magnetic;
        public string integrity;
        public string checksum;
        #endregion

        #region MessageGPRMC Constructor
        /// <summary>
        /// ObjectGPRMC constructor
        /// </summary>
        /// <param name="var">Used to initialize an instance of ObjectGPRMC</param>
        public MessageGPRMC(string[] var)
        {
            this.RMCInit(this, var);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="var"></param>
        /// <returns></returns>
        public MessageGPRMC RMCInit(MessageGPRMC obj, string[] var)
        {
            string separator = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            char a;
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            //TYPE OF TRAME
            obj.type = "GPRMC";

            //TIME UTC
            if (var[1] != String.Empty)
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

            //STATUS
            if (var[2] == "")
                obj.status = '\0';
            else obj.status = char.Parse(var[2]);

            //LATITUDE
            if (var[3] != String.Empty)
                obj.latitude = toLatitude(var[3], char.Parse(var[4]));
            else obj.latitude = 0;

            //LONGITUDE
            if (var[5] != String.Empty)
                obj.longitude = toLongitude(var[5], char.Parse(var[6]));
            else obj.longitude = 0;

            //SPEEED
            if (var[7] != String.Empty)
                obj.speed = Convert.ToDouble(var[7].Replace(".", separator));// * 1.852;   * 1.852 if you want km/h otherwise it's knot
            else obj.speed = 0;

            //CAP
            if (var[8] != String.Empty)
                obj.cap = var[8];
            else obj.cap = " ";

            //DATE
           if (var[9].Equals(string.Empty))
                obj.date = new DateTime(1,1,1);
           else   obj.date = new DateTime(int.Parse(var[9].Substring(4, 2)) + 2000, int.Parse(var[9].Substring(2, 2)), int.Parse(var[9].Substring(0, 2)));


            //MAGNETIC
            if (var[11] == "")
                a = '0';
            else a = char.Parse(var[11].Substring(0, 1));
            if (var[10] != String.Empty)
                obj.magnetic = var[10] + a;
            else obj.magnetic = "" + a;

            //INTEGRITY
            if (var[12] != String.Empty && var[12].Length > 5)
                obj.integrity = var[12].Substring(0, 1);
            else obj.integrity = "N";

            //CHECKSUM
           obj.checksum = var[12].Substring(var[12].Length - 3, 2); 
            

            return obj;
        }

        /// <summary>
        /// Print informations of an ObjectGPGGA
        /// </summary>
        /// <param name="p">Object to string</param>
        public static string printGPRMC(MessageGPRMC p)
        {
            return p.type + "  -  " +
                p.timeUTC + "  -  " +
                p.status + "  -  " +
                p.latitude + "  -  " +
                p.longitude + "  -  " +
                p.speed + "(knots)  -  " +
                p.cap + "  -  " +
                p.date + "  -  " +
                p.magnetic + "  -  " +
                p.integrity + "  -  " +
                p.checksum + "\n";
        }
    }
    #endregion
}