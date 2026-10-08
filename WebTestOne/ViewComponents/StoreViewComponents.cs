using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTestOne.Models;
using WebTestOne.Services;

namespace WebTestOne.ViewComponents;

public sealed class BookTopicsViewComponent(AppDbContext context) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var topics = await context.ChuDes
                .AsNoTracking()
                .Where(cd => cd.Saches.Any())
                .OrderBy(cd => cd.TenChuDe)
                .Select(cd => new TopicMenuItemViewModel
                {
                    Id = cd.Mcd,
                    Name = cd.TenChuDe ?? "Chủ đề khác",
                    BookCount = cd.Saches.Count
                })
                .ToListAsync();
            return View(topics);
        }
        catch
        {
            return View(Array.Empty<TopicMenuItemViewModel>());
        }
    }
}

public sealed class NewBooksViewComponent(AppDbContext context) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var books = await context.Saches
                .AsNoTracking()
                .OrderByDescending(s => s.NgayCapNhat)
                .Take(5)
                .Select(s => new SidebarBookViewModel
                {
                    Id = s.Ms,
                    Name = s.TenSach,
                    Price = s.DonGia ?? 0,
                    ImageUrl = StoreImage.Book(s.HinhMinhHoa)
                })
                .ToListAsync();
            return View(books);
        }
        catch
        {
            return View(Array.Empty<SidebarBookViewModel>());
        }
    }
}

public sealed class CartSummaryViewComponent(CartService cartService) : ViewComponent
{
    public IViewComponentResult Invoke() => View(cartService.GetCart());
}

public sealed class RandomAdsViewComponent(AppDbContext context) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var ads = await LoadActiveAds(context);
        var gifAds = ads
            .Where(ad => string.Equals(Path.GetExtension(ad.ImageUrl), ".gif", StringComparison.OrdinalIgnoreCase))
            .OrderBy(ad => ad.Id)
            .Concat(CreateDefaultGifAds())
            .GroupBy(ad => ad.ImageUrl, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .Take(4)
            .ToList();

        return View(gifAds);
    }

    private static async Task<List<AdvertisementViewModel>> LoadActiveAds(AppDbContext context)
    {
        try
        {
            var now = DateTime.Now;
            var ads = await context.QuangCaos
                .AsNoTracking()
                .Where(ad => (!ad.NgayBatDau.HasValue || ad.NgayBatDau <= now)
                    && (!ad.NgayHetHan.HasValue || ad.NgayHetHan >= now))
                .ToListAsync();
            return ads.Select(ad => new AdvertisementViewModel
            {
                Id = ad.Stt,
                CompanyName = ad.TenCty ?? "Đối tác Book Online",
                ImageUrl = StoreImage.Advertisement(ad.HinhMinhHoa),
                Link = string.IsNullOrWhiteSpace(ad.Href) ? "#" : ad.Href,
                StartDate = ad.NgayBatDau,
                EndDate = ad.NgayHetHan
            }).ToList();
        }
        catch
        {
            return [];
        }
    }

    private static IReadOnlyList<AdvertisementViewModel> CreateDefaultGifAds() =>
    [
        new() { Id = 101, CompanyName = "Khuyến mãi Book Online 1", ImageUrl = "/images/QC01.gif", Link = "#" },
        new() { Id = 102, CompanyName = "Khuyến mãi Book Online 2", ImageUrl = "/images/QC02.gif", Link = "#" },
        new() { Id = 103, CompanyName = "Khuyến mãi Book Online 3", ImageUrl = "/images/QC03.gif", Link = "#" },
        new() { Id = 104, CompanyName = "Khuyến mãi Book Online 4", ImageUrl = "/images/QC04.gif", Link = "#" }
    ];
}

public sealed class ActiveAdsViewComponent(AppDbContext context) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var now = DateTime.Now;
            var ads = await context.QuangCaos
                .AsNoTracking()
                .Where(ad => (!ad.NgayBatDau.HasValue || ad.NgayBatDau <= now)
                    && (!ad.NgayHetHan.HasValue || ad.NgayHetHan >= now))
                .OrderBy(ad => ad.NgayHetHan)
                .Take(3)
                .ToListAsync();
            IReadOnlyList<AdvertisementViewModel> viewModel = ads.Select(ad => new AdvertisementViewModel
            {
                Id = ad.Stt,
                CompanyName = ad.TenCty ?? "Đối tác Book Online",
                ImageUrl = StoreImage.AdvertisementJpeg(ad.HinhMinhHoa),
                Link = string.IsNullOrWhiteSpace(ad.Href) ? "#" : ad.Href,
                StartDate = ad.NgayBatDau,
                EndDate = ad.NgayHetHan
            }).ToList();

            return View(viewModel.Count > 0 ? viewModel : CreateDefaultAds());
        }
        catch
        {
            return View(CreateDefaultAds());
        }
    }

    private static IReadOnlyList<AdvertisementViewModel> CreateDefaultAds() =>
    [
        new()
        {
            Id = 1,
            CompanyName = "Ben",
            ImageUrl = "/images/QC_BEN.jpg",
            Link = "#"
        },
        new()
        {
            Id = 2,
            CompanyName = "Link Tâm",
            ImageUrl = "/images/QC_LT.jpg",
            Link = "#"
        },
        new()
        {
            Id = 3,
            CompanyName = "CBV",
            ImageUrl = "/images/QC_CBV.jpg",
            Link = "#"
        }
    ];
}

public sealed class BestSellersViewComponent(AppDbContext context) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var books = await context.Saches
                .AsNoTracking()
                .Select(s => new
                {
                    s.Ms,
                    s.TenSach,
                    s.HinhMinhHoa,
                    s.DonGia,
                    Sold = s.CtDatHangs.Sum(detail => detail.SoLuong) ?? 0
                })
                .OrderByDescending(s => s.Sold)
                .ThenBy(s => s.TenSach)
                .Take(5)
                .ToListAsync();
            return View(books.Select(s => new SidebarBookViewModel
            {
                Id = s.Ms,
                Name = s.TenSach,
                ImageUrl = StoreImage.Book(s.HinhMinhHoa),
                Price = s.DonGia ?? 0,
                SoldQuantity = s.Sold
            }).ToList());
        }
        catch
        {
            return View(Array.Empty<SidebarBookViewModel>());
        }
    }
}
