using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using Microsoft.ProjectOxford.Face;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace AgeBot.Dialogs
{
    [LuisModel("9e3d0580-7a58-4370-931c-363c69434df0", "c5677ce1bcfd4a46820e6acefbdbda99")]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        int count = 0;
        int male = 0;
        int female = 0;
        double ages = 0;

        protected async override Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var msg = await item;
            if (msg.Attachments==null || msg.Attachments.Count == 0)
            {
                await base.MessageReceived(context, item);
            }
            else
            {
                var cli = new FaceServiceClient("1a9025a2ec4d43928e60f90cd5db6002");
                var hcli = new HttpClient();
                var str = await hcli.GetStreamAsync(msg.Attachments[0].ContentUrl);
                var res = await cli.DetectAsync(str, returnFaceAttributes: new FaceAttributeType[] { FaceAttributeType.Age, FaceAttributeType.Gender });
                if (res.Length > 0)
                {
                    foreach (var x in res)
                    {
                        await context.PostAsync($"Age={x.FaceAttributes.Age}, Gender={x.FaceAttributes.Gender}");
                        count++;
                        if (x.FaceAttributes.Gender.ToLower() == "male") male++; else female++;
                        ages += x.FaceAttributes.Age;
                    }
                }
            }
        }

        [LuisIntent("None")]
        [LuisIntent("")]
        private async Task Default(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, unclear");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Hello")]
        private async Task Hello(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi there");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Statistics")]
        private async Task Statistics(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Processed {count} messages, {100.0 * male / count} male, {100.0 * female / count} females");
            await context.PostAsync($"Average age is {ages / count}");
            context.Wait(MessageReceived);
        }

        [LuisIntent("GenderCount")]
        private async Task Count(IDialogContext context, LuisResult result)
        {
            var gender = "male";
            foreach(var e in result.Entities)
            {
                if (e.Type == "Gender") gender = e.Entity;
            }
            if (gender.ToLower().StartsWith("male"))
            {
                await context.PostAsync($"There have been {male} male photos");
            }
            else if (gender.ToLower().StartsWith("female"))
            {
                await context.PostAsync($"There have been {female} female photos");
            }
            else await context.PostAsync($"Wrong gender specfied: {gender}");
            context.Wait(MessageReceived);
        }


    }
}