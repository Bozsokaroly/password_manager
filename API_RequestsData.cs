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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace password_manager
{
    internal static class API_RequestsData
    {
        public static async Task<Dictionary<string, object>> GetDataFromDBAsync(Context context,string id)
        {
            try
            {
                Constants.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", User.token);
                var response = await Constants.client.GetAsync($"{Constants.ip_address}/data/get/{id}?lang={User.lang}");
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(responsejson["data"].ToString());
                    return data;
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

        public static async Task<List<Dictionary<string, object>>> GetAllDataByUserFromDBAsync(Context context)
        {
            try
            {
                Constants.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", User.token);
                var response = await Constants.client.GetAsync($"{Constants.ip_address}/data/?lang={User.lang}");
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                    var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responsejson["data"].ToString());
                    return data;
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
        public static async void AddDataToDB(Context context, JObject json)
        {
            try
            {
                Constants.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", User.token);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = await Constants.client.PostAsync($"{Constants.ip_address}/data/?lang={User.lang}", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                    JObject data = (JObject)JObject.Parse(responseString)["data"];
                    Toast.MakeText(context, responsejson["message"].ToString(), ToastLength.Short).Show();
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
        }
        public static async void UpdateDataToDB(Context context, JObject json, string id)
        {
            try
            {
                Constants.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", User.token);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = await Constants.client.PutAsync($"{Constants.ip_address}/data/{id}?lang={User.lang}", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                    JObject data = (JObject)JObject.Parse(responseString)["data"];
                    Toast.MakeText(context, responsejson["message"].ToString(), ToastLength.Short).Show();
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
        }
    }
}