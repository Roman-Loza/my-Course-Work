using AutoSalonPro.Models;

namespace AutoSalonPro.Services;

public static class ReportService
{
    public static List<Order> Sales(DateTime from, DateTime to) =>
        Storage.Db.Orders
            .Where(o => o.Status == OrderStatus.Completed && o.ClosedAt.HasValue
                        && o.ClosedAt.Value.Date >= from.Date && o.ClosedAt.Value.Date <= to.Date)
            .ToList();

    public static List<BrandReportRow> ByBrand(List<Order> sales) =>
        sales.GroupBy(o => Storage.Db.Cars.FirstOrDefault(c => c.Id == o.CarId)?.Brand ?? "—")
             .Select(g => new BrandReportRow { Brand = g.Key, Count = g.Count(), Sum = g.Sum(o => o.FinalPrice) })
             .OrderByDescending(r => r.Sum)
             .ToList();
}
