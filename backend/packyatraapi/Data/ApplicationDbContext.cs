using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Models;

namespace MoversAndPackerApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Item> Item { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserAdmin> UserAdmin { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<TicketUpdate> TicketUpdate { get; set; }
        public DbSet<TicketAttachment> TicketAttachment { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<BookingItem> BookingItem { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<InventoryItem> MasterInventory { get; set; }
        public DbSet<DistanceCFT> DistanceCFTs { get; set; }
        public DbSet<CityDistanceCFT> CityDistanceCFTs { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<TimeSlot> TimeSlots { get;  set; }
        public DbSet<PhonePePayment> PhonePePayments { get; set; }
         
        public DbSet<PhonePeRefund> PhonePeRefunds { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserAdmin
            modelBuilder.Entity<UserAdmin>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.RoleId).HasMaxLength(50);
                entity.Property(u => u.Password).HasMaxLength(100);
                entity.Property(u => u.Email).HasMaxLength(100);
                entity.Property(u => u.Mobile).HasMaxLength(15);
                entity.Property(u => u.CreatedBy).HasMaxLength(100);

                entity.Property(u => u.FirstName).HasMaxLength(50);
                entity.Property(u => u.LastName).HasMaxLength(50);

            });

            // Configure Users (minimal)
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(u => u.UserID);
                entity.Property(u => u.Name).HasMaxLength(100);
                entity.Property(u => u.Email).HasMaxLength(100);
                entity.Property(u => u.PhoneNumber).HasMaxLength(15);
            });

            // Configure InventoryItem
            modelBuilder.Entity<InventoryItem>(entity =>
            {
                entity.HasKey(i => i.ItemID);
            });

            // Configure Payments
            modelBuilder.Entity<Payments>(entity =>
            {
                entity.HasKey(p => p.PaymentID);
            });

            // Configure Tickets
            modelBuilder.Entity<Tickets>(entity =>
            {
                entity.HasKey(t => t.TicketID);
            });

            // Configure Booking
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.BookingID);
            });

            // Configure BookingItem
            //modelBuilder.Entity<BookingItem>(entity =>
            //{
            //    entity.HasKey(bi => bi.BookingItemID);
            //    entity.HasOne(bi => bi.Booking)
            //          .WithMany()
            //          .HasForeignKey(bi => bi.BookingID);
            //    entity.HasOne(bi => bi.Item)
            //          .WithMany()
            //          .HasForeignKey(bi => bi.ItemID);
            //});

            // Configure other entities
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(i => i.ItemID);
            });

            modelBuilder.Entity<TicketUpdate>(entity =>
            {
                entity.HasKey(tu => tu.TicketID);
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.AddressID);
            });
            modelBuilder.Entity<TicketAttachment>(entity => { 
            entity.HasKey(s => s.AttachmentId);
        });
            modelBuilder.Entity<TicketComment>(entity =>
            {
                entity.HasKey(i => i.CommentId);
            });
            //modelBuilder.Entity<DistanceCFT>(entity =>
            //{
            //    entity.HasKey(d => d.DistanceCFTID);
            //});

            //modelBuilder.Entity<CityDistanceCFT>(entity =>
            //{
            //    entity.HasKey(cd => cd.CityDistanceCFTID);
            //});
        }
    }
}