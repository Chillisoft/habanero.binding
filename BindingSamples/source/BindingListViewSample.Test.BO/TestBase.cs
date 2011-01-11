using System;
using System.Collections.Generic;
using System.IO;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using BindingListViewSample.BO;

namespace BindingListViewSample.Test.BO
{
    /// <summary>
    /// Provides standard setup utilities for test classes can inherit from and use to
    /// initialise the testing environment.
    /// This class is only written once, so you can safely modify it.
    /// </summary>
    public class TestBase
    {
        /// <summary>
        /// Sets up the test environment once per test fixture
        /// </summary>
        public static void SetupTestFixture()
        {
            SetupDatabaseConnection();
            RefreshClassDefs();
        }

        //-------------------------------------------------------------------------------------
        //
        // Set the connection details for your database here.
        // Then add the necessary DLL for that database to the references in this test project.
        //
        //        *** CAUTION! ***
        //
        // The database tables are cleaned before tests are executed.  Do not run the tests
        // against a live database unless you are able to restore the data afterwards.  Rather
        // copy the live database and run the tests on the copy.
        //
        //-------------------------------------------------------------------------------------
        private static void SetupDatabaseConnection()
        {
/*            if (DatabaseConnection.CurrentConnection == null)
            {
                DatabaseConfig _databaseConfig =
                    new DatabaseConfig("mysql", "localhost", "mydatabase", "root", "root", "3306");
                DatabaseConnection.CurrentConnection = _databaseConfig.GetDatabaseConnection();
            }*/
        }

        /// <summary>
        /// Clears the loaded class definitions and reloads them.  Usually this only needs
        /// to be done once per test fixture, except if there are individual tests that
        /// modify the class definitions.
        /// </summary>
        public static void RefreshClassDefs()
        {
            ClassDef.ClassDefs.Clear();
            ClassDef.ClassDefs.Add(new XmlClassDefsLoader(BOBroker.GetClassDefsXml(), new DtdLoader()).LoadClassDefs());
        }

        /// <summary>
        /// Sets up the test environment for each single test.  If your test fixture has
        /// individual tests that modify the class definitions, call RefreshClassDefs before
        /// you call this method.
        /// </summary>
        public static void SetupTest()
        {
            //-------------------------------------------------------------------------------------
            // You can choose here whether to run against a database or whether to use an in-memory
            // database for all the tests, which runs quickly. It doesn't however check that
            // your database has the correct structure, which is partly the purpose of these tests.
            // The generated tests do already use an in-memory database where possible.
            // In your custom tests, you can set them to use an in-memory database by copying the
            // line to the first line of your test.
            //-------------------------------------------------------------------------------------
            //BORegistry.DataAccessor = new DataAccessorDB();
            BORegistry.DataAccessor = new DataAccessorInMemory();

            ClearAllTables();
            BusinessObjectManager.Instance.ClearLoadedObjects();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Clears all the database tables holding business object data.
        /// WARNING: This is an irreversible action, be sure to use a test database.
        /// </summary>
        public static void ClearAllTables()
        {
            if (BORegistry.DataAccessor is DataAccessorInMemory)
            {
                BORegistry.DataAccessor = new DataAccessorInMemory();
            }
/*            else if (BORegistry.DataAccessor is DataAccessorDB)
            {
                //-----------------------------------------------------------------------
                // Deletes the contents of each of the tables.
                // Orders the tables to prevent parents being deleted before their
                // children, by assuming that the object on the 1 side of 1-to-many
                // is the parent.
                // Replace these lines below if this does not work for your setup.
                // Table names may need delimiters - adjust as needed for your database.
                //-----------------------------------------------------------------------
                foreach (ClassDef classDef in GetClassDefsOrderedForDeletion())
                {
                    DatabaseConnection.CurrentConnection.ExecuteRawSql("Delete from " + classDef.TableName);
                }
            }*/
            else
            {
                throw new HabaneroDeveloperException("No recognised DataAccessor has been set up for the tests", "");
            }
        }

        /// <summary>
        /// Gets a list of class definitions ordered for deletion in order to avoid
        /// foreign key conflicts where parents are deleted before their children.
        /// </summary>
        private static IList<ClassDef> GetClassDefsOrderedForDeletion()
        {
            IList<ClassDef> classDefs = new List<ClassDef>();
            foreach (ClassDef classDef in ClassDef.ClassDefs)
            {
                classDefs.Add(classDef);
            }

            bool deleteOrderCorrect;
            do
            {
                deleteOrderCorrect = true;
                for (int position = 0; position < classDefs.Count; position++)
                {
                    ClassDef classDef = classDefs[position];
                    for (int parentPosition = 0; parentPosition < position; parentPosition++)
                    {
                        ClassDef potentialParentClassDef = classDefs[parentPosition];
                        if (ClassMustBeDeletedBeforeParent(classDef, potentialParentClassDef))
                        {
                            classDefs.Remove(classDef);
                            classDefs.Insert(parentPosition, classDef);
                            deleteOrderCorrect = false;
                            break;
                        }
                    }
                    if (!deleteOrderCorrect) break;
                }
            } while (!deleteOrderCorrect);

            return classDefs;
        }

        /// <summary>
        /// Indicates if the given class needs to be deleted before the one provided,
        /// which occurs when the parent is on the 1 side of 1-to-many
        /// </summary>
        private static bool ClassMustBeDeletedBeforeParent(IClassDef childClassDef, ClassDef potentialParentClassDef)
        {
            foreach (RelationshipDef relationshipDef in potentialParentClassDef.RelationshipDefCol)
            {
                if (relationshipDef.RelatedObjectClassName + relationshipDef.RelatedObjectAssemblyName ==
                        childClassDef.ClassName + childClassDef.AssemblyName &&
                        relationshipDef is MultipleRelationshipDef)
                {
                    return true;
                }
            }
            return false;
        }
    }
}