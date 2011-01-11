// ------------------------------------------------------------------------------
// This partial class was auto-generated for use with the Habanero Architecture.
// NB: This class is regenerated and you run the risk of losing any changes
// you make directly.  Try placing any custom code in the non "Def" code file.
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using BindingListViewSample.BO;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;
using Habanero.Testability;

namespace BindingListViewSample.Test.BO
{
    /// <summary>
    ///Creates sample Employee objects.
    /// If the default <see cref="BOTestFactory{T}"/> Creates invalid objects then uncomment the custom 
    /// factory (see Other part of this partial class). See the comments on <see cref="BOTestFactory{T}"/>
    /// for more details.
    /// </summary>
	public partial class TestUtilsEmployee
    {
		
        /// <summary>
        /// Creates a new saved Employee with valid random value assigned to all compulsoryProps
        /// <see cref="BOTestFactory"/>.<see cref="BOTestFactory.CreateValidBusinessObject"/>
        /// </summary>
        public static Employee CreateSavedEmployee()
		{
		    Employee employee = CreateUnsavedValidEmployee();
			employee.Save();
			return employee;
		}

        /// <summary>
        /// Creates a new unsaved Employee with a random value assigned to all compulsory props
        ///  <see cref="BOTestFactory"/>.<see cref="BOTestFactory.CreateValidBusinessObject"/>
        /// </summary>
		public static Employee CreateUnsavedValidEmployee()
		{
			return _factory.CreateValidBusinessObject();
		}
	    
	    /// <summary>
        /// Creates a new unsaved Employee where all properties are null, except ID properties
        /// and those with default values in the <see cref="IClassDef"/>.<see cref="IPropDef"/> rules.  
        /// If there are compulsory properties without
        /// defaults, saving the object will throw an exception if you try save it.<br/>
        ///  <see cref="BOTestFactory"/>.<see cref="BOTestFactory.CreateDefaultBusinessObject"/>
        /// </summary>
		public static Employee CreateUnsavedDefaultEmployee()
		{
			return _factory.CreateDefaultBusinessObject();

		}
		
		/// <summary>
        /// Updates the compulsory properties for an Employee with valid values.<br/>
        /// <see cref="BOTestFactory"/>.<see cref="BOTestFactory.UpdateCompulsoryProperties"/>
        /// </summary>
        /// <param name="employee"></param>
        public static void UpdateCompulsoryProperties(Employee employee)
        {
            _factory.UpdateCompulsoryProperties(employee);
        }

    }
}