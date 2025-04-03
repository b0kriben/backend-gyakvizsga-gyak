using BackendGyakProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendGyakProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly DatabaseContext _context = new DatabaseContext();

        [HttpGet("count")]
        public async Task<IActionResult> GetEventCountAsync()
        {
            var eventCount = await _context.Events.CountAsync();
            return Ok(new { event_count = eventCount });
        }

        [HttpGet("min_date")]
        public async Task<IActionResult> GetEarliestEventDateAsync()
        {
            var earliestEvent = await _context.Events
                .OrderBy(e => e.EventDateTime)
                .FirstOrDefaultAsync();
            return Ok(new { earliest_event = earliestEvent?.EventDateTime });
        }

        [HttpGet("future_conferences")]
        public async Task<IActionResult> GetFutureConferencesAsync()
        {
            var futureConferences = await _context.Events
                .Where(e => e.EventDateTime > new DateTime(2025, 1, 1) && e.Title.Contains("konferencia"))
                .ToListAsync();
            return Ok(new { events = futureConferences });
        }

        [HttpGet("titles_sorted")]
        public async Task<IActionResult> GetEventsSortedByDateAsync()
        {
            var events = await _context.Events
                .OrderBy(e => e.EventDateTime)
                .Select(e => new { e.Title, e.EventDateTime })
                .ToListAsync();
            return Ok(new { events });
        }

        [HttpGet("authors/event_count")]
        public async Task<IActionResult> GetEventCountByAuthorAsync()
        {
            var authorEventCount = await _context.Events
                .GroupBy(e => e.AuthorId)
                .Select(g => new { AuthorId = g.Key, event_count = g.Count() })
                .ToListAsync();
            return Ok(new { author_events = authorEventCount });
        }

        [HttpGet("events_by_author/{authorId}")]
        public async Task<IActionResult> GetEventsByAuthorAsync(int authorId)
        {
            var events = await _context.Events
                .Where(e => e.AuthorId == authorId)
                .ToListAsync();
            return Ok(new { events });
        }

        /*[HttpPut("update_title")]
        public async Task<IActionResult> UpdateEventTitleAsync([FromBody] UpdateEventTitleRequest request)
        {
            var eventToUpdate = await _context.Events.FindAsync(request.ID);
            if (eventToUpdate == null)
            {
                return NotFound();
            }
            eventToUpdate.Title = request.Title;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }*/

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEventAsync(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete == null)
            {
                return NotFound();
            }
            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpGet("events_with_authors")]
        public async Task<IActionResult> GetEventsWithAuthorsAsync()
        {
            var eventsWithAuthors = await _context.Events
                .Join(_context.Authors, e => e.AuthorId, a => a.Id, (e, a) => new { e.Id, e.Title, Author = a.Name })
                .ToListAsync();
            return Ok(new { events = eventsWithAuthors });
        }
    }
}

