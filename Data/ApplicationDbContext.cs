using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourismHajj.Models;
using TourismHajj.Models.Entities;

namespace TourismHajj.Data;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppConfig> AppConfigs { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingTraveler> BookingTravelers { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<ContactInquiry> ContactInquiries { get; set; }

    public virtual DbSet<DepartureFlight> DepartureFlights { get; set; }

    public virtual DbSet<Destination> Destinations { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<GalleryItem> GalleryItems { get; set; }

    public virtual DbSet<HajjSeason> HajjSeasons { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<HotelPackage> HotelPackages { get; set; }

    public virtual DbSet<PackageDeparture> PackageDepartures { get; set; }

    public virtual DbSet<PackageDestination> PackageDestinations { get; set; }

    public virtual DbSet<PackageItinerary> PackageItineraries { get; set; }

    public virtual DbSet<PackageService> PackageServices { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PilgrimGroup> PilgrimGroups { get; set; }

    public virtual DbSet<PilgrimGroupMember> PilgrimGroupMembers { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<RoomAllocation> RoomAllocations { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceAssignment> ServiceAssignments { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierPayment> SupplierPayments { get; set; }

    public virtual DbSet<TransportProvider> TransportProviders { get; set; }

    public virtual DbSet<TravelPackage> TravelPackages { get; set; }

    public virtual DbSet<Traveler> Travelers { get; set; }

    public virtual DbSet<TravelerDocument> TravelerDocuments { get; set; }

    public virtual DbSet<VisaApplication> VisaApplications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppConfig>(entity =>
        {
            entity.HasKey(e => e.AppConfigId);
            entity.HasIndex(e => e.ConfigKey).IsUnique();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CDDD67C8201");

            entity.Property(e => e.ActionDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951ACDE9175774");

            entity.Property(e => e.BalanceAmount).HasDefaultValue(0m);
            entity.Property(e => e.BookingDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.BookingStatus).HasDefaultValue("Pending");
            entity.Property(e => e.DiscountAmount).HasDefaultValue(0m);
            entity.Property(e => e.NetAmount).HasDefaultValue(0m);
            entity.Property(e => e.PaidAmount).HasDefaultValue(0m);
            entity.Property(e => e.PaymentStatus).HasDefaultValue("Unpaid");
            entity.Property(e => e.TaxAmount).HasDefaultValue(0m);
            entity.Property(e => e.TotalAdults).HasDefaultValue(1);
            entity.Property(e => e.TotalAmount).HasDefaultValue(0m);
            entity.Property(e => e.TotalChildren).HasDefaultValue(0);
            entity.Property(e => e.TotalInfants).HasDefaultValue(0);

            entity.HasOne(d => d.Customer).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Custom__151B244E");

            entity.HasOne(d => d.Departure).WithMany(p => p.Bookings).HasConstraintName("FK__Bookings__Depart__17036CC0");

            entity.HasOne(d => d.Package).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Packag__160F4887");
        });

        modelBuilder.Entity<BookingTraveler>(entity =>
        {
            entity.HasKey(e => e.BookingTravelerId).HasName("PK__BookingT__1963EE2CC04F913C");

            entity.Property(e => e.DiscountAmount).HasDefaultValue(0m);
            entity.Property(e => e.NetAmount).HasDefaultValue(0m);
            entity.Property(e => e.PackagePrice).HasDefaultValue(0m);
            entity.Property(e => e.TravelerCategory).HasDefaultValue("Adult");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingTravelers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingTr__Booki__1EA48E88");

            entity.HasOne(d => d.Traveler).WithMany(p => p.BookingTravelers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingTr__Trave__1F98B2C1");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__Cities__F2D21A96F1FB335D");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cities__CountryI__3B75D760");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Countrie__10D160BF97DDE2CE");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B816C4E1AB");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CustomerType).HasDefaultValue("Individual");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasIndex(e => e.AspNetUserId)
                .IsUnique()
                .HasFilter("[AspNetUserId] IS NOT NULL");

            entity.HasOne(d => d.City).WithMany(p => p.Customers).HasConstraintName("FK__Customers__CityI__45F365D3");
        });

        modelBuilder.Entity<DepartureFlight>(entity =>
        {
            entity.HasKey(e => e.DepartureFlightId).HasName("PK__Departur__93ABD96E361EF0C7");

            entity.HasOne(d => d.Departure).WithMany(p => p.DepartureFlights)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Departure__Depar__02FC7413");

            entity.HasOne(d => d.FromCity).WithMany(p => p.DepartureFlightFromCities).HasConstraintName("FK__Departure__FromC__04E4BC85");

            entity.HasOne(d => d.Supplier).WithMany(p => p.DepartureFlights).HasConstraintName("FK__Departure__Suppl__03F0984C");

            entity.HasOne(d => d.ToCity).WithMany(p => p.DepartureFlightToCities).HasConstraintName("FK__Departure__ToCit__05D8E0BE");
        });

        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.DestinationId).HasName("PK__Destinat__DB5FE4AC0601E72C");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);

            entity.HasOne(d => d.City).WithMany(p => p.Destinations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Destinati__CityI__403A8C7D");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF11329F833");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Salary).HasDefaultValue(0m);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__Expenses__1445CFF3EE7F72DE");

            entity.HasOne(d => d.Booking).WithMany(p => p.Expenses).HasConstraintName("FK__Expenses__Bookin__4B7734FF");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Expenses).HasConstraintName("FK__Expenses__Suppli__4C6B5938");
        });

        modelBuilder.Entity<HajjSeason>(entity =>
        {
            entity.HasKey(e => e.HajjSeasonId).HasName("PK__HajjSeas__70F4C69CAA0FCA2E");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.HotelId).HasName("PK__Hotels__46023BBF58026062");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.City).WithMany(p => p.Hotels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Hotels__CityID__5812160E");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Hotels).HasConstraintName("FK__Hotels__Supplier__571DF1D5");
        });

        modelBuilder.Entity<PackageDeparture>(entity =>
        {
            entity.HasKey(e => e.DepartureId).HasName("PK__PackageD__8F8A789908066D7A");

            entity.Property(e => e.AvailableSeats).HasDefaultValue(0);
            entity.Property(e => e.CurrencyCode).HasDefaultValue("PKR");
            entity.Property(e => e.DepartureStatus).HasDefaultValue("Open");
            entity.Property(e => e.TotalSeats).HasDefaultValue(0);

            entity.HasOne(d => d.Package).WithMany(p => p.PackageDepartures)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PackageDe__Packa__00200768");
        });

        modelBuilder.Entity<PackageDestination>(entity =>
        {
            entity.HasKey(e => e.PackageDestinationId).HasName("PK__PackageD__1B7B41E7A5E2CAC7");

            entity.Property(e => e.NumberOfDays).HasDefaultValue(1);
            entity.Property(e => e.SequenceNo).HasDefaultValue(1);

            entity.HasOne(d => d.Destination).WithMany(p => p.PackageDestinations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PackageDe__Desti__6D0D32F4");

            entity.HasOne(d => d.Package).WithMany(p => p.PackageDestinations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PackageDe__Packa__6C190EBB");
        });

        modelBuilder.Entity<PackageItinerary>(entity =>
        {
            entity.HasKey(e => e.ItineraryId).HasName("PK__PackageI__361216A6FAB5D66E");

            entity.HasOne(d => d.Destination).WithMany(p => p.PackageItineraries).HasConstraintName("FK__PackageIt__Desti__70DDC3D8");

            entity.HasOne(d => d.Package).WithMany(p => p.PackageItineraries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PackageIt__Packa__6FE99F9F");
        });

        modelBuilder.Entity<PackageService>(entity =>
        {
            entity.HasKey(e => e.PackageServiceId).HasName("PK__PackageS__5EAFC210CDA59FAA");

            entity.Property(e => e.AdditionalPrice).HasDefaultValue(0m);
            entity.Property(e => e.IsIncluded).HasDefaultValue(true);

            entity.HasOne(d => d.Package).WithMany(p => p.PackageServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PackageSe__Packa__787EE5A0");

            entity.HasOne(d => d.Service).WithMany(p => p.PackageServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PackageSe__Servi__797309D9");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A58B0A93E47");

            entity.Property(e => e.CurrencyCode).HasDefaultValue("PKR");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PaymentType).HasDefaultValue("Receipt");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Bookin__46B27FE2");
        });

        modelBuilder.Entity<PilgrimGroup>(entity =>
        {
            entity.HasKey(e => e.PilgrimGroupId).HasName("PK__PilgrimG__DD592BD91673C252");

            entity.Property(e => e.GroupStatus).HasDefaultValue("Planning");
            entity.Property(e => e.TotalPilgrims).HasDefaultValue(0);

            entity.HasOne(d => d.Departure).WithMany(p => p.PilgrimGroups).HasConstraintName("FK__PilgrimGr__Depar__3587F3E0");

            entity.HasOne(d => d.HajjSeason).WithMany(p => p.PilgrimGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PilgrimGr__HajjS__3493CFA7");
        });

        modelBuilder.Entity<PilgrimGroupMember>(entity =>
        {
            entity.HasKey(e => e.PilgrimGroupMemberId).HasName("PK__PilgrimG__E1FC1C3F341DFECE");

            entity.Property(e => e.IsGroupLeader).HasDefaultValue(false);

            entity.HasOne(d => d.BookingTraveler).WithMany(p => p.PilgrimGroupMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PilgrimGr__Booki__3B40CD36");

            entity.HasOne(d => d.PilgrimGroup).WithMany(p => p.PilgrimGroupMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PilgrimGr__Pilgr__3A4CA8FD");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.RefundId).HasName("PK__Refunds__725AB900A5A3BE50");

            entity.Property(e => e.RefundStatus).HasDefaultValue("Pending");

            entity.HasOne(d => d.Booking).WithMany(p => p.Refunds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Refunds__Booking__503BEA1C");

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds).HasConstraintName("FK__Refunds__Payment__51300E55");
        });

        modelBuilder.Entity<RoomAllocation>(entity =>
        {
            entity.HasKey(e => e.RoomAllocationId).HasName("PK__RoomAllo__3D96B811DCC9D1AB");

            entity.HasOne(d => d.BookingTraveler).WithMany(p => p.RoomAllocations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomAlloc__Booki__22751F6C");

            entity.HasOne(d => d.Hotel).WithMany(p => p.RoomAllocations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomAlloc__Hotel__236943A5");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EADBA69827");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<ServiceAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__ServiceA__32499E5781FF7A69");

            entity.Property(e => e.CostAmount).HasDefaultValue(0m);
            entity.Property(e => e.ServiceStatus).HasDefaultValue("Pending");

            entity.HasOne(d => d.Booking).WithMany(p => p.ServiceAssignments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServiceAs__Booki__282DF8C2");

            entity.HasOne(d => d.Hotel).WithMany(p => p.ServiceAssignments).HasConstraintName("FK__ServiceAs__Hotel__2B0A656D");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceAssignments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ServiceAs__Servi__29221CFB");

            entity.HasOne(d => d.Supplier).WithMany(p => p.ServiceAssignments).HasConstraintName("FK__ServiceAs__Suppl__2A164134");

            entity.HasOne(d => d.TransportProvider).WithMany(p => p.ServiceAssignments).HasConstraintName("FK__ServiceAs__Trans__2BFE89A6");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE66694E81F5743");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.City).WithMany(p => p.Suppliers).HasConstraintName("FK__Suppliers__CityI__534D60F1");

            entity.HasOne(d => d.Country).WithMany(p => p.Suppliers).HasConstraintName("FK__Suppliers__Count__52593CB8");
        });

        modelBuilder.Entity<SupplierPayment>(entity =>
        {
            entity.HasKey(e => e.SupplierPaymentId).HasName("PK__Supplier__DC6B0E5E16071368");

            entity.HasOne(d => d.Booking).WithMany(p => p.SupplierPayments).HasConstraintName("FK__SupplierP__Booki__5BAD9CC8");

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierPayments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SupplierP__Suppl__5AB9788F");
        });

        modelBuilder.Entity<TransportProvider>(entity =>
        {
            entity.HasKey(e => e.TransportProviderId).HasName("PK__Transpor__0F9014C212403579");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Supplier).WithMany(p => p.TransportProviders).HasConstraintName("FK__Transport__Suppl__5BE2A6F2");
        });

        modelBuilder.Entity<TravelPackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__TravelPa__322035EC19411B49");

            entity.Property(e => e.AdultPrice).HasDefaultValue(0m);
            entity.Property(e => e.ChildPrice).HasDefaultValue(0m);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CurrencyCode).HasDefaultValue("PKR");
            entity.Property(e => e.InfantPrice).HasDefaultValue(0m);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.Property(e => e.MinimumPersons).HasDefaultValue(1);
        });

        modelBuilder.Entity<Traveler>(entity =>
        {
            entity.HasKey(e => e.TravelerId).HasName("PK__Traveler__66FD3CF8442D1714");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.Travelers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Travelers__Custo__49C3F6B7");

            entity.HasOne(d => d.NationalityCountry).WithMany(p => p.Travelers).HasConstraintName("FK__Travelers__Natio__4AB81AF0");
        });

        modelBuilder.Entity<TravelerDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__Traveler__1ABEEF6FA191B5BA");

            entity.Property(e => e.UploadedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Traveler).WithMany(p => p.TravelerDocuments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TravelerD__Trave__4E88ABD4");
        });

        modelBuilder.Entity<VisaApplication>(entity =>
        {
            entity.HasKey(e => e.VisaApplicationId).HasName("PK__VisaAppl__20C62915275677AB");

            entity.Property(e => e.VisaStatus).HasDefaultValue("Pending");

            entity.HasOne(d => d.BookingTraveler).WithMany(p => p.VisaApplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VisaAppli__Booki__3F115E1A");

            entity.HasOne(d => d.Country).WithMany(p => p.VisaApplications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VisaAppli__Count__40058253");

            entity.HasOne(d => d.Supplier).WithMany(p => p.VisaApplications).HasConstraintName("FK__VisaAppli__Suppl__40F9A68C");
        });

        modelBuilder.Entity<ContactInquiry>(entity =>
        {
            entity.HasKey(e => e.InquiryId);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
