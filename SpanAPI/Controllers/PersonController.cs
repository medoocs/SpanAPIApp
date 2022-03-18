#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using SpanAPI.Data;

namespace SpanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<PersonController> logger;

        public PersonController(DataContext context, ILogger<PersonController> logger)
        {
            _context = context;
            this.logger = logger;
        }

        // GET: api/csv
        [HttpGet("csv")]
        public async Task<ActionResult<IEnumerable<PersonCsvDto>>> GetPersonsCsv()
        {
            try
            {
                Task<List<PersonCsvDto>> loadFromCsvTask = loadFromCsvAsync();
                List<PersonCsvDto> personCsvList = await loadFromCsvTask;

                logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {personCsvList.Count()} persons from file.");

                return personCsvList;
            }catch (FileNotFoundException)
            {
                logger.LogError("File podaci.csv not found!");
                return StatusCode(404, "File podaci.csv not found!");
            }
            
        }

        //POST: api/csv
        [HttpPost("csv")]
        public async Task<ActionResult<IEnumerable<PersonDto>>> PostCsvPersons()
        {
            try
            {
                Task<List<PersonCsvDto>> loadFromCsvTask = loadFromCsvAsync();
                List<PersonCsvDto> personCsvList = await loadFromCsvTask;

                personCsvList = personCsvList.Where(person => person.correctFormat.Equals(true)).ToList();

                int count = 0;

                try
                {
                    foreach (PersonCsvDto personCsvDto in personCsvList)
                    {
                        if (!PersonExistsByNumber(personCsvDto.Number))
                        {
                            _context.Persons.Add(personCsvDto.AsModel());
                            count++;
                        }
                    }
                    await _context.SaveChangesAsync();

                }catch (Microsoft.Data.SqlClient.SqlException)
                {
                    logger.LogError("Database error. Table or database doesn't exist!");
                    return StatusCode(404, "Database error. Table or database doesn't exist!");
                }

                logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Saved {count} persons from file.");

                return CreatedAtAction("GetPersonsCsv", personCsvList);
            }
            catch (FileNotFoundException)
            {
                logger.LogError("File podaci.csv not found!");
                return StatusCode(404, "File podaci.csv not found!");
            }

        }

        private bool PersonExistsByNumber(string number)
        {
            return _context.Persons.Any(e => e.Number == number);
        }

        private Task<List<PersonCsvDto>> loadFromCsvAsync()
        {
            string csvPath = System.Configuration.ConfigurationManager.AppSettings["csvPath"];

            List<PersonCsvDto> personCsvList = new List<PersonCsvDto>();
            using (TextFieldParser csvParser = new TextFieldParser(csvPath))
            {
                csvParser.SetDelimiters(new string[] { ";" });

                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();
                    if (Int32.TryParse(fields[2], out int postalCode))
                    {
                        personCsvList.Add(new PersonCsvDto(fields[0], fields[1], fields[2], fields[3], fields[4], true));
                    }
                    else
                    {
                        personCsvList.Add(new PersonCsvDto(fields[0], fields[1], fields[2], fields[3], fields[4], false));
                    }
                }
            }

            return Task.FromResult(personCsvList);
        }

        /*
        // *************************************** Made for testing database, not needed for production. ***************************************
        // GET: api/Person
        [HttpGet]
        public async Task<IEnumerable<PersonDto>> GetPersons(string name = null)
        {
            var persons = (await _context.Persons.ToListAsync())
                          .Select(person => person.AsPersonDto());

            if (!string.IsNullOrWhiteSpace(name))
            {
                persons = persons.Where(person => person.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {persons.Count()} persons from database.");

            return persons;
        }

        // GET: api/Person/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonDto>> GetPerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person.AsPersonDto();
        }

        // PUT: api/Person/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson(int id, PersonDto personDto)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            person.Name = personDto.Name;
            person.LastName = personDto.LastName;
            person.PostalCode = personDto.PostalCode;
            person.City = personDto.City;
            person.Number = personDto.Number;

            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExistsById(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Person
        [HttpPost]
        public async Task<ActionResult<PersonDto>> PostPerson(PersonDto personDto)
        {
            Person person = personDto.AsPersonModel();
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
        }

        // DELETE: api/Person/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonExistsById(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
        */

    }
}