using System;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using Habanero.Smooth;

namespace Habanero.ProgrammaticBinding.Tests
{
	public class FakeBo:BusinessObject
	{
		/// <summary>
		/// 
		/// </summary>
		public virtual string FakeStringProp
		{
			get { return ((string) (base.GetPropertyValue("FakeStringProp"))); }
			set { base.SetPropertyValue("FakeStringProp", value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public virtual string FakeStringProp2
		{
            get { return ((string)(base.GetPropertyValue("FakeStringProp2"))); }
            set { base.SetPropertyValue("FakeStringProp2", value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public virtual int? FakeIntProp
		{
			get { return ((int?)(base.GetPropertyValue("FakeIntProp"))); }
			set { base.SetPropertyValue("FakeIntProp", value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public virtual long? FakeLongProp
		{
			get { return ((int?)(base.GetPropertyValue("FakeLongProp"))); }
			set { base.SetPropertyValue("FakeLongProp", value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual DateTime? FakeDateProp
		{
			get { return ((DateTime?) (base.GetPropertyValue("FakeDateProp"))); }
			set { base.SetPropertyValue("FakeDateProp", value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public virtual bool? FakeBoolProp
		{
			get { return ((bool?)(base.GetPropertyValue("FakeBoolProp"))); }
			set { base.SetPropertyValue("FakeBoolProp", value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public virtual FakeEnum? FakeEnumProp
		{
			get { return ((FakeEnum?)(base.GetPropertyValue("FakeEnumProp"))); }
			set { base.SetPropertyValue("FakeEnumProp", value); }
		}
		/// <summary>
		/// 
		/// </summary>
		[AutoMapCompulsory]
		public virtual int? CompIntProp
		{
			get { return ((int?)(base.GetPropertyValue("CompIntProp"))); }
			set { base.SetPropertyValue("CompIntProp", value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual decimal? DecimalProp
		{
			get { return ((decimal?) (base.GetPropertyValue("DecimalProp"))); }
			set { base.SetPropertyValue("DecimalProp", value); }
		}
	}

	public enum FakeEnum
	{
	}

	[AutoMapIgnore]
	public class FakeBoWithLookupListProp : BusinessObject
	{

		public static IClassDef GetClassDef()
		{
			var itsLoader = new XmlClassLoader(new DtdLoader(), new DefClassFactory());
			var classDef = itsLoader.LoadClass(@"
			  <class name=""FakeBoWithLookupListProp"" assembly=""Habanero.ProgrammaticBinding.Tests"">
				<property name=""FakeBoWithLookupListPropID"" type=""Guid"" compulsory=""true"" />
				<property name=""LookupListProp"" type=""Guid"">
						<simpleLookupList>
							<item display=""s1"" value=""{C2887FB1-7F4F-4534-82AB-FED92F954783}"" />
							<item display=""s2"" value=""{B89CC2C9-4CBB-4519-862D-82AB64796A58}"" />
						</simpleLookupList>
				</property>
				<primaryKey>
				  <prop name=""FakeBoWithLookupListPropID"" />
				</primaryKey>
			  </class>
			");
			return classDef;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual Guid? LookupListProp
		{
			get { return ((Guid?)(base.GetPropertyValue("LookupListProp"))); }
			set { base.SetPropertyValue("LookupListProp", value); }
		}
	}
	public class FakeBoWithSingleRelationship : BusinessObject
	{
		/// <summary>
		/// The FakeBo this FakeBoWithSingleRelationship is for.
		/// </summary>
		public virtual FakeBo FakeBoRel
		{
			get { return Relationships.GetRelatedObject<FakeBo>("FakeBoRel"); }
			set { Relationships.SetRelatedObject("FakeBoRel", value); }
		}
	}
	public class FakeBoWithMultipleRelationship : BusinessObject
	{
		/// <summary>
		/// The FakeBOs associated with this FakeBoWithMultipleRelationship.
		/// </summary>
		public virtual BusinessObjectCollection<FakeBo> FakeBos
		{
			get { return Relationships.GetRelatedCollection<FakeBo>("FakeBos"); }
		}
	}
}