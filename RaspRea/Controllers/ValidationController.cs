using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaspRea.Dto;

namespace RaspRea.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidationController : ControllerBase
    {
        private readonly HttpClient _client = new HttpClient();
        private HtmlParser _parser = new HtmlParser();
        /// <summary>
        /// Return weekly timetable to request week
        /// </summary>
        /// <returns>Weekly timetable</returns>
        [HttpGet]
        [Route("/CheckGroupForAvailability/")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<bool> CheckGroupForAvailability([FromQuery] string group)
        {
            var htmlEncodeGroup = HttpUtility.HtmlEncode(group.ToLower());
            var request = new HttpRequestMessage() {
                RequestUri = new Uri($"https://rasp.rea.ru/Schedule/ScheduleCard?selection={htmlEncodeGroup}&weekNum=-1&catfilter=0"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Accept", "text/html, */*; q=0.01");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            
            var resultContent = await _client.SendAsync(request);
            var result = await resultContent.Content.ReadAsStringAsync();
            var parsResult = _parser.ParseDocument(result);
            var parsSelector = parsResult.QuerySelectorAll("h2");
            foreach (var element in parsSelector)
            {
                if (element.TextContent.Contains("не найдено результатов"))
                    return false;
            }

            return true;
        }
        
    }
}