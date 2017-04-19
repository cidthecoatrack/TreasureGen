﻿using RollGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TreasureGen.Domain.Selectors.Percentiles
{
    internal class TypeAndAmountPercentileSelector : ITypeAndAmountPercentileSelector
    {
        private IPercentileSelector percentileSelector;
        private Dice dice;

        public TypeAndAmountPercentileSelector(IPercentileSelector percentileSelector, Dice dice)
        {
            this.percentileSelector = percentileSelector;
            this.dice = dice;
        }

        public TypeAndAmountPercentileResult SelectFrom(string tableName)
        {
            var percentileResult = percentileSelector.SelectFrom(tableName);
            var result = ParseResult(percentileResult);

            return result;
        }

        private TypeAndAmountPercentileResult ParseResult(string percentileResult)
        {
            var result = new TypeAndAmountPercentileResult();

            if (string.IsNullOrEmpty(percentileResult))
                return result;

            if (percentileResult.Contains(",") == false)
            {
                throw new FormatException($"{percentileResult} is not formatted for type and amount parsing");
            }

            var parsedResult = percentileResult.Split(',');

            result.Type = parsedResult[0];
            result.Amount = dice.Roll(parsedResult[1]).AsSum();

            return result;
        }

        public IEnumerable<TypeAndAmountPercentileResult> SelectAllFrom(string tablename)
        {
            var percentileResults = percentileSelector.SelectAllFrom(tablename);
            var results = percentileResults.Select(r => ParseResult(r));

            return results;
        }
    }
}