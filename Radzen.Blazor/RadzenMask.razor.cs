﻿using Microsoft.AspNetCore.Components;

namespace Radzen.Blazor
{
    /// <summary>
    /// RadzenMask component.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;RadzenMask Mask="(***) ***-****" Pattern="[^0-9]" Placeholder="(000) 000-0000" @bind-Value=@phone Change=@(args => Console.WriteLine($"Value: {args}")) /&gt;
    /// </code>
    /// </example>
    public partial class RadzenMask : FormComponent<string>
    {
        /// <summary>
        /// Gets or sets a value indicating whether is read only.
        /// </summary>
        /// <value><c>true</c> if is read only; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether input automatic complete is enabled.
        /// </summary>
        /// <value><c>true</c> if input automatic complete is enabled; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool AutoComplete { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>The maximum length.</value>
        [Parameter]
        public long? MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the mask.
        /// </summary>
        /// <value>The mask.</value>
        [Parameter]
        public string Mask { get; set; }

        /// <summary>
        /// Gets or sets the pattern that will be used to replace all invalid characters with regular expression.
        /// </summary>
        /// <value>The invalid characters pattern.</value>
        [Parameter]
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the pattern that will be used to match all valid characters with regular expression. If both Pattern and CharacterPattern are set CharacterPattern will be used.
        /// </summary>
        /// <value>The valid characters pattern.</value>
        [Parameter]
        public string CharacterPattern { get; set; }

        /// <summary>
        /// Handles the <see cref="E:Change" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ChangeEventArgs"/> instance containing the event data.</param>
        protected async System.Threading.Tasks.Task OnChange(ChangeEventArgs args)
        {
            Value = $"{args.Value}";

            await ValueChanged.InvokeAsync(Value);
            if (FieldIdentifier.FieldName != null) { EditContext?.NotifyFieldChanged(FieldIdentifier); }
            await Change.InvokeAsync(Value);
        }

        /// <inheritdoc />
        protected override string GetComponentCssClass()
        {
            return GetClassList("rz-textbox").ToString();
        }
    }
}
