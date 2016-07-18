﻿using System;
using System.Collections.Generic;

using XYS.Lis;
using XYS.Util;
using XYS.Report;
using XYS.Lis.Report.Model;
namespace XYS.Lis.Report.Handler
{
    public class InfoHandle : HandleSkeleton
    {
        public InfoHandle()
            : base()
        {
        }
        protected override bool HandleElement(IFillElement element, ReportPK RK)
        {
            bool result = false;
            InfoElement info = element as InfoElement;
            if (info != null)
            {
                info.ReportID = RK.ID;
                if (info.SectionNo == 11)
                {
                    if (info.Memo != null)
                    {
                        info.Memo = info.Memo.Replace(";", SystemInfo.NewLine);
                    }
                }
                if (info.CID != null)
                {
                    info.CID = info.CID.Trim();
                }

                result = true;
            }
            return result;
        }

        protected override bool HandleElement(List<IFillElement> elements, ReportPK RK)
        {
            if (IsExist(elements))
            {
                bool result = false;
                for (int i = elements.Count - 1; i >= 0; i--)
                {
                    result = HandleElement(elements[i], RK);
                    if (!result)
                    {
                        elements.RemoveAt(i);
                    }
                }
                return true;
            }
            return false;
        }
    }
}