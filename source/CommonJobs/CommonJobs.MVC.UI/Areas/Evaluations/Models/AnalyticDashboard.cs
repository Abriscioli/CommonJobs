using CommonJobs.Application.EvalForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Mvc.UI.Areas.Evaluations.Models
{
    public class AnalyticDashboard
    {
        public AnalyticDashboard()
        {
            Evaluations = new List<EvaluationAnalyticDto>();
        }
        public List<EvaluationAnalyticDto> Evaluations { get; set; }
    }
}
