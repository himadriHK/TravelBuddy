﻿using Android.App;
using Android.Gms.Location;
using Android.Gms.Maps.Model;
using Android.Locations;
using GoogleApi;
using GoogleApi.Entities.Maps.Common.Enums;
using GoogleApi.Entities.Maps.Directions.Request;
using GoogleApi.Entities.Maps.Directions.Response;
using GoogleApi.Entities.Places.Details.Request;
using GoogleApi.Entities.Places.QueryAutoComplete.Request;
using GoogleApi.Entities.Places.QueryAutoComplete.Response;
using GoogleApi.Entities.Places.Search.Common.Enums;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Math = System.Math;
using System.Net;

namespace TravelBuddy
{
	public class LocationHelper
    {
	    public  Location currentLocation;
        private Activity screenActivity;
        public List<Tuple<LatLng, string>> allSites = new List<Tuple<LatLng, string>>();
        public async Task<bool> UpdateSitesData()
        {
            WebClient client = new WebClient();
            var sitesJson = client.DownloadString("http://10.0.2.2:8080/getAllSites/");
            //string  = await response.Content.ReadAsStringAsync();
            var allSites_temp = JsonConvert.DeserializeObject<List<Tuple<Tuple<double, double>, string>>>(sitesJson);
            allSites = allSites_temp.Select(x => Tuple.Create(new LatLng(x.Item1.Item1, x.Item1.Item2), x.Item2)).ToList();
            if (allSites.Any())
                return true;
            else
                return false;
        }

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


        public Dictionary<string,string> getLocationByText(string locationText)
        {
            var request = new PlacesQueryAutoCompleteRequest
            {
                Key = "AIzaSyBA58FFbrOgnkHm5k3-i1cF2lJOhfouQ1I",//this.ApiKey,
                Input = locationText //"jagtvej 2200"
            };
            //CancellationTokenSource tokenSource = new CancellationTokenSource(new System.TimeSpan(0, 0, 2));
            PlacesQueryAutoCompleteResponse response;
            var task = GooglePlaces.QueryAutoComplete.QueryAsync(request);
            //task.RunSynchronously();
            task.Wait(new System.TimeSpan(0, 0, 2));

            if (task.IsCompleted)
                response = task.Result;
            else
                response = new PlacesQueryAutoCompleteResponse();
            var result = response.Predictions?.Where(x => x.PlaceId != null)?.ToDictionary(x => x.PlaceId, x => string.Join(",",string.Concat(x.StructuredFormatting.MainText, ",", x.StructuredFormatting.SecondaryText).Split(",").Take(2)));
            return result??new Dictionary<string, string>() ;

        }

        public async Task<GoogleApi.Entities.Common.Location> GetLatLngFromPlaceId(string placeId)
        {
            var req = new PlacesDetailsRequest { PlaceId = placeId, Key = "AIzaSyBA58FFbrOgnkHm5k3-i1cF2lJOhfouQ1I" };
            var response = await GooglePlaces.Details.QueryAsync(req);
            return response.Result.Geometry.Location;
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

        public LatLng[] getPath(LatLng source, LatLng destination, out LatLngBounds latLngBounds)
        {
            DirectionsRequest directionsRequest = new DirectionsRequest();
            directionsRequest.Key = "AIzaSyBA58FFbrOgnkHm5k3-i1cF2lJOhfouQ1I";
            directionsRequest.Origin = new GoogleApi.Entities.Common.Location(source.Latitude, source.Longitude);
            directionsRequest.Destination = new GoogleApi.Entities.Common.Location(destination.Latitude, destination.Longitude);
            DirectionsResponse directionsResponse = GoogleApi.GoogleMaps.Directions.Query(directionsRequest);
            Route route = directionsResponse.Routes.First();
            LatLng[] points = route.OverviewPath.Line.Select(x=>new LatLng(x.Latitude,x.Longitude)).ToArray();
            latLngBounds = new LatLngBounds(new LatLng(route.Bounds.SouthWest.Latitude, route.Bounds.SouthWest.Longitude), new LatLng(route.Bounds.NorthEast.Latitude, route.Bounds.NorthEast.Longitude));
            return points;
        }

        public LatLng[] getNearbyLocations(LatLng[] path, LatLng[] locations)
        {
	        List<LatLng> result = new List<LatLng>();
	        foreach (var latLng in path)
	        {
		        result.AddRange(locations.Where(x=> ((Math.Abs((x.Latitude - latLng.Latitude)) <= 0.002) && (Math.Abs((x.Longitude - latLng.Longitude)) <= 0.002)) || (Math.Abs((x.Latitude-latLng.Latitude)) <=0.002) || (Math.Abs((x.Longitude - latLng.Longitude)) <= 0.002)));
	        }

	        return result.Distinct().ToArray();
        }
    }

    
}