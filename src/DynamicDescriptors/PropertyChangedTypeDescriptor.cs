using System.ComponentModel;

namespace DynamicDescriptors;

public class PropertyChangedTypeDescriptor : CustomTypeDescriptor, ICustomTypeDescriptor, INotifyPropertyChanged
{
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public PropertyChangedTypeDescriptor() : base() { }
    public PropertyChangedTypeDescriptor(ICustomTypeDescriptor parent) : base(parent) { }
}
