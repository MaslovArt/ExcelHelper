﻿using ExcelEnt.Extentions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExcelEnt.Write
{
    /// <summary>
    /// Entities to excel writer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XLSXWriter<T>
    {
        private List<WriteRule>                     _rules;
        private XLSXStyling<T>                      _styling;
        private XLSXTemplating<T>                   _templating;
        private List<Action<XSSFWorkbook, ISheet>>  _modifications;
        private string[]                            _columnsTitles;

        public XLSXWriter()
        {
            _rules = new List<WriteRule>();
            _modifications = new List<Action<XSSFWorkbook, ISheet>>();
        }

        private int InsertIndex
        {
            get
            {
                if (_templating != null) 
                    return _templating.InsertInd;
                if (_columnsTitles != null && _columnsTitles.Length > 0) 
                    return 1;

                return 0;
            }
        }

        /// <summary>
        /// Add entity property to excel cell value rule
        /// </summary>
        /// <param name="propName">Entity property name</param>
        /// <param name="colIndex">Column index</param>
        /// <returns></returns>
        public XLSXWriter<T> AddRule(Expression<Func<T, object>> propName, int colIndex)
        {
            var prop = TypeExtentions.GetProperty(propName);
            _rules.Add(new WriteRule(colIndex, prop));

            return this;
        }
        
        /// <summary>
        /// Create excel from template
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public XLSXWriter<T> UseTemplating(Action<IXLSXTemplating<T>> config)
        {
            _templating = _templating ?? new XLSXTemplating<T>();
            config(_templating);

            return this;
        }

        /// <summary>
        /// Add styles
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public XLSXWriter<T> UseStyling(Action<IXLSXStyling<T>> config)
        {
            _styling = _styling ?? new XLSXStyling<T>();
            config(_styling);

            return this;
        }

        public XLSXWriter<T> AddColumnsTitles(params string[] titles)
        {
            _columnsTitles = titles;

            return this;
        }

        /// <summary>
        /// Add modifications to final workbook
        /// </summary>
        /// <param name="modification"></param>
        /// <returns></returns>
        public XLSXWriter<T> Modify(Action<XSSFWorkbook, ISheet> modification)
        {
            _modifications.Add(modification);

            return this;
        }

        /// <summary>
        /// Generate excel from entities
        /// </summary>
        /// <param name="resultFilePath"></param>
        /// <param name="entities"></param>
        public XSSFWorkbook Generate(T[] entities)
        {
            var workbook = CreateWorkbook(entities.Length);
            var sheet = workbook.GetSheetAt(0);

            _styling?.Build(workbook);
            WriteEntities(sheet, entities, InsertIndex);
            _styling?.ApplyConditionRowStyles(sheet, entities, InsertIndex);
            ApplyModifications(workbook, sheet);

            return workbook;
        }

        private XSSFWorkbook CreateWorkbook(int entitiesCount)
        {
            XSSFWorkbook workbook = null;
            if (_columnsTitles != null && _columnsTitles.Length > 0)
            {
                workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet();
                var row = sheet.CreateRow(0);
                var startColInd = _rules.Select(r => r.ExcelColInd).Min();

                for (int i = 0; i < _columnsTitles.Length; i++)
                    row.CreateCell(startColInd + i).SetCellValue(_columnsTitles[i]);
            }
            else if (_templating != null)
            {
                workbook = _templating.CreateWorkbook(entitiesCount);
            }
            else
            {
                workbook = new XSSFWorkbook();
                workbook.CreateSheet();
            }

            return workbook;
        }

        private void ApplyModifications(XSSFWorkbook workbook, ISheet sheet)
        {
            foreach (var modification in _modifications)
                modification(workbook, sheet);
        }

        private void WriteEntities(ISheet sheet, T[] entities, int insertIndex)
        {
            var newRowInd = insertIndex;
            var minColIndex = _rules.Select(r => r.ExcelColInd).Min();
            var maxColIndex = _rules.Select(r => r.ExcelColInd).Max();

            foreach (var model in entities)
            {
                var row = sheet.CreateRow(newRowInd++);
                for (var colInd = minColIndex; colInd <= maxColIndex; colInd++)
                    CreateStyledCell(row, colInd);

                foreach (var rule in _rules)
                {
                    var value = rule.Prop.GetValue(model);
                    var newCell = row.GetCell(rule.ExcelColInd);

                    if (value == null)
                        newCell.SetCellValue("");
                    else if (value is string strValue)
                        newCell.SetCellValue(strValue);
                    else if (value is DateTime dateValue)
                        newCell.SetCellValue(dateValue);
                    else if (value is bool boolValue)
                        newCell.SetCellValue(boolValue);
                    else if (double.TryParse(value.ToString(), out double numValue))
                        newCell.SetCellValue(numValue);
                    else if (value is Enum enumValue)
                        newCell.SetCellValue(enumValue.ToDescription());
                    else
                        newCell.SetCellValue(value.ToString());
                }
            }
        }

        private ICell CreateStyledCell(IRow row, int cellIndex)
        {
            var newCell = row.CreateCell(cellIndex);
            _styling?.SetStyle(newCell);

            return newCell;
        }
    }
}
