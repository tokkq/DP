using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;

namespace DailyProject_221204
{
    public class AbstractCustomControl : Control
    {
        protected void _setBinding<TControlType>(string controlName, DependencyProperty dependencyProperty, string propertyName, BindingMode bindingMode = BindingMode.Default)
            where TControlType : FrameworkElement
        {
            var binding = new Binding(propertyName)
            {
                Source = this,
                Mode = bindingMode,
            };
            _setBinding<TControlType>(controlName, dependencyProperty, binding);
        }

        protected void _setBinding<TControlType>(string controlName, DependencyProperty dependencyProperty, Binding binding)
            where TControlType : FrameworkElement
        {
            var control = _getElement<TControlType>(controlName);
            control.SetBinding(dependencyProperty, binding);
        }

        protected TElementType _getElement<TElementType>(string elementName)
            where TElementType : FrameworkElement
        {
            var element = (TElementType)GetTemplateChild(elementName);
            if (element != null)
            {
                return element;
            }
            else
            {
                throw new InvalidCastException($"[elementName: {elementName}][element: {element}][TControlType: {typeof(TElementType)}]elementNameから要素をキャストできませんでした。");
            }
        }
    }
}
