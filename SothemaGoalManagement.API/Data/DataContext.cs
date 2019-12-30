using SothemaGoalManagement.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SothemaGoalManagement.API.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole,
                                                    IdentityUserLogin<int>, IdentityRoleClaim<int>,
                                                    IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Pole> Poles { get; set; }

        public DbSet<UserStatus> UserStatus { get; set; }

        public DbSet<Strategy> Strategies { get; set; }

        public DbSet<Axis> Axis { get; set; }

        public DbSet<AxisInstance> AxisInstances { get; set; }

        public DbSet<AxisPole> AxisPoles { get; set; }

        public DbSet<EvaluatedEvaluator> EvaluatedEvaluators { get; set; }

        public DbSet<BehavioralSkill> BehavioralSkills { get; set; }

        public DbSet<BehavioralSkillInstance> BehavioralSkillInstances { get; set; }

        public DbSet<EvaluationFile> EvaluationFiles { get; set; }

        public DbSet<EvaluationFileInstance> EvaluationFileInstances { get; set; }

        public DbSet<EvaluationFileInstanceLog> EvaluationFileInstanceLogs { get; set; }

        public DbSet<EvaluationFileBehavioralSkill> EvaluationFileBehavioralSkills { get; set; }


        public DbSet<EvaluationFileInstanceBehavioralSkillInstance> EvaluationFileInstanceBehavioralSkillInstances { get; set; }

        public DbSet<GoalType> GoalTypes { get; set; }
        public DbSet<Project> Projects { get; set; }

        public DbSet<Goal> Goals { get; set; }

        public DbSet<GoalEvaluation> GoalEvaluations { get; set; }
        public DbSet<BehavioralSkillEvaluation> BehavioralSkillEvaluations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();

                userRole.HasOne(ur => ur.User)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.UserId)
                        .IsRequired();
            });

            builder.Entity<EvaluatedEvaluator>(evaluatedEvaluator =>
            {
                evaluatedEvaluator.HasKey(ee => new { ee.EvaluatedId, ee.EvaluatorId });

                evaluatedEvaluator.HasOne(ee => ee.Evaluator)
                        .WithMany(u => u.EvaluatedEvaluators)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasForeignKey(ee => ee.EvaluatorId)
                        .IsRequired();
            });

            builder.Entity<AxisPole>(axisPole =>
            {
                axisPole.HasKey(ap => new { ap.AxisId, ap.PoleId });

                axisPole.HasOne(ap => ap.Pole)
                        .WithMany(p => p.AxisPoles)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasForeignKey(ap => ap.PoleId)
                        .IsRequired();

                axisPole.HasOne(ap => ap.Axis)
                        .WithMany(a => a.AxisPoles)
                        .HasForeignKey(ap => ap.AxisId)
                        .IsRequired();
            });

            builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Pole>().Property(p => p.Name).IsRequired();

            builder.Entity<Department>().Property(d => d.Name).IsRequired();

            builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);

            builder.Entity<UserStatus>().Property(d => d.Name).IsRequired();

            builder.Entity<Strategy>().HasOne(s => s.EvaluationFile).WithOne(ef => ef.Strategy)
                                                       .HasForeignKey<EvaluationFile>(ef => ef.StrategyId)
                                                       .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EvaluationFileBehavioralSkill>(evaluationFileBehavioralSkill =>
            {
                evaluationFileBehavioralSkill.HasKey(efbs => new { efbs.EvaluationFileId, efbs.BehavioralSkillId });

                evaluationFileBehavioralSkill.HasOne(bs => bs.BehavioralSkill)
                                            .WithMany(ef => ef.EvaluationFiles)
                                            .OnDelete(DeleteBehavior.Restrict)
                                            .HasForeignKey(bs => bs.BehavioralSkillId)
                                            .IsRequired();

                evaluationFileBehavioralSkill.HasOne(ef => ef.EvaluationFile)
                                            .WithMany(bs => bs.BehavioralSkills)
                                            .OnDelete(DeleteBehavior.Cascade)
                                            .HasForeignKey(ef => ef.EvaluationFileId)
                                            .IsRequired();
            });

            builder.Entity<EvaluationFileInstanceBehavioralSkillInstance>(evaluationFileInstanceBehavioralSkillInstance =>
            {
                evaluationFileInstanceBehavioralSkillInstance.HasKey(efibsi => new { efibsi.EvaluationFileInstanceId, efibsi.BehavioralSkillInstanceId });

                evaluationFileInstanceBehavioralSkillInstance.HasOne(bsi => bsi.BehavioralSkillInstance)
                                            .WithMany(efi => efi.EvaluationFileInstances)
                                            .OnDelete(DeleteBehavior.Cascade)
                                            .HasForeignKey(bsi => bsi.BehavioralSkillInstanceId)
                                            .IsRequired();

                evaluationFileInstanceBehavioralSkillInstance.HasOne(efi => efi.EvaluationFileInstance)
                                            .WithMany(bsi => bsi.BehavioralSkillInstances)
                                            .OnDelete(DeleteBehavior.Cascade)
                                            .HasForeignKey(efi => efi.EvaluationFileInstanceId)
                                            .IsRequired();
            });


            builder.Entity<EvaluationFileInstance>(efi =>
            {
                efi.HasOne<User>(u => u.Owner)
                    .WithMany(o => o.EvaluationFileInstances)
                    .HasForeignKey(u => u.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            builder.Entity<AxisInstance>(ai =>
            {
                ai.HasOne<EvaluationFileInstance>(u => u.EvaluationFileInstance)
                    .WithMany(o => o.AxisInstances)
                    .HasForeignKey(u => u.EvaluationFileInstanceId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            builder.Entity<GoalType>().Property(p => p.Name).IsRequired();
            builder.Entity<Project>().Property(p => p.Name).IsRequired();
            builder.Entity<Project>().Property(p => p.Closed).HasDefaultValue(false);

            builder.Entity<Goal>(g =>
            {
                g.HasOne<AxisInstance>(o => o.AxisInstance)
                    .WithMany(ai => ai.Goals)
                    .HasForeignKey(ai => ai.AxisInstanceId)
                    .OnDelete(DeleteBehavior.Restrict);

                g.HasOne<GoalType>(o => o.GoalType)
                    .WithMany(ai => ai.Goals)
                    .HasForeignKey(ai => ai.GoalTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<GoalEvaluation>(ge =>
            {
                ge.HasOne<Goal>(e => e.Goal)
                    .WithMany(g => g.GoalEvaluations)
                    .HasForeignKey(e => e.GoalId)
                    .OnDelete(DeleteBehavior.Restrict);

                ge.HasOne<User>(u => u.Evaluator)
                    .WithMany(o => o.GoalEvaluations)
                    .HasForeignKey(u => u.EvaluatorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<BehavioralSkillEvaluation>(ge =>
            {
                ge.HasOne<BehavioralSkillInstance>(e => e.BehavioralSkillInstance)
                    .WithMany(g => g.BehavioralSkillEvaluations)
                    .HasForeignKey(e => e.BehavioralSkillInstanceId)
                    .OnDelete(DeleteBehavior.Restrict);

                ge.HasOne<User>(u => u.Evaluator)
                    .WithMany(o => o.BehavioralSkillEvaluations)
                    .HasForeignKey(u => u.EvaluatorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}