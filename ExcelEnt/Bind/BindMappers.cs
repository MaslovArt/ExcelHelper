﻿using ExcelEnt.Extentions;
using NPOI.SS.UserModel;
using System;

namespace ExcelEnt.Bind
{
    /// <summary>
    /// Excel cell value to property value mappers
    /// </summary>
    public static class BindMappers
    {
        /// <summary>
        /// Get int value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <returns></returns>
        public static object Int(ICell cell) => 
            (int)cell.NumericCellValue;

        /// <summary>
        /// Get nullable int value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <returns></returns>
        public static object NullInt(ICell cell) =>
            cell == null ? null : (int?)cell.NumericCellValue;

        /// <summary>
        /// Get double value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <returns></returns>
        public static object Double(ICell cell) => 
            cell.NumericCellValue;

        /// <summary>
        /// Get nullable double value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <returns></returns>
        public static object NullDouble(ICell cell) =>
            cell == null ? null : (double?)cell.NumericCellValue;

        /// <summary>
        /// Get string value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <returns></returns>
        public static string String(ICell cell) => 
            cell.ToString();

        /// <summary>
        /// Get date value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <returns></returns>
        public static object Date(ICell cell) => 
            cell.DateCellValue;

        /// <summary>
        /// Get nullable date value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <returns></returns>
        public static object NullDate(ICell cell) =>
            cell == null ? null : (DateTime?)cell.DateCellValue;

        /// <summary>
        /// Get bool value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <returns></returns>
        public static object Bool(ICell cell) => 
            cell.BooleanCellValue;

        /// <summary>
        /// Get nullable bool value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <returns></returns>
        public static object NullBool(ICell cell) =>
            cell == null ? null : (bool?)cell.BooleanCellValue;

        /// <summary>
        /// Get bool value by true string value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <param name="trueValue">Excel cell value for true</param>
        /// <returns></returns>
        public static object StringBool(ICell cell, string trueValue) => 
            cell.ToString() == trueValue;

        /// <summary>
        /// Get enum value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <returns></returns>
        public static object Enum<Enum>(ICell cell) => 
            cell.ToString().ToEnum<Enum>();

        /// <summary>
        /// Get nullable enum value
        /// </summary>
        /// <param name="cell">Excell cell</param>
        /// <returns></returns>
        public static object NullEnum<Enum>(ICell cell) where Enum : struct =>
            cell == null ? null : (Enum?)cell.ToString().ToEnum<Enum>();
    }
}