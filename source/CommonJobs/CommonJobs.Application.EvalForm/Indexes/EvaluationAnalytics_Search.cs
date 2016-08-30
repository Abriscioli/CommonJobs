using CommonJobs.Domain.Evaluations;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Indexes
{
    public class EvaluationAnalytics_Search : AbstractMultiMapIndexCreationTask<EvaluationAnalytics_Search.Projection>
    {
        public class CalificationState
        {
            public string UserName;
            public bool Finished;
            public double? Value;
        }

        public class Projection
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Period { get; set; }
            public string[] Evaluators { get; set; }
            public bool AutoEvaluationDone { get; set; }
            public bool ResponsibleEvaluationDone { get; set; }
            public bool CompanyEvaluationDone { get; set; }
            public bool AnyEvaluatorEvaluationDone { get; set; }
            public bool OpenToDevolution { get; set; }
            public bool Finished { get; set; }
            public List<CalificationState> CalificationsState { get; set; }
        }

        public EvaluationAnalytics_Search()
        {

            AddMap<EmployeeEvaluation>(evaluations =>
                from evaluation in evaluations
                select new
                {
                    Id = evaluation.Id,
                    UserName = evaluation.UserName,
                    Period = evaluation.Period,
                    Evaluators = new dynamic[0],
                    AutoEvaluationDone = false,
                    ResponsibleEvaluationDone = false,
                    CompanyEvaluationDone = false,
                    AnyEvaluatorEvaluationDone = false,
                    OpenToDevolution = evaluation.ReadyForDevolution,
                    Finished = evaluation.Finished,
                    CalificationsState = new dynamic[0],
                });
            AddMap<EvaluationCalification>(califications =>
                from calification in califications
                select new
                {
                    Id = calification.EvaluationId,
                    UserName = calification.EvaluatedEmployee,
                    Period = calification.Period,
                    Evaluators = (calification.Owner != CalificationType.Auto && calification.Owner != CalificationType.Company && calification.Owner != CalificationType.Responsible)
                        ? new[] { calification.EvaluatorEmployee }
                        : new dynamic[0],
                    AutoEvaluationDone = calification.Owner == CalificationType.Auto && calification.Finished,
                    ResponsibleEvaluationDone = calification.Owner == CalificationType.Responsible && calification.Finished,
                    CompanyEvaluationDone = calification.Owner == CalificationType.Company && calification.Finished,
                    AnyEvaluatorEvaluationDone = calification.Owner == CalificationType.Evaluator && calification.Finished,
                    OpenToDevolution = false,
                    Finished = false,
                    CalificationsState = (calification.Owner != CalificationType.Auto && calification.Owner != CalificationType.Company && calification.Owner != CalificationType.Responsible)
                        ? new[] { new { UserName = calification.EvaluatorEmployee, Finished = calification.Finished, Value = calification.Califications.Count > 0 ? calification.Califications.Average(x => x.Value) : null } }
                        : new dynamic[0],
                });
            Reduce = docs =>
               from doc in docs
               group doc by new { doc.UserName, doc.Period } into g
               select new
               {
                   Id = g.Where(x => x.Id != null).Select(x => x.Id).FirstOrDefault(),
                   UserName = g.Key.UserName,
                   Period = g.Key.Period,
                   Evaluators = g.SelectMany(x => x.Evaluators).Where(x => x != null).ToArray(),
                   AutoEvaluationDone = g.Any(x => x.AutoEvaluationDone),
                   ResponsibleEvaluationDone = g.Any(x => x.ResponsibleEvaluationDone),
                   CompanyEvaluationDone = g.Any(x => x.CompanyEvaluationDone),
                   AnyEvaluatorEvaluationDone = g.Any(x => x.AnyEvaluatorEvaluationDone),
                   OpenToDevolution = g.Any(x => x.OpenToDevolution),
                   Finished = g.Any(x => x.Finished),
                   CalificationsState = g.SelectMany(x => x.CalificationsState).Where(x => x != null).ToArray()
               };

            Indexes.Add(x => x.Evaluators, FieldIndexing.Analyzed);
        }
    }
}
