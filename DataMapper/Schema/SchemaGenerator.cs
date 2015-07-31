#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using DataMapper.Extentions;

#endregion

namespace DataMapper.Schema
{
    public class SchemaGenerator
    {
        public static string ReadSchema(string connectionstrings, bool includeViews, string type,
            string procedureName = null , bool upper = false)
        {
            var sb = new StringBuilder();
            if (type.Equals("Table"))
            {
                const string providername = "System.Data.SqlClient";
                DatabaseSchema schema;
                using (var dbReader = new DatabaseReader(connectionstrings, providername))
                {
                    schema = dbReader.ReadAll();
                }
                var tablolar = schema.Tables;
                foreach (var table in tablolar)
                {
                    sb.AppendLine("\t[Table(" + "\"" + table.Name + "\"" + ")]");
                    sb.AppendLine("\tpublic partial class " + table.Name.ToLetterCase() + " { ");
                    var columns = table.Columns;
                    foreach (var colum in columns)
                    {

                        if (colum.IsPrimaryKey) sb.AppendLine("\t\t[Key]");
                        sb.AppendLine("\t\t[Column(" + "\"" + colum.Name + "\"" + ")]");
                        sb.AppendLine("\t\t public virtual " +
                                      DataTypeExtensions.GetNetString(colum.DataType.GetNetType().Name, colum.Nullable) +
                                      " " + colum.Name.ToLetterCase() + " { get; set; } ");
                    }
                    sb.AppendLine("\t } ");
                }
                if (includeViews)
                {
                    var views = schema.Views;
                    foreach (var view in views)
                    {
                        sb.AppendLine("\t[Table(" + "\"" + view.Name + "\"" + ")]");
                        sb.AppendLine("\tpublic partial class " + view.Name.ToLetterCase() + " { ");
                        var columns = view.Columns;
                        foreach (var colum in columns)
                        {
                            sb.AppendLine("\t\t[Column(" + "\"" + colum.Name + "\"" + ")]");
                            sb.AppendLine("\t\t public virtual " +
                                          DataTypeExtensions.GetNetString(colum.DataType.GetNetType().Name,
                                              colum.Nullable) +
                                          " " + colum.Name.ToLetterCase() + " { get; set; } ");
                        }
                        sb.AppendLine("\t } ");
                    }
                }

            }
            else if (type.Equals("Procedure"))
            {
                using (var db = new SqlConnection(connectionstrings))
                {

                    using (var cmd = db.CreateCommand())
                    {
                        db.Open();
                        cmd.CommandText = "Helper_CreatePocoFromProcName";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@procname",
                            DbType = DbType.String,
                            Value = procedureName
                        });
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sb.AppendLine(reader.GetString(0));
                            }
                            reader.Close();
                        }
                    }
                    db.Close();
                    db.Dispose();
                }

            }
            else if (type.Equals("Parameter"))
            {
                sb.AppendLine("var p = new DynamicParameters();");

                using (var db = new SqlConnection(connectionstrings))
                {

                    using (var cmd = db.CreateCommand())
                    {
                        db.Open();
                        cmd.CommandText = "Helper_CreateParameterFromProcedure";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@procedure",
                            DbType = DbType.String,
                            Value = procedureName
                        });
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                /*/*
                p.Add("@lang", lang, DbType.String);*/
                                var parameterName = reader["ParameterName"].ToString();
                                string cleanParameterName;
                                if (upper)
                                {
                                    cleanParameterName = parameterName.Replace("@", "");
                                }
                                else
                                {
                                    cleanParameterName = parameterName.Replace("@", "").ToLetterCase();
                                }
                                
                                var parameterDataType = reader["ParameterDataType"].ToString().ToClrType().ToString();
                                sb.AppendLine("p.Add(\"" + parameterName + "\", item." + cleanParameterName + ", DbType." +parameterDataType+");");
                            }
                            reader.Close();
                        }
                    }
                    db.Close();
                    db.Dispose();
                }
            }
            return sb.ToString();
        }

    }

    public static class TipExtentions
    {
        private static Dictionary<string, DbType> Mappings;

        static TipExtentions()
        {
            Mappings = new Dictionary<string, DbType>();
            Mappings.Add("bigint", DbType.Int64);
            Mappings.Add("binary", DbType.Byte);
            Mappings.Add("bit", DbType.Boolean);
            Mappings.Add("char", DbType.String);
            Mappings.Add("date", DbType.Date);
            Mappings.Add("datetime", DbType.DateTime);
            Mappings.Add("datetime2", DbType.DateTime2);
            Mappings.Add("datetimeoffset", DbType.DateTimeOffset);
            Mappings.Add("decimal", DbType.Decimal);
            Mappings.Add("float", DbType.Double);
            Mappings.Add("int", DbType.Int32);
            Mappings.Add("money", DbType.Decimal);
            Mappings.Add("nchar", DbType.String);
            Mappings.Add("ntext", DbType.String);
            Mappings.Add("numeric", DbType.Decimal);
            Mappings.Add("nvarchar", DbType.String);
            Mappings.Add("real", DbType.Single);
            Mappings.Add("rowversion", DbType.Byte);
            Mappings.Add("smalldatetime", DbType.DateTime);
            Mappings.Add("smallint", DbType.Int16);
            Mappings.Add("smallmoney", DbType.Decimal);
            Mappings.Add("text", DbType.String);
            Mappings.Add("tinyint", DbType.Byte);
            Mappings.Add("uniqueidentifier", DbType.Guid);
            Mappings.Add("varchar", DbType.String);

        }

        public static DbType ToClrType(this string sqlType)
        {
            DbType datatype;
            if (Mappings.TryGetValue(sqlType, out datatype))
                return datatype;
            throw new TypeLoadException(string.Format("Can not load CLR Type from {0}", sqlType));
        }

    }
}