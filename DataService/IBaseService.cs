﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DataService
{
    public interface IBaseService<T> : IDependency where T : class, new()
    {
        ISqlSugarClient db { set; get; }
        #region add

        bool Insert(T t);

        bool Insert(SqlSugarClient client, T t);

        long InsertBigIdentity(T t);

        bool Insert(List<T> t);

        DbResult<bool> InsertTran(T t);

        DbResult<bool> InsertTran(List<T> t);

        T InsertReturnEntity(T t);

        T InsertReturnEntity(T t, string sqlWith = SqlWith.UpdLock);

        bool ExecuteCommand(string sql, object parameters);

        bool ExecuteCommand(string sql, params SugarParameter[] parameters);

        bool ExecuteCommand(string sql, List<SugarParameter> parameters);

        #endregion add

        #region update

        bool UpdateEntity(T entity);

        bool Update(T entity, Expression<Func<T, bool>> expression);

        /// <summary>
        /// 只更新表达式的值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        bool Update(T entity, Expression<Func<T, object>> expression);

        bool Update(T entity, Expression<Func<T, object>> expression, Expression<Func<T, bool>> where);

        bool Update(SqlSugarClient client, T entity, Expression<Func<T, object>> expression, Expression<Func<T, bool>> where);

        /// <summary>
        ///
        /// </summary>
        /// <param name="entity">T</param>
        /// <param name="list">忽略更新</param>
        /// <param name="isNull">Null不更新</param>
        /// <returns></returns>
        bool Update(T entity, List<string> list = null, bool isNull = true);

        bool Update(List<T> entity);

        #endregion update

        DbResult<bool> UseTran(Action action);

        DbResult<bool> UseTran(SqlSugarClient client, Action action);

        bool UseTran2(Action action);

        #region delete

        bool Delete(Expression<Func<T, bool>> expression);

        bool Delete<PkType>(PkType[] primaryKeyValues);

        bool Delete(object obj);

        #endregion delete

        #region query

        bool IsAny(Expression<Func<T, bool>> expression);

        ISugarQueryable<T> Queryable();

        ISugarQueryable<ExpandoObject> Queryable(string tableName, string shortName);

        //ISugarQueryable<T, T1, T2> Queryable<T1, T2>() where T1 : class where T2 : new();

        List<T> QueryableToList(Expression<Func<T, bool>> expression);

        List<T> QueryableToListOrder(Expression<Func<T, bool>> expression, Expression<Func<T, object>> sort_expression, string orderByType, int num);

        List<T> QueryableToList(string tableName);

        T QueryableToEntity(Expression<Func<T, bool>> expression);

        List<T> QueryableToList(string tableName, Expression<Func<T, bool>> expression);

        (List<T>, int) QueryableToPage(Expression<Func<T, bool>> expression, int pageIndex = 0, int pageSize = 10);

        (List<T>, int) QueryableToPage(Expression<Func<T, bool>> expression, string order, int pageIndex = 0, int pageSize = 10);

        (List<T>, int) QueryableToPage(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderFiled, string orderBy, int pageIndex = 0, int pageSize = 10);

        List<T> SqlQueryToList(string sql, object obj = null);

        #endregion query

        #region Procedure

        DataTable UseStoredProcedureToDataTable(string procedureName, List<SugarParameter> parameters);

        (DataTable, List<SugarParameter>) UseStoredProcedureToTuple(string procedureName, List<SugarParameter> parameters);

        #endregion Procedure

        bool InitTables();

    }
}
