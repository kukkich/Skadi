using System.Linq.Expressions;
using Skadi.FEM.Core.Assembling.Params;

namespace Skadi.FEM.Assembling;

// Todo кажется что LambdaExpression - ненужное ограничение. Типо, а если захочу просто Func передать
// Нужно выделить отдельный парсерс Expression -> Func.
// Но есть проблема, что мы заранее не знаем, какие дженерик параметры у Func<TArg, T>
// Как воркарануд - можно завести список всех возможных наборов дженериков
// при парсинге просто проходиться по всем и пытаться скастить к ним
// Но тут тоже могут быть проблемы
// На подумать: можно тут хранить сразу всё вместе: и Func<...> и LambdaExpression
// Сделать метод "TExpression GetExpression<TExpression>(int id)" 
// Если по id лежит Func с запрашиваемым TExpression - возвращаем его
// Если лежит LambdaExpression - приводим к Expression<TExpression>, компилируем, получаем TExpression
// результат компиляции можно кэшировать или же прям перезаписывать по id
public class ArrayExpressionProvider(IReadOnlyList<LambdaExpression> expressions) : IExpressionProvider
{
    public LambdaExpression GetExpression(int id) => expressions[id];
}