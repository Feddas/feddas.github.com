using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

//copied from dropbox/Job/Hobby/SVN/PieChartIQ/FracGuessWP7/Model/Fraction.cs
namespace FracGuess
{
    public class Fraction
    {
        private const int minPrecision = 2,
            maxPrecision = 12;
        private string FracCreate = "";
        private static double precisionD;
        private static int precisionI,
            num = 1,
            den = 2,
            numChange = 0,
            denChange = 0,
            numPivot = 0,
            denPivot = 0;
        private static bool wasGreater = false;

        public static Point GetFrac(double Decimal, int DigitsOfPrecision)
        {
            precisionI = DigitsOfPrecision;
            if (precisionI < minPrecision) precisionI = minPrecision; //utilize the minimum precision
            precisionD = 1 / Math.Pow(10, precisionI);

            fraction(Decimal);
            return new Point(num, den);
        }
        public static Point GetFrac(double Decimal)
        {
            if (setPrecision(Decimal.ToString()))
            {
                fraction(Decimal);
                return new Point(num, den);
            }
            else
            {
                return new Point(0, 1);
            }
        }
        public static string GetFrac(string DecimalStr)
        {
            try
            {
                if (setPrecision(DecimalStr))
                {
                    return fraction(Convert.ToDouble(DecimalStr));
                }
                else
                {
                    return "Skipped possible long calculation.";
                }
            }
            catch (Exception exc)
            {
                return "error:" + exc.Message;
            }
        }

        private static bool setPrecision(string DecimalStr)
        {
            precisionI = DecimalStr.Length - DecimalStr.IndexOf(".") - 1;
            if (precisionI < minPrecision) precisionI = minPrecision; //utilize the minimum precision
            precisionD = 1 / Math.Pow(10, precisionI);

            if (precisionD < 1 / Math.Pow(10, maxPrecision))  //check if there are too many digits of precision
            {
                if (
                MessageBox.Show("You have a large number of digits of presicion.  This calculation may take a very long time, are you sure?", "Large calculation", MessageBoxButton.OKCancel)
                == MessageBoxResult.OK) return true;
                else
                    return false;
            }
            else
                return true;
        }

        private static string fraction(double InDecimal)
        {
            string wholepart = "";

            //initilize values
            //FracCreate = "";
            num = 1;
            den = 2;
            numChange = denChange = numPivot = denPivot = 0;

            double fracPart = InDecimal - (int)InDecimal;
            fracNoRecursion(Math.Abs(fracPart));  //only send the fractional part

            int InInt = 0;
            int Coef = 1;
            if (InDecimal >= 1 || InDecimal <= -1)
            {
                InInt = Math.Abs((int)InDecimal);
                Coef = Math.Sign(InDecimal);
                wholepart = " = " + Coef * InInt + " and " + num + "/" + den;
            }
            if (Math.Abs(fracPart) < precisionD)
                return (Coef * InInt) + "/1";
            else
                return
                String.Format("{0:#0."
                    + new string('0', precisionI + 1) + "}" //use precision to determine the display format
                    , Coef * ((double)num / den + InInt))
                + wholepart
                + " = " + Coef * (InInt * den + num) + "/" + den;
        }

        private static string fracNoRecursion(double InDecimal)
        {
            double diff = UpdateValues(InDecimal);

            //use first iteration to setup values
            if (diff < 0) //make fraction smaller
            {
                numChange = 0;
                denChange = 1;
                wasGreater = false;
            }
            else if (diff > 0) //make fraction bigger
            {
                numChange = 1;
                denChange = 1;
                wasGreater = true;
            }
            diff = UpdateValues(InDecimal);

            //subsequent iterations
            while (diff < -1 * precisionD || diff > precisionD) //then success
            {
                if (diff < 0) //make fraction smaller
                {
                    if (wasGreater) //modify pivot for new "left" tree direction
                    {
                        numChange = numPivot;
                        denChange = denPivot;
                    }
                    wasGreater = false;
                }
                else if (diff > 0) //make fraction bigger
                {
                    if (!wasGreater) //modify pivot for new "right" tree direction
                    {
                        numChange = numPivot;
                        denChange = denPivot;
                    }
                    wasGreater = true;
                }
                diff = UpdateValues(InDecimal);
            }
            return num + " / " + den;
        }

        private static double UpdateValues(double InDecimal)
        {
            numPivot = num; //save values so that they can be used 2 nodes down the tree
            denPivot = den;

            num += numChange; //iterate up to next fraction
            den += denChange;

            //Old documentation method
            //FracCreate += "\n" + ((float)num / den) + "|" + num + "/" + den + " + (" + numChange + "/" + denChange + ") ="; ;
            return InDecimal - ((float)num / den);
        }
    }
}