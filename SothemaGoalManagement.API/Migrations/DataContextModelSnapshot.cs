﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SothemaGoalManagement.API.Data;

namespace SothemaGoalManagement.API.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Axis", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<bool>("Sealed");

                    b.Property<DateTime>("SealedDate");

                    b.Property<int>("StrategyId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("StrategyId");

                    b.ToTable("Axis");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.AxisInstance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<int>("EvaluationFileInstanceId");

                    b.Property<int>("PoleId");

                    b.Property<int>("PoleWeight");

                    b.Property<string>("Title");

                    b.Property<int>("UserWeight");

                    b.HasKey("Id");

                    b.HasIndex("EvaluationFileInstanceId");

                    b.HasIndex("PoleId");

                    b.ToTable("AxisInstances");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.AxisPole", b =>
                {
                    b.Property<int>("AxisId");

                    b.Property<int>("PoleId");

                    b.Property<bool>("Sealed");

                    b.Property<DateTime>("SealedDate");

                    b.Property<int>("Weight");

                    b.HasKey("AxisId", "PoleId");

                    b.HasIndex("PoleId");

                    b.ToTable("AxisPoles");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.BehavioralSkill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<int>("CreatedById");

                    b.Property<string>("Definition");

                    b.Property<string>("LevelFour");

                    b.Property<string>("LevelFourDescription");

                    b.Property<string>("LevelFourGrade");

                    b.Property<string>("LevelOne");

                    b.Property<string>("LevelOneDescription");

                    b.Property<string>("LevelOneGrade");

                    b.Property<string>("LevelThree");

                    b.Property<string>("LevelThreeDescription");

                    b.Property<string>("LevelThreeGrade");

                    b.Property<string>("LevelTwo");

                    b.Property<string>("LevelTwoDescription");

                    b.Property<string>("LevelTwoGrade");

                    b.Property<bool>("Sealed");

                    b.Property<DateTime>("SealedDate");

                    b.Property<string>("Skill");

                    b.Property<string>("Status");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.ToTable("BehavioralSkills");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.BehavioralSkillInstance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BehavioralSkillId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Definition");

                    b.Property<string>("LevelFour");

                    b.Property<string>("LevelFourDescription");

                    b.Property<string>("LevelFourGrade");

                    b.Property<string>("LevelOne");

                    b.Property<string>("LevelOneDescription");

                    b.Property<string>("LevelOneGrade");

                    b.Property<string>("LevelThree");

                    b.Property<string>("LevelThreeDescription");

                    b.Property<string>("LevelThreeGrade");

                    b.Property<string>("LevelTwo");

                    b.Property<string>("LevelTwoDescription");

                    b.Property<string>("LevelTwoGrade");

                    b.Property<string>("Skill");

                    b.Property<string>("Status");

                    b.HasKey("Id");

                    b.ToTable("BehavioralSkillInstances");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PoleId");

                    b.HasKey("Id");

                    b.HasIndex("PoleId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.EvaluatedEvaluator", b =>
                {
                    b.Property<int>("EvaluatedId");

                    b.Property<int>("EvaluatorId");

                    b.Property<int>("Rank");

                    b.HasKey("EvaluatedId", "EvaluatorId");

                    b.HasIndex("EvaluatorId");

                    b.ToTable("EvaluatedEvaluators");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.EvaluationFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<int>("OwnerId");

                    b.Property<bool>("Sealed");

                    b.Property<DateTime>("SealedDate");

                    b.Property<string>("Status");

                    b.Property<int>("StrategyId");

                    b.Property<string>("Title");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("StrategyId");

                    b.ToTable("EvaluationFiles");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.EvaluationFileBehavioralSkill", b =>
                {
                    b.Property<int>("EvaluationFileId");

                    b.Property<int>("BehavioralSkillId");

                    b.HasKey("EvaluationFileId", "BehavioralSkillId");

                    b.HasIndex("BehavioralSkillId");

                    b.ToTable("EvaluationFileBehavioralSkills");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.EvaluationFileInstance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<int>("EvaluationFileId");

                    b.Property<int>("OwnerId");

                    b.Property<string>("Status");

                    b.Property<string>("StrategyDescription");

                    b.Property<string>("StrategyTitle");

                    b.Property<string>("Title");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("EvaluationFileId");

                    b.HasIndex("OwnerId");

                    b.ToTable("EvaluationFileInstances");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.EvaluationFileInstanceBehavioralSkillInstance", b =>
                {
                    b.Property<int>("EvaluationFileInstanceId");

                    b.Property<int>("BehavioralSkillInstanceId");

                    b.HasKey("EvaluationFileInstanceId", "BehavioralSkillInstanceId");

                    b.HasIndex("BehavioralSkillInstanceId");

                    b.ToTable("EvaluationFileInstanceBehavioralSkillInstances");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime?>("DateRead");

                    b.Property<bool>("IsRead");

                    b.Property<DateTime>("MessageSent");

                    b.Property<bool>("RecipientDeleted");

                    b.Property<int>("RecipientId");

                    b.Property<bool>("SenderDeleted");

                    b.Property<int>("SenderId");

                    b.HasKey("Id");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateAdded");

                    b.Property<string>("Description");

                    b.Property<bool>("IsApproved");

                    b.Property<bool>("IsMain");

                    b.Property<string>("PublicId");

                    b.Property<string>("Url");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Pole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Poles");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Strategy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<string>("DocumentationPublicId");

                    b.Property<string>("DocumentationUrl");

                    b.Property<int>("OwnerId");

                    b.Property<bool>("Sealed");

                    b.Property<DateTime>("SealedDate");

                    b.Property<string>("Status");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Strategies");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("Created");

                    b.Property<int>("DepartmentId");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("EmployeeNumber");

                    b.Property<string>("FirstName");

                    b.Property<DateTime>("LastActive");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<DateTime>("RecruitmentDate");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Title");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<int>("UserStatusId");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.HasIndex("UserStatusId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.UserRole", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.UserStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("UserStatus");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Axis", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.Strategy", "Strategy")
                        .WithMany("AxisList")
                        .HasForeignKey("StrategyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.AxisInstance", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.EvaluationFileInstance", "EvaluationFileInstance")
                        .WithMany("AxisInstances")
                        .HasForeignKey("EvaluationFileInstanceId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SothemaGoalManagement.API.Models.Pole", "Pole")
                        .WithMany()
                        .HasForeignKey("PoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.AxisPole", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.Axis", "Axis")
                        .WithMany("AxisPoles")
                        .HasForeignKey("AxisId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SothemaGoalManagement.API.Models.Pole", "Pole")
                        .WithMany("AxisPoles")
                        .HasForeignKey("PoleId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.BehavioralSkill", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Department", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.Pole", "Pole")
                        .WithMany("Departments")
                        .HasForeignKey("PoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.EvaluatedEvaluator", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.User", "Evaluated")
                        .WithMany()
                        .HasForeignKey("EvaluatedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SothemaGoalManagement.API.Models.User", "Evaluator")
                        .WithMany("EvaluatedEvaluators")
                        .HasForeignKey("EvaluatorId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.EvaluationFile", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SothemaGoalManagement.API.Models.Strategy", "Strategy")
                        .WithMany("EvaluationFiles")
                        .HasForeignKey("StrategyId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.EvaluationFileBehavioralSkill", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.BehavioralSkill", "BehavioralSkill")
                        .WithMany("EvaluationFiles")
                        .HasForeignKey("BehavioralSkillId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SothemaGoalManagement.API.Models.EvaluationFile", "EvaluationFile")
                        .WithMany("BehavioralSkills")
                        .HasForeignKey("EvaluationFileId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.EvaluationFileInstance", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.EvaluationFile", "EvaluationFile")
                        .WithMany()
                        .HasForeignKey("EvaluationFileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SothemaGoalManagement.API.Models.User", "Owner")
                        .WithMany("EvaluationFileInstances")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.EvaluationFileInstanceBehavioralSkillInstance", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.BehavioralSkillInstance", "BehavioralSkillInstance")
                        .WithMany("EvaluationFileInstances")
                        .HasForeignKey("BehavioralSkillInstanceId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SothemaGoalManagement.API.Models.EvaluationFileInstance", "EvaluationFileInstance")
                        .WithMany("BehavioralSkillInstances")
                        .HasForeignKey("EvaluationFileInstanceId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Message", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.User", "Recipient")
                        .WithMany("MessagesReceived")
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SothemaGoalManagement.API.Models.User", "Sender")
                        .WithMany("MessagesSent")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Photo", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.User", "User")
                        .WithMany("Photos")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.Strategy", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.User", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.Department", "Department")
                        .WithMany("Users")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SothemaGoalManagement.API.Models.UserStatus", "UserStatus")
                        .WithMany("Users")
                        .HasForeignKey("UserStatusId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SothemaGoalManagement.API.Models.UserRole", b =>
                {
                    b.HasOne("SothemaGoalManagement.API.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SothemaGoalManagement.API.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
