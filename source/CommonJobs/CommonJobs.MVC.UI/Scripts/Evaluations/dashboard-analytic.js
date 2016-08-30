$(document).ready(function () {
    var evaluationStates = ['En curso', 'Esperando Eval Empleado', 'Esperando Eval Responsable', 'Esperando Eval Empresa', 'Lista para devolución', 'Abierta para devolución', 'Finalizada'];

    var AnalyticDashboard = function (data) {
        this.items = ko.observableArray();
        this.period = ko.observable("");
        this.isLoading = ko.observable(false);

        this.totalEvaluations = ko.observable("");
        this.evaluationsInProcess = ko.observable("");
        this.evaluationsWaitinEmployee = ko.observable("");
        this.evaluationsWaitingResponsible = ko.observable("");
        this.evaluationsWaitingCompany = ko.observable("");
        this.evaluationsReadyForDevolution = ko.observable("");
        this.evaluationsOpenForDevolution = ko.observable("");
        this.evaluationsFinished = ko.observable("");
        if (data) {
            this.fromJS(data);
        }
    }

    AnalyticDashboard.prototype.fromJS = function (data) {
        this.items(_.map(data, function (e) {
            return new EvaluationItem(e);
        }));
        this.totalEvaluations(this.items().length);
        var countEvaluationsByState = _.countBy(this.items(), function (e) {
            return e.stateName;
        });
        this.evaluationsInProcess(getEvaluationsByStatePercetange(countEvaluationsByState, evaluationStates[0], this.totalEvaluations()) + "%");
        this.evaluationsWaitinEmployee(getEvaluationsByStatePercetange(countEvaluationsByState, evaluationStates[1], this.totalEvaluations()) + "%");
        this.evaluationsWaitingResponsible(getEvaluationsByStatePercetange(countEvaluationsByState, evaluationStates[2], this.totalEvaluations()) + "%");
        this.evaluationsWaitingCompany(getEvaluationsByStatePercetange(countEvaluationsByState, evaluationStates[3], this.totalEvaluations()) + "%");
        this.evaluationsReadyForDevolution(getEvaluationsByStatePercetange(countEvaluationsByState, evaluationStates[4], this.totalEvaluations()) + "%");
        this.evaluationsOpenForDevolution(getEvaluationsByStatePercetange(countEvaluationsByState, evaluationStates[5], this.totalEvaluations()) + "%");
        this.evaluationsFinished(getEvaluationsByStatePercetange(countEvaluationsByState, evaluationStates[6], this.totalEvaluations()) + "%");
    };

    var EvaluationItem = function (data) {
        this.responsibleId = "";
        this.evaluators = "";
        this.state = "";
        this.stateName = "";
        this.averageCalification = "";
        this.calificationUrl = "";
        if (data) {
            this.fromJS(data)
        }
    }

    EvaluationItem.prototype.fromJS = function (data) {
        var self = data;
        this.userName = data.UserName;
        this.period = data.Period;
        this.responsibleId = data.ResponsibleId;
        this.evaluators = data.Evaluators;
        this.state = data.State;
        this.stateName = evaluationStates[data.State];
        this.averageCalification = data.AverageCalification != null ? data.AverageCalification : "-";
    }

    var viewmodel = new AnalyticDashboard();
    getAnalyticDashboard(window.ViewData.period);
    ko.applyBindings(viewmodel);

    function getAnalyticDashboard(period) {
        viewmodel.isLoading(true);
        var ajax = $.get("/Evaluations/api/GetEvaluationAnalytics",
            {
                period: period
            },
            function (model) {
                viewmodel.fromJS(model);
            },
            "json");
    };

    function getEvaluationsByStatePercetange(data, state, totalItems) {
        return data[state] != null ? (data[state] / totalItems * 100).toFixed(2) : 0
    }
});
