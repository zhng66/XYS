﻿using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using XYS.Lis.Config;
using XYS.Lis.Core;
using XYS.Model;
using XYS.Lis.Model;

namespace XYS.Lis.Util
{
    public class LisMap
    {
        #region
        private static readonly ReporterSectionMap SECTION_MAP = new ReporterSectionMap();
        private static readonly ParItemMap PARITEM_MAP = new ParItemMap();
        private static readonly ReportModelMap MODEL_MAP = new ReportModelMap();
        private static readonly ReportElementTypeMap ELEMENT_TYPE_MAP = new ReportElementTypeMap();
        #endregion

        #region
        private static ParamSection paramSection = (ParamSection)ConfigurationManager.GetSection("lis-param");
        #endregion

        #region
        public static ReporterSection GetSection(int sectionNo)
        {
            if (SECTION_MAP.Count == 0)
            {
                //
                ConfigureSection();
            }
            return SECTION_MAP[sectionNo];
        }
        public static ParItem GetParItem(int parItemNo)
        {
            if (PARITEM_MAP.Count == 0)
            {
                //
                ConfigureParItem();
            }
            return PARITEM_MAP[parItemNo];
        }
        
      
        public static ReportModel GetPrintModel(int modelNo)
        {
            if (MODEL_MAP.Count == 0)
            {
 
            }
            return MODEL_MAP[modelNo];
        }
        public static ReportElementType GetElementType(string  elementName)
        {
            if (ELEMENT_TYPE_MAP.Count == 0)
            {
                //初始化
                ConfigureReportElement();
            }
            return ELEMENT_TYPE_MAP[elementName];
        }
        public static void InitParItem2PrintModelTable(Hashtable table)
        {
            table.Clear();
            if (PARITEM_MAP.Count == 0)
            {
                //
                ConfigureParItem();
            }
            foreach(ParItem item in PARITEM_MAP.AllParItem)
            {
                table.Add(item.ParItemNo, item.ModelNo);
            }
        }
        public static void InitParItem2OrderNoTable(Hashtable table)
        {
            table.Clear();
            if (PARITEM_MAP.Count == 0)
            {
                //
                ConfigureParItem();
            }
            foreach (ParItem item in PARITEM_MAP.AllParItem)
            {
                table.Add(item.ParItemNo, item.OrderNo);
            }
        }
        public static void InitSection2PrintModelTable(Hashtable table)
        {
            table.Clear();
            if (SECTION_MAP.Count == 0)
            {
                //
                ConfigureSection();
            }
            foreach (ReporterSection section in SECTION_MAP.AllReporterSection)
            {
                table.Add(section.SectionNo, section.ModelNo);
            }
        }
        public static void InitSection2OrderNoTable(Hashtable table)
        {
            table.Clear();
            if (SECTION_MAP.Count == 0)
            {
                //
                ConfigureSection();
            }
            foreach (ReporterSection section in SECTION_MAP.AllReporterSection)
            {
                table.Add(section.SectionNo, section.OrderNo);
            }
        }
        public static void InitSection2ElementTypeTable(Hashtable table,int sectionNo)
        {
            ReporterSection rs = GetSection(sectionNo);
            if (rs == null)
            {
                return;
            }
            HashSet<ReportElementType> elementSet = new HashSet<ReportElementType>();
            if (rs.ElementNameList.Count == 0)
            {
                elementSet.Add(ReportElementType.DEFAULTEXAM);
                elementSet.Add(ReportElementType.DEFAULTPATIENT);
                elementSet.Add(ReportElementType.DEFAULTREPORT);
            }
            else
            {
                ReportElementType ret;
                foreach (string name in rs.ElementNameList)
                {
                    ret = GetElementType(name);
                    if (ret != null)
                    {
                        elementSet.Add(ret);
                    }
                }
            }
            table[sectionNo] = elementSet;
        }
        public static ReportElementTypeCollection GetSection2ElementTypeCollcetion(int sectionNo)
        {
            ReporterSection rs = GetSection(sectionNo);
            if (rs == null)
            {
                return null;
            }
            ReportElementTypeCollection retc = new ReportElementTypeCollection();
            if (rs.ElementNameList.Count == 0)
            {
                retc.Add(ReportElementType.DEFAULTEXAM);
                retc.Add(ReportElementType.DEFAULTPATIENT);
                retc.Add(ReportElementType.DEFAULTREPORT);
            }
            else
            {
                ReportElementType ret;
                foreach (string name in rs.ElementNameList)
                {
                    ret = GetElementType(name);
                    if (ret != null)
                    {
                        retc.Add(ret);
                    }
                }
            }
            return retc;
        }
        #endregion
        
        #region
        private static void ConfigureModel()
        {
            ModelElement modelElement;
            ModelElementCollection modelCollection = paramSection.ModelCollection;
            foreach (ConfigurationElement element in modelCollection)
            {
                modelElement = element as ModelElement;
                if (modelElement != null)
                {
                    ConfigureModel(modelElement);
                }
            }
        }
        private static void ConfigureModel(ModelElement model)
        {
            if (model.Value != null && model.ModelPath != null)
            {
                string name = model.Name;
                int modelNo = (int)model.Value;
                string path = model.ModelPath;
                ReportModel reportModel = new ReportModel(modelNo, path, name);
                MODEL_MAP.Add(reportModel);
            }
        }
        private static void ConfigureSection()
        {
            SectionElement sectionElement;
            SectionElementCollection sectionElementCollection = paramSection.SectionCollection;
            foreach (ConfigurationElement element in sectionElementCollection)
            {
                sectionElement = element as SectionElement;
                if (sectionElement != null)
                {
                    ConfigureSection(sectionElement);
                }
            }
        }
        private static void ConfigureSection(SectionElement sectionElement)
        {
            if (sectionElement.Value != null)
            {
                ReporterSection section = new ReporterSection((int)sectionElement.Value, sectionElement.Name);
                if (sectionElement.ModelNo != null)
                {
                    section.ModelNo = (int)sectionElement.ModelNo;
                }
                if (sectionElement.OrderNo != null)
                {
                    section.OrderNo = (int)sectionElement.OrderNo;
                }
                if (sectionElement.ReportElementFlags != null && !sectionElement.ReportElementFlags.Equals(""))
                {
                    string[] reportElements = sectionElement.ReportElementFlags.Split(new char[] { ',' });
                    section.ClearElementNameList();
                    foreach (string element in reportElements)
                    {
                        section.AddElementName(element);
                    }
                }
                SECTION_MAP.Add(section);
            }
        }
        private static void ConfigureParItem()
        {
            ParItemElement parItemElement;
            ParItemElementCollection parItemCollection = paramSection.ParItemCollection;
            foreach (ConfigurationElement element in parItemCollection)
            {
                parItemElement = element as ParItemElement;
                if (parItemElement != null)
                {
                    ConfigureParItem(parItemElement);
                }
            }
        }
        private static void ConfigureParItem(ParItemElement parItemElement)
        {
            if (parItemElement.Value != null)
            {
                ParItem parItem = new ParItem((int)parItemElement.Value, parItemElement.Name);
                if (parItemElement.ModelNo != null)
                {
                    parItem.ModelNo = (int)parItemElement.ModelNo;
                }
                if (parItemElement.OrderNo != null)
                {
                    parItem.OrderNo = (int)parItemElement.OrderNo;
                }
                parItem.ImageFlag = parItemElement.CommonImageFlag;
                parItem.ImagePath = parItemElement.CommonImagePath;
                PARITEM_MAP.Add(parItem);
            }
        }
        private static void ConfigureReportElement()
        {
            ReportElementElement reportElement;
            ReportElementElementCollection reportElementCollection = paramSection.ReportElementCollection;
            foreach (ConfigurationElement element in reportElementCollection)
            {
                reportElement = element as ReportElementElement;
                if (reportElement != null)
                {
                    ConfigureReportElement(reportElement);
                }
            }
        }
        private static void ConfigureReportElement(ReportElementElement reportElement)
        {
            if (reportElement.Type != null && !reportElement.Type.Equals(""))
            {
                ReportElementType elementType = new ReportElementType(reportElement.Type);
                elementType.ElementName = reportElement.Name;
                ELEMENT_TYPE_MAP.Add(elementType);
            }
        }
        #endregion
    }
}