using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace RaspRea.Application
{
    public class ScheduleParser
    {
        private readonly HttpClient _client = new HttpClient();
        private HtmlParser _parser = new HtmlParser();

        public async Task GetWeeklyTimeTable()
        {
            var request = new HttpRequestMessage() {
                RequestUri = new Uri("https://rasp.rea.ru/Schedule/ScheduleCard?selection=291%D0%B4-11%D0%BC%D0%BE%2F18&weekNum=-1&catfilter=0"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Accept", "text/html, */*; q=0.01");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            var resultContent = await _client.SendAsync(request);
            var result = await resultContent.Content.ReadAsStringAsync();

            var parsResult = _parser.ParseDocument(result);

            var cellSelector = ".container";
            var cells = parsResult.QuerySelectorAll(cellSelector);
            //var pars = cells.Select(m => m.QuerySelectorAll());


            //Console.WriteLine(result);

        }

    }
}