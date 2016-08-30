using CommonJobs.Application.EvalForm.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm
{
    public class EvaluationAnalyticDto
    {
        public string Id { get; set; }

        public EvaluationState State { get; set; }

        public List<AnalyticCalification> Califications { get; set; }
    }

     public class AnalyticCalification
    {
        public string UserName { get; set; }

        public bool Finished { get; set; }
        
        public double? Average { get; set; }
    }

}
