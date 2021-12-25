using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using RaspRea.Dto;

namespace RaspRea.Application
{
    public class ScheduleParser
    {
        private readonly HttpClient _client = new HttpClient();
        private HtmlParser _parser = new HtmlParser();

        public async Task<List<DailyTimetable>> GetWeeklyTimeTable(string groupName, int week)
        {
            var htmlEncodeGroup = HttpUtility.HtmlEncode(groupName.ToLower());
            var request = new HttpRequestMessage() {
                RequestUri = new Uri($"https://rasp.rea.ru/Schedule/ScheduleCard?selection={htmlEncodeGroup}&weekNum={week}&catfilter=0"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Accept", "text/html, */*; q=0.01");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            var resultContent = await _client.SendAsync(request);
            var result = await resultContent.Content.ReadAsStringAsync();
            var parsResult = _parser.ParseDocument(result);

            return ParseWeek(parsResult);

        }

        public async Task<ClassInfo> GetDetails(string selection, string date, string timeSlot)
        {
            var htmlEncodeGroup = HttpUtility.HtmlEncode(selection.ToLower());
            var request = new HttpRequestMessage() {
                RequestUri = new Uri($"https://rasp.rea.ru/Schedule/GetDetails?selection={htmlEncodeGroup}&date={date}&timeSlot={timeSlot}"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Accept", "text/html, */*; q=0.01");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            
            var resultContent = await _client.SendAsync(request);
            var result = await resultContent.Content.ReadAsStringAsync();
            var parsResult = _parser.ParseDocument(result);

            return ParseDetails(parsResult);
        }

        public ClassInfo ParseDetails(IHtmlDocument parsResult)
        {
            var classInfo = new ClassInfo();
            var parsSelector = parsResult.QuerySelectorAll("*")[2].TextContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            classInfo.Name = parsSelector[0].Trim();
            classInfo.Type = parsSelector[1].Trim();
            classInfo.Date = parsSelector[2].Trim();
            classInfo.Building = parsSelector[4].Trim().TrimEnd('-').Trim();
            classInfo.Room = parsSelector[5].Trim();
            classInfo.Teacher = parsSelector[17].Trim().Remove(0, 7);
            classInfo.Сhair = parsSelector[20].Trim().TrimStart('(').TrimEnd(')');
            
            return classInfo;
        }

        public List<DailyTimetable> ParseWeek(IHtmlDocument parsResult)
        {
            var week = new List<DailyTimetable>();
            foreach (var weekResult in parsResult.QuerySelectorAll(".container"))
            {
                var day = new DailyTimetable
                {
                    Lessons = new List<Lesson>()
                };
                day.Name = weekResult.QuerySelectorAll(".dayh").First().TextContent;

                foreach (var lessonResult in weekResult.QuerySelectorAll(".slot"))
                {
                    var lesson = new Lesson();
                    if (!lessonResult.QuerySelectorAll(".pcap").Any())
                        day.IsFree = true;
                    else
                    {
                        day.IsFree = false;
                        lesson.Number = lessonResult.QuerySelectorAll(".pcap").First().TextContent;

                        if (lessonResult.QuerySelectorAll(".task").Any())
                        {
                            foreach (var para in lessonResult.QuerySelectorAll(".task"))
                            {
                                List<string> notNullOrSpace = new List<string>();
                                var info = para.TextContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                                foreach (var str in info)
                                {
                                    if (!string.IsNullOrWhiteSpace(str))
                                        notNullOrSpace.Add(str);
                                }

                                lesson.Name = notNullOrSpace[0].Trim(' ');
                                lesson.Type = notNullOrSpace[1].Trim(' ');
                                lesson.Building = notNullOrSpace[2].Trim(' ');
                                lesson.Room = notNullOrSpace[3].Trim(' ');
                                lesson.Info = notNullOrSpace[4].Trim(' ');
                            }
                            
                        }
                    }
                    day.Lessons.Add(lesson);
                }
                week.Add(day);
            }

            return week;
        }

    }
}