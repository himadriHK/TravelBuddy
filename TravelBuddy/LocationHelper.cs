using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using GoogleApi;
using GoogleApi.Entities.Places.Search.NearBy.Request;
using GoogleApi.Entities.Places.Search.NearBy.Response;

namespace TravelBuddy
{
    public class LocationHelper : LocationCallback
    {
	    public  Location currentLocation;
        private Activity screenActivity;
	    public  void GetGpsAccess(Activity activity, View view)
	    {
		    if (ActivityCompat.ShouldShowRequestPermissionRationale(activity, Manifest.Permission.AccessCoarseLocation) || ActivityCompat.ShouldShowRequestPermissionRationale(activity, Manifest.Permission.AccessFineLocation))
		    {

			    var requiredPermissions = new String[] { Manifest.Permission.AccessCoarseLocation,Manifest.Permission.AccessFineLocation };
			    Snackbar.Make(view,
					    Resource.String.permission_location_rationale,
					    Snackbar.LengthIndefinite)
				    .SetAction(Resource.String.ok,
					    new Action<View>(delegate (View obj) {
							    ActivityCompat.RequestPermissions(activity, requiredPermissions, 100);
						    }
					    )
				    ).Show();
		    }
		    else
		    {
			    ActivityCompat.RequestPermissions(activity, new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation }, 100);
		    }
		}

		public  async Task StartLocationRequestAsync(Activity activity)
		{
			FusedLocationProviderClient fusedLocationProvider = LocationServices.GetFusedLocationProviderClient(activity);
			await getLastLocationFromDevice(fusedLocationProvider);
			LocationRequest locationRequest = new LocationRequest()
				.SetPriority(LocationRequest.PriorityHighAccuracy)
				.SetInterval(5000)
				.SetFastestInterval(5000);

            await fusedLocationProvider.RequestLocationUpdatesAsync(locationRequest, this);

            screenActivity = activity;
		}

		 async Task getLastLocationFromDevice(FusedLocationProviderClient fusedLocationProviderClient)
		{

			currentLocation = await fusedLocationProviderClient.GetLastLocationAsync();

			if (currentLocation == null)
			{
				// Seldom happens, but should code that handles this scenario
			}

		}

        public override void OnLocationResult(LocationResult result)
        {
            if (result.Locations.Any())
            {
                currentLocation = result.Locations.First();
                string key = "AIzaSyBA58FFbrOgnkHm5k3-i1cF2lJOhfouQ1I";

                var request2 = new PlacesNearBySearchRequest
                {
                    Key = key,
                    Location = new GoogleApi.Entities.Common.Location() { Latitude = currentLocation.Latitude, Longitude = currentLocation.Longitude },
                    Radius = 500
                };

                var response2 = GooglePlaces.NearBySearch.Query(request2);
                TextView tv = screenActivity.FindViewById<TextView>(Resource.Id.textView1);
                EditText et = screenActivity.FindViewById<EditText>(Resource.Id.editText1);
                tv.Text = string.Format("Location: Lat:{0} Long:{1}", currentLocation.Latitude, currentLocation.Longitude);
                et.Text = string.Empty;
                if (response2 != null && response2.Results.Any())
                {
                    foreach (NearByResult nearByResult in response2.Results)
                        et.Text += nearByResult.Name + "\n";
                }
            }
            else
            {
                // No locations to work with.
            }
        }


    }
}