namespace SampleMauiMvvmApp.Views;

public partial class UpsertUnregReadingPage : ContentPage
{
	public UpsertUnregReadingPage(UpsertUnregReadingsViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}