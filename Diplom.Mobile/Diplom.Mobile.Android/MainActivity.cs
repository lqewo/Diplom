using Android.App;
using Android.Content.PM;
//using Android.Gms.Security;

using Android.OS;
using System.Net;
using Xamarin.Forms;

namespace Diplom.Mobile.Android
{
    [Activity(Label = "App1", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (message, certificate, chain, sslPolicyErrors) => true;

            //if (Android.bi)
            //{
            //    ProviderInstaller.InstallIfNeeded(ApplicationContext);
            //}

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
            base.OnCreate(savedInstanceState);
            Forms.SetFlags("SwipeView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

    }

}
