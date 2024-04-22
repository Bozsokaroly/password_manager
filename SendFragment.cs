using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace password_manager
{
    public class SendFragment : AndroidX.Fragment.App.Fragment
    {
        EditText sendInput;
        Button send;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((AndroidX.AppCompat.App.AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.title_send_data);
            View view = inflater.Inflate(Resource.Layout.send_layout, container, false);
            sendInput = view.FindViewById<EditText>(Resource.Id.shared_input);
            send = view.FindViewById<Button>(Resource.Id.share_button);

            send.Click += Send_ClickAsync;

            return view;
        }

        public void ShareUrl(string url, string title)
        {
            Intent shareIntent = new Intent(Intent.ActionSend);
            shareIntent.SetType("text/plain");
            shareIntent.PutExtra(Intent.ExtraSubject, title);
            shareIntent.PutExtra(Intent.ExtraText, url);

            Intent chooserIntent = Intent.CreateChooser(shareIntent, "Share via");
            chooserIntent.SetFlags(ActivityFlags.NewTask); // In case you call from a non-Activity context
            Android.App.Application.Context.StartActivity(chooserIntent);
        }
        private async Task SendData(JObject json)
        {
            try
            {
                Constants.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", User.token);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = await Constants.client.PostAsync($"{Constants.ip_address}/shared/generatelink/?lang={User.lang}", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                    ShareUrl(responsejson["link"].ToString() +$"?lang={User.lang}", "Itt a megosztott adatod linkje.");

                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);
                    Toast.MakeText(this.Context, responsejson["message"].ToString(), ToastLength.Short).Show();
                }
            }
            catch (WebException webex)
            {
                Toast.MakeText(this.Context, $"{GetString(Resource.String.network_error)} {webex.Message}.", ToastLength.Short).Show();
            }
            catch (System.OperationCanceledException ex)
            {
                Toast.MakeText(this.Context, $"{GetString(Resource.String.network_error)} {ex.Message}.", ToastLength.Short).Show();
            }
            catch (System.Exception hiba)
            {
                Toast.MakeText(this.Context, $"{GetString(Resource.String.generic_error)} {hiba.Message}.", ToastLength.Short).Show();
            }
        }

        private async void Send_ClickAsync(object sender, EventArgs e)
        {
            JObject json = new JObject
            {
                {"text", sendInput.Text.ToString() },
            };
            await SendData(json);
        }
    }
}