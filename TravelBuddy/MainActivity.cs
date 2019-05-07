using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using GoogleApi;
using GoogleApi.Entities.Maps.Geolocation.Request;
using GoogleApi.Entities.Places.Search.NearBy.Request;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Places.Search.Common.Enums;
using System.Linq;
using Android;
using Android.Content.PM;
using Android.Support.V4.Content;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Android.Gms.Location;

namespace TravelBuddy
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
	    private LocationHelper location = new LocationHelper();

		  protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) ==
                Permission.Denied || ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) ==
                Permission.Denied)
            {
                location.GetGpsAccess(this, FindViewById(Resource.Layout.activity_main));
            }
            else
                 await location.StartLocationRequestAsync(this);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }

        protected override void OnRestart()
        {
	        base.OnRestart();
	        SetContentView(Resource.Layout.activity_main);

        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
	        if (grantResults.All(x => x == Permission.Granted))
				await location.StartLocationRequestAsync(this);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

       
    }
}