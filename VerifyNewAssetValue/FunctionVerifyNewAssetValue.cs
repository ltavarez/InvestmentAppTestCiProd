using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Dtos.Email;
using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace VerifyNewAssetValue;

public class FunctionVerifyNewAssetValue(ILoggerFactory loggerFactory, IAssetService assetService, IEmailService emailService, UserManager<AppUser> userManager)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<FunctionVerifyNewAssetValue>();

    private readonly IAssetService _assetService = assetService;
    private readonly IEmailService _emailService = emailService;
    private readonly UserManager<AppUser> _userManager = userManager;

    [Function("FunctionVerifyNewAssetValue")]
    public async Task Run([TimerTrigger("%TimeTrigger%")] TimerInfo myTimer)
    {
        try
        {
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation("Next timer schedule at: {NextSchedule}", myTimer.ScheduleStatus.Next);
            }

            if (myTimer.IsPastDue)
            {
                _logger.LogWarning("Timer is past due!");
            }
            else
            {
                var assetDtos = await _assetService.GetAllWithInclude();

                if (assetDtos.Count == 0)
                {
                    _logger.LogInformation("No assets found.");
                    return;
                }

                var today = DateTime.UtcNow.Date;

                var assetWithPriceVariation = assetDtos
                    .Where(a => a.AssetHistories is not null
                        && a.AssetHistories.Any(h => h.HistoryValueDate.Date == today)
                    )
                    .Select(a =>
                    {
                        var todayPrice = a.AssetHistories!.First(h => h.HistoryValueDate.Date == today).Value;
                        var yesterdayPrice = a.AssetHistories?.First(h => h.HistoryValueDate.Date < today).Value;
                        string direction = "";

                        if (todayPrice > yesterdayPrice)
                            direction = "Increased";
                        else if (todayPrice < yesterdayPrice)
                            direction = "Decreased";
                        else
                            direction = "No change";

                        return new AssetPriceVariationDto
                        {
                            Asset = a,
                            TodayPrice = todayPrice,
                            YesterdayPrice = yesterdayPrice ?? 0m,
                            Change = (todayPrice - yesterdayPrice) ?? 0m,
                            Direction = direction
                        };
                    })
                    .Where(x => x.Change != 0)
                    .ToList();

                if (assetWithPriceVariation.Count == 0)
                {
                    _logger.LogInformation("There are no price variations today.");
                    return;
                }

                var allUsersEmail = await _userManager.Users
                    .Where(u => u.EmailConfirmed && !string.IsNullOrWhiteSpace(u.Email))
                    .Select(u => u.Email!)
                    .ToListAsync();

                if (allUsersEmail.Count == 0)
                {
                    _logger.LogInformation("There are no users with confirmed email.");
                    return;
                }

                var emailContent = string.Join("<br>", assetWithPriceVariation.Select(x =>
                    $"<strong>{x.Asset.Name}</strong>: {x.TodayPrice} ({x.Direction} from {x.YesterdayPrice})"));

                var emailSubject = "Price variations today";

                EmailRequestDto emailRequestDto = new()
                {
                    ToRange = allUsersEmail,
                    Subject = emailSubject,
                    HtmlBody = emailContent
                };

                await _emailService.SendAsync(emailRequestDto);

                _logger.LogInformation("Price variation notification sent.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while running the timer function: {Exception}", ex);
            return;
        }
        finally
        {
            _logger.LogInformation("FunctionVerifyNewAssetValue completed at: {Time}", DateTime.UtcNow);
        }
    }
}