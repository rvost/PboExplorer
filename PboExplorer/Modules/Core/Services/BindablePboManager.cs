﻿using BIS.PBO;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PboExplorer.Modules.Core.Services;

[Export(typeof(IPboManager))]
public class BindablePboManager: IPboManager
{
    public ICollection<ITreeItem> FileTree { get; }

    public BindablePboManager()
    {
         FileTree = new BindableCollection<ITreeItem>();
    }

    // TODO: Refactor
    public void LoadSupportedFiles(IEnumerable<string> fileNames)
    {
        var lookup = fileNames.ToLookup(f => string.Equals(Path.GetExtension(f), ".pbo", StringComparison.OrdinalIgnoreCase));
        var pbos = lookup[true];
        var nonPbos = lookup[false];

        Task.Factory
            .StartNew(() => pbos.OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
            .Select(fileName => new PboFile(new PBO(fileName, false))))
            .ContinueWith((r) =>
            {
                foreach (var e in r.Result)
                {
                    FileTree.Add(e);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

        var filesToAdd = nonPbos
            .Where(file => File.Exists(file))
            .Select(file => new PhysicalFile(Path.GetFullPath(file)))
        .ToList();

        if (filesToAdd.Any())
        {
            var openedFiles = FileTree.OfType<PhysicalFiles>().FirstOrDefault();

            if (openedFiles is null)
            {
                openedFiles = new PhysicalFiles();
                FileTree.Add(openedFiles);
            }

            foreach (var file in filesToAdd)
            {
                openedFiles.AddEntry(file);
            }
        }
    }
}