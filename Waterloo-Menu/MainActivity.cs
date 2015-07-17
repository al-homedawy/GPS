using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Webkit;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace WaterlooMenu
{
	[Activity (Label = "Waterloo-Menu", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity, ILocationListener
	{
		Location currentLocation;
		LocationManager locationManager;
		String LocationProvider;
		MapFragment mapFrag;
		GoogleMap map;
		Marker UserPos;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Start 'GPS'
			InitializeLocationManager ();

			// Setup 'Google Maps'
			mapFrag = (MapFragment) FragmentManager.FindFragmentById(Resource.Id.map);
			map = mapFrag.Map;

		}

		void GenerateLoc ( Location location )
		{
			map = mapFrag.Map;

			if (map != null) {
				if (map.MapType != GoogleMap.MapTypeHybrid) {
					// Setup map.
					map.MapType = GoogleMap.MapTypeHybrid;
					map.UiSettings.ZoomControlsEnabled = true;
					map.UiSettings.CompassEnabled = true;

					// Setup camera position
					LatLng myLoc = new LatLng(location.Latitude, location.Longitude);
					CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
					builder.Target(myLoc);
					builder.Zoom(18);
					builder.Bearing(155);
					builder.Tilt(65);
					CameraPosition cameraPosition = builder.Build();
					CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
					map.MoveCamera (cameraUpdate);

					// Setup user position
					MarkerOptions YourPos = new MarkerOptions ();
					YourPos.SetPosition (new LatLng (location.Latitude, location.Longitude));
					YourPos.SetTitle ("You");
					UserPos = map.AddMarker (YourPos);

					// Setup destination
					MarkerOptions UWaterloo = new MarkerOptions ();
					UWaterloo.SetPosition (new LatLng (43.4688995, -80.54));
					UWaterloo.SetTitle ("University of Waterloo");
					map.AddMarker (UWaterloo);
				} else {
					// Mark your position.
					UserPos.Position = new LatLng (location.Latitude, location.Longitude);
				}
			}
		}

		void InitializeLocationManager()
		{
			locationManager = (LocationManager)GetSystemService(LocationService);

			Criteria criteriaForLocationService = new Criteria { Accuracy = Accuracy.Fine };
			IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				LocationProvider = acceptableLocationProviders.First();
			}
			else
			{
				LocationProvider = String.Empty;
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			locationManager.RequestLocationUpdates(LocationProvider, 0, 0, this);
		}

		protected override void OnPause()
		{
			base.OnPause();
			locationManager.RemoveUpdates(this);
		}

		public void OnLocationChanged(Location location)
		{
			currentLocation = location;
			GenerateLoc (location);
		}

		public void OnProviderDisabled(string provider) 
		{
			// Nothing
		}

		public void OnProviderEnabled(string provider) 
		{
			// Nothing
		}

		public void OnStatusChanged(string provider, Availability status, Bundle extras) 
		{
			// Nothing
		}
	}
}


