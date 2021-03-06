﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;

using Loachs.Entity;
using Loachs.Data;

namespace Loachs.Data.WinSqlite
{
    public class Tag : ITag
    {
        /// <summary>
        /// 检查别名是否重复
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        private bool CheckSlug(TagInfo term)
        {
            while (true)
            {
                string cmdText = string.Empty;
                if (term.TagId == 0)
                {
                    cmdText = string.Format("select count(1) from Loachs_Terms where Slug='{0}' and type={1}", term.Slug, (int)TermType.Tag);
                }
                else
                {
                    cmdText = string.Format("select count(1) from Loachs_Terms where Slug='{0}'  and type={1} and termid<>{2}", term.Slug, (int)TermType.Tag, term.TagId);
                }
                int r = Convert.ToInt32(SqliteHelper.ExecuteScalar(cmdText));
                if (r == 0)
                {
                    return true;
                }
                term.Slug += "-2";
            }
        }

        public int InsertTag(TagInfo tag)
        {
            CheckSlug(tag);

            string cmdText = @"insert into Loachs_Terms
                            (
                            Type,Name,Slug,Description,Displayorder,Count,CreateDate
                            )
                            values
                            (
                            @Type,@Name,@Slug,@Description,@Displayorder,@Count,@CreateDate
                            )";
            SQLiteParameter[] prams = { 
                                SqliteHelper.MakeInParam("@Type",DbType.Int32,1,(int)TermType.Tag),
								SqliteHelper.MakeInParam("@Name",DbType.String,255,tag.Name),
                                SqliteHelper.MakeInParam("@Slug",DbType.String,255,tag.Slug),
								SqliteHelper.MakeInParam("@Description",DbType.String,255,tag.Description),
                                SqliteHelper.MakeInParam("@Displayorder",DbType.Int32,4,tag.Displayorder),
								SqliteHelper.MakeInParam("@Count",DbType.Int32,4,tag.Count),
								SqliteHelper.MakeInParam("@CreateDate",DbType.Date,8,tag.CreateDate)
							};
            SqliteHelper.ExecuteScalar(CommandType.Text, cmdText, prams);

            //int newId = Convert.ToInt32(SqliteHelper.ExecuteScalar("select top 1 termid from Loachs_Terms order by termid desc"));
            int newId = Convert.ToInt32(SqliteHelper.ExecuteScalar("select termid from Loachs_Terms order by termid desc limit 0,1"));


            return newId;
        }

        public int UpdateTag(TagInfo tag)
        {
            CheckSlug(tag);

            string cmdText = @"update Loachs_Terms set
                                Type=@Type,
                                Name=@Name,
                                Slug=@Slug,
                                Description=@Description,
                                Displayorder=@Displayorder,
                                Count=@Count,
                                CreateDate=@CreateDate
                                where termid=@termid";
            SQLiteParameter[] prams = { 
                                SqliteHelper.MakeInParam("@Type",DbType.Int32,1,(int)TermType.Tag),
								SqliteHelper.MakeInParam("@Name",DbType.String,255,tag.Name),
                                SqliteHelper.MakeInParam("@Slug",DbType.String,255,tag.Slug),
								SqliteHelper.MakeInParam("@Description",DbType.String,255,tag.Description),
                                SqliteHelper.MakeInParam("@Displayorder",DbType.Int32,4,tag.Displayorder),
								SqliteHelper.MakeInParam("@Count",DbType.Int32,4,tag.Count),
								SqliteHelper.MakeInParam("@CreateDate",DbType.Date,8,tag.CreateDate),
                                SqliteHelper.MakeInParam("@termid",DbType.Int32,1,tag.TagId),
							};
            return Convert.ToInt32(SqliteHelper.ExecuteScalar(CommandType.Text, cmdText, prams));
        }

        public int DeleteTag(int tagId)
        {
            string cmdText = "delete from Loachs_Terms where termid = @termid";
            SQLiteParameter[] prams = { 
								SqliteHelper.MakeInParam("@termid",DbType.Int32,4,tagId)
							};
            return SqliteHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);


        }

        public TagInfo GetTag(int tagId)
        {
            string cmdText = "select * from Loachs_Terms where termid = @termid";
            SQLiteParameter[] prams = { 
								SqliteHelper.MakeInParam("@termid",DbType.Int32,4,tagId)
							};

            List<TagInfo> list = DataReaderToList(SqliteHelper.ExecuteReader(CommandType.Text, cmdText, prams));
            return list.Count > 0 ? list[0] : null;
        }


        public List<TagInfo> GetTagList()
        {
            string condition = " type=" + (int)TermType.Tag;

            string cmdText = "select * from Loachs_Terms where " + condition + "  order by displayorder asc ,termid asc";

            return DataReaderToList(SqliteHelper.ExecuteReader(cmdText));

        }

        //public List<TagInfo> GetTagList(int pageSize, int pageIndex, out int recordCount)
        //{
        //    string condition = " [type]=" + (int)TermType.Tag;


        //    string cmdTotalRecord = "select count(1) from [Loachs_Terms] where " + condition;

        //    recordCount = Convert.ToInt32(SqliteHelper.ExecuteScalar(CommandType.Text, cmdTotalRecord));


        //    string cmdText = SqliteHelper.GetPageSql("[Loachs_Terms]", "[termid]", "*", pageSize, pageIndex, 1, condition);
        //    return DataReaderToList(SqliteHelper.ExecuteReader(cmdText));
        //}

        public List<TagInfo> GetTagList(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return new List<TagInfo>();
            }

            string cmdText = "select * from Loachs_Terms where  termid in (" + ids + ")";

            //  throw new Exception(cmdText);

            return DataReaderToList(SqliteHelper.ExecuteReader(cmdText));
        }


        //public int GetCount(int tagId, bool incUncategorized)
        //{
        //    string cmdText = "select count(1) from [Loachs_Posts] where [tag] like '%" + tagId + "}%'";

        //    if (!incUncategorized)
        //    {
        //        cmdText = "select count(1) from [Loachs_Posts] where [categoryid]>0 and [tag] like '%" + tagId + "}%'";
        //    }

        //    return Convert.ToInt32(SqliteHelper.ExecuteScalar(cmdText));
        //}

        /// <summary>
        /// 转换实体
        /// </summary>
        /// <param name="read">OleDbDataReader</param>
        /// <returns>TagInfo</returns>
        private static List<TagInfo> DataReaderToList(SQLiteDataReader read)
        {
            List<TagInfo> list = new List<TagInfo>();
            while (read.Read())
            {
                TagInfo tag = new TagInfo();
                tag.TagId = Convert.ToInt32(read["termid"]);
                //  tag.Type = Convert.ToInt32(read["Type"]);
                tag.Name = Convert.ToString(read["Name"]);
                tag.Slug = Convert.ToString(read["Slug"]);
                tag.Description = Convert.ToString(read["Description"]);
                tag.Displayorder = Convert.ToInt32(read["Displayorder"]);
                tag.Count = Convert.ToInt32(read["Count"]);
                tag.CreateDate = Convert.ToDateTime(read["CreateDate"]);

                list.Add(tag);
            }
            read.Close();
            return list;
        }


    }
}
