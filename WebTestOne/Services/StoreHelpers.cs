using System.Text.Json;
using WebTestOne.Models;

namespace WebTestOne.Services;

public static class StoreImage
{
    private static readonly IReadOnlyDictionary<string, string> AdvertisementJpegAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["axmobile_banner"] = "QC_BEN.jpg",
            ["damsen_banner"] = "damsen.jpg",
            ["cbv_logo"] = "QC_CBV.jpg",
            ["saigonpearl_banner"] = "QC_LT.jpg"
        };

    public static string Book(string? fileName)
    {
        var cleanName = Path.GetFileName(fileName?.Replace('\\', '/'));
        if (string.IsNullOrWhiteSpace(cleanName))
        {
            return "/images/new.jpg";
        }

        if (!Path.HasExtension(cleanName))
        {
            cleanName += ".jpg";
        }

        return $"/images/{cleanName}";
    }

    public static string Advertisement(string? fileName)
    {
        var cleanName = Path.GetFileName(fileName?.Replace('\\', '/'));
        if (string.IsNullOrWhiteSpace(cleanName))
        {
            return "/images/QC01.gif";
        }

        if (!Path.HasExtension(cleanName))
        {
            cleanName += ".gif";
        }

        return $"/images/{cleanName}";
    }

    public static string AdvertisementJpeg(string? fileName)
    {
        var cleanName = Path.GetFileNameWithoutExtension(fileName?.Replace('\\', '/'));
        if (string.IsNullOrWhiteSpace(cleanName))
        {
            return "/images/QC_BEN.jpg";
        }

        return AdvertisementJpegAliases.TryGetValue(cleanName, out var alias)
            ? $"/images/{alias}"
            : $"/images/{cleanName}.jpg";
    }
}

public sealed class CartService(IHttpContextAccessor httpContextAccessor)
{
    private const string CartKey = "BOOK_ONLINE_CART";
    private readonly ISession _session = httpContextAccessor.HttpContext?.Session
        ?? throw new InvalidOperationException("Không tìm thấy phiên làm việc hiện tại.");

    public CartViewModel GetCart()
    {
        var json = _session.GetString(CartKey);
        var items = string.IsNullOrWhiteSpace(json)
            ? []
            : JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? [];

        return new CartViewModel { Items = items };
    }

    public void Add(Sach book)
    {
        var items = GetCart().Items.ToList();
        var existing = items.FirstOrDefault(item => item.BookId == book.Ms);

        if (existing is null)
        {
            items.Add(new CartItemViewModel
            {
                BookId = book.Ms,
                Name = book.TenSach,
                ImageUrl = StoreImage.Book(book.HinhMinhHoa),
                UnitPrice = book.DonGia ?? 0,
                Quantity = 1
            });
        }
        else
        {
            existing.Quantity++;
        }

        Save(items);
    }

    public void ChangeQuantity(int bookId, int delta)
    {
        var items = GetCart().Items.ToList();
        var item = items.FirstOrDefault(candidate => candidate.BookId == bookId);
        if (item is null)
        {
            return;
        }

        item.Quantity += delta;
        if (item.Quantity <= 0)
        {
            items.Remove(item);
        }

        Save(items);
    }

    public void Remove(int bookId)
    {
        var items = GetCart().Items.Where(item => item.BookId != bookId).ToList();
        Save(items);
    }

    public void Clear() => _session.Remove(CartKey);

    private void Save(List<CartItemViewModel> items) =>
        _session.SetString(CartKey, JsonSerializer.Serialize(items));
}
