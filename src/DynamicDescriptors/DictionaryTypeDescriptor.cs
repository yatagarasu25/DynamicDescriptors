using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DynamicDescriptors;

/// <summary>
/// A dictionary-backed implementation of <see cref="ICustomTypeDescriptor"/>.
/// </summary>
public class DictionaryTypeDescriptor : PropertyChangedTypeDescriptor
{
    /// <summary>
    /// A dictionary mapping property names to property values.
    /// </summary>
    protected readonly IDictionary<string, object> _data;

    /// <summary>
    /// A list containing the properties associated with this type descriptor.
    /// </summary>
    protected readonly List<DictionaryPropertyDescriptor> _propertyDescriptors;

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryTypeDescriptor"/> class.
    /// </summary>
    /// <param name="data">A dictionary mapping property names to property values.</param>
    public DictionaryTypeDescriptor(IDictionary<string, object> data)
        : this(data, null) { }

    public DictionaryTypeDescriptor(IDictionary<string, (object, Type)> data)
        : this(data.ToDictionary(k => k.Key, k => (k.Value.Item1, k.Value.Item2, (Attribute[])null)))
    {
    }
    public DictionaryTypeDescriptor(IDictionary<string, (object, Type, Attribute[])> data)
    {
        Preconditions.CheckNotNull(data, nameof(data));

        _data = data.ToDictionary(i => i.Key, i => i.Value.Item1);
        _propertyDescriptors = new List<DictionaryPropertyDescriptor>();

        foreach (var pair in data)
        {
            var propertyDescriptor = new DictionaryPropertyDescriptor(_data, pair.Key, pair.Value.Item2, pair.Value.Item3);
            propertyDescriptor.AddValueChanged(this, (s, e) => OnPropertyChanged(pair.Key));

            _propertyDescriptors.Add(propertyDescriptor);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryTypeDescriptor"/> class.
    /// </summary>
    /// <param name="data">A dictionary mapping property names to property values.</param>
    /// <param name="types">A dictionary mapping property names to property types.</param>
    public DictionaryTypeDescriptor(IDictionary<string, object> data, IDictionary<string, Type> types)
    {
        Preconditions.CheckNotNull(data, nameof(data));

        _data = data;
        _propertyDescriptors = new List<DictionaryPropertyDescriptor>();

        foreach (var pair in _data)
        {
            if (types == null || !types.TryGetValue(pair.Key, out var type))
            {
                type = pair.Value?.GetType() ?? typeof(object);
            }

            var propertyDescriptor = new DictionaryPropertyDescriptor(data, pair.Key, type);
            propertyDescriptor.AddValueChanged(this, (s, e) => OnPropertyChanged(pair.Key));

            _propertyDescriptors.Add(propertyDescriptor);
        }
    }

    /// <summary>
    /// Returns an object that contains the property described by the specified property
    /// descriptor.
    /// </summary>
    /// <param name="pd">
    /// A <see cref="PropertyDescriptor"/> that represents the property whose owner is to be found.
    /// </param>
    /// <returns>An object that represents the owner of the specified property.</returns>
    public override object GetPropertyOwner(PropertyDescriptor pd) => this;

    /// <summary>
    /// Returns a collection of property descriptors for the object represented by this type
    /// descriptor.
    /// </summary>
    /// <returns>
    /// A <see cref="PropertyDescriptorCollection"/> containing the property descriptions for the object
    /// represented by this type descriptor.
    /// </returns>
    public override PropertyDescriptorCollection GetProperties() => GetProperties(null);

    /// <summary>
    /// Returns a collection of property descriptors for the object represented by this type
    /// descriptor.
    /// </summary>
    /// <param name="attributes">
    /// An array of attributes to use as a filter. This can be null.
    /// </param>
    /// <returns>
    /// A <see cref="PropertyDescriptorCollection"/> containing the property descriptions for the object
    /// represented by this type descriptor.
    /// </returns>
    public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
        return new PropertyDescriptorCollection(_propertyDescriptors.ToArray());
    }
}
