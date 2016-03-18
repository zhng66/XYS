﻿using System;
using System.Collections.Generic;

using XYS.Report.Lis.Core;
using XYS.Report.Lis.Model;
namespace XYS.Report.Lis.Handler
{
    public abstract class ReportHandlerSkeleton : IReportHandler
    {
        #region 私有字段
        private IReportHandler m_nextHandler;
        private readonly string m_handlerName;
        #endregion

        #region 构造函数
        protected ReportHandlerSkeleton(string name)
        {
            this.m_handlerName = name;
        }
        #endregion

        #region 实现IReportHandler接口
        public IReportHandler Next
        {
            get { return this.m_nextHandler; }
            set { this.m_nextHandler = value; }
        }
        public virtual string HandlerName
        {
            get { return this.m_handlerName; }
        }
        public virtual HandlerResult ReportOptions(ReportReportElement report)
        {
            if (report != null)
            {
                bool result = OperateReport(report);
                if (result)
                {
                    return HandlerResult.Continue;
                }
            }
            return HandlerResult.Fail;
        }
        #endregion

        #region 抽象方法(处理元素)
        protected abstract bool OperateReport(ReportReportElement report);
        #endregion

        #region 辅助方法
        protected bool IsExist(List<ILisReportElement> elementList)
        {
            if (elementList != null && elementList.Count > 0)
            {
                return true;
            }
            return false;
        }
        protected bool KVExsit(ReportKVElement kv)
        {
            if (kv != null && kv.Count > 0)
            {
                return true;
            }
            return false;
        }
        protected List<ReportKVElement> GetReportKVList(ReportReportElement report)
        {
            List<ReportKVElement> kvList = report.ReportKVList;
            if (kvList == null)
            {
                kvList = new List<ReportKVElement>(2);
                report.ReportKVList = kvList;
            }
            return kvList;
        }
        protected bool IsReport(Type type)
        {
            return typeof(ReportReportElement).Equals(type);
        }
        protected bool IsReport(ILisReportElement reportElement)
        {
            return reportElement is ReportReportElement;
        }
        protected bool IsElement(ILisReportElement reportElement, Type type)
        {
            return reportElement.GetType().Equals(type);
        }
        #endregion
    }
}