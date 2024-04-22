using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace password_manager
{
    internal static class API_RequestsVerify
    {
        public static async Task<string> GetVerifyFromDBAsync(Context context)
        {
            try
            {
                Constants.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", User.token);
                var response = await Constants.client.GetAsync($"{Constants.ip_address}/verify/?lang={User.lang}");
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                    JArray dataArray = (JArray)JObject.Parse(responseString)["data"];
                    if (dataArray.Count>0)
                    {
                        JObject firstDataObject = (JObject)dataArray[0];
                        return firstDataObject["verify"].ToString();
                    }
                    return null;
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                    Toast.MakeText(context, responsejson["message"].ToString(), ToastLength.Short).Show();
                }
            }
            catch (WebException webex)
            {
                Toast.MakeText(context, $"{context.GetString(Resource.String.network_error)} {webex.Message}.", ToastLength.Short).Show();
            }
            catch (System.OperationCanceledException ex)
            {
                Toast.MakeText(context, $"{context.GetString(Resource.String.network_error)} {ex.Message}.", ToastLength.Short).Show();
            }
            catch (System.Exception hiba)
            {
                Toast.MakeText(context, $"{context.GetString(Resource.String.generic_error)} {hiba.Message}.", ToastLength.Short).Show();
            }
            return null;
        }

        public static async Task<bool> AddVerifyToDB(Context context, JObject json)
        {
            try
            {
                Constants.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", User.token);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = await Constants.client.PostAsync($"{Constants.ip_address}/verify/?lang={User.lang}", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                    JObject data = (JObject)JObject.Parse(responseString)["data"];
                    Toast.MakeText(context, responsejson["message"].ToString(), ToastLength.Short).Show();
                    return true;
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                    Toast.MakeText(context, responsejson["message"].ToString(), ToastLength.Short).Show();
                    return false;
                }
            }
            catch (WebException webex)
            {
                Toast.MakeText(context, $"{context.GetString(Resource.String.network_error)} {webex.Message}.", ToastLength.Short).Show();
            }
            catch (System.OperationCanceledException ex)
            {
                Toast.MakeText(context, $"{context.GetString(Resource.String.network_error)} {ex.Message}.", ToastLength.Short).Show();
            }
            catch (System.Exception hiba)
            {
                Toast.MakeText(context, $"{context.GetString(Resource.String.generic_error)} {hiba.Message}.", ToastLength.Short).Show();
            }
            return false;
        }
    }
}