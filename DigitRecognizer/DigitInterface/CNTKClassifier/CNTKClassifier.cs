using CNTK;
using DigitReco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitInterface.CNTKClassifier
{
    public class CNTKClassifier : IClassifier
    {
        public int Classify(int[] value)
        {
            var Model = Function.LoadModel(@"d:\winapp\cntk\Examples\Image\GettingStarted\Output\Models\01_OneHidden");

            Variable Features = Model.Arguments[0];
            NDShape InShape = Features.Shape;
            var InputVals = new List<float>();
            for (int i = 0; i < 28 * 28; i++) InputVals.Add((float)value[i]);

            Value inp = Value.CreateBatch(InShape, InputVals, DeviceDescriptor.CPUDevice);

            var InputMap = new Dictionary<Variable, Value>();
            var OutputMap = new Dictionary<Variable, Value>();

            InputMap.Add(Features, inp);

            var Out = (from x in Model.Outputs where x.Name == "out.z" select x).First();

            OutputMap.Add(Out, null);

            Model.Evaluate(InputMap, OutputMap, DeviceDescriptor.CPUDevice);

            var O = OutputMap[Out];

            var res = new List<List<float>>();
            O.CopyVariableValueTo(Out, res);

            float m = -100; var n = -1;
            for(int i = 0;i<10;i++)
            {
                if (res[0][i]>m)
                {
                    m = res[0][i];
                    n = i;
                }
            }
            return n;

        }
    }
}
