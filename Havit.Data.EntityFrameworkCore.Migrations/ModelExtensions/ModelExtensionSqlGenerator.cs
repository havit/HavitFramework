namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
    /// <summary>
    /// Base implementation of <see cref="IModelExtensionSqlGenerator"/> that provides strongly-typed implementation of methods for inherited classes to use.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="IModelExtension"/> that this <see cref="IModelExtensionSqlGenerator"/> implementation should handle.</typeparam>
    public abstract class ModelExtensionSqlGenerator<T> : IModelExtensionSqlGenerator
		where T : IModelExtension
	{
		string IModelExtensionSqlGenerator.GenerateDropSql(IModelExtension modelExtension) => IsMatchingModelExtension(modelExtension) ? GenerateDropSql((T)modelExtension) : null;

        string IModelExtensionSqlGenerator.GenerateAlterSql(IModelExtension modelExtension) => IsMatchingModelExtension(modelExtension) ? GenerateAlterSql((T)modelExtension) : null;

        /// <summary>
        /// Generates DROP SQL script for <see cref="IModelExtension"/> of type <typeparamref name="T"/>.
        /// </summary>
        protected abstract string GenerateDropSql(T modelExtension);

        /// <summary>
        /// Generates ALTER SQL script for <see cref="IModelExtension"/> of type <typeparamref name="T"/>.
        /// </summary>
        protected abstract string GenerateAlterSql(T modelExtension);

		private static bool IsMatchingModelExtension(IModelExtension modelExtension) => typeof(T).IsAssignableFrom(modelExtension?.GetType());
	}
}