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
    public class CountController : Controller
    {
        private readonly DBContext _context;

        public CountController(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Counts the total number of loans which are queued for review pre and post closing
        /// Also finds the oldest date for each queue category
        /// </summary>
        /// <returns> a json formatted string which contains the counts and dates for the review categories </returns>
        // GET: /api/LoanStatus/count
        [HttpGet]
        public string GetLoanCount()
        {
            // codes ending with '10' are in review, codes ending in '22' are suspended & being reviewed after a resubmit
            var ComplianceQueueList = GetQueueList(410, 1);
            var ComplianceReviewDate = GetReviewDate(ComplianceQueueList);

            var ComplianceSuspenseQueueList = GetQueueList(422, 1);
            var ComplianceSuspenseDate = GetReviewDate(ComplianceSuspenseQueueList);

            var PurchaseQueueList = GetQueueList(510, 2);
            var PurchaseReviewDate = GetReviewDate(PurchaseQueueList);

            var PurchaseSuspenseQueueList = GetQueueList(522, 2);
            var PurchaseSuspenseDate = GetReviewDate(PurchaseSuspenseQueueList);

            string DateFormatting = "MMM d";
            string jsonData = String.Format("{{compliantQueue: {{count: '{0}', date: '{1}'}}, " +
                "compliantSuspenseQueue: {{ count: '{2}', date: '{3}' }}, " +
                "purchaseQueue: {{ count: '{4}', date: '{5}' }}, " +
                "purchaseSuspenseQueue: '{{ count: '{6}', date: '{7}' }} }}", ComplianceQueueList.Count,
                ComplianceReviewDate.ToString(DateFormatting),
                ComplianceSuspenseQueueList.Count,
                ComplianceSuspenseDate.ToString(DateFormatting),
                PurchaseQueueList.Count,
                PurchaseReviewDate.ToString(DateFormatting),
                PurchaseSuspenseQueueList.Count,
                PurchaseSuspenseDate.ToString(DateFormatting));

            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
        }

        /// <summary>
        /// Queries the database for the loans which match the specified statuscode and categoryID.
        /// The query is stored as a list with columns matching LoanID, LoanCategoryID, StatusCode, and StatusDate
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

        private DateTime GetReviewDate(List<ReviewQueue> list)
        {
            var reviewDate = DateTime.Now;

            if (list.Count != 0)
            {
                foreach (var loan in list)
                {
                    reviewDate = loan.StatusDate.Date;
                    // returns oldest (first in queue)
                    if (loan.StatusDate.Date < reviewDate)
                    {
                        reviewDate = loan.StatusDate;
                    }
                }
            }

            return reviewDate;
        }
    }
}
