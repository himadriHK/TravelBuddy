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
using GoogleApi.Entities.Places.QueryAutoComplete.Request;
using GoogleApi.Entities.Places.QueryAutoComplete.Response;
using GoogleApi.Entities.Maps.Common.Enums;
using GoogleApi.Entities.Places.Search.Common.Enums;

namespace TravelBuddy
{
    public class LocationHelper
    {
	    public  Location currentLocation;
        private Activity screenActivity;
	    

		public  async Task StartLocationRequestAsync(Activity activity)
		{
			FusedLocationProviderClient fusedLocationProvider = LocationServices.GetFusedLocationProviderClient(activity);
			await getLastLocationFromDevice(fusedLocationProvider);
			LocationRequest locationRequest = new LocationRequest()
				.SetPriority(LocationRequest.PriorityHighAccuracy)
				.SetInterval(10000)
				.SetFastestInterval(10000);

            await fusedLocationProvider.RequestLocationUpdatesAsync(locationRequest, ((MainActivity)activity).locationCallback);

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


        public string[] getLocationByText(string locationText)
        {
            var request = new PlacesQueryAutoCompleteRequest
            {
                Key = "AIzaSyBA58FFbrOgnkHm5k3-i1cF2lJOhfouQ1I",//this.ApiKey,
                Input = locationText //"jagtvej 2200"
            };

            PlacesQueryAutoCompleteResponse response;
            response = GooglePlaces.QueryAutoComplete.Query(request);

            return response.Predictions.Select(x => x.Description).ToArray();

           // EditText et = activity.FindViewById<EditText>(Resource.Id.editText1);
            //activity.RunOnUiThread(()=>et.Text = res);
            //TextView tv = screenActivity.FindViewById<TextView>(Resource.Id.textView1);
            //tv.Text = res;
        }

        //Method to search all around the given location taking a distance in radius to find
        //the location by location type ex. restaurant, hotels, museums etc..
        //return type is list of locations..which we will feed in method getAllTravelPlan
        public void getAllNearbyLocationsByLocationType(Location currentLocation, float distance, SearchPlaceType []searchPlaceType) { }

        // To get all the routes between source and destination..return type is list of routes based on Vehicle type(Travel Mode)
        public void getRoutes(Location source, Location destination, TravelMode travelMode) { }

        // Put identifier on route on Google Map to identify Location type ex. restaurant, hotels, museums etc..
        public void putLocationIdentifierOnRoute() { }

        //Propose route options to cover maximum places based on popularity and convinience and user choice and days of plan
        public void getAllTravelPlan(Location location, int no_of_days, Location[] selectedLocationstoVisit) { }

    }
}