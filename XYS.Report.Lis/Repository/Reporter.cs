﻿using System;
using System.Collections;
using System.Collections.Generic;

using XYS.Report.Lis.Core;
using XYS.Report.Lis.Model;
namespace XYS.Report.Lis.Repository
{
    public abstract class Reporter : ILisReporter
    {
        #region 变量
        private readonly string m_callerName;
        private readonly string m_strategyName;
        private Hierarchy m_hierarchy;

        private IReportFiller m_defaultFill;
        private IReportFiller m_filler;

        private IReportHandler m_headHandler;
        private IReportHandler m_tailHandler;
        #endregion

        #region 构造函数
        protected Reporter(string callerName, string strategyName)
        {
            this.m_callerName = callerName;
            this.m_strategyName = strategyName;
        }
        #endregion

        #region 实例属性
        public virtual IReportFiller DefaultFill
        {
            get
            {
                if (this.m_defaultFill == null)
                {
                    this.InitDefault();
                }
                return this.m_defaultFill;
            }
        }
        public virtual IReportFiller Filler
        {
            get
            {
                if (this.m_filler == null)
                {
                    return this.DefaultFill;
                }
                else
                {
                    return this.m_filler;
                }
            }
            set { this.m_filler = value; }
        }
        public virtual Hierarchy Hierarchy
        {
            get { return this.m_hierarchy; }
            set { this.m_hierarchy = value; }
        }
        public virtual IReportHandler HandlerHead
        {
            get { return this.m_headHandler; }
        }
        #endregion

        #region 实现IReport接口
        public IReporterRepository Repository
        {
            get { return this.m_hierarchy; }
        }
        public void FillReport(ILisReportElement report, LisReportPK RK)
        {
            if (IsLisReport(report))
            {
                this.Filler.Fill((ReportReportElement)report, RK);
            }
        }
        public bool OptionReport(ILisReportElement report)
        {
            if (IsLisReport(report))
            {
                return this.HandlerEvent((ReportReportElement)report);
            }
            return false;
        }
        #endregion

        #region 实现ILisReporter接口
        public bool IsLisReport(ILisReportElement report)
        {
            ReportReportElement rre = report as ReportReportElement;
            if (rre != null)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 受保护的虚方法
        protected virtual bool HandlerEvent(ReportReportElement element)
        {
            IReportHandler handler = this.HandlerHead;
            while (handler != null)
            {
                switch (handler.ReportOptions(element))
                {
                    case HandlerResult.Fail:
                        return false;
                    case HandlerResult.HasErrorButContinue:
                        handler = handler.Next;
                        break;
                    case HandlerResult.Continue:
                        handler = handler.Next;
                        break;
                    case HandlerResult.Success:
                        handler = null;
                        break;
                    default:
                        return true;
                }
            }
            return true;
        }
        protected virtual void AddHandler(Hashtable handlerTable, IList<string> handlerNameList)
        {
            IReportHandler handler;
            //清空
            this.m_headHandler = this.m_tailHandler = null;
            foreach (string name in handlerNameList)
            {
                handler = handlerTable[name] as IReportHandler;
                if (handler != null)
                {
                    AddHandler(handler);
                }
            }
        }
        protected virtual void AddHandler(IReportHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("filter param must not be null");
            }
            if (this.m_headHandler == null)
            {
                this.m_headHandler = this.m_tailHandler = handler;
            }
            else
            {
                this.m_tailHandler.Next = handler;
                this.m_tailHandler = handler;
            }
        }
        protected virtual void SetFiller(Hashtable fillerTable, string fillerName)
        {
            IReportFiller filler = fillerTable[fillerName] as IReportFiller;
            if (filler != null)
            {
                this.Filler = filler;
            }
        }
        #endregion

        #region 私有方法
        private void InitDefault()
        {
            if (this.Hierarchy == null)
            {
                throw new ArgumentNullException("Hierarchy");
            }
            if (this.m_defaultFill == null)
            {
                this.m_defaultFill = this.Hierarchy.DefaultFiller;
            }
        }
        #endregion

        #region 初始化
        public virtual void InitReporter()
        {
            if (this.Hierarchy == null)
            {
                throw new ArgumentNullException("Hierarchy");
            }
            ReporterStrategy stratrgy = this.Hierarchy.StrategyMap[this.m_strategyName] as ReporterStrategy;
            if (stratrgy == null)
            {
                throw new ArgumentNullException("can not find the stratrgy [" + this.m_strategyName + "]");
            }
            SetFiller(this.Hierarchy.FillerMap, stratrgy.FillerName);
            AddHandler(this.Hierarchy.HandlerMap, stratrgy.HandlerList);
        }
        public virtual void Reset()
        {
            lock (this)
            {
                InitReporter();
            }
        }
        #endregion
    }
}