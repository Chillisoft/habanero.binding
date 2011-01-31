using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Habanero.Base;
using Habanero.Faces.Base;
using Habanero.ProgramaticBinding;
using System.Windows.Forms;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.ProgrammaticBinding.Tester
{
    /// <summary>
    /// If any of these Asserts fail then an <see cref="AssertionException"/>. is thrown.
    /// Else the Assert executes without an Exception
    /// </summary>
    public class BindingTester<TBo> where TBo : class, IBusinessObject
    {
        private readonly HabaneroControlBinder<TBo> _habaneroControlBinder;

        public BindingTester(HabaneroControlBinder<TBo> habaneroControlBinder)
        {
            if (habaneroControlBinder == null) throw new ArgumentNullException("habaneroControlBinder");
            _habaneroControlBinder = habaneroControlBinder;
        }
        /// <summary>
        /// This merely asserts that the property is mapped to some control.
        /// It might not necessarily be mapped to the correct control.
        /// It also does not ensure that the prop is mapped to a valid property of the
        /// Business Object of Type TBo. Most types you should use <see cref="AssertPropBoundTo(System.Windows.Forms.Control,string)"/>
        /// </summary>
        /// <param name="propName">The property on the Business Object TBo that you are checking for</param>
        private void AssertPropBound(string propName)
        {
            var controlMapperForProp = GetControlMapperForProp(propName);
            var errorMssage = GetBaseMessage(propName) + " should be mapped";
            Assert.IsNotNull(controlMapperForProp, errorMssage);
        }

        private IControlMapper GetControlMapperForProp(string propName)
        {
            return _habaneroControlBinder.ControlMappers.FirstOrDefault(IsMapperForProp(propName));
        }

        /// <summary>
        /// This merely asserts that the property is mapped to some control.
        /// It might not necessarily be mapped to the correct control.
        /// It also does not ensure that the prop is mapped to a valid property of the
        /// Business Object of Type TBo. Most types you should use <see cref="AssertPropBoundTo(System.Windows.Forms.Control,string)"/>
        /// </summary>
        /// <param name="control">The Control you should be bound to</param>
        /// <param name="propName">The property on the Business Object TBo that you are checking for</param>
        public void AssertPropBoundTo(Control control, string propName)
        {
            this.AssertPropBound(propName);
            var controlMapperForProp = GetControlMapperForProp(propName);
            var errorMssage = GetBaseMessage(propName) + " should be mapped to '" + control.Name +"'";
            Assert.IsTrue(controlMapperForProp.Control.Equals(control), errorMssage);
        }

        /// <summary>
        /// This merely asserts that the property is mapped to some control.
        /// It might not necessarily be mapped to the correct control.
        /// It also does not ensure that the prop is mapped to a valid property of the
        /// Business Object of Type TBo. Most types you should use <see cref="AssertPropBoundTo(System.Windows.Forms.Control,string)"/>
        /// </summary>
        /// <param name="control">The Control you should be bound to</param>
        /// <param name="propExpression">The expression for the property on the Business Object TBo that you are checking for</param>
        public void AssertPropBoundTo(Control control, Expression<Func<TBo, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            AssertPropBoundTo(control, propertyName);
        }

        protected virtual string GetBaseMessage(string propName)
        {
            return string.Format("The Property '{0}' for class '{1}'", propName, ClassName);
        }

        private static string ClassName
        {
            get { return typeof (TBo).Name; }
        }

        private static Func<IControlMapper, bool> IsMapperForProp(string propName)
        {
            return mapper => mapper.IsForProperty(propName);
        }

    }
    public static class ControlMapperExtensions
    {
        public static bool IsForProperty(this IControlMapper controlMapper, string propName)
        {
            if (controlMapper == null) return false;
            return controlMapper.PropertyName == propName;
        }
    }
}

