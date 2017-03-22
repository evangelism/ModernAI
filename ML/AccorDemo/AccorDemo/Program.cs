using Accord.IO;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using Accord.Statistics.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccorDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            var data = new ExcelReader(@"c:\temp\accordemo\accordemo\titanic.xls").GetWorksheet("titanic3");
            data.Rows.RemoveAt(data.Rows.Count-1);
            var d = new Elimination("age").Apply(data);
            var fdata = new Codification(d, "sex").Apply(d);
            var outp = fdata.Columns["survived"].ToArray<int>();
            var input = fdata.ToArray<double>("pclass","sex","age","parch","sibsp");
            DecisionTree T = new DecisionTree( 
                DecisionVariable.FromData(input), 2);
            var learn = new C45Learning(T);
            learn.Run(input, outp);
            var r1 = T.Decide(new double[] { 0, 1, 23, 0, 0 });
            var r2 = T.Decide(new double[] { 1, 0, 30, 1, 1 });
            Console.WriteLine($"Male={r1}, Female={r2}");
            Console.ReadKey();
        }
    }
}
