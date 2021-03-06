﻿using System.Linq;
using System.Threading.Tasks;
using Adapt.Analyzer.Core.Datacards.Totals.Models;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;

namespace Adapt.Analyzer.Core.Datacards.Totals.Calculators
{
    public interface IOperationTotalsCalculator
    {
        Task<OperationTotals[]> Calculate(Field field, ApplicationDataModel dataModel);
    }

    public class OperationTotalsCalculator : IOperationTotalsCalculator
    {
        private readonly IRepresentationTotalsCalculator _representationTotalsCalculator;

        public OperationTotalsCalculator()
            : this(new RepresentationTotalsCalculator())
        {
            
        }

        public OperationTotalsCalculator(IRepresentationTotalsCalculator representationTotalsCalculator)
        {
            _representationTotalsCalculator = representationTotalsCalculator;
        }

        public async Task<OperationTotals[]> Calculate(Field field, ApplicationDataModel dataModel)
        {
            var operationTotals = dataModel.GetOperationDataForField(field)
                .Select(async o => await Calculate(o));
            return await Task.WhenAll(operationTotals);
        }

        private async Task<OperationTotals> Calculate(OperationData operationData)
        {
            var numericWorkingData = operationData.GetNumericWorkingData().ToList();
            var spatialRecords = operationData.GetSpatialRecordsSafely().ToList();
            var representationTotals = await _representationTotalsCalculator.Calculate(numericWorkingData, spatialRecords);
            return new OperationTotals(operationData.OperationType, representationTotals);
        }
    }
}
