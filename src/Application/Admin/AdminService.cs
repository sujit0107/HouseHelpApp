using System.Linq;
using HouseHelp.Contracts.Admin;
using HouseHelp.Domain.Enums;
using HouseHelp.Domain.Repositories;

namespace HouseHelp.Application.Admin;

public class AdminService
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IDisputeRepository _disputeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IAvailabilityRepository availabilityRepository, IDisputeRepository disputeRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _availabilityRepository = availabilityRepository;
        _disputeRepository = disputeRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdminHelpersResponseDto> GetHelpersAsync(KycStatus? status, CancellationToken cancellationToken)
    {
        var helpers = await _availabilityRepository.SearchHelpersAsync(new HelperSearchCriteria(null, null, null, null, null, null), cancellationToken);
        var filtered = helpers.Where(h => status is null || h.KycStatus == status).Select(h => new AdminHelperSummaryDto(h.Id, h.User?.Name, h.User?.Phone ?? string.Empty, h.KycStatus, h.RatingAvg, h.JobsDone)).ToList();
        return new AdminHelpersResponseDto(filtered);
    }

    public async Task<AdminDisputesResponseDto> GetDisputesAsync(CancellationToken cancellationToken)
    {
        var disputes = await _disputeRepository.GetOpenAsync(cancellationToken);
        return new AdminDisputesResponseDto(disputes.Select(d => new AdminDisputeDto(d.Id, d.BookingId, d.OpenedBy, d.Reason, d.Status, d.CreatedAt, d.Resolution, d.ResolvedAt)).ToList());
    }

    public async Task ResolveDisputeAsync(Guid id, string resolution, CancellationToken cancellationToken)
    {
        var dispute = await _disputeRepository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Dispute not found");
        dispute.Status = "Resolved";
        dispute.Resolution = resolution;
        dispute.ResolvedAt = DateTimeOffset.UtcNow;
        await _disputeRepository.UpdateAsync(dispute, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateHelperKycAsync(Guid helperId, KycStatus status, CancellationToken cancellationToken)
    {
        var helpers = await _availabilityRepository.SearchHelpersAsync(new HelperSearchCriteria(null, null, null, null, null, null), cancellationToken);
        var helper = helpers.FirstOrDefault(h => h.Id == helperId) ?? throw new InvalidOperationException("Helper not found");
        helper.KycStatus = status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public Task<AdminReportResponseDto> GetReportAsync(string metric, string period, CancellationToken cancellationToken)
    {
        // TODO: replace with warehouse-backed metrics
        var value = metric switch
        {
            "revenue" => 0m,
            "cancellations" => 0m,
            _ => 0m
        };

        return Task.FromResult(new AdminReportResponseDto(metric, period, value));
    }
}
