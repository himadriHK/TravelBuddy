using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using System.Linq;
using Android.Content.PM;
using Android.Gms.Maps;

namespace TravelBuddy
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback
    {
	    private LocationHelper location = new LocationHelper();

		  protected override  void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) ==
            //    Permission.Denied || ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) ==
            //    Permission.Denied)
            //{
            //    location.GetGpsAccess(this, FindViewById(Resource.Layout.activity_main));
            //}
            //else
            //     await location.StartLocationRequestAsync(this);

            // Set our view from the "main" layout resource
            
            //var mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.fragment1);
            //mapFragment.GetMapAsync(this);
           
            //EditText et = FindViewById<EditText>(Resource.Id.editText2);
            //et.KeyPress += Et_KeyPress;
        }

        private void Et_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            //e.Handled = false;
            if (e.Event.Action == Android.Views.KeyEventActions.Down && e.KeyCode == Android.Views.Keycode.Tab)
            {
                //Toast.MakeText(this, ((EditText)sender).Text, ToastLength.Short).Show();
                location.getLocationByText(((EditText)sender).Text,this);
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
            //throw new System.NotImplementedException();
        }
    }
}