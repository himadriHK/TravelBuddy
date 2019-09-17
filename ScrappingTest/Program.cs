using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using MongoDB.Bson;
using SpiderSharp;

namespace ScrappingTest
{
	class Program
	{
		static void Main(string[] args)
		{
			ScrapQuotesSpider spider = new ScrapQuotesSpider();
			spider.RunAsync().Wait();
			Console.ReadLine();
		}
	}

	public class ScrapQuotesSpider : SpiderEngine, ISpiderEngine
	{
		private Queue<string> links;
		public ScrapQuotesSpider()
		{
			this.SetUrl("http://en.wikivoyage.org/wiki/India");
			links= new Queue<string>();
		}

		protected override string FollowPage()
		{
			//if (!this.node.Exist("div#region_list > table > tbody > tr > td > a"))
			//{
			//	this.SetNofollow();
			//	return string.Empty;
			//}

			string nextLink = links.Count == 0
				? $"http://en.wikivoyage.org{this.node.GetHref("div#region_list > table > tbody > tr > td > b > a")}"
				: links.Dequeue();

			foreach (var node in this.node.SelectNodes("#region_list > table > tbody > tr > td > b > a"))
			{
				if(!links.Contains(node.GetHref("a")))
					links.Enqueue($"http://en.wikivoyage.org{node.GetHref("a")}");
			}

			return nextLink;
		}

		protected override IEnumerable<SpiderContext> OnRun()
		{
			var destinations = this.node.SelectNodes("span.listing-name");
			foreach (var item in destinations)
			{
				yield return this.Fetch(() => {
					ct.Data.text = item.GetInnerText();
				});
				if(!links.Contains($"http://en.wikivoyage.org{item.GetHref("a")}"))
					links.Enqueue($"http://en.wikivoyage.org{item.GetHref("a")}");
			}
		}

		protected override Task SuccessPipelineAsync(SpiderContext context)
		{
			Console.WriteLine(context.Data.text);
			return base.SuccessPipelineAsync(context);
		}

	}
}
