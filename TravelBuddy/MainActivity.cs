using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using System.Linq;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Support.V4.Content;
using Android;
using Android.Gms.Maps.Model;
using System;
using Android.Support.V4.App;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Gms.Location;
using Android.Locations;
using Xamarin.Essentials;
using System.Collections.Generic;

namespace TravelBuddy
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
	    private LocationHelper location = new LocationHelper();
        public LocationCallback locationCallback;
        CustomArrayAdapter adapter;
        GoogleMap googleMap;

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

            AutoCompleteTextView autoCompleteSource = FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextView1);
            adapter = new CustomArrayAdapter(this, Resource.Layout.list_view, location.getLocationByText);
            
            autoCompleteSource.Adapter = adapter;
            autoCompleteSource.Threshold = 2;
            autoCompleteSource.TextChanged += AutoCompleteSource_TextChanged;
            autoCompleteSource.ItemClick += AutoCompleteSource_ItemClick;
        }

        private async void AutoCompleteSource_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            View vw = FindViewById(Resource.Layout.activity_main);
            KeyValuePair<string,string> kvp = adapter.GetPlaceDetails(e.Position);
            GoogleApi.Entities.Common.Location loc = await location.GetLatLngFromPlaceId(kvp.Key);
            LatLng latLng = new LatLng(loc.Latitude, loc.Longitude);
            CameraPosition cameraPosition = new CameraPosition.Builder().Target(latLng).Zoom(13).Build();
            googleMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition), 3000, null);
            //Snackbar.Make(vw, str, Snackbar.LengthLong).SetAction(str,new Action<View>(delegate (View obj) { })).Show();
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

        private void AutoCompleteSource_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            
          //if (e.Text.ToString() != string.Empty)
          //{
          //    //RunOnUiThread(async () =>
          //    //{
          //    //    Dictionary<string, string> places = await location.getLocationByText(e.Text.ToString());
          //    //    adapter.AddNewPlaces(places);
          //    //    
          //    //});
          //    ((AutoCompleteTextView)sender).Adapter = adapter;
          //}
        }
    

        protected override void OnRestart()
        {
	        base.OnRestart();
	        SetContentView(Resource.Layout.activity_main);

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
                CameraPosition cameraPosition = new CameraPosition.Builder().Target(new Android.Gms.Maps.Model.LatLng(location.currentLocation.Latitude, location.currentLocation.Longitude)).Zoom(13).Build();
                googleMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition),3000,null);
            }
                
            //throw new System.NotImplementedException();
        }
    }
}