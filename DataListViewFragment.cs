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
using System.Text;
using System.Threading.Tasks;
using static Android.Graphics.ColorSpace;
using static AndroidX.RecyclerView.Widget.RecyclerView;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using Java.Interop;
using Google.Android.Material.FloatingActionButton;

namespace password_manager
{
    public class DataListViewFragment : AndroidX.Fragment.App.Fragment
    {
        private TaskCompletionSource<bool> confirmDeleteTaskSource;
        ListView listView;
        TextView textView;
        List<ListViewElement> listViewElements = null;
        string type = null;
        FloatingActionButton addButton;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Arguments != null)
            {
               type = Arguments.GetString("type", "");
            }

            LoadDataAsync(type);
        }
        public override void OnResume()
        {
            base.OnResume();
            LoadDataAsync(type);
        }
        private async void LoadDataAsync(string type)
        {
            try
            {
                List<ListViewElement> listViewElements = await FillListViewAsync(type);
                if (listViewElements.Count !=0)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        var adapter = new LoginAdapter(this.Context, listViewElements, type);
                        listView.Adapter = adapter;

                        listView.ItemClick -= ListView_ItemClick;
                        adapter.ModifyClicked -= Adapter_ModifyClicked;
                        adapter.DeleteClicked -= Adapter_DeleteClicked;

                        listView.ItemClick += ListView_ItemClick;
                        adapter.ModifyClicked += Adapter_ModifyClicked;
                        adapter.DeleteClicked += Adapter_DeleteClicked;

                    });
                }
                else
                {
                    textView.Text = GetString(Resource.String.no_data);
                    addButton.Visibility = ViewStates.Visible;
                    addButton.Click += AddButton_Click;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AndroidX.Fragment.App.Fragment fragment = type switch
            {
                "3" => new AddNoteFragment(),
                "2" => new AddIdentifyFragment(),
                "1" => new AddCardFragment(),
                "0" => new AddLoginFragment(),
                _ => null
            };

            if (fragment != null)
            {
                ParentFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.container, fragment, fragment.GetType().Name)
                    .AddToBackStack(null)
                    .Commit();
            }
        }

        private async void Adapter_DeleteClicked(object sender, ItemEventArgs e)
        {
            var listview = listView.Adapter;
            var javaObject = listview.GetItem(e.Position);
            bool dialogresponse = await GetUserConfirmation();
            bool apiresponse = await DeleteElement(javaObject.ToString());
            if (dialogresponse && apiresponse)
            {
                listViewElements.RemoveAt(e.Position);
                RefreshAdapter();
            }
        }
        private void Adapter_ModifyClicked(object sender, ItemEventArgs e)
        {
            var listview = listView.Adapter;
            var javaObject = listview.GetItem(e.Position);
            AndroidX.Fragment.App.Fragment fragment = type switch
            {
                "3" => new AddNoteFragment(),
                "2" => new AddIdentifyFragment(),
                "1" => new AddCardFragment(),
                "0" => new AddLoginFragment(),
                _ => null
            };

            if (fragment != null)
            {
                Bundle args = new Bundle();
                args.PutString("id", javaObject.ToString());
                fragment.Arguments = args;
                ParentFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.container, fragment, fragment.GetType().Name)
                    .AddToBackStack(null)
                    .Commit();
            }

        }
        private void RefreshAdapter()
        {
            var adapter = listView.Adapter as LoginAdapter;
            adapter?.NotifyDataSetChanged();
        }
        public void ConfirmDeleteAlertDialog()
        {
            confirmDeleteTaskSource = new TaskCompletionSource<bool>();
            AlertDialog.Builder builder = new AlertDialog.Builder(this.Context);
            View dialogView = LayoutInflater.From(this.Context).Inflate(Resource.Layout.dialog_custom_layout, null);

            builder.SetView(dialogView);

            AlertDialog dialog = builder.Create();

            Button yesButton = dialogView.FindViewById<Button>(Resource.Id.buttonYes);
            Button noButton = dialogView.FindViewById<Button>(Resource.Id.buttonNo);

            yesButton.Click += (sender, e) =>
            {
                confirmDeleteTaskSource.SetResult(true);
                dialog.Dismiss();
            };

            noButton.Click += (sender, e) =>
            {
                confirmDeleteTaskSource.SetResult(false);
                dialog.Dismiss();
            };

            dialog.Show();
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            // Leiratkozás az eseményről, ha a nézet megsemmisül
            if (listView.Adapter is LoginAdapter adapter)
            {
                adapter.ModifyClicked -= Adapter_ModifyClicked;
            }
        }
        private async Task<bool> GetUserConfirmation()
        {
            ConfirmDeleteAlertDialog();
            return await confirmDeleteTaskSource.Task;
        }
        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            var javaObject = listView.Adapter.GetItem(e.Position);
            if (javaObject != null)
            {
                ViewElementFragment fragment = new ViewElementFragment();
                Bundle args = new Bundle();
                args.PutString("_id", javaObject.ToString());
                fragment.Arguments = args;
                ParentFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.container, fragment, fragment.GetType().Name)
                    .AddToBackStack(null)
                    .Commit();
            }

        }

        private async Task<bool> DeleteElement(string id)
        {
            try
            {
                Constants.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", User.token);
                var response = await Constants.client.DeleteAsync($"{Constants.ip_address}/data/{id}?lang={User.lang}");
                if (response.IsSuccessStatusCode)
                {
                    Toast.MakeText(this.Context, GetString(Resource.String.deleted_item), ToastLength.Short).Show();
                    return true;
                }
                else
                {
                    Toast.MakeText(this.Context, GetString(Resource.String.no_deleted_item), ToastLength.Short).Show();
                    return false;
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
            return false;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((AndroidX.AppCompat.App.AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.title_list_data);
            View view = inflater.Inflate(Resource.Layout.data_listview_layout, container, false);

            addButton = view.FindViewById<FloatingActionButton>(Resource.Id.add_button);
            addButton.Visibility = ViewStates.Invisible;
            listView = view.FindViewById<ListView>(Resource.Id.listview);
            textView = view.FindViewById<TextView>(Resource.Id.textview);
            return view;
        }
        private async Task<List<ListViewElement>> FillListViewAsync(string type)
        {
            try
            {
                Constants.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", User.token);
                var response = await Constants.client.GetAsync($"{Constants.ip_address}/data/{type.ToString()}?lang={User.lang}");
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject responsejson = JsonConvert.DeserializeObject<JObject>(responseString);

                    listViewElements = JsonConvert.DeserializeObject<List<ListViewElement>>(responsejson["data"].ToString());
                    //Toast.MakeText(this.Context, responsejson["message"].ToString(), ToastLength.Short).Show();
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
            return listViewElements;
        }
    }
}