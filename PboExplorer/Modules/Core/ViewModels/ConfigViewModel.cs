﻿using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.PropertyGrid;
using PboExplorer.Interfaces;
using PboExplorer.Modules.Core.Models;
using PboExplorer.Modules.Core.Services;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;

namespace PboExplorer.Modules.Core.ViewModels;

// TODO: Remove duplication with ExplorerViewModel
[Export]
public class ConfigViewModel : Tool
{
    private readonly IPboManager _pboManager;
    private readonly IPreviewManager _previewManager;
    private readonly IPropertyGrid _propertyGrid;

    private ITreeItem _selectedItem;

    public ICollection<ITreeItem> Items { get => _pboManager.ConfigTree; }

    public ITreeItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            NotifyOfPropertyChange(nameof(SelectedItem));
        }
    }

    public override PaneLocation PreferredLocation => PaneLocation.Left;

    [ImportingConstructor]
    public ConfigViewModel(IPboManager pboManager, IPropertyGrid propertyGrid, IPreviewManager previewManager)
    {
        _pboManager = pboManager;
        _previewManager= previewManager;
        _propertyGrid = propertyGrid;

        DisplayName = "Config";
    }

    public async Task OpenPreview(ITreeItem item)
    {
        if(item != SelectedItem)
        {
            return; // Handle bubbling
        }

        if (item is ConfigClassItem classItem)
        {
            await _previewManager.ShowPreviewAsync(classItem);
        }
    }

    public void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> args)
    {
        if (args.NewValue is ITreeItem item)
        {
            _propertyGrid.SelectedObject = item.Metadata;
        }
    }
}
