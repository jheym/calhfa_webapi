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
    public class LoanTypesController : ControllerBase
    {
        private readonly cal_haf_Context _context;

        public LoanTypesController(cal_haf_Context context)
        {
            _context = context;
        }

        // GET: api/LoanTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanType>>> GetLoanTypes()
        {
            return await _context.LoanTypes.ToListAsync();
        }

        // GET: api/LoanTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanType>> GetLoanType(int id)
        {
            var loanType = await _context.LoanTypes.FindAsync(id);

            if (loanType == null)
            {
                return NotFound();
            }

            return loanType;
        }
    }
}
