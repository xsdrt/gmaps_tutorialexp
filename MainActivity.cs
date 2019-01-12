using Android.App;
using System;
using Android.Views;
using Android.Runtime;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Content;

//added jarbinding lost to app , try and use and see if can use with google maps with out using google play services api

//try this to make sure on 

namespace gmaps_tutorial
{
    [Activity(Label = "gmaps_tutorial2", MainLauncher = true)]
    public class MainActivity : Activity, IOnMapReadyCallback, GoogleMap.IInfoWindowAdapter,ILocationListener
    {
        private GoogleMap mMap;

        private Button btnNormal;
        private Button btnHybrid;
        private Button btnSatellite;
        private Button btnTerrain;

        readonly GeoFenceTriggerReciever reciever = new GeoFenceTriggerReciever();


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Register our broadcast reciever
            IntentFilter filter = new IntentFilter(GeoFenceTriggerReciever.IntentName);
                RegisterReciever(reciever, filter);

            EditText location = FindViewById<EditText>(Resource.Id.location);
            location.Text = "Home";

            Button addButton = FindViewById<Button>(Resource.Id.addButton);


            addButton.Click += (sender, e) =>
            {
                string text = location.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    SetUpGeofence(text);
                }
            };



            btnNormal = FindViewById<Button>(Resource.Id.btnNormal);
            btnHybrid = FindViewById<Button>(Resource.Id.btnHybrid);
            btnSatellite = FindViewById<Button>(Resource.Id.btnSatellite);
            btnTerrain = FindViewById<Button>(Resource.Id.btnTerrain);

            btnNormal.Click += BtnNormal_Click;
            btnHybrid.Click += BtnHybrid_Click;
            btnSatellite.Click += BtnSatellite_Click;
            btnTerrain.Click += BtnTerrain_Click;
            SetUpMap();


        }

        private void BtnTerrain_Click(object sender, EventArgs e)
        {
            mMap.MapType = GoogleMap.MapTypeTerrain;
        }

        private void BtnSatellite_Click(object sender, EventArgs e)
        {
            mMap.MapType = GoogleMap.MapTypeSatellite;
        }

        private void BtnHybrid_Click(object sender, EventArgs e)
        {
            mMap.MapType = GoogleMap.MapTypeHybrid;
        }

        private void BtnNormal_Click(object sender, EventArgs e)
        {
            mMap.MapType = GoogleMap.MapTypeNormal;
        }

        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);

            }

        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;
            mMap.MyLocationEnabled = true;//added this 11/26/18 Works but sends me to sunnydale california, need to try on actual device...This works after manaully setting permissions,

            //need to code runtime permission check....


            LatLng latlng = new LatLng(47.174610, -88.691258);//Manhattan New York  changed to house latlng
            LatLng latlng2 = new LatLng(47, -87);
            googleMap.UiSettings.ZoomControlsEnabled = true;//added 11/26/18
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 7);
            mMap.MoveCamera(camera);



            MarkerOptions options = new MarkerOptions()
                .SetPosition(latlng)
                .SetTitle("My House")
                //.SetSnippet("AKA: The Big Apple")
                .Draggable(true);

            //Marker 1
            mMap.AddMarker(options);

            //Marker 2
            mMap.AddMarker(new MarkerOptions()
                .SetPosition(latlng2)
                .SetTitle("Marker 2"))
                .SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueMagenta));//Customise the marker to a different color...


            mMap.MarkerClick += mMap_MarkerClick;//Uses the mammarker click method to animate camera to clicked marker..

            mMap.MarkerDragEnd += mMap_MarkerDragEnd;//Sets the LatLng when the user stops dragging the marker
            mMap.SetInfoWindowAdapter(this);//Trying to get the info window working , having problems

            LocationManager locManager = LocationManager.FromContext(this);
            locManager.RequestLocationUpdates(LocationManager.GpsProvider, 100, 1, this);


        }

        //Method to animate the camera to the marker when a user clicks on it...
        private void mMap_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            LatLng pos = e.Marker.Position;
            mMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(pos, 7));
        }

        //Method to set the markers latlng when the user is finished dragging.
        private void mMap_MarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
        {
            LatLng pos = e.Marker.Position;
            Console.WriteLine(pos.ToString());
        }

        public View GetInfoContents(Marker marker)
        {
            return null;
        }

        public View GetInfoWindow(Marker marker)
        {
            View view = LayoutInflater.Inflate(Resource.Layout.info_window, null, true);

            return view;
        }

        public void OnLocationChanged(Location location)
        {
            throw new NotImplementedException();
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }
    }
}

// AIzaSyB2CiqG_we0liVHBVpNWcaX2dnvZKefBuY 