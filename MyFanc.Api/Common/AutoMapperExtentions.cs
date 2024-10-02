using AutoMapper;

namespace MyFanc.Api.Common
{
    public static class AutoMapperExtentions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAllMembers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expr)
        {
            var destinationType = typeof(TDestination);
            
            foreach (var property in destinationType.GetProperties())
                expr.ForMember(property.Name, opt => opt.Ignore());
            
            return expr;
        }
    }
}
