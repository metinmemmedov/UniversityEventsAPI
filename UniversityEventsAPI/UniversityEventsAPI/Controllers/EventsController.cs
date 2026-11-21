using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UniversityEventsAPI.Models;

namespace UniversityEventsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        // In-memory storage (static so data survives between requests)
        private static readonly List<Event> _events = new();
        private static int _nextId = 1;

        // Returns an error message, or null if the event is valid
        private static string? Validate(Event e)
        {
            if (string.IsNullOrWhiteSpace(e.Name))
                return "Event name cannot be empty.";
            if (e.EventDate < DateTime.Now)
                return "Event date cannot be in the past.";
            if (e.MaxParticipants < 1)
                return "MaxParticipants must be at least 1.";
            return null;
        }

        // GET /api/events
        // Optional query params (bonus tasks):
        //   ?search=ai            -> search by name
        //   ?location=Baku        -> filter by city
        //   ?upcomingOnly=true    -> only future events
        //   ?minParticipants=100  -> events with MORE than 100 participants
        //   ?sort=asc | desc      -> sort by date
        [HttpGet]
        public ActionResult<IEnumerable<Event>> GetAll(
            string? search, string? location, bool upcomingOnly = false,
            int? minParticipants = null, string? sort = null)
        {
            IEnumerable<Event> query = _events;

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(e => e.Location.Equals(location, StringComparison.OrdinalIgnoreCase));

            if (upcomingOnly)
                query = query.Where(e => e.EventDate > DateTime.Now);

            if (minParticipants.HasValue)
                query = query.Where(e => e.MaxParticipants > minParticipants.Value);

            if (sort?.ToLower() == "asc")
                query = query.OrderBy(e => e.EventDate);
            else if (sort?.ToLower() == "desc")
                query = query.OrderByDescending(e => e.EventDate);

            return Ok(query.ToList());
        }

        // GET /api/events/export  -> downloads events as a JSON file
        [HttpGet("export")]
        public IActionResult Export()
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(_events,
                new JsonSerializerOptions { WriteIndented = true });
            return File(json, "application/json", "events.json");
        }

        // GET /api/events/{id}
        [HttpGet("{id:int}")]
        public ActionResult<Event> GetById(int id)
        {
            var ev = _events.FirstOrDefault(e => e.Id == id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }

        // POST /api/events
        [HttpPost]
        public ActionResult<Event> Create(Event newEvent)
        {
            var error = Validate(newEvent);
            if (error != null) return BadRequest(error);

            newEvent.Id = _nextId++;
            _events.Add(newEvent);

            // 201 Created + Location header pointing to the new resource
            return CreatedAtAction(nameof(GetById), new { id = newEvent.Id }, newEvent);
        }

        // PUT /api/events/{id}
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, Event updated)
        {
            var existing = _events.FirstOrDefault(e => e.Id == id);
            if (existing == null) return NotFound();

            var error = Validate(updated);
            if (error != null) return BadRequest(error);

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.Location = updated.Location;
            existing.EventDate = updated.EventDate;
            existing.MaxParticipants = updated.MaxParticipants;

            return Ok(existing);
        }

        // DELETE /api/events/{id}
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var existing = _events.FirstOrDefault(e => e.Id == id);
            if (existing == null) return NotFound();

            _events.Remove(existing);
            return NoContent(); // 204
        }
    }
}
