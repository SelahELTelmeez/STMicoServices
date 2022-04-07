using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionInfrastructure.Features.Tracker.DTO.Command
{
    public class SubjectDetailsResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
