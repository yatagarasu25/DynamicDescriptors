﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicDescriptors;

/// <summary>
/// A runtime-customizable implementation of <see cref="ICustomTypeDescriptor"/>.
/// </summary>
public class DynamicTypeDescriptor : PropertyChangedTypeDescriptor
{
    /// <summary>
    /// A list containing the properties associated with this type descriptor.
    /// </summary>
    private readonly IList<DynamicPropertyDescriptor> _dynamicProperties;

    /// <summary>
    /// Comparer to use when sorting a list of dynamic property descriptors.
    /// </summary>
    private readonly IComparer<DynamicPropertyDescriptor> _comparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicTypeDescriptor"/> class.
    /// </summary>
    /// <param name="parent">The parent custom type descriptor.</param>
    public DynamicTypeDescriptor(ICustomTypeDescriptor parent)
        : base(Preconditions.CheckNotNull(parent, nameof(parent)))
    {
        _dynamicProperties = new List<DynamicPropertyDescriptor>();
        _comparer = new DynamicPropertyDescriptorComparer();

        foreach (PropertyDescriptor propertyDescriptor in base.GetProperties())
        {
            var dynamicPropertyDescriptor = new DynamicPropertyDescriptor(propertyDescriptor);
            dynamicPropertyDescriptor.AddValueChanged(this, (s, e) => OnPropertyChanged(propertyDescriptor.Name));

            _dynamicProperties.Add(dynamicPropertyDescriptor);
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
    /// A <see cref="PropertyDescriptorCollection"/> containing the property descriptions
    /// for the object represented by this type descriptor.
    /// </returns>
    public override PropertyDescriptorCollection GetProperties()
    {
        return GetProperties(null);
    }

    /// <summary>
    /// Returns a collection of property descriptors for the object represented by this type
    /// descriptor.
    /// </summary>
    /// <param name="attributes">
    /// An array of attributes to use as a filter. This can be null.
    /// </param>
    /// <returns>
    /// A <see cref="PropertyDescriptorCollection"/> containing the property descriptions
    /// for the object represented by this type descriptor.
    /// </returns>
    public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
        var properties = _dynamicProperties
            .Where(p => attributes == null || p.Attributes.Contains(attributes))
            .ToArray();

        Array.Sort(properties, _comparer);

        return new PropertyDescriptorCollection(properties);
    }

    /// <summary>
    /// Returns the specified dynamic property descriptor for the object represented by this
    /// type descriptor.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>
    /// The specified dynamic property descriptor for the object represented by this type
    /// descriptor.
    /// </returns>
    public DynamicPropertyDescriptor GetDynamicProperty(string propertyName)
    {
        foreach (var property in _dynamicProperties)
        {
            if (string.Equals(property.Name, propertyName))
            {
                return property;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns the specified dynamic property descriptor for the object represented by this
    /// type descriptor.
    /// </summary>
    /// <typeparam name="TSource">Type containing the property.</typeparam>
    /// <typeparam name="TProperty">Type of the property.</typeparam>
    /// <param name="propertyExpression">
    /// An expression representing a function mapping an instance of type TSource to a
    /// property of type TProperty.
    /// </param>
    /// <returns>
    /// The specified dynamic property descriptor for the object represented by this type
    /// descriptor.
    /// </returns>
    public DynamicPropertyDescriptor GetDynamicProperty<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyExpression)
    {
        var propertyName = Reflect.GetPropertyName(propertyExpression);
        return GetDynamicProperty(propertyName);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the sequence containing all dynamic
    /// property descriptors for the object represented by this type descriptor.
    /// </summary>
    /// <returns>
    /// An enumerator that iterates through the sequence containing all dynamic property
    /// descriptors for the object represented by this type descriptor.
    /// </returns>
    public IEnumerable<DynamicPropertyDescriptor> GetDynamicProperties()
    {
        // This should return all dynamic properties, even those that are inactive.

        return _dynamicProperties
            .OrderBy(x => x.PropertyOrder ?? int.MaxValue)
            .ToArray();
    }
}
