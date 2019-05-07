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

namespace TravelBuddy
{
    public class LocationHelper : LocationCallback
    {
	    public  Location currentLocation;
	    public  void GetGpsAccess(Activity activity, View view)
	    {
		    if (ActivityCompat.ShouldShowRequestPermissionRationale(activity, Manifest.Permission.AccessCoarseLocation) || ActivityCompat.ShouldShowRequestPermissionRationale(activity, Manifest.Permission.AccessFineLocation))
		    {
			    // Provide an additional rationale to the user if the permission was not granted
			    // and the user would benefit from additional context for the use of the permission.
			    // For example if the user has previously denied the permission.
			    //Log.Info(TAG, "Displaying camera permission rationale to provide additional context.");

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
				.SetInterval(60 * 1000 * 5)
				.SetFastestInterval(60 * 1000 * 2);

			await fusedLocationProvider.RequestLocationUpdatesAsync(locationRequest, this);
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
				 //Log.Debug("Sample", "The latitude is :" + location.Latitude);
			 }
			 else
			 {
				 // No locations to work with.
			 }
		 }
	}
}