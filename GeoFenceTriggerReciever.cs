using Android.App;
using Android.Content;
using Android.Util;
using Android.Locations;
using System.Collections.Generic;
using System;

namespace gmaps_tutorial
{
    [BroadcastReceiver(Exported = false)]
    [IntentFilter(new[] { GeoFenceTriggerReciever.IntentName })]
   
    public class GeoFenceTriggerReciever : BroadcastReceiver
    {
        readonly Dictionary<int, Tuple<Action<int>, Action<int>>> actions = new Dictionary<int, Tuple<Action<int>, Action<int>>>();

        private const string IntentName = "com.silberne.g_mapstutorial2.geofence";

        public void RegisterActions(int key, Action<int> enterAction, Action<int> exitAction)
        {
            var work = Tuple.Create(enterAction, exitAction);
            actions.Add(key, work);
        }

        public void UnRegisterActions(int key)
        {
            actions.Remove(key);
        }

       

        public override void OnReceive(Context context, Intent intent)
        {
            bool entering = intent.GetBooleanExtra(LocationManager.KeyProximityEntering, false);

            var extras = intent.GetBundleExtra(IntentName);
            string poiName = extras.GetString("name");
            int id = extras.GetInt("id");

            Tuple<Action<int>, Action<int>> work;
            actions.TryGetValue(id, out work);

            if (entering)
            {
                Log.Debug(GetType().Name, "Entering" + poiName + "-" + id);
                if (work != null && work.Item1 != null)
                {
                    work.Item1(id);
                }
            }
            else
            {
                Log.Debug(GetType().Name, "Exiting" + poiName + "-" + id);
                if (work != null && work.Item2 != null)
                {
                    work.Item2(id);
                }
            }
        }

        
    }
}