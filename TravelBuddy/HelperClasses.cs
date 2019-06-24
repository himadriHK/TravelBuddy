using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Android.Util;
using System.Threading.Tasks;

namespace TravelBuddy
{
    public class CustomArrayAdapter : BaseAdapter, IFilterable
    {
        Context ctx;
        int resId;
        //Dictionary<string, string> placesMap;
        //string[] placesMapOriginal = new string[] { };
        ComparePlaces comparePlaces = new ComparePlaces();
        Func<string, Dictionary<string, string>> placeSearchCall;
        CustomFilter customFilter;
        public CustomArrayAdapter(Context context, int resourceId, Func<string, Dictionary<string, string>> placeSearchCall) : base()
        {
            ctx = context;
            resId = resourceId;
            this.placeSearchCall = placeSearchCall;
            customFilter = new CustomFilter(placeSearchCall, this);
            customFilter.placesMap = new Dictionary<string, string>();
            // this.placesMap = placesMap;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            //Pair placeDetails = new Pair(placesMap.ElementAt(position).Key, placesMap.ElementAt(position).Value);
            return customFilter.placesMap.ElementAt(position).Value;// placeDetails;
        }

        public override int Count
        {
            get
            {
                return customFilter.placesMap.Count;
            }
        }

        public void AddNewPlaces(Dictionary<string,string> placesMap)
        {
            //this.placesMap = this.placesMap.Union(placesMap, comparePlaces).ToDictionary(x=>x.Key,x=>x.Value);
            //Clear();
            //AddAll((ICollection)placesMap.Values.ToArray());
        }

       public KeyValuePair<string, string> GetPlaceDetails(int position)
       {
          KeyValuePair<string, string> kvp = customFilter.placesMap.ElementAt(position);
          
          return kvp;
       }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = LayoutInflater.From(ctx).Inflate(resId, parent, false);
            }
            TextView autoCompleteOption = convertView.FindViewById<TextView>(Resource.Id.autoCompleteOption);
            autoCompleteOption.Text = GetItem(position).ToString();//((Pair)GetItem(position)).Second.ToString();

            return convertView;
        }

        public override long GetItemId(int position)
        {
            return position;// placesMap.ElementAt(position).Key.GetHashCode();
        }

        public Filter Filter
        {
            get
            {
                return customFilter;
            }
        }
    }

    public class CustomFilter : Filter
    {
        Func<string, Dictionary<string, string>> placeSearchCall;
        public Dictionary<string, string> placesMap;
        //public string[] placesMap;
        BaseAdapter adapter;
        public CustomFilter(Func<string, Dictionary<string, string>> placeSearchCall,BaseAdapter adapter):base()
        {
            
            this.placeSearchCall = placeSearchCall;
            this.adapter = adapter;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            FilterResults results = new FilterResults();
            results.Values = new Pair[] { };
            results.Count = 0;
            if(constraint !=null)
            {
                try
                {
                    var placesDict = placeSearchCall(constraint.ToString());
                    results.Values = new JavaList(placesDict.Select(x => new Pair(x.Key, x.Value)).ToArray());
                    results.Count = placesDict.Count;
                }
                catch(System.AggregateException )
                {
                    
                }
                
            }
            return results;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            if (results.Count > 0)
            {
                var pairs = results.Values.JavaCast<JavaList>().ToArray();
                placesMap = new Dictionary<string, string>();
                foreach (Pair p in pairs)
                    placesMap.Add(p.First.ToString(), p.Second.ToString());

                //var pairs2 = results.Values.JavaCast<JavaList>();
                //placesMap = results.Values.JavaCast<JavaList>().ToArray<Pair>().ToDictionary(x => x.First.ToString(), x => x.Second.ToString());
                adapter.NotifyDataSetChanged();
            }
            else
                adapter.NotifyDataSetInvalidated();
        }
    }

    public class ComparePlaces : IEqualityComparer<KeyValuePair<string, string>>
    {

        public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
        {
            return x.Key == y.Key;
        }

        public int GetHashCode(KeyValuePair<string, string> obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}