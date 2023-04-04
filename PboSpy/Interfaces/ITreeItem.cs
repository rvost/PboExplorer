﻿namespace PboSpy.Interfaces;

public interface ITreeItem
{
    string Name { get; }
    
    ITreeItem Parent { get; }
    
    ICollection<ITreeItem> Children { get; }
}
