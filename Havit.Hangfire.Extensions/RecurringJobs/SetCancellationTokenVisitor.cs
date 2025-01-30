using System.Linq.Expressions;

namespace Havit.Hangfire.Extensions.RecurringJobs;

internal class SetCancellationTokenVisitor : ExpressionVisitor
{
	private readonly CancellationToken targetCancellationToken;

	public SetCancellationTokenVisitor(CancellationToken targetCancellationToken)
	{
		this.targetCancellationToken = targetCancellationToken;
	}

	public override Expression Visit(Expression node)
	{
		return base.Visit(node);
	}

	protected override Expression VisitConstant(ConstantExpression node)
	{
		return ((node.Type == typeof(CancellationToken)) && ((CancellationToken)node.Value == default(CancellationToken)))
			? Expression.Constant(targetCancellationToken)
			: base.VisitConstant(node);
	}

	protected override Expression VisitMember(MemberExpression node)
	{
		return ((node.Expression == null) && (node.Type == typeof(CancellationToken)) && (node.Member.Name == nameof(CancellationToken.None)))
			? Expression.Constant(targetCancellationToken)
			: base.VisitMember(node);
	}
}