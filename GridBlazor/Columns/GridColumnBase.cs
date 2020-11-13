using GridBlazor.Pages;
using GridShared;
using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Grouping;
using GridShared.OData;
using GridShared.Searching;
using GridShared.Sorting;
using GridShared.Totals;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace GridBlazor.Columns
{
    public abstract class GridColumnBase<T> : GridStyledColumn, IGridColumn<T>, IExpandColumn<T>, ICGridColumn, IConstrainedGridColumn
    {
        public Type ComponentType { get; private set; }
        public IList<Action<object>> Actions { get; private set; }
        public IList<Func<object, Task>> Functions { get; private set; }
        public object Object { get; private set; }

        public Type CreateComponentType { get; private set; }
        public Type ReadComponentType { get; private set; }
        public Type UpdateComponentType { get; private set; }
        public Type DeleteComponentType { get; private set; }
        public IList<Action<object>> CrudActions { get; private set; }
        public IList<Func<object, Task>> CrudFunctions { get; private set; }
        public object CrudObject { get; private set; }
        public bool EnableCard { get; private set; } = true;

        public Func<T, string> ValueConstraint { get; private set; }
        public string ValuePattern { get; private set; }

        #region IGridColumn<T> Members

        public bool EncodeEnabled { get; protected set; }
        public bool SanitizeEnabled { get; set; }

        public abstract string Width { get; set; }

        public int CrudWidth { get; set; } = 5;

        public int CrudLabelWidth { get; set; } = 2;

        public bool SortEnabled { get; protected set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string FieldName { get; protected set; }

        public bool IsSorted { get; set; }
        public GridSortDirection? Direction { get; set; }

        public GridSortDirection? InitialDirection { get; set; }

        public bool Hidden { get; set; }

        public bool? ExcelHidden { get; set; }

        public CrudHidden CrudHidden { get; protected set; } = CrudHidden.NONE;

        public bool ReadOnlyOnUpdate { get; protected set; } = false;

        public bool IsPrimaryKey { get; protected set; } = false;

        public bool IsAutoGeneratedKey { get; protected set; } = false;

        public string TabGroup { get; protected set; } = null;

        public bool HeaderCheckbox { get; protected set; } = false;

        public bool SingleCheckbox { get; protected set; } = false;

        public (bool IsSelectKey, Func<T, string> Expression, string Url, Func<IEnumerable<SelectItem>> SelectItemExpr, 
            Func<Task<IEnumerable<SelectItem>>> SelectItemExprAsync) IsSelectField { get; protected set; } 
            = (false, null, null, null, null);

        public IEnumerable<SelectItem> SelectItems { get; set; }

        public InputType InputType { get; protected set; }

        public bool MultipleInput { get; protected set; } = false;

        public bool IsSumEnabled { get; set; } = false;

        public bool IsAverageEnabled { get; set; } = false;

        public bool IsMaxEnabled { get; set; } = false;

        public bool IsMinEnabled { get; set; } = false;

        public string SumString { get; set; }

        public string AverageString { get; set; }

        public string MaxString { get; set; }

        public string MinString { get; set; }

        public (string,string)[] SubGridKeys { get; set; }

        public Func<object[], bool, bool, bool, bool, Task<IGrid>> SubGrids { get; set; }

        public string TooltipValue { get; set; }

        public IGridColumn<T> Titled(string title)
        {
            Title = title;
            return this;
        }

        public IGridColumn<T> Encoded(bool encode)
        {
            EncodeEnabled = encode;
            return this;
        }

        IGridColumn<T> IColumn<T>.SetWidth(string width)
        {
            Width = width;
            return this;
        }

        IGridColumn<T> IColumn<T>.SetCrudWidth(int width)
        {
            CrudWidth = width;
            return this;
        }

        IGridColumn<T> IColumn<T>.SetCrudWidth(int width, int labelWidth)
        {
            CrudWidth = width;
            CrudLabelWidth = labelWidth;
            return this;
        }

        IGridColumn<T> IColumn<T>.SetWidth(int width)
        {
            Width = width.ToString(CultureInfo.InvariantCulture) + "px";
            return this;
        }

        public IGridColumn<T> Css(string cssClasses)
        {
            AddCssClass(cssClasses);
            return this;
        }

        public IGridColumn<T> SetTabGroup(string tabGroup)
        {
            TabGroup = tabGroup;
            return this;
        }

        public IGridColumn<T> SetSingleCheckboxColumn()
        {
            SingleCheckbox = true;
            return SetCheckboxColumn(false, (T) => { return false; });
        }

        public IGridColumn<T> SetCheckboxColumn(bool headerCheckbox, Func<T, bool> expression)
        {
            if (string.IsNullOrWhiteSpace(Name))
                Name = Guid.NewGuid().ToString();
            HeaderCheckbox = headerCheckbox;
            return RenderComponentAs<CheckboxComponent<T>>((Name, expression));
        }

        public IGridColumn<T> SetCheckboxColumn(bool headerCheckbox, Func<T, bool> expression, Func<T, bool> readonlyExpr)
        {
            if (string.IsNullOrWhiteSpace(Name))
                Name = Guid.NewGuid().ToString();
            HeaderCheckbox = headerCheckbox;
            return RenderComponentAs<CheckboxComponent<T>>((Name, expression, readonlyExpr));
        }

        public IGridColumn<T> RenderValueAs(Func<T, string> constraint)
        {
            ValueConstraint = constraint;
            return this;
        }

        public IGridColumn<T> RenderComponentAs(Type componentType)
        {
            return RenderComponentAs(componentType, null, null);
        }

        public IGridColumn<T> RenderComponentAs(Type componentType, IList<Action<object>> actions)
        {
            return RenderComponentAs(componentType, actions, null);
        }

        public IGridColumn<T> RenderComponentAs(Type componentType, IList<Func<object, Task>> functions)
        {
            return RenderComponentAs(componentType, null, functions, null);
        }

        public IGridColumn<T> RenderComponentAs(Type componentType, IList<Action<object>> actions,
            IList<Func<object, Task>> functions)
        {
            return RenderComponentAs(componentType, actions, functions, null);
        }

        public IGridColumn<T> RenderComponentAs(Type componentType, object obj)
        {
            return RenderComponentAs(componentType, null, null, obj);
        }

        public IGridColumn<T> RenderComponentAs(Type componentType, IList<Action<object>> actions, object obj)
        {
            return RenderComponentAs(componentType, actions, null, obj);
        }

        public IGridColumn<T> RenderComponentAs(Type componentType, IList<Func<object, Task>> functions, object obj)
        {
            return RenderComponentAs(componentType, null, functions, obj);
        }

        public IGridColumn<T> RenderComponentAs(Type componentType, IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj)
        {
            /// Get type arguments from any <see cref="ICustomGridComponent<>"/> interface 
            /// in <paramref name="componentType"/> to make sure <see cref="T"/> is assignable to it
            List<Type> typeArgs = (from iType in componentType.GetInterfaces()
                                   where iType.IsGenericType && 
                                   iType.GetGenericTypeDefinition() == typeof(ICustomGridComponent<>)
                                   select iType.GetGenericArguments()[0]).ToList();
            
            if (typeArgs.Any(t => t.IsAssignableFrom(typeof(T))) &&
                componentType.IsSubclassOf(typeof(ComponentBase)))
            {
                ComponentType = componentType;
                Actions = actions;
                Functions = functions;
                Object = obj;
            }
            return this;
        }

        public IGridColumn<T> RenderComponentAs<TComponent>()
        {
            return RenderComponentAs<TComponent>(null, null, null);
        }

        public IGridColumn<T> RenderComponentAs<TComponent>(IList<Action<object>> actions)
        {
            return RenderComponentAs<TComponent>(actions, null, null);
        }

        public IGridColumn<T> RenderComponentAs<TComponent>(IList<Func<object, Task>> functions)
        {
            return RenderComponentAs<TComponent>(null, functions, null);
        }

        public IGridColumn<T> RenderComponentAs<TComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions)
        {
            return RenderComponentAs<TComponent>(actions, functions, null);
        }

        public IGridColumn<T> RenderComponentAs<TComponent>(object obj)
        {
            return RenderComponentAs<TComponent>(null, null, obj);
        }

        public IGridColumn<T> RenderComponentAs<TComponent>(IList<Action<object>> actions, object obj)
        {
            return RenderComponentAs<TComponent>(actions, null, obj);
        }

        public IGridColumn<T> RenderComponentAs<TComponent>(IList<Func<object, Task>> functions, object obj)
        {
            return RenderComponentAs<TComponent>(null, functions, obj);
        }

        public IGridColumn<T> RenderComponentAs<TComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj)
        {
            return RenderComponentAs(typeof(TComponent), actions, functions, obj);
        }

        public IGridColumn<T> RenderCrudComponentAs<TComponent>(bool enableCard = true)
        {
            return RenderCrudComponentAs<TComponent>(null, null, null, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TComponent>(IList<Action<object>> actions, bool enableCard = true)
        {
            return RenderCrudComponentAs<TComponent>(actions, null, null, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TComponent>(IList<Func<object, Task>> functions, bool enableCard = true)
        {
            return RenderCrudComponentAs<TComponent>(null, functions, null, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions, bool enableCard = true)
        {
            return RenderCrudComponentAs<TComponent>(actions, functions, null, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TComponent>(object obj, bool enableCard = true)
        {
            return RenderCrudComponentAs<TComponent>(null, null, obj, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TComponent>(IList<Action<object>> actions, object obj, bool enableCard = true)
        {
            return RenderCrudComponentAs<TComponent>(actions, null, obj, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TComponent>(IList<Func<object, Task>> functions, object obj, bool enableCard = true)
        {
            return RenderCrudComponentAs<TComponent>(null, functions, obj, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj, bool enableCard = true)
        {
            return RenderCrudComponentAs<TComponent, TComponent, TComponent, TComponent>(actions, functions, obj, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(bool enableCard = true)
        {
            return RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(null, null, null, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(IList<Action<object>> actions, bool enableCard = true)
        {
            return RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(actions, null, null, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(IList<Func<object, Task>> functions, bool enableCard = true)
        {
            return RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(null, functions, null, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions, bool enableCard = true)
        {
            return RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(actions, functions, null, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(object obj, bool enableCard = true)
        {
            return RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(null, null, obj, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(IList<Action<object>> actions, object obj, bool enableCard = true)
        {
            return RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(actions, null, obj, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(IList<Func<object, Task>> functions, object obj, bool enableCard = true)
        {
            return RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(null, functions, obj, enableCard);
        }

        public IGridColumn<T> RenderCrudComponentAs<TCreateComponent, TReadComponent, TUpdateComponent, TDeleteComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj, bool enableCard = true)
        {
            /// Get type arguments from any <see cref="ICustomGridComponent<>"/> interface 
            /// in <paramref name="componentType"/> to make sure <see cref="T"/> is assignable to it
            Type createComponentType = typeof(TCreateComponent);
            List<Type> createTypeArgs = (from iType in createComponentType.GetInterfaces()
                                   where iType.IsGenericType &&
                                   iType.GetGenericTypeDefinition() == typeof(ICustomGridComponent<>)
                                   select iType.GetGenericArguments()[0]).ToList();

            if (createTypeArgs.Any(t => t.IsAssignableFrom(typeof(T))) &&
                createComponentType.IsSubclassOf(typeof(ComponentBase)))
            {
                CreateComponentType = createComponentType;
            }

            /// Get type arguments from any <see cref="ICustomGridComponent<>"/> interface 
            /// in <paramref name="componentType"/> to make sure <see cref="T"/> is assignable to it
            Type readComponentType = typeof(TReadComponent);
            List<Type> readTypeArgs = (from iType in readComponentType.GetInterfaces()
                                   where iType.IsGenericType &&
                                   iType.GetGenericTypeDefinition() == typeof(ICustomGridComponent<>)
                                   select iType.GetGenericArguments()[0]).ToList();

            if (readTypeArgs.Any(t => t.IsAssignableFrom(typeof(T))) &&
                readComponentType.IsSubclassOf(typeof(ComponentBase)))
            {
                ReadComponentType = readComponentType;
            }

            /// Get type arguments from any <see cref="ICustomGridComponent<>"/> interface 
            /// in <paramref name="componentType"/> to make sure <see cref="T"/> is assignable to it
            Type updateComponentType = typeof(TUpdateComponent);
            List<Type> updateTypeArgs = (from iType in updateComponentType.GetInterfaces()
                                       where iType.IsGenericType &&
                                       iType.GetGenericTypeDefinition() == typeof(ICustomGridComponent<>)
                                       select iType.GetGenericArguments()[0]).ToList();

            if (updateTypeArgs.Any(t => t.IsAssignableFrom(typeof(T))) &&
                updateComponentType.IsSubclassOf(typeof(ComponentBase)))
            {
                UpdateComponentType = updateComponentType;
            }

            /// Get type arguments from any <see cref="ICustomGridComponent<>"/> interface 
            /// in <paramref name="componentType"/> to make sure <see cref="T"/> is assignable to it
            Type deleteComponentType = typeof(TDeleteComponent);
            List<Type> deleteTypeArgs = (from iType in deleteComponentType.GetInterfaces()
                                         where iType.IsGenericType &&
                                         iType.GetGenericTypeDefinition() == typeof(ICustomGridComponent<>)
                                         select iType.GetGenericArguments()[0]).ToList();

            if (deleteTypeArgs.Any(t => t.IsAssignableFrom(typeof(T))) &&
                deleteComponentType.IsSubclassOf(typeof(ComponentBase)))
            {
                DeleteComponentType = deleteComponentType;
            }

            CrudActions = actions;
            CrudFunctions = functions;
            CrudObject = obj;
            EnableCard = enableCard;

            return this;
        }

        public IGridColumn<T> Format(string pattern)
        {
            ValuePattern = pattern;
            return this;
        }

        public IGridColumn<T> Sum(bool enabled)
        {
            IsSumEnabled = enabled;
            return this;
        }

        public IGridColumn<T> Average(bool enabled)
        {
            IsAverageEnabled = enabled;
            return this;
        }

        public IGridColumn<T> Max(bool enabled)
        {
            IsMaxEnabled = enabled;
            return this;
        }

        public IGridColumn<T> Min(bool enabled)
        {
            IsMinEnabled = enabled;
            return this;
        }

        public IGridColumn<T> SetCrudHidden(bool create, bool read, bool update, bool delete)
        {
            if (create) CrudHidden |= CrudHidden.CREATE;
            if (read) CrudHidden |= CrudHidden.READ;
            if (update) CrudHidden |= CrudHidden.UPDATE;
            if (delete) CrudHidden |= CrudHidden.DELETE;

            return this;
        }

        public IGridColumn<T> SetExcelHidden(bool? excelHidden)
        {
            ExcelHidden = excelHidden;
            return this;
        }

        public IGridColumn<T> SetCrudHidden(bool all)
        {
            return SetCrudHidden(all, all, all, all);
        }

        public IGridColumn<T> SetReadOnlyOnUpdate(bool enabled)
        {
            ReadOnlyOnUpdate = enabled;
            return this;
        }

        public IGridColumn<T> SetPrimaryKey(bool enabled)
        {
            return SetPrimaryKey(enabled, true);
        }

        public IGridColumn<T> SetPrimaryKey(bool enabled, bool autoGenerated)
        {
            IsPrimaryKey = enabled;
            IsAutoGeneratedKey = autoGenerated;
            return this;
        }

        public IGridColumn<T> SetSelectField(bool enabled, Func<T, string> expression, Func<IEnumerable<SelectItem>> selectItemExpr)
        {
            IsSelectField = (enabled, expression, null, selectItemExpr, null);
            InputType = InputType.None;
            return this;
        }

        public IGridColumn<T> SetSelectField(bool enabled, Func<T, string> expression, Func<Task<IEnumerable<SelectItem>>> selectItemExprAsync)
        {
            IsSelectField = (enabled, expression, null, null, selectItemExprAsync);
            InputType = InputType.None;
            return this;
        }

        public IGridColumn<T> SetSelectField(bool enabled, Func<T, string> expression, string url)
        {
            IsSelectField = (enabled, expression, url, null, null);
            InputType = InputType.None;
            return this;
        }

        public IGridColumn<T> SetInputType(InputType inputType)
        {
            IsSelectField = (false, null, null, null, null);
            InputType = inputType;
            return this;
        }

        public IGridColumn<T> SetInputFileType(bool? multiple = null)
        {
            IsSelectField = (false, null, null, null, null);
            InputType = InputType.File;
            if (multiple.HasValue)
                MultipleInput = multiple.Value;
            if(string.IsNullOrWhiteSpace(Name))
                Name = Guid.NewGuid().ToString();
            if (string.IsNullOrWhiteSpace(FieldName))
                FieldName = Name;
            return this;
        }

        public abstract IGrid ParentGrid { get; }

        public virtual IGridColumn<T> Sanitized(bool sanitize)
        {
            SanitizeEnabled = sanitize;
            return this;
        }

        public IGridColumn<T> SetInitialFilter(GridFilterType type, string value)
        {
            var filter = new ColumnFilterValue
            {
                FilterType = type,
                FilterValue = value,
                ColumnName = Name
            };
            InitialFilterSettings = filter;
            return this;
        }

        public abstract IGridColumn<T> SortInitialDirection(GridSortDirection direction);

        public abstract IGridColumn<T> ThenSortBy<TKey>(Expression<Func<T, TKey>> expression);
        public abstract IGridColumn<T> ThenSortBy<TKey>(Expression<Func<T, TKey>> expression, IComparer<TKey> comparer);
        public abstract IGridColumn<T> ThenSortByDescending<TKey>(Expression<Func<T, TKey>> expression);
        public abstract IGridColumn<T> ThenSortByDescending<TKey>(Expression<Func<T, TKey>> expression, IComparer<TKey> comparer);

        public abstract IEnumerable<IColumnOrderer<T>> Orderers { get; }
        public abstract IGridColumn<T> Sortable(bool sort);

        public abstract IGridCell GetCell(object instance);

        public string GetFormatedValue(object value)
        {
            if (value == null)
                return null;
            string textValue;
            if (!string.IsNullOrEmpty(ValuePattern))
                textValue = string.Format(ValuePattern, value);
            else
                textValue = value.ToString();
            return textValue;
        }

        public string GetFormatedDateTime(object value, string type)
        {
            if (value == null)
                return null;
            string textValue;
            if(type == "date")
                textValue = string.Format("{0:yyyy-MM-dd}", value);
            else if (type == "time")
                textValue = string.Format("{0:HH:mm}", value);
            else if (type == "datetime-local")
                textValue = string.Format("{0:yyyy-MM-ddTHH:mm}", value);
            else if (type == "week")
                textValue = string.Format("{0:yyyy}-W{1}", value, DateTimeUtils.GetIso8601WeekOfYear(value));
            else if (type == "month")
                textValue = string.Format("{0:yyyy-MM}", value);
            else if (!string.IsNullOrEmpty(ValuePattern))
                textValue = string.Format(ValuePattern, value);
            else
                textValue = value.ToString();
            return textValue;
        }

        public string GetFormatedValue(Func<T, string> expression, object value)
        {
            if (value == null)
                return null;
            if (typeof(T) == value.GetType())
            {
                return expression.Invoke((T)value);
            }
            return null;
        }

        public (Type Type, object Value) GetTypeAndValue(T item)
        {
            PropertyInfo pi = null;
            var type = item.GetType();
            object value = item;
            if (FieldName != null)
            {
                var names = FieldName.Split('.');
                for (int i = 0; i < names.Length; i++)
                {
                    pi = type.GetProperty(names[i]);
                    if (pi == null)
                        return (null, null);

                    bool isNullable = pi.PropertyType.GetTypeInfo().IsGenericType &&
                        pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
                    type = isNullable ? Nullable.GetUnderlyingType(pi.PropertyType) : pi.PropertyType;

                    if (value != null)
                    {
                        value = pi.GetValue(value, null);
                    }
                }
            }
            return (type, value);
        }

        public abstract bool FilterEnabled { get; set; }

        public ColumnFilterValue InitialFilterSettings { get; set; }

        public abstract IGridColumn<T> Filterable(bool showColumnValuesVariants);


        public abstract IGridColumn<T> SetFilterWidgetType(string typeName);
        public abstract IGridColumn<T> SetFilterWidgetType(string typeName, object widgetData);

        public abstract IGridColumn<T> SetListFilter(IEnumerable<SelectItem> selectItems);

        public abstract IGridColumn<T> SetCellCssClassesContraint(Func<T, string> contraint);
        public abstract string GetCellCssClasses(object item);

        public abstract IColumnFilter Filter { get; }

        public abstract string FilterWidgetTypeName { get; }
        public object FilterWidgetData { get; protected set; }

        public abstract IColumnSearch<T> Search { get; }

        public abstract IColumnGroup<T> Group { get; }

        public abstract IColumnExpand<T> Expand { get; }

        public abstract IGridCell GetValue(T instance);

        public IGridColumn<T> SubGrid(Func<object[], bool, bool, bool, bool, Task<IGrid>> subGrids, params (string,string)[] keys)
        {
            return SubGrid(null, subGrids, keys);
        }

        public IGridColumn<T> SubGrid(string tabGroup, Func<object[], bool, bool, bool, bool, Task<IGrid>> subGrids, params (string, string)[] keys)
        {
            Hidden = true;
            TabGroup = tabGroup;
            SubGrids = subGrids;
            SubGridKeys = keys;
            return this;
        }

        public QueryDictionary<object> GetSubGridKeyValues(object item)
        {
            QueryDictionary<object> values = new QueryDictionary<object>();
            foreach (var key in SubGridKeys)
            {
                var value = item.GetType().GetProperty(key.Item1).GetValue(item);
                values.Add(key.Item2, value);
            }
            return values;
        }

        public IGridColumn<T> SetTooltip(string value)
        {
            TooltipValue = value;
            return this;
        }

        #endregion

        #region IConstrainedGridColumn Members

        public abstract bool HasConstraint { get; }

        #endregion

        public IColumnTotals<T> Totals { get; }
    }
}
