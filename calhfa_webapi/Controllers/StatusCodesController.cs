using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using calhfa_webapi.Models;

namespace calhfa_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusCodesController : ControllerBase
    {
        private readonly cal_haf_dummyContext _context;

        public StatusCodesController(cal_haf_dummyContext context)
        {
            _context = context;
        }

        // GET: api/StatusCodes
        /// <summary>
        /// Grabs every entry in the StatusCodes table
        /// </summary>
        /// <returns>a list of every entry in StatusCodes</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusCode>>> GetStatusCodes()
        {
            return await _context.StatusCodes.ToListAsync();
        }

        // GET: api/StatusCodes/5
        /// <summary>
        /// Returns the record for particular loan status code. 404 error if code not found.
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>record for a particular status code in table</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusCode>> GetStatusCode(int id)
        {
            var statusCode = await _context.StatusCodes.FindAsync(id);

            if (statusCode == null)
            {
                return NotFound();
            }

            return statusCode;
        }
    }
}
