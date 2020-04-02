using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MovieApp.API.Infrastructure
{
    public class SearchOptionsProcessor<T, TEntity>
    {
        private readonly string[] searchQuery;

        public SearchOptionsProcessor(string[] searchQuery)
        {
            this.searchQuery = searchQuery;
        }

        public IEnumerable<SearchTerm> GetAllTerms()
        {
            if (searchQuery == null)
            {
                yield break;
            }

            foreach (var expression in searchQuery)
            {
                if (string.IsNullOrEmpty(expression)) continue;

                var tokens = expression.Split(' ');

                if (tokens.Length == 0)
                {
                    yield return new SearchTerm 
                    { 
                        Name = expression,
                        ValidSyntax = false,

                    };
                    continue;
                }

                if (tokens.Length > 3)
                {
                    yield return new SearchTerm
                    {
                        Name = tokens[0],
                        ValidSyntax = false,

                    };
                    continue;
                }

                yield return new SearchTerm
                {
                    Name = tokens[0],
                    ValidSyntax = true,
                    Operator = tokens[1],
                    Value = string.Join(" ", tokens.Skip(2))
                };

            }
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var terms = GetValidTerms().ToArray();
            if (!terms.Any()) return query;

            var modifiedQuery = query;

            foreach (var term in terms)
            {
                var propertyInfo = ExpressionHelper
                    .GetPropertyInfo<TEntity>(term.Name);

                var obj = ExpressionHelper.Parameter<TEntity>();

                var left = ExpressionHelper
                    .GetPropertyExpression(obj, propertyInfo);

                var right = term.ExpressionProvider.GetValue(term.Value);

                var comparisonExpression = term.ExpressionProvider
                    .GetComparison(left, term.Operator, right);

                var lambdaExpression = ExpressionHelper.
                    GetLambda<TEntity, bool>(obj, comparisonExpression); 

                modifiedQuery = ExpressionHelper
                    .CallWhere(
                    modifiedQuery, lambdaExpression);

            }

            return modifiedQuery;
        }

        public IEnumerable<SearchTerm> GetValidTerms()
        {
            var queryTerms = GetAllTerms()
                .Where(x => x.ValidSyntax)
                .ToArray();

            if (!queryTerms.Any()) yield break;

            var declaredTerms = GetTermsFromModel();

            foreach (var term in queryTerms)
            {
                var declaredTerm = declaredTerms
                    .SingleOrDefault(x => x.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));

                if (declaredTerm == null) continue;

                yield return new SearchTerm
                {
                    Name = declaredTerm.Name,
                    ValidSyntax = term.ValidSyntax,
                    Value = term.Value,
                    Operator = term.Operator,
                    ExpressionProvider = declaredTerm.ExpressionProvider
                };
            }
        }

        private static IEnumerable<SearchTerm> GetTermsFromModel()
            => typeof(T).GetTypeInfo()
            .DeclaredProperties
            .Where(p => p.GetCustomAttributes<SearchableAttribute>().Any())
            .Select(p => new SearchTerm 
            { 
                Name = p.Name,
                ExpressionProvider = p.GetCustomAttribute<SearchableAttribute>().ExpressionProvider
            });
    }
}
