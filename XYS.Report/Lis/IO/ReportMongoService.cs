﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using XYS.Util;
using XYS.Report.Lis.Model;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
namespace XYS.Report.Lis.IO
{
    public class ReportMongoService
    {
        #region 字段
        static MongoClient MClient;
        static IMongoDatabase LisMDB;
        private static readonly string MongoConnectionStr = "mongodb://10.1.11.10:27017";
        #endregion
        
        #region 构造函数
        static ReportMongoService()
        {
            MClient = new MongoClient(MongoConnectionStr);
            LisMDB = MClient.GetDatabase("lis");
        }
        public ReportMongoService()
        { 
        }
        #endregion

        #region 同步方法
        public void Insert(ReportReportElement report)
        {
            try
            {
                IMongoCollection<ReportReportElement> ReportCollection = LisMDB.GetCollection<ReportReportElement>("report");
                ReportCollection.InsertOne(report);
                this.SetHandlerResult(report.HandleResult, 2, "save to mongo sucessfully");
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("fialed when save report to mongo,error message:");
                sb.Append(ex.Message);
                sb.Append(SystemInfo.NewLine);
                sb.Append(ex.ToString());
                this.SetHandlerResult(report.HandleResult, -1, this.GetType(), sb.ToString());
            }
        }
        public void Insert(IEnumerable<ReportReportElement> reportList)
        {
            IMongoCollection<ReportReportElement> ReportCollection = LisMDB.GetCollection<ReportReportElement>("report");
            ReportCollection.InsertMany(reportList);
        }
        public void Query()
        {
            FilterDefinition<ReportReportElement> filter = "{}";
            IMongoCollection<ReportReportElement> ReportCollection = LisMDB.GetCollection<ReportReportElement>("report");
            using (var cursor = ReportCollection.Find(filter).ToCursor())
            {
                while (cursor.MoveNext())
                {
                    foreach (ReportReportElement rre in cursor.Current)
                    {
                        // do something with the documents
                        Console.WriteLine(rre.SuperItemList.Count);
                    }
                }
            }
        }
        #endregion

        #region 异步方法
        public async Task InsertAsync(ReportReportElement report, Func<ReportReportElement, Task> callback = null)
        {
            if (report.HandleResult.ResultCode != -1)
            {
                try
                {
                    IMongoCollection<ReportReportElement> ReportCollection = LisMDB.GetCollection<ReportReportElement>("report");
                    await ReportCollection.InsertOneAsync(report);
                    this.SetHandlerResult(report.HandleResult, 2, this.GetType(), "save to mongo sucessfully");
                }
                catch (Exception ex)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("fialed when save report to mongo,error message:");
                    sb.Append(ex.Message);
                    sb.Append(SystemInfo.NewLine);
                    sb.Append(ex.ToString());
                    this.SetHandlerResult(report.HandleResult, -1, this.GetType(), sb.ToString());
                }
            }
            if (callback != null)
            {
                await callback(report);
            }
        }
        public async Task InsertAsync()
        {
 
        }
        #endregion

        #region 辅助方法
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