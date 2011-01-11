// ------------------------------------------------------------------------------
// This partial class was auto-generated for use with the Habanero Architecture.
// This class is generated only once and you can place custom code here.
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
    /// Creates sample Employee objects.
    /// </summary>
	public partial class TestUtilsEmployee
    {
		private static readonly BOTestFactory<Employee> _factory = GetTestFactory();
        //---------------------------------------------------------------------
        // Use this place to add custom test packs for Employee
        // and to replace existing test packs in the "Def" part of this
        // partial class (cut the existing method and paste it here - you will
        // need to do this each time you regenerate).
        //---------------------------------------------------------------------

        public static BOTestFactory<Employee> GetTestFactory()
        {
            return BOTestFactoryRegistry.Instance.Resolve<Employee>();
        }
    }
    
  /*//Uncomment this and create custom code in the UpdateCompulsoryProperties
    // if there is any custom code required to create a valid Employee
    public class BOTestFactoryEmployee : BOTestFactory<Employee>
    {
        public BOTestFactoryEmployee(Employee employee)
            : base(employee)
        {
        }

        public BOTestFactoryEmployee()
        {
        }

        public override void UpdateCompulsoryProperties(IBusinessObject businessObject)
        {
            base.UpdateCompulsoryProperties(businessObject);
            //Do Custom Stuff to create a valid object
        }
    }*/

}