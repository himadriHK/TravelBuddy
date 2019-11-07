
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderSharp
{
	public class Scrapper : SpiderEngine, ISpiderEngine
	{
		private Queue<string> links;
		public List<Tuple<Tuple<double, double>,string>> locations;
		public Scrapper()
		{
			this.SetUrl("http://en.wikivoyage.org/wiki/India");
			links = new Queue<string>();
			locations = new List<Tuple<Tuple<double, double>,string>>();
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
				if (!links.Contains(node.GetHref("a")) && nextLink != node.GetHref("a"))
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
			locations.Add(Tuple.Create(Tuple.Create(Double.Parse(context.Data.latitude), double.Parse(context.Data.longitude)),context.Data.name));
			Console.WriteLine(context.Data.latitude);
			Console.WriteLine(context.Data.longitude);
			Console.WriteLine(context.Data.name);
			Console.WriteLine("-------------------------------");

			return base.SuccessPipelineAsync(context);
		}

	}
}
