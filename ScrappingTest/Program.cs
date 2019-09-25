using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpiderSharp;

namespace ScrappingTest
{
	class Program
	{
		static void Main(string[] args)
		{
			//ScrapQuotesSpider spider = new ScrapQuotesSpider();
			//spider.RunAsync().Wait();
			double lon1 = 10;
			double lon2 = 10.1;
			double lat1 = 11.1;
			double lat2 = 11.2;
			Console.WriteLine(DistanceAlgorithm.DistanceBetweenPlaces(lon1,lat1,lon1+0.01,lat1+0.01));
			Console.ReadLine();
		}
	}
	static class DistanceAlgorithm
	{
		const double PIx = 3.141592653589793;
		const double RADIUS = 6371;

		/// <summary>
		/// Convert degrees to Radians
		/// </summary>
		/// <param name="x">Degrees</param>
		/// <returns>The equivalent in radians</returns>
		public static double Radians(double x)
		{
			return x * PIx / 180;
		}

		/// <summary>
		/// Calculate the distance between two places.
		/// </summary>
		/// <param name="lon1"></param>
		/// <param name="lat1"></param>
		/// <param name="lon2"></param>
		/// <param name="lat2"></param>
		/// <returns></returns>
		public static double DistanceBetweenPlaces(
			double lon1,
			double lat1,
			double lon2,
			double lat2)
		{
			double dlon = Radians(lon2 - lon1);
			double dlat = Radians(lat2 - lat1);

			double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(Radians(lat1)) * Math.Cos(Radians(lat2)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
			double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			return angle * RADIUS;
		}

	}
	public class ScrapQuotesSpider : SpiderEngine, ISpiderEngine
	{
		private Queue<string> links;
		//public List<Location> locations;
		public ScrapQuotesSpider()
		{
			this.SetUrl("http://en.wikivoyage.org/wiki/India");
			links= new Queue<string>();
			//locations=new List<Location>();
		}

		public void PrepareData()
		{
			RunAsync().Wait();
		}

		protected override string FollowPage()
		{
			//if (!this.node.Exist("div#region_list > table > tbody > tr > td > a"))
			//{
			//	this.SetNofollow();
			//	return string.Empty;
			//}

			string nextLink = links.Count == 0
				? $"{this.node.GetHref("div#region_list > table > tbody > tr > td > b > a")}"
				: links.Dequeue();

			foreach (var node in this.node.SelectNodes("#region_list > table > tbody > tr > td > b > a"))
			{
				if(!links.Contains(node.GetHref("a")) && nextLink!= node.GetHref("a"))
					links.Enqueue($"{node.GetHref("a")}");
			}

			return $"http://en.wikivoyage.org{nextLink}";
		}

		protected override IEnumerable<SpiderContext> OnRun()
		{
			var destination_coords = this.node.SelectNodes("span.vcard > span.listing-coordinates > span.geo").ToArray();
			var destination_names = this.node.SelectNodes("span.vcard > span.listing-name").ToArray();
			foreach (var item_cord in destination_coords)  
			{
				yield return this.Fetch(() => {
					ct.Data.latitude = item_cord.SelectNodes("abbr.latitude").First().GetInnerText();
					ct.Data.longitude = item_cord.SelectNodes("abbr.longitude").First().GetInnerText();
					ct.Data.name = destination_names[(Array.IndexOf(destination_coords, item_cord))].GetInnerText();
				});
				//if(!links.Contains($"http://en.wikivoyage.org{item.GetHref("a")}"))
				//	links.Enqueue($"http://en.wikivoyage.org{item.GetHref("a")}");
			}
		}

		protected override Task SuccessPipelineAsync(SpiderContext context)
		{
			//locations.Add(new Location(context.Data.latitude, context.Data.longitude));
			Console.WriteLine(context.Data.latitude);
			Console.WriteLine(context.Data.longitude);
			Console.WriteLine(context.Data.name);
			Console.WriteLine("-------------------------------");

			return base.SuccessPipelineAsync(context);
		}

	}
}
