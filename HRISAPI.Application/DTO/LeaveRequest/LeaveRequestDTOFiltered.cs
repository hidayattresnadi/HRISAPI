using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRISAPI.Application.DTO.LeaveRequest
{
    public class LeaveRequestDTOFiltered
    {
        public DateOnly StartDate {  get; set; }
        public DateOnly EndDate { get; set; }
    }
}
