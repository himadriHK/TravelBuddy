﻿using Android;
using Android.App;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
namespace TravelBuddy
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity, IOnMapReadyCallback
	{
		private LocationHelper location = new LocationHelper();
		public LocationCallback locationCallback;
		CustomArrayAdapter adapter;
		GoogleMap googleMap;
		MarkerOptions sourceMarkerOptions = new MarkerOptions();
		MarkerOptions destinationMarkerOptions = new MarkerOptions();
		InputMethodManager inputManager;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);
			if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) ==
				 Permission.Denied || ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) ==
				 Permission.Denied)
			{
				GetGpsAccess(this, FindViewById(Resource.Layout.activity_main));
			}
			else
			{
				ShowMap();
			}

			AutoCompleteTextView autoCompleteSource = FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteSource);
			AutoCompleteTextView autoCompleteDestination = FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteDestination);
			adapter = new CustomArrayAdapter(this, Resource.Layout.list_view, location.getLocationByText);

			autoCompleteSource.Adapter = adapter;
			autoCompleteSource.Threshold = 2;
			autoCompleteSource.ItemClick += AutoCompleteSource_ItemClick;

			autoCompleteDestination.Adapter = adapter;
			autoCompleteDestination.Threshold = 2;
			autoCompleteDestination.ItemClick += AutoCompleteSource_ItemClick;

			location.getLocationByText("India");

			inputManager = (InputMethodManager)GetSystemService(InputMethodService);
            if (!location.allSites.Any())
                location.UpdateSitesData().Wait();
		}

		private async void AutoCompleteSource_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			AutoCompleteTextView autoCompleteTextView = (AutoCompleteTextView)sender;
			KeyValuePair<string, string> kvp = adapter.GetPlaceDetails(e.Position);
			GoogleApi.Entities.Common.Location loc = await location.GetLatLngFromPlaceId(kvp.Key);
			LatLng latLng = new LatLng(loc.Latitude, loc.Longitude);
			CameraPosition cameraPosition = new CameraPosition.Builder().Target(latLng).Zoom(15).Build();
			googleMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition), 3000, null);
			googleMap.Clear();

			if (autoCompleteTextView.Id == Resource.Id.autoCompleteSource)
			{
				sourceMarkerOptions.SetPosition(latLng);
				sourceMarkerOptions.SetTitle("Start");
			}
			else if (autoCompleteTextView.Id == Resource.Id.autoCompleteDestination)
			{
				destinationMarkerOptions.SetPosition(latLng);
				destinationMarkerOptions.SetTitle("End");
			}

			if (sourceMarkerOptions.Position != null)
			{
				googleMap.AddMarker(sourceMarkerOptions);
			}

			if (destinationMarkerOptions.Position != null)
			{
				googleMap.AddMarker(destinationMarkerOptions);
			}

			if (sourceMarkerOptions.Position != null && destinationMarkerOptions.Position != null)
			{
				ShowTheWay();
			}
		}

		private void ShowTheWay()
		{
			PolylineOptions polylineOptions = new PolylineOptions();
			LatLngBounds bounds;
			LatLng[] points = location.getPath(sourceMarkerOptions.Position, destinationMarkerOptions.Position, out bounds);
			polylineOptions.Add(points);
			googleMap.AddPolyline(polylineOptions);

            var filteredSites = location.allSites.Where(x => bounds.Contains(x.Item1));

			
			googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(bounds, 60), 2000, null);


			inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);

            foreach(var site in filteredSites)
            {
                var markerOp = new MarkerOptions();
                markerOp.SetPosition(site.Item1);
                markerOp.SetTitle(site.Item2);
                googleMap.AddMarker(markerOp);

            }
		}

		private async void ShowMap()
		{
			var request = new GeolocationRequest(GeolocationAccuracy.Medium);
			var gps_location = await Geolocation.GetLocationAsync(request);
			location.currentLocation = new Android.Locations.Location(LocationService) { Latitude = gps_location.Latitude, Longitude = gps_location.Longitude };
			var mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.fragment1);
			mapFragment.GetMapAsync(this);
		}

		private void GetGpsAccess(Activity activity, View view)
		{
			if (ActivityCompat.ShouldShowRequestPermissionRationale(activity, Manifest.Permission.AccessCoarseLocation) || ActivityCompat.ShouldShowRequestPermissionRationale(activity, Manifest.Permission.AccessFineLocation))
			{

				var requiredPermissions = new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };
				Snackbar.Make(view,
						  Resource.String.permission_location_rationale,
						  Snackbar.LengthIndefinite)
					 .SetAction(Resource.String.ok,
						  new Action<View>(delegate (View obj)
						  {
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

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			if (grantResults.All(x => x == Permission.Granted))
			{
				ShowMap();
			}

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) ==
				 Permission.Granted || ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) ==
				 Permission.Granted)
			{
				this.googleMap = googleMap;
				googleMap.MyLocationEnabled = true;
				LatLng latLng = new Android.Gms.Maps.Model.LatLng(location.currentLocation.Latitude, location.currentLocation.Longitude);
				CameraPosition cameraPosition = new CameraPosition.Builder().Target(latLng).Zoom(13).Build();
				sourceMarkerOptions.SetPosition(latLng);

				googleMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition), 3000, null);
			}

			googleMap.UiSettings.ScrollGesturesEnabled = true;
			googleMap.UiSettings.ZoomGesturesEnabled = true;

		}


	}
}