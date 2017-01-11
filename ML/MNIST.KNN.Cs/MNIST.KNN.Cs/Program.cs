using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNIST.KNN.Cs
{

    public class Digit
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


    class Program
    {
        static Digit[] Data;

        static void Main(string[] args)
        {
            var fn = @"d:\TEMP\DigitReco\Data\train.csv";
            var lines = File.ReadLines(fn).Skip(1);
            var AllData =  from x in lines
                           select new Digit(x.Split(',')));
            var n = AllData.Count();
            var tr = (int)(n * 0.9);
            Console.WriteLine("Train={0}, Validate={1}", n - tr, tr);
            Data = AllData.Take(n - tr).ToArray();
            int i=0, c = 0;
            foreach(var x in AllData.Skip(n-tr).ToArray())
            {
                var z = Classify(x.Pixels);
                if (z == x.Value) c++; i++;
                Console.WriteLine("{0}% compete ({1} records), {2} correct ({3}%)", (int)(100 * i / tr), i, c, (int)(100 * c / i));
            }
            Console.ReadKey();
        }

        static int dist(int[] a, int[] b)
        {
            int d = 0;
            for (int i = 0; i < a.Length; i++) d += (a[i] - b[i]) * (a[i] - b[i]);
            return d;
        }

        static int Classify(int[] value)
        {
            var d = Data.First();
            var mn = dist(d.Pixels, value);
            foreach (var x in Data)
            {
                var t = dist(x.Pixels, value);
                if (t < mn)
                {
                    mn = t;
                    d = x;
                }
            }
            return d.Value;
        }

    }
}
