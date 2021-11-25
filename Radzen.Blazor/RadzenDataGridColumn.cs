﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Radzen.Blazor
{
    /// <summary>
    /// RadzenDataGridColumn component.
    /// Must be placed inside a <see cref="RadzenDataGrid{TItem}" />
    /// </summary>
    /// <typeparam name="TItem">The type of the DataGrid item.</typeparam>
    public partial class RadzenDataGridColumn<TItem> : ComponentBase, IDisposable
    {
        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        [CascadingParameter]
        public RadzenDataGrid<TItem> Grid { get; set; }

        /// <summary>
        /// Called when initialized.
        /// </summary>
        protected override void OnInitialized()
        {
            if (Grid != null)
            {
                Grid.AddColumn(this);

                if (!string.IsNullOrEmpty(FilterProperty) || Type == null)
                {
                    var property = GetFilterProperty();

                    if (!string.IsNullOrEmpty(property))
                    {
                        _filterPropertyType = PropertyAccess.GetPropertyType(typeof(TItem), property);
                    }
                }
            
                if (_filterPropertyType == null)
                {
                    _filterPropertyType = Type;
                }
                else
                {
                    propertyValueGetter = PropertyAccess.Getter<TItem, object>(Property);
                }

                if (_filterPropertyType == typeof(string))
                {
                    FilterOperator = FilterOperator.Contains;
                }
            }
        }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        [Parameter]
        public SortOrder? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenDataGridColumn{TItem}"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Parameter]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        /// <value>The property name.</value>
        [Parameter]
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets the sort property name.
        /// </summary>
        /// <value>The sort property name.</value>
        [Parameter]
        public string SortProperty { get; set; }

        /// <summary>
        /// Gets or sets the group property name.
        /// </summary>
        /// <value>The group property name.</value>
        [Parameter]
        public string GroupProperty { get; set; }

        /// <summary>
        /// Gets or sets the filter property name.
        /// </summary>
        /// <value>The filter property name.</value>
        [Parameter]
        public string FilterProperty { get; set; }

        /// <summary>
        /// Gets or sets the filter value.
        /// </summary>
        /// <value>The filter value.</value>
        [Parameter]
        public object FilterValue { get; set; }

        /// <summary>
        /// Gets or sets the second filter value.
        /// </summary>
        /// <value>The second filter value.</value>
        [Parameter]
        public object SecondFilterValue { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [Parameter]
        public string Width { get; set; }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        [Parameter]
        public string FormatString { get; set; }

        /// <summary>
        /// Gets or sets the CSS class applied to data cells.
        /// </summary>
        /// <value>The CSS class applied to data cells.</value>
        [Parameter]
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the header CSS class applied to header cell.
        /// </summary>
        /// <value>The header CSS class applied to header cell.</value>
        [Parameter]
        public string HeaderCssClass { get; set; }

        /// <summary>
        /// Gets or sets the footer CSS class applied to footer cell.
        /// </summary>
        /// <value>The footer CSS class applied to footer cell.</value>
        [Parameter]
        public string FooterCssClass { get; set; }

        /// <summary>
        /// Gets or sets the group footer CSS class applied to group footer cell.
        /// </summary>
        /// <value>The group footer CSS class applied to group footer cell.</value>
        [Parameter]
        public string GroupFooterCssClass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenDataGridColumn{TItem}"/> is filterable.
        /// </summary>
        /// <value><c>true</c> if filterable; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Filterable { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenDataGridColumn{TItem}"/> is sortable.
        /// </summary>
        /// <value><c>true</c> if sortable; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Sortable { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenDataGridColumn{TItem}"/> is frozen.
        /// </summary>
        /// <value><c>true</c> if frozen; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Frozen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenDataGridColumn{TItem}"/> is resizable.
        /// </summary>
        /// <value><c>true</c> if resizable; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Resizable { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenDataGridColumn{TItem}"/> is reorderable.
        /// </summary>
        /// <value><c>true</c> if reorderable; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Reorderable { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadzenDataGridColumn{TItem}"/> is groupable.
        /// </summary>
        /// <value><c>true</c> if groupable; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Groupable { get; set; } = true;

        /// <summary>
        /// Gets or sets the text align.
        /// </summary>
        /// <value>The text align.</value>
        [Parameter]
        public TextAlign TextAlign { get; set; } = TextAlign.Left;

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        [Parameter]
        public RenderFragment<TItem> Template { get; set; }

        /// <summary>
        /// Gets or sets the edit template.
        /// </summary>
        /// <value>The edit template.</value>
        [Parameter]
        public RenderFragment<TItem> EditTemplate { get; set; }

        /// <summary>
        /// Gets or sets the header template.
        /// </summary>
        /// <value>The header template.</value>
        [Parameter]
        public RenderFragment HeaderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the footer template.
        /// </summary>
        /// <value>The footer template.</value>
        [Parameter]
        public RenderFragment FooterTemplate { get; set; }

        /// <summary>
        /// Gets or sets the group footer template.
        /// </summary>
        /// <value>The group footer template.</value>
        [Parameter]
        public RenderFragment<Group> GroupFooterTemplate { get; set; }

        /// <summary>
        /// Gets or sets the filter template.
        /// </summary>
        /// <value>The filter template.</value>
        [Parameter]
        public RenderFragment<RadzenDataGridColumn<TItem>> FilterTemplate { get; set; }

        /// <summary>
        /// Gets or sets the logical filter operator.
        /// </summary>
        /// <value>The logical filter operator.</value>
        [Parameter]
        public LogicalFilterOperator LogicalFilterOperator { get; set; } = LogicalFilterOperator.And;

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        /// <value>The data type.</value>
        [Parameter]
        public Type Type { get; set; }

        Func<TItem, object> propertyValueGetter;

        /// <summary>
        /// Gets the value for specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Object.</returns>
        public object GetValue(TItem item)
        {
            var value = propertyValueGetter != null && !string.IsNullOrEmpty(Property) && !Property.Contains('.') ? propertyValueGetter(item) : !string.IsNullOrEmpty(Property) ? PropertyAccess.GetValue(item, Property) : "";

            return !string.IsNullOrEmpty(FormatString) ? string.Format(FormatString, value, Grid?.Culture ?? CultureInfo.CurrentCulture) : Convert.ToString(value, Grid?.Culture ?? CultureInfo.CurrentCulture);
        }

        internal object GetHeader()
        {
            if (HeaderTemplate != null)
            {
                return HeaderTemplate;
            }
            else if (!string.IsNullOrEmpty(Title))
            {
                return Title;
            }
            else
            {
                return Property;
            }
        }

        /// <summary>
        /// Gets the cell style.
        /// </summary>
        /// <param name="forCell">if set to <c>true</c> [for cell].</param>
        /// <param name="isHeaderOrFooterCell">if set to <c>true</c> [is header or footer cell].</param>
        /// <returns>System.String.</returns>
        public string GetStyle(bool forCell = false, bool isHeaderOrFooterCell = false)
        {
            var style = new List<string>();

            var width = GetWidth();

            if (width != null)
            {
                style.Add($"width:{width}");
            }
            else if (Grid != null && Grid.ColumnWidth != null)
            {
                style.Add($"width:{Grid.ColumnWidth}");
            }

            if (forCell && TextAlign != TextAlign.Left)
            {
                style.Add($"text-align:{Enum.GetName(typeof(TextAlign), TextAlign).ToLower()};");
            }

            if (forCell && Frozen)
            {
                var left = Grid.ColumnsCollection
                    .TakeWhile((c, i) => Grid.ColumnsCollection.IndexOf(this) > i && c.Frozen)
                    .Sum(c => {
                        var w = !string.IsNullOrEmpty(c.GetWidth()) ? c.GetWidth() : Grid.ColumnWidth;
                        var cw = 200;
                        if (!string.IsNullOrEmpty(w) && w.Contains("px"))
                        {
                            int.TryParse(w.Replace("px", ""), out cw);
                        }
                        return cw;
                    });

                style.Add($"left:{left}px");
            }

            if ((isHeaderOrFooterCell && Frozen || isHeaderOrFooterCell && !Frozen || !isHeaderOrFooterCell && Frozen) && Grid.ColumnsCollection.Where(c => c.Visible && c.Frozen).Any())
            {
                style.Add($"z-index:{(isHeaderOrFooterCell && Frozen ? 2 : 1)}");
            }

            return string.Join(";", style);
        }

        /// <summary>
        /// Gets the sort property.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetSortProperty()
        {
            if (!string.IsNullOrEmpty(SortProperty))
            {
                return SortProperty;
            }
            else
            {
                return Property;
            }
        }

        internal string GetSortOrderAsString(bool isOData)
        {
            var property = GetSortProperty();
            if (string.IsNullOrEmpty(property))
                return "";
            var p = isOData ? property.Replace('.', '/') : PropertyAccess.GetProperty(property);
            return $"{p} {(GetSortOrder() == Radzen.SortOrder.Ascending ? "asc" : "desc")}";
        }

        internal void SetSortOrder(SortOrder? order)
        {
            sortOrder = new SortOrder?[] { order };
        }
        internal void ResetSortOrder()
        {
            sortOrder = Enumerable.Empty<SortOrder?>();
            SortOrder = null;
        }

        /// <summary>
        /// Gets the group property.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetGroupProperty()
        {
            if (!string.IsNullOrEmpty(GroupProperty))
            {
                return GroupProperty;
            }
            else
            {
                return Property;
            }
        }

        /// <summary>
        /// Gets the filter property.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetFilterProperty()
        {
            if (!string.IsNullOrEmpty(FilterProperty))
            {
                return FilterProperty;
            }
            else
            {
                return Property;
            }
        }

        Type _filterPropertyType;

        internal Type FilterPropertyType
        {
            get
            {
                return _filterPropertyType;
            }
        }

        IEnumerable<SortOrder?> sortOrder = Enumerable.Empty<SortOrder?>();
        object filterValue;
        FilterOperator? filterOperator;
        object secondFilterValue;
        FilterOperator? secondFilterOperator;
        LogicalFilterOperator? logicalFilterOperator;

        /// <summary>
        /// Set parameters as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.DidParameterChange(nameof(Visible), Visible) ||
                parameters.DidParameterChange(nameof(Title), Title))
            {
                if (Grid != null)
                {
                    await Grid.ChangeState();
                }
            }

            if (parameters.DidParameterChange(nameof(SortOrder), SortOrder))
            {
                sortOrder = new SortOrder?[] { parameters.GetValueOrDefault<SortOrder?>(nameof(SortOrder)) };
            }

            if (parameters.DidParameterChange(nameof(FilterValue), FilterValue))
            {
                filterValue = parameters.GetValueOrDefault<object>(nameof(FilterValue));
                if (FilterTemplate != null)
                {
                    FilterValue = filterValue;
                    await Grid.Reload();
                    return;
                }
            }

            if (parameters.DidParameterChange(nameof(FilterOperator), FilterOperator))
            {
                filterOperator = parameters.GetValueOrDefault<FilterOperator>(nameof(FilterOperator));
            }

            if (parameters.DidParameterChange(nameof(SecondFilterValue), SecondFilterValue))
            {
                secondFilterValue = parameters.GetValueOrDefault<object>(nameof(SecondFilterValue));
            }

            if (parameters.DidParameterChange(nameof(SecondFilterOperator), SecondFilterOperator))
            {
                secondFilterOperator = parameters.GetValueOrDefault<FilterOperator>(nameof(SecondFilterOperator));
            }

            if (parameters.DidParameterChange(nameof(LogicalFilterOperator), LogicalFilterOperator))
            {
                logicalFilterOperator = parameters.GetValueOrDefault<LogicalFilterOperator>(nameof(LogicalFilterOperator));
            }

            await base.SetParametersAsync(parameters);
        }

        internal SortOrder? GetSortOrder()
        {
            return sortOrder.Any() ? sortOrder.FirstOrDefault() : SortOrder;
        }

        internal object GetFilterValue()
        {
            return filterValue ?? FilterValue;
        }

        internal FilterOperator GetFilterOperator()
        {
            return filterOperator ?? FilterOperator;
        }

        internal object GetSecondFilterValue()
        {
            return secondFilterValue ?? SecondFilterValue;
        }

        internal FilterOperator GetSecondFilterOperator()
        {
            return secondFilterOperator ?? SecondFilterOperator;
        }

        internal LogicalFilterOperator GetLogicalFilterOperator()
        {
            return logicalFilterOperator ?? LogicalFilterOperator;
        }

        internal void SetFilterValue(object value, bool isFirst = true)
        {
            if ((FilterPropertyType == typeof(DateTimeOffset) || FilterPropertyType == typeof(DateTimeOffset?)) && value != null && value is DateTime?)
            {
                DateTimeOffset? offset = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
                value = offset;
            }

            if (isFirst)
            {
                filterValue = value;
            }
            else
            {
                secondFilterValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the filter operator.
        /// </summary>
        /// <value>The filter operator.</value>
        [Parameter]
        public FilterOperator FilterOperator { get; set; }

        /// <summary>
        /// Gets or sets the second filter operator.
        /// </summary>
        /// <value>The second filter operator.</value>
        [Parameter]
        public FilterOperator SecondFilterOperator { get; set; }

        internal void SetFilterOperator(FilterOperator? value)
        {
            filterOperator = value;
        }

        internal void SetSecondFilterOperator(FilterOperator? value)
        {
            secondFilterOperator = value;
        }

        internal void SetLogicalFilterOperator(LogicalFilterOperator value)
        {
            LogicalFilterOperator = value;
        }

        string runtimeWidth;

        internal void SetWidth(string value)
        {
            runtimeWidth = value;
        }

        internal string GetWidth()
        {
            return !string.IsNullOrEmpty(runtimeWidth) ? runtimeWidth : Width;
        }

        internal IEnumerable<FilterOperator> GetFilterOperators()
        {
            return Enum.GetValues(typeof(FilterOperator)).Cast<FilterOperator>().Where(o => {
                var isStringOperator = o == FilterOperator.Contains ||  o == FilterOperator.DoesNotContain || o == FilterOperator.StartsWith || o == FilterOperator.EndsWith;
                return FilterPropertyType == typeof(string) ? isStringOperator || o == FilterOperator.Equals || o == FilterOperator.NotEquals : !isStringOperator;
            });
        }

        internal string GetFilterOperatorText(FilterOperator filterOperator)
        {
            switch (filterOperator)
            {
                case FilterOperator.Contains:
                    return Grid?.ContainsText;
                case FilterOperator.DoesNotContain:
                    return Grid?.DoesNotContainText;
                case FilterOperator.EndsWith:
                    return Grid?.EndsWithText;
                case FilterOperator.Equals:
                    return Grid?.EqualsText;
                case FilterOperator.GreaterThan:
                    return Grid?.GreaterThanText;
                case FilterOperator. GreaterThanOrEquals:
                    return Grid?.GreaterThanOrEqualsText;
                case FilterOperator.LessThan:
                    return Grid?.LessThanText;
                case FilterOperator.LessThanOrEquals:
                    return Grid?.LessThanOrEqualsText;
                case FilterOperator.StartsWith:
                    return Grid?.StartsWithText;
                case FilterOperator.NotEquals:
                    return Grid?.NotEqualsText;
                default:
                    return $"{filterOperator}";
            }
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            Grid?.RemoveColumn(this);
        }
    }
}