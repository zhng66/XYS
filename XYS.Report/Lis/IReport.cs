﻿using System;
using System.Collections.Generic;

using XYS.Report.Lis.Model;
namespace XYS.Report.Lis
{
    public interface IReport
    {
        void Operate(ReportReportElement report);
        void Operate(List<ReportReportElement> reportList);
        void OperateAsync(ReportReportElement report, Action<ReportReportElement> callbak = null);
        
        List<ReportReportElement> GetReports(Require req);
        List<ReportReportElement> GetReports(string where);
        
        List<LisReportPK> GetReportPK(Require req);
        List<LisReportPK> GetReportPK(string where);
    }
}