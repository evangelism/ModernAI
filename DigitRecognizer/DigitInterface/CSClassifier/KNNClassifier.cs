using DigitReco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitInterface.CSClassifier
{
    public class KNNClassifier : IClassifier
    {

        protected class Digit
        {
            public int Value;
            public int[] Pixels;
            public Digit(string[] arr)
            {
                Value = int.Parse(arr[0]);
                Pixels = (from x in arr.Skip(1)
                          select int.Parse(x)).ToArray();
            }
        }

        protected Digit[] Data;

        public KNNClassifier(string fn = @"c:\DEMO\Data\train.csv")
        {
            var lines = File.ReadLines(fn).Skip(1);
            Data = (from x in lines
                   select new Digit(x.Split(','))).ToArray();
        }

        protected int dist(int[] a, int[] b)
        {
            int d = 0;
            for (int i = 0; i < a.Length; i++) d += (a[i] - b[i])*(a[i] - b[i]);
            return d;
        }

        public int Classify(int[] value)
        {
            var d = Data.First();
            var mn = dist(d.Pixels, value);
            foreach(var x in Data)
            {
                var t = dist(x.Pixels, value);
                if (t<mn)
                {
                    mn = t;
                    d = x;
                }
            }
            return d.Value;
        }
    }
}
