import { HrService } from './../../_services/hr.service';
import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { TabsetComponent } from 'ngx-bootstrap';
import { ActivatedRoute, Router } from '@angular/router';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';

import { EvaluationFileInstance } from '../../_models/evaluationFileInstance';
import { Goal } from '../../_models/goal';
import { GoalType } from '../../_models/goalType';
import { UserService } from '../../_services/user.service';
import { AdminService } from '../../_services/admin.service';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { GoalByAxisInstance } from '../../_models/goalsByAxisInstance';
import { GoalEvaluation } from '../../_models/goalEvaluation';
import { BehavioralSkillInstance } from '../../_models/behavioralSkillInstance';
import { Project } from '../../_models/project';
import { Evaluator } from '../../_models/evaluator';

@Component({
  selector: 'app-sheet-detail',
  templateUrl: './sheet-detail.component.html',
  styleUrls: ['./sheet-detail.component.css']
})
export class SheetDetailComponent implements OnInit {
  @ViewChild('sheetTabs') sheetTabs: TabsetComponent;
  @Input() sheetToValidate: EvaluationFileInstance;
  @Input() tabIndex: number;
  @Output() switchOffDetailModeEvent = new EventEmitter();
  @Output() behavioralSkillEvaluationUpdatedEvent = new EventEmitter<boolean>();
  sheetDetail: EvaluationFileInstance;
  goalsByAxisInstanceList: GoalByAxisInstance[];
  behavioralSkillInstanceList: BehavioralSkillInstance[];
  goalTypeList: GoalType[];
  projectList: Project[];
  public loading = false;
  areGoalsCompleted: boolean;
  areGoalsReadOnly: boolean;
  areGoalsEvaluable: boolean;
  areBehavioralSkillsEvaluable: boolean;
  totalGrade: string;
  behavioralSkillEvaluationUpdated: boolean;
  showDetail: boolean;
  faArrowLeft = faArrowLeft;
  evaluators: Evaluator[] = [];

  constructor(private route: ActivatedRoute, private router: Router, private userService: UserService, private adminService: AdminService, private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    if (this.sheetToValidate) {
      this.sheetDetail = this.sheetToValidate;
      this.sheetTabs.tabs[this.tabIndex].active = true;
      this.fetchEvaluators();
      this.getGoalsForAxis();
      this.getBehavioralSkillEvaluations();
    } else {
      this.route.data.subscribe(data => {
        const resolvedData = data['resolvedData'];
        this.sheetDetail = resolvedData['sheetDetail'];
        this.goalTypeList = resolvedData['goalTypeList'];
        this.projectList = resolvedData['projectList'];
        this.fetchEvaluators();;
        this.getGoalsForAxis();
        this.getBehavioralSkillEvaluations();
      });
    }
  }

  getGoalsForAxis() {
    var axisInstanceIds = this.sheetDetail.axisInstances.map(a => a.id);
    this.loading = true;
    this.userService
      .getGoalsForAxis(this.sheetDetail.ownerId, axisInstanceIds)
      .subscribe(
        (result: GoalByAxisInstance[]) => {
          this.loading = false;
          this.goalsByAxisInstanceList = result;
          this.CanGoalsBeValidated();
          this.CheckReadOnly();
          this.CanGoalsBeEvaluated();
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  getBehavioralSkillEvaluations() {
    this.loading = true;
    this.userService
      .getBehavioralSkillEvaluations(this.sheetDetail.ownerId, this.sheetDetail.id)
      .subscribe(
        (result: BehavioralSkillInstance[]) => {
          this.loading = false;
          this.CanBehavioralSkillBeEvaluated();
          this.behavioralSkillInstanceList = result;
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  CheckReadOnly() {
    var goalsInInitialStatus = this.goalsByAxisInstanceList.filter(g => g.goalsStatus === 'Pas encore créé' || g.goalsStatus == 'Rédaction');
    if (goalsInInitialStatus.length == 0) {
      this.areGoalsReadOnly = true;
    } else {
      var indx = this.evaluators.findIndex(e => e.id == this.authService.decodedToken.nameid);
      if (this.sheetDetail.ownerId != this.authService.decodedToken.nameid && indx === -1) {
        this.areGoalsReadOnly = true;
      } else {
        this.areGoalsReadOnly = false;
      }
    }
    return this.areGoalsReadOnly;
  }

  handleCreateGoal(newGoal: any) {
    this.loading = true;
    this.userService.createGoal(this.authService.decodedToken.nameid, newGoal).subscribe(
      () => {
        this.loading = false;
        this.getGoalsForAxis();
        this.alertify.success('Objectif créé avec succès.');
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    );
  }

  handleEditGoal(goal: any) {
    this.loading = true;
    this.userService.updateGoal(goal.id, this.authService.decodedToken.nameid, goal).subscribe(() => {
      this.loading = false;
      this.alertify.success('Objectif a été mis à jour.');
      this.getGoalsForAxis();
    }, error => {
      this.loading = false;
      this.alertify.error(error);
    });
  }

  handleDeleteGoal(goal: Goal) {
    this.alertify.confirm('Supprimer',
      `Êtes-vous sûr de vouloir supprimer l'objectif: ${goal.description}?`,
      () => {
        this.loading = true;
        this.userService
          .deleteGoal(goal.id, this.authService.decodedToken.nameid)
          .subscribe(
            () => {
              this.loading = false;
              this.getGoalsForAxis();
              this.alertify.success('L\'objectif a été supprimée');
            },
            error => {
              this.loading = false;
              this.alertify.error(error);
            }
          );
      }
    );
  }

  CanGoalsBeValidated() {
    if (this.goalsByAxisInstanceList.filter(g => g.totalGoalWeight != 100).length == 0) {
      this.areGoalsCompleted = true;
    } else {
      this.areGoalsCompleted = false;
    }
    return this.areGoalsCompleted;
  }

  CanGoalsBeEvaluated() {
    if (this.goalsByAxisInstanceList.filter(g => g.goalsStatus === 'Publiée').length == 0) {
      this.areGoalsEvaluable = false;
    } else {
      this.areGoalsEvaluable = true;
    }
    return this.areGoalsEvaluable;
  }

  CanBehavioralSkillBeEvaluated() {
    var indx = this.evaluators.findIndex(e => e.id == this.authService.decodedToken.nameid);
    if (indx === -1) {
      this.areBehavioralSkillsEvaluable = false;
    } else {
      this.areBehavioralSkillsEvaluable = true;
    }
  }

  fetchEvaluators() {
    this.adminService.loadEvaluators(this.sheetDetail.ownerId).subscribe(result => {
      this.evaluators = result;
    },
      error => {
        console.log('error in CanBehavioralSkillBeEvaluated: ', error);
      }
    );
  }

  handleValidateGoals() {
    this.loading = true;

    var goals: any[] = [];
    this.goalsByAxisInstanceList.forEach(a => {
      a.goals.forEach(g => goals.push({
        id: g.id,
        description: this.sheetDetail.title,//For logs
        goalTypeId: g.goalType.id,
        axisInstanceId: g.axisInstance.id,
        weight: g.weight,
        status: 'En Revue',
        sheetTitle: this.sheetDetail.title,
        emailContent: `S'il vous plaît valider les objectives pour la fiche d'évaluation ${this.sheetDetail.title}.`,
        sheetOwnerId: this.sheetDetail.ownerId

      }));
    });

    this.userService
      .validateGoals(this.authService.decodedToken.nameid, goals)
      .subscribe(
        () => {
          this.loading = false;
          this.areGoalsReadOnly = true;
          this.getGoalsForAxis();
          this.alertify.success('Les objectives ont été envoyées pour validation');
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleAddGoalEvaluation(newEval: any) {
    this.loading = true;
    let goalEval = { ...newEval, evaluatorId: this.authService.decodedToken.nameid };
    this.userService
      .addGoalEvaluations(this.sheetDetail.ownerId, goalEval)
      .subscribe(
        () => {
          this.loading = false;
          this.getGoalsForAxis();
          this.alertify.success('L\'évaluation a été ajoutée avec succès.');
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleAddBehavioralSkillEvaluation(evals: any[]) {
    this.loading = true;
    this.userService
      .addBehavioralSkillEvaluations(this.sheetDetail.ownerId, evals)
      .subscribe(
        () => {
          this.loading = false;
          this.alertify.success('les évaluations de compétences comportementales ont été enregistrées avec succès.');
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleBehavioralSkillEvaluationUpdated(event: boolean) {
    this.behavioralSkillEvaluationUpdated = event;
    this.behavioralSkillEvaluationUpdatedEvent.emit(event);
  }

  returnToList() {
    if (this.sheetToValidate) {
      if (this.behavioralSkillEvaluationUpdated) {
        this.alertify.confirm('Confirmer',
          `Êtes-vous sûr de vouloir revenir à la liste avant d’enregistrer les modifications apportées aux compétences comportementales?`,
          () => {
            this.switchOffDetailModeEvent.emit();
            this.behavioralSkillEvaluationUpdated = false;
            this.behavioralSkillEvaluationUpdatedEvent.emit(false);
          }
        );
      } else {
        this.switchOffDetailModeEvent.emit();
      }
    } else {
      this.router.navigate(['/sheets']);
    }
  }

  handleCascadeMyGoal(golasForCascade: any) {
    this.loading = true;
    this.userService.casvadeGoal(this.authService.decodedToken.nameid, golasForCascade, this.sheetDetail.id).subscribe(
      () => {
        this.loading = false;
        this.alertify.success('Votre demande de création de sous-objectifs a été envoyée avec succès. Veuillez vérifier vos messages pour les résultats.');
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    );
  }
}
