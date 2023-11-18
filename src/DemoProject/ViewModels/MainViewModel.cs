﻿using DemoProject.Pages;
using System.Windows.Input;

namespace DemoProject.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly INameService _nameService;

    public ICommand GetNameCommand => new Command(() => GetName());

    public ICommand GoToPageParamCommand => new Command(async () => await GoToPageParamPage());

    public ICommand GoToVmParamCommand => new Command(async () => await GoToVmParamPage());

    public ICommand GoToMarkupCommand => new Command(async () => await GoToMarkup());

    public ICommand GoToScopeCheckCommand => new Command(async () => await GoToScopeCheck());

    public string Name { get; set; }

    public MainViewModel(INameService nameService)
    {
        _nameService = nameService;
    }

    void GetName()
    {
        Name = _nameService.GetName();

        OnPropertChanged(nameof(Name));
    }

    async Task GoToPageParamPage()
    {
        Name = _nameService.GetName();

        await Navigation.PushAsync<PageParamPage>(Name);
    }

    async Task GoToVmParamPage()
    {
        await Navigation.PushAsync<VmParamPage>("Name passed as parameter");
    }

    async Task GoToMarkup()
    {
        await Navigation.PushAsync(new MarkupPage());
    }

    async Task GoToScopeCheck()
    {
        await Navigation.PushAsync<ScopeCheckPage>();
    }
}
