﻿using CommonJobs.Application.EvalForm;
using CommonJobs.Application.EvalForm.Dtos;
using CommonJobs.Domain.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Mvc.UI.Areas.Evaluations.Models
{
    public class Calification
    {
        public UserView UserView { get; set; }

        public CalificationsEvaluationDto Evaluation { get; set; }

        public Template Template { get; set; }

        public List<EvaluationCalification> Califications { get; set; }

        public string UserLogged { get; set; }
    }
}
