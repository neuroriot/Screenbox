﻿#nullable enable

using Screenbox.Core.Factories;
using Screenbox.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace Screenbox.Core.ViewModels;
public sealed class UriMediaViewModel : MediaViewModel
{
    public Uri Uri { get; }

    private readonly IFilesService _filesService;

    public UriMediaViewModel(IMediaService mediaService, IFilesService fileService,
        AlbumViewModelFactory albumFactory, ArtistViewModelFactory artistFactory, Uri uri, string id = "")
        : base(uri, mediaService, albumFactory, artistFactory)
    {
        _filesService = fileService;
        Name = uri.Segments.Length > 0 ? Uri.UnescapeDataString(uri.Segments.Last()) : string.Empty;
        Location = uri.ToString();
        Id = string.IsNullOrEmpty(id) ? Location : id;
        Uri = uri;
    }

    private UriMediaViewModel(UriMediaViewModel source) : base(source)
    {
        _filesService = source._filesService;
        Uri = source.Uri;
    }

    public override MediaViewModel Clone()
    {
        return new UriMediaViewModel(this);
    }

    public override async Task LoadThumbnailAsync()
    {
        if (Thumbnail != null) return;
        if (!Uri.IsFile)
        {
            await base.LoadThumbnailAsync();
            return;
        }

        try
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(Uri.OriginalString);  // Return how StorageFile saves the path
            StorageItemThumbnail? source = ThumbnailSource = await _filesService.GetThumbnailAsync(file);
            if (source == null) return;
            BitmapImage image = new();
            await image.SetSourceAsync(ThumbnailSource);
            Thumbnail = image;
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
