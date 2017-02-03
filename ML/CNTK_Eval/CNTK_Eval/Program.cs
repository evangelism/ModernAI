using CNTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNTK_Eval
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = File.ReadLines(@"d:\work\ModernAI\ML\Data\MNIST5k.csv")
                           .Skip(1);

            var Model = Function.LoadModel(@"d:\winapp\cntk\Examples\Image\GettingStarted\Output\Models\01_OneHidden");

            Variable Features = Model.Arguments[0];
            NDShape InShape = Features.Shape;

            Variable Out = (from x in Model.Outputs where x.Name == "out.z" select x).First();

            // Проверяем первые 100 значений
            int cnt = 0;
            int corr = 0;
            foreach(var d in data)
            {
                if (cnt > 100) break;

                var dt = d.Split(',').Select(x => float.Parse(x));
                var ground_truth = dt.First();

                Value inp = Value.CreateBatch(InShape, dt.Skip(1).ToList(), DeviceDescriptor.CPUDevice);

                var InputMap = new Dictionary<Variable, Value>();
                var OutputMap = new Dictionary<Variable, Value>();

                InputMap.Add(Features, inp);
                OutputMap.Add(Out, null);

                Model.Evaluate(InputMap, OutputMap, DeviceDescriptor.CPUDevice);

                var O = OutputMap[Out];
                var res = new List<List<float>>();
                O.CopyVariableValueTo(Out, res);

                var n = nmax(res[0]);

                cnt++;
                if (n == ground_truth) corr++;

                Console.WriteLine($"Truth={ground_truth}, Predict={n}, Correct={(100.0*corr/cnt)}%");

            }
            Console.ReadKey();
        }

        static int nmax(IEnumerable<float> seq)
        {
            float m = float.MinValue;
            int n = 0;
            int i = 0;
            foreach(var x in seq)
            {
                if (x>m) { n = i; m = x; }
                i++;
            }
            return n;
        }

    }
}
