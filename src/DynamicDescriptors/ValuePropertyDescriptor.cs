using System;
using System.ComponentModel;

namespace DynamicDescriptors;

public class ValuePropertyDescriptor
{
    public static PropertyDescriptor Dynamic(string name, object initialValue, Type type)
    {
        if (type == typeof(bool)) return new ValuePropertyDescriptor<bool>(name, (bool)initialValue);
        if (type == typeof(int)) return new ValuePropertyDescriptor<int>(name, (int)initialValue);
        if (type == typeof(string)) return new ValuePropertyDescriptor<string>(name, (string)initialValue);

        return new ValuePropertyDescriptor<string>(name, (string)initialValue);
    }
}

public sealed class ValuePropertyDescriptor<T> : PropertyDescriptor
{
    private T _value;

    public ValuePropertyDescriptor(string name, T initialValue, Attribute[] attrs = null)
        : base(name, attrs)
    {
        _value = initialValue;
    }

    /// <summary>
    /// Returns a value indicating returns whether resetting an object changes its value.
    /// </summary>
    /// <param name="component">The component to test for reset capability.</param>
    /// <returns>true if resetting the component changes its value; otherwise, false.</returns>
    public override bool CanResetValue(object component) => false;

    /// <summary>
    /// Gets the type of the component this property is bound to.
    /// </summary>
    public override Type ComponentType => null;

    /// <summary>
    /// Returns the current value of the property on a component.
    /// </summary>
    /// <param name="component">
    /// The component with the property for which to retrieve the value.
    /// </param>
    /// <returns>The value of a property for a given component.</returns>
    public override object GetValue(object component) => _value;

    /// <summary>
    /// Gets a value indicating whether this property is read-only.
    /// </summary>
    public override bool IsReadOnly => false;

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    public override Type PropertyType => typeof(T);

    /// <summary>
    /// Resets the value for this property of the component to the default value.
    /// </summary>
    /// <param name="component">
    /// The component with the property value that is to be reset to the default value.
    /// </param>
    public override void ResetValue(object component)
    {
        _value = default;
        OnValueChanged(component, EventArgs.Empty);
    }

    /// <summary>
    /// Sets the value of the component to a different value.
    /// </summary>
    /// <param name="component">
    /// The component with the property value that is to be set.
    /// </param>
    /// <param name="value">
    /// The new value.
    /// </param>
    public override void SetValue(object component, object value)
    {
        _value = (T)value;
        OnValueChanged(component, EventArgs.Empty);
    }

    /// <summary>
    /// Determines a value indicating whether the value of this property needs to be
    /// persisted.
    /// </summary>
    /// <param name="component">
    /// The component with the property to be examined for persistence.
    /// </param>
    /// <returns>true if the property should be persisted; otherwise, false.</returns>
    public override bool ShouldSerializeValue(object component) => false;
}
