﻿using System;
using System.Collections.Generic;

using XYS.Common;
using XYS.Lis.Repository;
namespace XYS.Lis.Core
{
    public interface IReporter
    {
        //string CallerName { get; }
        //string StrategyName { get; }
        IReporterRepository Repository { get; }
        
        //报告元素填充
        #region
        void FillReportElement(IReportElement reportElement, ReportKey key);
        void FillReportElement(List<IReportElement> reportElementList, ReportKey key,Type type);
        #endregion
        
        //报告元素处理
        #region
        bool Option(IReportElement reportElement);
        bool Option(List<IReportElement> reportElementList, Type type);
        #endregion
    }
}