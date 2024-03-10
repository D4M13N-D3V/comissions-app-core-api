﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using comissions.app.database;

#nullable disable

namespace comissions.app.api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("comissions.app.database.Entities.ArtistPageSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ArtistId")
                        .HasColumnType("integer");

                    b.Property<string>("BackgroundColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DescriptionBackgroundColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DescriptionHeaderColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DescriptionHeaderImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("DescriptionHeaderSize")
                        .HasColumnType("integer");

                    b.Property<string>("DescriptionHeaderText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("DescriptionHeaderUseImage")
                        .HasColumnType("boolean");

                    b.Property<string>("DescriptionTextColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("DescriptionTextSize")
                        .HasColumnType("integer");

                    b.Property<string>("HeaderColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HeaderImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("HeaderTextSize")
                        .HasColumnType("integer");

                    b.Property<bool>("HeaderUseImage")
                        .HasColumnType("boolean");

                    b.Property<string>("PortfolioBackgroundColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PortfolioColumns")
                        .HasColumnType("integer");

                    b.Property<bool>("PortfolioEnabledScrolling")
                        .HasColumnType("boolean");

                    b.Property<bool>("PortfolioMasonry")
                        .HasColumnType("boolean");

                    b.Property<int>("PortfolioMaximumSize")
                        .HasColumnType("integer");

                    b.Property<string>("PortfolionHeaderColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PortfolionHeaderImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PortfolionHeaderSize")
                        .HasColumnType("integer");

                    b.Property<string>("PortfolionHeaderText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("PortfolionHeaderUseImage")
                        .HasColumnType("boolean");

                    b.Property<string>("RequestBackgroundColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequestButtonBGColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequestButtonHoverBGColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequestButtonHoverTextColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequestButtonTextColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequestHeaderColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequestHeaderImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RequestHeaderSize")
                        .HasColumnType("integer");

                    b.Property<string>("RequestHeaderText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("RequestHeaderUseImage")
                        .HasColumnType("boolean");

                    b.Property<string>("RequestTermsColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId")
                        .IsUnique();

                    b.ToTable("ArtistPageSettings");
                });

            modelBuilder.Entity("comissions.app.database.Entities.ArtistPortfolioPiece", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ArtistId")
                        .HasColumnType("integer");

                    b.Property<string>("FileReference")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.ToTable("ArtistPortfolioPieces");
                });

            modelBuilder.Entity("comissions.app.database.Entities.ArtistRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Accepted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("AcceptedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ArtistRequests");
                });

            modelBuilder.Entity("comissions.app.database.Entities.ArtistRequestMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArtistRequestId")
                        .HasColumnType("integer");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("SentDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ArtistRequestId");

                    b.HasIndex("UserId");

                    b.ToTable("ArtistRequestMessages");
                });

            modelBuilder.Entity("comissions.app.database.Entities.Ban", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("BanDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UnbanDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Voided")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Bans");
                });

            modelBuilder.Entity("comissions.app.database.Entities.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Accepted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("AcceptedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<int>("ArtistId")
                        .HasColumnType("integer");

                    b.Property<bool>("Completed")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("CompletedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Declined")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("DeclinedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Paid")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("PaidDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PaymentUrl")
                        .HasColumnType("text");

                    b.Property<double?>("Rating")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ReviewDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ReviewMessage")
                        .HasColumnType("text");

                    b.Property<bool>("Reviewed")
                        .HasColumnType("boolean");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.HasIndex("UserId");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("comissions.app.database.Entities.RequestAsset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FileReference")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RequestId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("RequestAssets");
                });

            modelBuilder.Entity("comissions.app.database.Entities.RequestReference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FileReference")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RequestId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("RequestReferences");
                });

            modelBuilder.Entity("comissions.app.database.Entities.Suspension", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("SuspensionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UnsuspensionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Voided")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Suspensions");
                });

            modelBuilder.Entity("comissions.app.database.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("AdminNotes")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Biography")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("UserArtistId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("comissions.app.database.Entities.UserArtist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("AgeRestricted")
                        .HasColumnType("boolean");

                    b.Property<int>("ArtistPageSettingsId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("PrepaymentRequired")
                        .HasColumnType("boolean");

                    b.Property<string>("RequestGuidelines")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SocialMediaLink1")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SocialMediaLink2")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SocialMediaLink3")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SocialMediaLink4")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StripeAccountId")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserArtists");
                });

            modelBuilder.Entity("comissions.app.database.Entities.ArtistPageSettings", b =>
                {
                    b.HasOne("comissions.app.database.Entities.UserArtist", "Artist")
                        .WithOne("ArtistPageSettings")
                        .HasForeignKey("comissions.app.database.Entities.ArtistPageSettings", "ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");
                });

            modelBuilder.Entity("comissions.app.database.Entities.ArtistPortfolioPiece", b =>
                {
                    b.HasOne("comissions.app.database.Entities.UserArtist", "Artist")
                        .WithMany("PortfolioPieces")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");
                });

            modelBuilder.Entity("comissions.app.database.Entities.ArtistRequest", b =>
                {
                    b.HasOne("comissions.app.database.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("comissions.app.database.Entities.ArtistRequestMessage", b =>
                {
                    b.HasOne("comissions.app.database.Entities.ArtistRequest", null)
                        .WithMany("ArtistRequestMessages")
                        .HasForeignKey("ArtistRequestId");

                    b.HasOne("comissions.app.database.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("comissions.app.database.Entities.Ban", b =>
                {
                    b.HasOne("comissions.app.database.Entities.User", "User")
                        .WithMany("Bans")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("comissions.app.database.Entities.Request", b =>
                {
                    b.HasOne("comissions.app.database.Entities.UserArtist", "Artist")
                        .WithMany("Requests")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("comissions.app.database.Entities.User", "User")
                        .WithMany("Requests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("User");
                });

            modelBuilder.Entity("comissions.app.database.Entities.RequestAsset", b =>
                {
                    b.HasOne("comissions.app.database.Entities.Request", "Request")
                        .WithMany("RequestAssets")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Request");
                });

            modelBuilder.Entity("comissions.app.database.Entities.RequestReference", b =>
                {
                    b.HasOne("comissions.app.database.Entities.Request", "Request")
                        .WithMany("RequestReferences")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Request");
                });

            modelBuilder.Entity("comissions.app.database.Entities.Suspension", b =>
                {
                    b.HasOne("comissions.app.database.Entities.User", "User")
                        .WithMany("Suspensions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("comissions.app.database.Entities.UserArtist", b =>
                {
                    b.HasOne("comissions.app.database.Entities.User", "User")
                        .WithOne("UserArtist")
                        .HasForeignKey("comissions.app.database.Entities.UserArtist", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("comissions.app.database.Entities.ArtistRequest", b =>
                {
                    b.Navigation("ArtistRequestMessages");
                });

            modelBuilder.Entity("comissions.app.database.Entities.Request", b =>
                {
                    b.Navigation("RequestAssets");

                    b.Navigation("RequestReferences");
                });

            modelBuilder.Entity("comissions.app.database.Entities.User", b =>
                {
                    b.Navigation("Bans");

                    b.Navigation("Requests");

                    b.Navigation("Suspensions");

                    b.Navigation("UserArtist");
                });

            modelBuilder.Entity("comissions.app.database.Entities.UserArtist", b =>
                {
                    b.Navigation("ArtistPageSettings")
                        .IsRequired();

                    b.Navigation("PortfolioPieces");

                    b.Navigation("Requests");
                });
#pragma warning restore 612, 618
        }
    }
}
