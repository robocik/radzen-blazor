using Microsoft.AspNetCore.Components;
using Radzen.Blazor.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Radzen.Blazor
{
    /// <summary>
    /// A component which is an item in a <see cref="RadzenTree" />
    /// </summary>
    public partial class RadzenTreeItem : IDisposable
    {
        ClassList ContentClassList => ClassList.Create("rz-treenode-content")
                                               .Add("rz-treenode-content-selected", selected);
        ClassList IconClassList => ClassList.Create("rz-tree-toggler rzi")
                                               .Add("rzi-caret-down", expanded)
                                               .Add("rzi-caret-right", !expanded);
        /// <summary>
        /// Gets or sets the child content.
        /// </summary>
        /// <value>The child content.</value>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the template. Use it to customize the appearance of a tree item.
        /// </summary>
        [Parameter]
        public RenderFragment<RadzenTreeItem> Template { get; set; }

        /// <summary>
        /// Gets or sets the text displayed by the tree item.
        /// </summary>
        [Parameter]
        public string Text { get; set; }

        private bool expanded;

        /// <summary>
        /// Specifies whether this item is expanded. Set to <c>false</c> by default.
        /// </summary>
        [Parameter]
        public bool Expanded { get; set; }

        /// <summary>
        /// Gets or sets the value of the tree item.
        /// </summary>
        [Parameter]
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has children.
        /// </summary>
        [Parameter]
        public bool HasChildren { get; set; }

        private bool selected;

        /// <summary>
        /// Specifies whether this item is selected or not. Set to <c>false</c> by default.
        /// </summary>
        [Parameter]
        public bool Selected { get; set; }

        /// <summary>
        /// The RadzenTree which this item is part of.
        /// </summary>
        [CascadingParameter]
        public RadzenTree Tree { get; set; }

        /// <summary>
        /// The RadzenTreeItem which contains this item.
        /// </summary>
        [CascadingParameter]
        public RadzenTreeItem ParentItem { get; set; }

        internal List<RadzenTreeItem> items = new List<RadzenTreeItem>();

        internal void AddItem(RadzenTreeItem item)
        {
            if (items.IndexOf(item) == -1)
            {
                items.Add(item);
            }
        }

        internal void RemoveItem(RadzenTreeItem item)
        {
            if (items.IndexOf(item) != -1)
            {
                items.Remove(item);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (ParentItem != null)
            {
                ParentItem.RemoveItem(this);
            }
            else if (Tree != null)
            {
                Tree.RemoveItem(this);
            }
        }

        internal async Task Toggle()
        {
            expanded = !expanded;

            if (expanded)
            {
                if (Tree != null)
                {
                    await Tree.ExpandItem(this);

                    if (Tree.SingleExpand)
                    {
                        var siblings = ParentItem?.items ?? Tree.items;

                        foreach (var sibling in siblings)
                        {
                            if (sibling != this && sibling.expanded)
                            {
                                await sibling.Toggle();
                            }
                        }
                    }
                }
            }
        }

        void Select()
        {
            selected = true;
            Tree?.SelectItem(this);
        }

        internal void Unselect()
        {
            selected = false;
            StateHasChanged();
        }

        internal void RenderChildContent(RenderFragment content)
        {
            ChildContent = content;
        }

        /// <inheritdoc />
        override protected void OnInitialized()
        {
            expanded = Expanded;

            if (expanded)
            {
                Tree?.ExpandItem(this);
            }

            selected = Selected;

            if (selected)
            {
                Tree?.SelectItem(this);
            }

            if (Tree != null && ParentItem == null)
            {
                Tree.AddItem(this);
            }

            if (ParentItem != null)
            {
                ParentItem.AddItem(this);
            }
        }

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var shouldExpand = false;

            if (parameters.DidParameterChange(nameof(Expanded), Expanded))
            {
                // The Expanded property has changed - update the expanded state
                expanded = parameters.GetValueOrDefault<bool>(nameof(Expanded));
                shouldExpand = true;
            }

            if (parameters.DidParameterChange(nameof(Value), Value))
            {
                // The Value property has changed - the children may have also changed
                shouldExpand = expanded;
            }

            if (shouldExpand)
            {
                // Either the expanded state or Value changed - expand the node to render its children
                Tree?.ExpandItem(this);
            }

            if (parameters.DidParameterChange(nameof(Selected), Selected))
            {
                selected = parameters.GetValueOrDefault<bool>(nameof(Selected));

                if (selected)
                {
                    Tree?.SelectItem(this);
                }
            }

            await base.SetParametersAsync(parameters);
        }

        async Task CheckedChange(bool? value)
        {
            if (Tree != null)
            {
                var checkedValues = GetCheckedValues();

                if (Tree.AllowCheckChildren)
                {
                    if (value == true)
                    {
                        var valueAndChildren = GetValueAndAllChildValues();
                        checkedValues = checkedValues.Union(valueAndChildren);
                        Tree.SetUncheckedValues(Tree.UncheckedValues.Except(valueAndChildren));
                    }
                    else
                    {
                        var valueAndChildren = GetValueAndAllChildValues();
                        checkedValues = checkedValues.Except(valueAndChildren);
                        Tree.SetUncheckedValues(valueAndChildren.Union(Tree.UncheckedValues));
                    }
                }
                else
                {
                    if (value == true)
                    {
                        var valueWithoutChildren = new[] { Value };
                        checkedValues = checkedValues.Union(valueWithoutChildren);
                        Tree.SetUncheckedValues(Tree.UncheckedValues.Except(valueWithoutChildren));
                    }
                    else
                    {
                        var valueWithoutChildren = new[] { Value };
                        checkedValues = checkedValues.Except(valueWithoutChildren);
                        Tree.SetUncheckedValues(valueWithoutChildren.Union(Tree.UncheckedValues));
                    }
                }

                if (Tree.AllowCheckParents)
                {
                    checkedValues = UpdateCheckedValuesWithParents(checkedValues, value);
                }

                await Tree.SetCheckedValues(checkedValues);
            }
        }

        bool? IsChecked()
        {
            var checkedValues = GetCheckedValues();

            if (HasChildren && IsOneChildUnchecked() && IsOneChildChecked())
            {
                return null;
            }

            return checkedValues.Contains(Value);
        }

        IEnumerable<object> GetCheckedValues()
        {
            return Tree.CheckedValues != null ? Tree.CheckedValues : Enumerable.Empty<object>();
        }

        IEnumerable<object> GetAllChildValues(Func<object, bool> predicate = null)
        {
            var children = items.Concat(items.SelectManyRecursive(i => i.items)).Select(i => i.Value);

            return predicate != null ? children.Where(predicate) : children;
        }

        IEnumerable<object> GetValueAndAllChildValues()
        {
            return new object[] { Value }.Concat(GetAllChildValues());
        }

        bool AreAllChildrenChecked(Func<object, bool> predicate = null)
        {
            var checkedValues = GetCheckedValues();
            return GetAllChildValues(predicate).All(i => checkedValues.Contains(i));
        }

        bool AreAllChildrenUnchecked(Func<object, bool> predicate = null)
        {
            var checkedValues = GetCheckedValues();
            return GetAllChildValues(predicate).All(i => !checkedValues.Contains(i));
        }

        bool IsOneChildUnchecked()
        {
            var checkedValues = GetCheckedValues();
            return GetAllChildValues().Any(i => !checkedValues.Contains(i));
        }

        bool IsOneChildChecked()
        {
            var checkedValues = GetCheckedValues();
            return GetAllChildValues().Any(i => checkedValues.Contains(i));
        }

        IEnumerable<object> UpdateCheckedValuesWithParents(IEnumerable<object> checkedValues, bool? value)
        {
            var p = ParentItem;
            while (p != null)
            {
                if (value == false && p.AreAllChildrenUnchecked(i => !object.Equals(i, Value)))
                {
                    checkedValues = checkedValues.Except(new object[] { p.Value });
                }
                else if (value == true && p.AreAllChildrenChecked(i => !object.Equals(i, Value)))
                {
                    checkedValues = checkedValues.Union(new object[] { p.Value });
                }

                p = p.ParentItem;
            }

            return checkedValues;
        }

        internal bool Contains(RadzenTreeItem child)
        {
            var parent = child.ParentItem;

            while (parent != null)
            {
                if (parent == this)
                {
                    return true;
                }

                parent = parent.ParentItem;
            }

            return false;
        }
    }
}
