﻿using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Radzen.Blazor
{
    /// <summary>
    /// RadzenSwitch component.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;RadzenSwitch @bind-Value=@value Change=@(args => Console.WriteLine($"Value: {args}")) /&gt;
    /// </code>
    /// </example>
    public partial class RadzenSwitch : FormComponent<bool>
    {
        [Parameter]
        public virtual string CssClass { get; set; }
        /// <inheritdoc />
        protected override string GetComponentCssClass()
        {
            return GetClassList("rz-switch").Add("rz-switch-checked", Value).ToString();
        }

        /// <summary>
        /// Toggles this instance checked state.
        /// </summary>
        public async System.Threading.Tasks.Task Toggle()
        {
            if (Disabled)
            {
                return;
            }

            Value = !Value;

            await ValueChanged.InvokeAsync(Value);
            if (FieldIdentifier.FieldName != null) { EditContext?.NotifyFieldChanged(FieldIdentifier); }
            await Change.InvokeAsync(Value);
        }
    }
}