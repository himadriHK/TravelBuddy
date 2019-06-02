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

namespace TravelBuddy
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
	    private LocationHelper location = new LocationHelper();

		  protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) ==
                Permission.Denied || ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) ==
                Permission.Denied)
            {
                location.GetGpsAccess(this, FindViewById(Resource.Layout.activity_main));
            }
            else
                 await location.StartLocationRequestAsync(this);

            // Set our view from the "main" layout resource
            
            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.fragment1);
            mapFragment.GetMapAsync(this);

            AutoCompleteTextView autoCompleteSource = FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextView1);
            autoCompleteSource.KeyPress += AutoCompleteSource_KeyPress;
            autoCompleteSource.TextChanged += AutoCompleteSource_TextChanged;
        }

        private void AutoCompleteSource_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            
            if (e.Text.ToString() != string.Empty)
            {
                AutoCompleteTextView autoCompleteSource = (AutoCompleteTextView)sender;
                string[] places = location.getLocationByText(autoCompleteSource.Text);
                var adapter = new ArrayAdapter<String>(this, Resource.Layout.list_view, places);
                autoCompleteSource.Adapter = adapter;
            }
        }

        private void AutoCompleteSource_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            //if (SystemClock.UptimeMillis() - e.Event.DownTime >= 500)
            {
               
            }
        }

        private void Et_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            //e.Handled = false;
            if (e.Event.Action == Android.Views.KeyEventActions.Down && e.KeyCode == Android.Views.Keycode.Tab)
            {
                //Toast.MakeText(this, ((EditText)sender).Text, ToastLength.Short).Show();
                //location.getLocationByText(((EditText)sender).Text,this);
                e.Handled = true;
            }
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

        public void OnMapReady(GoogleMap googleMap)
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) ==
                Permission.Granted || ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) ==
                Permission.Granted)
            {
                googleMap.MyLocationEnabled = true;
                CameraPosition cameraPosition = new CameraPosition.Builder().Target(new Android.Gms.Maps.Model.LatLng(location.currentLocation.Latitude, location.currentLocation.Longitude)).Zoom(15).Build();
                googleMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            }
                
            //throw new System.NotImplementedException();
        }
    }
}