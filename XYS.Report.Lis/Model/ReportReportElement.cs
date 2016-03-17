﻿using System;
using System.Collections;
using System.Collections.Generic;

using XYS.Util;
using XYS.Common;
using XYS.Report.Lis.Core;
namespace XYS.Report.Lis.Model
{
    [Export()]
    public class ReportReportElement : AbstractFillElement
    {
        #region 私有实例字段
        private int m_sectionNo;
        private string m_serialNo;
        private string m_parItemName;

        private string m_reportTitle;

        private int m_remarkFlag;
        private string m_remark;

        private string m_technician;
        private string m_checker;

        private DateTime m_receiveDateTime;
        private DateTime m_collectDateTime;
        private DateTime m_inceptDateTime;
        private DateTime m_testDateTime;
        private DateTime m_checkDateTime;
        private DateTime m_secondCheckDateTime;

        private readonly Hashtable m_reportItemTable;

        private readonly ReportExamElement m_reportExam;
        private readonly ReportPatientElement m_reportPatient;
        private ReportImageElement m_reportImage;
        #endregion

        #region 公共构造函数
        public ReportReportElement()
        {
            this.m_remarkFlag = 0;
            this.m_reportImage = null;
            this.m_reportExam = new ReportExamElement();
            this.m_reportPatient = new ReportPatientElement();
            this.m_reportItemTable = SystemInfo.CreateCaseInsensitiveHashtable(3);
        }
        #endregion

        #region 实现ILisReportElement接口
        #endregion

        #region 实现抽象类方法
        #endregion

        #region 实例属性
        [Column(true)]
        public int SectionNo
        {
            get { return m_sectionNo; }
            set { m_sectionNo = value; }
        }
        [Export()]
        [Column(true)]
        public string SerialNo
        {
            get { return m_serialNo; }
            set { m_serialNo = value; }
        }
        [Export()]
        public string ReportTitle
        {
            get { return this.m_reportTitle; }
            set { this.m_reportTitle = value; }
        }

        [Export()]
        [Column(true)]
        public int SectionNo
        {
            get { return this.m_sectionNo; }
            set { this.m_sectionNo = value; }
        }
        [Export()]
        [Column(true)]
        public string ParItemName
        {
            get { return this.m_parItemName; }
            set { this.m_parItemName = value; }
        }
        
        //备注标识 0表示没有备注 1 表示备注已设置  2 表示备注未设置
        public int RemarkFlag
        {
            get { return this.m_remarkFlag; }
            set { this.m_remarkFlag = value; }
        }
        [Export()]
        public string Remark
        {
            get { return this.m_remark; }
            set { this.m_remark = value; }
        }

        [Export()]
        [Column(true)]
        public DateTime ReceiveDateTime
        {
            get { return this.m_receiveDateTime; }
            set { this.m_receiveDateTime = value; }
        }
        [Export()]
        [Column(true)]
        public DateTime CollectDateTime
        {
            get { return this.m_collectDateTime; }
            set { this.m_collectDateTime = value; }
        }
        [Export()]
        [Column(true)]
        public DateTime InceptDateTime
        {
            get { return m_inceptDateTime; }
            set { m_inceptDateTime = value; }
        }
        [Export()]
        [Column(true)]
        public DateTime TestDateTime
        {
            get { return m_testDateTime; }
            set { m_testDateTime = value; }
        }
        [Export()]
        [Column(true)]
        public DateTime CheckDateTime
        {
            get { return m_checkDateTime; }
            set { m_checkDateTime = value; }
        }
        [Export()]
        [Column(true)]
        public DateTime SecondeCheckDateTime
        {
            get { return m_secondCheckDateTime; }
            set { m_secondCheckDateTime = value; }
        }

        [Export()]
        [Column(true)]
        public string Technician
        {
            get { return m_technician; }
            set { m_technician = value; }
        }
        [Export()]
        [Column(true)]
        public string Checker
        {
            get { return m_checker; }
            set { m_checker = value; }
        }
        [Export()]
        public byte[] TechnicianImage
        {
            get { return this.m_technicianImage; }
            set { this.m_technicianImage = value; }
        }
        [Export()]
        public byte[] CheckerImage
        {
            get { return this.m_checkerImage; }
            set { this.m_checkerImage = value; }
        }

        public Hashtable ReportItemTable
        {
            get { return this.m_reportItemTable; }
        }
        public ReportExamElement ReportExam
        {
            get { return this.m_reportExam; }
        }
        public ReportPatientElement ReportPatient
        {
            get { return this.m_reportPatient; }
        }
        public ReportImageElement ReportImage
        {
            get { return this.m_reportImage; }
        }
        #endregion

        #region 公共方法

        public void Init()
        {
        }
        public void Clear()
        {
            this.ParItemList.Clear();
            this.ReportItemTable.Clear();
            this.ReportTitle = "";
            this.RemarkFlag = 0;
            this.Remark = "";
            this.TechnicianImage = null;
            this.CheckerImage = null;
        }

        public void RemoveReportItem(Type type)
        {
            if (type != null)
            {
                if (this.m_reportItemTable.ContainsKey(type))
                {
                    lock (this.m_reportItemTable)
                    {
                        this.m_reportItemTable.Remove(type);
                    }
                }
            }
        }
        public List<ILisReportElement> GetReportItem(Type type)
        {
            if (type != null)
            {
                List<ILisReportElement> result = this.m_reportItemTable[type] as List<ILisReportElement>;
                if (result == null)
                {
                    result = new List<ILisReportElement>(10);
                    lock (this.m_reportItemTable)
                    {
                        this.m_reportItemTable[type] = result;
                    }
                }
                return result;
            }
            return null;
        }
        #endregion
    }
}