using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaspRea.Application;
using RaspRea.Dto;

namespace RaspRea.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TimetableController : ControllerBase
    {
        private ScheduleParser _parser = new ScheduleParser();
        /// <summary>
        /// Return weekly timetable to request week
        /// </summary>
        /// <returns>Weekly timetable</returns>
        [HttpGet]
        [Route("/GetWeeklyTimetable/")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(WeeklyTimetable), StatusCodes.Status200OK)]
        public async Task<List<DailyTimetable>> GetWeeklyTimetable([FromQuery] int week, [FromQuery] string group)
        {
            var listOfDays = await _parser.GetWeeklyTimeTable(group, week);
            return listOfDays;
        }
        
    }
}