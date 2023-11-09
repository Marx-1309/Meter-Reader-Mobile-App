using SampleMauiMvvmApp.ViewModels;

namespace SampleMauiMvvmApp.Views;

public partial class UnregReadingListPage : ContentPage
{
	public UnregReadingsListViewModel listViewModel;
	public UnregReadingListPage(UnregReadingsListViewModel _listViewModel)
	{
		InitializeComponent();
		listViewModel = _listViewModel;
		this.BindingContext = listViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        listViewModel.GetReadingsListCommand.Execute(null);
    }
}