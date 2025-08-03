using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBill.Models
{
    public static class BillSupport
    {
        public static string increamentInvoice(int dbCounter)
        {
            string InvoiceNo = string.Empty;
            int newDbCounter = dbCounter +1;
            if (newDbCounter > 0 && newDbCounter < 10)
            {
                InvoiceNo = "INV00000" + newDbCounter.ToString();
            }
            if (newDbCounter > 9 && newDbCounter < 100)
            {
                InvoiceNo = "INV0000" + newDbCounter.ToString();
            }
            if (newDbCounter > 99 && newDbCounter < 1000)
            {
                InvoiceNo = "INV000" + newDbCounter.ToString();
            }
            if (newDbCounter > 999 && newDbCounter < 10000)
            {
                InvoiceNo = "INV00" + newDbCounter.ToString();
            }
            if (newDbCounter > 9999 && newDbCounter < 100000)
            {
                InvoiceNo = "INV0" + newDbCounter.ToString();
            }
            return InvoiceNo;

        }
        public static string increamentDraft(int dbCounter)
        {
            string InvoiceNo = string.Empty;
            int newDbCounter = dbCounter + 1;
            if (newDbCounter > 0 && newDbCounter < 10)
            {
                InvoiceNo = "D00000" + newDbCounter.ToString();
            }
            if (newDbCounter > 9 && newDbCounter < 100)
            {
                InvoiceNo = "D0000" + newDbCounter.ToString();
            }
            if (newDbCounter > 99 && newDbCounter < 1000)
            {
                InvoiceNo = "D000" + newDbCounter.ToString();
            }
            if (newDbCounter > 999 && newDbCounter < 10000)
            {
                InvoiceNo = "D00" + newDbCounter.ToString();
            }
            if (newDbCounter > 9999 && newDbCounter < 100000)
            {
                InvoiceNo = "D0" + newDbCounter.ToString();
            }
            return InvoiceNo;

        }

        public static string increamentItem(int dbCounter)
        {
            string ItemNo = string.Empty;
            int newDbCounter = dbCounter + 1;
            if (newDbCounter > 0 && newDbCounter < 10)
            {
                ItemNo = "ITM00000" + newDbCounter.ToString();
            }
            if (newDbCounter > 9 && newDbCounter < 100)
            {
                ItemNo = "ITM0000" + newDbCounter.ToString();
            }
            if (newDbCounter > 99 && newDbCounter < 1000)
            {
                ItemNo = "ITM000" + newDbCounter.ToString();
            }
            if (newDbCounter > 999 && newDbCounter < 10000)
            {
                ItemNo = "ITM00" + newDbCounter.ToString();
            }
            if (newDbCounter > 9999 && newDbCounter < 100000)
            {
                ItemNo = "ITM0" + newDbCounter.ToString();
            }
            return ItemNo;

        }


        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }













    }
}