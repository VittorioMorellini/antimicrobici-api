using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Antimicrobici.Core.Utils
{
    public static class QueryBuilderExtensions
    {
        public static string ApplyPaging(this string query, QueryBuilderSearchModel model)
        {
            //if (!string.IsNullOrWhiteSpace(model.OrderBy))
            //    query += $" order by {model.OrderBy}";

            //if (model.Skip != null || model.Take != null)
            //{
            //    if (string.IsNullOrWhiteSpace(model.OrderBy))
            //        throw new Exception("OrderBy must be defined when using Skip/Take");

            //    if (model.Skip != null)
            //        query += $" offset {model.Skip.GetValueOrDefault()} rows";
            //    if (model.Take != null)
            //        query += $" fetch next {model.Take.GetValueOrDefault()} rows only";
            //}

            if (model.Pager != null && !model.Pager.Ignore)
            {
                if (!string.IsNullOrWhiteSpace(model.Pager.OrderBy))
                    query += $" order by {model.Pager.OrderBy}";

                if (model.Pager.Skip != null || model.Pager.Take != null)
                {
                    query += $" order by {(string.IsNullOrWhiteSpace(model.Pager.OrderBy) ? "1" : model.Pager.OrderBy)}";
                    query += $" offset {model.Pager.Skip.GetValueOrDefault()} rows";

                    if (model.Pager.Take != null)
                        query += $" fetch next {model.Pager.Take.GetValueOrDefault()} rows only";
                }
            }

            return query;
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, QueryBuilderSearchModel model)
        {
            if (model.Pager != null && !model.Pager.Ignore)
            {
                if (!string.IsNullOrWhiteSpace(model.Pager.OrderBy))
                {
                    bool descending = false;
                    string[] token = model.Pager.OrderBy.Split(' ');
                    string column = token[0].Trim();
                    if (token.Length > 1)
                        descending = token[1].ToUpper() == "DESC";

                    query = query.OrderBy(column, descending);
                }

                if (model.Pager.Skip != null)
                    query = query.Skip(model.Pager.Skip.Value);

                if (model.Pager.Take != null)
                    query = query.Take(model.Pager.Take.Value);

            }

            return query;
        }

        private static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc)
        {
            orderByProperty = orderByProperty.ToPascalCase();

            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            if (property == null)
                throw new Exception($"{orderByProperty} property not found in type {type.FullName}");

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }

    public class QueryBuilder
    {
        public static Dictionary<DbOperator, string> OperatorMaps = new Dictionary<DbOperator, string>()
        {
            { DbOperator.EQUALS,           "{0} = {1}" },
            { DbOperator.NOT_EQUALS,       "{0} <> {1}" },
            { DbOperator.GREATER,          "{0} > {1}" },
            { DbOperator.GREATER_THAN,     "{0} >= {1}" },
            { DbOperator.LESS,             "{0} < {1}" },
            { DbOperator.LESS_THAN,        "{0} <= {1}" },
            { DbOperator.LIKE,             "{0} like '%' + {1} + '%'" },
            { DbOperator.STARTS_WITH,      "{0} like {1} + '%'" },
            { DbOperator.ENDS_WITH,        "{0} like '%' + {1}" },
            { DbOperator.IN,               "{0} in ({1})" },
            { DbOperator.NOT_IN,           "{0} not in ({1})" },
            { DbOperator.IS_NULL_OR_EQUAL, "{0} is null or {0} = {1}" }
        };

        private string _s = "";     // select
        private string _f = "";     // from
        private string _j = "";     // join
        private string _w = "";     // where
        private string _g = "";     // group by
        private string _o = "";     // order by
        private string _wo = "";    // ramo della where che contiene tutti i predicati in OR
        private string _skip = "";  
        private string _take = "";

        public List<DbParameter> Parameters { get; private set; }
        public JoinTables JoinTables { get; set; }
        public SelectTables SelectTables { get; set; }
        public QueryBuilder Cte { get; set; }

        public QueryBuilder(QueryBuilderSearchModel model = null)
        {
            Parameters = new List<DbParameter>();
            JoinTables = new JoinTables();
            SelectTables = new SelectTables();

            //if (model != null)
            //{
            //    if (model.Skip != null || model.Take != null)
            //    {
            //        if (string.IsNullOrWhiteSpace(model.OrderBy))
            //            throw new Exception("OrderBy must be defined when using Skip/Take");

            //        Skip(model.Skip.GetValueOrDefault());
            //        if (model.Take != null)
            //            Take(model.Take.GetValueOrDefault());
            //    }

            //    if (!string.IsNullOrWhiteSpace(model.OrderBy))
            //        OrderBy(model.OrderBy);
            //}

            if (model != null && model.Pager != null && !model.Pager.Ignore)
            {
                OrderBy(string.IsNullOrWhiteSpace(model.Pager.OrderBy) ? "1" : model.Pager.OrderBy);

                if (model.Pager.Skip != null || model.Pager.Take != null)
                {                    
                    Skip(model.Pager.Skip.GetValueOrDefault());
                    if (model.Pager.Take != null)
                        Take(model.Pager.Take.GetValueOrDefault());
                }
            }
        }

        public string Create(bool excludeSkipTake = false)
        {
            foreach (var item in SelectTables.Values.Where(x => x.Apply))
                Select(item.Text);

            foreach (var item in JoinTables.Values.Where(x => x.Apply))
                Join(item.Text + " ");

            // scarico i predicati in OR sulla where
            Where(_wo);

            if (Cte != null)
            {
                if (string.IsNullOrEmpty(Cte._s))
                    Cte.Select("*");

                return $";with x as ({_s} {_f} {_j} {_w} {_g}) {Cte.Create()} {_o} {(excludeSkipTake ? "" : _skip)} {(excludeSkipTake ? "" : _take)}";
            }
            else
            {
                return $"{_s} {_f} {_j} {_w} {_g} {_o} {(excludeSkipTake ? "" : _skip)} {(excludeSkipTake ? "" : _take)}";
            }
        }

        public void Create(DbCommand command, bool excludeSkipTake = false)
        {
            command.CommandText = Create(excludeSkipTake);
            command.Parameters.AddRange(Parameters.ToArray());
            if (Cte != null)
                command.Parameters.AddRange(Cte.Parameters.ToArray());
        }

        public DbDataReader ExecuteReader(DbCommand command, bool excludeSkipTake = false)
        {
            Create(command, excludeSkipTake);
            return command.ExecuteReader();
        }

        public string ExecuteReaderCsv(DbCommand command, string[] header = null, bool excludeSkipTake = false)
        {
            string COL_SEPARATOR = ";";
            string ROW_SEPARATOR = Environment.NewLine;

            string csv = header != null ? (string.Join(COL_SEPARATOR, header) + ROW_SEPARATOR) : "";
            using (var reader = ExecuteReader(command, excludeSkipTake))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        object[] row = new object[reader.FieldCount];
                        reader.GetValues(row);

                        for (int i = 0; i < reader.FieldCount; i++)
                            row[i] = row[i].ToString().Replace(COL_SEPARATOR, "").Replace(ROW_SEPARATOR, " ");

                        csv += string.Join(";", row);
                        csv += Environment.NewLine;
                    }
                }
            }
            return csv;
        }

        public QueryBuilder Select(string value)
        {
            value = value.Trim();
            _s += _s.Length > 0 && !_s.EndsWith(",") ? (", " + value) : value;

            if (!_s.StartsWith("select"))
                _s = "select " + _s;

            return this;
        }

        public QueryBuilder Select(string[] values)
        {
            return Select(string.Join(", ", values));
        }

        public QueryBuilder From(string value)
        {
            _f = "from " + value;
            return this;
        }

        public QueryBuilder Join(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return this;

            _j += value.TrimEnd() + " ";
            return this;
        }

        public QueryBuilder Where(string value, string op = "and")
        {
            if (string.IsNullOrWhiteSpace(value))
                return this;

            _w += String.Format("{0} ({1})", _w.StartsWith("where") ? op : "where", value);
            return this;
        }


        /// <summary>
        /// Ramo della where che contiene tutti i predicati in OR
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public QueryBuilder WhereOr(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return this;

            _wo += string.Format("{0}({1})", _wo.Length > 0 ? "or " : "", value);
            return this;
        }

        public QueryBuilder GroupBy(string value)
        {
            _g = "group by " + value;
            return this;
        }

        public QueryBuilder OrderBy(string value)
        {
            _o = "order by " + value;
            return this;
        }

        public QueryBuilder ThenBy(string value)
        {
            _o += ", " +value;
            return this;
        }

        public QueryBuilder Skip(int value)
        {
            _skip = $"offset {value} rows";
            return this;
        }

        public QueryBuilder Take(int value)
        {
            _take = $"fetch next {value} rows only";
            return this;
        }

        public QueryBuilder AddPredicate<M, T>(M model, Func<M, T> func, string colName, DbOperator op = DbOperator.EQUALS, string conditionTable = null, Func<M, bool> isValid = null, bool or = false)
        {
            string predicate = GetPredicate(model, func, colName, op, conditionTable, isValid);
            if (!string.IsNullOrWhiteSpace(predicate))
            {
                if (or)
                    WhereOr(predicate);
                else
                    Where(predicate);
            }

            return this;
        }

        public string GetPredicate<M, T>(M model, Func<M, T> func, string colName, DbOperator op = DbOperator.EQUALS, string conditionTable = null, Func<M, bool> isValid = null)
        {
            string predicate = "";

            isValid = isValid ?? (x => { return func(x) != null && !string.IsNullOrWhiteSpace(func(x).ToString().Trim()); });
            if (isValid(model))
            {
                if (op.Equals(DbOperator.IN) || op.Equals(DbOperator.NOT_IN))
                {
                    var z = func(model);
                    List<object> tokens = new List<object>();
                    if (z.GetType().FullName == "System.String")
                    {
                        tokens.AddRange(z.ToString().Split(','));
                    }
                    else
                    {
                        tokens.AddRange((z as IEnumerable).Cast<object>());
                    }

                    List<string> inParams = new List<string>();
                    foreach(var v in tokens)
                    {
                        string parName = string.Format("@p{0}", Parameters.Count);
                        inParams.Add(parName);
                        Parameters.Add(new SqlParameter(parName, v));
                    }
                    predicate = string.Format(OperatorMaps[op], colName, string.Join(",", inParams));
                }
                else
                {
                    string parName = string.Format("@p{0}", Parameters.Count);
                    predicate = string.Format(OperatorMaps[op], colName, parName);

                    T v = func(model);
                    if (v.GetType().Name == "String")
                    {
                        // imposto esplicitamente il tipo a VarChar, altrimenti nascerebbe in automatico come NVarChar
                        var p = new SqlParameter(parName, System.Data.SqlDbType.VarChar);
                        p.Value = v;
                        Parameters.Add(p);
                    }
                    else
                    {
                        Parameters.Add(new SqlParameter(parName, v));
                    }
                }

                if (conditionTable != null)
                {
                    if (JoinTables.ContainsKey(conditionTable))
                        JoinTables[conditionTable].Apply = true;
                    else
                        throw new Exception($"JoinTable '{conditionTable}' not present");

                    if (SelectTables.ContainsKey(conditionTable))
                        SelectTables[conditionTable].Apply = true;
                }
            }

            return predicate;
        }

        public string AddPredicateIsNotNull<M>(M model, Func<M, bool?> func, string colName)
        {
            string predicate = "";

            var value = func(model);
            if (value != null)
                Where(string.Format("{0} is {1}null", colName, value.GetValueOrDefault() ? "not " : ""));

            return predicate;
        }

        public string And(string op1, string op2)
        {
            string result = "";

            if (string.IsNullOrEmpty(op1))
                result = op2;
            else if (string.IsNullOrEmpty(op2))
                result = op1;
            else
                result = string.Format("{0} and {1}", op1, op2);

            return !string.IsNullOrWhiteSpace(result) ? string.Format("({0})", result) : "";
        }

        public string Or(string op1, string op2)
        {
            string result = "";

            if (string.IsNullOrEmpty(op1))
                result = op2;
            else if (string.IsNullOrEmpty(op2))
                result = op1;
            else
                result = string.Format("{0} or {1}", op1, op2);

            return !string.IsNullOrWhiteSpace(result) ? string.Format("({0})", result) : "";
        }

        public string ApplyPaging(string query, QueryBuilderSearchModel model)
        {
            if (model.Pager != null)
            {
                query += $" order by {(string.IsNullOrWhiteSpace(model.Pager.OrderBy) ? "1" : model.Pager.OrderBy)}";

                if (!model.Pager.Ignore &&
                    (model.Pager.Skip != null || model.Pager.Take != null))
                {
                    query += $" offset {model.Pager.Skip.GetValueOrDefault()} rows";
                    if (model.Pager.Take != null)
                        query += $" fetch next {model.Pager.Take.GetValueOrDefault()} rows only";
                }
            }

            return query;
        }

        public QueryBuilder GetOrCreateCte()
        {   
            if (Cte == null)
                Cte = new QueryBuilder().From("x");

            return Cte;
        }

        public bool HasApplyCondition(string keyTable)
        {
            if (JoinTables.TryGetValue(keyTable, out ConditionTable jt) && jt != null && jt.Apply)
                return true;

            if (SelectTables.TryGetValue(keyTable, out ConditionTable st) && st != null && st.Apply)
                return true;

            return false;
        }
    }

    public class QueryBuilderSearchModel
    {
        //public int? Skip { get; set; }
        //public int? Take { get; set; }        
        //public string OrderBy { get; set; }

        public PagerModel Pager { get; set; } = new PagerModel();
    }

    public class PagerModel
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public string OrderBy { get; set; }
        public bool Ignore { get; set; }
    }

    public class JoinTables: Dictionary<string, ConditionTable>
    {
        public void Add(string key, string join)
        {
            //if (this.ContainsKey(key))
            //    throw new Exception($"JoinTable '{key}' already present");

            this[key] = new ConditionTable() { Text = join };
        }
    }

    public class SelectTables : Dictionary<string, ConditionTable>
    {
        public void Add(string key, string select)
        {
            //if (this.ContainsKey(key))
            //    throw new Exception($"SelectTable '{key}' already present");

            this[key] = new ConditionTable() { Text = select };
        }
    }

    public class ConditionTable
    {
        public bool Apply { get; set; }
        public string Text { get; set; }
    }

    public enum DbOperator
    {
        EQUALS,
        NOT_EQUALS,
        GREATER_THAN,
        GREATER,
        LESS_THAN,
        LESS,
        LIKE,
        STARTS_WITH,
        ENDS_WITH,
        IN,
        NOT_IN,
        IS_NULL_OR_EQUAL
    }
}

