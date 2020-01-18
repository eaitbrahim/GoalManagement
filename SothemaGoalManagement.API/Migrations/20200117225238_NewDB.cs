using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SothemaGoalManagement.API.Migrations
{
    public partial class NewDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BehavioralSkillInstances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BehavioralSkillId = table.Column<int>(nullable: false),
                    Skill = table.Column<string>(nullable: true),
                    Definition = table.Column<string>(nullable: true),
                    LevelOne = table.Column<string>(nullable: true),
                    LevelTwo = table.Column<string>(nullable: true),
                    LevelThree = table.Column<string>(nullable: true),
                    LevelFour = table.Column<string>(nullable: true),
                    LevelOneGrade = table.Column<string>(nullable: true),
                    LevelTwoGrade = table.Column<string>(nullable: true),
                    LevelThreeGrade = table.Column<string>(nullable: true),
                    LevelFourGrade = table.Column<string>(nullable: true),
                    LevelOneDescription = table.Column<string>(nullable: true),
                    LevelTwoDescription = table.Column<string>(nullable: true),
                    LevelThreeDescription = table.Column<string>(nullable: true),
                    LevelFourDescription = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehavioralSkillInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationFileInstanceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Log = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationFileInstanceLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoalTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Poles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    GoalTypeId = table.Column<int>(nullable: false),
                    Closed = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_GoalTypes_GoalTypeId",
                        column: x => x.GoalTypeId,
                        principalTable: "GoalTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    PoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Poles_PoleId",
                        column: x => x.PoleId,
                        principalTable: "Poles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    EmployeeNumber = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    RecruitmentDate = table.Column<DateTime>(nullable: false),
                    UserStatusId = table.Column<int>(nullable: false),
                    DepartmentId = table.Column<int>(nullable: false),
                    LastActive = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_UserStatus_UserStatusId",
                        column: x => x.UserStatusId,
                        principalTable: "UserStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BehavioralSkillEvaluations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created = table.Column<DateTime>(nullable: false),
                    EvaluatorId = table.Column<int>(nullable: false),
                    EvaluateeId = table.Column<int>(nullable: false),
                    Grade = table.Column<int>(nullable: false),
                    Level = table.Column<string>(nullable: true),
                    Sealed = table.Column<bool>(nullable: false),
                    BehavioralSkillInstanceId = table.Column<int>(nullable: false),
                    EvaluationFileInstanceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehavioralSkillEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BehavioralSkillEvaluations_BehavioralSkillInstances_BehavioralSkillInstanceId",
                        column: x => x.BehavioralSkillInstanceId,
                        principalTable: "BehavioralSkillInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BehavioralSkillEvaluations_AspNetUsers_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BehavioralSkills",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Skill = table.Column<string>(nullable: true),
                    Definition = table.Column<string>(nullable: true),
                    LevelOne = table.Column<string>(nullable: true),
                    LevelTwo = table.Column<string>(nullable: true),
                    LevelThree = table.Column<string>(nullable: true),
                    LevelFour = table.Column<string>(nullable: true),
                    LevelOneGrade = table.Column<string>(nullable: true),
                    LevelTwoGrade = table.Column<string>(nullable: true),
                    LevelThreeGrade = table.Column<string>(nullable: true),
                    LevelFourGrade = table.Column<string>(nullable: true),
                    LevelOneDescription = table.Column<string>(nullable: true),
                    LevelTwoDescription = table.Column<string>(nullable: true),
                    LevelThreeDescription = table.Column<string>(nullable: true),
                    LevelFourDescription = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    Sealed = table.Column<bool>(nullable: false),
                    SealedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehavioralSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BehavioralSkills_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluatedEvaluators",
                columns: table => new
                {
                    EvaluatedId = table.Column<int>(nullable: false),
                    EvaluatorId = table.Column<int>(nullable: false),
                    Rank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluatedEvaluators", x => new { x.EvaluatedId, x.EvaluatorId });
                    table.ForeignKey(
                        name: "FK_EvaluatedEvaluators_AspNetUsers_EvaluatedId",
                        column: x => x.EvaluatedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvaluatedEvaluators_AspNetUsers_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SenderId = table.Column<int>(nullable: false),
                    RecipientId = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    IsRead = table.Column<bool>(nullable: false),
                    DateRead = table.Column<DateTime>(nullable: true),
                    MessageSent = table.Column<DateTime>(nullable: false),
                    SenderDeleted = table.Column<bool>(nullable: false),
                    RecipientDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    IsMain = table.Column<bool>(nullable: false),
                    PublicId = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Strategies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    OwnerId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    DocumentationUrl = table.Column<string>(nullable: true),
                    DocumentationPublicId = table.Column<string>(nullable: true),
                    Sealed = table.Column<bool>(nullable: false),
                    SealedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strategies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Strategies_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Axis",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    StrategyId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Sealed = table.Column<bool>(nullable: false),
                    SealedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Axis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Axis_Strategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "Strategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    StrategyId = table.Column<int>(nullable: false),
                    Sealed = table.Column<bool>(nullable: false),
                    SealedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluationFiles_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvaluationFiles_Strategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "Strategies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AxisPoles",
                columns: table => new
                {
                    AxisId = table.Column<int>(nullable: false),
                    PoleId = table.Column<int>(nullable: false),
                    Weight = table.Column<int>(nullable: false),
                    Sealed = table.Column<bool>(nullable: false),
                    SealedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AxisPoles", x => new { x.AxisId, x.PoleId });
                    table.ForeignKey(
                        name: "FK_AxisPoles_Axis_AxisId",
                        column: x => x.AxisId,
                        principalTable: "Axis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AxisPoles_Poles_PoleId",
                        column: x => x.PoleId,
                        principalTable: "Poles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationFileBehavioralSkills",
                columns: table => new
                {
                    EvaluationFileId = table.Column<int>(nullable: false),
                    BehavioralSkillId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationFileBehavioralSkills", x => new { x.EvaluationFileId, x.BehavioralSkillId });
                    table.ForeignKey(
                        name: "FK_EvaluationFileBehavioralSkills_BehavioralSkills_BehavioralSkillId",
                        column: x => x.BehavioralSkillId,
                        principalTable: "BehavioralSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvaluationFileBehavioralSkills_EvaluationFiles_EvaluationFileId",
                        column: x => x.EvaluationFileId,
                        principalTable: "EvaluationFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationFileInstances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    StrategyTitle = table.Column<string>(nullable: true),
                    StrategyDescription = table.Column<string>(nullable: true),
                    EvaluationFileId = table.Column<int>(nullable: false),
                    OwnerComment = table.Column<string>(nullable: true),
                    ValidatorComment = table.Column<string>(nullable: true),
                    OwnerValidationDateTime = table.Column<DateTime>(nullable: false),
                    ValidatorValidationDateTime = table.Column<DateTime>(nullable: false),
                    ValidatorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationFileInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluationFileInstances_EvaluationFiles_EvaluationFileId",
                        column: x => x.EvaluationFileId,
                        principalTable: "EvaluationFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvaluationFileInstances_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Event = table.Column<string>(nullable: true),
                    StartEvent = table.Column<DateTime>(nullable: false),
                    EndEvent = table.Column<DateTime>(nullable: false),
                    EvaluationFileId = table.Column<int>(nullable: false),
                    ToggleChangeAxisWeight = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameters_EvaluationFiles_EvaluationFileId",
                        column: x => x.EvaluationFileId,
                        principalTable: "EvaluationFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AxisInstances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EvaluationFileInstanceId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    PoleName = table.Column<string>(nullable: true),
                    PoleWeight = table.Column<int>(nullable: false),
                    UserWeight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AxisInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AxisInstances_EvaluationFileInstances_EvaluationFileInstanceId",
                        column: x => x.EvaluationFileInstanceId,
                        principalTable: "EvaluationFileInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationFileInstanceBehavioralSkillInstances",
                columns: table => new
                {
                    EvaluationFileInstanceId = table.Column<int>(nullable: false),
                    BehavioralSkillInstanceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationFileInstanceBehavioralSkillInstances", x => new { x.EvaluationFileInstanceId, x.BehavioralSkillInstanceId });
                    table.ForeignKey(
                        name: "FK_EvaluationFileInstanceBehavioralSkillInstances_BehavioralSkillInstances_BehavioralSkillInstanceId",
                        column: x => x.BehavioralSkillInstanceId,
                        principalTable: "BehavioralSkillInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvaluationFileInstanceBehavioralSkillInstances_EvaluationFileInstances_EvaluationFileInstanceId",
                        column: x => x.EvaluationFileInstanceId,
                        principalTable: "EvaluationFileInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentGoalId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Weight = table.Column<int>(nullable: false),
                    GoalTypeId = table.Column<int>(nullable: false),
                    ProjectName = table.Column<string>(nullable: true),
                    AxisInstanceId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_AxisInstances_AxisInstanceId",
                        column: x => x.AxisInstanceId,
                        principalTable: "AxisInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Goals_GoalTypes_GoalTypeId",
                        column: x => x.GoalTypeId,
                        principalTable: "GoalTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoalEvaluations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created = table.Column<DateTime>(nullable: false),
                    EvaluatorId = table.Column<int>(nullable: false),
                    EvaluateeId = table.Column<int>(nullable: false),
                    CompletionRate = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    Sealed = table.Column<bool>(nullable: false),
                    GoalId = table.Column<int>(nullable: false),
                    SelfEvaluation = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoalEvaluations_AspNetUsers_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoalEvaluations_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserStatusId",
                table: "AspNetUsers",
                column: "UserStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Axis_StrategyId",
                table: "Axis",
                column: "StrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_AxisInstances_EvaluationFileInstanceId",
                table: "AxisInstances",
                column: "EvaluationFileInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_AxisPoles_PoleId",
                table: "AxisPoles",
                column: "PoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BehavioralSkillEvaluations_BehavioralSkillInstanceId",
                table: "BehavioralSkillEvaluations",
                column: "BehavioralSkillInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_BehavioralSkillEvaluations_EvaluatorId",
                table: "BehavioralSkillEvaluations",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BehavioralSkills_CreatedById",
                table: "BehavioralSkills",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_PoleId",
                table: "Departments",
                column: "PoleId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluatedEvaluators_EvaluatorId",
                table: "EvaluatedEvaluators",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationFileBehavioralSkills_BehavioralSkillId",
                table: "EvaluationFileBehavioralSkills",
                column: "BehavioralSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationFileInstanceBehavioralSkillInstances_BehavioralSkillInstanceId",
                table: "EvaluationFileInstanceBehavioralSkillInstances",
                column: "BehavioralSkillInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationFileInstances_EvaluationFileId",
                table: "EvaluationFileInstances",
                column: "EvaluationFileId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationFileInstances_OwnerId",
                table: "EvaluationFileInstances",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationFiles_OwnerId",
                table: "EvaluationFiles",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationFiles_StrategyId",
                table: "EvaluationFiles",
                column: "StrategyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoalEvaluations_EvaluatorId",
                table: "GoalEvaluations",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalEvaluations_GoalId",
                table: "GoalEvaluations",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_AxisInstanceId",
                table: "Goals",
                column: "AxisInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_GoalTypeId",
                table: "Goals",
                column: "GoalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_EvaluationFileId",
                table: "Parameters",
                column: "EvaluationFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId",
                table: "Photos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_GoalTypeId",
                table: "Projects",
                column: "GoalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Strategies_OwnerId",
                table: "Strategies",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AxisPoles");

            migrationBuilder.DropTable(
                name: "BehavioralSkillEvaluations");

            migrationBuilder.DropTable(
                name: "EvaluatedEvaluators");

            migrationBuilder.DropTable(
                name: "EvaluationFileBehavioralSkills");

            migrationBuilder.DropTable(
                name: "EvaluationFileInstanceBehavioralSkillInstances");

            migrationBuilder.DropTable(
                name: "EvaluationFileInstanceLogs");

            migrationBuilder.DropTable(
                name: "GoalEvaluations");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Axis");

            migrationBuilder.DropTable(
                name: "BehavioralSkills");

            migrationBuilder.DropTable(
                name: "BehavioralSkillInstances");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "AxisInstances");

            migrationBuilder.DropTable(
                name: "GoalTypes");

            migrationBuilder.DropTable(
                name: "EvaluationFileInstances");

            migrationBuilder.DropTable(
                name: "EvaluationFiles");

            migrationBuilder.DropTable(
                name: "Strategies");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "UserStatus");

            migrationBuilder.DropTable(
                name: "Poles");
        }
    }
}
