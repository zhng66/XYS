﻿using System;
using System.Collections.Generic;

using XYS.Common;
using XYS.Lis.Core;
using XYS.Lis.Core;

namespace XYS.Lis
{
    public interface IReport
    {
        void InitReport(ReportKey key, ReportReportElement report);
        void InitReport(LisSearchRequire require, ReportReportElement report);
        void InitReports(LisSearchRequire require, List<ILisReportElement> reportList);
        void InitReports(List<ReportKey> keyList, List<ILisReportElement> reportList);
        
        bool OperateReport(ReportReportElement report);
        bool OperateReport(List<ILisReportElement> reportList);
        
        void HandleReport(LisSearchRequire require, ReportReportElement report);
        void HandleReports(LisSearchRequire require, List<ILisReportElement> reportList);
        void HandleReports(List<ReportKey> keyList, List<ILisReportElement> reportList);
      
    }
}