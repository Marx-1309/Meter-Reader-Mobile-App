using SampleMauiMvvmApp.ViewModels;

namespace SampleMauiMvvmApp.Views;

public partial class LoadNewExportPage : ContentPage
{
	LoadingViewModel _viewModel;

    public LoadNewExportPage(LoadingViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        this.BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.GetNewExportDataCommand.Execute(null);
    }
}