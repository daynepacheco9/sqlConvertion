using System.Linq.Expressions;
using System.Reflection;
using System;

public static class MyDatabase
{
    public static string InnerJoin<T,R>(
        this Table<T> tabela1, Table<R> tabela2,
        Expression<Func<T,R,bool>> expression)
    {
        var body = expression.Body;
        string condition = toSql(body);

        string query = 
            $"""select * from {typeof(T).Name} inner join {typeof(R).Name} on {condition}""";
        
        return query;
    }

    public static string Insert<T>(
        this Table<T> tabela1,
        T entity
    ){
        string name = typeof(T).Name;

        var propriedade = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        string col = string.Join(", ", propriedade.Select(p => p.Name));
         string values = string.Join(", ", propriedade.Select(p => FormatValue(p.GetValue(entity))));
    }

    static string toSql(Expression exp) =>
        (exp, exp.NodeType) switch
        {
            (BinaryExpression bin, ExpressionType.And or ExpressionType.AndAlso) =>
                $"{toSql(bin.Left)} and {toSql(bin.Right)}",

            (BinaryExpression bin, ExpressionType.Equal) =>
                $"{toSql(bin.Left)} = {toSql(bin.Right)}",

            (BinaryExpression bin, ExpressionType.GreaterThan) =>
                $"{toSql(bin.Left)} > {toSql(bin.Right)}",

            (BinaryExpression bin, ExpressionType.LessThan) =>
                $"{toSql(bin.Left)} < {toSql(bin.Right)}",

            (BinaryExpression bin, ExpressionType.GreaterThanOrEqual) =>
                $"{toSql(bin.Left)} >= {toSql(bin.Right)}",

            (BinaryExpression bin, ExpressionType.LessThanOrEqual) =>
                $"{toSql(bin.Left)} <= {toSql(bin.Right)}",
            
            (MemberExpression mem, ExpressionType.MemberAccess) =>
                mem.Member.Name,
            
            (ConstantExpression cexp, ExpressionType.Constant) =>
                cexp.Value is string s ? $"'{s}'" : cexp.Value.ToString(),
            
            _ => throw new Exception($"Invalid expression type {exp.NodeType} ({exp.GetType()})")
        };
        static string GatTableName(Type type) => type.Name ;
}