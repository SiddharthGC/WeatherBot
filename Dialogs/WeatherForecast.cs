//Weather Forecast class
//Author : Siddharth G.C
//Last Modified : 15-May-2018, 10:57 PM IST

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.IO;
using Microsoft.Bot.Connector;
using AdaptiveCards;

namespace LuisBot.Dialogs
{
	public class WeatherForecast
	{
		public RootObject getForecast(string city, string state)
		{
			var responseString = string.Empty;
            var builder =new UriBuilder($"http://api.wunderground.com/api/dd328fca2cbee1db/conditions/q/{state}/{city}.json");
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                responseString = client.DownloadString(builder.Uri);
            }
            RootObject response;
            try
            {
                response = JsonConvert.DeserializeObject<RootObject>(responseString);
                return response;
            }
            catch
            {
                throw new Exception("Unable to deserialize Forecast response string.");
            }
		}
		
		public Attachment GetAdaptiveCard(RootObject obj)
        {
            string obtime = obj.current_observation.observation_time;
            obtime = obtime.Remove(0,16);
            var icon = (String)null;
            if(obj.current_observation.weather == "Sunny" || obj.current_observation.weather == "Clear")
                icon = "Sunny";
            else if(obj.current_observation.weather == "Mostly Cloudy")
                icon = "Mostly%20Cloudy";
            else if(obj.current_observation.weather == "Thunderstorms") 
                icon  = "Thunderstorms";
            else if(obj.current_observation.weather == "Cloudy"||obj.current_observation.weather == "Overcast")
                icon = "Cloudy";
            else if(obj.current_observation.weather == "Haze")
                icon = "Haze";
            else if(obj.current_observation.weather == "Snow")
                icon = "Snow";
            AdaptiveCard card = new AdaptiveCard()
            {
                Speak = $"The forecast for {obj.current_observation.display_location.full} is {obj.current_observation.weather} with a temperature of {obj.current_observation.temp_f} degrees farenheit",
                Body = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text =  $"{obj.current_observation.display_location.full}",
                        Weight = TextWeight.Bolder,
                        IsSubtle = true
                    },
                    new TextBlock()
                    {
                        Text =  $"{obtime}",
                        IsSubtle = true
                    },
                    new ColumnSet()
                    {
                        Columns = new List<Column>()
                        {
                           new Column()
                           {
                               Items = new List<CardElement>()
                               {
                                   new AdaptiveCards.Image()
                                   {
                                    Url = $"http://messagecardplayground.azurewebsites.net/assets/{icon}-Square.png",
                                    Size = ImageSize.Small  
                                   }
                               }
                           },
                           new Column()
                           {
                               Items = new List<CardElement>()
                               {
                                   new TextBlock()
                                   {
                                       Text = $"{obj.current_observation.temp_f} °F",
                                       Size = TextSize.Large
                                   }
                               }
                           },
                           new Column()
                           {
                               Items = new List<CardElement>()
                               {
                                   new TextBlock()
                                   {
                                       Text = $"{obj.current_observation.temp_c} °C",
                                       Size = TextSize.Large
                                   }
                               }
                           } 
                        }
                    }
                }
            };    
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCards.AdaptiveCard.ContentType,
                Content = card
            };
            return attachment;
        }
	}
	
	public class Features
	{
	    public int conditions { get; set; }
	}
	
	public class Response
	{
	    public string version { get; set; }
	    public string termsofService { get; set; }
	    public Features features { get; set; }
	}
	
	public class Image
	{
	    public string url { get; set; }
	    public string title { get; set; }
	    public string link { get; set; }
	}
	
	public class DisplayLocation
	{
	    public string full { get; set; }
	    public string city { get; set; }
	    public string state { get; set; }
	    public string state_name { get; set; }
	    public string country { get; set; }
	    public string country_iso3166 { get; set; }
	    public string zip { get; set; }
	    public string magic { get; set; }
	    public string wmo { get; set; }
	    public string latitude { get; set; }
	    public string longitude { get; set; }
	    public string elevation { get; set; }
	}
	
	public class ObservationLocation
	{
	    public string full { get; set; }
	    public string city { get; set; }
	    public string state { get; set; }
	    public string country { get; set; }
	    public string country_iso3166 { get; set; }
	    public string latitude { get; set; }
	    public string longitude { get; set; }
	    public string elevation { get; set; }
	}
	
	public class Estimated
	{
	}
	
	public class CurrentObservation
	{
	    public Image image { get; set; }
	    public DisplayLocation display_location { get; set; }
	    public ObservationLocation observation_location { get; set; }
	    public Estimated estimated { get; set; }
	    public string station_id { get; set; }
	    public string observation_time { get; set; }
	    public string observation_time_rfc822 { get; set; }
	    public string observation_epoch { get; set; }
	    public string local_time_rfc822 { get; set; }
	    public string local_epoch { get; set; }
	    public string local_tz_short { get; set; }
	    public string local_tz_long { get; set; }
	    public string local_tz_offset { get; set; }
	    public string weather { get; set; }
	    public string temperature_string { get; set; }
	    public double temp_f { get; set; }
	    public double temp_c { get; set; }
	    public string relative_humidity { get; set; }
	    public string wind_string { get; set; }
	    public string wind_dir { get; set; }
	    public int wind_degrees { get; set; }
	    public double wind_mph { get; set; }
	    public string wind_gust_mph { get; set; }
	    public double wind_kph { get; set; }
	    public string wind_gust_kph { get; set; }
	    public string pressure_mb { get; set; }
	    public string pressure_in { get; set; }
	    public string pressure_trend { get; set; }
	    public string dewpoint_string { get; set; }
	    public int dewpoint_f { get; set; }
	    public int dewpoint_c { get; set; }
	    public string heat_index_string { get; set; }
	    public string heat_index_f { get; set; }
	    public string heat_index_c { get; set; }
	    public string windchill_string { get; set; }
	    public string windchill_f { get; set; }
	    public string windchill_c { get; set; }
	    public string feelslike_string { get; set; }
	    public string feelslike_f { get; set; }
	    public string feelslike_c { get; set; }
	    public string visibility_mi { get; set; }
	    public string visibility_km { get; set; }
	    public string solarradiation { get; set; }
	    public string UV { get; set; }
	    public string precip_1hr_string { get; set; }
	    public string precip_1hr_in { get; set; }
	    public string precip_1hr_metric { get; set; }
	    public string precip_today_string { get; set; }
	    public string precip_today_in { get; set; }
	    public string precip_today_metric { get; set; }
	    public string icon { get; set; }
	    public string icon_url { get; set; }
	    public string forecast_url { get; set; }
	    public string history_url { get; set; }
	    public string ob_url { get; set; }
	    public string nowcast { get; set; }
	}
	
	public class RootObject
	{
	    public Response response { get; set; }
	    public CurrentObservation current_observation { get; set; }
	}
}