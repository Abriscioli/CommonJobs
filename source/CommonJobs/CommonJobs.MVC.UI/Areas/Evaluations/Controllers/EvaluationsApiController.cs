﻿using CommonJobs.Application.EvalForm;
using CommonJobs.Application.EvalForm.Commands;
using CommonJobs.Application.EvalForm.Dtos;
using CommonJobs.Application.Evaluations;
using CommonJobs.Domain;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Mvc.UI.Areas.Evaluations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Application.EvalForm.Helper;

namespace CommonJobs.Mvc.UI.Areas.Evaluations.Controllers
{
    public class EvaluationsApiController : CommonJobsController
    {
        private string DetectUser()
        {
            //TODO: remove hardcoded "CS\\"
            //TODO: move to an AuthorizeAttribute or something more elegant
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated && User.Identity.Name != null)
            {
                var user = User.Identity.Name;
                if (user.StartsWith("CS\\"))
                    user = user.Substring(3);
                return user;
            }
            else
            {
                throw new ApplicationException("User cannot be detected");
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [CommonJobsAuthorize(Roles = "EmployeeManagers")]
        public JsonNetResult GetEmployeesToGenerateEvalution(string period)
        {
            PeriodCreation periodCreation = new PeriodCreation();
            periodCreation.Employees = ExecuteCommand(new GetEmployeesForEvaluationCommand(period));
            return Json(periodCreation);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CommonJobsAuthorize(Roles = "EmployeeManagers")]
        public JsonNetResult GenerateEvalutions(PeriodCreation model)
        {
            ExecuteCommand(new GenerateEvaluationsCommand(model.Employees, model.Period));
            return Json(model.Employees.Count);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonNetResult GetDashboardEvaluationsForEmployeeManagers(string period)
        {
            PeriodEvaluation periodEvaluation = new PeriodEvaluation();
            var command = new GetEvaluatedEmployees();
            if (period != null)
            {
                command.Period = period;
            }
            periodEvaluation.Evaluations = ExecuteCommand(command);
            return Json(periodEvaluation);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonNetResult GetDashboardEvaluations(string period)
        {
            PeriodEvaluation periodEvaluation = new PeriodEvaluation();
            periodEvaluation.Evaluations = ExecuteCommand(new GetEvaluatorEmployeesCommand(DetectUser(), period));
            return Json(periodEvaluation);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonNetResult GetCalificatorsForEvaluation(string username, string period)
        {
            PeriodEvaluation periodEvaluation = new PeriodEvaluation();
            periodEvaluation.Evaluations = ExecuteCommand(new GetEvaluatorEmployeesCommand(username, period));
            return Json(periodEvaluation);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonNetResult UpdateCalificators(EmployeeEvaluationDTO evaluation, List<EvaluatorsUpdateDto> calificators)
        {
            ExecuteCommand(new UpdateEvaluatorsCommand(evaluation, calificators));
            return Json("ok");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonNetResult GetEvaluation(string username, string period)
        {
            var loggedUser = DetectUser();

            var sessionRoles = ExecuteCommand(new GetLoggedUserRoles(loggedUser));

            var required = new List<string>() { "EmployeeManagers" };
            var isManager = sessionRoles.Intersect(required).Any();

            Calification calification = new Calification();
            CalificationsDto calificationsDTO = ExecuteCommand(new GetEvaluationCalifications(period, username, DetectUser(), isManager));
            calification.UserView = calificationsDTO.View;
            calification.Evaluation = calificationsDTO.Evaluation;
            calification.Califications = calificationsDTO.Califications;
            calification.UserLogged = DetectUser();
            calification.Template = ExecuteCommand(new GetEvaluationTemplateCommand());
            return Json(calification);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonNetResult SaveEvaluationCalifications(UpdateEvaluationDto updateEvaluationDto)
        {
            ExecuteCommand(new UpdateCalificationsCommand(updateEvaluationDto, DetectUser()));
            return Json("ok");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonNetResult StartDevolution(string evaluationId)
        {
            ExecuteCommand(new StartDevolutionCommand(evaluationId, DetectUser()));
            return Json("OK");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CommonJobsAuthorize(Roles = "EvaluationManagers")]
        public JsonNetResult DeleteEmployeeEvalution(string period, string userName)
        {
            ExecuteCommand(new DeleteEvaluationCommand(period, userName));
            return Json("OK");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CommonJobsAuthorize(Roles = "EvaluationManagers")]
        public JsonNetResult DeletePeriod(string period)
        {
            ExecuteCommand(new DeleteEvaluationCommand(period));
            return Json("OK");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CommonJobsAuthorize(Roles = "EvaluationManagers")]
        public JsonNetResult ChangeResponsible(string period, string username, string newResponsibleName)
        {
            ExecuteCommand(new ChangeResponsibleCommand(username, period, newResponsibleName));
            return Json("OK");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CommonJobsAuthorize(Roles = "EmployeeManagers")]
        public JsonNetResult RevertEvaluationState(string period, string username, string operation)
        {
            ExecuteCommand(new RevertEvaluationStateCommand(period, username, operation));
            return Json("OK");
        }
    }
}
