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
        private readonly cal_haf_Context _context;

        public LoanStatusController(cal_haf_Context context)
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
        /// <summary>
        /// Returns the loan with the specififed LoanStatusId. Returns 404 if not found.
        /// </summary>
        /// <param name="id"> int </param>
        /// <returns> loanStatus </returns>
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

        /// <summary>
        /// Counts the total number of loans which are being reviwed for either compliance or for purchase.
        /// In these categories, it also counts which loans are being reviewed after suspension.
        /// Pulls data from the LoanStatus table for counting
        /// </summary>
        /// <returns> a json formatted string which contains the counts for the review categories </returns>
        // GET: /api/LoanStatus/count
        [Route("count")]
        [HttpGet]
        public string GetLoanCount()
        {
            // codes ending with '10' are in review, codes ending in '22' are suspended & being reviewed after a resubmit
            var ComplianceReviewList = GetQueueList(410, 1);
            //var ComplianceReviewDate = GetReviewDate(ComplianceReviewList);

            var ComplianceSuspenseReviewList = GetQueueList(422, 1);
            //var ComplianceSuspenseDate = GetReviewDate(ComplianceSuspenseReviewList);

            var PurchaseReviewList = GetQueueList(510, 2);
            //var PurchaseReviewDate = GetReviewDate(PurchaseReviewList);

            var PurchaseSuspenseReviewList = GetQueueList(522, 2);
            //var PurchaseSuspenseDate = GetReviewDate(PurchaseSuspenseReviewList);

            string jsonData = String.Format("{{compliantReview: {{count: '{0}', date: '{1}'}}, compliantSuspense: {{ count: '{2}', date: '{3}' }}, purchaseReview: {{ count: '{4}', date: '{5}' }}, purchaseSuspense: '{{ count: '{6}', date: '{7}' }} }}",
                ComplianceReviewList.Count, 0, ComplianceSuspenseReviewList.Count, 0, PurchaseReviewList.Count, 0, PurchaseSuspenseReviewList.Count, 0);

            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
        }

        /// <summary>
        /// takes a specific status code and counts how many loans are still at that specific stage of review
        /// </summary>
        /// <param name="statusCode">int</param>
        /// <param name="categoryID">int</param>
        /// <returns>list with all loans in specified queue</returns>
        private List<ReviewQueue> GetQueueList(int statusCode, int categoryID)
        {
            string SQLQuery = @"SELECT Loan.LoanID, LoanType.LoanCategoryID, StatusCode, LoanStatus.StatusDate
                                FROM Loan
                                INNER JOIN(
                                    SELECT LoanStatus.LoanID, LoanStatus.StatusCode, LoanStatus.StatusSequence, LoanStatus.StatusDate
                                    FROM LoanStatus
                                    INNER JOIN (
                                        SELECT LoanStatus.LoanID, MAX(LoanStatus.StatusSequence) AS StatusSequence
                                        FROM LoanStatus
                                        GROUP BY LoanID
                                    ) MaxTable ON LoanStatus.LoanID = MaxTable.LoanID AND LoanStatus.StatusSequence = MaxTable.StatusSequence
                                ) LoanStatus ON Loan.LoanID = LoanStatus.LoanID
                                INNER Join(
                                    SELECT LoanType.LoanCategoryID, LoanType.LoanTypeID
                                    FROM LoanType
                                    WHERE LoanType.LoanCategoryID = {0}
                                ) LoanType ON LoanType.LoanTypeID = Loan.LoanTypeID
                                WHERE StatusCode = {1}
                                ORDER BY Loan.LoanID";
            var queuedLoans = _context.ReviewQueue.FromSqlRaw(SQLQuery, categoryID, statusCode).ToList();
            
            return queuedLoans;
        }

        /**
        private DateTime GetReviewDate(List<Loan> list)
        {
            var reviewDate = list[0].StatusDate;
            foreach (var loan in list)
            {
                //NOTE this can change depending on whether the latest or oldest date is needed
                if (loan.StatusDate.HasValue && loan.StatusDate > reviewDate)
                {
                    reviewDate = loan.StatusDate;
                }
            }

            return (DateTime)reviewDate;
        }
        **/
    }
}
