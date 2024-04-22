using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password_manager
{
    public class ExportFragment : AndroidX.Fragment.App.Fragment
    {
        AndroidX.AppCompat.Widget.AppCompatSpinner fileTypeSpinner;
        EditText masterPasswordInput;
        ImageView showPassword;
        Button exportButton;
        private bool isPasswordVisible = false;
        delegate Task ExportMethodDelegate(List<Dictionary<string, object>> data, Java.IO.File downloadsDirectory);
        private ExportMethodDelegate currentExportMethod;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((AndroidX.AppCompat.App.AppCompatActivity)Activity).SupportActionBar.SetTitle(Resource.String.title_export);
            View view = inflater.Inflate(Resource.Layout.export_layout, container, false);

            fileTypeSpinner = view.FindViewById<AndroidX.AppCompat.Widget.AppCompatSpinner>(Resource.Id.file_spinner);
            masterPasswordInput = view.FindViewById<EditText>(Resource.Id.master_input);
            showPassword = view.FindViewById<ImageView>(Resource.Id.show_password);
            exportButton = view.FindViewById<Button>(Resource.Id.export_button);

            fileTypeSpinner.ItemSelected += FileTypeSpinner_ItemSelected;
            var adapter = ArrayAdapter.CreateFromResource(
            this.Context, Resource.Array.file_types, Android.Resource.Layout.SimpleSpinnerItem);
            fileTypeSpinner.Adapter = adapter;
            fileTypeSpinner.SetSelection(0); 

            showPassword.Click += ShowPassword_Click;
            exportButton.Click += ExportButton_Click;




            return view;
        }



        private void FileTypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string selectedFileType = (string)spinner.GetItemAtPosition(e.Position);
            switch (selectedFileType)
            {
                case ".TXT":
                    currentExportMethod = ExportToJsonTxtAsync;
                    break;
                case ".JSON":
                    currentExportMethod = ExportToJsonFileAsync;
                    break;
                default:
                    break;
            }
        }

        private async void ExportButton_Click(object sender, EventArgs e)
        {
            if (await CheckMasterPassword())
            {
                List<Dictionary<string, object>> data = await API_RequestsData.GetAllDataByUserFromDBAsync(this.Context);
                Java.IO.File downloadsDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                await currentExportMethod(data, downloadsDirectory);

            }
            else
            {
                Toast.MakeText(this.Context, GetString(Resource.String.key_invalid), ToastLength.Short).Show();
            }
        }
        private async Task<bool> CheckMasterPassword()
        {
            byte[] key = CryptoHelper.GenerateKey(masterPasswordInput.Text, User.salt);
            string verify = await API_RequestsVerify.GetVerifyFromDBAsync(this.Context);
            if (CryptoHelper.Verify(verify, key))
            {
                return true;
            }
            return false;
        }
        private void ShowPassword_Click(object sender, EventArgs e)
        {
            if (!isPasswordVisible)
            {
                masterPasswordInput.InputType = Android.Text.InputTypes.TextVariationVisiblePassword | Android.Text.InputTypes.ClassText;
                masterPasswordInput.SetSelection(masterPasswordInput.Text.Length);
                isPasswordVisible = true;
            }
            else
            {
                masterPasswordInput.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
                masterPasswordInput.SetSelection(masterPasswordInput.Text.Length);
                isPasswordVisible = false;
            }
        }
        public async Task ExportToJsonTxtAsync(List<Dictionary<string, object>> data, Java.IO.File downloadsDirectory)
        {
            string filePath = System.IO.Path.Combine(downloadsDirectory.AbsolutePath, "passwords.txt");

            // Erőforrás tömbök betöltése
            String[] months = Resources.GetStringArray(Resource.Array.months);
            String[] companies = Resources.GetStringArray(Resource.Array.companies);
            String[] types = Resources.GetStringArray(Resource.Array.types);

            using (StreamWriter file = File.CreateText(filePath))
            {
                foreach (var item in data)
                {
                    foreach (var element in item) // Közvetlenül iterálunk a Dictionary-n
                    {
                        if (string.IsNullOrEmpty(element.Value?.ToString())) continue; // Ellenőrizzük, hogy az érték nem null-e és nem üres string

                        string value = element.Value.ToString(); // Alapértelmezett értékadás

                        switch (element.Key)
                        {
                            case "_id":
                                continue; // Az "_id" kulcs esetén nem csinálunk semmit
                            case "type":
                                value = types[Convert.ToInt32(element.Value)];
                                break;
                            case "month_of_expire":
                                value = months[Convert.ToInt32(element.Value)];
                                break;
                            case "company":
                                value = companies[Convert.ToInt32(element.Value)];
                                break;
                            default:
                                value = User.CurrentDecryptionMethod.Invoke(element.Value.ToString(), CryptoHelper.key, User.salt);
                                break;
                        }

                        await file.WriteLineAsync($"{element.Key} = {value}");
                    }
                    await file.WriteLineAsync(""); // Üres sor hozzáadása az elemek közötti szeparáláshoz
                    Toast.MakeText(this.Context, GetString(Resource.String.success_export), ToastLength.Short).Show();
                }
            }
        }
        public async Task ExportToJsonFileAsync(List<Dictionary<string, object>> data, Java.IO.File downloadsDirectory)
        {
            string filePath = System.IO.Path.Combine(downloadsDirectory.AbsolutePath, "passwords.json");

            // Dekriptált adatokat tartalmazó lista létrehozása
            var decryptedData = new List<Dictionary<string, object>>();

            // Erőforrás tömbök betöltése (feltételezve, hogy ezek elérhetőek a kontextusban)
            String[] months = Resources.GetStringArray(Resource.Array.months);
            String[] companies = Resources.GetStringArray(Resource.Array.companies);
            String[] types = Resources.GetStringArray(Resource.Array.types);

            foreach (var item in data)
            {
                var decryptedItem = new Dictionary<string, object>();
                foreach (var element in item)
                {
                    if (string.IsNullOrEmpty(element.Value?.ToString())) continue; // Figyelmen kívül hagyja az üres értékeket

                    string value = element.Value.ToString(); // Alapértelmezett érték

                    switch (element.Key)
                    {
                        case "_id":
                            // Az "_id" kulcs esetén nem dekriptálunk
                            continue;
                        case "type":
                            value = types[Convert.ToInt32(element.Value)];
                            break;
                        case "month_of_expire":
                            value = months[Convert.ToInt32(element.Value)];
                            break;
                        case "company":
                            value = companies[Convert.ToInt32(element.Value)];
                            break;
                        default:
                            // Itt történik a dekriptálás a User által definiált módszerrel
                            value = User.CurrentDecryptionMethod.Invoke(element.Value.ToString(), CryptoHelper.key, User.salt);
                            break;
                    }
                    decryptedItem.Add(element.Key, value);
                }
                decryptedData.Add(decryptedItem); // Hozzáadja a dekriptált elemet a listához
            }

            // A dekriptált lista JSON formátumúvá konvertálása
            string jsonData = JsonConvert.SerializeObject(decryptedData, Formatting.Indented);

            // A JSON szöveg kiírása egy fájlba
            using (StreamWriter file = File.CreateText(filePath))
            {
                await file.WriteAsync(jsonData);
            }
            Toast.MakeText(this.Context, GetString(Resource.String.success_export), ToastLength.Short).Show();
        }

    }
}