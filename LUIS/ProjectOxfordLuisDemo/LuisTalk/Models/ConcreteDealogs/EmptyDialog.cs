using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuisTalk.Models.ConcreteDialogs
{
    public class EmptyDialog: Dialog
    {
        public EmptyDialog()
            :base("Empty")
        {

        }

        public override string Initialize()
        {
            CurrentStep = new DialogStep(FinalResponse, FinalResponse);
            return "I will be happy to have a talk with you a little later";
        }

        protected async Task<DialogStepResult> FinalResponse(object arg)
        {
            return new DialogStepResult("Sorry, I'm busy. Let's talk later", new DialogStep(FinalResponse, FinalResponse));
        }
    }
}
