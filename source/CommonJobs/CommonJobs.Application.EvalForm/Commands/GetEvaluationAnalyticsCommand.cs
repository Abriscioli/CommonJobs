using CommonJobs.Application.EvalForm.Helper;
using CommonJobs.Application.EvalForm.Indexes;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class GetEvaluationAnalyticsCommand : Command<List<EvaluationAnalyticDto>>
    {
        private string _period;
        private string _loggedUeer;

        public GetEvaluationAnalyticsCommand(string period)
        {
            _period = period;
        }

        public override List<EvaluationAnalyticDto> ExecuteWithResult()
        {
            IQueryable<EvaluationAnalytics_Search.Projection> querey = RavenSession
                .Query<EvaluationAnalytics_Search.Projection, EvaluationAnalytics_Search>()
                .Where(e => e.Period == _period);

            var evaluationAnalyticsProjection = querey.ToArray();
            var evaluationAnalitics = evaluationAnalyticsProjection.Select(x => new EvaluationAnalyticDto
            {
                Id = x.Id,
                State = EvaluationStateHelper.GetEvaluationState(x.AutoEvaluationDone, x.ResponsibleEvaluationDone, x.CompanyEvaluationDone, x.OpenToDevolution, x.Finished),
                Califications = x.CalificationsState.Select(y => new AnalyticCalification
                {
                    UserName = y.UserName,
                    Finished = y.Finished,
                    Average = y.Value
                }).ToList()
            }).ToList();
            return evaluationAnalitics;
         }
    }
}
