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

namespace TravelBuddy
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
	    private LocationHelper location = new LocationHelper();
		  protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) ==
                Permission.Denied || ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) ==
                Permission.Denied)
            {
	            location.GetGpsAccess(this,FindViewById(Resource.Layout.activity_main));
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            string key = "AIzaSyBA58FFbrOgnkHm5k3-i1cF2lJOhfouQ1I";
            var request = new GeolocationRequest { Key = key };
            var result = GoogleMaps.Geolocation.Query(request);

            var request2 = new PlacesNearBySearchRequest
            {
                Key = key,
                Location = result.Location,
                Radius = 500
            };

            var response2 = GooglePlaces.NearBySearch.Query(request2);
            TextView tv = FindViewById<TextView>(Resource.Id.textView1);
            EditText et = FindViewById<EditText>(Resource.Id.editText1);
            tv.Text = string.Format("Location: Lat:{0} Long:{1}",result.Location.Latitude,result.Location.Longitude);

            if(response2 != null && response2.Results.Any())
            {
                foreach (NearByResult nearByResult in response2.Results)
                    et.Text += nearByResult.Name + "\n";
            }
        }

        protected override void OnRestart()
        {
	        base.OnRestart();
	        SetContentView(Resource.Layout.activity_main);
	        string key = "AIzaSyBA58FFbrOgnkHm5k3-i1cF2lJOhfouQ1I";
	        var request = new GeolocationRequest { Key = key };
	        var result = GoogleMaps.Geolocation.Query(request);

	        var request2 = new PlacesNearBySearchRequest
	        {
		        Key = key,
		        Location = result.Location,
		        Radius = 500
	        };

	        var response2 = GooglePlaces.NearBySearch.Query(request2);
	        TextView tv = FindViewById<TextView>(Resource.Id.textView1);
	        EditText et = FindViewById<EditText>(Resource.Id.editText1);
	        tv.Text = string.Format("Location: Lat:{0} Long:{1}", result.Location.Latitude, result.Location.Longitude);

	        if (response2 != null && response2.Results.Any())
	        {
		        foreach (NearByResult nearByResult in response2.Results)
			        et.Text += nearByResult.Name + "\n";
	        }
		}

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
	        if (grantResults.All(x => x == Permission.Granted))
				await location.StartLocationRequestAsync(this);
        }
    }
}