﻿using System;
using System.Collections;
using System.Collections.Generic;

using XYS.Common;
using XYS.Lis.Core;
using XYS.Lis.Model;
using XYS.Lis.Util;
namespace XYS.Lis.Fill
{
    public abstract class ReportFillSkeleton : IReportFiller
    {
        #region 字段
        private readonly string m_fillerName;
        private readonly Hashtable m_section2InsideElementMap;
        private readonly Hashtable m_section2ExtendElementMap;
        #endregion

        #region 构造函数
        protected ReportFillSkeleton(string name)
        {
            this.m_fillerName = name;
            this.m_section2InsideElementMap = new Hashtable(20);
        }
        #endregion

        #region 实现IReportFiller接口
        public string FillerName
        {
            get { return this.m_fillerName.ToLower(); }
        }
        public virtual void Fill(IReportElement reportElement, ReportKey RK)
        {
            if (reportElement.ElementTag == ReportElementTag.Report)
            {
                ReportReportElement rre = reportElement as ReportReportElement;
                if (rre != null)
                {
                    FillReport(rre, RK);
                }
            }
            else
            {
                if (IsFill(reportElement))
                {
                    //填充元素
                    FillElement(reportElement, RK);
                }
            }
        }
        public virtual void Fill(List<IReportElement> reportElementList, ReportKey RK, Type type)
        {
            if (IsFill(type))
            {
                FillElements(reportElementList, RK, type);
            }
        }
        #endregion

        #region 内部处理逻辑
        protected virtual void FillReport(ReportReportElement rre, ReportKey RK)
        {
            List<Type> availableElementList = this.GetAvailableInsideElements(RK);
            if (availableElementList != null && availableElementList.Count > 0)
            {
                List<IReportElement> tempList = null;
                foreach (Type type in availableElementList)
                {
                    if (IsFill(type))
                    {
                        tempList = rre.GetReportItem(type.Name);
                        FillElements(tempList, RK, type);
                    }
                }
            }
        }
        #endregion

        #region 抽象方法
        protected abstract void FillElement(IReportElement reportElement, ReportKey RK);
        protected abstract void FillElements(List<IReportElement> reportElementList, ReportKey RK, Type type);
        #endregion

        #region 辅助方法
        protected virtual int GetSectionNo(ReportKey RK)
        {
            int sectionNo = 0;
            foreach (KeyColumn c in RK.KeySet)
            {
                if (c.Name.ToLower().Equals("sectionno") || c.Name.ToLower().Equals("r.sectionno"))
                {
                    try
                    {
                        sectionNo = Convert.ToInt32(c.Value);
                    }
                    catch (Exception ex)
                    {
                        return -1;
                    }
                    break;
                }
            }
            return sectionNo;
        }
        protected virtual List<Type> GetAvailableInsideElements(int sectionNo)
        {
            if (this.m_section2InsideElementMap.Count == 0)
            {
                InitInsideElementTable();
            }
            return this.m_section2InsideElementMap[sectionNo] as List<Type>;
        }
        protected virtual List<Type> GetAvailableInsideElements(ReportKey key)
        {
            int sectionNo = GetSectionNo(key);
            return GetAvailableInsideElements(sectionNo);
        }
        protected virtual ElementTypeMap GetAvailableExtendElements(int sectionNo)
        {
            if (this.m_section2InsideElementMap.Count == 0)
            {
                InitExtendElementTable();
            }
            ElementTypeMap result = this.m_section2InsideElementMap[sectionNo] as ElementTypeMap;
            return result;
        }

        #endregion

        #region 私有方法
        private void InitExtendElementTable()
        {
        }
        private void InitInsideElementTable()
        {
            lock (this.m_section2InsideElementMap)
            {
                LisMap.InitSection2InnerElementTable(this.m_section2InsideElementMap);
            }
        }
        private bool IsFill(IReportElement element)
        {
            return element is AbstractReportElement;
        }
        private bool IsFill(Type type)
        {
            if (type != null)
            {
                return typeof(AbstractReportElement).IsAssignableFrom(type);
            }
            return false;
        }
        #endregion

        #region
        //private bool IsTable(Type elementType)
        //{
        //    object[] attrs = elementType.GetCustomAttributes(typeof(TableAttribute), true);
        //    if (attrs != null && attrs.Length > 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //protected virtual ElementTypeMap GetAvailableInsideElements(int sectionNo)
        //{
        //    if (this.m_section2InsideElementMap.Count == 0)
        //    {
        //        InitInsideElementTable();
        //    }
        //    ElementTypeMap result = this.m_section2InsideElementMap[sectionNo] as ElementTypeMap;
        //    return result;
        //}
        //protected virtual ElementTypeMap GetAvailableInsideElements(ReportKey key)
        //{
        //    int sectionNo = GetSectionNo(key);
        //    return GetAvailableInsideElements(sectionNo);
        //}
        //protected Type GetElementType(string typeName)
        //{
        //    Type result;
        //    try
        //    {
        //        result = SystemInfo.GetTypeFromString(typeName, true, true);
        //    }
        //    catch (Exception ex)
        //    {
        //        result = null;
        //    }
        //    return result;
        //}
        #endregion
    }
}
