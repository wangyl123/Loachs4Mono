﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

using System.Data;
using Loachs.Entity;
using Loachs.Data;
using Loachs.Common;

namespace Loachs.Data.WinSqlite
{
    public class Post : IPost
    {
        /// <summary>
        /// 检查别名是否重复
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        private bool CheckSlug(PostInfo post)
        {
            if (string.IsNullOrEmpty(post.Slug))
            {
                return true;
            }
            while (true)
            {
                string cmdText = string.Empty;
                if (post.PostId == 0)
                {
                    cmdText = string.Format("select count(1) from Loachs_Posts where slug='{0}'  ", post.Slug);
                }
                else
                {
                    cmdText = string.Format("select count(1) from Loachs_Posts where slug='{0}'   and postid<>{1}", post.Slug, post.PostId);
                }
                int r = Convert.ToInt32(SqliteHelper.ExecuteScalar(cmdText));
                if (r == 0)
                {
                    return true;
                }
                post.Slug += "-2";
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="postinfo">实体</param>
        /// <returns>成功返回新记录的ID,失败返回 0</returns>
        public int InsertPost(PostInfo postinfo)
        {
            CheckSlug(postinfo);
            string cmdText = @"insert into Loachs_Posts
                                (
                               CategoryId,Title,Summary,Content,Slug,UserId,CommentStatus,CommentCount,ViewCount,Tag,UrlFormat,Template,Recommend,Status,TopStatus,HideStatus,CreateDate,UpdateDate
                                )
                                values
                                (
                                @CategoryId,@Title,@Summary,@Content,@Slug,@UserId,@CommentStatus,@CommentCount,@ViewCount,@Tag,@UrlFormat,@Template,@Recommend,@Status,@TopStatus,@HideStatus,@CreateDate,@UpdateDate
                                )";
            SQLiteParameter[] prams = { 
								
                                SqliteHelper.MakeInParam("@CategoryId",DbType.Int32,4,postinfo.CategoryId),
								SqliteHelper.MakeInParam("@Title",DbType.String,255,postinfo.Title),
								SqliteHelper.MakeInParam("@Summary",DbType.String,0,postinfo.Summary),
								SqliteHelper.MakeInParam("@Content",DbType.String,0,postinfo.Content),
								SqliteHelper.MakeInParam("@Slug",DbType.String,255,postinfo.Slug),
								SqliteHelper.MakeInParam("@UserId",DbType.Int32,4,postinfo.UserId),
								SqliteHelper.MakeInParam("@CommentStatus",DbType.Int32,1,postinfo.CommentStatus),
								SqliteHelper.MakeInParam("@CommentCount",DbType.Int32,4,postinfo.CommentCount),
								SqliteHelper.MakeInParam("@ViewCount",DbType.Int32,4,postinfo.ViewCount),
								SqliteHelper.MakeInParam("@Tag",DbType.String,255,postinfo.Tag),
                                SqliteHelper.MakeInParam("@UrlFormat",DbType.Int32,1,postinfo.UrlFormat),
                                SqliteHelper.MakeInParam("@Template",DbType.String,50,postinfo.Template ),
                                SqliteHelper.MakeInParam("@Recommend",DbType.Int32,1,postinfo.Recommend),
								SqliteHelper.MakeInParam("@Status",DbType.Int32,1,postinfo.Status),
                                SqliteHelper.MakeInParam("@TopStatus",DbType.Int32,1,postinfo.TopStatus),
                                SqliteHelper.MakeInParam("@HideStatus",DbType.Int32,1,postinfo.HideStatus),
								SqliteHelper.MakeInParam("@CreateDate",DbType.Date,8,postinfo.CreateDate),
								SqliteHelper.MakeInParam("@UpdateDate",DbType.Date,8,postinfo.UpdateDate)
							};
            SqliteHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);
            //int newId = StringHelper.ObjectToInt(SqliteHelper.ExecuteScalar("select top 1 PostId from Loachs_Posts order by PostId desc"));
            int newId = StringHelper.ObjectToInt(SqliteHelper.ExecuteScalar("select PostId from Loachs_Posts order by PostId desc limit 0,1"));

            //if (newId > 0)
            //{
            //    SqliteHelper.ExecuteNonQuery(string.Format("update [Loachs_Users] set [postcount]=[postcount]+1 where [userid]={0}", postinfo.UserId));
            //    SqliteHelper.ExecuteNonQuery("update [Loachs_Sites] set [postcount]=[postcount]+1");
            //    SqliteHelper.ExecuteNonQuery(string.Format("update [Loachs_Terms] set [count]=[count]+1 where [termid]={0}", postinfo.CategoryId));
            //}
            return newId;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="postinfo">实体</param>
        /// <returns>修改的行数</returns>
        public int UpdatePost(PostInfo postinfo)
        {
            CheckSlug(postinfo);

            //PostInfo oldPost = GetPost(postinfo.PostId);        //修改前

            //if (oldPost.CategoryId != postinfo.CategoryId)
            //{

            //    SqliteHelper.ExecuteNonQuery(string.Format("update [Loachs_Terms] set [count]=[count]-1 where [termid]={0}", oldPost.CategoryId));

            //    SqliteHelper.ExecuteNonQuery(string.Format("update [Loachs_Terms] set [count]=[count]+1 where [termid]={0}", postinfo.CategoryId));

            //}


            string cmdText = "update Loachs_Posts set  CategoryId=@CategoryId,Title=@Title,Summary=@Summary,Content=@Content,Slug=@Slug,UserId=@UserId,CommentStatus=@CommentStatus,CommentCount=@CommentCount,ViewCount=@ViewCount,Tag=@Tag,UrlFormat=@UrlFormat,Template=@Template,Recommend=@Recommend,Status=@Status,TopStatus=@TopStatus,HideStatus=@HideStatus,CreateDate=@CreateDate,UpdateDate=@UpdateDate where PostId=@PostId";
            SQLiteParameter[] prams = { 
                               
                                SqliteHelper.MakeInParam("@CategoryId",DbType.Int32,4,postinfo.CategoryId),
								SqliteHelper.MakeInParam("@Title",DbType.String,255,postinfo.Title),
								SqliteHelper.MakeInParam("@Summary",DbType.String,0,postinfo.Summary),
								SqliteHelper.MakeInParam("@Content",DbType.String,0,postinfo.Content),
								SqliteHelper.MakeInParam("@Slug",DbType.String,255,postinfo.Slug),
								SqliteHelper.MakeInParam("@UserId",DbType.Int32,4,postinfo.UserId),
								SqliteHelper.MakeInParam("@CommentStatus",DbType.Int32,1,postinfo.CommentStatus),
								SqliteHelper.MakeInParam("@CommentCount",DbType.Int32,4,postinfo.CommentCount),
								SqliteHelper.MakeInParam("@ViewCount",DbType.Int32,4,postinfo.ViewCount),
								SqliteHelper.MakeInParam("@Tag",DbType.String,255,postinfo.Tag),
                                SqliteHelper.MakeInParam("@UrlFormat",DbType.Int32,1,postinfo.UrlFormat),
                                SqliteHelper.MakeInParam("@Template",DbType.String,50,postinfo.Template ),
                                SqliteHelper.MakeInParam("@Recommend",DbType.Int32,1,postinfo.Recommend),
								SqliteHelper.MakeInParam("@Status",DbType.Int32,1,postinfo.Status),
                                SqliteHelper.MakeInParam("@TopStatus",DbType.Int32,1,postinfo.TopStatus),
                                SqliteHelper.MakeInParam("@HideStatus",DbType.Int32,1,postinfo.HideStatus),
								SqliteHelper.MakeInParam("@CreateDate",DbType.Date,8,postinfo.CreateDate),
								SqliteHelper.MakeInParam("@UpdateDate",DbType.Date,8,postinfo.UpdateDate),
                                SqliteHelper.MakeInParam("@PostId",DbType.Int32,4,postinfo.PostId),
							};
            return SqliteHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="PostId">主键</param>
        /// <returns>删除的行数</returns>
        public int DeletePost(int postid)
        {
            PostInfo oldPost = GetPost(postid);        //删除前

            string cmdText = "delete from Loachs_Posts where PostId = @PostId";
            SQLiteParameter[] prams = { 
								SqliteHelper.MakeInParam("@PostId",DbType.Int32,4,postid)
							};
            int result = SqliteHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);



            //if (oldPost != null)
            //{
            //    SqliteHelper.ExecuteNonQuery(string.Format("update [Loachs_Users] set [postcount]=[postcount]-1 where [userid]={0}", oldPost.UserId));
            //    SqliteHelper.ExecuteNonQuery("update [Loachs_Sites] set [postcount]=[postcount]-1");
            //    SqliteHelper.ExecuteNonQuery(string.Format("update [Loachs_Terms] set [count]=[count]-1 where [termid]={0}", oldPost.CategoryId));
            //}

            return result;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="PostId">主键</param>
        /// <returns></returns>
        public PostInfo GetPost(int postid)
        {
            //string cmdText = "select top 1 * from [Loachs_Posts] where [PostId] = @PostId";
            string cmdText = "select * from Loachs_Posts where PostId = @PostId limit 0,1";
            SQLiteParameter[] prams = { 
								        SqliteHelper.MakeInParam("@PostId",DbType.Int32,4,postid)
							            };


            List<PostInfo> list = DataReaderToCommentList(SqliteHelper.ExecuteReader(cmdText, prams));

            return list.Count > 0 ? list[0] : null;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public PostInfo GetPost(string slug)
        {
            //string cmdText = "select top 1 * from [Loachs_Posts] where [slug] = @slug";
            string cmdText = "select * from Loachs_Posts where slug = @slug limit 0,1";
            SQLiteParameter[] prams = { 
								        SqliteHelper.MakeInParam("@slug",DbType.String,200,slug)
							            };


            List<PostInfo> list = DataReaderToCommentList(SqliteHelper.ExecuteReader(cmdText, prams));

            return list.Count > 0 ? list[0] : null;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns>IList</returns>
        public List<PostInfo> GetPostList()
        {
            string cmdText = "select * from Loachs_Posts order by postid desc";
            return DataReaderToCommentList(SqliteHelper.ExecuteReader(cmdText));
        }

        public List<PostInfo> GetPostList(int pageSize, int pageIndex, out int recordCount, int categoryId, int tagId, int userId, int recommend, int status, int topstatus, int hidestatus, string begindate, string enddate, string keyword)
        {
            string condition = " 1=1 ";

            if (categoryId != -1)
            {
                condition += " and categoryId=" + categoryId;
            }
            if (tagId != -1)
            {
                condition += " and tag like '%{" + tagId + "}%'";
            }
            if (userId != -1)
            {
                condition += " and userid=" + userId;
            }
            if (recommend != -1)
            {
                condition += " and recommend=" + recommend;
            }
            if (status != -1)
            {
                condition += " and status=" + status;
            }

            if (topstatus != -1)
            {
                condition += " and topstatus=" + topstatus;
            }

            if (hidestatus != -1)
            {
                condition += " and hidestatus=" + hidestatus;
            }

            if (!string.IsNullOrEmpty(begindate))
            {
                condition += " and createdate>=#" + begindate + "#";
            }
            if (!string.IsNullOrEmpty(enddate))
            {
                condition += " and createdate<#" + enddate + "#";
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                condition += string.Format(" and (summary like '%{0}%' or title like '%{0}%'  )", keyword);
            }

            string cmdTotalRecord = "select count(1) from Loachs_Posts where " + condition;

            //   throw new Exception(cmdTotalRecord);

            recordCount = StringHelper.ObjectToInt(SqliteHelper.ExecuteScalar(CommandType.Text, cmdTotalRecord));


            string cmdText = SqliteHelper.GetPageSql("Loachs_Posts", "PostId", "*", pageSize, pageIndex, 1, condition);



            return DataReaderToCommentList(SqliteHelper.ExecuteReader(cmdText));
        }

        public List<PostInfo> GetPostListByRelated(int postId, int rowCount)
        {
            string tags = string.Empty;

            PostInfo post = GetPost(postId);

            if (post != null && post.Tag.Length > 0)
            {
                tags = post.Tag;


                tags = tags.Replace("}", "},");
                string[] idList = tags.Split(',');

                string where = " (";
                foreach (string tagID in idList)
                {
                    if (!string.IsNullOrEmpty(tagID))
                    {
                        where += string.Format("  tags like '%{0}%' or ", tagID);
                    }
                }
                where += " 1=2 ) and status=1 and postid<>" + postId;

                //string cmdText = string.Format("select top {0} * from Loachs_Posts where {1} order by postid desc", rowCount, where);
                string cmdText = string.Format("select  * from Loachs_Posts where {1} order by postid desc limit 0,{0}", rowCount, where);


                return DataReaderToCommentList(SqliteHelper.ExecuteReader(cmdText));
            }
            return new List<PostInfo>();
        }

        ///// <summary>
        ///// 根据别名获取文章ID
        ///// </summary>
        ///// <param name="slug"></param>
        ///// <returns></returns>
        //public int GetPostId(string slug)
        //{
        //    string cmdText = "select [postid] from [Loachs_Posts] where [slug]=@slug";
        //    SQLiteParameter[] prams = {  
        //                           SqliteHelper.MakeInParam("@slug",DbType.String,200,slug),

        //                            };
        //    return StringHelper.ObjectToInt(SqliteHelper.ExecuteScalar(CommandType.Text, cmdText, prams));

        //}

        public List<ArchiveInfo> GetArchive()
        {
            //string cmdText = "select format(createdate, 'yyyymm') as [date] ,  count(*) as [count] from [Loachs_Posts] where [status]=1 and [hidestatus]=0  group by  format(createdate, 'yyyymm')  order by format(createdate, 'yyyymm') desc";
            string cmdText = "select strftime(\"%Y%m\",createdate) as date ,  count(*) as count from Loachs_Posts where status=1 and hidestatus=0  group by  strftime(\"%Y%m\",createdate)  order by strftime(\"%Y%m\",createdate) desc";

            List<ArchiveInfo> list = new List<ArchiveInfo>();
            using (SQLiteDataReader read = SqliteHelper.ExecuteReader(cmdText))
            {

                while (read.Read())
                {
                    ArchiveInfo archive = new ArchiveInfo();
                    string date = read["date"].ToString().Substring(0, 4) + "-" + read["date"].ToString().Substring(4, 2);
                    archive.Date = Convert.ToDateTime(date);
                    // archive.Title = read["date"].ToString();
                    archive.Count = StringHelper.ObjectToInt(read["count"]);
                    list.Add(archive);
                }
            }
            return list;

        }

        public int UpdatePostViewCount(int postId, int addCount)
        {
            string cmdText = "update Loachs_Posts set viewcount = viewcount + @addcount where postid=@postid";
            SQLiteParameter[] prams = { 
								SqliteHelper.MakeInParam("@addcount",DbType.Int32,4,addCount),
                                SqliteHelper.MakeInParam("@postid",DbType.Int32,4,postId),
							};
            return SqliteHelper.ExecuteNonQuery(CommandType.Text, cmdText, prams);
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        private List<PostInfo> DataReaderToCommentList(SQLiteDataReader read)
        {
            List<PostInfo> list = new List<PostInfo>();
            while (read.Read())
            {
                PostInfo postinfo = new PostInfo();
                postinfo.PostId = StringHelper.ObjectToInt(read["PostId"]);

                postinfo.CategoryId = StringHelper.ObjectToInt(read["CategoryId"]);
                postinfo.Title = Convert.ToString(read["Title"]);
                postinfo.Summary = Convert.ToString(read["Summary"]);
                postinfo.Content = Convert.ToString(read["Content"]);
                postinfo.Slug = Convert.ToString(read["Slug"]);
                postinfo.UserId = StringHelper.ObjectToInt(read["UserId"]);
                postinfo.CommentStatus = StringHelper.ObjectToInt(read["CommentStatus"]);
                postinfo.CommentCount = StringHelper.ObjectToInt(read["CommentCount"]);
                postinfo.ViewCount = StringHelper.ObjectToInt(read["ViewCount"]);
                postinfo.Tag = Convert.ToString(read["Tag"]);

                postinfo.UrlFormat = StringHelper.ObjectToInt((read["UrlFormat"]));
                postinfo.Template = Convert.ToString(read["Template"]);

                postinfo.Recommend = StringHelper.ObjectToInt(read["Recommend"]);
                postinfo.Status = StringHelper.ObjectToInt(read["Status"]);
                postinfo.TopStatus = StringHelper.ObjectToInt(read["TopStatus"]);
                postinfo.HideStatus = StringHelper.ObjectToInt(read["HideStatus"]);

                postinfo.CreateDate = Convert.ToDateTime(read["CreateDate"]);
                postinfo.UpdateDate = Convert.ToDateTime(read["UpdateDate"]);
                list.Add(postinfo);
            }
            read.Close();
            return list;
        }

    }
}
