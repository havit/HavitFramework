using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Havit.Data.SqlServer;

namespace Havit.Business.Query;

    /// <summary>
    /// Podmínka, která neguje vnitřní (kompozitní) podmínky.
    /// </summary>
    public class NotCondition : Condition
    {
    /// <summary>
        /// Podmínky, které jsou negovány. Mezi podmínkami je operátor AND.
        /// </summary>
        public ConditionList Conditions
        {
            get
            {
                return _andConditions.Conditions;
            }
        }
        private readonly AndCondition _andConditions;

        /// <summary>
        /// Vytvoří instanci podmínky NotCondition a případně ji inicializuje zadanými vnitřními podmínkami.
        /// </summary>
        public NotCondition(params Condition[] conditions)
        {
            _andConditions = new AndCondition();
            if (conditions != null)
            {
                for (int index = 0; index < conditions.Length; index++)
                {
                    _andConditions.Conditions.Add(conditions[index]);
                }
            }
        }

        /// <summary>
        /// Přidá část SQL příkaz pro sekci WHERE.
        /// </summary>
	public override void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions)
        {
		Debug.Assert(command != null);
		Debug.Assert(whereBuilder != null);

            if (IsEmptyCondition())
            {
                return;
            }

            whereBuilder.Append("NOT ("); 
            _andConditions.GetWhereStatement(command, whereBuilder, sqlServerPlatform, commandBuilderOptions);
            whereBuilder.Append(")");
        }

        /// <summary>
        /// Udává, zda podmínka reprezentuje prázdnou podmínku, která nebude renderována.
        /// </summary>
        public override bool IsEmptyCondition()
        {
            return _andConditions.IsEmptyCondition();
        }

        /// <summary>
        /// Vytvoří instanci podmínky NotCondition a případně ji inicializuje zadanými vnitřními podmínkami.
        /// </summary>
        public static NotCondition Create(params Condition[] conditions)
        {
            return new NotCondition(conditions);
        }
    }
