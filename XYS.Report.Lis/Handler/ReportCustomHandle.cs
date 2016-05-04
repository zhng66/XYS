﻿using System;
using System.Collections.Generic;

using XYS.Report.Lis.Model;
namespace XYS.Report.Lis.Handler
{
    public class ReportCustomHandle : ReportHandleSkeleton
    {
        #region 构造函数
        public ReportCustomHandle()
            : base()
        {
        }
        #endregion

        #region 实现父类抽象方法
        protected override void OperateReport(ReportReportElement report)
        {
            LOG.Info("报告自定义项处理");
            ReportCustomElement rce = null;
            List<IFillElement> customList = report.GetReportItem(typeof(ReportCustomElement));
            if (IsExist(customList))
            {
                foreach (ISubElement custom in customList)
                {
                    rce = custom as ReportCustomElement;
                    if (rce != null)
                    {
                        //custom项处理
                    }
                }
            }
            this.SetHandlerResult(report.HandleResult, 60, "there is no ReportCustomElement to handle and continue!");
        }
        #endregion

        #region custom内部处理逻辑
        #endregion
    }
}
