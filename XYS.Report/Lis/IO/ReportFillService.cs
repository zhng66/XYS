﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using XYS.Util;
using XYS.Common;
using XYS.Report.Lis.Util;
using XYS.Report.Lis.Model;
using XYS.Report.Lis.IO.SQLServer;
namespace XYS.Report.Lis.IO
{
    public delegate HandleResult FillReportComplete(ReportReportElement report);
    public class ReportFillService
    {
        #region 字段
        private LisReportCommonDAL m_reportDAL;
        private static readonly Hashtable Section2FillTypeMap;
        #endregion

        #region 构造函数
        static ReportFillService()
        {
            Section2FillTypeMap = new Hashtable(20);
            ReportFillService.InitFillElementTable();
        }
        public ReportFillService()
        {
            this.m_reportDAL = new LisReportCommonDAL();
        }
        #endregion

        #region 实例属性
        public virtual LisReportCommonDAL ReportDAL
        {
            get
            {
                if (this.m_reportDAL == null)
                {
                    this.m_reportDAL = new LisReportCommonDAL();
                }
                return this.m_reportDAL;
            }
            set { this.m_reportDAL = value; }
        }
        #endregion

        #region 同步方法
        public void FillReport(ReportReportElement report)
        {
            LisReportPK PK = report.LisPK;
            //填充报告元素
            FillReportElement(report, PK, report.HandleResult);
            if (report.HandleResult.ResultCode == -1)
            {
                return;
            }
            //填充子项
            List<Type> availableElementList = GetAvailableFillElements(PK);
            if (availableElementList != null && availableElementList.Count > 0)
            {
                List<AbstractFillElement> tempList = null;
                foreach (Type type in availableElementList)
                {
                    tempList = report.GetReportItem(type);
                    FillSubElements(tempList, PK, type, report.HandleResult);
                    if (report.HandleResult.ResultCode == -1)
                    {
                        return;
                    }
                }
            }
            this.SetHandlerResult(report.HandleResult, 2, "fill report successfully");
        }
        #endregion

        #region 异步方法
        public async Task FillReportAsync(ReportReportElement report, Func<ReportReportElement, Task> callback = null)
        {
            if (report.HandleResult.ResultCode != -1)
            {
                await FillReportTask(report);
            }
            if (callback != null)
            {
                await callback(report);
            }
        }
        private Task FillReportTask(ReportReportElement report)
        {
            return Task.Run(() =>
            {
                this.Fill(report);
            });
        }
        private void Fill(ReportReportElement report)
        {
            string sql = null;
            LisReportCommonDAL lisDAL = new LisReportCommonDAL();
            //生成sql 字符串
            sql = GenderSql(report, report.LisPK);
            //尝试填充数据
            try
            {
                lisDAL.Fill(report, sql);
                this.SetHandlerResult(report.HandleResult, 1, "fill ReportReportElement successfully and continue!");
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("fill ReportReportElement failed! error message:");
                sb.Append(ex.Message);
                sb.Append(SystemInfo.NewLine);
                sb.Append(ex.ToString());
                this.SetHandlerResult(report.HandleResult, -1, this.GetType(), sb.ToString());
                return;
            }
            //填充子项
            List<Type> availableElementList = GetAvailableFillElements(report.LisPK);
            if (availableElementList != null && availableElementList.Count > 0)
            {
                List<AbstractFillElement> tempList = null;
                foreach (Type type in availableElementList)
                {
                    tempList = report.GetReportItem(type);
                    sql = GenderSql(type, report.LisPK);
                    try
                    {
                        this.ReportDAL.FillList(tempList, type, sql);
                        this.SetHandlerResult(report.HandleResult, 1, "fill SubReportElements successfully and continue!");
                    }
                    catch (Exception ex)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("fill SubReportElements failed! error message:");
                        sb.Append(ex.Message);
                        sb.Append(SystemInfo.NewLine);
                        sb.Append(ex.ToString());
                        this.SetHandlerResult(report.HandleResult, -1, this.GetType(), sb.ToString());
                        return;
                    }
                }
            }
            this.SetHandlerResult(report.HandleResult, 2, "fill report successfully");
        }
        #endregion

        #region 填充数据
        private void FillReportElement(AbstractFillElement report, LisReportPK PK, HandleResult result)
        {
            string sql = GenderSql(report, PK);
            try
            {
                this.ReportDAL.Fill(report, sql);
                this.SetHandlerResult(result, 1, "fill report successfully and continue!");
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("fill ReportReportElement failed! error message:");
                sb.Append(ex.Message);
                sb.Append(SystemInfo.NewLine);
                sb.Append(ex.ToString());
                this.SetHandlerResult(result, -1, this.GetType(), sb.ToString());
            }
        }
        private void FillSubElements(List<AbstractFillElement> subElementList, LisReportPK PK, Type type, HandleResult result)
        {
            string sql = GenderSql(type, PK);
            try
            {
                this.ReportDAL.FillList(subElementList, type, sql);
                this.SetHandlerResult(result, 1, "fill subelements successfully and continue!");
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("fill SubReportElements failed! error message:");
                sb.Append(ex.Message);
                sb.Append(SystemInfo.NewLine);
                sb.Append(ex.ToString());
                this.SetHandlerResult(result, -1, this.GetType(), sb.ToString());
            }
        }
        #endregion

        #region 生成sql语句
        protected string GenderSql(AbstractFillElement element, LisReportPK PK)
        {
            return GenderSql(element.GetType(), PK);
        }
        protected string GenderSql(Type type, LisReportPK PK)
        {
            return GenderPreSQL(type) + GenderWhere(PK);
        }
        protected string GenderPreSQL(Type type)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (IsColumn(prop))
                {
                    sb.Append(prop.Name);
                    sb.Append(',');
                }
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" from ");
            sb.Append(type.Name);
            return sb.ToString();
        }
        protected string GenderWhere(LisReportPK PK)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" where");
            sb.Append(" receivedate='");
            sb.Append(PK.ReceiveDate.ToString("yyyy-MM-dd"));
            sb.Append("' and sectionno=");
            sb.Append(PK.SectionNo);
            sb.Append(" and testtypeno=");
            sb.Append(PK.TestTypeNo);
            sb.Append(" and sampleno='");
            sb.Append(PK.SampleNo);
            sb.Append("'");
            return sb.ToString();
        }
        private bool IsColumn(PropertyInfo prop)
        {
            if (prop != null)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(ColumnAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 辅助方法
        private static void InitFillElementTable()
        {
            lock (Section2FillTypeMap)
            {
                ConfigManager.InitSection2FillElementTable(Section2FillTypeMap);
            }
        }
        protected static List<Type> GetAvailableFillElements(LisReportPK RK)
        {
            return Section2FillTypeMap[RK.SectionNo] as List<Type>;
        }

        protected void SetHandlerResult(HandleResult result, int code, string message)
        {
            result.ResultCode = code;
            result.Message = message;
        }
        protected void SetHandlerResult(HandleResult result, int code, Type type, string message)
        {
            SetHandlerResult(result, code, message);
            result.HandleType = type;
        }
        #endregion
    }
}
