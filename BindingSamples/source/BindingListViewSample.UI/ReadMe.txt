This sample shows the use of WinForms 2.0 Binding with Habanero Business Objects.
When binding Business Objects there are two scenarious
1) Binding a list of Business Object to a ListBox, Grid, Combo Box etc.
2) Binding of an individual Business Object to a Set of Controls e.g. Binding person.Surname to txtSurname.


1) Binding a List
To achieve this the List of Business Object needs to implement a certain set of Interfaces.
In Habanero.Binding the BindingListView implements IBindingListView and provides the capabalities to
bind a list of Business Objects as a Sortable and filterable BindingList.

2) The individual Business Object has to implement the Interfaces INotifyPropertyChanged and IDataErrorInfo.
In this example see the Employee.cs which Shows how to Implement these Interfaces for A Habanero Business Object.


For Winforms 2.0 style binding you do not need this modify the standard habanero Program.cs
