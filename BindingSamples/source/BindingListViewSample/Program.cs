using System;
using System.Windows.Forms;
using BindingListViewSample.Logic;
using BindingListViewSample.Test.BO;
using BindingListViewSample.UI;
using Habanero.BO;
using Habanero.Faces.Base;
using Habanero.Faces.Win;
using Habanero.Base;
using HabaneroProgramaticBinding.UI;
using log4net;
using log4net.Config;
using BindingListViewSample.BO;

namespace BindingListViewSample
{
	static class Program
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));

		[STAThread]
		static void Main()
		{
			GlobalRegistry.ApplicationName = "BindingListViewSample";
			GlobalRegistry.ApplicationVersion = "v1.0";
			var mainApp = new HabaneroAppWin(GlobalRegistry.ApplicationName, GlobalRegistry.ApplicationVersion);
			mainApp.ClassDefsXml = BOBroker.GetClassDefsXml();
			try
			{
				log.Debug(string.Format("-------- {0} {1} Starting  ---------", GlobalRegistry.ApplicationName, GlobalRegistry.ApplicationVersion));
				if (!mainApp.Startup()) return;
			    GlobalUIRegistry.ControlFactory = new ControlFactoryManualBindingWin();
			    SampleDataGenerator.CreateNewRandomEmployees(4);
			    // Uncomment the next line of code to use an "In Memory" data accessor (instead of your Database).
			    //BORegistry.DataAccessor = new DataAccessorInMemory();
			}
			catch (Exception ex)
			{
				log.Error(ex.Message);
			}

			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				var programForm = new FormWin();
				SetupMainForm(programForm);
				Application.Run(programForm);
			}
			catch (Exception ex)
			{
				GlobalRegistry.UIExceptionNotifier.Notify(ex,
						"An error has occurred in the application.",
						"Application Error");
			}
		}

		public static void SetupMainForm(IFormHabanero programForm)
		{
			IMainMenuHabanero mainMenuHabanero = SetupMainMenu(programForm, GlobalUIRegistry.ControlFactory, new MenuBuilderWin(GlobalUIRegistry.ControlFactory));
			mainMenuHabanero.DockInForm(programForm);
			programForm.Text = GlobalRegistry.ApplicationName + " " + GlobalRegistry.ApplicationVersion;
			programForm.WindowState = Habanero.Faces.Base.FormWindowState.Maximized;
		}        

		public static IMainMenuHabanero SetupMainMenu(IFormHabanero programForm, IControlFactory controlFactory, IMenuBuilder menuBuilder)
		{
			var mainMenu = new HabaneroMenu("Main", programForm, GlobalUIRegistry.ControlFactory);

			HabaneroMenu currentMenu = mainMenu.AddSubMenu("&File");

			HabaneroMenu.Item currentMenuItem = currentMenu.AddMenuItem("E&xit");
			currentMenuItem.CustomHandler += delegate { programForm.Close(); };

			HabaneroMenu dataMenu = mainMenu.AddSubMenu("&WinForms Binding List View Examples");

			HabaneroMenu.Item employeeMenuItem = dataMenu.AddMenuItem("Add & Remove Employee Grid");
			employeeMenuItem.CustomHandler += ShowEmployeeForm;

			HabaneroMenu.Item employeeFilterMenuItem = dataMenu.AddMenuItem("Filter Employee Grid");
			employeeFilterMenuItem.CustomHandler += ShowEmployeeFilterForm;

			HabaneroMenu.Item employeeEditMenuItem = dataMenu.AddMenuItem("Edit Employee ");
			employeeEditMenuItem.CustomHandler += ShowEmployeeEditForm;

			HabaneroMenu habaneroBindingSamples = mainMenu.AddSubMenu("&Habanero Binding Examples");

			HabaneroMenu.Item habaneroBindEditEmployee = habaneroBindingSamples.AddMenuItem("Edit Employee ");
			habaneroBindEditEmployee.CustomHandler += ShowHabaneroBindEmployeeEditForm;

			return menuBuilder.BuildMainMenu(mainMenu);
		}

		private static void ShowEmployeeForm(object sender, EventArgs eventArgs)
		{
			var control = new BindingListViewSample.UI.EmployeeAddAndRemoveForm();
			control.Show();
		}

		private static void ShowEmployeeFilterForm(object sender, EventArgs e)
		{
			var control = new BindingListViewSample.UI.EmployeeFilterForm();
			control.Show();
		}

		private static void ShowEmployeeEditForm(object sender, EventArgs e)
		{
			var control = new BindingListViewSample.UI.EmployeeEditForm();
			control.Show();
		}
		private static void ShowHabaneroBindEmployeeEditForm(object sender, EventArgs e)
		{
			var control = new HabaneroProgramaticBinding.UI.EmployeeEditForm();
			control.Show();
		}
	}
}