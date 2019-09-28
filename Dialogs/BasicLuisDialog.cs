//Main Bot Dialog
//Author : Siddharth G.C
//Last Modified : 15-May-2018, 10:57 PM IST

using System;
using System.Configuration;
using System.Threading.Tasks;
using LuisBot.Dialogs;                      //Add this to link with the WeatherForecast class
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;              //Essential package
using System.IO;                            //Essential package
using System.Web;                           //Essential package
using System.Collections.Generic;           //Essential package
using AdaptiveCards;                        //Essential package. Make sure you have downloaded it first.

namespace Microsoft.Bot.Sample.LuisBot
{
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }      
        
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }
        
        public string city = null;
        public string state = null;
        [LuisIntent("WeatherForecast")]
        public async Task ForecastIntent1(IDialogContext context, LuisResult result)
        {
            city = null;
            state = null;
            foreach(var entity in result.Entities)
            {
                if(entity.Type == "city")
                {
                    city = entity.Entity;
                }
                else if(entity.Type == "state")
                {
                    state = entity.Entity;
                }
            }
            if(city==null)
            {
                PromptDialog.Text(
                                context,
                                ForecastIntent2,	
                                "Please enter the name of the city (Example: Detroit, New York)"
                                );	
            }
            else
            {
                if(state==null)
                {
                    PromptDialog.Text(
                                    context,
                                    ForecastIntent3,	
                                    "Please enter the name of the state (Example: Michigan, New York)"
                                    );
                }
                else
                {
                    WeatherForecast wf = new WeatherForecast();
                    RootObject ro = new RootObject();
                    ro = wf.getForecast(city,state);
                    Attachment attachment = new Attachment();
                    attachment = wf.GetAdaptiveCard(ro);
                    var reply = context.MakeMessage();
                    reply.Attachments.Add(attachment);
                    await context.PostAsync(reply);
                }
            }
        }
        public async Task ForecastIntent2(IDialogContext context, IAwaitable<string> result)
        {
            city = await result;
            if(state==null)
            {
               PromptDialog.Text(
                                context,
                                ForecastIntent3,	
                                "Please enter the name of the state (Example: Michigan, New York)"
                                ); 
            }
            else
            {
                WeatherForecast wf = new WeatherForecast();
                RootObject ro = new RootObject();
                ro = wf.getForecast(city,state);
                Attachment attachment = new Attachment();
                attachment = wf.GetAdaptiveCard(ro);
                var reply = context.MakeMessage();
                reply.Attachments.Add(attachment);
                await context.PostAsync(reply);
            }
        }
        
        public async Task ForecastIntent3(IDialogContext context, IAwaitable<string> result)
        {
            state = await result;
            WeatherForecast wf = new WeatherForecast();
            RootObject ro = new RootObject();
            ro = wf.getForecast(city,state);
            Attachment attachment = new Attachment();
            attachment = wf.GetAdaptiveCard(ro);
            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply);
        }
        
        private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            context.Wait(MessageReceived);
        }
    }
}
