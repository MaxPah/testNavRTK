using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Model
{
    public class GPSParsor
    {
        /// <summary>
        /// Used to calculate checksum
        /// </summary>
        /// <param name="trame">trame which we calculate the checksum</param>
        public static bool checksumCalculator(string trame) {
            int checksum = 0;
            string Checksum;
            int Output=0;

            for (int i = 0; i < trame.Length - 4; i++)
            {
                checksum ^= Convert.ToByte(trame[i]);
            }

            try
            {
                if (trame.Length != 0)
                {
                    string[] baseChecksum = trame.Split('*');
                    Checksum =  baseChecksum[1].Substring(0,2);
                }
                else Checksum = "00";

                Output = int.Parse(Checksum, System.Globalization.NumberStyles.HexNumber);
            }
            catch { Output = 00;
            }

           /// Returns
            if (checksum.Equals(Output))
                return true;
            else
            {
               return false;
            }
        }

        /// <summary>
        /// Used to parse frames (DGPS) in array of string
        /// </summary>
        /// <param name="t">String to parse</param>
        /// <returns>Return array parsed</returns>
       public static void splitMessage(string t, Queue<Object> list) {
            string[] split;
            string messageReturn="\tInitialisation"; 

           if (t != null)
           {

               try
               {
                    t = t.Substring(1, t.Length-1);
                    split= t.Split('$');
                    string[][] split2 = new string[split.Length][];

                   for(int i=0; i < split.Length;i++)
                    {
                        split[i] = split[i].Substring(0, split[i].Length);
                        if (checksumCalculator(split[i]))
                        {
                            split2[i] = split[i].Split(',');

                            if (split2[i][0] != "" )
                            {
                                if (list.Count > 25)
                                    list.Clear();

                                if (split2[i][0] == "GPGGA")
                                {
                                   
                                    MessageGPGGA objGPGGA = new MessageGPGGA(split2[i]);
                                    list.Enqueue(objGPGGA);
                                   messageReturn = MessageGPGGA.printGPGGA(objGPGGA);
                                }
                                else if (split2[i][0] == "GPRMC")
                                {
                                   
                                    MessageGPRMC objGPRMC = new MessageGPRMC(split2[i]);
                                    list.Enqueue(objGPRMC);
                                    messageReturn = MessageGPRMC.printGPRMC(objGPRMC);
                                }
                            }
                        }
                    }
                }

               catch
               {
                   
               }
           }
        }
        
        public static MessageGPGGA objGPRMC { get; set; }
        public static MessageGPRMC objGPGGA { get; set; }
    }
}
