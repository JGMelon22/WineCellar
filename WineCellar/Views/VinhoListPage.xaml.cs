using WineCellar.Data;

namespace WineCellar.Views;

public partial class VinhoListPage : ContentPage
{
    private readonly VinhoRepositorioMemoria _repositorio;

    public VinhoListPage(VinhoRepositorioMemoria repositorio)
    {
        InitializeComponent();
        _repositorio = repositorio;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        VinhosCollectionView.ItemsSource = _repositorio.ObterTodos();
    }
}