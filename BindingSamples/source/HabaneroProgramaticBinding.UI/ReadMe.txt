This sample shows the use of Habanero Binding with a Designed form. Habanero Binding is much richer than standard windows binding.
And this sample shows how to use Habanero.Binding's power with a User Designed form.

When binding Business Objects there are two scenarious
1) Binding a list of Business Object to a ListBox, Grid, Combo Box etc.
2) Binding of an individual Business Object to a Set of Controls e.g. Binding person.Surname to txtSurname.

1) Binding a List
 In this sample I have simply bound the List of BusinessObjects but if you wanted two way binding i.e. the updating of a combo box value based on changes to the Business Object you will need
  more comprehensive example. We will work on including another example of this later.

2) The individual Business Object DOES NOT NEED TO implement the Interfaces INotifyPropertyChanged and IDataErrorInfo since we are using
Habanero.Binding.
 In the Example Employee does implement this interface but it is TOTALLY UNECESSARY.


For Habanero's Binding on steriods to work you have to do a few more things
1) in the startup projects program form add this line

Immediately after
		if (!mainApp.Startup()) return;

Put this line.
		GlobalUIRegistry.ControlFactory = new ControlFactoryManualBindingWin();