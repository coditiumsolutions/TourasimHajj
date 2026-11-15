using System.ComponentModel.DataAnnotations;
using TourismHajj.Models.Entities;

namespace TourismHajj.ViewModels;

public class HajjSeasonListViewModel
{
    public IReadOnlyList<HajjSeason> Seasons { get; set; } = Array.Empty<HajjSeason>();
    public bool IncludeInactive { get; set; }
}

public class HajjSeasonInputModel
{
    public int HajjSeasonId { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Season Name")]
    public string SeasonName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Hajj Year")]
    public int HajjYear { get; set; } = DateTime.Today.Year;

    [Display(Name = "Registration Start")]
    public DateOnly? RegistrationStartDate { get; set; }

    [Display(Name = "Registration End")]
    public DateOnly? RegistrationEndDate { get; set; }

    [Display(Name = "Departure Start")]
    public DateOnly? DepartureStartDate { get; set; }

    [Display(Name = "Departure End")]
    public DateOnly? DepartureEndDate { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}

public class PilgrimGroupListViewModel
{
    public IReadOnlyList<PilgrimGroup> Groups { get; set; } = Array.Empty<PilgrimGroup>();
    public int? HajjSeasonId { get; set; }
    public string? Status { get; set; }
    public IReadOnlyList<HajjSeason> SeasonOptions { get; set; } = Array.Empty<HajjSeason>();
}

public class PilgrimGroupInputModel
{
    public int PilgrimGroupId { get; set; }

    [Required]
    [Display(Name = "Hajj Season")]
    public int HajjSeasonId { get; set; }

    [Display(Name = "Departure")]
    public int? DepartureId { get; set; }

    [Required, StringLength(30)]
    [Display(Name = "Group Code")]
    public string GroupCode { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Group Name")]
    public string? GroupName { get; set; }

    [StringLength(150)]
    [Display(Name = "Leader Name")]
    public string? GroupLeaderName { get; set; }

    [StringLength(30)]
    [Display(Name = "Leader Mobile")]
    public string? GroupLeaderMobile { get; set; }

    [StringLength(30)]
    [Display(Name = "Status")]
    public string? GroupStatus { get; set; } = "Planning";

    [StringLength(500)]
    public string? Remarks { get; set; }

    public IReadOnlyList<HajjSeason> SeasonOptions { get; set; } = Array.Empty<HajjSeason>();
    public IReadOnlyList<PackageDeparture> DepartureOptions { get; set; } = Array.Empty<PackageDeparture>();
}

public class PilgrimGroupDetailsViewModel
{
    public PilgrimGroup Group { get; set; } = null!;
    public IReadOnlyList<PilgrimGroupMember> Members { get; set; } = Array.Empty<PilgrimGroupMember>();
}

public class PilgrimGroupMemberInputModel
{
    public int PilgrimGroupMemberId { get; set; }

    [Required]
    public int PilgrimGroupId { get; set; }

    [Required]
    [Display(Name = "Booking Traveler")]
    public int BookingTravelerId { get; set; }

    [Display(Name = "Group Leader")]
    public bool IsGroupLeader { get; set; }

    [StringLength(50)]
    [Display(Name = "Room Group")]
    public string? RoomGroup { get; set; }

    [StringLength(50)]
    [Display(Name = "Transport Group")]
    public string? TransportGroup { get; set; }

    [StringLength(300)]
    public string? Remarks { get; set; }

    public string? GroupCode { get; set; }
    public IReadOnlyList<BookingTravelerOption> TravelerOptions { get; set; } = Array.Empty<BookingTravelerOption>();
}

public class BookingTravelerOption
{
    public int BookingTravelerId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public class HajjPilgrimRegisterRow
{
    public string? SeasonName { get; set; }
    public string? GroupCode { get; set; }
    public string? GroupName { get; set; }
    public string? PilgrimName { get; set; }
    public string? PassportNo { get; set; }
    public string? MobileNo { get; set; }
    public string? BookingNo { get; set; }
    public bool IsGroupLeader { get; set; }
    public string? RoomGroup { get; set; }
    public string? TransportGroup { get; set; }
}