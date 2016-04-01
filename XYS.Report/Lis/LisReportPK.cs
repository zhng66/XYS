﻿using System;
using XYS.Report;
namespace XYS.Report.Lis
{
    public class LisReportPK:IReportKey
    {
        #region
        private bool m_config;
        private int m_sectionNo;
        private int m_testTypeNo;
        private string m_sampleNo;
        private DateTime m_receiveDate;
        #endregion

        #region
        public LisReportPK()
        {
            this.m_config = false;
        }
        #endregion

        public int SectionNo
        {
            get { return this.m_sectionNo; }
            set { this.m_sectionNo = value; }
        }
        public int TestTypeNo
        {
            get { return this.m_testTypeNo; }
            set { this.m_testTypeNo = value; }
        }
        public string SampleNo
        {
            get { return this.m_sampleNo; }
            set { this.m_sampleNo = value; }
        }
        public DateTime ReceiveDate
        {
            get { return this.m_receiveDate; }
            set { this.m_receiveDate = value; }
        }

        public string ID
        {
            get { return this.m_receiveDate.ToString("yyyyMMdd") + "-" + this.m_sectionNo + "-" + this.m_testTypeNo + "-" + this.m_sampleNo; }
        }

        public bool Configured
        {
            get { return this.m_config; }
            set { this.m_config = value; }
        }
    }
}
