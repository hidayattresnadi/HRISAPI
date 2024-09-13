using HRISAPI.Application.DTO.LeaveRequest;
using HRISAPI.Application.IServices;
using HRISAPI.Application.Repositories;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace HRISAPI.Application.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<byte[]> GenerateLeaveRequestsPDF(LeaveRequestDTOFiltered request)
        {
            var leaveRequests = await _leaveRequestRepository.GetGroupedLeaveRequests(request);
            string Name = $"{request.StartDate} - {request.EndDate}";

            string htmlContent = $"<h1>Report of Leave Requests in period: {Name}</h1>";
            htmlContent += "<table>";
            htmlContent += "<tr>" +
                "<th>Leave Type</th>" +
                "<th>Total Leaves</th>" +
                "</tr>";

            foreach (var leaveRequest in leaveRequests)
            {
                htmlContent += $"<tr>" +
                               $"<td>{leaveRequest.LeaveType}</td>" +
                               $"<td>{leaveRequest.TotalLeaves}</td>" +
                               $"</tr>";
            }

            htmlContent += "</table>";

            var document = new PdfDocument();
            var config = new PdfGenerateConfig
            {
                PageOrientation = PageOrientation.Landscape,
                PageSize = PageSize.A4
            };

            string cssStr = File.ReadAllText(@"./Templates/PDFReportTemplate/style.css");
            CssData css = PdfGenerator.ParseStyleSheet(cssStr);
            PdfGenerator.AddPdfPages(document, htmlContent, config, css);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, false);

            byte[] bytes = stream.ToArray();

            return bytes;
        }
        public async Task<IEnumerable<LeaveRequestGroupDTO>> GetLeavesType(LeaveRequestDTOFiltered request)
        {
            var leaveRequests = await _leaveRequestRepository.GetGroupedLeaveRequests(request);
            return leaveRequests;
        }
    }
}
