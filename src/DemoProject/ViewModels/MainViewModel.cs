﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoProject.Pages;
using DemoProject.Popups.Pages;
using Mopups.Services;
using System.Diagnostics;

namespace DemoProject.ViewModels;

[ObservableObject]
public partial class MainViewModel(INameService nameService) : BaseViewModel
{
    [ObservableProperty]
    private string _name = string.Empty;

    [RelayCommand]
    private void GetName()
    {
        Name = nameService.GetName();
    }

    [RelayCommand]
    private async Task GoToPageParamPage()
    {
        Name = nameService.GetName();

        await Navigation.PushAsync<PageParamPage>(Name);
    }

    [RelayCommand]
    private async Task GoToVmParamPage()
    {
        await Navigation.PushAsync<VmParamPage>("Name passed as parameter");
    }

    [RelayCommand]
    private async Task GoToMarkup()
    {
        await Navigation.PushAsync(new MarkupPage());
    }

    [RelayCommand]
    private async Task GoToScopeCheck()
    {
        await Navigation.PushAsync<ScopeCheckPage>();
    }

    [RelayCommand]
    private async Task GoToBrokenPage()
    {
        await Navigation.PushAsync<BrokenPage>();
    }

    [RelayCommand]
    private Task ShowEasyPopup()
    {
        return MopupService.Instance.PushAsync<EasyPopup>();
    }

    [RelayCommand]
    private Task ShowParamPopup()
    {
        return MopupService.Instance.PushAsync<ParamPopup>("It's alive!");
    }

    [RelayCommand]
    private async Task TriggerAggregateException()
    {
        try
        {
            await Navigation.PushAsync<AggregateExceptionPage>("test");
        }
        catch(AggregateException ex)
        {
            Debug.WriteLine(ex);
            Debugger.Break();
            throw;
        }
    }
}