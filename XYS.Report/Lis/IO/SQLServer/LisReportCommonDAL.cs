﻿using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;

using XYS.Common;
using XYS.Report.Lis.Model;
using XYS.Report.Lis.IO.Utility;
namespace XYS.Report.Lis.IO.SQLServer
{
    public class LisReportCommonDAL
    {
        #region
        public LisReportCommonDAL()
        {
        }
        #endregion

        #region
        public void Fill(AbstractFillElement element, string sql)
        {
            DataTable dt = GetDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                FillData(element, element.GetType(), dt.Rows[0], dt.Columns);
            }
        }
        public void FillList(List<AbstractFillElement> elementList, Type elementType, string sql)
        {
            DataTable dt = GetDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                AbstractFillElement element;
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        element = (AbstractFillElement)elementType.Assembly.CreateInstance(elementType.FullName);
                        FillData(element, dr, dt.Columns);
                        //AfterFill(element);
                        elementList.Add(element);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
        }
        #endregion

        #region
        //填充对象属性
        protected void FillData(AbstractFillElement element, DataRow dr, DataColumnCollection columns)
        {
            Type type = element.GetType();
            FillData(element, type, dr, columns);
        }
        protected void FillData(AbstractFillElement element, Type type, DataRow dr, DataColumnCollection columns)
        {
            PropertyInfo prop = null;
            foreach (DataColumn dc in columns)
            {
                prop = type.GetProperty(dc.ColumnName);
                if (IsColumn(prop))
                {
                    FillProperty(element, prop, dr[dc]);
                }
            }
        }
        protected bool FillProperty(AbstractFillElement element, PropertyInfo p, DataRow dr)
        {
            try
            {
                if (dr[p.Name] != null)
                {
                    if (dr[p.Name] != DBNull.Value)
                    {
                        object value = Convert.ChangeType(dr[p.Name], p.PropertyType);
                        p.SetValue(element, value, null);
                    }
                    else
                    {
                        p.SetValue(element, DefaultForType(p.PropertyType), null);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        protected bool FillProperty(AbstractFillElement element, PropertyInfo p, object v)
        {
            try
            {
                if (v != DBNull.Value)
                {
                    object value = Convert.ChangeType(v, p.PropertyType);
                    p.SetValue(element, value, null);
                }
                else
                {
                    p.SetValue(element, DefaultForType(p.PropertyType), null);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        protected object DefaultForType(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
        protected DataTable GetDataTable(string sql)
        {
            DataTable dt = null;
            if (!string.IsNullOrEmpty(sql))
            {
                dt = DbHelperSQL.Query(sql).Tables["dt"];
            }
            return dt;
        }
        #endregion

        #region
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
        private bool StrEqual(string str1, string str2)
        {
            return str1.ToLower().Equals(str2.ToLower());
        }
        private PropertyInfo GetProperty(PropertyInfo[] props, string propName)
        {
            PropertyInfo p = null;
            foreach (PropertyInfo prop in props)
            {
                if (StrEqual(prop.Name, propName))
                {
                    p = prop;
                    break;
                }
            }
            return p;
        }
        #endregion
    }
}
