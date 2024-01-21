namespace SampleMauiMvvmApp.Views;

public partial class SynchronizationPage : ContentPage
{
    ReadingViewModel viewModel;
	public SynchronizationPage(ReadingViewModel _viewModel)
	{
		InitializeComponent();
        viewModel = _viewModel;
        BindingContext = _viewModel;
	}
}