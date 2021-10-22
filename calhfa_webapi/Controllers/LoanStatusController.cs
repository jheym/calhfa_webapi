using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using calhfa_webapi.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace calhfa_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanStatusController : ControllerBase
    {
        private readonly cal_haf_dummyContext _context;

        public LoanStatusController(cal_haf_dummyContext context)
        {
            _context = context;
        }

        // GET: api/LoanStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanStatus>>> GetLoanStatuses()
        {
            return await _context.LoanStatuses.ToListAsync();
        }

        // GET: api/LoanStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanStatus>> GetLoanStatus(int id)
        {
            var loanStatus = await _context.LoanStatuses.FindAsync(id);

            if (loanStatus == null)
            {
                return NotFound();
            }

            return loanStatus;


        }

        [Route("count")]
        [HttpGet]
        public async Task<string> GetLoanCountAsync()
        {
            var ComplianceReview = await _context.LoanStatuses.Where(l => l.StatusCode == 110 ||
                                                                                    l.StatusCode == 210 ||
                                                                                    l.StatusCode == 310 ||
                                                                                    l.StatusCode == 410).ToListAsync();
            var ComplianceSuspenseReview = await _context.LoanStatuses.Where(l => l.StatusCode == 422).ToListAsync();
            var PurchaseReview = await _context.LoanStatuses.Where(l => l.StatusCode == 510 ||
                                                                                    l.StatusCode == 810).ToListAsync();
            var PurchaseSuspenseReview = await _context.LoanStatuses.Where(l => l.StatusCode == 522).ToListAsync();

            int complianceReviewCount = ComplianceReview.Count;
            int complianceSuspenseReviewCount = ComplianceSuspenseReview.Count;
            int purchaseReviewCount = PurchaseReview.Count;
            int purchaseSuspenseReviewCount = PurchaseSuspenseReview.Count;

            Console.WriteLine(complianceReviewCount);

            string jsonData = String.Format("{{compliantReviewCount: '{0}', compliantSuspenseCount: '{1}', purchaseReviewCount: '{2}', purchaseSuspenseCount: '{3}'}}",
                complianceReviewCount, complianceSuspenseReviewCount, purchaseReviewCount, purchaseSuspenseReviewCount);

            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
        }
    }
}
